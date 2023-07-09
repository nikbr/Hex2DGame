using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
        if(tile==Mountain) return false;
        return true;
    }
    public static bool IsRiverTraversible(HexModel hex, HashSet<int> visitedWaters){
        if(hex.terrainTile==Mountain) return false;
        if(hex.terrainTile==Water&&visitedWaters.Contains((int)hex.terrainChunk))return false;
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
    public static int IntToDirection(int num){
        if(num<0){
            while(num<0)num+=6;
            return num;
        }else{
            return num%6;
        }
    }
}

public static class MinimapSettings{
    public const float widthRatio = 0.01892f;
    public const float heightRatio = 0.01f;

    public const float fovRatio = 0.51f;
}

public class GameActivity : MonoBehaviour
{

    //public UnityEditor.Tilemaps.PrefabBrush[] prefabBrushes;
    public GameModel gameModel;
    public Tilemap terrain;
    public Tilemap resources;
    public Tilemap improvements;
    public Tilemap rivers;

    public Tilemap terrainMinimap;
    public Tilemap resourcesMinimap;
    public Tilemap improvementsMinimap;
    public Tilemap riversMinimap;
    public Tilemap cameraEdgeMinimap;

    public Tile[] terrainTile;
    public Tile[] resourceTile;
    public Tile[] improvementTile;
    public Tile[] riverTile;

    public Tile cameraEdgeTile;

    public TerrainMap terrainMap;
    public MiniMap miniMap;

    public GameObject minimapObject;

    private Vector3 oldCameraPostion;

    private List<Observer> observers = new List<Observer>();

    public Camera minimapCamera;

    private WorldCamera worldCamera;

    public bool cameraInMiddle = true;

    public bool down = false;
    public bool up = false;
    public bool left = false;
    public bool right = false;

    private readonly float MAP_SCROLL_CONSTANT = 10;


    //Turn manager stuff
    public int currentTurn = 1;
    public int objectsThatFinishedTurn=0;

    void Start()
    {
        gameModel = new GameModel(this, 100, 50, 12);

        SetupMinimapCamera();

        

        terrainMap = new TerrainMap(this);
        miniMap = new MiniMap(this);
        worldCamera = new WorldCamera(this);
        AddObserver(terrainMap);
        
        AddObserver(worldCamera);
        AddObserver(miniMap);
        oldCameraPostion = Camera.main.transform.position;
        gameModel.GameCameraPos =  Camera.main.transform.position;
        RunTests();
    }

