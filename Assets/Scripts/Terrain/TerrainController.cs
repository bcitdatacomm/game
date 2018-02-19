﻿using UnityEngine;
using System.Collections;

public class TerrainController
{
    enum TileTypes
    {
        GROUND,
        CACTUS,
        BUSH
    };

    public struct Encoding
    {
        int[,] tiles;
        Building[] buildings;
    };

    public Encoding Data { get; set; }

    public long Width { get; set; }
    public long Length { get; set; }
    public long TileSize { get; set; }

    public double CactusCoeff { get; set; }
    public double BushCoeff { get; set; }

    public Building[] Buildings { get; set; }

    public GameObject CactusPrefab { get; set; }
    public GameObject BushPrefab { get; set; }

    // Define default constants
    public const long DEFAULT_WIDTH = 1000;
    public const long DEFAULT_LENGTH = 1000;
    public const long DEFAULT_TILE_SIZE = 20;
    public const long DEFAULT_CACTUS_COEFF = 1;
    public const long DEFAULT_BUSH_COEFF = 1;
    public const string DEFAULT_NAME = "Terrain";

    // Assigns default values to the object
    public TerrainController()
    {
        this.Width = DEFAULT_WIDTH;
        this.Length = DEFAULT_LENGTH;
        this.TileSize = DEFAULT_TILE_SIZE;
        this.CactusCoeff = DEFAULT_CACTUS_COEFF;
        this.BushCoeff = DEFAULT_BUSH_COEFF;
    }

    public bool GenerateEncoding()
    {
        int[,] map = new int[this.Width, this.Length];

        for (long i = 0; i < this.Width; i++)
        {
            for (long j = 0; j < this.Length; j++)
            {
                // Check for border
                if (i == 0 || i == this.Width || j == 0 || j == this.Length)
                {
                    map[i, j] = (int)TileTypes.GROUND;
                }
                else
                {
                    if (this.CactusCoeff < this.BushCoeff)
                    {
                        if (Random.value * 101 < this.CactusCoeff)
                        {
                            map[i, j] = (int)TileTypes.CACTUS;
                        }
                        else if (Random.value * 101 < this.BushCoeff)
                        {
                            map[i, j] = (int)TileTypes.BUSH;
                        }
                        else
                        {
                            map[i, j] = (int)TileTypes.GROUND;
                        }
                    }
                    else
                    {
                        if (Random.value * 101 < this.BushCoeff)
                        {
                            map[i, j] = (int)TileTypes.BUSH;
                        }
                        else if (Random.value * 101 < this.CactusCoeff)
                        {
                            map[i, j] = (int)TileTypes.CACTUS;
                        }
                        else
                        {
                            map[i, j] = (int)TileTypes.GROUND;
                        }
                    }
                }
            }
        }
        return true;
    }

    // This instantiates the Terrain GameObject
    public bool Instantiate()
    {
        // Create TerrainData and set its relevant values
        TerrainData tData = new TerrainData
        {
            size = new Vector3(Width, 0, Length),
            name = DEFAULT_NAME
        };

        // Instantiate the Terrain GameObject and set its name and position
        GameObject terrain = (GameObject)Terrain.CreateTerrainGameObject(tData);

        terrain.name = DEFAULT_NAME;
        terrain.transform.position = new Vector3(-Width / 2, 0, -Length / 2);

        return true;
    }
}


public class Building
{
    Building()
    {

    }
}