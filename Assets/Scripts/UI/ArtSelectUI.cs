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
    public Text DescriptionText;
    public GameObject Holder;

    void OnEnable()
    {
        if (!Instance) Instance = this;

        DropdownList.ClearOptions();

        DropdownList.AddOptions(Player.ArtWorks.Select(art => new TMP_Dropdown.OptionData(art.name, art.Sprite)).ToList());

        DropdownList.onValueChanged.RemoveAllListeners();

        DropdownList.onValueChanged.AddListener(ChangeSelectedArt);

        ChangeSelectedArt(0);
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
        Instance.ArtImage.sprite = artWork.Sprite;
        Instance.DescriptionText.text = artWork.Info;

        Instance.Holder.SetActive(true);
    }

    public void Submit()
    {
        FindObjectOfType<TradeSystem>().SelectArt(ArtWork);
        gameObject.SetActive(false);
    }

}
