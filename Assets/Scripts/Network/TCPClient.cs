using System;

namespace Networking
{
	public unsafe class TCPClient
	{
		private IntPtr tcpClient;
        private Int32 clientSocket;

		public TCPClient()
		{
			tcpClient = ServerLibrary.TCPClient_CreateClient();
		}

		public Int32 Send(byte[] buffer, Int32 len)
		{
			fixed( byte* tmpBuf = buffer)
			{
				UInt32 bufLen = Convert.ToUInt32 (len);
				Int32 ret = ServerLibrary.TCPClient_sendBytes(tcpClient, new IntPtr(tmpBuf), bufLen);
				return ret;
			}
		}

		public Int32 Recv(byte[] buffer, Int32 len)
		{
			Int32 length;
			fixed (byte* tmpBuf = buffer)
			{
				UInt32 bufLen = Convert.ToUInt32(len);
				length = ServerLibrary.TCPClient_recvBytes(tcpClient, new IntPtr(tmpBuf), bufLen);

				return length;
			}

		}

		public Int32 Init(EndPoint ep)
		{
			clientSocket = ServerLibrary.TCPClient_initClient(tcpClient, ep);
			return clientSocket;
		}

		public Int32 CloseConnection(Int32 sockfd)
		{
			Int32 err = ServerLibrary.TCPClient_closeConnection(sockfd);
			return err;
		}
	}
}