    void SetupMinimapCamera(){
        minimapCamera.aspect = (gameModel.COLS*MinimapSettings.widthRatio)/(gameModel.ROWS*MinimapSettings.heightRatio);
        GridLayout gridLayout = cameraEdgeMinimap.GetComponentInParent<GridLayout>();
        Vector3 worldPos = gridLayout.CellToWorld(new Vector3Int(gameModel.COLS/2-1, gameModel.ROWS/2+1, 0));

        float cellHeight = gridLayout.CellToWorld(new Vector3Int(0, 1, 0)).y-gridLayout.CellToWorld(new Vector3Int(0, 0, 0)).y;

         

        minimapCamera.transform.position = new Vector3(worldPos.x+cellHeight, worldPos.y-cellHeight, -59);
        minimapCamera.fieldOfView = gameModel.ROWS*MinimapSettings.fovRatio;

        //minimapObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, gameModel.ROWS*(float)2.67);
        minimapObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, minimapCamera.aspect *  gameModel.ROWS*(float)2.67);
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfCameraInMiddle();
        CheckIfCameraMoved();
        MoveCamera();
        if(Input.GetMouseButtonDown(0)){
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            try{
            
                Vector3Int gridPosition = terrain.WorldToCell(mousePosition);

                HexModel hex = gameModel.GetHexModel(gridPosition.x, gridPosition.y);
                Debug.Log("Grid position: "+ gridPosition.x + " " + gridPosition.y + " " + gridPosition.z + "\n"+
                "Hex info: " + hex.COL + " " + hex.ROW + " Terrain Body Type: " + hex.terrainBodyType + " Coastal: " + hex.coastal+ "\n"+
                "Hex chunk: " + hex.terrainChunk+ "\n"+
                "Chunk info: "+ "Terrain: " + gameModel.terrainChunks[(int)hex.terrainChunk].TERRAIN_TYPE + " ID: " + gameModel.terrainChunks[(int)hex.terrainChunk].CHUNK_ID + 
                " Size: " + gameModel.terrainChunks[(int)hex.terrainChunk].Size() +" Previous direction: " + hex.previousRiverDirection + "\n" + 
                " RiverNeighbors: " + hex.RiverTileIndex())  ;
            
            }catch(Exception ex){
                Debug.Log("Outside of grid.");
            }
        }
    }

    public void UpdateCameraCornerPositions(){
        Vector3 TopLeftScreen = new Vector3(0, Screen.height );
        Vector3 TopRightScreen = new Vector3(Screen.width, Screen.height);
        Vector3 BottomLeftScreen = new Vector3(0, 0);
        Vector3 BottomRightScreen = new Vector3(Screen.width, 0);
        Vector2 TopLeftWorld = Camera.main.ScreenToWorldPoint(TopLeftScreen);
        Vector2 TopRightWorld = Camera.main.ScreenToWorldPoint(TopRightScreen);
        Vector2 BottomLeftWorld = Camera.main.ScreenToWorldPoint(BottomLeftScreen);
        Vector2 BottomRightWorld = Camera.main.ScreenToWorldPoint(BottomRightScreen);
        
        Vector3Int TopLeftGrid;
        Vector3Int TopRightGrid;
        Vector3Int BottomLeftGrid;
        Vector3Int BottomRightGrid;

        try{
            TopLeftGrid = terrain.WorldToCell(TopLeftWorld);
        }catch(Exception ex){
            BottomLeftGrid = terrain.WorldToCell(BottomLeftWorld);
            TopLeftGrid = terrain.WorldToCell(new Vector3Int(BottomLeftGrid.x, gameModel.ROWS-1, 0));
        }

        try{
            TopRightGrid = terrain.WorldToCell(TopRightWorld);
        }catch(Exception ex){
            BottomRightGrid = terrain.WorldToCell(BottomRightWorld);
            TopRightGrid = terrain.WorldToCell(new Vector3Int(BottomRightGrid.x, gameModel.ROWS-1, 0));
        }

        try{
            BottomLeftGrid = terrain.WorldToCell(BottomLeftWorld);
        }catch(Exception ex){
            BottomLeftGrid = terrain.WorldToCell(new Vector3Int(TopLeftGrid.x,0,0));
        }

        try{
            BottomRightGrid = terrain.WorldToCell(BottomRightWorld);
        }catch(Exception ex){
            BottomRightGrid = terrain.WorldToCell(new Vector3Int(TopRightGrid.x,0,0));
        }
        
        gameModel.SetCornerPosition(GameModel.TOP_LEFT, TopLeftGrid);
        gameModel.SetCornerPosition(GameModel.TOP_RIGHT, TopRightGrid);
        gameModel.SetCornerPosition(GameModel.BOTTOM_LEFT, BottomLeftGrid);
        gameModel.SetCornerPosition(GameModel.BOTTOM_RIGHT, BottomRightGrid);

    }

    void MoveCamera(){
        float currentX = gameModel.GameCameraPos.x;
        float currentY = gameModel.GameCameraPos.y;
        float currentZ = gameModel.GameCameraPos.z;
        

        if(Input.GetKeyDown(KeyCode.W)){
            up = true;
        }else if(Input.GetKeyDown(KeyCode.S)){
            down = true;
        }
        if(Input.GetKeyDown(KeyCode.A)){
            left = true;
        }else if(Input.GetKeyDown(KeyCode.D)){
            right = true;
        }

        if(up){
            currentY+=Time.deltaTime*MAP_SCROLL_CONSTANT;
        }else if(down){
            currentY-=Time.deltaTime*MAP_SCROLL_CONSTANT;
        }

        if(left){
            currentX-=Time.deltaTime*MAP_SCROLL_CONSTANT;
        }else if(right){
            currentX+=Time.deltaTime*MAP_SCROLL_CONSTANT;
        }

        if(Input.GetKeyUp(KeyCode.W)){
            up = false;
        }else if(Input.GetKeyUp(KeyCode.S)){
            down = false;
        }
        if(Input.GetKeyUp(KeyCode.A)){
            left = false;
        }else if(Input.GetKeyUp(KeyCode.D)){
            right = false;
        }
        gameModel.GameCameraPos= new Vector3(currentX, currentY, currentZ);
    }

    void CheckIfCameraMoved(){
      //  Debug.Log(oldCameraPostion + "         " + Camera.main.transform.position);
        if (oldCameraPostion.x !=  gameModel.GameCameraPos.x || oldCameraPostion.y !=  gameModel.GameCameraPos.y ){
            oldCameraPostion =  gameModel.GameCameraPos;
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
