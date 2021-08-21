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

public class GameActivity : MonoBehaviour
{

    //public UnityEditor.Tilemaps.PrefabBrush[] prefabBrushes;
    public GameModel gameModel;
    public Tilemap terrain;
    public Tilemap resources;
    public Tilemap improvements;
    public Tile[] terrainTile;
    public Tile[] resourceTile;
    public Tile[] improvementTile;

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
            "Hex info: " + hex.COL + " " + hex.ROW + " Terrain Tile: " + hex.terrainTile + " Resource Tile: " + hex.resourceTile+ "\n"+
            "Hex chunk: " + hex.terrainChunk+ "\n"+
            "Chunk info: "+ "Terrain: " + gameModel.terrainChunks[(int)hex.terrainChunk].TERRAIN_TYPE + " ID: " + gameModel.terrainChunks[(int)hex.terrainChunk].CHUNK_ID + " Size: " + gameModel.terrainChunks[(int)hex.terrainChunk].Size());
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
