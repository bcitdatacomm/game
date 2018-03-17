using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Networking;
using System.Threading;

// This is the core script for the server.

public unsafe class gameServer : MonoBehaviour
{
    // Member Data
    private static float nextTickTime = 0.0f;
    private static int ticksPerSecond = 64;
    private float tickTime = (1 / ticksPerSecond);
    private int ticks = 0;
	private static int currClients = 0;

    private static Server server;
    private static IntPtr serverInstance;
    private static bool running;
    private static Thread recvThread;

    private TerrainController terrainController;
    private static byte[] clientData = new byte[R.Net.Size.SERVER_TICK];

	private static connectionData[] endpoints;
    static byte playerID = 1;
    static float spawnPoint = 2.0f;

    // Use this for initialization
    void Start()
    {
        server = new Server();
        terrainController = new TerrainController();
        while (!terrainController.GenerateEncoding()) ;
        TerrainController.Encoding encoded = terrainController.Data;

        server.Init(R.Net.PORT);

		endpoints = new connectionData[31];
        recvThread = new Thread(recvThrdFunc);
        running = true;
		recvThread.IsBackground = true;
        recvThread.Start();

        /*
         TODO: Implement this in TCP
        // Make a terrain packet (byte array) with encoded
        byte[] terrainPacket = new byte[1200];endpoints[i]

        // Set header and amount of packet taken up
        terrainPacket[0] = 15;
        int packetSize = 1;

        long tileWidth = this.terrainController.Width;
        long tileLength = this.terrainController.Length;

        // Wait for all IDs to be echoed back as ACK, retransit ID on timeout
        */

        // Set the game timer
        // Start the game timer

    }

    void Update()
    {
		if (Time.time > nextTickTime && currClients > 0)
        {
            ticks++;
            nextTickTime += tickTime;

			craftTickPacket ();
			sendPacketToClients ();
        }
    }

    private static void craftTickPacket()
    {
        int offset = R.Net.Offset.PLAYER_IDS;
        clientData[0] = R.Net.Header.TICK;

        for (int i = 1; i <= currClients; i++)
        {
			byte id = endpoints [i].id;

            // Update clientdata with new coordinates
			if (endpoints [i].buffer != null)
            {
				updateCoord(endpoints [i]);
				endpoints [i].buffer = null;
            }


            // Add player id to clientdata
			clientData[offset] = id;
            offset++;
        }
    }

    private static void sendPacketToClients()
    {
        //Debug.Log("send | " + byteArrayToString(clientData));
        for (int i = 1; i <= currClients; i++)
        {
            if (endpoints[i].id != 0)
            {
                server.Send(endpoints[i].ep, clientData, R.Net.Size.SERVER_TICK);
            }
        }
    }

	public static void recvThrdFunc ()
	{

		byte[] recvBuffer = new byte[R.Net.Size.CLIENT_TICK];
		Int32 numRead;
		EndPoint ep = new EndPoint ();

		while (running) {
			// If poll returns 1 (SOCKET_DATA_WAITING), there is data waiting to be read
			// If poll returns 0 (SOCKET_NODATA), there is no data waiting to be read
			//Debug.Log ("ey");
			numRead = server.Recv (ref ep, recvBuffer, R.Net.Size.CLIENT_TICK);

			if (numRead != R.Net.Size.CLIENT_TICK) {
				//Debug.Log ("Failed to read from socket.");
			} else {
				//Debug.Log ("recv | " + byteArrayToString (recvBuffer));
				switch (recvBuffer [0]) {
				case R.Net.Header.NEW_CLIENT:
					addNewClient (ep);
					break;
                    
				case R.Net.Header.TICK:
					saveBuffer (ep, recvBuffer);
					break;

				default:
					break;
				}
			}
		}
				
        	
	}

    private static void addNewClient(EndPoint ep)
    {
        connectionData newPlayer = new connectionData();
        
        if (playerID < 31)
        {
            newPlayer.ep = ep;
			newPlayer.id = playerID;

			endpoints[playerID] = newPlayer;
			sendInitData (endpoints[playerID]);

			playerID++;
			currClients++;
        }
    }

    private static void saveBuffer(EndPoint ep, byte[] buffer)
    {
		endpoints [buffer [1]].buffer = buffer;
    }

    //Creates a new player's information
    private static void sendInitData(connectionData conn)
    {
        clientData[0] = 0;
        clientData[R.Net.Offset.PLAYER_IDS] = conn.id;

        int positionOffset = (R.Net.Offset.PLAYER_POSITIONS + (conn.id * 8)) - 8;
        int rotationOffset = (R.Net.Offset.PLAYER_ROTATIONS + (conn.id * 4)) - 4;

        conn.x = spawnPoint * 5;
        conn.z = spawnPoint * 5;
        spawnPoint *= spawnPoint;
        conn.r = 0.0f;
        conn.h = 100;

        Array.Copy(BitConverter.GetBytes(conn.x), 0, clientData, positionOffset, 4);
        Array.Copy(BitConverter.GetBytes(conn.z), 0, clientData, positionOffset + 4, 4);
        Array.Copy(BitConverter.GetBytes(conn.r), 0, clientData, rotationOffset, 4);

        server.Send(conn.ep, clientData, R.Net.Size.SERVER_TICK);
    }

    // Takes the recieved coords and updates client data
    private static void updateCoord(connectionData conn)
    {
        if (!conn.buffer[0].Equals(85))
        {
            return;
        }

        if (conn.buffer[R.Net.Offset.PID] != conn.id)
        {
            return;
        }

        int positionOffset = (R.Net.Offset.PLAYER_POSITIONS + (conn.id * 8)) - 8;
        int rotationOffset = (R.Net.Offset.PLAYER_ROTATIONS + (conn.id * 4)) - 4;

        Array.Copy(conn.buffer, R.Net.Offset.X, clientData, positionOffset, 4);
        Array.Copy(conn.buffer, R.Net.Offset.Z, clientData, positionOffset + 4, 4);
        Array.Copy(conn.buffer, R.Net.Offset.R, clientData, rotationOffset, 4);

        conn.x = BitConverter.ToSingle(conn.buffer, R.Net.Offset.X);
        conn.z = BitConverter.ToSingle(conn.buffer, R.Net.Offset.Z);
        conn.r = BitConverter.ToSingle(conn.buffer, R.Net.Offset.R);
    }

    static string byteArrayToString(byte[] ba)
    {
        StringBuilder sb = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
        {
            sb.AppendFormat("{0:x2}", b);
        }
        return sb.ToString();
    }
}
