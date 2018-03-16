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
public unsafe class unityEchoServer : MonoBehaviour {

    private static int MAX_BUFFER_SIZE = 1200;
    public static int SOCKET_NODATA = 0;
    public static int SOCKET_DATA_WAITING = 1;

    private Server server;
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
        server = new Server();
        result = server.Init(portNo);
        if (result != 0)
        {
            Debug.Log("Failed to initialize socket");
        }
        if (result == -5)
        {
            Debug.Log("ERR-5");
        }
        if (result == -4)
        {
            Debug.Log("ERR-4");
        }
        if (result == -3)
        {
            Debug.Log("ERR-3");
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

        byte[] recvBuffer = new byte[MAX_BUFFER_SIZE];
        Int32 numRecv;

        while (running)
        {
            result = server.Poll();
			// If data is waiting at the socket
            if (result)
            {
                Debug.Log("Poll success.");

                numRecv = server.Recv(ref ep, recvBuffer, MAX_BUFFER_SIZE);
                if (numRecv <= 0)
                {
                    Debug.Log("Received Nothing.");
                }
                else
                {
                    numRecvPass++;
                    Debug.Log("Received.");

					/*Collapsable debug logs*/
                    //string contents = System.Text.Encoding.UTF8.GetString(recvBuffer);
                    ////Debug.Log("Received: " + numRecv);
                    //Debug.Log("Contents: " + contents);
                    //Debug.Log("From EP: " + ep.addr.Byte3 + '.' + ep.addr.Byte2 + '.' + ep.addr.Byte1 + '.' + ep.addr.Byte0);

                    numBytesSent = server.Send(ep, recvBuffer, MAX_BUFFER_SIZE);
                    if (numBytesSent != 0)
                    {
                        Debug.Log("Sent.");
                    }
                }
            }
			// No data is waiting at the socket, used for testing
            else
            {
                numPollFail++;
                if (numPollFail % 100 == 0)
                {
                    Debug.Log("Poll Fail.");

					/*Collapsable debug logs*/
                    //Debug.Log("Num Poll Fails: " + numPollFail);
                    //Debug.Log("Num Recv Success: " + numRecvPass);
                }

                //System.Threading.Thread.Sleep(10);
            }

        }
    } // End of recvThrdFunc()

	// Update is called once per frame
	void Update ()
	{

	}
}
