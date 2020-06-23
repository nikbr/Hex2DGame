using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class HexModel
{
    public readonly int COL;
    public readonly int ROW;

    public Tile tile;
   // public int tile;

    public int actualCOL;


    public HexModel(int col, int row){
        COL = col;
        ROW = row;
        actualCOL = col;
    }

    public Vector3Int Position(){
        return new Vector3Int(actualCOL, ROW, 0);
    }

    public Vector3Int PositionFromCamera(Vector3 cameraPosition, int numCols, int numRows){
        double howManyWidthsFromCamera = (actualCOL - cameraPosition.x)/numCols;

        if(howManyWidthsFromCamera>0) howManyWidthsFromCamera+=0.5;
        else howManyWidthsFromCamera -= 0.5;

        int howManyWidthsToFix = (int) howManyWidthsFromCamera;
        actualCOL = actualCOL - (int)(howManyWidthsToFix*numCols);
        return new Vector3Int(actualCOL, ROW, 0);
    }


}
