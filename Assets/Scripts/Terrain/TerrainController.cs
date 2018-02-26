using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*---------------------------------------------------------------------------------------
--	SOURCE FILE:	TerrainController.cs
--
--	PROGRAM:		TerrainController
--
--	FUNCTIONS:		public TerrainController()
--                  public bool GenerateEncoding()
--                  public bool Instantiate()
--
--	DATE:			Feb 16th, 2018
--
--	REVISIONS:		Feb 24th, 2018
--
--	DESIGNERS:		Angus Lam, Benny Wang, Roger Zhang
--
--	PROGRAMMER:		Angus Lam, Benny Wang, Roger Zhang
--
--	NOTES:
--     This is a csharp script to initialize a 2D terrain consists of
--     cactus, bushes and ground.
---------------------------------------------------------------------------------------*/
public class TerrainController
{
    /*
     * Tile Types
     * Ground = 0
     * Cactus = 1
     * Bush = 2
     */
    enum TileTypes
    {
        GROUND,
        CACTUS,
        BUSH
    };

    /*
     * Encoding structure
     * tiles - 2D array
     * buildings - 1D building array
     */
    public struct Encoding
    {
        public int[,] tiles;
        public Building[] buildings;
    };
    public Encoding Data { get; set; }
    public byte[] CompressedData { get; set; }

    // Width of the terrain
    public long Width { get; set; }

    // Length of the terrain
    public long Length { get; set; }

    // Tile size
    public long TileSize { get; set; }

    // Cactus appearing percent
    public double CactusCoeff { get; set; }

    // Bush appearing percent
    public double BushCoeff { get; set; }

    // Building array of building objects
    public Building[] Buildings { get; set; }

    // Cactus gameobject prefab
    public GameObject CactusPrefab { get; set; }
    // Bush gameobject prefab
    public GameObject BushPrefab { get; set; }

    // Define default constants
    public const long DEFAULT_WIDTH = 1000;
    public const long DEFAULT_LENGTH = 1000;
    public const long DEFAULT_TILE_SIZE = 20;
    public const long DEFAULT_CACTUS_COEFF = 1;
    public const long DEFAULT_BUSH_COEFF = 1;
    public const string DEFAULT_NAME = "Terrain";

    /*-------------------------------------------------------------------------------------------------
    -- FUNCTION: TerrainController()
    --
    -- DATE: Feb 16th, 2018
    --
    -- REVISIONS: N/A
    --
    -- DESIGNER: Angus Lam, Benny Wang, Roger Zhang
    --
    -- PROGRAMMER: Angus Lam, Benny Wang, Roger Zhang
    --
    -- INTERFACE: TerrainController()
    --
    -- RETURNS: void
    --
    -- NOTES:
    -- The constructor for terrainController, sets the default width, height,
    -- Cactus, bush percent values for the terrain.
    -------------------------------------------------------------------------------------------------*/
    public TerrainController()
    {
        this.Width = DEFAULT_WIDTH;
        this.Length = DEFAULT_LENGTH;
        this.TileSize = DEFAULT_TILE_SIZE;
        this.CactusCoeff = DEFAULT_CACTUS_COEFF;
        this.BushCoeff = DEFAULT_BUSH_COEFF;
    }

    /*-------------------------------------------------------------------------------------------------
    -- FUNCTION: GenerateEncoding()
    --
    -- DATE: Feb 18, 2018
    --
    -- REVISIONS: N/A
    --
    -- DESIGNER: Roger Zhang
    --
    -- PROGRAMMER: Roger Zhang
    --
    -- INTERFACE: GenerateEncoding()
    --
    -- RETURNS: void
    --
    -- NOTES:
    -- Generates an encoded 2D array with given width and height.
    -- Populates the map array with tile types based on given coefficients.
    -------------------------------------------------------------------------------------------------*/
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
                    double randomValue = Random.value * 101;
                    if (this.CactusCoeff < this.BushCoeff)
                    {
                        if (randomValue < this.CactusCoeff)
                        {
                            map[i, j] = (int)TileTypes.CACTUS;
                        }
                        else if (randomValue < this.BushCoeff)
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
                        if (randomValue < this.BushCoeff)
                        {
                            map[i, j] = (int)TileTypes.BUSH;
                        }
                        else if (randomValue < this.CactusCoeff)
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

        this.Data = new Encoding() { tiles = map };
        this.compressData();

        return true;
    }

    /*-------------------------------------------------------------------------------------------------
    -- FUNCTION: compressData()
    --
    -- DATE: Jan 23, 2018
    --
    -- REVISIONS: N/A
    --
    -- DESIGNER: Benny Wang 
    --
    -- PROGRAMMER: Benny Wang 
    --
    -- INTERFACE: compressData()
    --
    -- RETURNS: void
    --
    -- NOTES:
    -- Compresses whatever is inside the Data member variable of this class and places it into the
    -- CompressedData member of this class.
    -------------------------------------------------------------------------------------------------*/
    private void compressData()
    {
        byte[] tmp = { };
        List<byte> compressed = new List<byte>();

        tmp = System.BitConverter.GetBytes(this.Width);
        foreach (byte t in tmp)
        {
            compressed.Add(t);
        }
        tmp = System.BitConverter.GetBytes(this.Length);
        foreach (byte t in tmp)
        {
            compressed.Add(t);
        }

        for (int i = 0; i < this.Data.tiles.GetLength(0); i++)
        {
            for (int j = 0; j < this.Data.tiles.GetLength(1); j++)
            {
                tmp = System.BitConverter.GetBytes(this.Data.tiles[i, j]);
                foreach (byte t in tmp)
                {
                    compressed.Add(t);
                }
            }
        }

        this.CompressedData = compressed.ToArray();
    }

