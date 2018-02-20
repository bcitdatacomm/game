using System;
using System.Net.Sockets;

namespace Connection
{
    public unsafe struct sockaddr_in
    {
        private short sin_family;
        private ushort sin_port;
        private in_addr sin_addr;
        private fixed char sin_zero[8];
    }

    public struct in_addr
    {
        ulong s_addr;
    }

/*    public unsafe struct buffer_t{
        public char[] buffer = new char[2048];
        public int bufPos;
    }*/

    public unsafe class Connection
    {

        public static int MAX_BUF_SIZE = 2048;
        static int PORT_NO = 99999;

        uint m_ackNo;
        uint m_connectionId;
        uint m_connectionStatus;
        sockaddr_in m_in;
        sockaddr_in m_out;
        uint m_socketId;
        //buffer_t m_buffer;
        char[] buffer;
        int bufPos;

        //fixed char m_buffer[MAX_BUF_SIZE];
        //uint bufPos;

        public Connection(sockaddr_in client, sockaddr_in server, uint socketId, uint connectionId)
        {
            m_in = server;
            m_out = client;
            m_socketId = socketId;
            m_connectionId = connectionId;
            m_ackNo = 0;
            m_connectionStatus = 1;
            buffer = new char[2048];
        }

        public sockaddr_in getClient()
        {
            return m_out;
        }

        public uint getSocketId()
        {
            return m_socketId;
        }

        public unsafe char[] getBufferAddress()
        {
            return buffer;
        }

        public unsafe void write(char[] data)
        {
            if (data.Length + bufPos < MAX_BUF_SIZE)
            {
                Buffer.BlockCopy(data, 0, buffer, bufPos, data.Length);
            }
        }

        public void writeToBuffer(char[] otherBuffer)
        {
            Array.Clear(buffer, 0, buffer.Length);
            Buffer.BlockCopy(otherbuffer, 0, buffer, 0, otherBuffer.Length);
        }

        public void connectionLoop()
        {
            bool connected = true;
            while (connected)
            {
                //Placeholder function call
                if (pollsocket()) //pollsocket returns true if there is data
                {
                  /*
                  Placeholder function call
                  Recvfrom should:
                    receive from socket
                    write to buffer
                    (n chars)
                    store source data in sockaddr
                  */
                  recvfrom();
                }

            }
        }

        public uint getConnectionStatus()
        {
            return m_connectionStatus;
        }

        public void setConnectionStatus(uint connectionStatus)
        {
            m_connectionStatus = connectionStatus;
        }
    }
}
