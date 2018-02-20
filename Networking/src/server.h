#include <sys/types.h>
#include <sys/socket.h>
#include <unistd.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <map>
#include "Connection.h"

struct EndPoint {
	uint32_t addr;
	uint16_t port;
};




class Server {

public:
	Server();
	int initializeSocket(short port);
	void sendBytes(int clientId, char * data, unsigned len);
	std::map<int, Connection> * clientMap;
private:
	int udpRecvSocket;
	sockaddr_in serverAddr;
	struct pollfd* poll_events;	

};

extern "C" void Server_sendBytes(void * serverPtr, int clientId, char * data, unsigned len);

extern "C" Server * Server_CreateServer();
