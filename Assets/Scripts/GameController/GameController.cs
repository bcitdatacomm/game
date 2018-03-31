using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Networking;
using InitGuns;

public class GameController : MonoBehaviour
{
    private class PlayerData
    {
        public byte Id { get; set; }
        public float X { get; set; }
        public float Z { get; set; }
        public float R { get; set; }
        public byte Weapon { get; set; }
        public Vector3 Position
        {
            get
            {
                return new Vector3(this.X, 0, this.Z);
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return Quaternion.Euler(0, R, 0);
            }
        }

        public PlayerData()
        {
            this.Id = 0;
            this.X = 0;
            this.Z = 0;
            this.R = 0;
            this.Weapon = 0;
        }

        public PlayerData(byte id)
        {
            this.Id = id;
            this.X = 0;
            this.Z = 0;
            this.R = 0;
            this.Weapon = 0;
        }

        public PlayerData(byte id, float x, float z, float r, byte weapon)
        {
            this.Id = id;
            this.X = x;
            this.Z = z;
            this.R = r;
            this.Weapon = weapon;
        }
    }

    private class HeaderDecoder
    {
        public static int GetPlayerCount(byte header)
        {
            return (0x1F & header);
        }

        public static bool IsTickPacket(byte header)
        {
            return (0x80 & header) > 0;
        }

        public static bool HasBullet(byte header)
        {
            return (0x40 & header) > 0;
        }

        public static bool HasWeapon(byte header)
        {
            return (0x20 & header) > 0;
        }
    }

    public const string SERVER_ADDRESS = "192.168.0.8";
    public const int MAX_INIT_BUFFER_SIZE = 8192;

    private byte currentPlayerId;

    private Dictionary<byte, GameObject> players;

    byte[] buffer;
    private Client client;

    // packet for terrain data
    private byte[] terrainData;
    // packet for item data
    private byte[] itemData;

    public GameObject PlayerCamera;
    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;
    public Bullet Bullet;

    // ADDED: Game initialization variables
    private TCPClient tcpClient;
    private Int32 clientsockfd;
    private byte[] mapBuffer;
    private byte[] itemBuffer;
    private EndPoint epServer;

    void Start()
    {
        mapBuffer = new byte[MAX_INIT_BUFFER_SIZE];
        itemBuffer = new byte[MAX_INIT_BUFFER_SIZE];
        // Adding TCP receive code here, move as needed
        epServer = new EndPoint(SERVER_ADDRESS, R.Net.PORT);
        tcpClient = new TCPClient();
        Int32 result = tcpClient.Init(epServer);

        if (result <= 0)
        {
            Debug.Log("Error initializing TCP client socket: " + result);
        }
        if (result > 0)
        {
            int numRecvMap;
            int numRecvItem;

            Debug.Log("Successfully connected:" + result);

            // Receiving the item packet
            numRecvItem = tcpClient.Recv(itemBuffer, MAX_INIT_BUFFER_SIZE);
            if (numRecvItem <= 0)
            {
                Debug.Log(System.Text.Encoding.Default.GetString(itemBuffer));
                Debug.Log("This REALLY shouldn't happen.");
            }
            Debug.Log("Received: " + numRecvItem);

            // Receiving the map packet
            numRecvMap = tcpClient.Recv(mapBuffer, MAX_INIT_BUFFER_SIZE);
            if (numRecvMap <= 0)
            {
                Debug.Log(System.Text.Encoding.Default.GetString(mapBuffer));
                Debug.Log("This shouldn't happen.");
            }
            Debug.Log("Received: " + numRecvMap);

            // Close the TCP connection after the work is done
            Debug.Log("Close socket result: " + (result = tcpClient.CloseConnection(result)));

            if (result != 0)
            {
                Debug.Log("Error closing TCP client socket.");
            }
        }

        this.client = new Client();
        this.client.Init(SERVER_ADDRESS, R.Net.PORT);

        this.currentPlayerId = 0;
        this.players = new Dictionary<byte, GameObject>();
        this.buffer = new byte[R.Net.Size.SERVER_TICK];

        initializeGame();
    }

    void FixedUpdate()
    {
        if (this.currentPlayerId != 0)
        {
            this.updateGameState();
        }
        else
        {
            this.syncWithServer();
        }
    }

    void updateGameState()
    {
        this.sendPlayerDataToServer();

        if (!this.client.Poll())
        {
            return;
        }

        if (client.Recv(this.buffer, R.Net.Size.SERVER_TICK) < R.Net.Size.SERVER_TICK)
        {
            return;
        }

        int numberOfPlayers = HeaderDecoder.GetPlayerCount(this.buffer[0]);
        List<PlayerData> playerData = this.getPlayerData(numberOfPlayers);

        // Add any new players
        if (numberOfPlayers > this.players.Count)
        {
            for (int i = 0; i < numberOfPlayers; i++)
            {
                if (this.players.ContainsKey(playerData[i].Id))
                {
                    continue;
                }
                this.addPlayer(playerData[i]);
            }
        }

        if (HeaderDecoder.HasBullet(this.buffer[0]))
        {
            int numBullets = Convert.ToInt32(this.buffer[R.Net.Offset.BULLETS]);
            Debug.Log("Number of bullets: " + numBullets);

            int offset = R.Net.Offset.BULLETS + 1;
            byte ownerId;

            for(int i = 0; i < numBullets; i++)
            {
                if (this.buffer[offset + 6] == 1)
                {
                    ownerId = this.buffer[offset];
                    Debug.Log("owner is " + ownerId);
                    Bullet newBullet = (Bullet)GameObject.Instantiate(this.Bullet, this.players[ownerId].transform.position, this.players[ownerId].transform.rotation);
                    newBullet.direction = this.players[ownerId].transform.rotation.y * Vector3.forward;
                    Debug.Log("creating bullet");
                }
                offset += 7;
            }
        }

        this.movePlayers(playerData);
    }

