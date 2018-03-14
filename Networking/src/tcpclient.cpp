#include "tcpclient.h"



TCPClient::TCPClient()
{

}




/**
	Initializes client TCP socket to receive initial game data.
	@author Calvin Lai
**/
int TCPClient::initializeSocket(EndPoint * ep)
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
	serverAddr.sin_port = htons(ep->port);
	serverAddr.sin_addr.s_addr = htonl(ep->addr);


	if (connect(clientSocket, (struct sockaddr *)&serverAddr, sizeof(serverAddr)) < 0)
	{
		printf("\n Error : Connect Failed \n");
		perror("failure");
		return -1;
	}
	return 0;

}







void TCPClient::closeConnection() {
	close(clientSocket);
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
	return (len - bytesToRead);
}


int main()
{
	int result;
	TCPClient client;
	EndPoint ep;

	byte * addr = reinterpret_cast<byte *>(&(ep.addr));

	addr[0] = (byte) 113;
	addr[1] = (byte) 18;
	addr[2] = (byte) 232;
	addr[3] = (byte) 142;
	ep.port = 9999;


	result = client.initializeSocket(ep);
	printf("result is %d\n", result);
	char temp[] = "Hello\n";
	char buffer[200];
	char rBuffer[200];
	sprintf(buffer, "hello wilson");
	client.sendBytes(buffer, sizeof(buffer));
	printf("got past send\n");
	int count  = 0;
	while (true) {
		result = client.receiveBytes(rBuffer, sizeof(rBuffer));

		if(result == 0)
			printf("%s\n", rBuffer);
		break;
			//printf("%s\n", buffer);
		//int y = client.UdpPollSocket();
		/*
		if (y == SOCKET_DATA_WAITING) {
			client.receiveTCPBytes(buffer, 6);
			printf("%s\n", buffer);
		}*/
	}
	return 1;
}
