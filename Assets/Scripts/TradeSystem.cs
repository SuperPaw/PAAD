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
    private bool ShakeDown;
    public Community Buyer { private set;  get; }
    public ArtWork.Property WantedProperty;
    public ArtWork.Color WantedColor;
    public UnityEvent OnUpdate;
    public UnityEvent OnNewCustomer;
    //maybe just parse to update event
    public string CurrentComment;
    [HideInInspector]
    public TradeState CurrentTradeState = TradeState.NoCustomer;
    public enum TradeState { NeedsArt, Negotiating, Success, Collapse,ShareRumour,NoCustomer,UnderAttack}
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

        OnNewCustomer.AddListener(SelectCustomer);

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
            OnNewCustomer.Invoke();
        }
        else
        {
            var r = GetRaider();
            if (r)
            {
                RaidController.StartFight(r);
            }
            
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
        SoundController.PlayShopBell();

        Buyer = GetNextBuyer();

        Buyer.LastVisit = Player.Day;

        //TODO: maybe just one attribute

        WantedProperty = Buyer.FavoriteProperties[Random.Range(0, Buyer.FavoriteProperties.Length)];
        WantedColor = Buyer.FavoriteColors[Random.Range(0, Buyer.FavoriteColors.Length)];

        Patience = Buyer.Attitude;

        CurrentComment = $"I want something <b>{WantedColor}</b> and <b>{WantedProperty}</b>!";

        CurrentTradeState = TradeState.NeedsArt;

        HasMadeSalesPitch = false;

        OnUpdate.Invoke();
    }

    private Community GetNextBuyer()
    {
        var cs = Database.Instance.AllCommunities.OrderByDescending(c => c.Attitude
           + (Player.Location && Player.Location.ClosestNeighbours.Contains(c) ? 8 : 0)
           - (c.LastVisit > -1 && c.LastVisit > Player.Day - 5 ? 10 : 0)); //[Random.Range(0, Database.Instance.AllCommunities.Length)];

        return cs.First();
    }
    private Community GetRaider()
    {
        //if() !any return null
        if (!Database.Instance.AllCommunities.Any(c => c.Raiders && c.Attitude <= 0))
            return null;

        //any angry order by last visit
        return Database.Instance.AllCommunities.Where(c => c.Raiders && c.Attitude <= 0).OrderBy(c => c.Attitude).First(); //[Random.Range(0, Database.Instance.AllCommunities.Length)];

    }

    public void SelectArt(ArtWork art)
    {
        CurrentArt = art;

        ShakeDown = Buyer.Attitude <= 10 && Buyer.Raiders;

        if (ShakeDown)
            StartShakeDown();
        else
            StartTrade();
    }

    private void StartShakeDown()
    {
        CurrentOffer = new CommodityGroup();

        CurrentComment = $"That's nice! How about you give it to me and I will make sure nothing bad happens to you.";

        Patience = 3;

        CurrentTradeState = TradeState.Negotiating;

        OnUpdate.Invoke();
    }

    private void StartTrade()
    {
        int artAptitude = 0;
        if (CurrentArt.Colors.Contains(WantedColor)) artAptitude++;
        if (CurrentArt.Properties.Contains(WantedProperty)) artAptitude++;


        CurrentOffer = Buyer.Inventory.GetResourcesOfValue(Random.Range(CurrentArt.Value / 5, CurrentArt.Value+Patience));

        switch (artAptitude)
        {
            case 0:
                Patience = Buyer.Attitude / 2;

                CurrentComment = $"That's not really what I wanted. I can give you <b>{CurrentOffer.AsText()}</b>";
                break;
            case 1:
                Patience = Buyer.Attitude;

                CurrentComment = $"Alright. I can give you <b>{CurrentOffer.AsText()}</b> for that";
                break;
            case 2:
                Patience = Buyer.Attitude + 5;

                CurrentComment = $"That's perfect! I will give you <b>{CurrentOffer.AsText()}</b>";
                break;
        }
        CurrentTradeState = TradeState.Negotiating;

        OnUpdate.Invoke();
    }

    //TODO: are the commodities checked against inventory first?
    public void CounterOffer(CommodityGroup additionals) //Dictionary<Commodity,int> addons, 
    {

        int additionalsValue = additionals.GetValue();

        if (ShakeDown)
        {
            CurrentComment = "it would be a shame if something happened to this nice shop.";

            Patience--;

        }
        else if (additionalsValue < Patience)
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

    internal void ThrowOut()
    {
        Buyer.Attitude -= 5;
        CurrentComment = "Fuck you, idiot!";
        TradeFailure();

    }

    public void AcceptOffer()
    {
        TradeSuccess();
    }

    public void DeclineOffer()
    {
        //TODO: chance for them to up their offer, if they really want it
        

        if(ShakeDown)
        {
            Buyer.Attitude = 0;
            CurrentComment = "You will regret this!";

        }
        else
        {
            Buyer.Attitude--;
            CurrentComment = "alright. See ya!";

        }


        TradeFailure();
    }

    //todo: use events
    private void TradeSuccess()
    {
        if (ShakeDown)
        {
            Buyer.Attitude += CurrentArt.Value;
            CurrentComment = "I knew you would understand.";
        }
        else
        {
            Buyer.Attitude++;
            CurrentComment = "That's a deal!";
        }

        Player.ArtWorks.Remove(CurrentArt);
        SelectionWheel.DestroySaleObject();

        Player.Food += CurrentOffer.GetFoodValue();
        Player.Security += CurrentOffer.GetSecurityValue();

        //TODO: remove items from inventory

        Debug.Log($"You just sold {CurrentArt} for {CurrentOffer.AsText()}!");

        CurrentTradeState = TradeState.Success;

        OnUpdate.Invoke();
    }

    private void TradeFailure()
    {

        Debug.Log($"{CurrentArt} not sold. Last offer: {CurrentOffer.AsText()}");
        SelectionWheel.DestroySaleObject();

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

        if(ShakeDown)
        {
            CurrentComment = "I don't care.";
            Patience--;
        }
        else if(arg.LikesArgument.Any( Buyer.LeaderTraits.Contains))
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
