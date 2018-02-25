using UnityEngine;
using System.Collections;

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
        return true;
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
