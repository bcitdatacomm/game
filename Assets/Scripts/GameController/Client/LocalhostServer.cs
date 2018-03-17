using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Networking;

public class LocalhostServer : MonoBehaviour {

	Client currentClient;
	const int PACKET_SIZE = 1200;

	// Use this for initialization
	void Start () {
		currentClient = new Client();
		currentClient.Init("127.0.0.1", 7000);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("sending packet1");

			byte[] playerPacket = new byte[PACKET_SIZE];
			// curent player
			float playerX = 1.0f;//Random.Range(-4.0f, 4.0f);
			float playerZ = 1.0f;//Random.Range(-4.0f, 4.0f);
			float playerRot = 12.0f;
			uint currentPlayerKey = 0;
			playerPacket[0] = 2;

			byte[] playerIdBytes = System.BitConverter.GetBytes(currentPlayerKey);
			System.Array.Copy(playerIdBytes, 0, playerPacket, 373, playerIdBytes.Length );

			int offset = 13;
	        byte[] playerXBytes = System.BitConverter.GetBytes(playerX);
	        System.Array.Copy(playerXBytes, 0, playerPacket, offset, playerXBytes.Length);
	        offset += 4;
	        byte[] playerZBytes = System.BitConverter.GetBytes(playerZ);
	        System.Array.Copy(playerZBytes, 0, playerPacket, offset, playerZBytes.Length);
	        offset += 4;
	        byte[] playerRotBytes = System.BitConverter.GetBytes(playerRot);
	        System.Array.Copy(playerRotBytes, 0, playerPacket, offset, playerRotBytes.Length);
			offset += 4;

			// player 2, dummy player
			System.Array.Clear(playerIdBytes, 0, playerIdBytes.Length);
			System.Array.Clear(playerXBytes, 0, playerXBytes.Length);
			System.Array.Clear(playerZBytes, 0, playerZBytes.Length);
			System.Array.Clear(playerRotBytes, 0, playerRotBytes.Length);
			playerX = Random.Range(-1.0f, 1.0f);
			playerZ = Random.Range(-1.0f, 1.0f);
			playerRot = 12.0f;
			currentPlayerKey = 1;

			playerIdBytes = System.BitConverter.GetBytes(currentPlayerKey);
			System.Array.Copy(playerIdBytes, 0, playerPacket, 373+1, playerIdBytes.Length );

			playerXBytes = System.BitConverter.GetBytes(playerX);
			System.Array.Copy(playerXBytes, 0, playerPacket, offset, playerXBytes.Length);
			offset += 4;
			playerZBytes = System.BitConverter.GetBytes(playerZ);
			System.Array.Copy(playerZBytes, 0, playerPacket, offset, playerZBytes.Length);
			offset += 4;
			playerRotBytes = System.BitConverter.GetBytes(playerRot);
			System.Array.Copy(playerRotBytes, 0, playerPacket, offset, playerRotBytes.Length);
			offset += 4;

			// player 3, dummy player
			System.Array.Clear(playerIdBytes, 0, playerIdBytes.Length);
			System.Array.Clear(playerXBytes, 0, playerXBytes.Length);
			System.Array.Clear(playerZBytes, 0, playerZBytes.Length);
			System.Array.Clear(playerRotBytes, 0, playerRotBytes.Length);
			playerX = Random.Range(-1.0f, 1.0f);
			playerZ = Random.Range(-1.0f, 1.0f);
			playerRot = 12.0f;
			currentPlayerKey = 2;

			playerIdBytes = System.BitConverter.GetBytes(currentPlayerKey);
			System.Array.Copy(playerIdBytes, 0, playerPacket, 373+2, playerIdBytes.Length );

			playerXBytes = System.BitConverter.GetBytes(playerX);
			System.Array.Copy(playerXBytes, 0, playerPacket, offset, playerXBytes.Length);
			offset += 4;
			playerZBytes = System.BitConverter.GetBytes(playerZ);
			System.Array.Copy(playerZBytes, 0, playerPacket, offset, playerZBytes.Length);
			offset += 4;
			playerRotBytes = System.BitConverter.GetBytes(playerRot);
			System.Array.Copy(playerRotBytes, 0, playerPacket, offset, playerRotBytes.Length);

	        currentClient.Send(playerPacket, PACKET_SIZE);

        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
			Debug.Log("sending packet2");
			byte[] updatePacket = new byte[PACKET_SIZE];
	        updatePacket[0] = 2;
	        currentClient.Send(updatePacket, PACKET_SIZE);
        }
	}
}
