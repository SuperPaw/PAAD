using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Group<T>
{
    public Dictionary<T, int> Commodities;

    public Group(Dictionary<T,int> c) => Commodities = c;

    //Use this to 

    //TODO: create unit test of this

    public static Group<T> operator +(Group<T> x, Group<T> y)
    {
        foreach(var c in y.Commodities)
        {
            x.Commodities[c.Key] = c.Value + (x.Commodities.ContainsKey(c.Key) ? x.Commodities[c.Key] : 0);
        }
        return x;
    }
    public static Group<T> operator -(Group<T> x, Group<T> y)
    {
        foreach (var c in y.Commodities)
        {
            x.Commodities[c.Key] = - c.Value + (x.Commodities.ContainsKey(c.Key) ? x.Commodities[c.Key] : 0);

            if (x.Commodities[c.Key] <= 0)
                x.Commodities.Remove(c.Key);
        }

        return x;
    }
}
