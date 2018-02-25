#include "client.h"


/*------------------------------------------------------------------------------------------------------------------
--FUNCTION: Client
--
--DATE : February 25, 2018
--
--REVISIONS : (Date and Description)
--
--DESIGNER :
--
--PROGRAMMER : 
--
--INTERFACE : Client::Client()
--
--RETURNS : Client - New Client object
--
--NOTES :
--This function is called to construct a new Client object
----------------------------------------------------------------------------------------------------------------------*/
Client::Client()
	{

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
--INTERFACE : int Client::initializeSocket(short port, const char * server)
--				short port: The port to set the socket to
--				const char * server: The Server IP address to connect to
--
--RETURNS : int
--				-1 on failure, 0 on success
--
--NOTES :
--This function is called to initialize a new socket for the Client to connect to the game server
----------------------------------------------------------------------------------------------------------------------*/
int Client::initializeSocket(short port, const char * server)
{
	if ((clientSocket = socket(AF_INET, SOCK_DGRAM | SOCK_NONBLOCK, 0)) == -1)
	{
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
	serverAddr.sin_port = htons(port);

	if (inet_aton(server, &serverAddr.sin_addr) == 0)
    {
        perror("inet_aton() failed\n");
        exit(1);
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
--INTERFACE : void Client::sendBytes(char * data, unsigned len)
--				char * data: The buffer of data to send to the game server
--				unsigned len: The size of the buffer to send
--
--RETURNS : void
--
--NOTES :
--This function is called to send data from the client to the game server
----------------------------------------------------------------------------------------------------------------------*/
void Client::sendBytes(char * data, unsigned len)
{
	if (sendto(clientSocket, data, len , 0 , (struct sockaddr *) &serverAddr, sizeof(serverAddr)) == -1) {
		perror("client send error");
	}
}



/*------------------------------------------------------------------------------------------------------------------
--FUNCTION: receiveBytes
--
--DATE : February 25, 2018
--
--REVISIONS : (Date and Description)
--
--DESIGNER :
--
--PROGRAMMER : 
--
--INTERFACE : int32_t Client::receiveBytes(char * buffer, uint32_t size)
--				char * buffer: The buffer to fill with read in data
--				uint32_t size: The size of the buffer that can be filled with data
--
--RETURNS : int32_t
--				The number of bytes read from the socket
--
--NOTES :
--This function is called to read data from the client socket
----------------------------------------------------------------------------------------------------------------------*/
int32_t Client::receiveBytes(char * buffer, uint32_t size)
{
	socklen_t len = sizeof(serverAddr);
	int32_t bytesRead = recvfrom(clientSocket, buffer, size, 0, (struct sockaddr *)&serverAddr, &len);
	return bytesRead;
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
--INTERFACE : int32_t Client::UdpPollSocket()
--
--RETURNS : int32_t
--				SOCKET_DATA_WAITING if there is data ready to be read
--				SOCKET_NODATA if there is no data ready to be read
--
--NOTES :
--This function is called to check if there is data in the socket buffer ready to read
----------------------------------------------------------------------------------------------------------------------*/
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


/*------------------------------------------------------------------------------------------------------------------
--FUNCTION: closeConnection
--
--DATE : February 25, 2018
--
--REVISIONS : (Date and Description)
--
--DESIGNER :
--
--PROGRAMMER : 
--
--INTERFACE : bool Client::closeConnection()
--
--RETURNS : void
--
--NOTES :
--This function is called to send data from the server to a specified client
----------------------------------------------------------------------------------------------------------------------*/
void Client::closeConnection() {
	close(clientSocket);
}


int main() {
	Client client;
	client.initializeSocket(5150, "192.168.0.2");

	char temp[] = "Hello";
	char buffer[100];

	client.sendBytes(temp, 6);

	int count  = 0;
	while (true) {
		int y = client.UdpPollSocket();
		if (y == SOCKET_DATA_WAITING) {
			client.receiveBytes(buffer, 6);
			printf("%s\n", buffer);
		}
	}
	return 1;
}
