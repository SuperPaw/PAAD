using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeUI : MonoBehaviour
{
    public TradeSystem TradeSystem;
    public CounterOffer CounterOffer;
    public Text TradeText, PatienceText, CurrentOfferText;
    public Button AcceptTradeButton, CounterOfferButton, NextDay;
    public GameObject SelectItemMenu;
    public TextMeshProUGUI CommentText;
    public Image CharacterImage;

    public void OpenCounterOffer()
    {
        Debug.Log("Counter offer...");
        CounterOffer.Setup(TradeSystem.Buyer, TradeSystem.CurrentOffer);
    }

    private void Start()
    {
        TradeSystem.OnUpdate.AddListener(UpdateUI);
        NextDay.onClick.AddListener(Player.NextDay);
        Player.OnDayChange.AddListener(UpdateUI);
        AcceptTradeButton.onClick.AddListener(TradeSystem.AcceptOffer);
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
            CurrentOfferText.text = $"Current Offer: {TradeSystem.CommoditiesAsString(TradeSystem.CurrentOffer)}";

        }
        else
        {

            TradeText.text = "No customer today";
            PatienceText.text = CurrentOfferText.text = "";
        }

        CharacterImage.gameObject.SetActive(TradeSystem.CurrentTradeState != TradeSystem.TradeState.NoCustomer);

        //Enabling/disabling based on trade state
        SelectItemMenu.SetActive(TradeSystem.CurrentTradeState == TradeSystem.TradeState.NeedsArt);
        AcceptTradeButton.gameObject.SetActive( TradeSystem.CurrentTradeState == TradeSystem.TradeState.Negotiating);
        CounterOfferButton.gameObject.SetActive( TradeSystem.CurrentTradeState == TradeSystem.TradeState.Negotiating);
        NextDay.gameObject.SetActive( TradeSystem.CurrentTradeState == TradeSystem.TradeState.Success ||TradeSystem.CurrentTradeState == TradeSystem.TradeState.Collapse || TradeSystem.CurrentTradeState == TradeSystem.TradeState.NoCustomer);

    
    }

}
