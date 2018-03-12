using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Networking;


/*
* This is the client version of the networking test program.
*
* To use this, create a Server prefab and attach this file as a script.
* This test works with an echo-server implementation that is also run in Unity.
*
* It creates a single thread to read the client-EndPoint (socket) and
* prints the received buffer to the Unity console.
*
* It sends a 600B packet every time Update is called.
*/
public class unityClientTest : MonoBehaviour {

	public static int SOCKET_NODATA = 0;
	public static int SOCKET_DATA_WAITING = 1;
	private static int MAX_BUFFER_SIZE 	= 1200;
	private static string destIP 		= "192.168.0.13";
	private ushort portNo	 			= 9999;
	private static bool running;
	byte i								= 65;

	private Client client;

	/*
    * Start initializes the Client object and recvThread.
    */
	void Start () {
		Debug.Log ("Start Client-Send test");
		Int32 result;
		byte[] sendBuffer = new Byte[MAX_BUFFER_SIZE];

		Thread recvThread;

		if (client == null)
		{
			client = new Client ();
			client.Init (destIP, portNo);
		}

		// This is probably redundant.
		result = client.Init (destIP, portNo);

		if (result != 0)
		{
			Debug.Log ("Failed to initialize socket.");
		}

		// Starts the recv thread (listens for the echo)
		running = true;
		recvThread = new Thread (recvThrdFunc);
		recvThread.Start();

	} // End of Start()

	// Update is called once per frame
	void Update () {
		byte[] sendBuffer = new Byte[MAX_BUFFER_SIZE];
		int numSent;

		// Initializes client if Update() is called before Start()
		// Unity flipped out if I didn't initialize it here.
		if (client == null)
		{
			client = new Client ();
			client.Init (destIP, portNo);
		}

		// reset i to avoid byte overflow
		// i = 0 - 9
		if (i > 57)
		{
			i = 48;
		}

		// Write 600 of the current char to the byte array
		for (int j = 0; j < 600; j++)
		{
			sendBuffer [j] = i;
		}

		// Send data to server
		numSent = client.Send (sendBuffer, 600);

		if (numSent <= 0)
		{
			Debug.Log ("Send fail.");
		}
		Debug.Log ("Sent");
		i++;

	} // End of Update()


	// Terminate thread when we stop running.
	private void OnDisable()
	{
		Debug.Log ("DISABLED");
		running = false;
	}

	/*
    * Thread function to read data from the EndPoint.
    * Prints the received buffer to the Unity console.
    */
	private void recvThrdFunc()
	{
		byte[] recvBuffer = new byte[MAX_BUFFER_SIZE];
		Int32 numRead;
		UInt64 totalRead = 0;
		int i = 0;
		int numPollPass = 0;
		int numPollFail = 0;

		Debug.Log ("recvThread started.");

		while (running)
		{
			if (client.Poll() == SOCKET_DATA_WAITING)
			{
				Debug.Log ("Poll success.");
				numRead = client.Recv(recvBuffer, MAX_BUFFER_SIZE);
				totalRead += (UInt64) numRead;
				if (numRead <= 0)
				{
					Debug.Log ("Client Recv error");
				}
				else
				{
					Debug.Log ("Received.");

					// Collapsable debug logs
//					string contents = System.Text.Encoding.UTF8.GetString (recvBuffer);
//					Debug.Log("Received: " + contents);

//					numPollPass++;
//					if (numPollPass % 1000 == 0)
//					{
//						Debug.Log ("Successful Packs Received: " + numPollPass);
//					}

				}
			}
			else
			{
				numPollFail++;

				// Collapsable debug logs
//				if (numPollFail % 100 == 0)
//				{
//					Debug.Log("Num Poll Fails: " + numPollFail);
//					Debug.Log("Num Poll Success: " + numPollPass);
//				}
				Debug.Log("Poll Fail.");
				//System.Threading.Thread.Sleep (10);
			}
			//			if (i++ % 1000 == 0) {
			//				Debug.Log ("Total received so far: " + totalRead);
			//			}
		}
	}
}
