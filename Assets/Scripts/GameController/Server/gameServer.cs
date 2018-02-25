using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is the core script for the server.

public class gameServer : MonoBehaviour
{
	private TerrainController terrainController;
	
	// Use this for initialization
	void Start () {
		this.terrainController = new TerrainController();
		while (!this.terrainController.GenerateEncoding());
		TerrainController.Encoding encoded = terrainController.Data;
		
		// Make a terrain packet with encoded
		// Make the network call to send the terrain packet

		// Generate player spawns
		// Set player IDs (1B available)
		// Send each player their spawn location and ID

		// Wait for all IDs to be echoed back as ACK, retransit ID on timeout
		// Set the game timer
		// Start the game timer

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
