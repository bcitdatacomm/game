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

    private static Server server;
    private static IntPtr serverInstance;
    private static bool running;
    private static Thread recvThread;

    private TerrainController terrainController;
    private static byte[] clientData = new byte[R.Net.Size.SERVER_TICK];

    private static List<connectionData> endpoints;
    static byte playerID = 1;
    static float spawnPoint = 2.0f;

    // Use this for initialization
    void Start()
    {
        server = new Server();
        server.Init(R.Net.PORT);

        // Listen for new players with tcp for a set time period or until max players have joined
        players = new List<connection>();

        // Create the terrain packet with format 1B header + 4B size as int + data
        terrainController = new TerrainController();
        while (!terrainController.GenerateEncoding()) ;

        int terrainDataLength = terrainController.CompressedData.Length;
        byte[] terrainPacket = new byte[5 + terrainDataLength];
        terrainPacket[0] = R.Net.Header.TERRAIN_DATA;
        Array.Copy(BitConverter.GetBytes(terrainDataLength), 0, terrainPacket, 1, 4);
        Array.Copy(terrainController.CompressedData, 0, terrainPacket, 5, terrainDataLength);

        endpoints = new List<connectionData>();
        recvThread = new Thread(recvThrdFunc);
        running = true;
        recvThread.Start();

        /*
         TODO: Implement this in TCP
        // Make a terrain packet (byte array) with encoded
        byte[] terrainPacket = new byte[1200];endpoints[i]

        getitems(player.Count, terrainController.occupiedPositions);
        itemData = getitems.pcktarray;

        // Send terrain data and item spawn data to all clients

        recvThread = new Thread(recvThrdFunc);
        running = true;
        recvThread.Start();

        // Set the game timer
        // Start the game timer
        */
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
        int offset = R.Net.Offset.PLAYER_IDS;
        clientData[0] = R.Net.Header.TICK;

        for (int i = 0; i < players.Count; i++)
        {
            connectionData conn = endpoints[i];

            // New connection
            if (conn.id == 0 && playerID < 30)
            {
                conn.id = playerID;
                playerID++;
                sendInitData(conn);
            }

            // Update clientdata with new coordinates
            if (conn.buffer != null)
            {
                updateCoord(conn);
                conn.buffer = null;
            }

            players[i] = conn;

            // Add player id to clientdata
            clientData[offset] = conn.id;
            offset++;
        }
    }

    private static void sendPacketToClients()
    {
        Debug.Log("send | " + byteArrayToString(clientData));
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].id != 0)
            {
                server.Send(players[i].ep, clientData, R.Net.Size.SERVER_TICK);
            }
        }
    }

    public static void recvThrdFunc()
    {

        byte[] recvBuffer = new byte[R.Net.Size.CLIENT_TICK];
        Int32 numRead;
        EndPoint ep = new EndPoint();

        while (running)
        {
            // If poll returns 1 (SOCKET_DATA_WAITING), there is data waiting to be read
            // If poll returns 0 (SOCKET_NODATA), there is no data waiting to be read
            if (server.Poll() == 1)
            {
                numRead = server.Recv(&ep, recvBuffer, R.Net.Size.CLIENT_TICK);

                if (numRead != R.Net.Size.CLIENT_TICK)
                {
                    Debug.Log("Failed to read from socket.");
                }
                else
                {
                    Debug.Log("recv | " + byteArrayToString(recvBuffer));
                    switch (recvBuffer[0])
                    {
                        case R.Net.Header.NEW_CLIENT:
                            addNewClient(ep);
                            break;
                        
                        case R.Net.Header.TICK:
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
        connectionData newPlayer = new connectionData();
        
        if (playerID < 31)
        {
            newPlayer.ep = ep;
            newPlayer.id = 0;

            players.Add(newPlayer);
        }
    }

    private static void saveBuffer(EndPoint ep, byte[] buffer)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].id == buffer[1])
            {
                if (ep.addr.Byte0 == players[i].ep.addr.Byte0 && ep.addr.Byte1 == players[i].ep.addr.Byte1
                    && ep.addr.Byte2 == players[i].ep.addr.Byte2 && ep.addr.Byte3 == players[i].ep.addr.Byte3)
                {
                    connectionData tmp = endpoints[i];
                    tmp.buffer = buffer;
                    players[i] = tmp;
                }
            }
        }
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
