using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Networking;

public class GameController_singleThread : GameController {
    public struct playerCoord
    {
        public uint id;
        public Vector3 position;
        public Quaternion rotation;
    }
    public Item[] Pickups;
    public GameObject OtherPlayerPrefab;
    public GameObject PlayerPrefab;
    public GameObject BulletPrefab;


    public Dictionary<uint, GameObject> BulletDict { get; set; }
    // key of OwnBulletDict is InstanceID of Bullet
    public Dictionary<int, GameObject> OwnBulletDict { get; set; }
    //public List<GameObject> BulletList { get; set; }

    public Dictionary<uint, GameObject> PlayerDict { get; set; }
    public int NumberOfPlayers = 2;

    // used for unrendering dead players
    List<uint> livingPlayerIDs;
    const int PACKET_SIZE = 1200;
    byte[] clientData = new byte[PACKET_SIZE];
    // current Player is onStart index 0, no need to have var for it
    uint currentPlayerKey;

    // stops Update from running too soon
    Client currentClient;

    private bool initRender = false;
    private bool pollRunning = false;
    private bool dataReady = false;

    void Start()
    {
        Debug.Log("starting");
        //setup client
        if(currentClient == null)
        {
            currentClient = new Client();
            // @TODO get user input & change this
            currentClient.Init("127.0.0.1", 7000);
        }

        if( currentClient.Poll() == 0)
        {
            return;
        }
        //data is ready to be loaded asap
        initRender = true;
        currentClient.Recv(clientData, PACKET_SIZE);
        // clientData can be either a playerStart or a tickPacket
        if (clientData[0] != 2 && clientData[0] != 3)
            return;

        List<playerCoord> startCoords = getPlayersCoord();
        currentPlayerKey = System.BitConverter.ToUInt32(clientData, 372);
        foreach (playerCoord coord in startCoords)
        {
            Vector3 pos = coord.position;
            Quaternion rot = coord.rotation;
            GameObject playerGameObj;
            Player playerScript;
            if (coord.id == currentPlayerKey)
            {
                playerGameObj = (GameObject)Instantiate(PlayerPrefab, pos, rot);
                // bind camera to current player
                Camera mainCam = FindObjectOfType<Camera>();
                mainCam.GetComponent<CameraFollow>().SetOffset( playerGameObj.transform );

                playerScript = playerGameObj.GetComponent<Player>();
                playerScript.ClientController = this;
            }
            else
            {
                playerGameObj = (GameObject)Instantiate(OtherPlayerPrefab, pos, rot);
                playerScript = playerGameObj.GetComponent<DummyPlayer>();
            }
            playerScript.PlayerId = coord.id;
            PlayerDict.Add(playerScript.PlayerId, playerGameObj);
        }

        byte[] renderAckPacket = new byte[PACKET_SIZE];
        byte[] playerIdBytes = System.BitConverter.GetBytes(currentPlayerKey);
        renderAckPacket[0] = 4;
        System.Array.Copy(playerIdBytes, 0, renderAckPacket, 1, playerIdBytes.Length);
        currentClient.Send(renderAckPacket, PACKET_SIZE);

        // @TODO API call to get map packet
        // byte[] mapDataPacket = new byte[PACKET_SIZE];
        // currentClient.Recv(mapDataPacket, PACKET_SIZE);
        // if (mapDataPacket[0] == 1);
        // do map stuff


    }

