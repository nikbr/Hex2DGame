using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModel
{
   
    //public List<HexModel> terrainModel = new List<HexModel>();
    public HexModel[,] terrainModel; 

    public readonly int COLS;
    public readonly int ROWS;

    public GameModel(GameActivity game, int cols, int rows){
        COLS = cols;
        ROWS = rows;
        terrainModel= new HexModel[rows, cols];
    }
    public int TotalHexes(){
        return COLS*ROWS;
    }

    public int GetMinCOL(){
        int min = COLS;
        int minCol = COLS;
        for (int col =0;col<COLS;col++){
            if(terrainModel[0,col].actualCOL<min){
                min = terrainModel[0,col].actualCOL;
                minCol = col;
            }
        }
        return terrainModel[0,minCol].COL;
    }

    public int GetMaxCOL(){
        int max = -COLS;
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

    public List<Vector3Int> GetLocationsWithinRadiusOf(Vector3Int center, int radius){
        List<Vector3Int> locs = new List<Vector3Int>();
        Vector3Int cubeloc = CubeCoord(new Vector3Int(center.x, center.y, 0));
        for (int i = -radius; i<radius;i++){
            for (int j = -radius; j<radius; j++){
            Vector3Int offsetloc = OffsetCoord(new Vector3Int(cubeloc.x+i,cubeloc.y+j,0));
            locs.Add(offsetloc);
            }
        }
        return locs;
    }

    
}
