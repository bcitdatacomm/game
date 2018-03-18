#include "tcpclient.h"
#include <cerrno>



TCPClient::TCPClient()
{

}

/**
	Initializes client TCP socket to receive initial game data.
	@author Calvin Lai
**/
int TCPClient::initializeSocket(EndPoint ep)
{
	if ((clientSocket = socket(AF_INET, SOCK_STREAM  , 0)) == -1) {
		perror("failed to initialize socket");
		return -1;
	}

	int optFlag = 1;

	if(setsockopt(clientSocket, SOL_SOCKET, SO_REUSEADDR, &optFlag, sizeof(optFlag)) == -1)
	{
		perror("set opts failed");
		return -1;
	}

	memset(&serverAddr, 0, sizeof(struct sockaddr_in));
	serverAddr.sin_family = AF_INET;
	serverAddr.sin_port = htons(ep.port);
	serverAddr.sin_addr.s_addr = htonl(ep.addr);


	if (connect(clientSocket, (struct sockaddr *)&serverAddr, sizeof(serverAddr)) < 0)
	{
		printf("\n Error : Connect Failed \n");
		perror("failure");
		return -1;
	}
	return clientSocket;

}



int32_t TCPClient::closeConnection(int32_t sockfd)
{
	return close(sockfd);
}

/**
	Sends char array to all connected clients
**/
int32_t TCPClient::sendBytes(char * data, uint32_t len)
{
	int32_t result;
	if ((result = send(clientSocket, data, len , 0 )) == -1) {
		perror("client send error");
	}

	return result;
}



/**
	Receives upto "size" bytes char array
**/
int32_t TCPClient::receiveBytes(char * buffer, uint32_t len)
{

	size_t n = 0;
	size_t bytesToRead = len;
	while ((n = recv (clientSocket, buffer, bytesToRead, 0)) < len)
	{
		buffer += n;
		bytesToRead -= n;
	}
	return (bytesToRead);
}
