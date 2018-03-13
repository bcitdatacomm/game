using System;

namespace Server
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			byte[] outbuffer = new byte[2048];
			byte[] inBuffer = new byte[2048];
			Int32 rcvLen;
			outbuffer[0] = (byte)'g';
			outbuffer[1] = (byte)'o';
			outbuffer[2] = (byte)'o';
			outbuffer[3] = (byte)'d';
			outbuffer[4] = (byte)'b';
			outbuffer[5] = (byte)'y';
			outbuffer[6] = (byte)'e';

			ServerWrapper s = new ServerWrapper();
			EndPoint ep = new EndPoint ();

			int initVal = s.Init(42000);
			int  len = 2048;
			for (;;)
			{
				if(s.Poll() == ServerWrapper.SOCKET_DATA_WAITING)
				{
					rcvLen = s.Recv (ref ep, inBuffer, len);
					s.Send (ep, outbuffer, 7);
				}
			}
		}
	}
}
