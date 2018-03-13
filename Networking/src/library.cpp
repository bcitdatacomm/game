#ifndef SERVER_DEF
#include "server.h"
#define SERVER_DEF
#endif

#ifndef CLIENT_DEF
#include "client.h"
#define CLIENT_DEF
#endif

extern "C" Server *Server_CreateServer()
{
    return new Server();
}

extern "C" int32_t Server_initServer(void *serverPtr, short port)
{
    return ((Server *)serverPtr)->initializeSocket(port);
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

    int32_t result = ((Server *)serverPtr)->UdpRecvFrom(buffer, bufSize, addr);
    return result;
}

extern "C" Client *Client_CreateClient()
{
    return new Client();
}

extern "C" int32_t Client_sendBytes(void *clientPtr, char *buffer, uint32_t len)
{
    Client *p = (Client *)clientPtr;
    return p->sendBytes(buffer, len);
}

extern "C" int32_t Client_recvBytes(void *clientPtr, char *buffer, uint32_t len)
{
    return ((Client *)clientPtr)->receiveBytes(buffer, len);
}

extern "C" int32_t Client_PollSocket(void *clientPtr)
{
    return ((Client *)clientPtr)->UdpPollSocket();
}

extern "C" int32_t Client_SelectSocket(void *clientPtr)
{
    return ((Client *)clientPtr)->UdpSelectSocket();
}

extern "C" int32_t Client_initClient(void *clientPtr, EndPoint ep)
{
    return ((Client *)clientPtr)->initializeSocket(ep);
}
