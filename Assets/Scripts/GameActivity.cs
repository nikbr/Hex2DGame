using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameActivity : MonoBehaviour
{

    //public UnityEditor.Tilemaps.PrefabBrush[] prefabBrushes;
    public GameModel gameModel;
    public Tilemap terrain;
    public HexComponent[] hexComponents;
    public Tile waterTile;
    public Tile gravelTile;
    public Tile mountainTile;
    public Tile grassTile;


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
            Debug.Log("Gamer");
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
}
