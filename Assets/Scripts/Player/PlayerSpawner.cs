using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject OtherPlayerPrefab;
    public int NumberOfPlayers = 9;
    public List<GameObject> OtherPlayerList { get; set; }

    // Use this for initialization
    void Start()
    { 
		// grab other player's data from server and unpacketize
        for (int i = 0; i < NumberOfPlayers; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-8.0f, 8.0f), 1.0f, Random.Range(-8.0f, 8.0f));

            Quaternion spawnRotation = Quaternion.Euler(0.0f, Random.Range(0, 180), 0.0f);

            GameObject temp = (GameObject)Instantiate(OtherPlayerPrefab, spawnPosition, spawnRotation);

            OtherPlayerList.Add(temp);
        }
	}
	
	void FixedUpdate()
    {
        MoveOther();
    }

    void MoveOther()
    {
        // get other player's x, z position and y rotation from server
        foreach (GameObject otherPlayer in OtherPlayerList)
        {
            Vector3 movement = new Vector3(Random.Range(-1.0f, 1.0f), 1.0f, Random.Range(-1.0f, 1.0f));
            Quaternion yRotation = Quaternion.Euler(0.0f, Random.Range(0, 180), 0.0f);
            Rigidbody rBody = otherPlayer.GetComponent<Rigidbody>();
            rBody.MovePosition(otherPlayer.transform.position + movement);
            rBody.MoveRotation(yRotation);
        }
    }
}