    void Update()
    {
        Debug.Log("tick");

        if (initRender)
        {
            MoveOther();
            return;
        }
        // start code, if data wasn't available during start
        if( currentClient.Poll() == 0)
        {
            return;
        }
        //start data is finally ready
        currentClient.Recv(clientData, PACKET_SIZE);
        // clientData can be either a playerStart or a tickPacket
        if (clientData[0] != 2 && clientData[0] != 3)
            return;

        List<playerCoord> startCoords = getPlayersCoord();
        currentPlayerKey = System.BitConverter.ToUInt32(clientData, 372);
        foreach (playerCoord coord in startCoords)
        {
            Vector3 pos = coord.position;
            Quaternion rot = coord.rotation;
            GameObject playerGameObj;
            Player playerScript;
            if (coord.id == currentPlayerKey)
            {
                playerGameObj = (GameObject)Instantiate(PlayerPrefab, pos, rot);
                // bind camera to current player
                Camera mainCam = FindObjectOfType<Camera>();
                mainCam.GetComponent<CameraFollow>().SetOffset( playerGameObj.transform );

                playerScript = playerGameObj.GetComponent<Player>();
                playerScript.ClientController = this;
            }
            else
            {
                playerGameObj = (GameObject)Instantiate(OtherPlayerPrefab, pos, rot);
                playerScript = playerGameObj.GetComponent<DummyPlayer>();
            }
            playerScript.PlayerId = coord.id;
            PlayerDict.Add(playerScript.PlayerId, playerGameObj);
        }

        byte[] renderAckPacket = new byte[PACKET_SIZE];
        byte[] playerIdBytes = System.BitConverter.GetBytes(currentPlayerKey);
        renderAckPacket[0] = 4;
        System.Array.Copy(playerIdBytes, 0, renderAckPacket, 1, playerIdBytes.Length);
        currentClient.Send(renderAckPacket, PACKET_SIZE);
        initRender = true;
    }

    // Optionally can go directly inside update
    void MoveOther()
    {
        GameObject currentPlayer = PlayerDict[currentPlayerKey];
        float playerX = currentPlayer.transform.position.x;
        float playerZ = currentPlayer.transform.position.z;
        float playerRot = currentPlayer.transform.rotation.y;
        uint id = currentPlayerKey;
        byte[] currentPlayerPacket = new byte[1200];
        int offset = 0;
        currentPlayerPacket[offset] = 2;
        offset += 1;
        byte[] playerIdBytes = System.BitConverter.GetBytes(currentPlayerKey);
        System.Array.Copy(playerIdBytes, 0, currentPlayerPacket, offset,playerIdBytes.Length );
        offset += 1;
        byte[] playerXBytes = System.BitConverter.GetBytes(playerX);
        System.Array.Copy(playerXBytes, 0, currentPlayerPacket, offset, playerXBytes.Length);
        offset += 4;
        byte[] playerZBytes = System.BitConverter.GetBytes(playerZ);
        System.Array.Copy(playerZBytes, 0, currentPlayerPacket, offset, playerZBytes.Length);
        offset += 4;
        byte[] playerRotBytes = System.BitConverter.GetBytes(playerRot);
        System.Array.Copy(playerRotBytes, 0, currentPlayerPacket, offset, playerRotBytes.Length);
        currentClient.Send(currentPlayerPacket, PACKET_SIZE);

        if (currentClient.Poll() == 0)
        {
            return;
        }
        currentClient.Recv(clientData, PACKET_SIZE);
        if (clientData[0] != 2)
            return;
        List<playerCoord> updatedCoords = getPlayersCoord();
        // @TODO update server with bullet stuff here
        // UpdateBullets();

        // update living player list
        livingPlayerIDs.Clear();
        // move alive players, but skip currentPlayer
        foreach (playerCoord coord in updatedCoords)
        {
            livingPlayerIDs.Add(coord.id);
            if (coord.id == currentPlayerKey)
            {
                continue;
            }
            GameObject player = PlayerDict[coord.id];
            Rigidbody playerBody = player.GetComponent<Rigidbody>();
            playerBody.MoveRotation(coord.rotation);
            playerBody.MovePosition(player.transform.position + coord.position);
        }
        // clear dead players
        foreach (KeyValuePair<uint, GameObject> player in PlayerDict)
        {
            if (!livingPlayerIDs.Contains(player.Key))
            {
                // erase from records & remove from world
                PlayerDict.Remove(player.Key);
                Destroy(player.Value);
            }
        }
    }

