using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainMap : Observer
{
    public TerrainMap(GameActivity context){
        CreateWater(context);
        CreateContinents (context);
    }

    private void CreateWater(GameActivity context){
        for (int column = 0; column<context.gameModel.COLS;column++){
            for(int row = 0; row < context.gameModel.ROWS; row++){
                HexModel h = new HexModel(column, row);
                h.tile = context.waterTile;
                context.gameModel.terrainModel[row,column] = h;
                context.terrain.SetTile(h.PositionFromCamera(Camera.main.transform.position, context.gameModel.COLS, context.gameModel.ROWS), h.tile );
            }
        }
    }

    private void CreateContinents(GameActivity context){
        LandHex(new Vector3Int(90,51,0), 4, context);
        RefreshMap(context);
    }

    private void LandHex(Vector3Int center, int radius, GameActivity context){
        List<Vector3Int> locs = context.gameModel.GetLocationsWithinRadiusOf(center, radius);
        foreach(Vector3Int loc in locs){
            context.gameModel.terrainModel[loc.y, loc.x].tile = context.grassTile;
        }
    }


    private void RefreshMap(GameActivity context){
        context.terrain.ClearAllTiles();
        for(int q = 0;q<context.gameModel.COLS;q++){
            for(int r = 0;r<context.gameModel.ROWS;r++){
                HexModel h = context.gameModel.terrainModel[r,q];
                context.terrain.SetTile(h.PositionFromCamera(Camera.main.transform.position, context.gameModel.COLS, context.gameModel.ROWS),h.tile );
            }
        }
    }

    private void UpdateMap(GameActivity context){
        int minCOL = context.gameModel.GetMinCOL();
        int maxCOL = context.gameModel.GetMaxCOL();
        for (int row = 0; row<context.gameModel.ROWS; row++){
            HexModel hLeft = context.gameModel.terrainModel[row,minCOL];
            HexModel hRight = context.gameModel.terrainModel[row,maxCOL];

            Vector3Int oldLeftPos = hLeft.Position();
            Vector3Int newLeftPos = hLeft.PositionFromCamera(Camera.main.transform.position, context.gameModel.COLS, context.gameModel.ROWS);
            Vector3Int oldRightPos = hRight.Position();
            Vector3Int newRightPos = hRight.PositionFromCamera(Camera.main.transform.position, context.gameModel.COLS, context.gameModel.ROWS);

            if(oldLeftPos.x!=newLeftPos.x||oldRightPos.x!=newRightPos.x) {
                context.cameraInMiddle=false;
            }else{
                context.cameraInMiddle=true;
            }

            context.terrain.SetTile(oldLeftPos, null);
            context.terrain.SetTile(newLeftPos, hLeft.tile);
            context.terrain.SetTile(oldRightPos, null);
            context.terrain.SetTile(newRightPos, hRight.tile);
        }

    }

    public void Update(GameActivity gameActivity){
        UpdateMap(gameActivity);
    }
}
