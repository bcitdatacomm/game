using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Networking;

/**
 * Simple echo server implementation using our Networking library.
 * This class is used as a Unity script which can be attached to a prefab for 
 * testing. (I used Server)
 * 
 * Throughput seems to be limited by Client's Update rate, ~200 updates sent/received per second 
 */
public unsafe class unityTCPEchoServer : MonoBehaviour {

	private static int MAX_BUFFER_SIZE = 8192;
	public static int SOCKET_NODATA = 0;
	public static int SOCKET_DATA_WAITING = 1;

	private TCPServer server;
	private ushort portNo = 9999;
	private bool running;
	private EndPoint ep;


	/*
	 * Creates and initializes the Server object and recv thread.
	 */ 
	void Start ()
	{
		Debug.Log("Starting Server-Echo test");

		// Creates a blank EndPoint which will be filled by the Server.Recv call.
		ep = new EndPoint();
		Thread recvThread;
		Int32 result;

		// Create and initialize a server.
		server = new TCPServer();
		result = server.Init(portNo);
		if (result != 0)
		{
			Debug.Log("Failed to initialize socket");
		}
			
		recvThread = new Thread(recvThrdFunc);
		running = true;
		recvThread.Start();

	} // End of Start()


	//Terminate thread when we stop running.
	private void OnDisable()
	{
		Debug.Log("DISABLED.");
		running = false;
	}

	/*
     * Thread function to read incoming packets.
     * 
     * Currently works when tested using lab computers.
     * Initially, the server would recv only the first packet. 
     * 
     * Sleep call was removed, with successful recv on the server side.
     *
     */
	private void recvThrdFunc()
	{
		bool result;
		Int32 totalRecv = 0;
		Int32 numRecvPass = 0;
		Int32 numPollFail = 0;
		Int32 numSent = 0;
		Int32 numBytesSent;
		Int32 client;

		byte[] recvBuffer = new byte[MAX_BUFFER_SIZE];
		Int32 numRecv;

		while (running)
		{
			
			client = server.AcceptConnection (ref ep);
			if (client == -1)
			{
				Debug.Log ("Fuck I couldn't accept fuck.");
			}
			server.Recv (client, recvBuffer, MAX_BUFFER_SIZE);
			string contents = System.Text.Encoding.UTF8.GetString (recvBuffer);
			Debug.Log ("Received: " + contents);
			numSent = server.Send (client, recvBuffer, MAX_BUFFER_SIZE);
			if (numSent > 0)
			{
				Debug.Log ("Sent.");
			}

		}
	} // End of recvThrdFunc()

	// Update is called once per frame
	void Update ()
	{

	}
}
