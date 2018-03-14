using System;
using System.Text;
using System.Threading;

namespace Client
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ClientWrapper client = new ClientWrapper();
			int retVal = client.Init("127.0.0.1", 42000);
			byte[] buffer = new byte[2048];
			byte[] inbuffer = new byte[2048];
			buffer[0] = (byte)'H';
			buffer[1] = (byte)'E';
			buffer[2] = (byte)'L';
			buffer[3] = (byte)'L';
			buffer[4] = (byte)'O';



			for (int i = 0; i < 5; i++)
			{
				client.Send(buffer, 6);
				for (;;) 
				{
					if (client.Poll () != ClientWrapper.SOCKET_DATA_WAITING) {
						Thread.Sleep(100);
					} else {
						client.Recv(inbuffer, 2048);
						Console.WriteLine ("bytes recv: {0}", Encoding.Default.GetString (inbuffer));
						break;
					}
				}
					
			}

		}
		}
}

