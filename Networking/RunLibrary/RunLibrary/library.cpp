#include "server.h"

extern "C" Server *Server_CreateServer()
{
	return new Server();
}

extern "C" int32_t Server_initServer(void *serverPtr)
{
	return ((Server *)serverPtr)->initializeSocket();
}

extern "C" int32_t Server_PollSocket(void *serverPtr)
{
	return ((Server *)serverPtr)->UdpPollSocket();
}

extern "C" int32_t Server_SelectSocket(void *serverPtr)
{
	return ((Server *)serverPtr)->UdpSelectSocket();
}

extern "C" int32_t Server_sendBytes(void *serverPtr, EndPoint ep, char *data, uint32_t len)
{
	return ((Server *)serverPtr)->sendBytes(ep, data, len);
}

extern "C" int32_t Server_recvBytes(void *serverPtr, EndPoint *addr, char *buffer, uint32_t bufSize)
{

	int32_t result = static_cast<Server *>(serverPtr)->UdpRecvFrom(buffer, bufSize, addr);
	return result;
}