using System;

using System.Runtime.InteropServices;
using System.Security;

namespace Client
{
	internal static unsafe class ServerLibrary
	{

		[DllImport("Network")]
		public static extern IntPtr Server_CreateServer();

		[DllImport("Network")]
		public static extern Int32 Server_sendBytes(IntPtr serverPtr, EndPoint ep, IntPtr buffer, UInt32 len);

		[DllImport("Network")]
		public static extern Int32 Server_recvBytes(IntPtr serverPtr, EndPoint* ep, IntPtr buffer, UInt32 len);

		[DllImport("Network")]
		public static extern Int32 Server_PollSocket(IntPtr serverPtr);

		[DllImport("Network")]
		public static extern Int32 Server_initServer(IntPtr serverPtr, ushort port);

		[DllImport("Network")]
		public static extern IntPtr Client_CreateClient();

		[DllImport("Network")]
		public static extern Int32 Client_sendBytes(IntPtr clientPtr, IntPtr buffer, UInt32 len);

		[DllImport("Network")]
		public static extern Int32 Client_recvBytes(IntPtr clientPtr, IntPtr buffer, UInt32 len);

		[DllImport("Network")]
		public static extern Int32 Client_PollSocket(IntPtr clientPtr);

		[DllImport("Network")]
		public static extern Int32 Client_initClient(IntPtr clientPtr, EndPoint ep);




	}

}

