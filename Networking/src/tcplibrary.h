#include "tcpserver.h"

extern "C" TCPServer * TCPServer_CreateServer();

extern "C" int32_t TCPServer_initServer(void *);

extern "C" int32_t TCPServer_acceptConnection(void *);

extern "C" int32_t TCPServer_sendBytes(void *, int32_t, char *, uint32_t);

extern "C" int32_t TCPServer_recvBytes(void *, int32_t, char *, uint32_t);
