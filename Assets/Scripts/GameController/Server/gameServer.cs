using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Networking;
using System.Threading;
using InitGuns;

// This is the core script for the server.

public unsafe class gameServer : MonoBehaviour
{
    // Constant max packet size for terrain & spawn data
    private const Int32 MAX_INIT_BUFFER_SIZE = 8192;
    private const Int32 MAX_NUM_CLIENTS = 30;

    // HARD CODED PORT NUMBER
    private const ushort PORT_NO = 42069;

    // Global client sockdescriptor array
    Int32[] clientArr = new Int32[MAX_NUM_CLIENTS];



    // Member Data
    private static float nextTickTime = 0.0f;
    private static int ticksPerSecond = 64;
    private float tickTime = (1 / ticksPerSecond);
    private int ticks = 0;

    private static Server server;
    private static TCPServer tcpServer;     // global TCP server added for TCP transmission
    private static IntPtr serverInstance;
    private static bool running;
    // GLOBAL LISTENING BOOLEAN, used for listening for client connections
    private bool listening = false;
    private static Thread recvThread;

    // Terrain Object and packet byte
    private TerrainController terrainController;
    private static byte[] clientData = new byte[R.Net.Size.SERVER_TICK];

    // Random Weapon/Spell object and packet byte
    private InitRandomGuns getitems;
    private static byte[] itemData;

    private static List<connectionData> endpoints;
    static byte playerID = 1;
    static float spawnPoint = 2.0f;

    // Use this for initialization
    void Start()
    {



        server = new Server();
        server.Init(R.Net.PORT);

        // Listen for new players with tcp for a set time period or until max players have joined
        endpoints = new List<connectionData>();

        // Create the terrain packet with format 1B header + 4B size as int + data
        terrainController = new TerrainController();
        while (!terrainController.GenerateEncoding());

        int terrainDataLength = terrainController.CompressedData.Length;
        // **MODIFIED** Use a constant buffer size of 8192 bytes now.
        byte[] terrainPacket = new byte[MAX_INIT_BUFFER_SIZE];

        terrainPacket[0] = R.Net.Header.TERRAIN_DATA;
        Array.Copy(BitConverter.GetBytes(terrainDataLength), 0, terrainPacket, 1, 4);
        Array.Copy(terrainController.CompressedData, 0, terrainPacket, 5, terrainDataLength);

        endpoints = new List<connectionData>();
        // recvThread = new Thread(recvThrdFunc);
        // running = true;
        // recvThread.Start();

        // Create the terrain packet with format 1B header + 4B size as int + data
        terrainController = new TerrainController();
        while (!terrainController.GenerateEncoding()) ;

        terrainPacket[0] = R.Net.Header.TERRAIN_DATA;
        Array.Copy(BitConverter.GetBytes(terrainDataLength), 0, terrainPacket, 1, 4);
        Array.Copy(terrainController.CompressedData, 0, terrainPacket, 5, terrainDataLength);


        // INITIALIZE WEAPONS AND SPELLS SECTION
        // NOTE this must happen:
        // -after terrain data has been generated
        // -after you have total number of players/endpoints
        // -before itemData is sent via TCP
        getitems = new InitRandomGuns(endpoints.Count, terrainController.occupiedPositions);
        itemData = getitems.pcktarray;
        // ENDSECTION

        // TODO: Implement this in TCP
        // Make a terrain packet (byte array) with encoded
        // endpoints[i];



        // Move this to the top, should be initialized first.
        Int32 servsockfd;

        Thread listenThread;

        tcpServer = new TCPServer();
        servsockfd = tcpServer.Init(PORT_NO);
        // Socket initialization fail, should probably abandon ship at this point.
        if (servsockfd <= 0)
        {
            Debug.Log("Server failed to initialize socket.");
        }
        else
        {
            // Start the listen thread
            listenThread = new Thread(listenThrdFunc);
            listening = true;
            listenThread.Start();
        }


        // TODO: CLOSE THE SERVER SOCKET AFTER TCP TRANSMISSION.
        // result = tcpServer.CloseListenSocket(serversockfd);
        //if (result != 0)
        //{
        //    Debug.Log("Shit i couldnt' close the server tcp socket");
        //}
        //for (int i = 0; i < clientArr.Length; i++)
        //{
        //    tcpServer.CloseClientSocket(clientsockfd);
        //}

        // Send terrain data and item spawn data to all clients


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
        int offset = R.Net.Offset.PLAYER_IDS;
        clientData[0] = R.Net.Header.TICK;

        for (int i = 0; i < endpoints.Count; i++)
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
                server.Send(endpoints[i].ep, clientData, R.Net.Size.SERVER_TICK);
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
            if (server.Poll())
            {
                numRead = server.Recv(ref ep, recvBuffer, R.Net.Size.CLIENT_TICK);

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
                    connectionData tmp = endpoints[i];
                    tmp.buffer = buffer;
                    endpoints[i] = tmp;
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

    /*
    *   Added listenThrdFunc
    *   This thread function is used by the TCPServer object to listen for incoming
    *   connection requests.
    *   Once the server maxes out the number of clients (30) or is forced to continue,
    *   it will create separate threads to write the terrainbuffer and spawnbuffer
    *   to each client via TCP.
    */
    private void listenThrdFunc()
    {
        EndPoint ep = new EndPoint();
        Int32 clientSockFD;
        Thread[] transmitThrdArray = new Thread[MAX_NUM_CLIENTS];
        Int32 numClients = 0;

        while (listening && numClients < MAX_NUM_CLIENTS);
        {
            clientSockFD = tcpServer.AcceptConnection(ref ep);
            // Accept fails
            if (clientSockFD == -1)
            {
                //Iduno, probably continue and log the error.
            }
            // Accept passes
            else
            {
                this.clientArr[numClients] = clientSockFD;
                numClients++;
            }
        }
        // Completed listening, either

        for (int i = 0; i < numClients; i++)
        {
            //Create new threads to send here
            transmitThrdArray[i] = new Thread(this.transmitThrdFunc);
            transmitThrdArray[i].Start(this.clientArr[i]);
        }
    }

    //TODO: Pass in clientsockfd (client socket descriptor) into thread function
    /*
    * This thread function is used to send the contents of the terrainbuffer and
    * spawnbuffer to a client.
    */
    private void transmitThrdFunc(object clientsockfd)
    {
        Int32 numSentMap;
        Int32 numSentWep;
        Int32 sockfd = (Int32)clientsockfd;
        // 2 send calls

        //Send terrain data
        numSentMap = tcpServer.Send(sockfd, clientData, MAX_INIT_BUFFER_SIZE);
        if (numSentMap > 0)
        {   //Send spawn data
            numSentWep = tcpServer.Send(sockfd, itemData, MAX_INIT_BUFFER_SIZE);
        }
    }
}
