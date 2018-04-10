/**
 * SOURCE FILE:	tcpclient.cpp
 * 
 * PROGRAM:		game
 * 
 * FUNCTIONS:	TCPClient();
 *				int32_t initializeSocket(EndPoint ep);
 *				int32_t sendBytes(char * data, uint32_t len);
 *				int32_t receiveBytes(char * buffer, uint32_t size);
 *				int32_t closeConnection(int32_t sockfd);
 * 
 * DATE:		Mar.
 * 
 * REVISIONS:	Mar.
 * 				Apr.
 * 
 * DESIGNER:	Matthew Shew, Delan Elliot, Wilson Hu
 * 
 * PROGRAMMER:	Matthew Shew, Delan Elliot, Wilson Hu
 * 
 * NOTES:
 * This file is a class wrapper around the client-side TCP 
 */
#include "tcpclient.h"



TCPClient::TCPClient()
{

}

/**
 * FUNCTION:
 * 
 * DATE:
 * 
 * REVISIONS:
 * 
 * DESIGNER:
 * 
 * PROGRAMMER:	Delan Elliot, Wilson Hu, Calvin Lai
 * 
 * INTERFACE:	int TCPClient::initializeSocket(EndPoint ep)
 * 
 * RETURNS:
 * 
 * NOTES:
 * 
 */
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


/**
 * FUNCTION:
 * 
 * DATE:
 * 
 * REVISIONS:
 * 
 * DESIGNER:
 * 
 * PROGRAMMER:
 * 
 * INTERFACE:
 * 
 * RETURNS:
 * 
 * NOTES:
 * 
 */
int32_t TCPClient::closeConnection(int32_t sockfd)
{
	return close(sockfd);
}

/**
 * FUNCTION:
 * 
 * DATE:
 * 
 * REVISIONS:
 * 
 * DESIGNER:
 * 
 * PROGRAMMER:
 * 
 * INTERFACE:
 * 
 * RETURNS:
 * 
 * NOTES:
 * 
 */
int32_t TCPClient::sendBytes(char * data, uint32_t len)
{
	int32_t result;
	if ((result = send(clientSocket, data, len , 0 )) == -1) {
		perror("client send error");
	}

	return result;
}

/**
 * FUNCTION:
 * 
 * DATE:
 * 
 * REVISIONS:
 * 
 * DESIGNER:
 * 
 * PROGRAMMER:
 * 
 * INTERFACE:
 * 
 * RETURNS:
 * 
 * NOTES:
 * 
 */
int32_t TCPClient::receiveBytes(char * buffer, uint32_t len)
{

	size_t n = 0;
	size_t bytesToRead = len;
	while ((n = recv (clientSocket, buffer, bytesToRead, 0)) < bytesToRead)
	{
		buffer += n;
		bytesToRead -= n;
	}
	return (bytesToRead);
}
