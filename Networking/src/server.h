#include <sys/types.h>
#include <sys/socket.h>
#include <unistd.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <map>
#include <poll.h>
#include <iostream>
#include "Connection.h"
#include "Connection.cpp"
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

class Server {

public:
	Server();
	int initializeSocket(short port);
	int32_t sendBytes(EndPoint ep, char * data, unsigned len);
	int32_t UdpPollSocket();
	int32_t UdpRecvFrom(char * buffer, uint32_t size, EndPoint * addr);
	sockaddr_in getServerAddr();
	
	void setEndPointIp(EndPoint * ep, char zero, char one, char two, char three);
	
	

private:
	int udpSocket;
	sockaddr_in serverAddr;
	struct pollfd* poll_events;

};

extern "C" void Server_sendBytes(void * serverPtr, EndPoint ep, char * data, unsigned len);

extern "C" Server * Server_CreateServer();
