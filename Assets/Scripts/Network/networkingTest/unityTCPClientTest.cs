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
public class unityTCPClientTest : MonoBehaviour {

	public static int SOCKET_NODATA 		= 0;
	public static int SOCKET_DATA_WAITING 	= 1;
	private static int MAX_BUFFER_SIZE 		= 8192;
	private static string destIP 			= "142.232.18.94";
	private ushort portNo	 				= 42069;
	private EndPoint ep;
	private static bool running;
	byte i									= 65;

	private TCPClient client;

	/*
    * Start initializes the Client object and recvThread.
    */
	void Start () {
		Debug.Log ("Start Client-Send test");
		Int32 result;
		byte[] sendBuffer = new Byte[MAX_BUFFER_SIZE];

		Thread recvThread;

		client = new TCPClient ();
		ep = new EndPoint (destIP, portNo);
		result = client.Init (ep);
        Debug.Log("Init result: " + result);
        if (result == 0)
        {
            // Starts the recv thread (listens for the echo)
            running = true;
            recvThread = new Thread(recvThrdFunc);
            recvThread.Start();
        }
		

	} // End of Start()

	// Update is called once per frame
	void Update () {
	} // End of Update()


	// Terminate thread when we stop running.
	private void OnDisable()
	{
		Debug.Log ("DISABLED");
        running = false;
        int closeresult;
        closeresult = client.CloseConnection();
        Debug.Log("Close result: " + closeresult);
	}

	/*
    * Thread function to read data from the EndPoint.
    * Prints the received buffer to the Unity console.
    */
	private void recvThrdFunc()
	{
		byte[] recvBuffer = new byte[MAX_BUFFER_SIZE];
		Int32 numRecv;
		UInt64 totalRead = 0;
       
        int numSent;

		Debug.Log ("recvThread started.");


        Debug.Log("Entered loop.");

		numRecv = client.Recv (recvBuffer, MAX_BUFFER_SIZE);
        if (numRecv > 0)
        {
            string contents = System.Text.Encoding.UTF8.GetString(recvBuffer);
            Debug.Log("Received: " + contents);
        }
	


	}
}