    void UpdateBullets()
    {
        // sending our bullet packet
        byte[] ourBulletPacket = new byte[PACKET_SIZE];
        ourBulletPacket[0] = 3;
        int offset = 1;
        foreach (KeyValuePair<int, GameObject> bulletPair in OwnBulletDict)
        {
            //Bullet ownBullet = bulletPair.Value.GetComponent<Bullet>();
            //if (ownBullet.src.GetComponent<Player>().PlayerId == currentPlayerKey && bulletPair.Value != null)
            //{
            Bullet bullet = bulletPair.Value.GetComponent<Bullet>();

            byte[] ownerIdBytes = System.BitConverter.GetBytes(currentPlayerKey);
            System.Array.Copy(ownerIdBytes, 0, ourBulletPacket ,offset, ownerIdBytes.Length);
            offset += 1;

            byte[] bulletLifeBytes = System.BitConverter.GetBytes(bullet.BulletLifeTime);
            System.Array.Copy(bulletLifeBytes, 0, ourBulletPacket, offset, bulletLifeBytes.Length);
            offset += 2;

            byte[] bulletXBytes = System.BitConverter.GetBytes(bullet.transform.position.x);
            System.Array.Copy(bulletXBytes, 0, ourBulletPacket, offset, bulletXBytes.Length);
            offset += 4;

            byte[] bulletZBytes = System.BitConverter.GetBytes(bullet.transform.position.z);
            System.Array.Copy(bulletZBytes, 0, ourBulletPacket, offset, bulletZBytes.Length);
            offset += 4;

            byte[] bulletVelocityXBytes = System.BitConverter.GetBytes(bulletPair.Value.GetComponent<Rigidbody>().velocity.x);
            System.Array.Copy(bulletVelocityXBytes, 0, ourBulletPacket, offset, bulletVelocityXBytes.Length);
            offset += 4;

            byte[] bulletVelocityZBytes = System.BitConverter.GetBytes(bulletPair.Value.GetComponent<Rigidbody>().velocity.z);
            System.Array.Copy(bulletVelocityXBytes, 0, ourBulletPacket, offset, bulletVelocityZBytes.Length);
            currentClient.Send(ourBulletPacket, PACKET_SIZE);
            //}
        }

        // receiving bullet packets
        byte[] bulletPacket = new byte[PACKET_SIZE];
        currentClient.Recv(bulletPacket, PACKET_SIZE);
        if (bulletPacket[0] != 4)
            return;

        BulletDict.Clear();
        int maxBullets = 1199 % 19;
        offset = 1;
        for (int i = 0; i < maxBullets; i += 19)
        {
            uint owner = System.BitConverter.ToUInt32(bulletPacket, offset);
            offset += 1;
            short bulletLife = System.BitConverter.ToInt16(bulletPacket, offset); // updated every tick by server
            offset += 2;
            float bulletX = System.BitConverter.ToSingle(bulletPacket, offset);
            offset += 4;
            float bulletZ = System.BitConverter.ToSingle(bulletPacket, offset);
            offset += 4;
            float bulletVelocityX = System.BitConverter.ToSingle(bulletPacket, offset);
            offset += 4;
            float bulletVelocityZ = System.BitConverter.ToSingle(bulletPacket, offset);
            if (owner == currentPlayerKey)
            {
                continue;
            }
            if (bulletLife == 0)
            {
                continue;
            }
            GameObject bullet = Instantiate(BulletPrefab, new Vector3(bulletX, 0, bulletZ), new Quaternion());
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletVelocityX;
            bullet.GetComponent<Bullet>().BulletLifeTime = bulletLife;
            bullet.GetComponent<Bullet>().src = PlayerDict[owner];
        }

    }

    // Called by Player.cs to add their own bullets to ClientController's OwnBulletDict
    public void AddBullet(GameObject bulletObj)
    {
        OwnBulletDict.Add(bulletObj.GetComponent<Bullet>().BulletId, bulletObj);
    }

    // Called by Bullet.cs to add their own bullets to ClientController's OwnBulletDict
    public void RemoveBullet(Bullet bullet)
    {
        OwnBulletDict.Remove(bullet.BulletId);
    }

    // must populate clientData byte array first
    // Returns a list of player coordinates
    List<playerCoord> getPlayersCoord()
    {
        int offset = 13;
        playerCoord player;
        List<playerCoord> coords = new List<playerCoord>();
        for (uint i = 0; i < NumberOfPlayers; i++)
        {
            // Converts Bytes to coordinates
            float x = System.BitConverter.ToSingle(clientData, offset);
            offset += 4;
            float z = System.BitConverter.ToSingle(clientData, offset);
            offset += 4;
            float rotation = System.BitConverter.ToSingle(clientData, offset);
            offset += 4;

            // Fills out the playerCoord structure
            // This assumes all players are in order
            player.id = i;
            player.position = new Vector3(x, 0, z);
            player.rotation = Quaternion.Euler(new Vector3(0, rotation, 0));

            // Add player to the list
            coords.Add(player);
        }
        return coords;
    }

}
