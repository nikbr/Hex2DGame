using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TerrainBodyType;
using System;

public class GameModel
{
    public static readonly int  TOP_LEFT = 0;
    public static readonly int  TOP_RIGHT = 1;
    public static readonly int  BOTTOM_LEFT = 2;
    public static readonly int  BOTTOM_RIGHT = 3;
   
    //public List<HexModel> terrainModel = new List<HexModel>();
    public HexModel[,] terrainModel; 

    public readonly int COLS;
    public readonly int ROWS;

    
    public Vector3 GameCameraPos = new Vector3(0,0,-59);

    public Dictionary<int, TerrainChunk> terrainChunks;

    private Vector3Int[] cameraCorners = new Vector3Int[4];

    public GameModel(GameActivity game, int cols, int rows){
        COLS = cols;
        ROWS = rows;
        terrainModel= new HexModel[rows, cols];
        terrainChunks = new Dictionary<int, TerrainChunk>();

    }
    public int TotalHexes(){
        return COLS*ROWS;
    }

    public Vector3Int GetCornerPosition(int p){
        return cameraCorners[p];
    }

    public void SetCornerPosition(int p, Vector3Int pos){
        cameraCorners[p] = pos;
    }

    public int GetMinCOL(){
        int min = Int32.MaxValue;
        int minCol = 0;
        for (int col =0;col<COLS;col++){
            if(terrainModel[0,col].actualCOL<min){
                min = terrainModel[0,col].actualCOL;
                minCol = col;
            }
        }

        return terrainModel[0,minCol].COL;
    }

    public int GetMaxCOL(){
        int max = Int32.MinValue;
        int maxCol = 0;
        for (int col =0;col<COLS;col++){
            if(terrainModel[0,col].actualCOL>max){
                max = terrainModel[0,col].actualCOL;
                maxCol = col;
            }
        }
        return terrainModel[0,maxCol].COL;
    }

    public Vector3Int OffsetCoord(Vector3Int cube){
        int col = cube.x + (cube.y - (cube.y&1))/2;
        return new Vector3Int(col,cube.y,cube.z);
    }

    public Vector3Int CubeCoord(Vector3Int offset){
        int col = offset.x-(offset.y-(offset.y&1))/2;
        return new Vector3Int(col, offset.y, offset.z);
    }

    public HexModel GetHexModel(int col, int row){
        if(row >=ROWS||row<0) return null;
        if(col>=COLS) col = col%COLS;
        else if(col<0){
            while(col<0) col+=COLS;
        }
        return terrainModel[row, col];
    }

    public void SetHexModelTile(int col, int row, int tile){
        if(row >=ROWS||row<0) return;
        if(col>=COLS) col = col%COLS;
        else if(col<0){
            while(col<0) col+=COLS;
        }
        terrainModel[row, col].terrainTile = tile;
    }

    public void SetHexModelTerrainChunk(int col, int row, int chunk_id){
        if(row >=ROWS||row<0) return;
        if(col>=COLS) col = col%COLS;
        else if(col<0){
            while(col<0) col+=COLS;
        }
        terrainModel[row, col].terrainChunk = chunk_id;
    }

    public List<Vector3Int> GetLocationsWithinRangeOf(Vector3Int center, int radius){
        List<Vector3Int> locs = new List<Vector3Int>();
        Vector3Int cubeloc = CubeCoord(center);
        for (int i = -radius; i<radius-1;i++){
            for (int j = Mathf.Max(-radius+1, -i-radius); j<Mathf.Min(radius, -i+radius-1); j++){
                Vector3Int offsetloc = OffsetCoord(new Vector3Int(cubeloc.x+i,cubeloc.y+j,0));
                if(offsetloc.x<0)offsetloc.x = COLS+offsetloc.x;
                else if(offsetloc.x>=COLS)offsetloc.x = offsetloc.x-COLS;
                offsetloc.x++; // Hope this fix works
                if(offsetloc.y<ROWS&&offsetloc.y>=0)locs.Add(offsetloc);
            }
        }
        return locs;
    }
    public bool[] GetNeighborsWithRiverTileType(int x, int y, int tileType){
        bool[] neighborsWithRiverTileType = new bool[Direction.LENGTH];
        Vector3Int[] neighborLocs =  GetNeighbors(new Vector3Int(x,y,0));
        for(int i = 0;i<neighborLocs.Length;i++){
            if(GetHexModel(neighborLocs[i].x, neighborLocs[i].y)!=null){
                neighborsWithRiverTileType[i]=GetHexModel(neighborLocs[i].x, neighborLocs[i].y).riverTile==tileType;
            }else{
                neighborsWithRiverTileType[i]=false;
            }
        }
        return neighborsWithRiverTileType; 
    }

