using System;

namespace Networking
{
	public class TCPClient
	{
		private IntPtr tcpClient;

		public TCPClient()
		{
			tcpClient = ServerLibrary.TCPClient_CreateClient();
		}

        public Int32 Send(byte[] buffer, Int32 len)
        {
            unsafe
            { 
                fixed (byte* tmpBuf = buffer)
                {
                    UInt32 bufLen = Convert.ToUInt32(len);
                    Int32 ret = ServerLibrary.TCPClient_sendBytes(tcpClient, new IntPtr(tmpBuf), bufLen);
                    return ret;
                }
            }
		}

		public Int32 Recv(byte[] buffer, Int32 len)
		{
			Int32 length;
            unsafe
            {
                fixed (byte* tmpBuf = buffer)
                {
                    UInt32 bufLen = Convert.ToUInt32(len);
                    length = ServerLibrary.TCPClient_recvBytes(tcpClient, new IntPtr(tmpBuf), bufLen);

                    return length;
                }
            }

		}

		public Int32 Init(EndPoint ep)
		{
			Int32 err = ServerLibrary.TCPClient_initClient(tcpServer, ep);
			return err;
		}
	}
}

