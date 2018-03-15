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
        public EndPoint ep;
        public byte[] buffer;
        public byte id;

        public float x;
        public float z;
        public float r;

        public int h;
    }
    static byte playerID = 1;
    static float spawnPoint = 2.0f;

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
        if (Time.time > nextTickTime)
        {
            ticks++;
            nextTickTime += tickTime;

            craftTickPacket();
            sendPacketToClients();
        }
    }

    private static void craftTickPacket()
    {
        int offset = 373;
        clientData[0] = 85;

        for (int i = 0; i < endpoints.Count; i++)
        {
            connection conn = endpoints[i];

            // New connection
            if (conn.id == 0 && playerID < 30)
            {
                conn.id = playerID;
                playerID++;
                sendInitData(ref conn);
            }

            // Update clientdata with new coordinates
            if (conn.buffer != null)
            {
                updateCoord(ref conn);
                conn.buffer = null;
            }

            endpoints[i] = conn;

            // Add player id to clientdata
            clientData[offset] = conn.id;
            offset++;
        }
    }

    private static void sendPacketToClients()
    {
        Debug.Log("send | " + byteArrayToString(clientData));
        for (int i = 0; i < endpoints.Count; i++)
        {
            if (endpoints[i].id != 0)
            {
                server.Send(endpoints[i].ep, clientData, MAX_BUFFER_SIZE);
            }
        }
    }

    public static void recvThrdFunc()
    {

        byte[] recvBuffer = new byte[MAX_BUFFER_SIZE];
        Int32 numRead;
        EndPoint ep = new EndPoint();

        while (running)
        {
            // If poll returns 1 (SOCKET_DATA_WAITING), there is data waiting to be read
            // If poll returns 0 (SOCKET_NODATA), there is no data waiting to be read
            if (server.Poll())
            {
                connection recvConn = new connection();
                numRead = server.Recv(ref ep, recvBuffer, MAX_BUFFER_SIZE);

                if (numRead != MAX_BUFFER_SIZE)
                {
                    Debug.Log("Failed to read from socket.");
                }
                else
                {
                    Debug.Log("recv | " + byteArrayToString(recvBuffer));
                    switch (recvBuffer[0])
                    {
                        case 69:
                            addNewClient(ep);
                            break;
                        
                        case 85:
                            saveBuffer(ep, recvBuffer);
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }

    private static void addNewClient(EndPoint ep)
    {
        connection newPlayer = new connection();
        
        if (playerID < 31)
        {
            newPlayer.ep = ep;
            newPlayer.id = 0;

            endpoints.Add(newPlayer);
        }
    }

    private static void saveBuffer(EndPoint ep, byte[] buffer)
    {
        for (int i = 0; i < endpoints.Count; i++)
        {
            if (endpoints[i].id == buffer[1])
            {
                if (ep.addr.Byte0 == endpoints[i].ep.addr.Byte0 && ep.addr.Byte1 == endpoints[i].ep.addr.Byte1
                    && ep.addr.Byte2 == endpoints[i].ep.addr.Byte2 && ep.addr.Byte3 == endpoints[i].ep.addr.Byte3)
                {
                    connection tmp = endpoints[i];
                    tmp.buffer = buffer;
                    endpoints[i] = tmp;
                }
            }
        }
    }

    //Creates a new player's information
    private static void sendInitData(ref connection conn)
    {
        clientData[0] = 0;
        clientData[373] = conn.id;

        int positionOffset = (13 + (conn.id * 8)) - 8;
        int rotationOffset = (253 + (conn.id * 4)) - 4;

        conn.x = spawnPoint * 5;
        conn.z = spawnPoint * 5;
        spawnPoint *= spawnPoint;
        conn.r = 0.0f;
        conn.h = 100;

        Array.Copy(BitConverter.GetBytes(conn.x), 0, clientData, positionOffset, 4);
        Array.Copy(BitConverter.GetBytes(conn.z), 0, clientData, positionOffset + 4, 4);
        Array.Copy(BitConverter.GetBytes(conn.r), 0, clientData, rotationOffset, 4);

        server.Send(conn.ep, clientData, MAX_BUFFER_SIZE);
    }

    // Takes the recieved coords and updates client data
    private static void updateCoord(ref connection conn)
    {
        if (!conn.buffer[0].Equals(85))
        {
            return;
        }

        if (conn.buffer[1] != conn.id)
        {
            return;
        }

        int positionOffset = (13 + (conn.id * 8)) - 8;
        int rotationOffset = (253 + (conn.id * 4)) - 4;

        Array.Copy(conn.buffer, 2, clientData, positionOffset, 4);
        Array.Copy(conn.buffer, 6, clientData, positionOffset + 4, 4);
        Array.Copy(conn.buffer, 10, clientData, rotationOffset, 4);

        conn.x = BitConverter.ToSingle(conn.buffer, 2);
        conn.z = BitConverter.ToSingle(conn.buffer, 6);
        conn.r = BitConverter.ToSingle(conn.buffer, 10);
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
