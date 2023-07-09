using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IParty
{
    int id{
        get;
    }

    int factionId{
        get;
        set;
    }

    int turnsAlive{
        get;
    }

    int turnsDone{
        get;
    }

    ITroop[] troops{
        get;
    }

    bool AI{
        get;
    }

    void ExecuteTurn(GameActivity gameActivity);

}
