#include <sys/types.h>
#include <sys/socket.h>
#include <unistd.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <map>
#include <poll.h>
#include <iostream>
#include <string.h>
#include <stdlib.h>



#include <stdio.h>
#include <netdb.h>
#include <errno.h>



#ifndef SOCK_NONBLOCK
#include <fcntl.h>
#define SOCK_NONBLOCK O_NONBLOCK
#endif

#define SOCKET_NODATA 0
#define SOCKET_DATA_WAITING 1

struct EndPoint {
	uint32_t addr;
	uint16_t port;
};

class TCPServer {

public:
	TCPServer();
	int32_t initializeSocket(short port);
	int32_t acceptConnection();
	int32_t sendBytes(int clientSocket, char * data, unsigned len);
	int32_t receiveBytes(int clientSocket, char * buffer, unsigned len);
	
	

private:
	int udpSocket;

	sockaddr_in serverAddr;
	struct pollfd* poll_events;

};

 