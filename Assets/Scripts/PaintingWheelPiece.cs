using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintingWheelPiece : MonoBehaviour
{
    public float StartYRotation;
    [SerializeField]
    private SpriteRenderer SpriteRenderer;
    [SerializeField]
    private Text NoteText;
    public ArtWork ArtWork;
    public RectTransform Sticker;
    public float MinXMod, MaxXMod, MinYMod, MaxYMod, MinSizeXMod, MaxSizeXMod, MinSizeYMod, MaxSizeYMod;
    public float MinRot, MaxRot;
    [HideInInspector]
    public bool PieceForSale;
    //TODO: curve that moves painting out when close to 0

    void Awake()
    {
        if (PieceForSale) return;

        SetRotation(transform.localEulerAngles.y);

        Sticker.localPosition += new Vector3(Random.Range(MinXMod, MaxXMod), Random.Range(MinYMod, MaxYMod));

        Sticker.localEulerAngles += new Vector3(0, 0, Random.Range(MinRot, MaxRot));

        Sticker.localScale += new Vector3(Random.Range(MinSizeXMod, MaxSizeXMod), Random.Range(MinSizeYMod, MaxSizeYMod));
    }

    public void SetRotation(float rotationY)
    {
        StartYRotation = rotationY;
        transform.localEulerAngles = new Vector3(0, StartYRotation + SelectionWheel.WheelPosition, 0);
    }

    public void SetArt(ArtWork artWork)
    {
        SpriteRenderer.sprite = artWork.Sprite;
        NoteText.text = artWork.name;
        ArtWork = artWork;
    }

    void Update()
    {
        if (PieceForSale) return;

        transform.localEulerAngles = new Vector3(0, StartYRotation + SelectionWheel.WheelPosition, 0);
        var rotation = transform.localEulerAngles.y;

        transform.localPosition = new Vector3(SelectionWheel.Instance.HighlightPositionCurve.Evaluate(rotation), 0);
        if (rotation > 180) rotation -= 360;
        if (SelectionWheel.MinSelectRotation < rotation && SelectionWheel.MaxSelectRotation > rotation)
        {
            //Debug.Log("Selecting: " + ArtWork.name);
            SelectionWheel.SelectedPiece = this;
        }
    }
}
