using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WarbandModel : IParty
{
  public int id{
        get;
    }

    public int factionId{
        get;
        set;
    }

    public int turnsAlive{
        get;
    }

   public int turnsDone{
        get;
        set;
    }

   public ITroop[] troops{
        get;
    }

    public bool AI{
        get;
        set;
    }

    public WarbandModel(int id, int factionId, GameActivity ga){
        this.id = id;
        turnsDone = ga.currentTurn-1;
        AI=true;
        this.factionId = factionId;
    }

   public void ExecuteTurn(GameActivity ga){
        if(turnsDone<ga.currentTurn){
                if(AI){

                    }else{
                        
                    }
                Debug.Log("Warband " + id + " of Faction " + factionId + " is executing their turn"); 
                    turnsDone++;
                    ga.objectsThatFinishedTurn++;
            }
        }
   }
 
