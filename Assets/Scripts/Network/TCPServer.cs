using System;

namespace Networking
{
	public unsafe class TCPServer
    {

		private IntPtr tcpServer;

		public TCPServer()
		{
			tcpServer = ServerLibrary.TCPServer_CreateServer();

		}

		public Int32 Init(ushort port)
		{
			Int32 err = ServerLibrary.TCPServer_initServer(tcpServer, port);
			return err;
		}


		public Int32 AcceptConnection(ref EndPoint ep)
        {
			fixed(EndPoint* p = &ep)
			{
				return ServerLibrary.TCPServer_acceptConnection(tcpServer, p);
			}
        }

    
		public Int32 Recv(Int32 socket, byte[] buffer, Int32 len)
		{
			Int32 length;
			fixed (byte* tmpBuf = buffer)
			{
				UInt32 bufLen = Convert.ToUInt32(len);
				length = ServerLibrary.TCPServer_recvBytes(tcpServer, socket, new IntPtr(tmpBuf), bufLen);
				
				return length;
			}
		}

        public Int32 Send(Int32 socket, byte[] buffer, Int32 len)
		{
			fixed( byte* tmpBuf = buffer)
			{
				UInt32 bufLen = Convert.ToUInt32 (len);
				Int32 ret = ServerLibrary.TCPServer_sendBytes(tcpServer, socket, new IntPtr(tmpBuf), bufLen);
				return ret;
			}
		}
	}
}

