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
    public CommodityGroup Inventory = new CommodityGroup();
    [HideInInspector]
    public int Attitude;




    //TODO: use this when the game starts
    public void SetupInventory()
    {

        //TODO: use wealth to determine how much stuff they have
        foreach (var c in PrimaryResources)
        {
            Inventory.Add(c, Random.Range(2, 20));
        }

        Attitude = StartAttitude;
    }



    public void RemoveCommodities(CommodityGroup commodities)
    {
        Inventory.Substract(commodities);
    }
}
