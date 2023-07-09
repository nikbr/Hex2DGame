using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITroop
{
    int id{
        get;
    }

    int race{
        get;
    }

    int maxSize{
        get;
    }

    int currentSize{
        get;
    }

    List<IItem> inventory{
        get;
    }

    Price price{
        get;
    }
}