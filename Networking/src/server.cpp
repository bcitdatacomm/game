#include "server.h"

/*------------------------------------------------------------------------------------------------------------------
--FUNCTION: Server
--
--DATE : February 25, 2018
--
--REVISIONS : (Date and Description)
--
--DESIGNER :
--
--PROGRAMMER : 
--
--INTERFACE : Server::Server()
--
--RETURNS : Server - new Server object
--
--NOTES :
--This function is called to construct a new Server object
----------------------------------------------------------------------------------------------------------------------*/
Server::Server()
	{
		clientMap = new std::map<int, Connection>();
		poll_events = new pollfd;
	}


/*------------------------------------------------------------------------------------------------------------------
--FUNCTION: initializeSocket
--
--DATE : February 25, 2018
--
--REVISIONS : (Date and Description)
--
--DESIGNER :
--
--PROGRAMMER : 
--
--INTERFACE : int Server::initializeSocket(short port)
--				short port: The port to set the socket to
--
--RETURNS : int
--				-1 on failure, 0 on success
--
--NOTES :
--This function is called to initialize a new socket for the server to listen to
----------------------------------------------------------------------------------------------------------------------*/
int Server::initializeSocket(short port)
{
	int optFlag = 1;
	if ((udpRecvSocket = socket(AF_INET, SOCK_DGRAM, 0)) == -1)
	{
		perror("failed to initialize socket");
		return -1;
	}

	if (setsockopt(udpRecvSocket, SOL_SOCKET, SO_REUSEADDR, &optFlag, sizeof(int)) == -1)
	{
		perror("set opts failed");
		return -1;
	}

	memset(&serverAddr, 0, sizeof(struct sockaddr_in));
	serverAddr.sin_family = AF_INET;
	serverAddr.sin_port = htons(port);

	int error = -1;

	if ((error = bind(udpRecvSocket, (struct sockaddr *) &serverAddr, sizeof(serverAddr)) == -1))
	{
		perror("bind error: ");
		return error;
	}

	return 0;

}



/*------------------------------------------------------------------------------------------------------------------
--FUNCTION: sendBytes
--
--DATE : February 25, 2018
--
--REVISIONS : (Date and Description)
--
--DESIGNER :
--
--PROGRAMMER : 
--
--INTERFACE : void Server::sendBytes(int clientId, char * data, unsigned len)
--				int clientId: The id for the client to send data to
--				char * data: The buffer of data to send to client
--				unsigned len: The size of the buffer to send
--
--RETURNS : void
--
--NOTES :
--This function is called to send data from the server to a specified client
----------------------------------------------------------------------------------------------------------------------*/
void Server::sendBytes(int clientId, char * data, unsigned len)
{
	std::map<int, Connection>::iterator it = clientMap->find(clientId);
	//const char* buff = "HELLO";
	if (it != clientMap->end())
	{
			socklen_t sockLen = sizeof(it->second.getSocketID());
			struct sockaddr_in temp = it->second.getClient();
			sendto(it->second.getSocketID(), data, len, 0, (struct sockaddr*)&temp, sockLen);
	}
}

/////////////////////////////////////////// do we need these? 

sockaddr_in Server::getServerAddr() {
	return serverAddr;
}

void Server::initializeConnectionPool()
{

}

/*------------------------------------------------------------------------------------------------------------------
--FUNCTION: removeConnection
--
--DATE : February 25, 2018
--
--REVISIONS : (Date and Description)
--
--DESIGNER :
--
--PROGRAMMER : 
--
--INTERFACE : bool Server::removeConnection(int socketID)
--				int socketID: The id of the client socket to close
--
--RETURNS : bool
--				true if successful, false if unsuccessful
--
--NOTES :
--This function is called to send data from the server to a specified client
----------------------------------------------------------------------------------------------------------------------*/
bool Server::removeConnection(int socketID)
{
	if (clientMap->erase(socketID) == 1) {
		return true;
	}
	return false;
}


