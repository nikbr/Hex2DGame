using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using static TerrainTile;
using static ResourceTile;
using static TerrainBodyType;
using static Direction;
public class TerrainMap : Observer
{   private SystemRandom randomGen;
    private double northPole;
    private double northTundra;
    private double northGreen;
    private double northDesert;
    private double southDesert;
    private double southGreen;
    private double southTundra;
    private double southPole;

    private int currentChunkNumber;
    public TerrainMap(GameActivity context){
        randomGen = new SystemRandom();
        southPole = 0.05*context.gameModel.ROWS;
        southTundra = 0.1 * context.gameModel.ROWS;
        southGreen = 0.35* context.gameModel.ROWS;
        southDesert = 0.43*context.gameModel.ROWS;
        northPole = context.gameModel.ROWS-southPole;
        northTundra = context.gameModel.ROWS-southTundra;
        northGreen = context.gameModel.ROWS-southGreen;
        northDesert = context.gameModel.ROWS - southDesert;

        currentChunkNumber=0;

        CreateWater(context);
        CreateContinents (context);
        ChunkifyByTerrain(context);
        MarkBodiesOfWater(context);
        CreateRivers(context);
        CreateCities(context);
        RefreshMap(context);
    }

    private void CreateWater(GameActivity context){
        for (int column = 0; column<context.gameModel.COLS;column++){
            for(int row = 0; row < context.gameModel.ROWS; row++){
                HexModel h = new HexModel(column, row, Water);
                context.gameModel.terrainModel[row,column] = h;
                context.terrain.SetTile(h.PositionFromCamera(Camera.main.transform.position, context.gameModel.COLS, context.gameModel.ROWS), h.terrain(context));
            }
        }
    }

    private void CreateContinents(GameActivity context){

        //Test
       // LandMass(new Vector3Int(50, 50, 0), 1, context);//
       // LandMass(new Vector3Int(70,50, 0), 1, context);
        //RandomSpawns(context.gameModel.GetLocationsOnRing(new Vector3Int(50,50,0), 6), context,0);
       // List<Vector3Int> ringLocs = context.gameModel.GetLocationsOnRing(new Vector3Int(50,50,0), 6);
       // foreach(Vector3Int loc in ringLocs){
        //    context.gameModel.SetHexModelTile(loc.x, loc.y, 1);
        //}
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
                else if (perlinVal<0.1) context.gameModel.terrainModel[row, column].terrainTile = Water;
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
                    if (context.gameModel.terrainModel[row, column].terrainTile == Grass){
                        context.gameModel.terrainModel[row, column].terrainTile = GrassHill;
                    } else if (context.gameModel.terrainModel[row, column].terrainTile == Desert){
                        context.gameModel.terrainModel[row, column].terrainTile = DesertHill;
                    } else if (context.gameModel.terrainModel[row, column].terrainTile == Tundra){
                        context.gameModel.terrainModel[row, column].terrainTile = TundraHill;
                    } else if (context.gameModel.terrainModel[row, column].terrainTile == Snow){
                        context.gameModel.terrainModel[row, column].terrainTile = SnowHill;
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
                if (perlinVal<0.1&&context.gameModel.terrainModel[row, column].terrainTile != Water){
                    context.gameModel.terrainModel[row, column].terrainTile = Mountain;
                    context.gameModel.terrainModel[row, column].resourceTile = MountainDefault;
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

                HexModel curTile = context.gameModel.terrainModel[row, column];
                //if (perlinVal > 0.5 && curTile.terrainTile = context.terrainTile[Grass])
                if (perlinVal>0.5&&(curTile.terrainTile == Grass ||curTile.terrainTile == GrassHill)){
                    curTile.resourceTile = GrassForest;
                }else if(perlinVal>0.5&&(curTile.terrainTile == Tundra ||curTile.terrainTile == TundraHill)){
                    curTile.resourceTile = TundraForest;
                }else if(perlinVal>0.65&&(curTile.terrainTile == Desert ||curTile.terrainTile == DesertHill)){
                    curTile.resourceTile = DesertJungle;
                }else if(perlinVal>0.6&&(curTile.terrainTile == Snow ||curTile.terrainTile == SnowHill)){
                    curTile.resourceTile = SnowForest;
                }
            }
        }
    }

    private void ChunkifyByTerrain(GameActivity context){
        for (int column = 0; column<context.gameModel.COLS;column++){
            for(int row = 0; row < context.gameModel.ROWS; row++){
                AssignChunkNumber(row, column, context);
            }
        }
    }

