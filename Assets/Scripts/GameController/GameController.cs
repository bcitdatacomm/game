using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Networking;
using InitGuns;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public const string SERVER_ADDRESS = "192.168.0.19";
    public const int MAX_INIT_BUFFER_SIZE = 8192;

    //private const float TOTAL_GAME_TIME = 900000f;
    private const float FIRST_SHRINK_STAGE = 600000f;
    private const float SECOND_SHRINK_STAGE = 300000f;

    private byte currentPlayerId;

    private Dictionary<byte, GameObject> players;
    private Dictionary<int, GameObject> weapons;
    private Dictionary<int, Bullet> bullets;

    byte[] buffer;
    private Client client;

    // packet for terrain data
    private byte[] terrainData;
    // packet for item data
    private byte[] itemData;

    public GameObject PlayerCamera;
    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;
    public GameObject DangerZonePrefab;
    public Text GameTimeText;
    public Text DisplayText;

    public Bullet PistolBullet;
    public Bullet ShotGunBullet;
    public Bullet RifleBullet;
    public Bullet MeleeBullet;

    private float GameTime;
    private GameObject DgZone;
    private bool dangerZoneInit;

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
        dangerZoneInit = false;
        GameTime = 0;

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
        this.bullets = new Dictionary<int, Bullet>();
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

    // This method will get the terrain and weapons and put them on the map
    // It is necessary to have tcp fill terrainData and itemData byte arrays before calling this
    void initializeGame()
    {
        // Get the data for terrain
        TerrainController tc = new TerrainController();
        weapons = tc.LoadGuns(itemBuffer);
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

        GameTime = this.getGameTime();
        Debug.Log("Current time: " + GameTime + "ms");
        this.displayGameTime();
        this.dangerZoneMessage();
        if (!dangerZoneInit)
        {
            this.initDangerZone();
        }
        else
        {
            this.updateDangerZone();
        }

        this.moveWeapons();
        this.spawnBullets();
        this.movePlayers();
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

    void initDangerZone()
    {
        int offset = R.Net.Offset.DANGER_ZONE;
        float x = BitConverter.ToSingle(this.buffer, offset);
        float z = BitConverter.ToSingle(this.buffer, offset + 4);
        float rad = BitConverter.ToSingle(this.buffer, offset + 8);
        DgZone = Instantiate(this.DangerZonePrefab, new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0));
        DgZone.transform.localScale = new Vector3(rad, 1, rad);
        Debug.Log("Danger zone initiated. Player health will start decreasing after 1 min.");
        dangerZoneInit = true;
    }

    void updateDangerZone()
    {
        int offset = R.Net.Offset.DANGER_ZONE;
        float rad = BitConverter.ToSingle(this.buffer, offset + 8);
        DgZone.transform.localScale = new Vector3(rad, 1, rad);
    }

    float getGameTime()
    {
        int offset = R.Net.Offset.TIME;
        float time = BitConverter.ToSingle(this.buffer, offset);
        return time;
    }

    void displayGameTime()
    {
        int mins = Mathf.FloorToInt(GameTime / 60000);
        int secs = Mathf.FloorToInt(GameTime % 60000 / 1000);
        GameTimeText.text = mins.ToString() + ":" + secs.ToString();
    }

    void dangerZoneMessage()
    {
        if (GameTime <= FIRST_SHRINK_STAGE + 10000 && GameTime > FIRST_SHRINK_STAGE)
        {
            // 10 secs before 10 mins mark
            DisplayText.text = "Panic mode starts in 10 secs";
        }
        else if (GameTime <= FIRST_SHRINK_STAGE && GameTime > FIRST_SHRINK_STAGE - 10000)
        {
            // 10 mins to 10 secs after 10 mins mark
            Debug.Log("10 minutes left.");
            DisplayText.text = "PANIC MODE";
        }
        else if (GameTime <= SECOND_SHRINK_STAGE)
        {
            Debug.Log("5 minutes left.");
            DisplayText.text = "Panic mode ends; 5 mins left";
        }
        else
        {
            DisplayText.text = "";
        }
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

    void movePlayers()
    {
        int numberOfPlayers = HeaderDecoder.GetPlayerCount(this.buffer[0]);
        List<PlayerData> playerDatas = this.getPlayerData(numberOfPlayers);

        // Add any new players
        if (numberOfPlayers > this.players.Count)
        {
            for (int i = 0; i < numberOfPlayers; i++)
            {
                if (this.players.ContainsKey(playerDatas[i].Id))
                {
                    continue;
                }
                this.addPlayer(playerDatas[i]);
            }
        }

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

    void spawnBullets()
    {
        if (HeaderDecoder.HasBullet(this.buffer[0]))
        {
            int numBullets = Convert.ToInt32(this.buffer[R.Net.Offset.BULLETS]);

            int offset = R.Net.Offset.BULLETS + 1;
            byte ownerId;

            for(int i = 0; i < numBullets; i++)
            {
                if (this.buffer[offset + 6] == 1)
                {
                    ownerId = this.buffer[offset];

                    if (ownerId == this.currentPlayerId)
                    {
                        continue;
                    }

                    Bullet newBullet = null;
                    switch(this.buffer[offset + 5]) {
                        case R.Type.KNIFE:
                            newBullet = (Bullet)GameObject.Instantiate(this.MeleeBullet, this.players[ownerId].transform.position, this.players[ownerId].transform.rotation);
                            break;
                        case R.Type.PISTOL:
                            newBullet = (Bullet)GameObject.Instantiate(this.PistolBullet, this.players[ownerId].transform.position, this.players[ownerId].transform.rotation);
                            break;
                        case R.Type.SHOTGUN:
                            newBullet = (Bullet)GameObject.Instantiate(this.ShotGunBullet, this.players[ownerId].transform.position, this.players[ownerId].transform.rotation);
                            break;
                        case R.Type.RIFLE:
                            newBullet = (Bullet)GameObject.Instantiate(this.RifleBullet, this.players[ownerId].transform.position, this.players[ownerId].transform.rotation);
                            break;
                    }
                    newBullet.direction = this.players[ownerId].transform.rotation * Vector3.forward;
                    bullets[BitConverter.ToInt32(this.buffer, offset + 1)] = newBullet;
                }
                offset += 7;
            }
        }
    }

    void moveWeapons()
    {
        if (HeaderDecoder.HasWeapon(this.buffer[0]))
        {
            int offset = R.Net.Offset.WEAPONS;
            int weaponSwapEventCount = Convert.ToInt32(this.buffer[offset]);
            offset++;

            for (int i = 0; i < weaponSwapEventCount; i++)
            {
                byte ownerId = this.buffer[offset];
                int newWeaponId = BitConverter.ToInt32(this.buffer, offset + 1);

                Debug.Log("Player " + ownerId + " has just picked up weapon with id " + newWeaponId);

                if (ownerId != this.currentPlayerId)
                {
                    GameObject parent = this.players[ownerId];
                    Transform gun = this.weapons[newWeaponId].transform;

                    // Drop the old gun if they have one
                    if (parent.transform.Find("Inventory").childCount > 0)
                    {
                        parent.transform.Find("Inventory").GetChild(0).transform.parent = null;
                    }

                    // Pick up the new gun
                    gun.parent = parent.transform.Find("Inventory").transform;
                    gun.transform.position = parent.transform.position + new Vector3(0.2f, 1, 0);
                    gun.transform.rotation = parent.transform.rotation;
                }

                offset += 5;
            }
        }
    }

    void sendPlayerDataToServer()
    {
        GameObject currentPlayer = this.players[this.currentPlayerId];
        Player playerRef = currentPlayer.transform.GetComponent<Player>();
        byte[] x = BitConverter.GetBytes(currentPlayer.transform.position.x);
        byte[] z = BitConverter.GetBytes(currentPlayer.transform.position.z);
        byte[] pheta = BitConverter.GetBytes(currentPlayer.transform.eulerAngles.y);
        byte[] bullet = new byte[5];
        byte[] packet = new byte[R.Net.Size.CLIENT_TICK];

        if (playerRef.FiredShots.Count > 0)
        {
            Bullet bulletRef = playerRef.FiredShots.Pop();
            bullet = bulletRef.ToBytes();
        }

        // Put position data into the packet
        packet[0] = R.Net.Header.TICK;
        packet[R.Net.Offset.PID] = this.currentPlayerId;
        Array.Copy(x    , 0, packet, R.Net.Offset.X,  4);
        Array.Copy(z    , 0, packet, R.Net.Offset.Z,  4);
        Array.Copy(pheta, 0, packet, R.Net.Offset.R,  4);
        Array.Copy(playerRef.Weapon, 0, packet, R.Net.Offset.WEAPON, 5);
        Array.Copy(bullet, 0, packet, R.Net.Offset.BULLET, 5);
        this.client.Send(packet, R.Net.Size.CLIENT_TICK);
    }
}
