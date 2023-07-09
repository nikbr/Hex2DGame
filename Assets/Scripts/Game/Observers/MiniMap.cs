using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : Observer
{
    public MiniMap(GameActivity context){
        RefreshMiniMap(context);
    }

    public void RefreshMiniMap(GameActivity context){
        for(int q = 0;q<context.gameModel.COLS;q++){
            for(int r = 0;r<context.gameModel.ROWS;r++){
                HexModel h = context.gameModel.terrainModel[r,q];
                context.terrainMinimap.SetTile(h.Coordinates(), h.terrain(context));
                context.resourcesMinimap.SetTile(h.Coordinates(), h.resource(context));
                context.riversMinimap.SetTile(h.Coordinates(), h.river(context));
            }
        }
    }

    private void DrawCameraEdges(GameActivity context){

        Vector3Int topLeftHex = context.gameModel.GetCornerPosition(GameModel.TOP_LEFT);
        Vector3Int topRightHex = context.gameModel.GetCornerPosition(GameModel.TOP_RIGHT);
        Vector3Int bottomLeftHex = context.gameModel.GetCornerPosition(GameModel.BOTTOM_LEFT);
        Vector3Int bottomRightHex = context.gameModel.GetCornerPosition(GameModel.BOTTOM_RIGHT);
        context.cameraEdgeMinimap.ClearAllTiles();
        DrawCameraEdgeHorizontal(context, topLeftHex, topRightHex);
        DrawCameraEdgeHorizontal(context, bottomLeftHex, bottomRightHex);
        DrawCameraEdgeVertical(context, bottomLeftHex,topLeftHex);
        DrawCameraEdgeVertical(context, bottomRightHex,topRightHex);

    }

    private void DrawCameraEdgeHorizontal(GameActivity context, Vector3Int left, Vector3Int right){
        int y = left.y;
        if(y<0){
            y=0;
        } else if(y>context.gameModel.ROWS-1){
            y = context.gameModel.ROWS-1;
        }
        HexModel hModelLeft = context.gameModel.GetHexModel(left.x, y);
        HexModel hModelRight = context.gameModel.GetHexModel(right.x, y);
        int rightCol = hModelRight.COL;
        if(rightCol < hModelLeft.COL) rightCol+=context.gameModel.COLS;

        for(int i = hModelLeft.COL; i<=rightCol; i++){
            int posX = i%context.gameModel.COLS;
            context.cameraEdgeMinimap.SetTile(new Vector3Int(posX, hModelLeft.ROW, 0), context.cameraEdgeTile);
        }
        
    }

    private void DrawCameraEdgeVertical(GameActivity context, Vector3Int bottom, Vector3Int top){

        int bottomY = bottom.y;
        if(bottomY<0){
            bottomY=0;
        }

        int topY = top.y;
        if(topY>context.gameModel.ROWS-1){
            topY = context.gameModel.ROWS-1;
        }

        HexModel hModelBottom = context.gameModel.GetHexModel(bottom.x, bottomY);
        HexModel hModelTop = context.gameModel.GetHexModel(top.x, topY);
        for(int i = hModelBottom.ROW; i<=hModelTop.ROW; i++){
            context.cameraEdgeMinimap.SetTile(new Vector3Int(hModelBottom.COL, i , 0), context.cameraEdgeTile);
        }
    }

    public void Update(GameActivity gameActivity){
        DrawCameraEdges(gameActivity);
    }
}