    private void AssignChunkNumber(int row, int column, GameActivity context){
        HexModel hex = context.gameModel.GetHexModel(column, row);
        Queue<HexModel> processQueue = new Queue<HexModel>();
        processQueue.Enqueue(hex);
        int chunkNumOverride = -1;
        while(processQueue.Count>0){
            hex = processQueue.Dequeue();
            if(hex.terrainChunk==null){
                Vector3Int[] neighbors = context.gameModel.GetNeighbors(new Vector3Int(hex.COL, hex.ROW, 0));

                if(chunkNumOverride>=0){
                    context.gameModel.SetHexModelTerrainChunk(hex.COL, hex.ROW, chunkNumOverride);
                    context.gameModel.terrainChunks[chunkNumOverride].Insert(context.gameModel.GetHexModel(hex.COL, hex.ROW));
                }else{
                    context.gameModel.SetHexModelTerrainChunk(hex.COL, hex.ROW, currentChunkNumber);
                    context.gameModel.terrainChunks.Add(currentChunkNumber, new TerrainChunk(currentChunkNumber, (int)context.gameModel.GetHexModel(column, row).terrainTile));
                    context.gameModel.terrainChunks[currentChunkNumber].Insert(context.gameModel.GetHexModel(hex.COL, hex.ROW));
                    chunkNumOverride=currentChunkNumber;
                    currentChunkNumber++;
                }
                foreach(Vector3Int neighbor in neighbors){
                    HexModel neighborHex = context.gameModel.GetHexModel(neighbor.x, neighbor.y);
                    if(neighborHex!=null&&hex.terrainTile==neighborHex.terrainTile){
                        processQueue.Enqueue(neighborHex);
                    }
                }
            }
        }
    }

