using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommodityGroup : IEnumerable<KeyValuePair<Commodity,int>>
{
    private Dictionary<Commodity, int> Inventory = new Dictionary<Commodity, int>();

    public CommodityGroup(Dictionary<Commodity, int> initialInventory)
    {
        this.Inventory = initialInventory;
    }

    public CommodityGroup()
    {
    }

    public void Add(CommodityGroup commodityGroup)
    {
        foreach (var a in commodityGroup.Inventory)
            Add(a.Key, a.Value);
    }

    public void Add(Commodity c, int amount)
    {
        Inventory[c] = amount + (Inventory.ContainsKey(c) ? Inventory[c] : 0);
    }

    public void Substract(CommodityGroup commodityGroup)
    {
        foreach (var a in commodityGroup.Inventory)
        {
            //TODO: should it go to negative?
            if (Inventory.ContainsKey(a.Key))
                Inventory[a.Key] -= a.Value;
            else
                Debug.LogWarning("Trying to subtract a commodity below zero: " + a.Key.name);
        }
    }

    public int GetValue()
    {
        return Inventory.Sum(a => a.Key.Value * a.Value);
    }
    public int GetAmount()
    {
        return Inventory.Sum(c => c.Value);
    }

    public int GetResourceAmount(Commodity c) => Inventory.ContainsKey(c)? Inventory[c] : 0;
    public int GetResourceValue(Commodity c) => Inventory.ContainsKey(c) ? Inventory[c] *c.Value: 0;

    /// <summary>
    /// Non-deterministically returns an amount of resources equal to or lower in value than specified.
    /// </summary>
    public CommodityGroup GetResourcesOfValue(int value)
    {
        //TODO: should be shorter and smarter

        var valuables = Inventory.Where(kv => kv.Key.Value * kv.Value > value / 2);

        Commodity primaryResource;
        if (valuables.Any())
            primaryResource = valuables.ToArray()[Random.Range(0, valuables.Count())].Key;
        else
            primaryResource = Inventory.ToArray()[Random.Range(0, Inventory.Count)].Key;

        var resources = new Dictionary<Commodity, int>();

        int amount = Mathf.Min(value / primaryResource.Value, Inventory[primaryResource]);

        if (amount < 1) amount = 1;

        value -= amount * primaryResource.Value;

        resources[primaryResource] = amount;

        var minRes = Inventory.Keys.Except(resources.Keys).Min(c => c);

        if (value > minRes.Value)
        {
            amount = Mathf.Min(value / minRes.Value, Inventory[minRes]);

            value -= amount * minRes.Value;

            resources[minRes] = amount;
        }

        return new CommodityGroup(resources);
    }


    public string AsText()
    {
        if (Inventory.Count() == 0) return "nothing";

        return Inventory.Where(c => c.Value > 0).Select(c => c.Key.AsText(c.Value)).Aggregate((current, next) => current + " and " + next);
    }

    internal void Clear()
    {
        Inventory.Clear();
    }

    public int GetFoodValue()=> Inventory.Sum(c => c.Key.FoodValue* c.Value);
    public int GetSecurityValue()=> Inventory.Sum(c => c.Key.SecurityValue* c.Value);

    internal object First() => Inventory.First(a => a.Value > 0);

    //Returns true if they contain the same keys and values
    public bool Equals(CommodityGroup other)
    {
        return Inventory.Count == other.Inventory.Count && !Inventory.Except(other.Inventory).Any();
    }


    IEnumerator<KeyValuePair<Commodity, int>> IEnumerable<KeyValuePair<Commodity, int>>.GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<Commodity, int>>)Inventory).GetEnumerator();
    }

    public IEnumerator GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<Commodity, int>>)Inventory).GetEnumerator();
    }
}