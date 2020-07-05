using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainMap : Observer
{
    private double northPole;
    private double northTundra;
    private double northGreen;
    private double northDesert;
    private double southDesert;
    private double southGreen;
    private double southTundra;
    private double southPole;
    public TerrainMap(GameActivity context){
        southPole = 0.05*context.gameModel.ROWS;
        southTundra = 0.1 * context.gameModel.ROWS;
        southGreen = 0.35* context.gameModel.ROWS;
        southDesert = 0.43*context.gameModel.ROWS;
        northPole = context.gameModel.ROWS-southPole;
        northTundra = context.gameModel.ROWS-southTundra;
        northGreen = context.gameModel.ROWS-southGreen;
        northDesert = context.gameModel.ROWS - southDesert;

        CreateWater(context);
        CreateContinents (context);
        //context.terrain.SetTile(new Vector3Int(50,50,0), context.gravelTile);
    }

    private void CreateWater(GameActivity context){
        for (int column = 0; column<context.gameModel.COLS;column++){
            for(int row = 0; row < context.gameModel.ROWS; row++){
                HexModel h = new HexModel(column, row);
                h.terrainTile = context.waterTile;
                context.gameModel.terrainModel[row,column] = h;
                context.terrain.SetTile(h.PositionFromCamera(Camera.main.transform.position, context.gameModel.COLS, context.gameModel.ROWS), h.terrainTile );
            }
        }
    }

    private void CreateContinents(GameActivity context){

        //Test
       // LandMass(new Vector3Int(50, 50, 0), 1, context);//
       // LandMass(new Vector3Int(70,50, 0), 1, context);
       // RandomSpawns(context.gameModel.GetLocationsOnRing(new Vector3Int(50,50,0), 6), context,0);
        //LandMass(new Vector3Int(49, 50, 0), 1, context);//
       // LandMass(new Vector3Int(51, 50, 0), 1, context);//
        //LandMass(new Vector3Int(49, 49, 0), 1, context);//
        //LandMass(new Vector3Int(50, 51, 0), 1, context);//
       // LandMass(new Vector3Int(49, 51, 0), 1, context);//
        //LandMass(new Vector3Int(50, 49, 0), 1, context); 
       // LandMass(context.gameModel.OffsetCoord(new Vector3Int(50,50,0)), 1, context);
      //  LandMass(context.gameModel.OffsetCoord(new Vector3Int(49,50,0)), 1, context);
       // LandMass(context.gameModel.OffsetCoord(new Vector3Int(49,51,0)), 1, context);
        //LandMass(context.gameModel.OffsetCoord(new Vector3Int(50,51,0)), 1, context);
        //LandMass(context.gameModel.OffsetCoord(new Vector3Int(51,50,0)), 1, context);
        //LandMass(context.gameModel.OffsetCoord(new Vector3Int(50,49,0)), 1, context);
        //LandMass(context.gameModel.OffsetCoord(new Vector3Int(51,49,0)), 1, context);

       // Main Landmass
        
        int rangeMin = 3;
        int rangeMax = 5;
        int continentSpacing = 40;
        int numContinents = context.gameModel.COLS/continentSpacing;
        for(int c = 0; c< numContinents; c++){
            int numSplats = Random.Range(context.gameModel.ROWS/(rangeMax)*3/2, context.gameModel.ROWS/(rangeMin)*3/2);
            for (int i = 0; i<numSplats; i++){
                int range = Random.Range(rangeMin,rangeMax);
                int y = Random.Range(range-1, context.gameModel.ROWS-range);
                int x = Random.Range(0,continentSpacing/2) + (c*continentSpacing);
                LandMass(new Vector3Int(x,y,0), range, context, GetAltitudeTile(y, context));
                RandomSpawns(context.gameModel.GetLocationsOnRing(new Vector3Int(x,y,0), range), context,0, GetAltitudeTile(y, context));
            }
        } 
 
        //Perlin noise islands
 
        float noiseRes = 0.05f;
        float noiseScale = 0.7f;
        Vector2 noiseOffset = new Vector2(Random.Range(0f,1f), Random.Range(0f,1f));
        for (int column = 0; column<context.gameModel.COLS;column++){
            for(int row = 0; row < context.gameModel.ROWS; row++){
                float perlinVal = Mathf.PerlinNoise((float)(column+1)/Mathf.Max(context.gameModel.COLS, context.gameModel.ROWS)/noiseRes + noiseOffset.x,
                 (float)(row+1)/Mathf.Max(context.gameModel.COLS, context.gameModel.ROWS)/noiseRes + noiseOffset.y)*noiseScale;
                if (perlinVal>0.5) context.gameModel.terrainModel[row, column].terrainTile = GetAltitudeTile(row, context);
                else if (perlinVal<0.1) context.gameModel.terrainModel[row, column].terrainTile = context.waterTile;
            }
        }

        //Perlin noise hills
        noiseRes = 0.01f;
        noiseScale = 0.85f;
        noiseOffset = new Vector2(Random.Range(0f,1f), Random.Range(0f,1f));
        for (int column = 0; column<context.gameModel.COLS;column++){
            for(int row = 0; row < context.gameModel.ROWS; row++){
                float perlinVal = Mathf.PerlinNoise((float)(column+1)/Mathf.Max(context.gameModel.COLS, context.gameModel.ROWS)/noiseRes + noiseOffset.x,
                 (float)(row+1)/Mathf.Max(context.gameModel.COLS, context.gameModel.ROWS)/noiseRes + noiseOffset.y)*noiseScale;
                if (perlinVal>0.5){
                    if(context.gameModel.terrainModel[row, column].terrainTile==context.grassTile){
                        context.gameModel.terrainModel[row, column].terrainTile = context.grassHillTile;
                    } else if(context.gameModel.terrainModel[row, column].terrainTile==context.desertTile){
                        context.gameModel.terrainModel[row, column].terrainTile = context.desertHillTile;
                    }else if(context.gameModel.terrainModel[row, column].terrainTile==context.tundraTile){
                        context.gameModel.terrainModel[row, column].terrainTile = context.tundraHillTile;
                    }else if(context.gameModel.terrainModel[row, column].terrainTile==context.snowTile){
                        context.gameModel.terrainModel[row, column].terrainTile = context.snowHillTile;
                    }  
                }
            }
        }

        //Perlin noise mountains
        noiseRes = 0.06f;
        noiseScale = 0.65f;
        noiseOffset = new Vector2(Random.Range(0f,1f), Random.Range(0f,1f));
        for (int column = 0; column<context.gameModel.COLS;column++){
            for(int row = 0; row < context.gameModel.ROWS; row++){
                float perlinVal = Mathf.PerlinNoise((float)(column+1)/Mathf.Max(context.gameModel.COLS, context.gameModel.ROWS)/noiseRes + noiseOffset.x,
                 (float)(row+1)/Mathf.Max(context.gameModel.COLS, context.gameModel.ROWS)/noiseRes + noiseOffset.y)*noiseScale;
                if (perlinVal<0.1&&context.gameModel.terrainModel[row, column].terrainTile != context.waterTile){
                    context.gameModel.terrainModel[row, column].terrainTile = context.mountainTile;
                    context.gameModel.terrainModel[row, column].resourceTile = context.mountainDefaultTile;
                }
            }
        }

       //Perlin noise forests
        noiseRes = 0.1f;
        noiseScale = 0.85f;
        noiseOffset = new Vector2(Random.Range(0f,1f), Random.Range(0f,1f));
        for (int column = 0; column<context.gameModel.COLS;column++){
            for(int row = 0; row < context.gameModel.ROWS; row++){
                float perlinVal = Mathf.PerlinNoise((float)(column+1)/Mathf.Max(context.gameModel.COLS, context.gameModel.ROWS)/noiseRes + noiseOffset.x,
                 (float)(row+1)/Mathf.Max(context.gameModel.COLS, context.gameModel.ROWS)/noiseRes + noiseOffset.y)*noiseScale;
                if (perlinVal>0.5&&(context.gameModel.terrainModel[row, column].terrainTile == context.grassTile||context.gameModel.terrainModel[row, column].terrainTile == context.grassHillTile)){
                    context.gameModel.terrainModel[row, column].resourceTile = context.grassForestTile;
                }else if(perlinVal>0.5&&(context.gameModel.terrainModel[row, column].terrainTile == context.tundraTile||context.gameModel.terrainModel[row, column].terrainTile == context.tundraHillTile)){
                    context.gameModel.terrainModel[row, column].resourceTile = context.tundraForestTile;
                }else if(perlinVal>0.65&&(context.gameModel.terrainModel[row, column].terrainTile == context.desertTile||context.gameModel.terrainModel[row, column].terrainTile == context.desertHillTile)){
                    context.gameModel.terrainModel[row, column].resourceTile = context.desertJungleTile;
                }else if(perlinVal>0.6&&(context.gameModel.terrainModel[row, column].terrainTile == context.snowTile||context.gameModel.terrainModel[row, column].terrainTile == context.snowHillTile)){
                    context.gameModel.terrainModel[row, column].resourceTile = context.snowForestTile;
                }
            }
            
        }

        RefreshMap(context);
    }

    private void LandMass(Vector3Int center, int radius, GameActivity context, Tile tile){
        List<Vector3Int> locs = context.gameModel.GetLocationsWithinRangeOf(center, radius);
        foreach(Vector3Int loc in locs){
            context.gameModel.SetHexModelTile(loc.x, loc.y, tile);
        }
    }

    private void RandomSpawns(List<Vector3Int> locs, GameActivity context, int count, Tile tile){
        foreach (Vector3Int loc in locs){
            Vector3Int cubeloc = context.gameModel.CubeCoord(loc);
            for (int i =0;i<3;i++){
                int r = Random.Range(0,10);
                if(r==1){
                    cubeloc.x++;
                }else if(r==2){
                    cubeloc.y++;
                }else if(r==3){
                    cubeloc.y++;
                    cubeloc.x--;
                }else if(r==4){
                    cubeloc.x--;
                }else if(r==5){
                    cubeloc.y--;
                }else if(r==6){
                    cubeloc.y--;
                    cubeloc.x++;
                }
                Vector3Int offsetloc = context.gameModel.OffsetCoord(cubeloc);
                if(r==7&&count<3){
                    LandMass(offsetloc, 2, context, tile);
                    List<Vector3Int> ringlocs = context.gameModel.GetLocationsOnRing(offsetloc,2);
                    RandomSpawns(ringlocs, context, count+1, tile);
                    break;
                }else{
                    context.gameModel.SetHexModelTile(offsetloc.x, offsetloc.y, tile);
                }
            }
        }
    }



    private void RefreshMap(GameActivity context){
        context.terrain.ClearAllTiles();
        for(int q = 0;q<context.gameModel.COLS;q++){
            for(int r = 0;r<context.gameModel.ROWS;r++){
                HexModel h = context.gameModel.terrainModel[r,q];
                context.terrain.SetTile(h.PositionFromCamera(Camera.main.transform.position, context.gameModel.COLS, context.gameModel.ROWS),h.terrainTile );
                context.resources.SetTile(h.PositionFromCamera(Camera.main.transform.position, context.gameModel.COLS, context.gameModel.ROWS),h.resourceTile);
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
            context.terrain.SetTile(newLeftPos, hLeft.terrainTile);
            context.terrain.SetTile(oldRightPos, null);
            context.terrain.SetTile(newRightPos, hRight.terrainTile);

            context.resources.SetTile(oldLeftPos, null);
            context.resources.SetTile(newLeftPos, hLeft.resourceTile);
            context.resources.SetTile(oldRightPos, null);
            context.resources.SetTile(newRightPos, hRight.resourceTile);
        }

    }

    private Tile GetAltitudeTile(int y, GameActivity context){
        //Debug.Log(y);
        if(y>=0&&y<southPole){
            return context.snowTile;
        }else if(y>=southPole&&y<southTundra){
            return context.tundraTile;
        }else if(y>=southTundra&&y<southGreen){
            return context.grassTile;
        }else if(y>=southGreen&&y<southDesert){
            return context.desertTile;
        }else if(y>=southDesert&&y<=northDesert){
            return context.grassTile;
        }else if(y>northDesert&&y<=northGreen){
            return context.desertTile;
        }else if(y>northGreen&&y<=northTundra){
            return context.grassTile;
        }else if(y>northTundra&&y<=northPole){
            return context.tundraTile;
        }else if(y>northPole&&y<context.gameModel.ROWS){
            return context.snowTile;
        }else{
            return context.grassTile;
        }
    }

    public void Update(GameActivity gameActivity){
        UpdateMap(gameActivity);
    }
}
