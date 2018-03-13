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

    private static Int32 SOCKET_NODATA = 0;
    private static Int32 SOCKET_DATA_WAITING = 1;
    private static ushort PORT_NO = 9999;
    private static int MAX_BUFFER_SIZE = 1200;
    private static Server server;
    private static IntPtr serverInstance;
    private static bool running;
    private static Thread recvThread;

    private TerrainController terrainController;
    private static byte[] clientData = new byte[MAX_BUFFER_SIZE];


    private static List<connection> endpoints;
    public struct connection
    {
        public EndPoint end;
        public byte[] recvBuffer;
        public byte connID;

        public float coordX;
        public float coordZ;
        public float rotation;

        public int playerHealth;
    }
    static byte playerID = 1;

    // Use this for initialization
    void Start()
    {
        server = new Server();
        terrainController = new TerrainController();
        while (!terrainController.GenerateEncoding()) ;
        TerrainController.Encoding encoded = terrainController.Data;

        server.Init(42069);

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

            int offset = 373;

            clientData[0] = 85;

            connection conn = new connection();
            for (int i = 0; i < endpoints.Count; i++)
            {
                conn = endpoints[i];

                // New connection
                if (endpoints[i].connID == 0 && playerID < 31)
                {
                    conn.connID = playerID;
                    conn.end = endpoints[i].end;
                    conn.playerHealth = 100;

                    endpoints[i] = conn;
                    sendInitData(conn.connID, conn.end, ref conn.coordX, ref conn.coordZ, ref conn.rotation);
                    playerID++;
                }

                // Update clientdata with new coordinates
                if (endpoints[i].recvBuffer != null)
                {
                    updateCoord(conn.recvBuffer, conn.connID, ref conn.coordX, ref conn.coordZ, ref conn.rotation);
                    conn.recvBuffer = null;
                    endpoints[i] = conn;
                }

                // Add player id to clientdata
                clientData[offset] = conn.connID;
                offset++;
            }

            // Send the packet to each client
            for (int i = 0; i < endpoints.Count; i++)
            {
                if (endpoints[i].connID != 0)
                {
                    server.Send(endpoints[i].end, clientData, MAX_BUFFER_SIZE);
                }
            }
        }
    }

    public static void recvThrdFunc()
    {
        byte[] recvBuffer = new byte[MAX_BUFFER_SIZE];
        Int32 numRead;
        EndPoint ep = new EndPoint();
        bool newConn = true;
        connection recvConn = new connection();

        while (running)
        {
            // If poll returns 1 (SOCKET_DATA_WAITING), there is data waiting to be read
            // If poll returns 0 (SOCKET_NODATA), there is no data waiting to be read
            if (server.Poll() == SOCKET_DATA_WAITING)
            {
                numRead = server.Recv(&ep, recvBuffer, MAX_BUFFER_SIZE);
                if (numRead <= 0)
                {
                    Debug.Log("Failed to read from socket.");
                }
                else
                {
                    newConn = true;
                    string contents = System.Text.Encoding.UTF8.GetString(recvBuffer);

                    for (int i = 0; i < endpoints.Count; i++)
                    {
                        // If it's in there
                        if (ep.addr.Byte0 == endpoints[i].end.addr.Byte0 && ep.addr.Byte1 == endpoints[i].end.addr.Byte1
                            && ep.addr.Byte2 == endpoints[i].end.addr.Byte2 && ep.addr.Byte3 == endpoints[i].end.addr.Byte3)
                        {
                            recvConn = endpoints[i];
                            if (recvBuffer[0].Equals(85))
                            {
                                recvConn.recvBuffer = recvBuffer;
                                endpoints[i] = recvConn;
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
                            recvConn.connID = 0;

                            endpoints.Add(recvConn);

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
    private static void sendInitData(byte pID, EndPoint ep, ref float coordX, ref float coordZ, ref float rotate)
    {
        clientData[0] = 0;
        clientData[373] = pID;
        float playerX = 0 + Convert.ToInt32(pID);
        float playerZ = 0 + Convert.ToInt32(pID);
        float rotation = 0;

        // Store player coordinates in Connection object
        coordX = playerX;
        coordZ = playerZ;
        rotate = rotation;

        int positionOffset = (13 + (pID * 8)) - 8;
        int rotationOffset = (253 + (pID * 4)) - 4;

        Buffer.BlockCopy(BitConverter.GetBytes(playerX), 0, clientData, positionOffset, 4);

        Buffer.BlockCopy(BitConverter.GetBytes(playerZ), 0, clientData, positionOffset + 4, 4);

        Buffer.BlockCopy(BitConverter.GetBytes(rotation), 0, clientData, rotationOffset, 4);

        server.Send(ep, clientData, MAX_BUFFER_SIZE);
    }

    // Takes the recieved coords and updates client data
    private static void updateCoord(byte[] recvConn, byte pID, ref float coordX, ref float coordZ, ref float rotate)
    {
        int offset = 2;
        float playerX = BitConverter.ToSingle(recvConn, offset);
        offset += 4;

        float playerZ = BitConverter.ToSingle(recvConn, offset);
        offset += 4;

        float rotation = BitConverter.ToSingle(recvConn, offset);
        offset += 4;

        // Store player coordinates in Connection object
        coordX = playerX;
        coordZ = playerZ;
        rotate = rotation;

        int positionOffset = (13 + (pID * 8)) - 8;
        int rotationOffset = (253 + (pID * 4)) - 4;

        Buffer.BlockCopy(BitConverter.GetBytes(playerX), 0, clientData, positionOffset, 4);

        Buffer.BlockCopy(BitConverter.GetBytes(playerZ), 0, clientData, positionOffset + 4, 4);

        Buffer.BlockCopy(BitConverter.GetBytes(rotation), 0, clientData, rotationOffset, 4);
    }
}
