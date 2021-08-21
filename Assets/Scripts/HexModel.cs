using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class HexModel
{
    public readonly int COL;
    public readonly int ROW;

    public int? terrainTile = null;
    public int? resourceTile = null;
    public int? improvementTile = null;

    public int? terrainChunk = null;

    public int cityScore = 0;
    public int movementPoints = 0;

    public int actualCOL;


    public HexModel(int col, int row){
        COL = col;
        ROW = row;
        actualCOL = col;
    }

    public HexModel(int col, int row, int tile){
        terrainTile = tile;
        resourceTile = null;
        COL = col;
        ROW = row;
        actualCOL = col;
    }

    public Tile terrain(GameActivity context) {
        return (this.terrainTile != null) ? context.terrainTile[(int)terrainTile] : null;
    }

    public Tile resource(GameActivity context) {
        return (this.resourceTile != null) ? context.resourceTile[(int)resourceTile] : null;
    }

    public Tile improvement(GameActivity context){
        return (this.improvementTile != null) ? context.improvementTile[(int) improvementTile] : null;
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