    private void MarkBodiesOfWater(GameActivity context){
        for (int column = 0; column<context.gameModel.COLS;column++){
            for(int row = 0; row < context.gameModel.ROWS; row++){
                HexModel hex = context.gameModel.GetHexModel(column, row);
                Vector3Int[] neighborLocs = context.gameModel.GetNeighbors(new Vector3Int(column, row, 0));
                if(!hex.IsWater()){
                    context.gameModel.GetHexModel(column, row).terrainBodyType=Land;
                    if(context.gameModel.GetHexModel(column, row).terrainTile==Mountain){
                        context.gameModel.GetHexModel(column, row).terrainBodyType=MountainRange;
                    }
                    foreach(Vector3Int neighborLoc in neighborLocs){
                        HexModel neighbor = context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y);
                        if(neighbor!=null&&neighbor.IsWater()) hex.coastal = true;
                    };
                }else{
                    if(context.gameModel.terrainChunks[(int)hex.terrainChunk].Size()<40){
                        context.gameModel.GetHexModel(column, row).terrainBodyType=Lake;
                    }else if(context.gameModel.terrainChunks[(int)hex.terrainChunk].Size()>=40&&context.gameModel.terrainChunks[(int)hex.terrainChunk].Size()<500){
                        context.gameModel.GetHexModel(column, row).terrainBodyType=Sea;
                    }
                    foreach(Vector3Int neighborLoc in neighborLocs){
                        HexModel neighbor = context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y);
                        if(neighbor!=null&&!neighbor.IsWater()) hex.coastal = true;
                    };
                }
            }
        }
    }

    private void CreateRivers(GameActivity context){
        for (int column = 0; column<context.gameModel.COLS;column++){ //maybe reduce this to specific chunks of appropriate type
            for(int row = 0; row < context.gameModel.ROWS; row++){
                HexModel hex = context.gameModel.GetHexModel(column, row);
                if((hex.terrainBodyType==Sea||hex.terrainBodyType==Lake||hex.terrainBodyType==Ocean)&&hex.coastal||hex.terrainBodyType==MountainRange){
                    Vector3Int[] neighborLocs = context.gameModel.GetNeighbors(new Vector3Int(column, row, 0));
                    int randOffset = randomGen.Range(0, neighborLocs.Length); //to remove directional bias
                    for(int i = randOffset;i<neighborLocs.Length+randOffset;i++){
                        int index = i%neighborLocs.Length;
                        HexModel neighborHex = context.gameModel.GetHexModel(neighborLocs[index].x, neighborLocs[index].y);
                        if(neighborHex==null) continue;
                        int blockerOneLoc = index-1;
                        if(blockerOneLoc<0){
                            blockerOneLoc+=6;
                        }else if(blockerOneLoc>0){
                            blockerOneLoc%=6;
                        }
                        int blockerTwoLoc = index+1;
                        if(blockerTwoLoc<0){
                            blockerTwoLoc+=6;
                        }else if(blockerTwoLoc>0){
                            blockerTwoLoc%=6;
                        }
                        //Debug.Log(neighborLocs.Length + " "+ blockerOneLoc + " " + blockerTwoLoc);
                        HexModel blockerOneHex = context.gameModel.GetHexModel(neighborLocs[blockerOneLoc].x, neighborLocs[blockerOneLoc].y);
                        HexModel blockerTwoHex = context.gameModel.GetHexModel(neighborLocs[blockerTwoLoc].x, neighborLocs[blockerTwoLoc].y);
                        int? blockerOne=null;
                        int? blockerTwo=null;
                        if(blockerOneHex!=null)blockerOne=blockerOneHex.riverTile;
                        if(blockerTwoHex!=null)blockerTwo=blockerTwoHex.riverTile;
                        if(IsRiverTraversible((int)neighborHex.terrainTile)&&blockerOne==null&&blockerTwo==null&&blockerOneHex!=null&&blockerOneHex.terrainTile!=Water&&blockerOneHex.terrainTile!=Mountain){
                            int randInt = randomGen.Range(0, 30);
                            if((hex.terrainBodyType==Ocean&&randInt==2)||(hex.terrainBodyType!=Ocean&&randInt<4)){
                                context.gameModel.GetHexModel(column, row).riverTile=RiverTile.Source;
                                context.gameModel.GetHexModel(neighborLocs[index].x, neighborLocs[index].y).riverTile=RiverTile.Left;
                                context.gameModel.GetHexModel(neighborLocs[blockerOneLoc].x, neighborLocs[blockerOneLoc].y).riverTile=RiverTile.Right;
                                Debug.Log(Direction.OppositeDirection(index));
                                context.gameModel.GetHexModel(column, row).nextLeftRiverDirection = index;
                                context.gameModel.GetHexModel(neighborLocs[index].x, neighborLocs[index].y).previousLeftRiverDirection = Direction.OppositeDirection(index);
                                SpawnNextLeftRiver(neighborLocs[index].x, neighborLocs[index].y, context);
                                return;
                            }
                        }
                    }
                    //Replace placeholders here
                    
                }
            }
        }
    }

    public void SpawnNextLeftRiver(int x, int y, GameActivity context){
        Vector3Int[] neighborLocs = context.gameModel.GetNeighbors(new Vector3Int(x, y, 0));
        int randOffset = randomGen.Range(0, neighborLocs.Length); //to remove directional bias

        for(int i = randOffset;i<neighborLocs.Length+randOffset;i++){
            int index = i%neighborLocs.Length;
            Vector3Int neighborLoc = neighborLocs[index];
            HexModel neighborHex = context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y);
            if(neighborHex==null) continue;
            int randomGenMax = 1000;
            int randInt = randomGen.Range(0, randomGenMax);
            int probability;
            if(IsDry((int) neighborHex.terrainTile)){
                probability = 730;
            }else{
                probability = 950;
            }
            Debug.Log("OppositeDirection: " + Direction.OppositeDirection(index));
            Debug.Log(randInt);
            if(randInt>500&&neighborHex.terrainTile==Water&&CanFitRightSide(x, y, neighborLoc.x, neighborLoc.y, index, context)){
                FillRightSide(x, y, neighborLoc.x, neighborLoc.y, index, context);
                return;
            }
            if(IsRiverTraversible((int) neighborHex.terrainTile)&&neighborHex.riverTile!=RiverTile.Right&&neighborHex.riverTile!=RiverTile.Left&&CanFitRightSide(x, y, neighborLoc.x, neighborLoc.y, index, context)){
                
                if(randInt<probability){
                    context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y).riverTile=RiverTile.Left;
                    context.gameModel.GetHexModel(x, y).nextLeftRiverDirection = index;
                    context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y).previousLeftRiverDirection = Direction.OppositeDirection(index);
                    SpawnNextLeftRiver(neighborLoc.x, neighborLoc.y, context);
                    FillRightSide(x, y, neighborLoc.x, neighborLoc.y, index, context);
                    return;
                }
            }
        }
        return;
    }

    public bool CanFitRightSide(int currentX, int currentY, int nextX, int nextY, int nextDirection, GameActivity context){
        HexModel currentHex = context.gameModel.GetHexModel(currentX, currentY);
        HexModel nextHex = context.gameModel.GetHexModel(nextX, nextY);
        Vector3Int previousHexLoc = context.gameModel.GetNeighbor(new Vector3Int(currentX, currentY, 0), (int)currentHex.previousLeftRiverDirection);
        HexModel previousHex = context.gameModel.GetHexModel(previousHexLoc.x, previousHexLoc.y);

        if(currentHex.previousLeftRiverDirection+1>nextDirection) nextDirection+=Direction.LENGTH;

        bool forLoopRan = false;
        Debug.Log(currentHex.previousLeftRiverDirection + " " + nextDirection);
        for(int i = (int)currentHex.previousLeftRiverDirection+1;i<nextDirection;i++){
            Debug.Log(i);
            forLoopRan = true;
            int index = i%Direction.LENGTH;
            Vector3Int hexToCheckLoc = context.gameModel.GetNeighbor(new Vector3Int(currentX, currentY, 0), index);
            HexModel hexToCheck = context.gameModel.GetHexModel(hexToCheckLoc.x, hexToCheckLoc.y);
            if(!IsRiverTraversible((int) hexToCheck.terrainTile)||hexToCheck.riverTile==RiverTile.Left){
                return false;
            }
        }
        return forLoopRan;
    }

    public void FillRightSide(int currentX, int currentY, int nextX, int nextY, int nextDirection, GameActivity context){
        HexModel currentHex = context.gameModel.GetHexModel(currentX, currentY);
        HexModel nextHex = context.gameModel.GetHexModel(nextX, nextY);
        Vector3Int previousHexLoc = context.gameModel.GetNeighbor(new Vector3Int(currentX, currentY, 0), (int)currentHex.previousLeftRiverDirection);
        HexModel previousHex = context.gameModel.GetHexModel(previousHexLoc.x, previousHexLoc.y);
        if(currentHex.previousLeftRiverDirection+1>nextDirection) nextDirection+=Direction.LENGTH;
        for(int i = (int)currentHex.previousLeftRiverDirection+1;i<nextDirection;i++){
            int index = i%Direction.LENGTH;
            Vector3Int neighborLoc = context.gameModel.GetNeighbor(new Vector3Int(currentX, currentY, 0), index);
            context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y).riverTile=RiverTile.Right;
        }
    }

    private void CreateCities(GameActivity context){

    }

    private void LandMass(Vector3Int center, int radius, GameActivity context, int tile){
        List<Vector3Int> locs = context.gameModel.GetLocationsWithinRangeOf(center, radius);
        foreach(Vector3Int loc in locs){
            context.gameModel.SetHexModelTile(loc.x, loc.y, tile);
        }
    }

    private void RandomSpawns(List<Vector3Int> locs, GameActivity context, int count, int tile){
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
                context.terrain.SetTile(h.PositionFromCamera(Camera.main.transform.position, context.gameModel.COLS, context.gameModel.ROWS), h.terrain(context));
                context.resources.SetTile(h.PositionFromCamera(Camera.main.transform.position, context.gameModel.COLS, context.gameModel.ROWS), h.resource(context));
                context.rivers.SetTile(h.PositionFromCamera(Camera.main.transform.position, context.gameModel.COLS, context.gameModel.ROWS), h.river(context));
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
            context.terrain.SetTile(newLeftPos, hLeft.terrain(context));
            context.terrain.SetTile(oldRightPos, null);
            context.terrain.SetTile(newRightPos, hRight.terrain(context));

            context.resources.SetTile(oldLeftPos, null);
            context.resources.SetTile(newLeftPos, hLeft.resource(context));
            context.resources.SetTile(oldRightPos, null);
            context.resources.SetTile(newRightPos, hRight.resource(context));

            context.rivers.SetTile(oldLeftPos, null);
            context.rivers.SetTile(newLeftPos, hLeft.river(context));
            context.rivers.SetTile(oldRightPos, null);
            context.rivers.SetTile(newRightPos, hRight.river(context));
        }

    }

    private int GetAltitudeTile(int y, GameActivity context){
        //Debug.Log(y);
        if(y>=0&&y<southPole){
            return Snow;
        }else if(y>=southPole&&y<southTundra){
            return Tundra;
        }else if(y>=southTundra&&y<southGreen){
            return Grass;
        }else if(y>=southGreen&&y<southDesert){
            return Desert;
        }else if(y>=southDesert&&y<=northDesert){
            return Grass;
        }else if(y>northDesert&&y<=northGreen){
            return Desert;
        }else if(y>northGreen&&y<=northTundra){
            return Grass;
        }else if(y>northTundra&&y<=northPole){
            return Tundra;
        }else if(y>northPole&&y<context.gameModel.ROWS){
            return Snow;
        }else{
            return Grass;
        }
    }

    public void Update(GameActivity gameActivity){
        UpdateMap(gameActivity);
    }
}
