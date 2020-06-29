using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainMap : Observer
{
    public TerrainMap(GameActivity context){
        CreateWater(context);
        CreateContinents (context);
        context.terrain.SetTile(new Vector3Int(50,50,0), context.gravelTile);
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

        //Test
        LandMass(new Vector3Int(50, 50, 0), 1, context,0);//
       // LandMass(new Vector3Int(70,50, 0), 1, context, 0);
        
        //LandMass(new Vector3Int(49, 50, 0), 1, context,0);//
       // LandMass(new Vector3Int(51, 50, 0), 1, context,0);//
        //LandMass(new Vector3Int(49, 49, 0), 1, context,0);//
        //LandMass(new Vector3Int(50, 51, 0), 1, context,0);//
       // LandMass(new Vector3Int(49, 51, 0), 1, context,0);//
        //LandMass(new Vector3Int(50, 49, 0), 1, context,0); 
       // LandMass(context.gameModel.OffsetCoord(new Vector3Int(50,50,0)), 1, context, 0);
      //  LandMass(context.gameModel.OffsetCoord(new Vector3Int(49,50,0)), 1, context, 0);
       // LandMass(context.gameModel.OffsetCoord(new Vector3Int(49,51,0)), 1, context, 0);
        //LandMass(context.gameModel.OffsetCoord(new Vector3Int(50,51,0)), 1, context, 0);
        //LandMass(context.gameModel.OffsetCoord(new Vector3Int(51,50,0)), 1, context, 0);
        //LandMass(context.gameModel.OffsetCoord(new Vector3Int(50,49,0)), 1, context, 0);
        //LandMass(context.gameModel.OffsetCoord(new Vector3Int(51,49,0)), 1, context, 0);


       // Main Landmass
       /*  int rangeMin = 3;
        int rangeMax = 8;
        int continentSpacing = 40;
        int numContinents = context.gameModel.COLS/continentSpacing;
        for(int c = 0; c< numContinents; c++){
            int numSplats = Random.Range(context.gameModel.ROWS/(rangeMax)*3/2, context.gameModel.ROWS/(rangeMin)*3/2);
            for (int i = 0; i<numSplats; i++){
                int range = Random.Range(rangeMin,rangeMax);
                int y = Random.Range(range-1, context.gameModel.ROWS-range);
                int x = Random.Range(0,continentSpacing/2) + (c*continentSpacing);
                LandMass(new Vector3Int(x,y,0), range, context, 0);
            }
        } */
 
        //Perlin noise
/* 
        float noiseRes = 0.05f;
        float noiseScale = 0.7f;
        Vector2 noiseOffset = new Vector2(Random.Range(0f,1f), Random.Range(0f,1f));
        for (int column = 0; column<context.gameModel.COLS;column++){
            for(int row = 0; row < context.gameModel.ROWS; row++){
                float perlinVal = Mathf.PerlinNoise((float)column/Mathf.Max(context.gameModel.COLS, context.gameModel.ROWS)/noiseRes + noiseOffset.x,
                 (float)row/Mathf.Max(context.gameModel.COLS, context.gameModel.ROWS)/noiseRes + noiseOffset.y)*noiseScale;
                if (perlinVal>0.5) context.gameModel.terrainModel[row, column].tile = context.grassTile;
                else if (perlinVal<0.1) context.gameModel.terrainModel[row, column].tile = context.waterTile;
            }
        }
*/
        RefreshMap(context);
    }

    private void LandMass(Vector3Int center, int radius, GameActivity context, int count){
        List<Vector3Int> locs = context.gameModel.GetLocationsWithinRangeOf(center, radius);
        foreach(Vector3Int loc in locs){
            context.gameModel.SetHexModelTile(loc.x, loc.y, context.grassTile);
            if(loc.y<context.gameModel.ROWS&&loc.y>=0&&count<3){
                
                int r = Random.Range(0,8);  
                Debug.Log(loc);
                Debug.Log(r);   
                
                Vector3Int cubeCoord = context.gameModel.CubeCoord(loc);       
                if(r==1){
                    LandMass(new Vector3Int(loc.x, loc.y, 0), 2, context, count++);
                }else if(r==2){
                    cubeCoord.x--;
                    cubeCoord.y++;
                    LandMass(context.gameModel.OffsetCoord(cubeCoord), 1, context, count++);
                }else if(r==3){
                    cubeCoord.y++;
                    LandMass(context.gameModel.OffsetCoord(cubeCoord), 1, context, count++);
                }else if(r==4){
                    cubeCoord.x++;
                    LandMass(context.gameModel.OffsetCoord(cubeCoord), 1, context, count++);
                }else if(r==5){
                    cubeCoord.x++;
                    cubeCoord.y--;
                    LandMass(context.gameModel.OffsetCoord(cubeCoord), 1, context, count++);
                }else if(r==6){
                    cubeCoord.y--;
                    LandMass(context.gameModel.OffsetCoord(cubeCoord), 1, context, count++);
                }else if(r==7){
                    cubeCoord.x--;
                    LandMass(context.gameModel.OffsetCoord(cubeCoord), 1, context, count++);
                }  
            }
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