    /*-------------------------------------------------------------------------------------------------
    -- FUNCTION: LoadByteArray()
    --
    -- DATE: Jan 23, 2018
    --
    -- REVISIONS: N/A
    --
    -- DESIGNER: Benny Wang 
    --
    -- PROGRAMMER: Benny Wang 
    --
    -- INTERFACE: LoadByteArray(byte[] compressed)
    --                  byte[] compressed: A byte array containt the terrain data.
    --
    -- RETURNS: void
    --
    -- NOTES:
    -- Takes in a byte array representation of the terrain data that was send over by the server and
    -- loads it.
    -------------------------------------------------------------------------------------------------*/
    public void LoadByteArray(byte[] compressed)
    {
        this.Width = System.BitConverter.ToInt32(compressed, 0);
        this.Length = System.BitConverter.ToInt32(compressed, 4);

        int[,] map = new int[this.Width, this.Length];
        for (int i = 8, x = 0, y = 0; i < compressed.Length; i += 4)
        {
            map[x, y] = System.BitConverter.ToInt32(compressed, i);
            x++;
            y++;
        }

        this.Data = new Encoding() { tiles = map, buildings = { } };
    }

    /*-------------------------------------------------------------------------------------------------
    -- FUNCTION: compressData()
    --
    -- DATE: Jan 23, 2018
    --
    -- REVISIONS: N/A
    --
    -- DESIGNER: Benny Wang 
    --
    -- PROGRAMMER: Benny Wang 
    --
    -- INTERFACE: compressData()
    --
    -- RETURNS: void
    --
    -- NOTES:
    -- Compresses whatever is inside the Data member variable of this class and places it into the
    -- CompressedData member of this class.
    -------------------------------------------------------------------------------------------------*/
    private void compressData()
    {
        byte[] tmp = { };
        List<byte> compressed = new List<byte>();

        tmp = System.BitConverter.GetBytes(this.Width);
        foreach (byte t in tmp)
        {
            compressed.Add(t);
        }
        tmp = System.BitConverter.GetBytes(this.Length);
        foreach (byte t in tmp)
        {
            compressed.Add(t);
        }

        for (int i = 0; i < this.Data.tiles.GetLength(0); i++)
        {
            for (int j = 0; j < this.Data.tiles.GetLength(1); j++)
            {
                tmp = System.BitConverter.GetBytes(this.Data.tiles[i, j]);
                foreach (byte t in tmp)
                {
                    compressed.Add(t);
                }
            }
        }

        this.CompressedData = compressed.ToArray();
    }

    /*-------------------------------------------------------------------------------------------------
    -- FUNCTION: LoadByteArray()
    --
    -- DATE: Jan 23, 2018
    --
    -- REVISIONS: N/A
    --
    -- DESIGNER: Benny Wang 
    --
    -- PROGRAMMER: Benny Wang 
    --
    -- INTERFACE: LoadByteArray(byte[] compressed)
    --                  byte[] compressed: A byte array containt the terrain data.
    --
    -- RETURNS: void
    --
    -- NOTES:
    -- Takes in a byte array representation of the terrain data that was send over by the server and
    -- loads it.
    -------------------------------------------------------------------------------------------------*/
    public void LoadByteArray(byte[] compressed)
    {
        this.Width = System.BitConverter.ToInt32(compressed, 0);
        this.Length = System.BitConverter.ToInt32(compressed, 4);

        int[,] map = new int[this.Width, this.Length];
        for (int i = 8, x = 0, y = 0; i < compressed.Length; i += 4)
        {
            map[x, y] = System.BitConverter.ToInt32(compressed, i);
            x++;
            y++;
        }

        this.Data = new Encoding() { tiles = map, buildings = { } };
    }

    /*-------------------------------------------------------------------------------------------------
    -- FUNCTION: Instantiate()
    --
    -- DATE: Jan 23, 2018
    --
    -- REVISIONS: N/A
    --
    -- DESIGNER: Angus Lam
    --
    -- PROGRAMMER: Angus Lam
    --
    -- INTERFACE: Instantiate()
    --
    -- RETURNS: boolean
    --
    -- NOTES:
    -- Creates TerrainData and set its relevant values.
    -- Instantiate the Terrain GameObject and set its name and position.
    -------------------------------------------------------------------------------------------------*/
    public bool Instantiate()
    {
        TerrainData tData = new TerrainData
        {
            size = new Vector3(Width, 0, Length),
            name = DEFAULT_NAME
        };

        GameObject terrain = (GameObject)Terrain.CreateTerrainGameObject(tData);

        terrain.name = DEFAULT_NAME;
        terrain.transform.position = new Vector3(-Width / 2, 0, -Length / 2);

        return true;
    }
}

/*------------------------------------------------------------------------------------------------------------------
-- SOURCE FILE: TerrainController.cs
--
-- PROGRAM: Building
--
-- FUNCTIONS: public class Building
--
-- DATE: Feb 24th, 2018
--
-- REVISIONS: N/A
--
-- DESIGNER: 
--
-- PROGRAMMER: 
--
-- NOTES:
-- This is the building initializer to create the buildings and returns an array
-- of building objects.
----------------------------------------------------------------------------------------------------------------------*/
public class Building
{
    Building()
    {

    }
}
