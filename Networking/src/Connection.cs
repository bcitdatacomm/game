using System;
using System.Net.Sockets;

namespace Connection
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

 

/*    public unsafe struct buffer_t{
        public char[] buffer = new char[2048];
        public int bufPos;
    }
*/

    public unsafe class Connection
    {

        public static int MAX_BUF_SIZE = 2048;
        static ushort PORT_NO = 99999;

        UInt32 m_rcvAck;
		UInt32 m_sendAck;
		UInt32 m_sendSeq;
		UInt32 m_rcvSeq;

        uint m_connectionId;
        uint m_connectionStatus;
		UdpAddr m_client;
        uint m_socketId;
        //buffer_t m_buffer;
        char[] buffer;
        int bufPos;

        //fixed char m_buffer[MAX_BUF_SIZE];
        //uint bufPos;

        public Connection(UdpAddr client, uint socketId, uint connectionId)
        {
            m_in = server;
            m_out = client;
            m_socketId = socketId;
            m_connectionId = connectionId;
            m_ackNo = 0;
            m_connectionStatus = 1;
            buffer = new char[2048];
        }

        public UdpAddr getClient()
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
