#include "client.h"



Client::Client()
	{

	}


/**
	Initializes client socket
**/
int Client::initializeSocket(short port, char * server)
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



/**
	Sends char array to all connected clients
**/
void Client::sendBytes(char * data, unsigned len)
{
	if (sendto(clientSocket, data, len , 0 , (struct sockaddr *) &serverAddr, sizeof(serverAddr)) == -1) {
		perror("client send error");
	}
}



/**
	Sends char array to all connected clients
**/
int32_t Client::receiveBytes(char * buffer, uint32_t size)
{

	socklen_t len = sizeof(serverAddr);
	int32_t bytesRead = recvfrom(clientSocket, buffer, size, 0, (struct sockaddr *)&serverAddr, &len);

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



void Client::closeConnection() {
	close(clientSocket);
}


int main() {
	Client client;
	client.initializeSocket(5150, (char *)"192.168.0.2");

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
