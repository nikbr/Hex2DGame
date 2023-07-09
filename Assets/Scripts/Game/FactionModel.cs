using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FactionModel
{
    int id;
    Vector3Int color;
    int flag;
    HashSet<int> allies;
    HashSet<int> enemies;
    HashSet<int> parties;
    int wood=0;
    int ore=0;
    int gold=0;
    int supply=0;
    int gems=0;
    int mercury=0;
    int sulfur=0;
    bool AI = true;
    int turnsDone = 0;
    bool alive = true;
}
