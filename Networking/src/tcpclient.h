#ifndef TCPCLIENT_DEF
#define TCPCLIENT_DEF
#include <netinet/in.h>
#include <arpa/inet.h>
#include <poll.h>
#include <stdio.h>
#include <string.h>
#include <unistd.h>
#ifndef SOCK_NONBLOCK
#include <fcntl.h>
#define SOCK_NONBLOCK O_NONBLOCK
#endif

#include "EndPoint.h"

#define SERVER "142.232.135.38"
#define SOCKET_DATA_WAITING 555
#define SOCKET_NODATA 666


class TCPClient {

public:
	TCPClient();
	int initializeSocket(EndPoint ep);
	int32_t sendBytes(char * data, uint32_t len);
	int32_t receiveBytes(char * buffer, uint32_t size);
	void closeConnection();

private:
	int clientSocket;
	sockaddr_in serverAddr;

};

#endif
