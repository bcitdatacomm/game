#include "server.h"



	

Server::Server()
	{
		clientMap = new std::map<int, Connection>();
	}

void Server::sendBytes(int clientId, char * data, unsigned len)
{
	std::map<int, Connection>::iterator it = clientMap->find(clientId);
	 
	if (it != clientMap->end())
	{
			
	}
}





extern "C" Server * Server_CreateServer()
{
	return new Server();
}

extern "C" void Server_sendBytes(void * serverPtr, int clientId, char * data, unsigned len) 
{
	((Server *)serverPtr)->sendBytes(clientId, data, len);
}