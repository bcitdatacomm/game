using System;
namespace Client
{
	public unsafe class ClientWrapper
	{
		private IntPtr connection;
		private EndPoint server;
		private EndPoint rcvEndPoint;
		EndPoint* ptr;

		public static Int32 SOCKET_NO_DATA = 0;
		public static Int32 SOCKET_DATA_WAITING = 1;

		public ClientWrapper()
		{
			connection = ServerLibrary.Client_CreateClient();

		}

		public Int32 Init(string ipaddr, ushort port)
		{
			CAddr addr = new CAddr(ipaddr);
			server = new EndPoint(ipaddr, port);
			rcvEndPoint = new EndPoint();
			Int32 err = ServerLibrary.Client_initClient(connection, server);
			return err;
		}

		public Int32 Poll()
		{
			return ServerLibrary.Client_PollSocket(connection);
		}

		public Int32 Recv(byte[] buffer, Int32 len)
		{
			fixed (byte* tmpBuf = buffer)
			{
				UInt32 bufLen = Convert.ToUInt32(len);
				Int32 length = ServerLibrary.Client_recvBytes(connection, new IntPtr(tmpBuf), bufLen);
				return length;
			}
		}

		public Int32 Send(byte[] buffer, Int32 len)
		{
			uint bufLen = Convert.ToUInt32(len);

			fixed (byte* tmpBuf = buffer)
			{
				Int32 ret = ServerLibrary.Client_sendBytes(connection, new IntPtr(tmpBuf), bufLen);
				return ret;
			}

		}
	}
}