    // This method will get the terrain and weapons and put them on the map
    // It is necessary to have tcp fill terrainData and itemData byte arrays before calling this
    void initializeGame()
    {
        // Get the data for terrain
        TerrainController tc = new TerrainController();
        tc.LoadGuns(itemBuffer);
        tc.LoadByteArray(mapBuffer);

        byte[] ackPack = new byte[R.Net.Size.CLIENT_TICK];
        ackPack[0] = R.Net.Header.ACK;

        client.Send(ackPack, R.Net.Size.CLIENT_TICK);
    }


    void syncWithServer()
    {
        if (!this.client.Poll())
        {
            return;
        }

        if (this.client.Recv(buffer, R.Net.Size.SERVER_TICK) < R.Net.Size.SERVER_TICK)
        {
            return;
        }

        if (this.buffer[0] != R.Net.Header.INIT_PLAYER)
        {
            return;
        }

        // Where is the id in the init packet
        this.currentPlayerId = this.buffer[1];
        Debug.Log("My id is " + this.currentPlayerId);
        float x = BitConverter.ToSingle(buffer, 2);
        float z = BitConverter.ToSingle(buffer, 6);
        Debug.Log("Spawn location at " + x + ", " + z);
        this.addPlayer(new PlayerData(this.currentPlayerId, x, z, 0, 0));
    }

    List<PlayerData> getPlayerData(int n)
    {
        List<PlayerData> data = new List<PlayerData>();
        int offset = R.Net.Offset.PLAYERS;

        for (int i = 0; i < n; i++)
        {
            byte id = this.buffer[offset + R.Net.Offset.Player.ID];
            float x = BitConverter.ToSingle(this.buffer, offset + R.Net.Offset.Player.X);
            float z = BitConverter.ToSingle(this.buffer, offset + R.Net.Offset.Player.Z);
            float r = BitConverter.ToSingle(this.buffer, offset + R.Net.Offset.Player.R);
            byte weapon = this.buffer[offset + R.Net.Offset.Player.W];
            offset += R.Net.Size.PLAYER_DATA;

            data.Add(new PlayerData(id, x, z, r, weapon));
        }

        return data;
    }

    void addPlayer(PlayerData newPlayer)
    {
        GameObject player;

        if (newPlayer.Id == this.currentPlayerId)
        {
            player = (GameObject)Instantiate(this.PlayerPrefab, newPlayer.Position, newPlayer.Rotation);
            float x = newPlayer.Position.x;
            float z = newPlayer.Position.z;
            this.PlayerCamera.GetComponent<PlayerCamera>().Player = player;
            Instantiate(this.PlayerCamera, new Vector3(x, 15, z), Quaternion.Euler(90, 0, 0));
        }
        else
        {
            player = (GameObject)Instantiate(this.EnemyPrefab, newPlayer.Position, newPlayer.Rotation);

        }

        this.players.Add(newPlayer.Id, player);
    }

    // Refactor
    void movePlayers(List<PlayerData> playerDatas)
    {
        try
        {
            for (int i = 0; i < playerDatas.Count; i++)
            {
                if (this.currentPlayerId == playerDatas[i].Id)
                {
                    continue;
                }
                this.players[playerDatas[i].Id].transform.position = playerDatas[i].Position;
                this.players[playerDatas[i].Id].transform.rotation = playerDatas[i].Rotation;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    void sendPlayerDataToServer()
    {
        int index = 2;
        GameObject currentPlayer = this.players[this.currentPlayerId];
        Player playerRef = currentPlayer.transform.GetComponent<Player>();
        byte[] x = BitConverter.GetBytes(currentPlayer.transform.position.x);
        byte[] z = BitConverter.GetBytes(currentPlayer.transform.position.z);
        byte[] pheta = BitConverter.GetBytes(currentPlayer.transform.rotation.y);
        byte[] bullet = new byte[5];
        byte[] packet = new byte[R.Net.Size.CLIENT_TICK];

        if(playerRef.FiredShots.Count > 0)
        {
            Bullet bulletRef = playerRef.FiredShots.Pop();
            bullet = bulletRef.ToBytes();
        }

        // Put position data into the packet
        packet[0] = R.Net.Header.TICK;
        packet[1] = this.currentPlayerId;
        Array.Copy(x    , 0, packet, index,  4);
        index += 4;
        Array.Copy(z    , 0, packet, index,  4);
        index += 4;
        Array.Copy(pheta, 0, packet, index,  4);
        index += 4;
        Array.Copy(playerRef.getInventory(), 0, packet, index, 5);
        index += 5;
        Array.Copy(bullet, 0, packet, index, 5);
        index += 5;
        this.client.Send(packet, R.Net.Size.CLIENT_TICK);
    }
}
