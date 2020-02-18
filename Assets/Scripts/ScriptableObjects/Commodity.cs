using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu]
public partial class Commodity : ScriptableObject, IComparable<Commodity>
{
    //todo: food, fuel, tools and weapon class. Or just use fields

    public new string name;
    public  string plural;
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

    public string AsText(int amount)
    {
        if (plural == "")
            plural = name + "'s";

        switch (amount)
        {
            case 0:
                return "no " + plural;
            case 1:
                return "aeiouAEIOU".IndexOf(name) >= 0 ? "an " : "a "  + name;
            default:
                return amount.ToString("N0") +" "+ plural;
        }
    }
}