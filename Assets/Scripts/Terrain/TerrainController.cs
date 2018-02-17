public class TerrainController {

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
        return false;
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
