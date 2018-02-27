
#ifndef SERVER_DEF
#include "server.h"
#define SERVER_DEF
#endif

Server::Server()
	{
		poll_events = new pollfd;
	}


int32_t Server::initializeSocket(short port)
{
	int optFlag = 1;
	if ((udpSocket = socket(AF_INET, SOCK_DGRAM, 0)) == -1)
	{
		perror("failed to initialize socket");
		return -1;
	}

	if (setsockopt(udpSocket, SOL_SOCKET, SO_REUSEADDR, &optFlag, sizeof(int)) == -1)
	{
		perror("set opts failed");
		return -1;
	}

	memset(&serverAddr, 0, sizeof(struct sockaddr_in));
	serverAddr.sin_family = AF_INET;
	serverAddr.sin_port = htons(port);
	serverAddr.sin_addr.s_addr = htonl(INADDR_ANY);

	int error = -1;

	if ((error = bind(udpSocket, (struct sockaddr *) &serverAddr, sizeof(serverAddr)) == -1))

	{
		perror("bind error: ");
		return error;
	}

	return 0;
}

int32_t Server::sendBytes(EndPoint ep, char * data, unsigned len)
{	
	struct sockaddr_in temp;
			
	memset(&temp, 0, sizeof(sockaddr_in));
	temp.sin_family = AF_INET;
	temp.sin_addr.s_addr = htonl(ep.addr);
	temp.sin_port = htons(ep.port);
			
	int32_t result = sendto(udpSocket, data, len, 0, (struct sockaddr*)&temp, sizeof(sockaddr_in));
	return result;

}

sockaddr_in Server::getServerAddr() {
	return serverAddr;
}



int32_t Server::UdpRecvFrom(char * buffer, uint32_t size, EndPoint * addr)
{
	sockaddr_in clientAddr;
	socklen_t addrSize = sizeof(clientAddr);
	memset(&clientAddr, 0, addrSize);
	
	int32_t result = recvfrom(udpSocket, buffer, size, 0, (struct sockaddr *)&clientAddr, &addrSize);

	addr->port = ntohs(clientAddr.sin_port);
	addr->addr = ntohl(clientAddr.sin_addr.s_addr);
	
	return result;
}

void  Server::setEndPointIp(EndPoint * ep, char zero, char one, char two, char three)
{
	char * tmp = (char*)&(ep->addr);
	
	tmp[0] = zero;
	tmp[1] = one;
	tmp[2] = two;
	tmp[3] = three;
}

int32_t Server::UdpPollSocket()
{
	int numfds = 1;
	struct pollfd pollfds;
	pollfds.fd = udpSocket;

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





