/*---------------------------------------------------------------------------------------
--	SOURCE FILE:	ServerLibrary.cs -   A C# wrapper class providing UDP server functions
--
--	PROGRAM:		game
--
--	FUNCTIONS:		 
        
--      IntPtr Server_CreateServer ();
--      Int32 Server_sendBytes (IntPtr serverPtr, EndPoint ep, IntPtr buffer, UInt32 len);        
--      Int32 Server_recvBytes (IntPtr serverPtr, EndPoint * ep, IntPtr buffer, UInt32 len);
--      Int32 Server_PollSocket (IntPtr serverPtr);
--      Int32 Server_SelectSocket (IntPtr serverPtr);
--      Int32 Server_initServer (IntPtr serverPtr, ushort port);
--      IntPtr Client_CreateClient ();
--      Int32 Client_sendBytes (IntPtr clientPtr, IntPtr buffer, UInt32 len);
--      Int32 Client_recvBytes (IntPtr clientPtr, IntPtr buffer, UInt32 len);
--      Int32 Client_PollSocket (IntPtr clientPtr);
--      Int32 Client_SelectSocket(IntPtr clientPtr);
--      Int32 Client_initClient (IntPtr clientPtr, EndPoint ep);
--      IntPtr TCPServer_CreateServer();
--      Int32 TCPServer_initServer(IntPtr serverPtr, ushort port);
--      Int32 TCPServer_acceptConnection(IntPtr serverPtr, EndPoint * ep);
--      Int32 TCPServer_sendBytes(IntPtr serverPtr, Int32 clientSocket, IntPtr data, UInt32 len);
--      Int32 TCPServer_recvBytes(IntPtr serverPtr, Int32 clientSocket, IntPtr data, UInt32 len);
--      Int32 TCPServer_closeClientSocket(Int32 clientSocket);
--      Int32 TCPServer_closeListenSocket(Int32 sockfd);
--      IntPtr TCPClient_CreateClient();
--      Int32 TCPClient_initClient(IntPtr serverPtr, EndPoint ep);
--
--	DATE:			March 10th, 2018
--
--	REVISIONS:		(Date and Description)
--
--	DESIGNERS:		Delan Elliot, Wilson Hu, Jeff Chou, Jeremy Lee
--
--	PROGRAMMER:		Delan Elliot
--
--	NOTES:
--		This file is simply a header file to interface with the Networking shared object 
--      library. It includes all the C++ function with the appropriate C# parameter and 
--      return types. 
--		
--      The file imports "Network", which is interpreted as such:
--          Unity:  GAME_ROOT/Assets/Plugins/Network.so
--          Mono: /usr/lib/libNetwork.so
--
--      The library is copied to those locations after compilation. 
---------------------------------------------------------------------------------------*/

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Networking {

    internal static unsafe class ServerLibrary {

        [DllImport ("Network")]
        public static extern IntPtr Server_CreateServer ();

        [DllImport ("Network")]
        public static extern Int32 Server_sendBytes (IntPtr serverPtr, EndPoint ep, IntPtr buffer, UInt32 len);

        [DllImport ("Network")]
        public static extern Int32 Server_recvBytes (IntPtr serverPtr, EndPoint * ep, IntPtr buffer, UInt32 len);

        [DllImport ("Network")]
        public static extern Int32 Server_PollSocket (IntPtr serverPtr);

        [DllImport ("Network")]
        public static extern Int32 Server_SelectSocket (IntPtr serverPtr);

        [DllImport ("Network")]
        public static extern Int32 Server_initServer (IntPtr serverPtr, ushort port);

        [DllImport ("Network")]
        public static extern IntPtr Client_CreateClient ();

        [DllImport ("Network")]
        public static extern Int32 Client_sendBytes (IntPtr clientPtr, IntPtr buffer, UInt32 len);

        [DllImport ("Network")]
        public static extern Int32 Client_recvBytes (IntPtr clientPtr, IntPtr buffer, UInt32 len);

        [DllImport ("Network")]
        public static extern Int32 Client_PollSocket (IntPtr clientPtr);

        [DllImport("Network")]
        public static extern Int32 Client_SelectSocket(IntPtr clientPtr);

        [DllImport ("Network")]
        public static extern Int32 Client_initClient (IntPtr clientPtr, EndPoint ep);

        [DllImport("Network")]
        public static extern IntPtr TCPServer_CreateServer();

        [DllImport("Network")]
        public static extern Int32 TCPServer_initServer(IntPtr serverPtr, ushort port);

        [DllImport("Network")]
        public static extern Int32 TCPServer_acceptConnection(IntPtr serverPtr, EndPoint * ep);

        [DllImport("Network")]
        public static extern Int32 TCPServer_sendBytes(IntPtr serverPtr, Int32 clientSocket, IntPtr data, UInt32 len);

        [DllImport("Network")]
        public static extern Int32 TCPServer_recvBytes(IntPtr serverPtr, Int32 clientSocket, IntPtr data, UInt32 len);

        [DllImport("Network")]
        public static extern Int32 TCPServer_closeClientSocket(Int32 clientSocket);

        [DllImport("Network")]
        public static extern Int32 TCPServer_closeListenSocket(Int32 sockfd);

        [DllImport("Network")]
        public static extern IntPtr TCPClient_CreateClient();

        [DllImport("Network")]
        public static extern Int32 TCPClient_initClient(IntPtr serverPtr, EndPoint ep);

        [DllImport("Network")]
        public static extern Int32 TCPClient_sendBytes(IntPtr serverPtr, IntPtr data, UInt32 len);

        [DllImport("Network")]
        public static extern Int32 TCPClient_recvBytes(IntPtr serverPtr, IntPtr data, UInt32 len);

        [DllImport("Network")]
        public static extern Int32 TCPClient_closeConnection(Int32 sockfd);

    }

}
