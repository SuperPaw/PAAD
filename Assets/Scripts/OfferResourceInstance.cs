using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfferResourceInstance : MonoBehaviour
{
    public Button UpButton, DownButton;
    public TextMeshProUGUI ResourceNameText;
    public Image ResourceImage;
    public TMP_InputField AmountInputField;
    public int Max;

    private void Start()
    {
        UpButton.onClick.AddListener(ValueUp);
        DownButton.onClick.AddListener(ValueDown);
    }

    void ValueUp()
    {
        var i = int.Parse(AmountInputField.text);
        if (i >= Max) return;

        AmountInputField.text ="" +  (i + 1);
    }
    void ValueDown()
    {
        var i = int.Parse(AmountInputField.text);
        if (i <= 0) return;

        AmountInputField.text = "" + (i - 1);

    }

}
