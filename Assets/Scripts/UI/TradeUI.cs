using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeUI : MonoBehaviour
{
    public TradeSystem TradeSystem;
    public CounterOffer CounterOffer;
    public Text TradeText, PatienceText, CurrentOfferText;
    public Button AcceptTradeButton, CounterOfferButton, NextDay,DenyTradeButton,SellingArgumentButton, RumourButton;
    public GameObject SelectItemMenu;
    public TextMeshProUGUI CommentText;
    public Image CharacterImage;
    private List<GameObject> InstantiatedArguments = new List<GameObject>();
    public Button ArgumentInstance;
    [SerializeField]
    private bool AskedForRumour;

    public Image ArtImage;

    public void OpenCounterOffer()
    {
        CounterOffer.Setup(TradeSystem.Buyer, TradeSystem.CurrentOffer);
    }

    private void Start()
    {
        TradeSystem.OnUpdate.AddListener(UpdateUI);
        NextDay.onClick.AddListener(Player.NextDay);
        Player.OnDayChange.AddListener(UpdateUI);
        AcceptTradeButton.onClick.AddListener(TradeSystem.AcceptOffer);
        DenyTradeButton.onClick.AddListener(TradeSystem.DeclineOffer);
        SellingArgumentButton.onClick.AddListener(SetupArgumentButtons);
        RumourButton.onClick.AddListener(AskForRumour);
        UpdateUI();
    }

    
    void UpdateUI()
    {
        CommentText.text = TradeSystem.CurrentComment;

        if(TradeSystem.CurrentTradeState != TradeSystem.TradeState.NoCustomer)
        {
            CharacterImage.sprite = TradeSystem.Buyer?.LeaderImage;
            TradeText.text = $"Trading {TradeSystem.CurrentArt.name} with {TradeSystem.Buyer.LeaderName} from the {TradeSystem.Buyer.name}";
            PatienceText.text = $"Patience; {TradeSystem.Patience}";
            CurrentOfferText.text = $"Current Offer: {TradeSystem.CurrentOffer.AsText()}";
            ArtImage.sprite = TradeSystem.CurrentArt.Sprite;

        }
        else
        {
            AskedForRumour = false;

            TradeText.text = "No customer today";
            PatienceText.text = CurrentOfferText.text = "";
        }

        if(TradeSystem.CurrentTradeState == TradeSystem.TradeState.NeedsArt)
        {

            TradeText.text = $"What Piece of Art do you offer with {TradeSystem.Buyer.LeaderName} from the {TradeSystem.Buyer.name}?";
        }

        CharacterImage.gameObject.SetActive(TradeSystem.CurrentTradeState != TradeSystem.TradeState.NoCustomer);

        ArtImage.enabled = TradeSystem.CurrentTradeState == TradeSystem.TradeState.Negotiating;

        //Enabling/disabling based on trade state
        //TODO: make a class that listens to Trade update. TradeUpdate sends the new tradestate. Enables/disables button
        SelectItemMenu.SetActive(TradeSystem.CurrentTradeState == TradeSystem.TradeState.NeedsArt);
        AcceptTradeButton.gameObject.SetActive( TradeSystem.CurrentTradeState == TradeSystem.TradeState.Negotiating);
        CounterOfferButton.gameObject.SetActive( TradeSystem.CurrentTradeState == TradeSystem.TradeState.Negotiating);
        DenyTradeButton.gameObject.SetActive(TradeSystem.CurrentTradeState == TradeSystem.TradeState.Negotiating);
        CurrentOfferText.gameObject.SetActive(TradeSystem.CurrentTradeState == TradeSystem.TradeState.Negotiating);
        RumourButton.gameObject.SetActive(TradeSystem.CurrentTradeState != TradeSystem.TradeState.NoCustomer &! AskedForRumour);

        SellingArgumentButton.gameObject.SetActive(TradeSystem.CurrentTradeState == TradeSystem.TradeState.Negotiating &! TradeSystem.HasMadeSalesPitch);
        NextDay.gameObject.SetActive(TradeSystem.CurrentTradeState == TradeSystem.TradeState.Success || TradeSystem.CurrentTradeState == TradeSystem.TradeState.Collapse || TradeSystem.CurrentTradeState == TradeSystem.TradeState.NoCustomer);
    }

    private void SetupArgumentButtons()
    {
        //TODO: take from art
        foreach(var arg in TradeSystem.SaleArguments.Skip(Random.Range(0,3)).Take(3))
        {
            var o = Instantiate(ArgumentInstance,ArgumentInstance.transform.parent);

            o.GetComponentInChildren<TextMeshProUGUI>().text = arg.Text;

            o.onClick.AddListener(() => MakeArgument(arg.Type));

            o.gameObject.SetActive(true);

            InstantiatedArguments.Add(o.gameObject);
        }
    }

    private void AskForRumour()
    {
        AskedForRumour = true;
        RumourButton.gameObject.SetActive(false);
        CommentText.text = RumourManager.GetRumourSpeech(TradeSystem.Buyer);
    }

    private void MakeArgument(TradeSystem.ArgumentType argumentType)
    {
        TradeSystem.MakeSaleArgument(argumentType);

        foreach (var i in InstantiatedArguments)
            Destroy(i);

        InstantiatedArguments.Clear();
    }
}
