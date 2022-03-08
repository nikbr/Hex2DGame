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
        RefreshMap(context);
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
/*
        //Perlin noise extra lakes
        noiseRes = 0.1f;
        noiseScale = 0.85f;
        noiseOffset = new Vector2(Random.Range(0f,1f), Random.Range(0f,1f));
        for (int column = 0; column<context.gameModel.COLS;column++){
            for(int row = 0; row < context.gameModel.ROWS; row++){
                float perlinVal = Mathf.PerlinNoise((float)(column+1)/Mathf.Max(context.gameModel.COLS, context.gameModel.ROWS)/noiseRes + noiseOffset.x,
                 (float)(row+1)/Mathf.Max(context.gameModel.COLS, context.gameModel.ROWS)/noiseRes + noiseOffset.y)*noiseScale;

                HexModel curTile = context.gameModel.terrainModel[row, column];
                //if (perlinVal > 0.5 && curTile.terrainTile = context.terrainTile[Grass])
                if (perlinVal>0.8&&(curTile.terrainTile == Grass ||curTile.terrainTile == GrassHill)){
                    curTile.resourceTile = null;
                    curTile.terrainTile = Water;
                }else if(perlinVal>0.8&&(curTile.terrainTile == Tundra ||curTile.terrainTile == TundraHill)){
                    curTile.resourceTile = null;
                    curTile.terrainTile = Water;
                }else if(perlinVal>0.95&&(curTile.terrainTile == Desert ||curTile.terrainTile == DesertHill)){
                    curTile.resourceTile = null;
                    curTile.terrainTile = Water;
                }else if(perlinVal>0.95&&(curTile.terrainTile == Snow ||curTile.terrainTile == SnowHill)){
                    curTile.resourceTile = null;
                    curTile.terrainTile = Water;
                }
            }
        }
        */
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
        ChunkifyByTerrain(context);
        MarkBodiesOfWater(context);

        //CreateSeaLakeRivers(context);
        //CreateMountainRivers(context);
        SpawnMountainRivers(context, Ocean);
        SpawnMountainRivers(context, Lake);
        ClearRedundantRivers(context);
        SpawnAquiferRivers(context, Ocean);
        ClearRedundantRivers(context);

        RemoveDryLakes(context);
        RefreshMap(context);
        
