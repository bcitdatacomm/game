#include "tcpclient.h"



TCPClient::TCPClient()
	{

	}




/**
	Initializes client TCP socket to receive initial game data.
	@author Calvin Lai
**/
int TCPClient::initializeSocket(short port, char * server) 
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
	serverAddr.sin_port = htons(port);

	if(inet_pton(AF_INET, server, &serverAddr.sin_addr) <= 0)
    {
        printf("\n inet_pton error occured\n");
        return -1;
    } 

    if (connect(clientSocket, (struct sockaddr *)&serverAddr, sizeof(serverAddr)) < 0)
    {
       printf("\n Error : Connect Failed \n");
	perror("failure");
       return -1;
    } 
	return 1;
}







void TCPClient::closeConnection() {
	close(clientSocket);
}

/**
	Sends char array to all connected clients
**/
void TCPClient::sendBytes(char * data, uint32_t len)
{
	if (send(clientSocket, data, len , 0 ) == -1) {
		perror("client send error");
	}
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


int main() {
	int result;
	TCPClient client;
	result = client.initializeSocket(9999, (char *)"142.232.18.92");
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