/*------------------------------------------------------------------------------------------------------------------
--FUNCTION: UdpRecvFrom
--
--DATE : February 25, 2018
--
--REVISIONS : (Date and Description)
--
--DESIGNER :
--
--PROGRAMMER : 
--
--INTERFACE : int32_t Server::UdpRecvFrom(char * buffer, uint32_t size, EndPoint * addr)
--				char * buffer: The buffer to fill with read in data
--				uint32_t size: The size of the buffer that can be filled with data
--				EndPoint * addr: Struct to hold connection info
--
--RETURNS : int32_t
--				The number of bytes read from the socket
--
--NOTES :
--This function is called to read data from the server socket
----------------------------------------------------------------------------------------------------------------------*/
int32_t Server::UdpRecvFrom(char * buffer, uint32_t size, EndPoint * addr)
{
	sockaddr_in clientAddr;
	socklen_t addrSize = sizeof(clientAddr);

	int32_t result = recvfrom(udpRecvSocket, buffer, size, 0, (struct sockaddr *)&clientAddr, &addrSize);

	addr->port = ntohs(clientAddr.sin_port);
	addr->addr = ntohl(clientAddr.sin_addr.s_addr);
	addr->ip.s_addr = clientAddr.sin_addr.s_addr;
	addr->uport = clientAddr.sin_port;
	return result;
}



/*------------------------------------------------------------------------------------------------------------------
--FUNCTION: UdpPollSocket
--
--DATE : February 25, 2018
--
--REVISIONS : (Date and Description)
--
--DESIGNER :
--
--PROGRAMMER : 
--
--INTERFACE : int32_t Server::UdpPollSocket()
--
--RETURNS : int32_t
--				SOCKET_DATA_WAITING if there is data ready to be read
--				SOCKET_NODATA if there is no data ready to be read
--
--NOTES :
--This function is called to check if there is data in the socket buffer ready to read
----------------------------------------------------------------------------------------------------------------------*/
int32_t Server::UdpPollSocket()
{
	int numfds = 1;
	struct pollfd pollfds;
	pollfds.fd = udpRecvSocket;
	pollfds.events = POLLIN;

	int retVal = poll(&pollfds, numfds, 0);
 	if (retVal == -1) {
		 perror("poll failed with error: ");
	 }

	if(pollfds.revents & POLLIN)
	{
		return SOCKET_DATA_WAITING;
	}

	return SOCKET_NODATA;
}



////////////////////////////////////////////

extern "C" Server * Server_CreateServer()
{
	return new Server();
}

extern "C" void Server_sendBytes(void * serverPtr, int clientId, char * data, uint32_t len)
{
	((Server *)serverPtr)->sendBytes(clientId, data, len);
}

extern "C" int32_t Server_recvBytes(void * serverPtr, EndPoint addr, char * buffer, uint32_t * bufSize)
{

	int32_t result = static_cast<Server*>(serverPtr)->UdpRecvFrom(buffer, *bufSize, &addr);
	return result;
}

int main() {
	Server* serv = new Server();
	serv->initializeSocket(5150);
	int retVal;
	int sockCount = 100;
	while (true) {
		if ((retVal = serv->UdpPollSocket()) == SOCKET_DATA_WAITING) {
			std::cout << "received data" << std::endl;
			EndPoint ep;
			char buff[512];
			serv->UdpRecvFrom(buff, 512, &ep);
			std::cout << buff << std::endl;
			struct sockaddr_in temp;
			temp.sin_family = AF_INET;
			temp.sin_port = ep.uport;
			temp.sin_addr = ep.ip;
			Connection *c = new Connection(temp, serv->getServerAddr(), sockCount++);
			std::pair<int, Connection> temp2(c->getSocketID(), *c);
			serv->clientMap->insert(temp2);
			serv->removeConnection(102);
			for (auto it = serv->clientMap->begin(); it != serv->clientMap->end(); ++it) {
				std::cout << it->first << std::endl;
			}
		}
	}
	return 1;
}
