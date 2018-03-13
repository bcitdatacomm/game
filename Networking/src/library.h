#include "server.h"

extern "C" Server *Server_CreateServer();

extern "C" int32_t Server_initServer(void *);

extern "C" int32_t Server_PollSocket(void *);

extern "C" int32_t Server_SelectSocket(void *);

extern "C" int32_t Server_sendBytes(void *, EndPoint, char *, uint32_t len);

extern "C" int32_t Server_recvBytes(void *, EndPoint *, char *, uint32_t);
