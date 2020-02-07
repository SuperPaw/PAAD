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
    public Button AcceptTradeButton, CounterOfferButton, NextCustomerButton;
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
        NextCustomerButton.onClick.AddListener(NewCustomer);
        AcceptTradeButton.onClick.AddListener(TradeSystem.AcceptOffer);
        NewCustomer();
    }


    //TODO: SHould be handled by game manager
    public void NewCustomer()
    {
        Player.Food--;

        TradeSystem.SelectCustomer();

        CharacterImage.sprite = TradeSystem.Buyer.LeaderImage;

        TradeSystem.Buyer.SetupInventory();
    }
    
    void UpdateUI()
    {
        CommentText.text = TradeSystem.CurrentComment;

        //Enabling/disabling based on trade state
        SelectItemMenu.SetActive(TradeSystem.CurrentState == TradeSystem.TradeState.NeedsArt);
        AcceptTradeButton.gameObject.SetActive( TradeSystem.CurrentState == TradeSystem.TradeState.Negotiating);
        CounterOfferButton.gameObject.SetActive( TradeSystem.CurrentState == TradeSystem.TradeState.Negotiating);
        NextCustomerButton.gameObject.SetActive( TradeSystem.CurrentState == TradeSystem.TradeState.Success ||TradeSystem.CurrentState == TradeSystem.TradeState.Collapse);

        TradeText.text = $"Trading {TradeSystem.CurrentArt.name} with {TradeSystem.Buyer.LeaderName} from the {TradeSystem.Buyer.name}";
        PatienceText.text = $"Patience; {TradeSystem.Patience}";
        CurrentOfferText.text = $"Current Offer: {TradeSystem.CommoditiesAsString(TradeSystem.CurrentOffer)}";
    
    }

}