//
        //CreateHillRivers(context);
    }

    private void ClearRedundantRivers(GameActivity context){
        //remove rivers in water
        for (int column = 0; column<context.gameModel.COLS;column++){
            for(int row = 0; row < context.gameModel.ROWS; row++){
                HexModel h = context.gameModel.GetHexModel(column, row);
                if(h.terrainTile==Water&&h.riverNeighbors!=null){
                    for(int i = 0; i<Direction.LENGTH;i++){
                        if(h.riverNeighbors[i]){
                            Vector3Int neighborLoc = context.gameModel.GetNeighbor(new Vector3Int(column, row,0), i);
                            context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y).RemoveRiverNeighbor(Direction.OppositeDirection(i));
                            context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y).nextRiverDirection=null;
                            context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y).previousRiverDirection=null;
                        }
                    }
                    context.gameModel.GetHexModel(column, row).riverNeighbors=null;
                    context.gameModel.GetHexModel(column, row).nextRiverDirection=null;
                    context.gameModel.GetHexModel(column, row).previousRiverDirection=null;
                }

                if(h.riverNeighbors!=null){
                    for(int i = 0;i<Direction.LENGTH;i++){
                        if(h.riverNeighbors[i]){
                            HexModel n = h.GetNeighbor(i, context);
                            if(n.riverNeighbors!=null){
                                if(!n.riverNeighbors[Direction.OppositeDirection(i)])context.gameModel.GetHexModel(column, row).RemoveRiverNeighbor(i);
                            }else{
                                context.gameModel.GetHexModel(column, row).RemoveRiverNeighbor(i);
                            }
                        }
                    }
                }
            }
        }
    }
    private void RemoveDryLakes(GameActivity context){

    }

    private void SpawnAquiferRivers(GameActivity context, int destination){
        foreach(KeyValuePair<int, TerrainChunk> entry in context.gameModel.terrainChunks){ 
            if(entry.Value.GetBodyType(context)==Land){
                foreach(Vector3Int hexLoc in entry.Value.GetHexesLocations()){
                    HexModel hex = context.gameModel.GetHexModel(hexLoc.x, hexLoc.y);
                    if((hex.terrainTile==GrassHill||hex.terrainTile==TundraHill)&&!hex.riverSource&&hex.riverNeighbors==null){
                         bool isStart = false;
                        HashSet<int> visitedWaters =new HashSet<int>();;

                        if(hexLoc.y>southPole&&hexLoc.y<northPole){
                            int randomFail = randomGen.Range(0, 2);
                            if(randomFail==0){
                                Vector3Int[] neighborLocs = context.gameModel.GetNeighbors(new Vector3Int(hexLoc.x, hexLoc.y, 0));
                                int randOffset = randomGen.Range(0, neighborLocs.Length); //to remove directional bias
                                for(int i = randOffset;i<neighborLocs.Length+randOffset;i++){
                                    int index = i%neighborLocs.Length;
                                    HexModel neighborHex = context.gameModel.GetHexModel(neighborLocs[index].x, neighborLocs[index].y);
                                    if(neighborHex==null) continue;
                                    int blockerDir = index-1;
                                    if(blockerDir<0){
                                        blockerDir+=Direction.LENGTH;
                                    }else if(blockerDir>0){
                                        blockerDir%=Direction.LENGTH;
                                    }
                                    HexModel blockerHex = context.gameModel.GetHexModel(neighborLocs[blockerDir].x, neighborLocs[blockerDir].y);
                                    if(IsRiverTraversible((int)neighborHex.terrainTile)&&neighborHex.terrainTile!=Water&&blockerHex!=null&&blockerHex.terrainTile!=Water&&IsRiverTraversible((int)blockerHex.terrainTile)){
                                        isStart = true;
                                        
                                        context.gameModel.GetHexModel(neighborLocs[index].x, neighborLocs[index].y).previousRiverDirection = Direction.OppositeDirection(index);
                                        context.gameModel.GetHexModel(hexLoc.x, hexLoc.y).riverSource = true;
                                        context.gameModel.GetHexModel(hexLoc.x, hexLoc.y).riverTile=RiverTile.Source;    
                                        context.gameModel.GetHexModel(hexLoc.x, hexLoc.y).nextRiverDirection = index;
                                       // Debug.Log("yarse1");
                                        if(!SpawnNextLeftRiver(neighborLocs[index].x, neighborLocs[index].y, context, visitedWaters, destination)){
                                            isStart = false;
                                            context.gameModel.GetHexModel(hexLoc.x, hexLoc.y).nextRiverDirection = null;
                                            context.gameModel.GetHexModel(neighborLocs[index].x, neighborLocs[index].y).previousRiverDirection = null;
                                            context.gameModel.GetHexModel(hexLoc.x, hexLoc.y).riverSource = false;
                                            context.gameModel.GetHexModel(hexLoc.x, hexLoc.y).riverTile=null;
                                            continue;
                                        };
                                        break;
                                    }
                                }
                            }
                        }
                        if(isStart){
                            context.gameModel.GetHexModel(hexLoc.x,  hexLoc.y).riverTile=null;
                            Vector3Int nextLeftLoc = context.gameModel.GetNeighbor(new Vector3Int(hexLoc.x,  hexLoc.y, 0), (int)context.gameModel.GetHexModel(hexLoc.x,  hexLoc.y).nextRiverDirection); 
                            //Debug.Log("Start Clear");
                            ClearRiverPlaceHolders(nextLeftLoc.x, nextLeftLoc.y, context, visitedWaters);
                        }
                    }
                }
            }
        }
    }
    private void SpawnMountainRivers(GameActivity context, int destination){ //only to be used with chunkTypeA 
        //go through each chunk
        foreach(KeyValuePair<int, TerrainChunk> entry in context.gameModel.terrainChunks){ 
            if(entry.Value.GetBodyType(context)==MountainRange){
                foreach(Vector3Int hexLoc in entry.Value.GetHexesLocations()){
                    bool isStart = false;
                    HashSet<int> visitedWaters =new HashSet<int>();;
                    if(hexLoc.y>southPole&&hexLoc.y<northPole){
                        Vector3Int[] neighborLocs = context.gameModel.GetNeighbors(new Vector3Int(hexLoc.x, hexLoc.y, 0));
                        int randOffset = randomGen.Range(0, neighborLocs.Length); //to remove directional bias
                        for(int i = randOffset;i<neighborLocs.Length+randOffset;i++){
                            int index = i%neighborLocs.Length;
                            HexModel neighborHex = context.gameModel.GetHexModel(neighborLocs[index].x, neighborLocs[index].y);
                            if(neighborHex==null) continue;
                            int blockerDir = index-1;
                            if(blockerDir<0){
                                blockerDir+=Direction.LENGTH;
                            }else if(blockerDir>0){
                                blockerDir%=Direction.LENGTH;
                            }
                            HexModel blockerHex = context.gameModel.GetHexModel(neighborLocs[blockerDir].x, neighborLocs[blockerDir].y);
                            if(IsRiverTraversible((int)neighborHex.terrainTile)&&neighborHex.terrainTile!=Water&&blockerHex!=null&&blockerHex.terrainTile!=Water&&IsRiverTraversible((int)blockerHex.terrainTile)){
                                isStart = true;
                                
                                context.gameModel.GetHexModel(neighborLocs[index].x, neighborLocs[index].y).previousRiverDirection = Direction.OppositeDirection(index);
                                context.gameModel.GetHexModel(hexLoc.x, hexLoc.y).riverSource = true;
                                context.gameModel.GetHexModel(hexLoc.x, hexLoc.y).riverTile=RiverTile.Source;    
                                context.gameModel.GetHexModel(hexLoc.x, hexLoc.y).nextRiverDirection = index;
                                //("yarse1");
                                if(!SpawnNextLeftRiver(neighborLocs[index].x, neighborLocs[index].y, context, visitedWaters, destination)){
                                    isStart = false;
                                    context.gameModel.GetHexModel(hexLoc.x, hexLoc.y).nextRiverDirection = null;
                                    context.gameModel.GetHexModel(neighborLocs[index].x, neighborLocs[index].y).previousRiverDirection = null;
                                    context.gameModel.GetHexModel(hexLoc.x, hexLoc.y).riverSource = false;
                                    context.gameModel.GetHexModel(hexLoc.x, hexLoc.y).riverTile=null;
                                    continue;
                                };
                                break;
                            }
                        }
                    }
                    if(isStart){
                        context.gameModel.GetHexModel(hexLoc.x,  hexLoc.y).riverTile=null;
                        Vector3Int nextLeftLoc = context.gameModel.GetNeighbor(new Vector3Int(hexLoc.x,  hexLoc.y, 0), (int)context.gameModel.GetHexModel(hexLoc.x,  hexLoc.y).nextRiverDirection); 
                        //Debug.Log("Start Clear");
                        ClearRiverPlaceHolders(nextLeftLoc.x, nextLeftLoc.y, context, visitedWaters);
                    }
                }
            }
        }
    }
    
    

    private bool SpawnNextLeftRiver(int x, int y, GameActivity context, HashSet<int> visitedWaters, int  destination){
        
        Vector3Int[] neighborLocs = context.gameModel.GetNeighbors(new Vector3Int(x, y, 0));
        int randOffset = randomGen.Range(0, neighborLocs.Length);
        context.gameModel.GetHexModel(x, y).riverTile=RiverTile.Left;

        int destinationNeighborDir = context.gameModel.GetIfHasNeighborWithTerrainBodyType(x,y,destination);
        if(destinationNeighborDir!=-1){
            if(CanFitRightSide(x,y,destinationNeighborDir, context, visitedWaters)){
                FillRightSide(x, y, destinationNeighborDir, context);
                bool[] rightNeighbors = context.gameModel.GetNeighborsWithRiverTileType(x,y,RiverTile.Right);
                context.gameModel.GetHexModel(x,y).AddRiverNeighbors(rightNeighbors);
                return true;
            }
        }
        if(context.gameModel.GetHexModel(x, y).riverNeighbors!=null){
            //Debug.Log("yarse4");

            if(context.gameModel.GetHexModel(x, y).visitedWaters!=null){
                var temp = visitedWaters;
                temp.IntersectWith(context.gameModel.GetHexModel(x, y).visitedWaters);
                if(temp.Count!=0) return false;
            }
            for(int j = 0;j<Direction.LENGTH;j++){
                if(context.gameModel.GetHexModel(x, y).riverNeighbors[j]){
                    if(CanFitRightSide(x,y,j,context, visitedWaters)){
                        FillRightSide(x, y, j, context);
                        bool[] rightNeighbors = context.gameModel.GetNeighborsWithRiverTileType(x,y,RiverTile.Right);
                        context.gameModel.GetHexModel(x,y).AddRiverNeighbors(rightNeighbors);
                        return true;
                    }else{
                        return false;
                    }
                }
            }
        }
        for(int i = randOffset;i<neighborLocs.Length+randOffset;i++){
            int index = i%neighborLocs.Length;
            Vector3Int neighborLoc = neighborLocs[index];
            HexModel neighborHex = context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y);
            if(neighborHex==null) continue;
            if(IsRiverTraversible(neighborHex, visitedWaters)&&neighborHex.terrainBodyType!=Ocean&&neighborHex.riverTile!=RiverTile.Right&&neighborHex.riverTile!=RiverTile.Source&&neighborHex.riverTile!=RiverTile.Left&&!neighborHex.riverSource&&CanFitRightSide(x, y, index, context, visitedWaters)){
                context.gameModel.GetHexModel(x, y).nextRiverDirection = index;
                context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y).previousRiverDirection = Direction.OppositeDirection(index);
                FillRightSide(x, y, index, context);
                //populate riverNeighbors
                bool[] rightNeighbors = context.gameModel.GetNeighborsWithRiverTileType(x,y,RiverTile.Right);
                context.gameModel.GetHexModel(x,y).AddRiverNeighbors(rightNeighbors);
                //("yarse2");
                HexModel curHex = context.gameModel.GetHexModel(x, y);
                Vector3Int prevLoc = context.gameModel.GetNeighbor(new Vector3Int(x,y,0), (int)curHex.previousRiverDirection);
                HexModel prevHex = context.gameModel.GetHexModel(prevLoc.x, prevLoc.y);
                HexModel nextHex = context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y);
                if(prevHex.terrainTile==Water&&curHex.terrainTile!=Water){
                    visitedWaters.Add((int)prevHex.terrainChunk);
                }
                if(!SpawnNextLeftRiver(neighborLoc.x, neighborLoc.y,  context, visitedWaters, destination)){
                    EmptyRightSide(x,y, index, context);
                    context.gameModel.GetHexModel(x, y).nextRiverDirection = null;
                    context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y).previousRiverDirection = null;
                    
                    
                    //Debug.Log("Emptying L:");
                    context.gameModel.GetHexModel(x, y).riverTile=null;
                    context.gameModel.GetHexModel(x,y).RemoveRiverNeighbors(rightNeighbors);
                    return false;
                }
                return true;
            }
        }
        //context.gameModel.GetHexModel(x, y).riverTile=null;
        return false;
    } 

    private void ClearRiverPlaceHolders(int x, int y, GameActivity context, HashSet<int> visitedWaters ){
        int currentX = x;
        int currentY = y;
        int? nextLeftDir = context.gameModel.GetHexModel(currentX, currentY).nextRiverDirection;
        int lastNextLeftDir=0;
        Vector3Int nextLeftLoc;
        while(nextLeftDir!=null&&!context.gameModel.GetHexModel(currentX, currentY).riverSource){
            
            //Debug.Log("yarse5" + "x " + currentX + "y " + currentY + "next left ");
//Debug.Log("Location " +x+" "+y+"            "+nextLeftDir);
            EmptyRightSideTiles(currentX, currentY, (int) nextLeftDir, context, visitedWaters);

            if(context.gameModel.GetHexModel(currentX, currentY).riverTile==null) return;
            context.gameModel.GetHexModel(currentX, currentY).riverTile = null;
            context.gameModel.GetHexModel(currentX, currentY).visitedWaters = visitedWaters;

            nextLeftLoc  = context.gameModel.GetNeighbor(new Vector3Int(currentX, currentY, 0), (int) nextLeftDir);
            currentX = nextLeftLoc.x;
            currentY = nextLeftLoc.y; 
            nextLeftDir = context.gameModel.GetHexModel(currentX, currentY).nextRiverDirection;
            if(context.gameModel.GetHexModel(currentX, currentY).previousRiverDirection==null){
                context.gameModel.GetHexModel(currentX, currentY).previousRiverDirection=Direction.OppositeDirection(lastNextLeftDir);
            }
            if(nextLeftDir!=null){
                lastNextLeftDir=(int)nextLeftDir;
            }
        }
        context.gameModel.GetHexModel(currentX, currentY).riverTile = null;
        EmptyRightSideTiles(currentX, currentY, (int)context.gameModel.GetHexModel(currentX, currentY).previousRiverDirection, context, visitedWaters);

       // Debug.Log("yarse8");
    }

    

    public bool CanFitRightSide(int currentX, int currentY, int nextDirection, GameActivity context, HashSet<int> visitedWaters ){
        HexModel currentHex = context.gameModel.GetHexModel(currentX, currentY);
        int prevDir = (int)currentHex.previousRiverDirection;

        if(prevDir+1>nextDirection) nextDirection+=Direction.LENGTH;
        bool forLoopRan = false;
        bool waterVisited = false;
        int lastWaterChunkVisited = -1;
        for(int i = (int)prevDir+1;i<nextDirection;i++){
            //Debug.Log(i);
            forLoopRan = true;
            int index = i%Direction.LENGTH;
            Vector3Int hexToCheckLoc = context.gameModel.GetNeighbor(new Vector3Int(currentX, currentY, 0), index);
            HexModel hexToCheck = context.gameModel.GetHexModel(hexToCheckLoc.x, hexToCheckLoc.y);
            
            if(hexToCheck==null||!IsRiverTraversible(hexToCheck, visitedWaters)||hexToCheck.riverTile==RiverTile.Left||hexToCheck.GetNumberOfRiverNeighbors()>3){
                return false;
            }
            if(hexToCheck.terrainTile==Water){
                if(visitedWaters.Contains((int)hexToCheck.terrainChunk)){
                    return false;
                }else{
                    lastWaterChunkVisited=(int)hexToCheck.terrainChunk;
                    waterVisited=true;
                }
            }else{
                if(waterVisited){
                    visitedWaters.Add(lastWaterChunkVisited);
                    waterVisited=false;
                }
            }
        }
        return forLoopRan;
    }

    public void FillRightSide(int currentX, int currentY, int nextDirection, GameActivity context){
        HexModel currentHex = context.gameModel.GetHexModel(currentX, currentY);
        bool[] rightNeighbors = new bool[Direction.LENGTH];
        int backDirection=(int)currentHex.previousRiverDirection;

        if(backDirection+1>nextDirection) nextDirection+=Direction.LENGTH;
        for(int i = (int)backDirection+1;i<nextDirection;i++){
            int index = i%Direction.LENGTH;
            Vector3Int neighborLoc = context.gameModel.GetNeighbor(new Vector3Int(currentX, currentY, 0), index);
            context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y).riverTile=RiverTile.Right;
            bool[] newLeftNeighbor = new bool[Direction.LENGTH];
            newLeftNeighbor[Direction.OppositeDirection(index)] = true;
            context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y).AddRiverNeighbors(newLeftNeighbor);
        }
    }

    public void EmptyRightSide(int currentX, int currentY, int nextDirection, GameActivity context){
        HexModel currentHex = context.gameModel.GetHexModel(currentX, currentY);
        int backDirection=(int)currentHex.previousRiverDirection;

        if(backDirection+1>nextDirection) nextDirection+=Direction.LENGTH;
        for(int i = backDirection+1;i<nextDirection;i++){
            int index = i%Direction.LENGTH;
            Vector3Int neighborLoc = context.gameModel.GetNeighbor(new Vector3Int(currentX, currentY, 0), index);
            context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y).riverTile=null;
            bool[] newLeftNeighbor = new bool[Direction.LENGTH];
            newLeftNeighbor[Direction.OppositeDirection(index)] = true;
            context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y).RemoveRiverNeighbors(newLeftNeighbor);
        }
    }

    public void EmptyRightSideTiles(int currentX, int currentY, int nextDirection, GameActivity context, HashSet<int> visitedWaters){
        HexModel currentHex = context.gameModel.GetHexModel(currentX, currentY);
        if(currentHex.previousRiverDirection+1>nextDirection) nextDirection+=Direction.LENGTH;
        for(int i = (int)currentHex.previousRiverDirection+1;i<nextDirection;i++){
            int index = i%Direction.LENGTH;
            Vector3Int neighborLoc = context.gameModel.GetNeighbor(new Vector3Int(currentX, currentY, 0), index);
            context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y).riverTile=null;
            context.gameModel.GetHexModel(neighborLoc.x, neighborLoc.y).visitedWaters = visitedWaters;
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
        //Debug.Log(minCOL + ", " + maxCOL);
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
