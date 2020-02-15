using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class Community : ScriptableObject
{
    public enum LeaderTrait
    {
        Economical, //cheapskate
        Extravagant, //opposite
        Nostalgic, //like analogue things
        Modernist, //tech-lover
        Decadent,
        Practical,
        Narcissist,
        Selfless,
        Virtuous,
        Wicked,
        Violent,
        Pacifist // Anarchist?
    }

    [SerializeField]
    private int StartAttitude = 5;
    [SerializeField]
    private int StartWealth;

    public new string name;
    public string LeaderName;
    public Sprite LeaderImage;
    public Sprite CommunityIcon;
    public bool Producers;
    public bool Scavengers;
    public bool Raiders;
    public LeaderTrait[] LeaderTraits;
    public Commodity[] PrimaryResources;
    public ArtWork.Property[] FavoriteProperties;
    public ArtWork.Color[] FavoriteColors;
    [HideInInspector]
    public Dictionary<Commodity, int> Inventory = new Dictionary<Commodity, int>();
    [HideInInspector]
    public int Attitude;

    public int GetResourceAmount(string name) => Inventory.First(r => r.Key.name == name).Value;

    public int TotalValue() => Inventory.Sum(x => x.Key.Value);


    //TODO: use this when the game starts
    public void SetupInventory()
    {

        //TODO: use wealth to determine how much stuff they have
        foreach (var c in PrimaryResources)
        {
            Inventory[c] = Random.Range(2, 20);
        }

        Attitude = StartAttitude;
    }

    /// <summary>
    /// Non-deterministically returns an amount of resources equal to or lower in value than specified.
    /// </summary>
    public Dictionary<Commodity, int> GetResourcesOfValue(int value)
    {
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

        return resources;
    }

    public void RemoveCommodities(Dictionary<Commodity, int> commodities)
    {
        foreach (var c in commodities)
            Inventory[c.Key] -= c.Value;
    }
}
