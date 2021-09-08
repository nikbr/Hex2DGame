using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TerrainBodyType;
using static TerrainTile;

public class HexModel
{
    public readonly int COL;
    public readonly int ROW;
#nullable enable
    public int? terrainTile = null;
    public int? resourceTile = null;
    public int? improvementTile = null;
    public int? riverTile = null;
    public bool[]? riverNeighbors = null; 
    public int? previousLeftRiverDirection = null;
    public int? nextLeftRiverDirection = null;

    public int? terrainChunk = null;
#nullable disable
    public int cityScore = 0;
    public int movementPoints = 0;

    public bool coastal = false;
    public int terrainBodyType = Ocean;

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
    public Tile river(GameActivity context){
        if(riverNeighbors==null) return null;
        this.riverTile = RiverTile.RiverTileIndex[(int)RiverTileIndex()];
        return (this.riverTile != null) ? context.riverTile[(int) riverTile] : null;
    } 
    public bool IsWater(){
        return terrainTile==Water;
    }

    public int? RiverTileIndex(){
        if(riverNeighbors==null) return null;
        int index = 0;
        for(int i = 0;i<riverNeighbors.Length;i++){
            if(riverNeighbors[i])index+=(int)Math.Pow(10, i);
        }
        return index;
    }
    public int GetNumberOfRiverNeighbors(){
        if(riverNeighbors==null) return 0;
        int total = 0;
        foreach(bool n in riverNeighbors){
            if(n) total++;
        }
        return total;
    }

    //array of length 6
    public void AddRiverNeighbors(bool[] newNeighbors){
        if(riverNeighbors==null) riverNeighbors=newNeighbors;
        for(int i = 0;i<riverNeighbors.Length;i++){
            riverNeighbors[i]=riverNeighbors[i]||newNeighbors[i];
        }
    }

    public void RemoveRiverNeighbors(bool[] neighors){
        if(riverNeighbors==null) return;
        bool notSetToNull = false;
        for(int i = 0;i<riverNeighbors.Length;i++){
            if(neighors[i])riverNeighbors[i]=false;
            notSetToNull = riverNeighbors[i]||notSetToNull;
        }
        if(!notSetToNull) riverNeighbors=null;
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
