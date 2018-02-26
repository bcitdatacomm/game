using System;
using System.Runtime.InteropServices;

namespace RunLibrary
{
	unsafe class MainClass
	{
		private static Int32 SOCKET_NODATA = 0;
		private static Int32 SOCKET_DATA_WAITING = 1;

		public static void Main (string[] args)
		{
			EndPoint ep = new EndPoint ();
			IntPtr serverInstance = Server.Server_CreateServer();
			byte[] buffer = new byte[2048];

			Int32 err = Server.Server_initServer(serverInstance, 7676);
			while (true) 
			{
				Int32 res = Server.Server_PollSocket (serverInstance);
				if (res == SOCKET_DATA_WAITING) 
				{
					fixed(byte* tmpBuf = buffer) 
					{
						Int32 length = Server.Server_recvBytes(serverInstance, &ep, new IntPtr(tmpBuf), 2048);
						Console.WriteLine ("Received: {0}", buffer);

						Server.Server_sendBytes(serverInstance, ep, new IntPtr(tmpBuf), 7);
					}

			
				}
			}


			
		}
	}
}
