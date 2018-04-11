/*---------------------------------------------------------------------------------------
--	SOURCE FILE:	Client.cs -   A C# wrapper class providing UDP client functions
--
--	PROGRAM:		game
--
--	FUNCTIONS:		Init(string ipaddr, ushort port)
--					Poll()
--					Select()
--					Recv(byte[] buffer, Int32 len)
--					Send(byte[] buffer, Int32 len)
--
--	DATE:			March 10th, 2018
--
--	REVISIONS:		(Date and Description)
--
--	DESIGNERS:		Delan Elliot, Wilson Hu, Jeff Chou, Jeremy Lee
--
--	PROGRAMMER:		Delan Elliot, Li Yan Tong
--
--	NOTES:
--		Client.cs provides a C# wrapper for the C++ functions implemented in the shared library.
--		It uses C# interopservices to interact with unmanaged data at the binary level.
---------------------------------------------------------------------------------------*/


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


/*------------------------------------------------------------------------------------------------------------
-- FUNCTION: Init
--
-- DATE: March 7th 2018
--
-- REVISIONS:
--
-- DESIGNER: Delan Elliot, Wilson Hu, Jeremy Lee, Jeff Chou
--
-- PROGRAMMER: Delan Elliot
--
-- INTERFACE: Int32 Init(string ipaddr, ushort port)
--								port: open a server on this port
--
-- RETURNS: 0 on success, or -1 if unsuccessfully opened. 
--
-- NOTES:
-- 		Init is called once the unmanaged client has been instantiated, and it attempts to connect to the given server
--		
--------------------------------------------------------------------------------------------------------------*/
		public Int32 Init(string ipaddr, ushort port)
		{
			CAddr addr = new CAddr (ipaddr);
			server = new EndPoint (ipaddr, port);
			rcvEndPoint = new EndPoint ();
			Int32 err = ServerLibrary.Client_initClient(connection, server);
			return err;
		}

/*------------------------------------------------------------------------------------------------------------
-- FUNCTION: Poll
--
-- DATE: March 7th 2018
--
-- REVISIONS:
--
-- DESIGNER: Delan Elliot, Wilson Hu, Jeff Chou, Jeremy Lee
--
-- PROGRAMMER: Delan Elliot
--
-- INTERFACE: Int32 Poll()
--
-- RETURNS: True when data is waiting, false otherwise
--
-- NOTES:
-- 		Call Poll on the unmanaged client and returns true if data is waiting to be recevied, and false otherwise.
--------------------------------------------------------------------------------------------------------------*/
		public bool Poll()
		{
			Int32 p = ServerLibrary.Client_PollSocket (connection);
			return Convert.ToBoolean (p);
		}


/*------------------------------------------------------------------------------------------------------------
-- FUNCTION: Select
--
-- DATE: March 7th 2018
--
-- REVISIONS:
--
-- DESIGNER: Delan Elliot, Wilson Hu, Jeff Chou, Jeremy Lee
--
-- PROGRAMMER: Jeremy Lee
--
-- INTERFACE: Int32 Select()
--
-- RETURNS: True when data is waiting, false otherwise
--
-- NOTES:
-- 		Call Select on the unmanaged client and returns true if data is waiting to be recevied, and false otherwise.
--------------------------------------------------------------------------------------------------------------*/
        public bool Select()
        {
            Int32 s = ServerLibrary.Client_SelectSocket(connection);
            return Convert.ToBoolean(s);
        }


/*------------------------------------------------------------------------------------------------------------
-- FUNCTION: Recv
--
-- DATE: March 7th 2018
--
-- REVISIONS:
--
-- DESIGNER: Delan Elliot, Wilson Hu, Jeff Chou, Jeremy Lee
--
-- PROGRAMMER: Delan Elliot
--
-- INTERFACE: Int32 Recv(byte[] buffer, Int32 len)
--				buffer: the byte array to write received data into
--				len: the max length of received data (the length of the buffer)
--
-- RETURNS: the number of bytes received
--
-- NOTES:
--		The client does a "Connect" call on the UDP socket so that the address only has to be provided once. 
--		A fixed reference to the byte array is created so that C++ can access the binary data with being garbage
--		collected. 
--------------------------------------------------------------------------------------------------------------*/
		public Int32 Recv(byte[] buffer, Int32 len)
		{
			fixed(byte* tmpBuf = buffer) 
			{
				UInt32 bufLen = Convert.ToUInt32 (len);
				Int32 length = ServerLibrary.Client_recvBytes(connection, new IntPtr(tmpBuf), bufLen);
				return length;
			}
		}


/*------------------------------------------------------------------------------------------------------------
-- FUNCTION: Send
--
-- DATE: March 7th 2018
--
-- REVISIONS:
--
-- DESIGNER: Delan Elliot, Wilson Hu, Jeff Chou, Jeremy Lee
--
-- PROGRAMMER: Delan Elliot
--
-- INTERFACE: Int32 Recv(byte[] buffer, Int32 len)
--				buffer: the byte array to send data into
--				len: the length of the data to send 
--
-- RETURNS: the number of bytes received
--
-- NOTES:
-- 		Creates a fixed refernce to ensure the ref is not changed by the garbage collector during execution. 
--------------------------------------------------------------------------------------------------------------*/
		public Int32 Send(byte[] buffer, Int32 len)
		{
			uint bufLen = Convert.ToUInt32 (len);

			fixed( byte* tmpBuf = buffer) 
			{
				Int32 ret = ServerLibrary.Client_sendBytes (connection, new IntPtr (tmpBuf), bufLen);
				return ret;
			}

		}
	}
}

