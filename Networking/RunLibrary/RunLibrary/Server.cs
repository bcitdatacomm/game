using System;
using RunLibrary;
using System.Runtime.InteropServices;
using System.Security;

internal static unsafe class Server
{

		[DllImport("Library.so")]
		public static extern IntPtr Server_CreateServer();

		[DllImport("Library.so")]
		public static extern Int32 Server_sendBytes (IntPtr serverPtr, EndPoint ep, IntPtr buffer, UInt32 len);

		[DllImport("Library.so")]
		public static extern Int32 Server_recvBytes (IntPtr serverPtr, EndPoint* ep, IntPtr buffer, UInt32 len);

		[DllImport("Library.so")]
		public static extern Int32 Server_PollSocket(IntPtr serverPtr);

		[DllImport("Library.so")]
		public static extern Int32 Server_initServer (IntPtr serverPtr, ushort port);


	}


