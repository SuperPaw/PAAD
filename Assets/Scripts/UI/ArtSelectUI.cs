using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ArtSelectUI : MonoBehaviour
{
    public ArtWork ArtWork;
    public Image ArtImage;
    public TMP_Dropdown DropdownList;
    public TextMeshProUGUI DescriptionText;

    void OnEnable()
    {
        DropdownList.ClearOptions();

        DropdownList.AddOptions(Player.ArtWorks.Select(art => new TMP_Dropdown.OptionData(art.name, art.Sprite)).ToList());

        DropdownList.onValueChanged.RemoveAllListeners();

        DropdownList.onValueChanged.AddListener(ChangeSelectedArt);

        ChangeSelectedArt(0);
    }

    void ChangeSelectedArt(int i)
    {
        ArtWork = Player.ArtWorks[i];

        ArtImage.sprite = ArtWork.Sprite;
        DescriptionText.text = ArtWork.Info;
    }

    public void Submit()
    {
        FindObjectOfType<TradeSystem>().SelectArt(ArtWork);
        gameObject.SetActive(false);
    }

}
