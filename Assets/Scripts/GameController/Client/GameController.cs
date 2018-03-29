using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Networking;

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
                return new Vector3(this.X, 1, this.Z);
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

    public const string SERVER_ADDRESS = "192.168.0.13";

    private byte currentPlayerId;

    private Dictionary<byte, GameObject> players;

    byte[] buffer;
    private Client client;

    public GameObject PlayerCamera;
    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;

    void Start()
    {
        currentPlayerId = 0;
        players = new Dictionary<byte, GameObject>();

        buffer = new byte[R.Net.Size.SERVER_TICK];
        client = new Client();
        client.Init(SERVER_ADDRESS, R.Net.PORT);
        Debug.Log("Init");

        byte[] initPacket = new byte[R.Net.Size.CLIENT_TICK];
        initPacket[0] = 69;

        client.Send(initPacket, R.Net.Size.CLIENT_TICK);
        Debug.Log("Send");
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
        List<PlayerData> packetData = this.getPacketData(numberOfPlayers);

        // Add any new players
        if (numberOfPlayers > this.players.Count)
        {
            for (int i = 0; i < numberOfPlayers; i++)
            {
                if (this.players.ContainsKey(packetData[i].Id))
                {
                    continue;
                }
                this.addPlayer(packetData[i]);
            }
        }

        this.movePlayers(packetData);
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
        this.currentPlayerId = buffer[1];
        this.addPlayer(new PlayerData(this.currentPlayerId));
    }

    List<PlayerData> getPacketData(int n)
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
            this.PlayerCamera.GetComponent<PlayerCamera>().Player = player;
            Instantiate(this.PlayerCamera);
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
        byte[] x = BitConverter.GetBytes(currentPlayer.transform.position.x);
        byte[] z = BitConverter.GetBytes(currentPlayer.transform.position.z);
        byte[] pheta = BitConverter.GetBytes(currentPlayer.transform.rotation.y);

        // Put position data into the packet
        this.buffer[0] = R.Net.Header.TICK;
        this.buffer[1] = this.currentPlayerId;
        Array.Copy(x    , 0, this.buffer, index,  4);
        index += 4;
        Array.Copy(z    , 0, this.buffer, index,  4);
        index += 4;
        Array.Copy(pheta, 0, this.buffer, index,  4);
        index += 4;

        this.client.Send(this.buffer, R.Net.Size.CLIENT_TICK);
    }
}
