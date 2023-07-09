using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TownModel : IParty
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

    public TownModel(int id, int factionId, GameActivity ga){
        this.id = id;
        turnsDone = ga.currentTurn-1;
        AI = true;
        this.factionId = factionId;
    }

    public bool AI{
        get;
        set;
    }

   public void ExecuteTurn(GameActivity ga){
       if(turnsDone<ga.currentTurn){
           if(AI){

            }else{
                
            }
            Debug.Log("Town " + id + " of Faction " + factionId + " is executing their turn"); 
            turnsDone++;
            ga.objectsThatFinishedTurn++;
       }
   }
}
