using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameActivity : MonoBehaviour
{

    //public UnityEditor.Tilemaps.PrefabBrush[] prefabBrushes;
    public GameModel gameModel;
    public Tilemap terrain;

    public Tilemap resources;

    //terrain tiles
    public Tile waterTile;
    public Tile mountainTile;
    public Tile grassTile;
    public Tile desertTile;
    public Tile tundraTile;
    public Tile snowTile;
    public Tile grassHillTile;
    public Tile desertHillTile;
    public Tile tundraHillTile;
    public Tile snowHillTile;

    //resource tiles
    public Tile grassForestTile;
    public Tile tundraForestTile;
    public Tile desertJungleTile;
    public Tile snowForestTile;
    public Tile mountainDefaultTile;


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
