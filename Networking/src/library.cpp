#ifndef SERVER_DEF
#include "server.h"
#define SERVER_DEF
#endif


extern "C" Server * Server_CreateServer()
{
	return new Server();
}

extern "C" int32_t Server_initServer(void * serverPtr, short port)
{
	return ((Server *)serverPtr)->initializeSocket(port);
}

extern "C" int32_t Server_PollSocket(void * serverPtr)
{
	return ((Server *)serverPtr)->UdpPollSocket();
}


extern "C" int32_t Server_sendBytes(void * serverPtr, EndPoint ep, char * data, uint32_t len)
{
	return ((Server *)serverPtr)->sendBytes(ep, data, len);
}
  
extern "C" int32_t Server_recvBytes(void * serverPtr, EndPoint * addr, char * buffer, uint32_t bufSize)
{

	int32_t result = static_cast<Server*>(serverPtr)->UdpRecvFrom(buffer, bufSize, addr);
	return result;
}
