using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HeroModel : ITroop
{

    private readonly int _id;
  public int id
  {
      get => _id;
  }
    private readonly int _race;
  public int race
  {
      get => _race;
  }
  private readonly int _maxSize;
  public int maxSize{
      get => _maxSize;
  }

private int _currentSize;
  public int currentSize{
      get => _currentSize;
  }

private List<IItem> _inventory;
  public List<IItem> inventory{
      get => _inventory;
  }

private Price _price;

  public Price price{
      get => _price;
  }
}
