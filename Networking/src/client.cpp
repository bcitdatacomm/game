#include "client.h"



Client::Client()
{
}


/**
	Initializes client socket
**/
int32_t Client::initializeSocket(EndPoint ep)
{
	if ((clientSocket = socket(AF_INET, SOCK_DGRAM, 0)) == -1)
	{
		return -1;
	}

	int optFlag = 1;

	if(setsockopt(clientSocket, SOL_SOCKET, SO_REUSEADDR, &optFlag, sizeof(optFlag)) == -1)
	{
		return -1;
	}

	memset(&serverAddr, 0, sizeof(struct sockaddr_in));
	serverAddr.sin_family = AF_INET;
	serverAddr.sin_port = htons(ep.port);
        serverAddr.sin_addr.s_addr = htonl(ep.addr);


    int error = -1;

    if ((error = connect(clientSocket, (struct sockaddr *) &serverAddr, sizeof(serverAddr)) == -1))

    {
        return error;
    }

    return 0;
}



/**
	Sends char array to all connected clients
**/
int32_t Client::sendBytes(char * data, unsigned len)
{
	int32_t retVal;
	if ((retVal = send(clientSocket, data, len , 0)) == -1) {
	}

	return retVal;
}



/**
	Sends char array to all connected clients
**/
int32_t Client::receiveBytes(char * buffer, uint32_t size)
{
	int32_t bytesRead = recv(clientSocket, buffer, size, 0);

	return bytesRead;
}



int32_t Client::UdpPollSocket()
{
	int numfds = 1;
	struct pollfd pollfds;
	pollfds.fd = clientSocket;
	pollfds.events = POLLIN;

	int retVal = poll(&pollfds, numfds, 0);

	if(pollfds.revents & POLLIN)
	{
		return SOCKET_DATA_WAITING;
	}
	return SOCKET_NODATA;
}


