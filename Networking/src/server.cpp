#include "server.h"



	

Server::Server()
	{
		clientMap = new std::map<int, Connection>();
		poll_events = new pollfd;
	}


int Server::initializeSocket(short port)
{
	if (udpRecvSocket = socket(AF_INET, SOCK_DGRAM | SOCK_NONBLOCK, 0) == -1)
	{
		perror("failed to initialize socket");
		return -1;
	}

	int optFlag = 1;
	if(setsockopt(udpRecvSocket, SOL_SOCKET, SO_REUSEADDR, &flag, sizeof(flag) == -1)
	{
		perror("set opts failed");
		return -1;
	}

	memset(&serverAddr, 0, sizeof(struct sockaddr_in));
	serverAddr.sin_family = AF_INET;
	serverAddr.sin_port = htons(port);

	int error = -1;

	if((error = bind(udpRecvSocket, (struct  sockaddr *) &serverAddr, sizeof(serverAddr)) == -1)
	{
		std::cout << "bind error: " errno << std::endl;
		return error;
	}
	


	return 0;



}

void Server::sendBytes(int clientId, char * data, unsigned len)
{
	std::map<int, Connection>::iterator it = clientMap->find(clientId);
	 
	if (it != clientMap->end())
	{
			
	}
}


void initializeConnectionPool()
{
	
}

int32_t UdpRecvFrom(char * buffer, uint32_t size, EndPoint * addr)
{
	sockaddr_in clientAddr;
	size_t addrSize = sizeof(clientAddr);
	
	int32_t result = recvfrom(udpRecvSocket, buffer, size, 0, (sockaddr *)clientAddr, &addrSize);

	
	addr->port = ntohs(clientAddr.sin_port);
	addr->address = ntohl(clientAddr.sin_addr.s_addr);

	return result;
}

int32_t  UdpPollSocket()
{	
	int numfds = 1;
	struct pollfd pollfds;
	pollfds.fd = udpRecvSocket;
	pollfds.events = POLLIN;

	int retVal = poll(&pollfds, numfds, 0);
 	
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
	int32_t result = (Server *)serverPtr)->UdpRecvFrom(buffer, bufSize, addr);
	return result;
}
