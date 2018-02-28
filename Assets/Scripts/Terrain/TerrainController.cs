using UnityEngine;
using System.Collections;
using System.IO;
using System.IO.Compression;
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
        public byte[,] tiles;
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
    public const long DEFAULT_CACTUS_COEFF = 30; //30%
    public const long DEFAULT_BUSH_COEFF = 30;
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
    -- RETURNS: boolean
    --
    -- NOTES:
    -- Generates an encoded 2D array with given width and height.
    -- Populates the map array with tile types based on given coefficients.
    -------------------------------------------------------------------------------------------------*/
    public bool GenerateEncoding()
    {
        byte[,] map = new byte[this.Width, this.Length];

        for (long i = 0; i < this.Width; i++)
        {
            for (long j = 0; j < this.Length; j++)
            {
                // Check for border
                if (i == 0 || i == this.Width || j == 0 || j == this.Length)
                {
                    map[i, j] = (byte)TileTypes.GROUND;
                }
                else
                {
                    double randomValue = Random.value * 101;
                    if (this.CactusCoeff < this.BushCoeff)
                    {
                        if (randomValue < this.CactusCoeff)
                        {
                            map[i, j] = (byte)TileTypes.CACTUS;
                        }
                        else if (randomValue < this.BushCoeff)
                        {
                            map[i, j] = (byte)TileTypes.BUSH;
                        }
                        else
                        {
                            map[i, j] = (byte)TileTypes.GROUND;
                        }
                    }
                    else
                    {
                        if (randomValue < this.BushCoeff)
                        {
                            map[i, j] = (byte)TileTypes.BUSH;
                        }
                        else if (randomValue < this.CactusCoeff)
                        {
                            map[i, j] = (byte)TileTypes.CACTUS;
                        }
                        else
                        {
                            map[i, j] = (byte)TileTypes.GROUND;
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
                compressed.Add(this.Data.tiles[i, j]);
            }
        }

        this.CompressedData = compressed.ToArray();
        this.CompressedData = compressByteArray();
    }

    /*-------------------------------------------------------------------------------------------------
    -- FUNCTION: compressByteArray()
    --
    -- DATE: Feb 28, 2018
    --
    -- REVISIONS: N/A
    --
    -- DESIGNER: Roger Zhang 
    --
    -- PROGRAMMER: Roger Zhang 
    --
    -- INTERFACE: compressByteArray()
    --
    -- RETURNS: byte array of compressed data
    --
    -- NOTES:
    -- Compress the byteArrayData to a smaller size using system I/O.
    -------------------------------------------------------------------------------------------------*/
    private byte[] compressByteArray()
    {
        MemoryStream compressedBA = new MemoryStream();
        DeflateStream cstream = new DeflateStream(compressedBA, CompressionMode.Compress, true);
        cstream.Write(this.CompressedData, 0, this.CompressedData.Length);
        cstream.Close();
        return compressedBA.ToArray();
    }

    /*-------------------------------------------------------------------------------------------------
    -- FUNCTION: compressByteArray()
    --
    -- DATE: Feb 28, 2018
    --
    -- REVISIONS: N/A
    --
    -- DESIGNER: Roger Zhang 
    --
    -- PROGRAMMER: Roger Zhang 
    --
    -- INTERFACE: decompressByteArray(byte[] compBA)
    --                          compBA - compressed data passed in
    --
    -- RETURNS: byte array of decompressed data
    --
    -- NOTES:
    -- Decompress the byteArray to the normal byteArray size using system I/O.
    -------------------------------------------------------------------------------------------------*/
    private byte[] decompressByteArray(byte[] compBA)
    {
        MemoryStream decompressedBA = new MemoryStream(compBA);
        DeflateStream dstream = new DeflateStream(decompressedBA, CompressionMode.Decompress, true);
        int size = compBA.Length;
        byte[] decompressedEncoding = new byte[size];
        for (int i = 0; i < size; i++)
        {
            decompressedBA.Write(decompressedEncoding, 0, size);
        }
        dstream.Close();
        return decompressedBA.ToArray();
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
        // Decompress first
        byte[] decompressed = decompressByteArray(compressed);

        this.Width = System.BitConverter.ToInt64(decompressed, 0);
        this.Length = System.BitConverter.ToInt64(decompressed, 8);

        byte[,] map = new byte[this.Width, this.Length];

        for (int i = 16; i < decompressed.Length;)
        {
            for (long x = 0; x < this.Width; x++)
            {
                for (long y = 0; y < this.Length; y++)
                {
                    map[x, y] = decompressed[i];
                    i++;
                }
            }
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
