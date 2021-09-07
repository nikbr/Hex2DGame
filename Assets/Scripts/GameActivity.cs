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
       {10111,6},
       {101111,7},
       {111111,8},
       {11111,9},
       {1111,10},
       {111,11},
       {110011,12},
       {10011,13},
       {101011,14},
       {111011,15},
       {11011,16},
       {1011,17},
       {11,18},
       {100001,19},
       {100101,20},
       {110101,21},
       {10101,22},
       {101101,23},
       {111101,24},
       {11101,25},
       {1101,26},
       {101,27},
       {110001,28},
       {10001,29},
       {101001,30},
       {111001,31},
       {11001,32},
       {1001,33},
       {1,34},
       {100010,35},
       {100110,36},
       {110110,37},
       {10110,38},
       {101110,39},
       {111110,40},
       {11110,41},
       {1110,42},
       {110,43},
       {110010,44},
       {10010,45},
       {101010,46},
       {111010,47},
       {11010,48},
       {1010,49},
       {10,50},
       {100000,51},
       {100100,52},
       {110100,53},
       {10100,54},
       {101100,55},
       {111100,56},
       {11100,57},
       {1100,58},
       {100,59},
       {110000,60},
       {10000,61},
       {101000,62},
       {111000,63},
       {11000,64},
       {1000,65}
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
            " Size: " + gameModel.terrainChunks[(int)hex.terrainChunk].Size() +" Previous direction: " + hex.previousLeftRiverDirection + "\n" + 
            " RiverNeighbors: " + hex.RiverTileIndex())  ;
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
