using System;
using System.Runtime.InteropServices;
using System.Security;



namespace Networking{


internal static unsafe class ServerLibrary
{

	[DllImport("Network")]
	public static extern IntPtr Server_CreateServer();

	[DllImport("Network")]
	public static extern Int32 Server_sendBytes (IntPtr serverPtr, EndPoint ep, IntPtr buffer, UInt32 len);

	[DllImport("Network")]
	public static extern Int32 Server_recvBytes (IntPtr serverPtr, EndPoint* ep, IntPtr buffer, UInt32 len);

	[DllImport("Network")]
	public static extern Int32 Server_PollSocket(IntPtr serverPtr);

	[DllImport("Network")]
	public static extern Int32 Server_initServer (IntPtr serverPtr, ushort port);


}

}