    public int GetIfHasNeighborWithTerrainBodyType(int x, int y, int terrainBodyType){
        bool[] neighborsWithTerrainBodyType = GetNeighborsWithTerrainBodyType(x, y, terrainBodyType);
        for(int i = 0;i<Direction.LENGTH;i++){
            if(neighborsWithTerrainBodyType[i]){
                return i;
            }
        }
        return -1;
    }

    public bool[] GetNeighborsWithTerrainBodyType(int x, int y, int terrainBodyType){
        bool[] neighborsWithTerrainBodyType = new bool[Direction.LENGTH];
        Vector3Int[] neighborLocs =  GetNeighbors(new Vector3Int(x,y,0));
        for(int i = 0;i<neighborLocs.Length;i++){
            if(GetHexModel(neighborLocs[i].x, neighborLocs[i].y)!=null){
                neighborsWithTerrainBodyType[i]=GetHexModel(neighborLocs[i].x, neighborLocs[i].y).terrainBodyType==terrainBodyType;
            }else{
                neighborsWithTerrainBodyType[i]=false;
            }
        }
        return neighborsWithTerrainBodyType; 
    }
    public Vector3Int[] GetNeighbors(Vector3Int center){
        return GetLocationsOnRing(center, 2).ToArray();
    }
    public Vector3Int GetNeighbor(Vector3Int center, int direction){
        return GetNeighbors(center)[direction];
    }
    public Vector3Int GetBottomLeftNeighbor(Vector3Int center){
        return GetNeighbors(center)[Direction.BottomLeft];
    }
    public Vector3Int GetBottomRightNeighbor(Vector3Int center){
        return GetNeighbors(center)[Direction.BottomRight];
    }
    public Vector3Int GetRightNeighbor(Vector3Int center){
        return GetNeighbors(center)[Direction.Right];
    }
    public Vector3Int GetTopRightNeighbor(Vector3Int center){
        return GetNeighbors(center)[Direction.TopRight];
    }
    public Vector3Int GetTopLeftNeighbor(Vector3Int center){
        return GetNeighbors(center)[Direction.TopLeft];
    }
    public Vector3Int GetLeftNeighbor(Vector3Int center){
        return GetNeighbors(center)[Direction.Left];
    }
    public List<Vector3Int> GetLocationsOnRing(Vector3Int center, int radius){
        List<Vector3Int> locs = new List<Vector3Int>();
        Vector3Int cubeloc = CubeCoord(center);
        Vector3Int ringloc = cubeloc;
        ringloc.y-=(radius-1);
        locs.Add(OffsetCoord(ringloc));
        // Go right
        for(int j=0;j<radius-1;j++){
            ringloc.x++;
            locs.Add(OffsetCoord(ringloc));
        }
        // Go up right
        for(int j=0;j<radius-1;j++){
            ringloc.y++;
            locs.Add(OffsetCoord(ringloc));
        }
        //Go up left
        for(int j=0;j<radius-1;j++){
            ringloc.y++;
            ringloc.x--;
            locs.Add(OffsetCoord(ringloc));
        }
        // Go left
        for(int j=0;j<radius-1;j++){
            ringloc.x--;
            locs.Add(OffsetCoord(ringloc));
        }
        // Go down left
        for(int j=0;j<radius-1;j++){
            ringloc.y--;
            locs.Add(OffsetCoord(ringloc));
        }
        // Go down right
        for(int j=0;j<radius-2;j++){ //-2 here so that the first one doesnt get looked at twice
            ringloc.y--;
            ringloc.x++;
            locs.Add(OffsetCoord(ringloc));
        }
        return locs;
    }

    
}
