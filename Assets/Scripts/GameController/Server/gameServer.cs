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
    byte[] toBroadcast = new byte[MAX_BUFFER_SIZE];
   
    // Some example server calls
    private EndPoint endpoint;
    private static List<connection> endpoints;
    public struct connection
    {
        public EndPoint end;
        public byte connID; 
    }
    static byte playerID = 0;

    // Use this for initialization
    void Start ()
    {
        server = new Server();
        endpoint = new EndPoint("192.168.0.18", 42069);
        terrainController = new TerrainController();
        while (!terrainController.GenerateEncoding());
        TerrainController.Encoding encoded = terrainController.Data;

        server.Init(42069);

        endpoints = new List<connection>();
        recvThread = new Thread(recvThrdFunc);
        running = true;
        recvThread.Start();


        server.Send(endpoint, Encoding.ASCII.GetBytes("ShitDick"), 8);
        // Make a terrain packet (byte array) with encoded
        byte[] terrainPacket = new byte[1200];

        // Set header and amount of packet taken up
        terrainPacket[0] = 15;
        int packetSize = 1;

        // Add data (compressedData should get merged in soon)
        //foreach (byte dataByte in encoded.compressedData)
        //{
        //    // Add all the bytes to the packet
        //    terrainPacket[packetSize] = dataByte;

        //    packetSize += 1;

        //    // If we hit the packet size, send the packet and start over
        //    if (packetSize == 1200)
        //    {
        //        // Send the packet THIS NEEDS TO BE RELIABLE (future)
        //        server.Broadcast(terrainPacket);
        //        packetSize = 1;
        //    }
        //}

        long tileWidth = this.terrainController.Width;
        long tileLength = this.terrainController.Length;

        // Wait for all IDs to be echoed back as ACK, retransit ID on timeout
        // Set the game timer
        // Start the game timer

    }
 
    void Update()
    {
        if (Time.time > nextTickTime)
        {
            ticks++;
            nextTickTime += tickTime;
            Debug.Log("Tick Number: " + ticks);

            // Receive data from each client

            // Add ID's of all players in the game
            int offset = 373;
            clientData[0] = 85;

            foreach (connection conn in endpoints)
            {
                clientData[offset] = conn.connID;
                offset++;
            }

            // Send the packet to each client
            foreach (connection conn in endpoints)
            {
                server.Send(conn.end, clientData, MAX_BUFFER_SIZE);
            }

            // Clear toBroadcast
            Array.Clear(toBroadcast, 0, toBroadcast.Length);
 
            // Add all clients coordinates to toBroadcast
 
            // Broadcast update to all connections
//            server.Broadcast(toBroadcast);
 
 
            // TODO: Bullets
            //      for(bullet in bullets) {
            //          for(player in players) {
            //              //copied http://cgp.wikidot.com/circle-to-circle-collision-detection
            //              //compare the distance to combined radii
            //              int dx = x2 - x1;
            //              int dy = y2 - y1;
            //              int radii = radius1 + radius2;
            //              if ( ( dx * dx )  + ( dy * dy ) < radii * radii )
            //              {
            //                  Console.WriteLine("The 2 circles are colliding!");
            //              }
            //          }
            //      }
 
        }
    }

    public static void recvThrdFunc()
    {
        byte[] recvBuffer = new byte[MAX_BUFFER_SIZE];
        Int32 numRead;
        EndPoint ep = new EndPoint();
        bool newConn = true;
        connection recvConn;

        while (running)
        {
            // If poll returns 1 (SOCKET_DATA_WAITING), there is data waiting to be read
            // If poll returns 0 (SOCKET_NODATA), there is no data waiting to be read
            if (server.Poll() == SOCKET_DATA_WAITING)
            {
                numRead = server.Recv(&ep, recvBuffer, MAX_BUFFER_SIZE);
                if (numRead <= 0)
                {
                    Console.WriteLine("Failed to read from socket.");
                }
                else
                {
                    newConn = true;
                    string contents = System.Text.Encoding.UTF8.GetString(recvBuffer);

                    foreach(connection conn in endpoints)
                    {
                        if (ep.addr.Byte0 == conn.end.addr.Byte0 && ep.addr.Byte1 == conn.end.addr.Byte1
                            && ep.addr.Byte2 == conn.end.addr.Byte2 && ep.addr.Byte3 == conn.end.addr.Byte3)
                        {
                            recvConn = conn;
                            if (recvBuffer[0].Equals(85))
                            {
                                updateCoord(recvConn.connID, recvBuffer);
                            }
                            newConn = false;
                        }
                    }
                    if(newConn == true)
                    {
                        recvConn.end = ep;
                        recvConn.connID = playerID;
                        playerID++;
                        endpoints.Add(recvConn);
                        sendInitData(playerID, ep);

                        playerID++;
                    }

                    Console.WriteLine("Received: " + contents);
                    Console.WriteLine("From EndPoint: " + ep.addr.Byte3 + "." + ep.addr.Byte2 + "." + ep.addr.Byte1 + "." + ep.addr.Byte0 + '\n');
                    // Console.WriteLine(ep.CAddr);
                }
            }
        }
    } //End of recvThrdFunc

    //Creates a new player's information
    private static void sendInitData(byte pID, EndPoint ep)
    {
        clientData[0] = 0;
        clientData[373] = pID;
        float playerx = 0 + playerID;
        float playerz = 0 + playerID;
        float rotation = 0;
        int offset = 13 + (playerID * 12);

        // sets the coordinates for each player connected
        Buffer.BlockCopy(BitConverter.GetBytes(playerx), 0, clientData, offset, 4);
        offset += 4;

        Buffer.BlockCopy(BitConverter.GetBytes(playerz), 0, clientData, offset, 4);
        offset += 4;

        Buffer.BlockCopy(BitConverter.GetBytes(rotation), 0, clientData, offset, 4);
        offset += 4;

        server.Send(ep, clientData, MAX_BUFFER_SIZE);
    }

    // Takes the recieved coords and updates client data
    private static void updateCoord(byte pID, byte[] recvConn)
    {
        int offset = 13 + (pID * 12);

        float playerX = BitConverter.ToSingle(recvConn, offset);
        offset += 4;

        float playerZ = BitConverter.ToSingle(recvConn, offset);
        offset += 4;

        float rotation = BitConverter.ToSingle(recvConn, offset);
        offset += 4;

        offset = 13 + (pID * 12);

        Buffer.BlockCopy(BitConverter.GetBytes(playerX), 0, clientData, offset, 4);
        offset += 4;

        Buffer.BlockCopy(BitConverter.GetBytes(playerZ), 0, clientData, offset, 4);
        offset += 4;

        Buffer.BlockCopy(BitConverter.GetBytes(rotation), 0, clientData, offset, 4);
        offset += 4;
    }
}