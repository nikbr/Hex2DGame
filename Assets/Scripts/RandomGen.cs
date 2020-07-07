using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface RandomGenerator<T> {
    T Range(T low, T high);
}

public class UnityRandom : RandomGenerator<int> {
    int seed;
    UnityRandom(int seed) {
        this.seed = seed;
        UnityEngine.Random.InitState(seed);
    }

    public int Range(int low, int high) {
        return (int)UnityEngine.Random.Range((float)low, (float)high);
    }
}

public class SystemRandom : RandomGenerator<int> {
    System.Random r;
    SystemRandom() {
        r = new System.Random();
    }

    public int Range(int low, int high) {
        return r.Next(low, high);
    }
}