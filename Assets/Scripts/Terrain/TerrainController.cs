using UnityEngine;
using System.Collections;

public class TerrainController{

    enum TileTypes {
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
    public long Height { get; set; }
    public long TileSize { get; set; }

    public double CactusCoeff { get; set; }
    public double BushCoeff { get; set; }

    public Building[] Buildings { get; set; }

    TerrainController()
    {
    }

    public bool GenerateEncoding()
    {
        int[,] map = new int[Width, Height];

        for (long i = 0; i < Width; i++)
        {
            for (long j = 0; j < Height; j++)
            {
                // Check for border
                if (i == 0 || i == Width || j == 0 || j == Height)
                {
                    map[i, j] = (int)TileTypes.GROUND;
                }
                else
                {
                    if (CactusCoeff < BushCoeff)
                    {
                        if (Random.value * 101 < CactusCoeff)
                        {
                            map[i, j] = (int)TileTypes.CACTUS;
                        }
                        else if (Random.value * 101 < BushCoeff)
                        {
                            map[i, j] = (int)TileTypes.BUSH;
                        }
                        else
                        {
                            map[i, j] = (int)TileTypes.GROUND;
                        }
                    } else
                    {
                        if (Random.value * 101 < BushCoeff)
                        {
                            map[i, j] = (int)TileTypes.BUSH;
                        }
                        else if (Random.value * 101 < CactusCoeff)
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

    public bool Instantiate()
    {
        return false;
    }
}


public class Building
{
    Building()
    {

    }
}
