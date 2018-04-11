/*------------------------------------------------------------------------------------------------------------
-- FUNCTION: sendBytes
--
-- DATE: February 24th 2018
--
-- REVISIONS: March 5th 2018
--	
--
-- DESIGNER: Delan Elliot, Matthew Shew, Calvin Lai
--
-- PROGRAMMER: Delan Elliot, Matthew Shew
--
-- INTERFACE: int32_t sendBytes(short port)
--								port: open a server on this port
--
-- RETURNS: the number of bytes sent, or -1 if there is an error.
--
-- NOTES:
-- 		Sends bytes of length len to the address specified by the EndPoint struct. The endpoint is host byte order.
--		The EndPoint struct is filled in C# and the binary data is interpreted reliably because of fixed width types. 
--------------------------------------------------------------------------------------------------------------*/

#include "client.h"



Client::Client()
{
}


/*------------------------------------------------------------------------------------------------------------
-- FUNCTION: initializeSocket
--
-- DATE: March 7th 2018
--
-- REVISIONS:
--
-- DESIGNER: Delan Elliot, Matthew Shew, Calvin Lai, Wilson Hu
--
-- PROGRAMMER: Delan Elliot, Calvin Lai
--
-- INTERFACE: int32_t initializeSocket(EndPoint ep)
--								EndPoint: the IP address and port of the server  
--
-- RETURNS: 0 on success, or -1 if unsuccessfully opened. 
--
-- NOTES:
-- 		init is called once the unmanaged client has been instantiated, and it creates the socket, calls "connect" to
--		bind the socket to a single address. 
--------------------------------------------------------------------------------------------------------------*/
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



/*------------------------------------------------------------------------------------------------------------
-- FUNCTION: sendBytes
--
-- DATE: March 7th 2018
--
-- REVISIONS:
--
-- DESIGNER: Delan Elliot, Matthew Shew, Calvin Lai
--
-- PROGRAMMER: Delan Elliot, Calvin Lai
--
-- INTERFACE: int32_t sendBytes(char* data, unsigned len)
--								data: array of binary data to send
--								len: length of data to send in bytes
--
-- RETURNS: the number of bytes sent, or -1 if there is an error.
--
-- NOTES:
-- 		Sends the bytes specified by the array on the connected socket. Will return -1 if the socket has not been 
--		initalized. 
--------------------------------------------------------------------------------------------------------------*/
int32_t Client::sendBytes(char * data, unsigned len)
{
	int32_t retVal;
	if ((retVal = send(clientSocket, data, len , 0)) == -1) {
		perror("client send error");
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


