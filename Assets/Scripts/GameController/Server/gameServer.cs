using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is the core script for the server.

public class gameServer : MonoBehaviour
{
	private TerrainController terrainController;
	//private Server server;
	//server.Broadcast(ByteArray)
	//server.SendTo(Server.Connections[0], ByteArray);
	
	// Use this for initialization
	void Start () {
		//this.terrainController = new TerrainController();
		//while (!this.terrainController.GenerateEncoding());
		//TerrainController.Encoding encoded = terrainController.Data;
		
		// Make a terrain packet (byte array) with encoded
		byte[] terrainPacket = new byte[1200];
		
		// Set header
		terrainPacket[0] = 15;
		//Add data
		foreach (int i in encoded.tiles)
		{
			
		}
		
		
		// Make the network call to send the terrain packet

		// Generate player spawns
		// Set player IDs (1B available)
		// Send each player their spawn location and ID

		// Wait for all IDs to be echoed back as ACK, retransit ID on timeout
		// Set the game timer
		// Start the game timer

	}
	
	// Update is called once per frame
	//void Update () {
//		Receive data from each client
//		Update local data		
//		Check for bullet - player collisions
//		for(bullet in bullets) {
//			for(player in players) {
//				//copied http://cgp.wikidot.com/circle-to-circle-collision-detection
//				//compare the distance to combined radii
//				int dx = x2 - x1;
//				int dy = y2 - y1;
//				int radii = radius1 + radius2;
//				if ( ( dx * dx )  + ( dy * dy ) < radii * radii ) 
//				{
//					Console.WriteLine("The 2 circles are colliding!");
//				}
//			}
//		}
//
//		Apply control input to player coordinates
//	
//	
//		Send updated data (either one tick packet, or a tick packet and a bullet packet) to each player
//
	//}

    private float nextTickTime = 0.0f;
    private static int ticksPerSecond = 32;
    private float tickTime = (1 / ticksPerSecond);
    private int ticks = 0;

    void Update()
    {
        if (Time.time > nextTickTime)
        {
            ticks++;
            nextTickTime += tickTime;
            Debug.Log("Tick Number: " + ticks);
        }
    }
}
