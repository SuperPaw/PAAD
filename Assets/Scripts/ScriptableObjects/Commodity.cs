using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu]
public class Commodity : ScriptableObject, IComparable<Commodity>
{
    //todo: food, fuel, tools and weapon class. Or just use fields

    public new string name;
    //TODO: create plural version of name
    public Sprite Sprite;
    public int InitialValue = 10;
    [HideInInspector]
    public int Value;
    public int FoodValue;
    public int SecurityValue;
    public bool Producable;
    public bool Scavengable;

    private void Awake()
    {
        Value = InitialValue;//Random.Range(InitialValue / 5, InitialValue * 2);
        //TODO: use commodity non-scriptable object
    }

    public int CompareTo(Commodity other)
    {
        return this.Value.CompareTo(other.Value);
    }
}
