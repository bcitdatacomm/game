#ifndef SERVER_DEF
#include "tcpserver.h"
#define SERVER_DEF
#endif


extern "C" TCPServer * TCPServer_CreateServer()
{
	return new TCPServer();
}

extern "C" int32_t TCPServer_initServer(void * serverPtr, short port)
{
	return ((TCPServer *)serverPtr)->initializeSocket(port);
}

extern "C" int32_t TCPServer_acceptConnection(void * serverPtr)
{
	return ((TCPServer *)serverPtr)->acceptConnection();
}

extern "C" int32_t TCPServer_sendBytes(void * serverPtr, int32_t clientSocket, char * data, uint32_t len)
{
	return ((TCPServer *)serverPtr)->sendBytes(clientSocket, data, len);
}
  
extern "C" int32_t TCPServer_recvBytes(void * serverPtr, int32_t clientSocket, char * buffer, uint32_t bufSize)
{
	return ((TCPServer *)serverPtr)->receiveBytes(clientSocket, buffer, bufSize);
}
