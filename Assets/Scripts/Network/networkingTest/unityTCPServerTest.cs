using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Networking;


public unsafe class unityTCPServerTest : MonoBehaviour
{
    private const int MAX_MAP_SIZE      = 8192;
    private const int MAX_NUM_CLIENTS   = 1;
    private const ushort PORT_NO        = 42069;

    private byte[] mapBuffer = new byte[MAX_MAP_SIZE];
    private Int32[] clientArray = new Int32[MAX_NUM_CLIENTS];
    private EndPoint[] epArray = new EndPoint[MAX_NUM_CLIENTS];
    private int numClients = 0;
    private int serverSockFd;

    private TCPServer server;
    private bool listening = false;


    void Start()
    {
        Int32 result;
        Thread listenThread;

        server = new TCPServer();
        serverSockFd = server.Init(PORT_NO);
        if (serverSockFd <= 0)
        {
            Debug.Log("Error initializing socket");
        }
        else
        {
            listenThread = new Thread(listenThrdFunc);
            listening = true;
            listenThread.Start();

            // Fills mapBuffer
            byte j = 65;
            for (int i = 0; i < MAX_MAP_SIZE; i++)
            {
               if (j >= 123)
               {
                   j = 65;
               }
               mapBuffer[i] = j;
               j++;
            }
        }
    }

    void Update()
    {

    }

    private void OnDisable()
    {
        listening = false;
        server.CloseListenSocket(serverSockFd);
    }

    private void listenThrdFunc()
    {
        EndPoint ep = new EndPoint();
        Int32 clientSockFD;
        Thread[] transmitThrdArray = new Thread[MAX_NUM_CLIENTS];
        Int32 numSent;


        while (listening && numClients < MAX_NUM_CLIENTS)
        {
            Debug.Log("Listening for connections");
            clientSockFD = server.AcceptConnection(ref ep);
            // accept fails
            if (clientSockFD == -1)
            {
                Debug.Log("Error accepting connection. SAD!");
            }
            // accept succeeds
            else
            {
                Debug.Log("I got a client!");
                clientArray[numClients] = clientSockFD;
                numClients++;
            }

        }
        if (numClients >= MAX_NUM_CLIENTS)
        {
            Debug.Log("Exited max clients");
        }
        else
        {
            Debug.Log("Force Exit");
        }

        for (int i = 0; i < numClients; i++)
        {
            numSent = server.Send(clientArray[i], mapBuffer, MAX_MAP_SIZE);
        }
        Debug.Log("Finished Sending");
        Debug.Log("Closing client sockets");
        for (int i = 0; i < numClients; i++)
        {
            server.CloseClientSocket(clientArray[i]);
        }
    }

    private void transmitThrdFunc(object clientSockID)
    {

    }
}
