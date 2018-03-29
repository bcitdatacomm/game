using UnityEngine;
using System;
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
        BUILDINGS,
        HOTSPOTS
    };

    /*
     * Encoding structure
     * tiles - 2D array
     * buildings - 1D building array
     */
    public struct Encoding
    {
        public byte[,] tiles;
    };

    // The map encoding
    public Encoding Data { get; set; }
    // The compressed version of the map encoding
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

    // Cactus gameobject prefab
    public GameObject CactusPrefab { get; set; }
    // Bush gameobject prefab
    public GameObject BushPrefab { get; set; }

    // Occupied positions on the map
    public List<Vector2> occupiedPositions;

    // The list of guns
    List<WeaponSpell> gunsList = new List<WeaponSpell>();

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
        this.Width = R.Game.Terrain.DEFAULT_WIDTH;
        this.Length = R.Game.Terrain.DEFAULT_LENGTH;
        this.TileSize = R.Game.Terrain.DEFAULT_TILE_SIZE;
        this.CactusPerc = R.Game.Terrain.DEFAULT_CACTUS_PERC;
        this.BushPerc = R.Game.Terrain.DEFAULT_BUSH_PERC;
        this.occupiedPositions = new List<Vector2>();
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
                    float randomValue = UnityEngine.Random.value;

                    // Changed the comparison signs around
                    if (randomValue > R.Game.Terrain.DEFAULT_BUILDING_PERC)
                    {
                        map[i, j] = (byte)TileTypes.BUILDINGS;
                    }
                    else if (randomValue > this.CactusPerc)
                    {
                        map[i, j] = (byte)TileTypes.CACTUS;
                    }
                    else if (randomValue > this.BushPerc)
                    {
                        map[i, j] = (byte)TileTypes.BUSH;
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


    public static byte[] compressByteArray(byte[] data)
    {
        using (var compressedStream = new MemoryStream())
        using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
        {
            zipStream.Write(data, 0, data.Length);
            zipStream.Close();
            return compressedStream.ToArray();
        }
    }

    /*-------------------------------------------------------------------------------------------------
    -- FUNCTION: decompressByteArray()
    --
    -- DATE: Feb 28, 2018
    --
    -- REVISIONS: N/A
    --
    -- DESIGNER: Roger Zhang
    --
    -- PROGRAMMER: Roger Zhang
    --
    -- INTERFACE: decompressByteArray(byte[] output)
    --              byte[] output: They byte array to decompress.
    --
    -- RETURNS: byte array of decompressed data
    --
    -- NOTES:
    -- Compress the byteArrayData to a smaller size using system I/O.
    -------------------------------------------------------------------------------------------------*/
    public static byte[] decompressByteArray(byte[] data)
    {
        int size = data.Length;

        using (var compressedStream = new MemoryStream(data))
        using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
        using (var resultStream = new MemoryStream())
        {
            byte[] decompressedEncoding = new byte[size];
            int bytesRead;

            while ((bytesRead = zipStream.Read(decompressedEncoding, 0, decompressedEncoding.Length)) > 0)
            {
                resultStream.Write(decompressedEncoding, 0, bytesRead);
            }
            return resultStream.ToArray();
        }
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
        //TODO: LONG_OFFSET = 8
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

        this.Data = new Encoding() { tiles = map };
        Instantiate();
    }

    /*-------------------------------------------------------------------------------------------------
    -- FUNCTION: LoadGuns(byte[] guns)
    --
    -- DATE: March 21, 2018
    --
    -- REVISIONS: N/A
    --
    -- DESIGNER: Alfred Swinton & Roger Zhang
    --
    -- PROGRAMMER: Alfred Swinton & Roger Zhang
    --
    -- INTERFACE: LoadGuns(byte[] guns)
    --                  byte[] guns: A byte array contains the gun data.
    --
    -- RETURNS: void
    --
    -- NOTES:
    -- Takes in a byte array representation of the guns data that was send over by the server and
    -- loads it.
    -------------------------------------------------------------------------------------------------*/
    public void LoadGuns(byte[] compressedGuns)
    {
        // Decompress the guns bytearray
        byte[] guns = decompressByteArray(compressedGuns);

        float offsetX = this.Width / 2;
        float offsetZ = this.Length / 2;
        int size = guns.Length / R.Game.Terrain.GUN_OBJECT_SIZE;
        int count = 0;
        byte[] tempw = new byte[R.Game.Terrain.GUN_OBJECT_SIZE];

        // Clear out any old data
        gunsList.Clear();

        // Go through the decompressed gun packet, create WeaponSpell objects,
        // and the assign it too the gun list
        for (int i = 0; i < size; i++)
        {
            Buffer.BlockCopy(guns, count, tempw, 0, R.Game.Terrain.GUN_OBJECT_SIZE);
            WeaponSpell tempwpn = GetWeaponFromBytes(tempw);
            gunsList.Add(tempwpn);
            count += R.Game.Terrain.GUN_OBJECT_SIZE;
        }

        //Load all of the gun prefabs
        GameObject gun1 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Guns/Knife.prefab", typeof(GameObject));
        GameObject gun2 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Guns/Pistol.prefab", typeof(GameObject));
        GameObject gun3 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Guns/Rifle.prefab", typeof(GameObject));
        GameObject gun4 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Guns/Shotgun.prefab", typeof(GameObject));
        GameObject gun5 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Guns/Pistol.prefab", typeof(GameObject));
        GameObject gun6 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Guns/Knife.prefab", typeof(GameObject));
        GameObject gun7 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Guns/Pistol.prefab", typeof(GameObject));
        GameObject gun8 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Guns/Rifle.prefab", typeof(GameObject));
        GameObject gun9 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Guns/Shotgun.prefab", typeof(GameObject));
        GameObject gun10 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Guns/Shotgun.prefab", typeof(GameObject));
        GameObject gun11 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Guns/Knife.prefab", typeof(GameObject));
        GameObject gun12 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Guns/Pistol.prefab", typeof(GameObject));
        GameObject gun13 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Guns/Rifle.prefab", typeof(GameObject));

        //Go through every gun in the list and instantiate the gun based on type
        foreach (var w in gunsList)
        {
            switch (w.Type)
            {
                case 1:
                    UnityEngine.Object.Instantiate(gun1, new Vector3(w.X - offsetX, 0, w.Z - offsetZ), Quaternion.identity);
                    break;
                case 2:
                    UnityEngine.Object.Instantiate(gun2, new Vector3(w.X - offsetX, 0, w.Z - offsetZ), Quaternion.identity);
                    break;
                case 3:
                    UnityEngine.Object.Instantiate(gun3, new Vector3(w.X - offsetX, 0, w.Z - offsetZ), Quaternion.identity);
                    break;
                case 4:
                    UnityEngine.Object.Instantiate(gun4, new Vector3(w.X - offsetX, 0, w.Z - offsetZ), Quaternion.identity);
                    break;
                case 5:
                    UnityEngine.Object.Instantiate(gun5, new Vector3(w.X - offsetX, 0, w.Z - offsetZ), Quaternion.identity);
                    break;
                case 6:
                    UnityEngine.Object.Instantiate(gun6, new Vector3(w.X - offsetX, 0, w.Z - offsetZ), Quaternion.identity);
                    break;
                case 7:
                    UnityEngine.Object.Instantiate(gun7, new Vector3(w.X - offsetX, 0, w.Z - offsetZ), Quaternion.identity);
                    break;
                case 8:
                    UnityEngine.Object.Instantiate(gun8, new Vector3(w.X - offsetX, 0, w.Z - offsetZ), Quaternion.identity);
                    break;
                case 9:
                    UnityEngine.Object.Instantiate(gun9, new Vector3(w.X - offsetX, 0, w.Z - offsetZ), Quaternion.identity);
                    break;
                case 10:
                    UnityEngine.Object.Instantiate(gun10, new Vector3(w.X - offsetX, 0, w.Z - offsetZ), Quaternion.identity);
                    break;
                case 11:
                    UnityEngine.Object.Instantiate(gun11, new Vector3(w.X - offsetX, 0, w.Z - offsetZ), Quaternion.identity);
                    break;
                case 12:
                    UnityEngine.Object.Instantiate(gun12, new Vector3(w.X - offsetX, 0, w.Z - offsetZ), Quaternion.identity);
                    break;
                case 13:
                    UnityEngine.Object.Instantiate(gun13, new Vector3(w.X - offsetX, 0, w.Z - offsetZ), Quaternion.identity);
                    break;
                default:
                    break;
            }
        }
    }

    // This is a helper function that parses chunks of the decompressed
    // gun packet and returns the parsed packet data as a WeaponSpell object
    //
    // Alfred's code, modified by Roger
    private WeaponSpell GetWeaponFromBytes(byte[] weaponinbytes)
    {
        WeaponSpell Weapon = new WeaponSpell();

        //Grab the first byte
        Weapon.Type = weaponinbytes[0];

        byte[] ID = new byte[R.Game.Terrain.ID_BYTE_SIZE];
        Weapon.ID = BitConverter.ToInt32(weaponinbytes, R.Game.Terrain.ID_OFFSET);

        byte[] X = new byte[R.Game.Terrain.X_BYTE_SIZE];
        Weapon.X = BitConverter.ToInt32(weaponinbytes, R.Game.Terrain.X_OFFSET);

        byte[] Z = new byte[R.Game.Terrain.Z_BYTE_SIZE];
        Weapon.Z = BitConverter.ToInt32(weaponinbytes, R.Game.Terrain.Z_OFFSET);

        return Weapon;
    }

    // The client side version of the WeaponSpell class
    //
    // Alfred's code, modified by Roger
    public class WeaponSpell
    {
        public static int inc = 0;
        public int X { get; set; }
        public int Z { get; set; }
        public int ID { get; set; }
        public byte Type { get; set; }

        // Empty Constructor
        public WeaponSpell()
        {
        }
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
        float offsetX = this.Width / 2;
        float offsetZ = this.Length / 2;

        TerrainData tData = new TerrainData
        {
            size = new Vector3(Width, 0, Length),
            name = R.Game.Terrain.DEFAULT_NAME
        };

        // Grab the prefabs
        GameObject rockPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Scenery/Rocks Pack/Rock1/RockScaled.prefab", typeof(GameObject));
        GameObject cactusPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Environment/CactusScaled.prefab", typeof(GameObject));
        GameObject buildingPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Buildings/CityBuildingScaled.prefab", typeof(GameObject));
        GameObject townPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Scenery/Town/Town1.prefab", typeof(GameObject));

        //Set the collider boundaries of the game objects
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
        int RandomRotationPerc = 0;

        // Spawning the town at the center with collider 300X300
        GameObject TownObject = (GameObject)UnityEngine.Object.Instantiate(townPrefab, new Vector3(Width / 2 - offsetX, 0, Length / 2 - offsetZ), Quaternion.identity);

        //Spawn the game obstacles
        for (int i = 0; i < Data.tiles.GetLength(0); i++)
        {
            for (int j = 0; j < Data.tiles.GetLength(1); j++)
            {
                if (Data.tiles[i, j] == (byte)TileTypes.BUSH)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(new Vector3(i - offsetX, 0, j - offsetZ), ROCK_COLLIDER_SIZE);
                    if (hitColliders.Length <= 0)
                    {
                        if ((i - offsetX + rockColliderX / 2) < Width / 2 && (j - offsetZ + rockColliderZ / 2) < Length / 2 && (i - offsetX - rockColliderX / 2) > -Width / 2 && (j - offsetZ - rockColliderZ / 2) > -Length / 2)
                        {
                            GameObject newObject = (GameObject)UnityEngine.Object.Instantiate(rockPrefab, new Vector3(i - offsetX, 0, j - offsetZ), Quaternion.identity);
                            for (long ii = i - (long)offsetX - (long)rockColliderX / 2 - 1; ii <= i - (long)offsetX + (long)rockColliderX / 2 + 1; ii++)
                            {
                                for (long jj = j - (long)offsetZ - (long)rockColliderZ / 2 - 1; jj <= j - (long)offsetZ + (long)rockColliderZ / 2 + 1; jj++)
                                {
                                    // this list will return coordinates with 0,0 at center.
                                    this.occupiedPositions.Add(new Vector2(ii, jj));
                                }
                            }
                        }
                    }

                }

                if (Data.tiles[i, j] == (byte)TileTypes.CACTUS)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(new Vector3(i - offsetX, 0, j - offsetZ), CACTUS_COLLIDER_SIZE);
                    if (hitColliders.Length <= 0)
                    {
                        if ((i - offsetX + cactusColliderX / 2) < Width / 2 && (j - offsetZ + cactusColliderZ / 2) < Length / 2 && (i - offsetX - cactusColliderX / 2) > -Width / 2 && (j - offsetZ - cactusColliderZ / 2) > -Length / 2)
                        {
                            GameObject newObject = (GameObject)UnityEngine.Object.Instantiate(cactusPrefab, new Vector3(i - offsetX, 0, j - offsetZ), Quaternion.identity);
                            for (long ii = i - (long)offsetX - (long)cactusColliderX / 2 - 1; ii <= i - (long)offsetX + (long)cactusColliderX / 2 + 1; ii++)
                            {
                                for (long jj = j - (long)offsetZ - (long)cactusColliderZ / 2 - 1; jj <= j - offsetZ + (long)cactusColliderZ / 2 + 1; jj++)
                                {
                                    this.occupiedPositions.Add(new Vector2(ii, jj));
                                }
                            }
                        }
                    }
                }

                if (Data.tiles[i, j] == (byte)TileTypes.BUILDINGS)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(new Vector3(i - offsetX, 0, j - offsetZ), BUILDING_COLLIDER_SIZE);
                    if (hitColliders.Length <= 0)
                    {
                        // Checks for border
                        if ((i - offsetX + buildingColliderX / 2) < Width / 2 && (j - offsetZ + buildingColliderZ / 2) < Length / 2 && (i - offsetX - buildingColliderX / 2) > -Width / 2 && (j - offsetZ - buildingColliderZ / 2) > -Length / 2)
                        {
                            RandomRotationPerc = i % 4 * 90 + 90;
                            // Spawning buildings with a random rotation
                            // !!!!!!REMEMBER TO CHANGE THE PREFAB OBJECT YOU MUST CHANGE THE BOX COLLIDER TO BE THE SAME FOR X AND Z otherwise they might overlap after rotation
                            GameObject newObject = (GameObject)UnityEngine.Object.Instantiate(buildingPrefab, new Vector3(i - offsetX, 0, j - offsetZ), Quaternion.Euler(0, RandomRotationPerc, 0));
                            for (long ii = i - (long)offsetX - (long)buildingColliderX / 2 - 1; ii <= i - (long)offsetX + (long)buildingColliderX / 2 + 1; ii++)
                            {
                                for (long jj = j - (long)offsetZ - (long)buildingColliderZ / 2 - 1; jj <= j - (long)offsetZ + (long)buildingColliderZ / 2 + 1; jj++)
                                {
                                    this.occupiedPositions.Add(new Vector2(ii, jj));
                                }
                            }
                        }
                    }
                }
            }
        }

        // Create a new splat prototype array
        SplatPrototype[] newSplatPrototypes = new SplatPrototype[1];

        //Create a new splat prototype object
        newSplatPrototypes[0] = new SplatPrototype();

        // Grab the texture and set it
        newSplatPrototypes[0].texture = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Textures/Ground/Ground_Path.png", typeof(Texture2D));

        // Assign the new splat prototype array to tData
        tData.splatPrototypes = newSplatPrototypes;

        // Spawn the terrain
        GameObject terrain = (GameObject)Terrain.CreateTerrainGameObject(tData);
        terrain.name = R.Game.Terrain.DEFAULT_NAME;
        terrain.transform.Translate(-offsetX, 0, -offsetZ);

        return true;
    }
}
