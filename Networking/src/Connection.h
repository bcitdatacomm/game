#pragma once
#include <unistd.h>
#include <stdlib.h>
#include <memory.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <sys/un.h>
#include <stdint.h>




#define MAX_BUF_SIZE        2048


    class Connection
    {
        public:
            Connection(sockaddr_in client, sockaddr_in server, int socketID);
            ~Connection();
            Connection(const Connection& other);
            struct sockaddr_in getClient();
            short getConnectionStatus();
            int getSocketID();
			void write(const char * data, uint32_t len);
            char* getBufferAddress();
            void writeToBuffer(char* otherBuffer, size_t numChars);
            void setConnectionStatus(short newStatus);
            

        private:
            
            int m_ackNo;
            short m_connectionStatus;
            struct sockaddr_in m_in;
            struct sockaddr_in m_out;
            int m_socketID;
            char m_buffer[MAX_BUF_SIZE];
			uint32_t bufPos;
    };


