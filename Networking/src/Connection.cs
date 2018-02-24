using System;
using System.Net.Sockets;

namespace COMP4981_NetworkingTest
{

    	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	    public struct CAddr{

		public static readonly CAddr Loopback = new CAddr("127.0.0.1");

		[FieldOffset(0)]
		public readonly uint Packet;
		[FieldOffset(0)]
		public readonly byte Byte0;
		[FieldOffset(1)]
		public readonly byte Byte1;
		[FieldOffset(2)]
		public readonly byte Byte2;
		[FieldOffset(3)]
		public readonly byte Byte3;


		public CAddr (string ip) {
			string[] parts = ip.Split('.');
			Packet = 0;
			Byte0 = byte.Parse(parts[3]);
			Byte1 = byte.Parse(parts[2]);
			Byte2 = byte.Parse(parts[1]);
			Byte3 = byte.Parse(parts[0]);
		}

		public CAddr (byte a, byte b, byte c, byte d) {
			Packet = 0;
			Byte0 = d;
			Byte1 = c;
			Byte2 = b;
			Byte3 = a;
		}

	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UdpAddr {
		public readonly CAddr addr;
		public readonly ushort port;


	}

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

    public unsafe class Connection
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
        //buffer_t m_buffer;
        char[] m_buffer;
        int m_bufPos;

        //fixed char m_buffer[MAX_BUF_SIZE];
        //uint bufPos;

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
            return m_buffer;
        }


        public unsafe void ReadFromBuffer(char[] data) //This is now read from buffer. It makes sense, kidn of. It used to be write()
        {
            if (data.Length + m_bufPos < MAX_BUF_SIZE)
            {
                Buffer.BlockCopy(data, 0, m_buffer, m_bufPos, 2 * data.Length * sizeof(Byte));  //Figure out why the member buffer requires 2B/element
                m_bufPos += data.Length;
            }
        }

        public void WriteToBuffer(char[] otherBuffer)   //This is now write to buffer. 
        {
            Array.Clear(m_buffer, 0, m_buffer.Length);  //This part doesn't make sense. 
            Buffer.BlockCopy(otherBuffer, 0, m_buffer, 0, 2 * otherBuffer.Length); //why does this do this
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
}
