﻿#ifndef TCP_DEF
#include "tcpserver.h"
#define TCP_DEF
#endif

#ifndef CLIENT_DEF
#include "client.h"
#define CLIENT_DEF
#endif

#ifndef SERVER_DEF
#include "server.h"
#define SERVER_DEF
#endif



// UDP SERVER
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


//UDP CLIENT
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


//TCP SERVER
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
