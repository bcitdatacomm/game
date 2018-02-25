using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public Player ownPlayer;
    public GameObject otherPlayerPrefab;
    public int numberOfPlayers;

    // Use this for initialization
    void Start ()
    { 
        Vector3 ownSpawnPos = new Vector3(Random.Range(-8.0f, 8.0f), 1.0f, Random.Range(-8.0f, 8.0f));
        Quaternion ownSpawnRot = Quaternion.Euler(0.0f, Random.Range(0, 180), 0.0f);
        GameObject ownTemp = Instantiate(playerPrefab, ownSpawnPos, ownSpawnRot);
        ownPlayer = (Player)ownTemp.GetComponent<Player>();

		// grab other player's data from server and unpacketize
        for (int i = 0; i < numberOfPlayers; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-8.0f, 8.0f), 1.0f, Random.Range(-8.0f, 8.0f));

            Quaternion spawnRotation = Quaternion.Euler(0.0f, Random.Range(0, 180), 0.0f);

            GameObject temp = (GameObject)Instantiate(otherPlayerPrefab, spawnPosition, spawnRotation);

            ownPlayer.otherPlayerList.Add(temp);
        }
	}
	
	void FixedUpdate ()
    {
        MoveOther();
    }

    void MoveOther()
    {
        // get other player's x, z position and y rotation
        foreach (GameObject oPlayer in ownPlayer.otherPlayerList)
        {
            Vector3 movement = new Vector3(Random.Range(-1.0f, 1.0f), 1.0f, Random.Range(-1.0f, 1.0f));
            Quaternion yRotation = Quaternion.Euler(0.0f, Random.Range(0, 180), 0.0f);
            Rigidbody rBody = oPlayer.GetComponent<Rigidbody>();
            rBody.MovePosition(oPlayer.transform.position + movement);
            rBody.MoveRotation(yRotation);
        }
    }
}
