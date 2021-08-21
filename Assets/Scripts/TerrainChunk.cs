using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk
{
    public readonly int CHUNK_ID;
    public readonly int TERRAIN_TYPE;
    private List<Vector3Int> hexesInChunk;
    public TerrainChunk(int id, int terrain){
        CHUNK_ID = id;
        TERRAIN_TYPE = terrain;
        hexesInChunk = new List<Vector3Int>();
    }

    public void Insert(HexModel hex){
        if(TERRAIN_TYPE == hex.terrainTile&&CHUNK_ID == hex.terrainChunk){
            hexesInChunk.Add(new Vector3Int(hex.COL, hex.ROW, 0));
        }else{
            Debug.Log("Could insert into chunk. Check terrain type and chunk ID.");
        }
    }

    public int Size(){
        return hexesInChunk.Count;
    }
}
