using System;
using System.Runtime.InteropServices;

namespace Networking
{
	public unsafe class Client
	{
		private IntPtr connection;
		private EndPoint server;
		private EndPoint rcvEndPoint;
		EndPoint * ptr;

		public static Int32 SOCKET_NO_DATA = 0;
		public static Int32 SOCKET_DATA_WAITING = 1;

		public Client()
		{
			connection = ServerLibrary.Server_CreateServer();

		}

		public Int32 Init(string ipaddr, ushort port)
		{
			CAddr addr = new CAddr (ipaddr);
			server = new EndPoint (ipaddr, port);
			rcvEndPoint = new EndPoint ();
			Int32 err = ServerLibrary.Server_initServer(connection, port);
			return err;
		}

		public Int32 Poll()
		{
			return ServerLibrary.Server_PollSocket (connection);
		}

		public Int32 Recv(byte[] buffer, Int32 len)
		{
			fixed(byte* tmpBuf = buffer) 
			{
				fixed(EndPoint* p = &rcvEndPoint)
				{
					UInt32 bufLen = Convert.ToUInt32 (len);
					Int32 length = ServerLibrary.Server_recvBytes(connection, p, new IntPtr(tmpBuf), bufLen);
					return length;
				}
			}
		}

		public Int32 Send(byte[] buffer, Int32 len)
		{
			uint bufLen = Convert.ToUInt32 (len);

			fixed( byte* tmpBuf = buffer) 
			{
					Int32 ret = ServerLibrary.Server_sendBytes (connection, server, new IntPtr (tmpBuf), bufLen);
					return ret;
			}

		}
	}
}

