using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is the core script for the server.

public class gameServer : MonoBehaviour
{
	private TerrainController terrainController;
	private Server server;
    byte[] clientData = new byte[1200];
    server.Broadcast(ByteArray)
	server.SendTo(Server.Connections[0], ByteArray);
	
	// Use this for initialization
	void Start () {
        byte playerID = 0;
        this.terrainController = new TerrainController();
        while (!this.terrainController.GenerateEncoding());
        TerrainController.Encoding encoded = terrainController.Data;
        int offset = 13;

        // Make a terrain packet with encoded
        // Make the network call to send the terrain packet

        long tileWidth = this.terrainController.Width;
        long tileLength = this.terrainController.Length;

        for (int i  = 0; i < server.getNumConnections(); i++)
        {
            // Sets the coordinates for each player connected
            double playerX = 0 + playerID;
            double playerY = 0 + playerID;

            System.Buffer.BlockCopy(System.BitConverter.GetBytes(playerX), 0, clientData, offset, 4);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(playerY), 0, clientData, offset + 4, 4);

            offset += 8;
            playerID++;
        }

        playerID = 0;

        for (int i = 0; i < server.getNumConnections(); i++)
        {
            // Sets the player ID before being sent
            clientData[372 + playerID] = playerID;

            playerID++;
            // Send each player their spawn location and ID
            // Need to send clientData here
        }

        // Wait for all IDs to be echoed back as ACK, retransit ID on timeout
        // Set the game timer
        // Start the game timer

    }

    private float nextTickTime = 0.0f;
    private static int ticksPerSecond = 32;
    private float tickTime = (1 / ticksPerSecond);
    private int ticks = 0;

    byte[] toBroadcast = new byte[1200];

    void Update()
    {
        if (Time.time > nextTickTime)
        {
            ticks++;
            nextTickTime += tickTime;
            Debug.Log("Tick Number: " + ticks);

            // Receive data from each client

            // Clear toBroadcast
            Array.Clear(toBroadcast, 0, toBroadcast.Length);

            // Add all clients coordinates to toBroadcast

            // Broadcast update to all connections
            server.Broadcast(toBroadcast);


            // TODO: Bullets
            //      for(bullet in bullets) {
            //          for(player in players) {
            //              //copied http://cgp.wikidot.com/circle-to-circle-collision-detection
            //              //compare the distance to combined radii
            //              int dx = x2 - x1;
            //              int dy = y2 - y1;
            //              int radii = radius1 + radius2;
            //              if ( ( dx * dx )  + ( dy * dy ) < radii * radii ) 
            //              {
            //                  Console.WriteLine("The 2 circles are colliding!");
            //              }
            //          }
            //      }

            // TODO: Unit collision
        }
    }
}
