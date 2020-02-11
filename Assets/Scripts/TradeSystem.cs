using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class TradeSystem : MonoBehaviour
{
    //TODO: singleton

    public ArtWork CurrentArt;
    //TODO: use or remove
    private readonly List<Commodity> AddOns = new List<Commodity>();
    public Dictionary<Commodity,int> CurrentOffer = new Dictionary<Commodity, int>();
    private int CurrentSellValue;
    public int Patience;
    public Community Buyer { private set;  get; }
    public ArtWork.Property WantedProperty;
    public ArtWork.Color WantedColor;
    public UnityEvent OnUpdate;
    //maybe just parse to update event
    public string CurrentComment;
    [HideInInspector]
    public TradeState CurrentTradeState = TradeState.NoCustomer;
    public enum TradeState { NeedsArt, Negotiating, Success, Collapse,NoCustomer}
    private int NoCustomerDelayCount = 0;
    private Queue<int> NoCustomerCountups =  new Queue<int>(new int[] { 4, 0, 3, 5, 5 }) ;



    private void Start()
    {
        Player.OnDayChange.AddListener(CustomerChance);

        //TEST for customerchance
        //foreach(var loc in FindObjectsOfType<LocationObject>().Where(l => !l.Location.Community))
        //{
        //    Player.Visibility = loc.Location.Visibility;
        //    var cust = 0f;
        //    for(int i = 0; i < 1000; i++)
        //    {
        //        if (CustomerToday()) cust++;
        //    }
        //    Debug.Log($"{loc.Location} ({Player.Visibility} visibility) has {cust/1000f} chance for customers");
        //}
    }

    private void CustomerChance()
    {
        if (CustomerToday())
        {
            SelectCustomer();
        }
        else
        {
            CurrentTradeState = TradeState.NoCustomer;
        }
    }

    private bool CustomerToday()
    {

        if (NoCustomerDelayCount < Player.Visibility)
        {
            var count = NoCustomerCountups.Dequeue();
            NoCustomerDelayCount += count;
            NoCustomerCountups.Enqueue(count);
            return true;
        }
        else
        {
            NoCustomerDelayCount = Mathf.Max(0, NoCustomerDelayCount - Player.Visibility);

            return false;
        }
    }
    
    public void SelectCustomer()
    {
        Buyer = Database.Instance.AllCommunities[Random.Range(0, Database.Instance.AllCommunities.Length)];

        //TODO: should happen automatically
        Buyer.SetupInventory();

        //TODO: maybe just one attribute

        WantedProperty = Buyer.FavoriteProperties[Random.Range(0, Buyer.FavoriteProperties.Length)];
        WantedColor = Buyer.FavoriteColors[Random.Range(0, Buyer.FavoriteColors.Length)];

        CurrentComment = $"I want something {WantedColor} and {WantedProperty}!";

        CurrentTradeState = TradeState.NeedsArt;

        OnUpdate.Invoke();
    }

    public void SelectArt(ArtWork art)
    {
        CurrentArt = art;
        StartTrade();
    }

    private void StartTrade()
    {
        int artAptitude = 0;
        if (CurrentArt.Colors.Contains(WantedColor)) artAptitude++;
        if (CurrentArt.Properties.Contains(WantedProperty)) artAptitude++;

        switch (artAptitude)
        {
            case 0:
                Patience = Buyer.Attitude /2;

                CurrentComment = "That's not really what I wanted. I guess I can give you this";
                break;
            case 1:
                Patience = Buyer.Attitude;

                CurrentComment = "Alright. this is what I can give you for that";
                break;
            case 2:
                Patience = Buyer.Attitude +5;

                CurrentComment = "That's perfect! I can give you this";
                break;
        }

        CurrentOffer = Buyer.GetResourcesOfValue(Random.Range(CurrentArt.Value / 5, CurrentArt.Value+Patience));

        CurrentTradeState = TradeState.Negotiating;

        OnUpdate.Invoke();
    }

    //TODO: are the commodities checked against inventory first?
    public void CounterOffer(Dictionary<Commodity, int> additionals) //Dictionary<Commodity,int> addons, 
    {
        int additionalsValue = additionals.Sum(a => a.Key.Value * a.Value);

        if (additionalsValue < Patience)
        {
            foreach (var a in additionals)
                CurrentOffer[a.Key] = a.Value + (CurrentOffer.ContainsKey(a.Key) ? CurrentOffer[a.Key] : 0);

            TradeSuccess();
        }
        else if (additionalsValue < Patience * 2)
        {

            //will suggest less
            if (additionals.Sum(c => c.Value) > 1)
            {
                CurrentComment = "What about this?";

                var extra = additionals.First(a => a.Value > 0);

                CurrentOffer[extra.Key] = ((extra.Value > 1) ? extra.Value / 2 : 1) + ((CurrentOffer.ContainsKey(extra.Key) ? CurrentOffer[extra.Key] : 0));

                Patience /= 2;
            }
            else
            {
                CurrentComment = "nah..";

                Patience--;
            }
        }
        else if (additionalsValue < Patience * 4)
        {
            CurrentComment = "That's too much.";

            Patience--;
        }
        else
        {
            CurrentComment = "No! I'm outta here. See ya!";
            TradeFailure(); 
        }


        OnUpdate.Invoke();
    }

    public void AcceptOffer()
    {
        TradeSuccess();
    }

    public void DeclineOffer()
    {
        //TODO: chance for them to up their offer, if they really want it

        CurrentComment = "alright. See ya!";
        TradeFailure();
    }

    //todo: use events
    private void TradeSuccess()
    {
        Buyer.Attitude++;
        CurrentComment = "That's a deal!";

        Player.ArtWorks.Remove(CurrentArt);

        Player.Food += CurrentOffer.Sum(c => c.Key.FoodValue * c.Value);
        Player.Security += CurrentOffer.Sum(c => c.Key.SecurityValue * c.Value);

        //TODO: remove items from inventory

        Debug.Log($"{CurrentArt} sold for {CommoditiesAsString(CurrentOffer)}");

        CurrentTradeState = TradeState.Success;

        OnUpdate.Invoke();
    }

    private void TradeFailure()
    {

        Debug.Log($"{CurrentArt} not sold. Last offer: {CommoditiesAsString(CurrentOffer)}");

        CurrentTradeState = TradeState.Collapse;

        OnUpdate.Invoke();
    }

    private void Clear()
    {
        CurrentOffer.Clear();
        AddOns.Clear();

    }

    public static string CommoditiesAsString(Dictionary<Commodity, int> commodities)
    {
        if (!commodities.Any()) return "nothing";

        return commodities.Where(c=> c.Value > 0). Select(c => $"{c.Value} {c.Key.name}'s").Aggregate((current, next) => current + " and " + next);
    }

}
