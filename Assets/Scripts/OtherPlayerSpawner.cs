using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerSpawner : MonoBehaviour {

    public List<GameObject> otherPlayerList;
    public GameObject otherPlayerPrefab;
    public int numberOfPlayers;

    // Use this for initialization
    void Start ()
    {
        // server call to grab otherPlayer data
        // unpacketize
	    for (int i = 0; i < numberOfPlayers; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-10.0f, 10.0f), 1.0f, Random.Range(-10.0f, 10.0f));
            Quaternion spawnRotation = Quaternion.Euler(0.0f, Random.Range(0, 180), 0.0f);
            GameObject temp = (GameObject)Instantiate(otherPlayerPrefab, spawnPosition, spawnRotation);

            otherPlayerList.Add(temp);
        }
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
