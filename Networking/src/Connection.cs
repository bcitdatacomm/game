using System;
using System.Net.Sockets;
using System.Collections.Queue;

namespace COMP4981_NetworkingTest
{
    enum connectionStatus : byte
    {
        disconnected        = 0,
        connection_unstable = 1,
        connection_stable   = 2,
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

    /*  public unsafe struct buffer_t{
            public char[] buffer = new char[2048];
            public int bufPos;
        }*/

    public unsafe class Connection
    {

        public static int MAX_BUF_SIZE = 2048;
        public static ushort PORT_NO = 9999;
        public static short AF_INET = 2;

        private uint ackNo;
        private uint connectionId;
        private uint connectionStatus = 0;
        private sockaddr_in in;
        private sockaddr_in out;
        private uint socketId;
        private byte[] buffer;
        private Queue<Array> inputQueue;
        private Queue<Array> outputQueue;
        //We need two buffers: input and output
        //byte[] inputBuffer;
        //byte[] outputBuffer;
        private int bufPos;

        //fixed char this.buffer[MAX_BUF_SIZE];
        //uint bufPos;

        public Connection(sockaddr_in client, sockaddr_in server, uint socketId, uint connectionId)
        {
            this.in = server;
            this.out = client;
            this.socketId = socketId;
            this.connectionId = connectionId;
            this.ackNo = 0;
            this.connectionStatus = 0;
            this.buffer = new byte[MAX_BUF_SIZE];
            this.bufPos = 0;
            this.ackNo = 0;=
        }

        /*
         * TODO: Refactor to ReadFromInputBuffer, reads inputbuffer from client / server
         */
        public unsafe bool ReadFromBuffer(byte[] data) //This is now read from buffer. It makes sense, kidn of. It used to be write()
        {
            if (data.Length + this.bufPos < MAX_BUF_SIZE)
            {
                Buffer.BlockCopy(data, 0, this.buffer, this.bufPos, data.Length * sizeof(Byte));
                this.bufPos += data.Length;
                return true;
            }
            return false;
        }

        /*
         * TODO: Refactor to WriteToOutputBuffer, writes output to buffer for server / client
         */
        public void WriteToBuffer(byte[] otherBuffer)
        {
            //Array.Clear(this.buffer, 0, this.buffer.Length);  //This part doesn't make sense. Currently clears buffer to be available to be written to. 
            Buffer.BlockCopy(this.buffer, 0, otherBuffer, 0, otherBuffer.Length); //dest: otherBuffer, src: this.buffer;
        }


        public void CheckSocket()
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

        public void AddToInputQueue()
        {

        }

        public void RemoveFromInputQueue()
        {

        }

        public void AddToOutputQueue()
        {

        }

        public void RemoveFromOutputQueue()
        {

        }

        public sockaddr_in GetClient() { return this.out; }

        public uint GetSocketId() { return this.socketId; }

        public unsafe byte[] GetBuffer() { return this.buffer; }

        public uint GetConnectionStatus() { return this.connectionStatus; }

        public void SetConnectionStatus(uint status) { this.connectionStatus = status; }
    }
}
