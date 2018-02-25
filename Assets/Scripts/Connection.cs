using System;
using System.Net.Sockets;
using UnityEngine;


    enum connectionStatus : byte
    {
        disconnected = 0,
        connection_bad = 1,
        connection_good = 2,
    }

    public unsafe struct sockaddr_in
    {
        public short sin_family;
        public ushort sin_port;
        public in_addr sin_addr;
        public fixed char sin_zero[8];
    }

    public struct in_addr
    {
        public ulong s_addr;
    }

    /*    public unsafe struct buffer_t{
            public char[] buffer = new char[2048];
            public int bufPos;
        }*/

    public unsafe class Connection : MonoBehaviour
    {

        public static int MAX_BUF_SIZE = 2048;
        public static ushort PORT_NO = 9999;
        public static short AF_INET = 2;

        uint m_ackNo;
        uint m_connectionId;
        uint m_connectionStatus = 0;
        sockaddr_in m_in;
        sockaddr_in m_out;
        uint m_socketId;
        char[] m_buffer;
        //We need two buffers: input and output
        //char[] m_inputBuffer;
        //char[] m_outputBuffer;
        int m_bufPos;

        //fixed char m_buffer[MAX_BUF_SIZE];
        //uint bufPos;

        public Connection()
        {
            m_socketId = 1;
            m_connectionId = 1;
            m_ackNo = 0;
            m_connectionStatus = 0;
            m_buffer = new char[MAX_BUF_SIZE];
            m_bufPos = 0;
            m_ackNo = 0;

        }
    /*
    public Connection(sockaddr_in client, sockaddr_in server, uint socketId, uint connectionId)
    {
        m_in = server;
        m_out = client;
        m_socketId = socketId;
        m_connectionId = connectionId;
        m_ackNo = 0;
        m_connectionStatus = 0;
        m_buffer = new char[MAX_BUF_SIZE];
        m_bufPos = 0;
        m_ackNo = 0;

    }*/

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
            return m_buffer;
        }

        /*
         * TODO: Refactor to ReadFromInputBuffer, reads inputbuffer from client / server
         */
        public unsafe void ReadFromBuffer(char[] data) //This is now read from buffer. It makes sense, kidn of. It used to be write()
        {
            if (data.Length + m_bufPos < MAX_BUF_SIZE)
            {
                Buffer.BlockCopy(data, 0, m_buffer, m_bufPos, 2 * data.Length * sizeof(Byte));
                m_bufPos += data.Length;
            Debug.Log("src: " + data[0]);
            Debug.Log("dest: " + m_buffer[0]);
        }
        }

        /*
         * TODO: Refactor to WriteToOutputBuffer, writes output to buffer for server / client
         */
        public void WriteToBuffer(char[] otherBuffer)
        {
            //Array.Clear(m_buffer, 0, m_buffer.Length);  //This part doesn't make sense. Currently clears buffer to be available to be written to. 
            Buffer.BlockCopy(m_buffer, 0, otherBuffer, 0, 2 * otherBuffer.Length); //dest: otherBuffer, src: m_buffer;
        Debug.Log("src: " + m_buffer[0]);
        Debug.Log("dest: " + otherBuffer[0]);
    }


        public void checkSocket()
        {
            //if (pollsocket()) 
            //{

            //}
        }

        //public void connectionLoop()
        //{
        //    bool connected = true;
        //    while (connected)
        //    {
        //        //Placeholder function call
        //        if (pollsocket()) //pollsocket returns true if there is data
        //        {
        //            /*
        //            Placeholder function call
        //            Recvfrom should:
        //              receive from socket
        //              write to buffer
        //              (n chars)
        //              store source data in sockaddr
        //            */
        //            recvfrom();
        //        }
        //    }
        //}



        public uint GetConnectionStatus()
        {
            return m_connectionStatus;
        }

        public void setConnectionStatus(uint status)
        {
            m_connectionStatus = status;
        }
    }

