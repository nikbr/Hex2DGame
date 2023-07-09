using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    int id{
        get;
    }

    Price price{
        set;
        get;
    }

    int weight{
        get;
    }



}
