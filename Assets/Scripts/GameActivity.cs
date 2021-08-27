using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TerrainTile {
    public const int Water = 0;
    public const int Mountain = 1;
    public const int Grass = 2;
    public const int Desert = 3;
    public const int Tundra = 4;
    public const int Snow = 5;
    public const int GrassHill = 6;
    public const int DesertHill = 7;
    public const int TundraHill = 8;
    public const int SnowHill = 9;
    public const int LENGTH = 10;
    public static bool IsRiverTraversible(int tile){
        if(tile==Water||tile==Mountain) return false;
        return true;
    }
    public static bool IsDry(int? tile){
        if(tile==Desert||tile==Snow||tile==DesertHill||tile==SnowHill)return true;
        return false;
    }
}

public static class ResourceTile {
    public const int GrassForest = 0;
    public const int TundraForest = 1;
    public const int DesertJungle = 2;
    public const int SnowForest = 3;
    public const int MountainDefault = 4;
    public const int LENGTH = 5;
}

public static class ImprovementTile {
    public const int City = 0;
    public const int LENGTH = 1;
}

public static class TerrainBodyType { 
    public const int Ocean = 0;
    public const int Sea = 1;
    public const int Lake = 2;
    public const int Land = 3;
    public const int MountainRange = 4;
    public const int LENGTH = 5;
}

public static class RiverTile { 
    public const int Left = 0;
    public const int Right = 1;
    public const int Source = 2;
    public static Dictionary<int, int> RiverTileIndex = new Dictionary<int, int>
    {
       {100011, 3},
       {100111, 4},
       {110111, 5},
       {010111,6},
       {101111,7},
       {111111,8},
       {011111,9},
       {001111,10},
       {000111,11},
       {110011,12},
       {010011,13},
       {101011,14},
       {111011,15},
       {011011,16},
       {001011,17},
       {000011,18},
       {100001,19},
       {100101,20},
       {110101,21},
       {010101,22},
       {101101,23},
       {111101,24},
       {011101,25},
       {001101,26},
       {000101,27},
       {110001,28},
       {010001,29},
       {101001,30},
       {111001,31},
       {011001,32},
       {001001,33},
       {000001,34},
       {100010,35},
       {100110,36},
       {110110,37},
       {010110,38},
       {101110,39},
       {111110,40},
       {011110,41},
       {001110,42},
       {000110,43},
       {110010,44},
       {010010,45},
       {101010,46},
       {111010,47},
       {011010,48},
       {001010,49},
       {000010,50},
       {100000,51},
       {100100,52},
       {110100,53},
       {010100,54},
       {101100,55},
       {111100,56},
       {011100,57},
       {001100,58},
       {000100,59},
       {110000,60},
       {010000,61},
       {101000,62},
       {111000,63},
       {011000,64},
       {001000,65}
    };
}

public static class Direction { //depends on the order of directions looked at in GetLocationsOnRing
    // adding makes you go counterclockwise
    // subtracting makes you go clockwise
    public const int BottomLeft = 0; //bl
    public const int BottomRight = 1; //br
    public const int Right = 2; //r
    public const int TopRight = 3; //tr
    public const int TopLeft = 4; //tl
    public const int Left = 5; //l
    public const int LENGTH = 6;
    public static int OppositeDirection(int dir){
        switch(dir){
            case BottomLeft:
                return TopRight;
            case BottomRight:
                return TopLeft;
            case Right:
                return Left;
            case TopRight:
                return BottomLeft;
            case TopLeft:
                return BottomRight;
            case Left:
                return Right;
        }
        return -1;
    }
}

public class GameActivity : MonoBehaviour
{

    //public UnityEditor.Tilemaps.PrefabBrush[] prefabBrushes;
    public GameModel gameModel;
    public Tilemap terrain;
    public Tilemap resources;
    public Tilemap improvements;
    public Tilemap rivers;
    public Tile[] terrainTile;
    public Tile[] resourceTile;
    public Tile[] improvementTile;
    public Tile[] riverTile;
    public TerrainMap terrainMap;

    private Vector3 oldCameraPostion;

    private List<Observer> observers = new List<Observer>();

    public bool cameraInMiddle = true;

    void Start()
    {
        gameModel = new GameModel(this, 120, 100);
        terrainMap = new TerrainMap(this);
        AddObserver(terrainMap);
        oldCameraPostion = Camera.main.transform.position;
        RunTests();
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfCameraInMiddle();
        CheckIfCameraMoved();

        if(Input.GetMouseButtonDown(0)){
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = terrain.WorldToCell(mousePosition);

            HexModel hex = gameModel.GetHexModel(gridPosition.x, gridPosition.y);
            Debug.Log("Grid position: "+ gridPosition.x + " " + gridPosition.y + " " + gridPosition.z + "\n"+
            "Hex info: " + hex.COL + " " + hex.ROW + " Terrain Body Type: " + hex.terrainBodyType + " Coastal: " + hex.coastal+ "\n"+
            "Hex chunk: " + hex.terrainChunk+ "\n"+
            "Chunk info: "+ "Terrain: " + gameModel.terrainChunks[(int)hex.terrainChunk].TERRAIN_TYPE + " ID: " + gameModel.terrainChunks[(int)hex.terrainChunk].CHUNK_ID + 
            " Size: " + gameModel.terrainChunks[(int)hex.terrainChunk].Size() +" Previous direction: " + hex.previousLeftRiverDirection + "\n")  ;
        }
    }

    void CheckIfCameraMoved(){
       // Debug.Log(oldCameraPostion + "         " + Camera.main.transform.position);
        if (oldCameraPostion.x != Camera.main.transform.position.x){
            oldCameraPostion = Camera.main.transform.position;
            NotifyObservers();
        }
    } 
    void CheckIfCameraInMiddle(){
         if (!cameraInMiddle) NotifyObservers(); 
    }
    public void AddObserver(Observer o){
        observers.Add(o);
    }

    public void NotifyObservers(){
        foreach(Observer o in observers){
            o.Update(this);
        }
    }

    // Tests
    private void RunTests(){
        CoordConversionTest();

    }
    private void CoordConversionTest(){
        Vector3Int initOffset = new Vector3Int(10,11,0);
        Vector3Int cube = gameModel.CubeCoord(initOffset);
        Vector3Int resultOffset = gameModel.OffsetCoord(cube);
        Debug.Assert(initOffset.x==resultOffset.x && initOffset.y==resultOffset.y, "CoordConversion failed;");
    }
}
