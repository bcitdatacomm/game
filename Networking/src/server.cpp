#include "server.h"

Server::Server()
	{
		clientMap = new std::map<int, Connection>();
		poll_events = new pollfd;
	}


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

void Server::sendBytes(int clientId, char * data, unsigned len)
{
	std::map<int, Connection>::iterator it = clientMap->find(clientId);
	const char* buff = "HELLO";
	if (it != clientMap->end())
	{
			socklen_t len = sizeof(it->second.getSocketID());
			struct sockaddr_in temp = it->second.getClient();
			sendto(it->second.getSocketID(), buff, 16, 0, (struct sockaddr*)&temp, len);
	}
}

sockaddr_in Server::getServerAddr() {
	return serverAddr;
}

void Server::initializeConnectionPool()
{

}

bool Server::removeConnection(int socketID)
{
	if (clientMap->erase(socketID) == 1) {
		return true;
	}
	return false;
}

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

int32_t  Server::UdpPollSocket()
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
