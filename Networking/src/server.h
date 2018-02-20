#include <sys/types.h>
#include <sys/socket.h>
#include <unistd.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <map>
#include "Connection.h"


class Server {

public:
	Server();
	void sendBytes(int clientId, char * data, unsigned len);
	std::map<int, Connection> * clientMap;


};

extern "C" void Server_sendBytes(void * serverPtr, int clientId, char * data, unsigned len);

extern "C" Server * Server_CreateServer();