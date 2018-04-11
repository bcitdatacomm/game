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
    public const string SERVER_ADDRESS = "192.168.0.24";
    public const int MAX_INIT_BUFFER_SIZE = 8192;

    //private const float TOTAL_GAME_TIME = 900000f;
    private const float SHRINK_PHASE_1 = 780000f;       // 2 mins mark
    private const float SHRINK_PHASE_1_END = 600000f;   // 5 mins mark
    private const float SHRINK_PHASE_2 = 480000f;       // 7 mins mark
    private const float SHRINK_PHASE_2_END = 300000f;   // 10 mins mark
    private const float SHRINK_PHASE_3 = 180000f;       // 12 mins mark

    private byte currentPlayerId;
    private bool currentPlayerDead = false;

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
    private Transform DZIndicator;

    // DZ indicator
    private LineRenderer dzLine;
    private int dzLineSeg = 50;
    public float xradius = 5;
    public float yradius = 5;
    public int numSegments = 128;

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
        this.displayGameTime();
        this.dangerZoneMessage();
        this.updateDangerZone();
        this.updateDZIndicator();

        this.setHealth();
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

    void updateDangerZone()
    {
        int offset = R.Net.Offset.DANGER_ZONE;
        float x = BitConverter.ToSingle(this.buffer, offset);
        float z = BitConverter.ToSingle(this.buffer, offset + 4);
        float rad = BitConverter.ToSingle(this.buffer, offset + 8);
        if (!dangerZoneInit)
        {
            DgZone = Instantiate(this.DangerZonePrefab, new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0));
            Debug.Log("Danger zone initiated.");
            dangerZoneInit = true;

            // dzLine = DgZone.GetComponent<LineRenderer>();
            // dzLine.SetVertexCount (dzLineSeg + 1);
            // dzLine.useWorldSpace = false;
            // Color c1 = new Color(0.5f, 0.5f, 0.5f, 1);
            // dzLine.material = new Material(Shader.Find("Particles/Additive"));
            // dzLine.SetColors(c1, c1);
            // dzLine.SetWidth(2f, 2f);
            // dzLine.SetVertexCount(numSegments + 1);
            // dzLine.useWorldSpace = false;
            //
            // float deltaTheta = (float) (2.0 * Mathf.PI) / numSegments;
            // float theta = 0f;
            //
            // for (int i = 0 ; i < numSegments + 1 ; i++) {
            // float x = radius * Mathf.Cos(theta);
            // float z = radius * Mathf.Sin(theta);
            // Vector3 pos = new Vector3(x, 0, z);
            // dzLine.SetPosition(i, pos);
            // theta += deltaTheta;
        }
        else
        {
            DgZone.transform.position = new Vector3(x, 0, z);
        }
        DgZone.transform.localScale = new Vector3(rad * 2, 0.5f, rad * 2);
    }

    void createDZPoints()
    {
        float x;
        float y;
        float z;

        float angle = 20f;

        for (int i = 0; i < (dzLineSeg + 1); i++)
        {
            x = Mathf.Sin (Mathf.Deg2Rad * angle) * xradius;
            z = Mathf.Cos (Mathf.Deg2Rad * angle) * yradius;

            dzLine.SetPosition (i,new Vector3(x,1,z) );

            angle += (360f / dzLineSeg);
        }
    }

    void updateDZIndicator()
    {
        float xDiff = DgZone.transform.position.x - players[currentPlayerId].transform.position.x;
        float zDiff = DgZone.transform.position.z - players[currentPlayerId].transform.position.z;
        float theta = (float)(Math.Atan(zDiff / xDiff) * Mathf.Rad2Deg);

        Vector3 target = DgZone.transform.position - players[currentPlayerId].transform.position;
        float angle = Vector3.Angle(target, Vector3.forward);
        Quaternion rot = Quaternion.identity;

        if (angle < 90)
        {
            rot = Quaternion.Euler(0, -angle, 0);
        }
        else
        {
            rot = Quaternion.Euler(0, angle, 0);
        }

        DZIndicator.rotation = rot;
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
        GameTimeText.text = "Time: " + mins.ToString() + ":" + secs.ToString();
    }

    void dangerZoneMessage()
    {
        if (GameTime <= SHRINK_PHASE_1 + 10000 && GameTime > SHRINK_PHASE_1)
        {
            // 10 secs before phase 1 shrinks
            DisplayText.text = "Safe zone starts to shrink in 10 secs";
        }
        else if (GameTime <= SHRINK_PHASE_1_END + 10000 && GameTime > SHRINK_PHASE_1_END)
        {
            // 10 secs before phase 1 ends
            DisplayText.text = "Safe zone stablizes in 10 secs";
        }
        else if (GameTime <= SHRINK_PHASE_2 + 10000 && GameTime > SHRINK_PHASE_2)
        {
            // 10 secs before phase 2 shrinks
            DisplayText.text = "Safe zone starts to shrink in 10 secs";
        }
        else if (GameTime <= SHRINK_PHASE_2_END + 10000 && GameTime > SHRINK_PHASE_2_END)
        {
            DisplayText.text = "Safe zone stablizes in 10 secs";
        }
        else if (GameTime <= SHRINK_PHASE_3 + 10000 && GameTime > SHRINK_PHASE_3)
        {
            // 10 secs before phase 3 shrinks
            DisplayText.text = "Safe zone starts to shrink in 10 secs";
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
            DZIndicator = player.transform.Find("DZ Indicator");
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
                //checkPlayerHealth(playerDatas[i]);
                continue;
            }
            this.players[playerDatas[i].Id].transform.position = playerDatas[i].Position;
            this.players[playerDatas[i].Id].transform.rotation = playerDatas[i].Rotation;
        }
    }

    //void checkPlayerHealth(PlayerData pData)
    //{
    //    Player playerRef = this.players[pData.Id].GetComponent<Player>();
    //    playerRef.Health = pData.Health;
    //    Debug.Log("Player health: " + playerRef.Health);
    //    if (playerRef.Health == 0)
    //    {
    //        // player dead
    //        //removePlayer(pData);
    //    }
    //}

    void setHealth()
    {
        byte health = this.buffer[R.Net.Offset.HEALTH];

        this.players[this.currentPlayerId].GetComponent<Player>().Health = Convert.ToInt32(health);
    }

    void removePlayer(PlayerData deadPlayer)
    {
        // player is dead, do something here
        Destroy(this.players[deadPlayer.Id]);
        currentPlayerDead = true;
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
