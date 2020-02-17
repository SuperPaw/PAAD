using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using System;
using Random = UnityEngine.Random;

public class TradeSystem : MonoBehaviour
{
    //TODO: singleton

    public ArtWork CurrentArt;
    //TODO: use or remove
    private readonly List<Commodity> AddOns = new List<Commodity>();
    public CommodityGroup CurrentOffer = new CommodityGroup();
    [HideInInspector]
    public bool HasMadeSalesPitch;
    public int Patience;
    public Community Buyer { private set;  get; }
    public ArtWork.Property WantedProperty;
    public ArtWork.Color WantedColor;
    public UnityEvent OnUpdate;
    //maybe just parse to update event
    public string CurrentComment;
    [HideInInspector]
    public TradeState CurrentTradeState = TradeState.NoCustomer;
    public enum TradeState { NeedsArt, Negotiating, Success, Collapse,ShareRumour,NoCustomer}
    private int NoCustomerDelayCount = 0;
    private Queue<int> NoCustomerCountups =  new Queue<int>(new int[] { 4, 0, 3, 5, 5 }) ;

    public SaleArgument[] SaleArguments;

    [Serializable]
    public struct SaleArgument
    {
        public ArgumentType Type;
        public string Text;
        public Community.LeaderTrait[] LikesArgument;
        public Community.LeaderTrait[] DislikesArgument;
    }
    public enum ArgumentType { Cheap, Valuable, Antique, Modern, Beautiful, Poetic, Historical, Anarchistic, Charm}


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

        CurrentComment = $"I want something <b>{WantedColor}</b> and <b>{WantedProperty}</b>!";

        CurrentTradeState = TradeState.NeedsArt;

        HasMadeSalesPitch = false;

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

        CurrentOffer = Buyer.Inventory.GetResourcesOfValue(Random.Range(CurrentArt.Value / 5, CurrentArt.Value+Patience));

        CurrentTradeState = TradeState.Negotiating;

        OnUpdate.Invoke();
    }

    //TODO: are the commodities checked against inventory first?
    public void CounterOffer(CommodityGroup additionals) //Dictionary<Commodity,int> addons, 
    {
        int additionalsValue = additionals.GetValue();

        if (additionalsValue < Patience)
        {
            CurrentOffer.Add(additionals);

            TradeSuccess();
        }
        else if (additionalsValue < Patience * 2)
        {

            //will suggest less
            if (additionals.GetAmount() > 1)
            {
                CurrentComment = "What about this?";

                CurrentOffer.Add(additionals.GetResourcesOfValue(additionals.GetValue() - 1));

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

        Player.Food += CurrentOffer.GetFoodValue();
        Player.Security += CurrentOffer.GetSecuritValue();

        //TODO: remove items from inventory

        Debug.Log($"{CurrentArt} sold for {CurrentOffer.AsText()}");

        CurrentTradeState = TradeState.Success;

        OnUpdate.Invoke();
    }

    private void TradeFailure()
    {

        Debug.Log($"{CurrentArt} not sold. Last offer: {CurrentOffer.AsText()}");

        CurrentTradeState = TradeState.Collapse;

        OnUpdate.Invoke();
    }

    private void Clear()
    {
        CurrentOffer.Clear();
        AddOns.Clear();

    }

    public string MakeSaleArgument(ArgumentType argumentType)
    {
        var arg = SaleArguments.First(a => a.Type == argumentType);

        HasMadeSalesPitch = true;

        if(arg.LikesArgument.Any( Buyer.LeaderTraits.Contains))
        {
            CurrentComment = $"I like {argumentType} stuff. This is my new offer.";
            //TODO: test that it is not possible to get lower amount
            CurrentOffer = Buyer.Inventory.GetResourcesOfValue(CurrentOffer.GetValue() * 2);
        }
        else if (arg.DislikesArgument.Any(Buyer.LeaderTraits.Contains))
        {
            CurrentComment = $"Why would I like {argumentType} stuff!? You Moron!";

            Patience /= 2;
        }
        else 
        {
            CurrentComment = $"So...";

            Patience--;
        }

        OnUpdate.Invoke();

        //TODO: actually use this in player speech bubble
        return arg.Text;
    }
    
}
