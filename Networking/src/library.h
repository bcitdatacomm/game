
extern "C" Server *Server_CreateServer();

extern "C" int32_t Server_initServer(void *);

extern "C" int32_t Server_PollSocket(void *);

extern "C" int32_t Server_SelectSocket(void *);

extern "C" int32_t Server_sendBytes(void *, EndPoint, char *, uint32_t len);

extern "C" int32_t Server_recvBytes(void *, EndPoint *, char *, uint32_t);

extern "C" TCPServer * TCPServer_CreateServer();

extern "C" int32_t TCPServer_initServer(void *);

extern "C" int32_t TCPServer_acceptConnection(void *);

extern "C" int32_t TCPServer_sendBytes(void *, int32_t, char *, uint32_t);

extern "C" int32_t TCPServer_recvBytes(void *, int32_t, char *, uint32_t);


extern "C" Client *Client_CreateClient();

extern "C" int32_t Client_sendBytes(void *clientPtr, char *buffer, uint32_t len);

extern "C" int32_t Client_recvBytes(void *clientPtr, char *buffer, uint32_t len);

extern "C" int32_t Client_PollSocket(void *clientPtr);

extern "C" int32_t Client_SelectSocket(void *clientPtr);

extern "C" int32_t Client_initClient(void *clientPtr, EndPoint ep);





extern "C" TCPClient *TCPClient_CreateClient();

extern "C" int32_t TCPClient_initClient(void *clientPtr, EndPoint ep);

extern "C" int32_t TCPClient_sendBytes(void *clientPtr, char *buffer, uint32_t len);

extern "C" int32_t TCPClient_recvBytes(void *clientPtr, char *buffer, uint32_t len);

extern "C" int32_t TCPClient_closeConnection(void *clientPtr);


