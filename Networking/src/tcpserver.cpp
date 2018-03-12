#include "tcpserver.h"

#define SERVER_TCP_PORT 7000	// Default port
#define BUFLEN	200		//Buffer length
#define TRUE	1


TCPServer::TCPServer()
{
	
}


int32_t TCPServer::initializeSocket	(short port) 
{
	struct	sockaddr_in server; 
	if ((udpSocket = socket(AF_INET, SOCK_STREAM, 0)) == -1)
	{
		perror ("Can't create a socket");
		exit(1);
	}

	bzero((char *)&server, sizeof(struct sockaddr_in));
	server.sin_family = AF_INET;
	server.sin_port = htons(port);
	server.sin_addr.s_addr = htonl(INADDR_ANY); // Accept connections from any client

	if (bind(udpSocket, (struct sockaddr *)&server, sizeof(server)) == -1)
	{
		perror("Can't bind name to socket");
		exit(1);
	}

	listen(udpSocket, 5);
	return 1;
}




int32_t TCPServer::acceptConnection() {
	int clientSocket;
	struct sockaddr_in client;
	socklen_t client_len = sizeof(client);
	if ((clientSocket = accept(udpSocket, (struct sockaddr *)&client, &client_len)) == -1)
	{
		fprintf(stderr, "Can't accept client\n");
		return 0;
	}
	return clientSocket;
}




int32_t TCPServer::sendBytes(int clientSocket, char * data, unsigned len) {
	return send(clientSocket, data, len, 0);
}





int32_t TCPServer::receiveBytes(int clientSocket, char * buffer, unsigned len) {
	int n = 0;
	int bytesToRead = len;
	while ((n = recv (clientSocket, buffer, bytesToRead, 0)) < len) {
		buffer += n;
		bytesToRead -= n;
	}
	return (len - bytesToRead);
}





int main ()
{

	int	clientSocket;
	int port;
	struct sockaddr_in client;
	char buffer[200];

	TCPServer tcpserver;

	tcpserver.initializeSocket(SERVER_TCP_PORT);

	clientSocket = tcpserver.acceptConnection(&client);

	while (TRUE)
	{
		tcpserver.receiveBytes(clientSocket, buffer, 200);

		tcpserver.sendBytes(clientSocket, buffer, 200);
	}

	close(clientSocket);
	return(0);
}
