#include "tcpserver.h"

#define SERVER_TCP_PORT 9999	// Default port
#define BUFLEN					1200		//Buffer length
#define MAX_NUM_CLIENTS 30
#define TRUE						1
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
		std::cout << strerror(errno) << std::endl;
		exit(1);
	}
	int optFlag = 1;

	if(setsockopt(tcpSocket, SOL_SOCKET, SO_REUSEADDR, &optFlag, sizeof(optFlag)) == -1)
	{
		perror("set opts failed");
		return -1;
	}

	// Zero memory of server sockaddr_in struct
	bzero((char *)&server, sizeof(struct sockaddr_in));

	server.sin_family = AF_INET;
	server.sin_port = htons(port);
	server.sin_addr.s_addr = htonl(INADDR_ANY); // Accept connections from any client

	if (bind(tcpSocket, (struct sockaddr *)&server, sizeof(server)) == -1)
	{
		perror("Can't bind name to socket");
		std::cout << strerror(errno) << std::endl;
		exit(1);
	}

	listen(tcpSocket, MAX_NUM_CLIENTS);
	return 1;
}


int32_t TCPServer::acceptConnection(sockaddr_in* client)
{
	int clientSocket;
	//struct sockaddr_in client;
	socklen_t client_len = sizeof(client);
	if ((clientSocket = accept(tcpSocket, (struct sockaddr *)&client, &client_len)) == -1)
	{
		std::cout << strerror(errno) << std::endl;
		return 0;
	}
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


int main ()
{

	int	clientSocket;
	int clientArray[MAX_NUM_CLIENTS];
	int port;
	struct sockaddr_in client;
	char buffer[200];

	TCPServer tcpserver;

	tcpserver.initializeSocket(SERVER_TCP_PORT);

	clientSocket = tcpserver.acceptConnection(&client);

	while (TRUE)
	{
		tcpserver.receiveBytes(clientSocket, buffer, 200);

		fprintf(stderr, "Received: %s", buffer);
		fflush(stderr);
		tcpserver.sendBytes(clientSocket, buffer, 200);
	}

	close(clientSocket);
	return(0);
}
