#include "tcpserver.h"
#include <cerrno>

#define BUFLEN					1200		//Buffer length
#define MAX_NUM_CLIENTS 		30
#define TRUE					1
#define FALSE 					0


TCPServer::TCPServer()
{

}


int32_t TCPServer::initializeSocket	(short port)
{
	struct	sockaddr_in server;
	if ((tcpSocket = socket(AF_INET, SOCK_STREAM, 0)) == -1)
	{
		perror ("Can't create a socket");
		//std::cout << strerror(errno) << std::endl;
		return -5;
	}
	int optFlag = 1;

	// Sets server receive timeout to 30 seconds.
	struct timeval tv;
	tv.tv_sec = 30;
	tv.tv_usec = 0;

	if (setsockopt(tcpSocket, SOL_SOCKET, SO_RCVTIMEO, &tv, sizeof(struct timeval)) == -1)
	{
		perror("Failed to setsockopt: timeout");
	}

	if (setsockopt(tcpSocket, SOL_SOCKET, SO_REUSEADDR, &optFlag, sizeof(optFlag)) == -1)
	{
		perror("Failed to setsockopt: reuseaddr");
		return -4;
	}
	if (setsockopt(tcpSocket, SOL_SOCKET, SO_REUSEPORT, &optFlag, sizeof(optFlag)) == -1)
	{
		perror("Failed to setsockopt: reuseport");
		return -8;
	}

	// Zero memory of server sockaddr_in struct
	memset(&server, 0, sizeof(struct sockaddr_in));

	server.sin_family = AF_INET;
	server.sin_port = htons(port);
	server.sin_addr.s_addr = htonl(INADDR_ANY); // Accept connections from any client

	if (bind(tcpSocket, (struct sockaddr *)&server, sizeof(server)) == -1)
	{
		perror("Can't bind name to socket");
		perror("failed bind.");
		return errno;
	}
	if (listen(tcpSocket, MAX_NUM_CLIENTS) == -1)
	{
		return errno;
	}
	return tcpSocket;
}


int32_t TCPServer::acceptConnection(EndPoint* ep)
{
	int clientSocket;
	sockaddr_in clientAddr;
	socklen_t addrSize = sizeof(clientAddr);
	memset(&clientAddr, 0, addrSize);
	errno = 0;

	if ((clientSocket = accept(tcpSocket, (struct sockaddr *)&clientAddr, &addrSize)) == -1)
	{
		// Accept call times out
		if (errno == EAGAIN)
		{
			return -errno;
		}
		return 0;
	}

	ep->port = ntohs(clientAddr.sin_port);
	ep->addr = ntohl(clientAddr.sin_addr.s_addr);

	return clientSocket;
}


int32_t TCPServer::sendBytes(int clientSocket, char * data, unsigned len)
{
	return send(clientSocket, data, len, 0);
}

int32_t TCPServer::receiveBytes(int clientSocket, char * buffer, unsigned len)
{
	size_t n = 0;
	size_t bytesToRead = len;
	while ((n = recv (clientSocket, buffer, bytesToRead, 0)) < len)
	{
		buffer += n;
		bytesToRead -= n;
	}
	return (len - bytesToRead);
}

int32_t TCPServer::closeClientSocket(int32_t clientSocket)
{
	return close (clientSocket);
}

int32_t TCPServer::closeListenSocket(int32_t sockfd)
{
	int32_t result = close(sockfd);
	return result;
}
