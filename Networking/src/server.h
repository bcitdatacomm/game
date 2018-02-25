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
	struct in_addr ip;
	unsigned short uport;
};

class Server {

public:
	Server();
	int initializeSocket(short port);
	void sendBytes(int clientId, char * data, unsigned len);
	std::map<int, Connection> * clientMap;
	int32_t UdpPollSocket();
	int32_t UdpRecvFrom(char * buffer, uint32_t size, EndPoint * addr);
	void initializeConnectionPool();
	sockaddr_in getServerAddr();
	bool removeConnection(int socketID);

private:
	int udpRecvSocket;
	sockaddr_in serverAddr;
	struct pollfd* poll_events;

};

extern "C" void Server_sendBytes(void * serverPtr, int clientId, char * data, unsigned len);

extern "C" Server * Server_CreateServer();
