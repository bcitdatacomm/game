using UnityEngine;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using UnityEditor;

/*---------------------------------------------------------------------------------------
--	SOURCE FILE:	TerrainController.cs
--
--	PROGRAM:		TerrainController
--
--	FUNCTIONS:		public TerrainController()
--                  public bool GenerateEncoding()
--                  public bool Instantiate()
--                  public byte[] compressByteArray()
--                  public byte[] decompressByteArray()
--                  public void compressData()
--                  public void LoadByteArray()
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
        BUSH,
        BUILDINGS
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
    public float CactusPerc { get; set; }

    // Bush appearing percent
    public float BushPerc { get; set; }

    // Building array of building objects
    public Building[] Buildings { get; set; }

    // Cactus gameobject prefab
    public GameObject CactusPrefab { get; set; }
    // Bush gameobject prefab
    public GameObject BushPrefab { get; set; }

    //Occupied positions on the map
    public List<Vector2> occupiedPositions;

    // Define default constants
    public const long DEFAULT_WIDTH = 1000;
    public const long DEFAULT_LENGTH = 1000;
    public const long DEFAULT_TILE_SIZE = 20;
    public const long DEFAULT_COLLIDER_SIZE = 20;
    // Changed to a percentage - ALam
    public const float DEFAULT_CACTUS_PERC = 0.9997f;
    public const float DEFAULT_BUSH_PERC = 0.9995f;
    public const string DEFAULT_NAME = "Terrain";

    public const float DEFAULT_BUILDING_PERC = 0.9999f;
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
        this.CactusPerc = DEFAULT_CACTUS_PERC;
        this.BushPerc = DEFAULT_BUSH_PERC;
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
                    // Changed by Alam

                    // Changed back to just being 0.0 to 1.0
                    float randomValue = Random.value;

                    // Changed the comparison signs around
                    if (randomValue > DEFAULT_BUILDING_PERC)
                    {
                        map[i, j] = (byte)TileTypes.BUILDINGS;
                        //this.occupiedPositions.Add(new Vector2(i, j));
                    }
                    else if (randomValue > this.CactusPerc)
                    {
                        map[i, j] = (byte)TileTypes.CACTUS;
                        //this.occupiedPositions.Add(new Vector2(i, j));
                    }
                    else if (randomValue > this.BushPerc)
                    {
                        map[i, j] = (byte)TileTypes.BUSH;
                        //this.occupiedPositions.Add(new Vector2(i, j));
                    }
                    else
                    {
                        map[i, j] = (byte)TileTypes.GROUND;
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

        this.CompressedData = compressByteArray(compressed.ToArray());
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
    -- INTERFACE: compressByteArray(byte[] input)
    --              byte[] input: They byte array to compress.
    --
    -- RETURNS: byte array of compressed data
    --
    -- NOTES:
    -- Compress the byteArrayData to a smaller size using system I/O.
    -------------------------------------------------------------------------------------------------*/
    private byte[] compressByteArray(byte[] input)
    {
        MemoryStream compressedBA = new MemoryStream();
        DeflateStream cstream = new DeflateStream(compressedBA, CompressionMode.Compress, true);
        cstream.Write(input, 0, input.Length);
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
    -- DATE: March 16, 2018
    --
    -- REVISIONS: N/A
    --
    -- DESIGNER: Angus Lam & Roger Zhang
    --
    -- PROGRAMMER: Angus Lam & Roger Zhang
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

        ///////////////////////////////////////////////////

        // Gets the number of tile types
        int numTileTypes = TileTypes.GetNames(typeof(TileTypes)).Length;

        // Grab the rock prefabs
        GameObject rockPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Scenery/Rocks Pack/Rock1/Rock1_B.prefab", typeof(GameObject));
        GameObject cactusPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Scenery/Rocks Pack/Rock2/Rock2_A.prefab", typeof(GameObject));
        GameObject buildingPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/RPG Character Animation Pack/Pro_Western_Starter_Pack/Prefabs/CityBuilding1.prefab", typeof(GameObject));

        float rockColliderX = rockPrefab.gameObject.GetComponent<Renderer>().bounds.size.x;
        float rockColliderY = rockPrefab.gameObject.GetComponent<Renderer>().bounds.size.y;
        float rockColliderZ = rockPrefab.gameObject.GetComponent<Renderer>().bounds.size.z;
        float ROCK_COLLIDER_SIZE = rockColliderX > rockColliderZ ? rockColliderX : rockColliderZ;

        float cactusColliderX = cactusPrefab.gameObject.GetComponent<Renderer>().bounds.size.x;
        float cactusColliderY = cactusPrefab.gameObject.GetComponent<Renderer>().bounds.size.y;
        float cactusColliderZ = cactusPrefab.gameObject.GetComponent<Renderer>().bounds.size.z;
        float CACTUS_COLLIDER_SIZE = cactusColliderX > cactusColliderZ ? cactusColliderX : cactusColliderZ;

        float buildingColliderX = buildingPrefab.gameObject.GetComponent<Renderer>().bounds.size.x;
        float buildingColliderY = buildingPrefab.gameObject.GetComponent<Renderer>().bounds.size.y;
        float buildingColliderZ = buildingPrefab.gameObject.GetComponent<Renderer>().bounds.size.z;
        float BUILDING_COLLIDER_SIZE = buildingColliderX > buildingColliderZ ? buildingColliderX : buildingColliderZ;

        // Roger
        for (int i = 0; i < Data.tiles.GetLength(0); i++)
        {
            for (int j = 0; j < Data.tiles.GetLength(1); j++)
            {
                if (Data.tiles[i, j] == (byte)TileTypes.BUSH)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(new Vector3(i, 0, j), ROCK_COLLIDER_SIZE);
                    if (hitColliders.Length <= 0)
                    {
                        if ((i + rockColliderX / 2) < Width && (j + rockColliderZ / 2) < Length && (i - rockColliderX / 2) > 0 && (j - rockColliderZ / 2) > 0)
                        {
                            GameObject newObject = (GameObject)Object.Instantiate(rockPrefab, new Vector3(i, 0, j), Quaternion.identity);
                            for (long ii = i - (long)rockColliderX / 2 - 1; ii <= i + (long)rockColliderX / 2 + 1; ii++)
                            {
                                for (long jj = j - (long)rockColliderZ / 2 - 1; jj <= j + (long)rockColliderZ / 2 + 1; jj++)
                                {
                                    this.occupiedPositions.Add(new Vector2(ii, jj));
                                }
                            }
                        }
                    }

                }

                if (Data.tiles[i, j] == (byte)TileTypes.CACTUS)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(new Vector3(i, 0, j), CACTUS_COLLIDER_SIZE);
                    if (hitColliders.Length <= 0)
                    {
                        if ((i + cactusColliderX / 2) < Width && (j + cactusColliderZ / 2) < Length && (i - cactusColliderX / 2) > 0 && (j - cactusColliderZ / 2) > 0)
                        {
                            GameObject newObject = (GameObject)Object.Instantiate(cactusPrefab, new Vector3(i, 0, j), Quaternion.identity);
                            for (long ii = i - (long)cactusColliderX / 2 - 1; ii <= i + (long)cactusColliderX / 2 + 1; ii++)
                            {
                                for (long jj = j - (long)cactusColliderZ / 2 - 1; jj <= j + (long)cactusColliderZ / 2 + 1; jj++)
                                {
                                    this.occupiedPositions.Add(new Vector2(ii, jj));
                                }
                            }
                        }
                    }
                }

                if (Data.tiles[i, j] == (byte)TileTypes.BUILDINGS)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(new Vector3(i, 0, j), BUILDING_COLLIDER_SIZE);
                    if (hitColliders.Length <= 0)
                    {
                        // Checks for border
                        if ((i + buildingColliderX / 2) < Width && (j + buildingColliderZ / 2) < Length && (i - buildingColliderX / 2) > 0 && (j - buildingColliderZ / 2) > 0)
                        {
                            GameObject newObject = (GameObject)Object.Instantiate(buildingPrefab, new Vector3(i, 0, j), Quaternion.identity);
                            for (long ii = i - (long)buildingColliderX / 2 - 1; ii <= i + (long)buildingColliderX / 2 + 1; ii++)
                            {
                                for (long jj = j - (long)buildingColliderZ / 2 - 1; jj <= j + (long)buildingColliderZ / 2 + 1; jj++)
                                {
                                    this.occupiedPositions.Add(new Vector2(ii, jj));
                                }
                            }
                        }
                    }
                }
            }
        }

        ///////////////////////////////////////////////////

        // Create a new splat prototype array
        SplatPrototype[] newSplatPrototypes = new SplatPrototype[1];

        //Create a new splat prototype object
        newSplatPrototypes[0] = new SplatPrototype();

        // Grab the texture and set it
        newSplatPrototypes[0].texture = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Textures/GrassyRocks.jpg", typeof(Texture2D));

        // Assign the new splat prototype array to tData
        tData.splatPrototypes = newSplatPrototypes;

        ///////////////////////////////////////////////////

        // Spawn the terrain
        GameObject terrain = (GameObject)Terrain.CreateTerrainGameObject(tData);
        terrain.name = DEFAULT_NAME;
        //Debug.Log("Occupied position: " + occupiedPositions.Count);

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
