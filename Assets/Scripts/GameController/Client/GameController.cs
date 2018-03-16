using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Networking;

public class GameController : MonoBehaviour {

    public const string SERVER_ADDRESS = "192.168.0.20";

    private byte currentPlayerId;

    private Dictionary<byte, GameObject> players;

    byte[] buffer;
    private Client client;

    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;

    void Start()
    {
        currentPlayerId = 0;
        players = new Dictionary<byte, GameObject>();

        buffer = new byte[R.Net.Size.SERVER_TICK];
        client = new Client();
        client.Init(SERVER_ADDRESS, R.Net.PORT);

        byte[] initPacket = new byte[R.Net.Size.CLIENT_TICK];
        initPacket[0] = R.Net.Header.NEW_CLIENT;

        client.Send(initPacket, R.Net.Size.CLIENT_TICK);
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

        if (client.Poll())
        {
            return;
        }

        if (client.Recv(buffer, R.Net.Size.SERVER_TICK) < R.Net.Size.SERVER_TICK)
        {
            return;
        }

        List<byte> playerIDs = this.getPlayerIDs(buffer);
        List<Vector3> positions = this.getPlayerPositions(buffer);
        List<Quaternion> rotations = this.getPlayerRotations(buffer);

        // Add any new players
        if (playerIDs.Count > this.players.Count)
        {
            for (int i = 0; i < playerIDs.Count; i++)
            {
                if (this.players.ContainsKey(playerIDs[i]))
                {
                    continue;
                }
                this.addPlayer(playerIDs[i], positions[i], rotations[i]);
            }
        }

        this.movePlayers(playerIDs, positions, rotations);
    }

    void syncWithServer()
    {
        if (this.client.Poll())
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

        this.currentPlayerId = buffer[R.Net.Offset.PID];

        float x = BitConverter.ToSingle(buffer, R.Net.Offset.PLAYER_POSITIONS + (this.currentPlayerId * 8));
        float z = BitConverter.ToSingle(buffer, R.Net.Offset.PLAYER_POSITIONS + (this.currentPlayerId * 8) + 4);
        float r = BitConverter.ToSingle(buffer, R.Net.Offset.PLAYER_ROTATIONS + (this.currentPlayerId * 4));
        
        this.addPlayer(this.currentPlayerId, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, r, 0)));
    }

    List<byte> getPlayerIDs(byte[] data)
    {
        List<byte> playerIDs = new List<byte>();
        for (int i = 0; i < 30; i++)
        {
            byte id = data[R.Net.Offset.PID + i];

            if (id == 0)
            {
                return playerIDs;
            }

            playerIDs.Add(id);
        }
        return playerIDs;
    }

    List<Vector3> getPlayerPositions(byte[] data)
    {
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < 30; i++)
        {
            float x = BitConverter.ToSingle(data, R.Net.Offset.PLAYER_POSITIONS + (i * 8));
            float z = BitConverter.ToSingle(data, R.Net.Offset.PLAYER_POSITIONS + (i * 8) + 4);
            Vector3 position = new Vector3(x, 0, z);
            positions.Add(position);
        }
        return positions;
    }

    List<Quaternion> getPlayerRotations(byte[] data)
    {
        List<Quaternion> rotations = new List<Quaternion>();
        for (int i = 0; i < 30; i++)
        {
            float pheta = BitConverter.ToSingle(data, R.Net.Offset.PLAYER_ROTATIONS + (i * 4));
            Quaternion rotation = Quaternion.Euler(new Vector3(0, pheta, 0));
            rotations.Add(rotation);
        }
        return rotations;
    }

    void addPlayer(byte id, Vector3 position, Quaternion rotation)
    {
        GameObject player;

        if (id == this.currentPlayerId)
        {
            player = (GameObject)Instantiate(this.PlayerPrefab, position, rotation);
        }
        else
        {
            player = (GameObject)Instantiate(this.EnemyPrefab, position, rotation);
        }

        this.players.Add(id, player);
    }

    void movePlayers(List<byte> playerIDs, List<Vector3> positions, List<Quaternion> rotations)
    {
        try
        {
            for (int i = 0; i < playerIDs.Count; i++)
            {
                if (playerIDs[i] == this.currentPlayerId)
                {
                    continue;
                }
                Debug.Log("Applying position : " + positions[i] + " and rotation : " + rotations[i] + " to id : " + playerIDs[i]);
                this.players[playerIDs[i]].transform.position = positions[i];
                this.players[playerIDs[i]].transform.rotation = rotations[i];
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
        byte[] packet = new byte[R.Net.Size.CLIENT_TICK];

        // Put position data into the packet
        packet[0] = R.Net.Header.TICK;
        packet[1] = this.currentPlayerId;
        Array.Copy(x    , 0, packet, index,  4);
        index += 4;
        Array.Copy(z    , 0, packet, index,  4);
        index += 4;
        Array.Copy(pheta, 0, packet, index, 4);
        index += 4;

        /*
        // The reference to the stack of fired bullets does not work

        Stack<Bullet> playerBullets = this.players[this.currentPlayerId].GetComponent<Gun>().FiredShots;

        while (playerBullets.Count > 0)
        {
            Bullet bullet = playerBullets.Pop();
            byte[] bulletID = BitConverter.GetBytes(bullet.ID);

            Array.Copy(bulletID, 0, packet, index, 4);
            index += 4;

            packet[index] = bullet.Type;
        }
        */

        Debug.Log(this.client.Send(packet, R.Net.Size.CLIENT_TICK));
    }
}
