using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FactionModel
{
    readonly int  id;
  // TODO  Vector3Int color;
    int flag;
    public HashSet<int> allies = new HashSet<int>();
    public HashSet<int> enemies = new HashSet<int>();
    public HashSet<int> parties = new HashSet<int>();
    public HashSet<int> leaders = new HashSet<int>();
    public int wood=0;
    public int ore=0;
    public int gold=0;
    public int gems=0;
    public int mercury=0;
    public int sulfur=0;
    private bool AI = true;
    int turnsDone;
    int woodIncome=0;
    int oreIncome=0;
    int goldIncome=100;
    int gemsIncome=0;
    int mercuryIncome=0;
    int sulfurIncome=0;
    bool alive = true;

    public FactionModel( int id, GameActivity ga){
        this.id = id;
        turnsDone = ga.currentTurn-1;
    }

    public void  ExecuteTurn(GameActivity ga){
        if(turnsDone<ga.currentTurn){

            UpdateResources();

            if(AI){

            }else{
                
            }

            turnsDone++;
            ga.objectsThatFinishedTurn++;
            Debug.Log("Faction " + id.ToString() + " is executing their turn " + turnsDone +" curent turn: " +ga.currentTurn + " objects that finished their move " + ga.objectsThatFinishedTurn);
            
        }
    }

    public void SetToHuman(){
        AI=false;
    }

 

    private void UpdateResources(){
        wood+=woodIncome;
        ore+=oreIncome;
        gold+=goldIncome;
        gems+=gemsIncome;
        mercury+=mercuryIncome;
        sulfur+=sulfurIncome;
    }
}
