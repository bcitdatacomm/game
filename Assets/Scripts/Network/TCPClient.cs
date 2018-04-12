/************************************************************************************
* SOURCE FILE:	TCPClient.cs
* 
* PROGRAM:		game
* 
* FUNCTIONS:	TCPClient()
* 				Send()
* 				Recv()
* 				Init()
* 				CloseConnection()
* 
* DATE:			Mar. 14, 2018
* 
* REVISIONS:	Mar. 14, 2018
* 
* DESIGNER:		Delan Elliot, Wilson Hu, Jeremy Lee, Jeff Chou
* 
* PROGRAMMER:	Delan Elliot, Wilson Hu
* 
* NOTES:
* This class is in the Networking namespace. 
* The class contains the wrapper functions which allow the game
* to use the networking library.
**********************************************************************************/
using System;
using UnityEngine;

namespace Networking
{
	public unsafe class TCPClient
	{
		private IntPtr tcpClient;
        private Int32 clientSocket;

		/************************************************************************************
		FUNCTION:	TCPClient

		DATE:		Mar. 14, 2018

		REVISIONS:

		DESIGNER:	Delan Elliot

		PROGRAMMER:	Delan Elliot

		INTERFACE:	TCPClient()

		NOTES:
		This constructor creates the library's TCPClient object. 
		**********************************************************************************/
		public TCPClient()
		{
			tcpClient = ServerLibrary.TCPClient_CreateClient();
		}

		/************************************************************************************
		FUNCTION:	Send

		DATE:		Mar. 14, 2018

		REVISIONS:

		DESIGNER:	Delan Elliot

		PROGRAMMER:	Delan Elliot

		INTERFACE:	public Int32 Send(byte[] buffer, Int32 len)
						byte[] buffer: buffer to send
						Int32 len: length of butter to send

		RETURNS:	Returns the number of bytes sent by the library's client send function

		NOTES:
		This function is the C# wrapper for the library's TCPClient::sendBytes function.
		**********************************************************************************/
		public Int32 Send(byte[] buffer, Int32 len)
		{
			fixed( byte* tmpBuf = buffer)
			{
				UInt32 bufLen = Convert.ToUInt32 (len);
				Int32 ret = ServerLibrary.TCPClient_sendBytes(tcpClient, new IntPtr(tmpBuf), bufLen);
				return ret;
			}
		}

		/************************************************************************************
		FUNCTION:	Recv

		DATE:		Mar. 2018

		REVISIONS:	

		DESIGNER:	Delan Elliot

		PROGRAMMER:	Delan Elliot

		INTERFACE:	public Int32 Recv(byte[] buffer, Int32 len)
						byte[] buffer: receive buffer
						Int32 len: number of bytes to read into the receive buffer

		RETURNS:	Returns number of bytes received by the library's client recv function

		NOTES:
		This function is the C# wrapper for the library's TCPClient::receiveBytes function.
		
		It returns the number of bytes received.
		**********************************************************************************/
		public Int32 Recv(byte[] buffer, Int32 len)
		{
			Int32 length;
			fixed (byte* tmpBuf = buffer)
			{
				UInt32 bufLen = Convert.ToUInt32(len);
				Debug.Log("Converted length: " + len);
				length = ServerLibrary.TCPClient_recvBytes(tcpClient, new IntPtr(tmpBuf), bufLen);

				return length;
			}

		}

		/************************************************************************************
		FUNCTION:	Init

		DATE:		Mar. 14, 2018

		REVISIONS:

		DESIGNER:	Delan Elliot

		PROGRAMMER:	Delan Elliot

		INTERFACE:	public Int32 Init(EndPoint ep)
						EndPoint ep: EndPoint struct that will hold the port number and IP address of the listen socket

		RETURNS:	Returns value indicating success or failure of the library's init function
						- <=0 on failure, >0 socket descriptor on success

		NOTES:
		This function is the C# wrapper for the library's TCPClient::initializeSocket function.
		**********************************************************************************/
		public Int32 Init(EndPoint ep)
		{
			clientSocket = ServerLibrary.TCPClient_initClient(tcpClient, ep);
			return clientSocket;
		}

		/************************************************************************************
		FUNCTION:	CloseConnection

		DATE:		Mar. 14, 2018

		REVISIONS:

		DESIGNER:	Wilson Hu

		PROGRAMMER:	Wilson Hu

		INTERFACE:	public Int32 CloseConnection(Int32 sockfd)
						Int32 sockfd: socket file descriptor

		RETURNS:	Returns the result of the library closeConnection call
						- 0 on success, -1 on failure

		NOTES:
		This function is the C# wrapper for the library's TCPClient::closeConnection function.
		**********************************************************************************/
		public Int32 CloseConnection(Int32 sockfd)
		{
			Int32 err = ServerLibrary.TCPClient_closeConnection(sockfd);
			return err;
		}
	}
}
