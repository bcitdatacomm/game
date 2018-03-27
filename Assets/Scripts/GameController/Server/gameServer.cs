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
    private float nextTickTime = 0.0f;
    private static int ticksPerSecond = 32;
    private float tickTime = (1 / ticksPerSecond);
    private int ticks = 0;

    private static Mutex mutex = new Mutex();

    private static Int32 SOCKET_NODATA = 0;
    private static Int32 SOCKET_DATA_WAITING = 1;
    private static ushort PORT_NO = 9999;
    private static Server server;
    private static IntPtr serverInstance;
    private static bool running;
    private static Thread recvThread;

    private TerrainController terrainController;
    private static byte[] clientData = new byte[R.Net.Size.SERVER_TICK];


    private static List<connection> endpoints;
    public struct connection
    {
        public EndPoint end;
        public byte connID;
    }
    static byte playerID = 1;

    // Use this for initialization
    void Start()
    {
        server = new Server();
        terrainController = new TerrainController();
        while (!terrainController.GenerateEncoding()) ;
        TerrainController.Encoding encoded = terrainController.Data;

        server.Init(R.Net.PORT);

        endpoints = new List<connection>();
        recvThread = new Thread(recvThrdFunc);
        running = true;
        recvThread.Start();

        /*
         TODO: Implement this in TCP
        // Make a terrain packet (byte array) with encoded
        byte[] terrainPacket = new byte[1200];

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
        if (Time.time > nextTickTime)
        {
            ticks++;
            nextTickTime += tickTime;
            
            mutex.WaitOne();
            clientData[0] = R.Net.Header.TICK;

            // Send the packet to each client
            foreach (connection conn in endpoints)
            {
                server.Send(conn.end, clientData, R.Net.Size.SERVER_TICK);
            }
            mutex.ReleaseMutex();

        }
    }

    public static void recvThrdFunc()
    {
        byte[] recvBuffer = new byte[R.Net.Size.CLIENT_TICK];
        Int32 numRead;
        EndPoint ep = new EndPoint();
        bool newConn = true;
        connection recvConn;

        while (running)
        {
            // If poll returns 1 (SOCKET_DATA_WAITING), there is data waiting to be read
            // If poll returns 0 (SOCKET_NODATA), there is no data waiting to be read
            if (server.Poll())
            {
                numRead = server.Recv(ref ep, recvBuffer, R.Net.Size.CLIENT_TICK);
                if (numRead <= 0)
                {
                    Debug.Log("Failed to read from socket.");
                }
                else
                {
                    newConn = true;
                    string contents = System.Text.Encoding.UTF8.GetString(recvBuffer);

                    foreach (connection conn in endpoints)
                    {
                        // If it's in there
                        if (ep.addr.Byte0 == conn.end.addr.Byte0 && ep.addr.Byte1 == conn.end.addr.Byte1
                            && ep.addr.Byte2 == conn.end.addr.Byte2 && ep.addr.Byte3 == conn.end.addr.Byte3)
                        {
                            recvConn = conn;
                            if (recvBuffer[0].Equals(R.Net.Header.TICK))
                            {
                                UpdateCoord(recvConn.connID, recvBuffer);
                            }
                            newConn = false;
                        }
                    }

                    // Add the new connection
                    if (newConn == true)
                    {
                        if (playerID < 31)
                        {
                            recvConn.end = ep;
                            recvConn.connID = playerID;
                            endpoints.Add(recvConn);
                            SendInitData(playerID, ep);
                            playerID++;

                            Debug.Log("New client added");

                            string debug = "";
                            foreach (connection c in endpoints)
                            {
                                debug += c.connID + " ";
                            }

                            Debug.Log(debug);
                        }
                    }

                }
            }
        }
    }

    //Creates a new player's information
    private static void SendInitData(byte pID, EndPoint ep)
    {
        clientData[0] = 0;
        byte[] newPlayer = new byte[R.Net.Size.SERVER_TICK];

        float playerx = 0 + playerID;
        float playerz = 0 + playerID;
        float rotation = 0;

        // Creates the player init packet
        newPlayer[0] = R.Net.Header.INIT_PLAYER;
        newPlayer[1] = pID;
        
        server.Send(ep, newPlayer, R.Net.Size.SERVER_TICK);

        mutex.WaitOne();

        // Find the offset to add the player to clientdata
        int offset = R.Net.Offset.PLAYERS;
        while (clientData[offset] != 0)
        {
            offset += R.Net.Size.PLAYER_DATA;
        }

        // Sets the player ID
        clientData[offset] = pID;
        offset++;

        // sets the coordinates for each player connected
        Buffer.BlockCopy(BitConverter.GetBytes(playerx), 0, clientData, offset, 4);
        offset += 4;

        Buffer.BlockCopy(BitConverter.GetBytes(playerz), 0, clientData, offset, 4);
        offset += 4;

        Buffer.BlockCopy(BitConverter.GetBytes(rotation), 0, clientData, offset, 4);
        offset += 4;

        mutex.ReleaseMutex();
    }

    // Takes the recieved coords and updates client data
    private static void UpdateCoord(byte pID, byte[] recvConn)
    {
        int offset = R.Net.Offset.X;
        float playerX = BitConverter.ToSingle(recvConn, offset);

        offset = R.Net.Offset.Z;
        float playerZ = BitConverter.ToSingle(recvConn, offset);

        offset = R.Net.Offset.R;
        float rotation = BitConverter.ToSingle(recvConn, offset);

        offset = R.Net.Offset.PLAYERS;
        // Find the existing player in the array
        while (clientData[offset] != pID)
        {
            offset += R.Net.Size.PLAYER_DATA;
        }

        mutex.WaitOne();

        offset += R.Net.Offset.Player.X;
        Buffer.BlockCopy(BitConverter.GetBytes(playerX), 0, clientData, offset, 4);

        offset += 4;
        Buffer.BlockCopy(BitConverter.GetBytes(playerZ), 0, clientData, offset, 4);

        offset += 4;
        Buffer.BlockCopy(BitConverter.GetBytes(rotation), 0, clientData, offset, 4);
        mutex.ReleaseMutex();
    }
}