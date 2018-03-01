#include "Connection.h"

/*
1. Server runs connection loop, waits for incoming connections
    - call recvfrom()...
    - store contents of datagram in buffer
    - if datagram == some verification packet
        - call Connection constructor, pass in sourceAddr struct
*/


    Connection::Connection(sockaddr_in client, sockaddr_in server, int socketID)
        : m_in(server)
        , m_out(client)
        , m_socketID(socketID)
    {
        m_ackNo = 0;
        m_connectionStatus = 1;
        memset(m_buffer, '\0', MAX_BUF_SIZE);
        
    }

    Connection::~Connection()
    {
    }

    Connection::Connection(const Connection& other)
    {
        m_socketID = other.m_socketID;
        m_out = other.m_in;
        m_in = other.m_in;
        m_ackNo = other.m_ackNo;
        m_connectionStatus = other.m_connectionStatus;
    }

    struct sockaddr_in Connection::getClient()
    {
        return m_out;
    }

    short Connection::getConnectionStatus()
    {
        return m_connectionStatus;
    }

    int Connection::getSocketID()
    {
        return m_socketID;
    }

	void Connection::write(const char * data, uint32_t len)
	{

		if (len + bufPos < MAX_BUF_SIZE)
		{
			memcpy(&m_buffer[bufPos], data, len);

		}
	}

    char* Connection::getBufferAddress()
    {
        return m_buffer;
    }

    void Connection::writeToBuffer(char* otherBuffer, size_t numChars)
    {
        memset(m_buffer, '\0', MAX_BUF_SIZE);
        memcpy(m_buffer, otherBuffer, numChars);
    }

    void Connection::setConnectionStatus(short newStatus)
    {
        m_connectionStatus = newStatus;
    }

// int main() {
//     return 1;
// }
