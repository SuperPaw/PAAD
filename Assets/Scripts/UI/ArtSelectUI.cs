using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class ArtSelectUI : MonoBehaviour
{
    private static ArtSelectUI Instance;
    public ArtWork ArtWork;
    public Image ArtImage;
    public TMP_Dropdown DropdownList;
    public Text NameText,ArtistText,ValueText, DescriptionText;
    public GameObject Holder;

    void OnEnable()
    {
        if (!Instance) Instance = this;

    }

    void ChangeSelectedArt(int i)
    {
        ArtWork = Player.ArtWorks[i];

        SetArt(ArtWork);
    }

    public static void Disable()
    {
        Instance.Holder.SetActive(false);
    }

    public static void SetArt(ArtWork artWork)
    {
        if (!artWork) return;

        Instance.ArtImage.sprite = artWork.Sprite;
        Instance.DescriptionText.text = artWork.Info;
        Instance.NameText. text = $"{artWork.name}, {artWork.Year}";
        Instance.ValueText. text = $"Old world value: {artWork.EstimatedValue}";
        Instance.ArtistText. text = $"{artWork.Artist}";
        Instance.ArtWork = artWork;

        Instance.Holder.SetActive(true);
    }

    public void Submit()
    {
        Debug.Log("Selected " + ArtWork);

        FindObjectOfType<TradeSystem>().SelectArt(ArtWork);
        SelectionWheel.CloseWheel();
        Holder.gameObject.SetActive(false);
    }
}
