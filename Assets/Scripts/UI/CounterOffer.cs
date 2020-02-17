using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterOffer : MonoBehaviour
{
    public Community Buyer;
    public OfferResourceInstance OfferPrefab;
    public GameObject Holder;
    public CommodityGroup OnOffer = new CommodityGroup();
    public List<GameObject> InitializedObjects = new List<GameObject>();

    public void Setup(Community buyer, CommodityGroup currentOffer)
    {
        OnOffer.Clear();

        foreach (var i in InitializedObjects)
            Destroy(i);

        foreach (var rin in buyer.Inventory)
        {
            var res = (KeyValuePair<Commodity, int>)rin;

            var r = res.Key;
            int max = res.Value - currentOffer.GetResourceAmount(r);

            if (max <= 0) continue;

            var o = Instantiate(OfferPrefab, OfferPrefab.transform.parent);
            
            o.Max = max;
            o.ResourceNameText.text = r.name;
            o.ResourceImage.sprite = r.Sprite;

            o.gameObject.SetActive(true);

            o.AmountInputField.onValueChanged.AddListener(v => UpdateResourceInOffer(v, r));


            InitializedObjects.Add(o.gameObject);
        }

        Holder.SetActive(true);
    }

    private void UpdateResourceInOffer(string s,Commodity r)
    {
        OnOffer.Add(r,int.Parse(s));
    }

    public void Submit()
    {
        FindObjectOfType<TradeSystem>().CounterOffer(OnOffer);

        Holder.SetActive(false);
    }
}
