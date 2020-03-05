using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionWheel : MonoBehaviour
{
    public static SelectionWheel Instance;
    public static float WheelPosition = 0;
    public GameObject Holder;
    public ArtWork[] ArtWorks;
    //How much the x position should move out
    public AnimationCurve HighlightPositionCurve;
    // public static bool MovingWheel;
    public PaintingWheelPiece PaintingWheelPieceInstance;
    private List<GameObject> InstantiatedPaintings = new List<GameObject>();
    private float lastMousePos;
    public static PaintingWheelPiece SelectedPiece;
    public static float MinSelectRotation;
    public static float MaxSelectRotation;
    private float MoveDelta;
    private PaintingWheelPiece PieceForSale;

    void Awake()
    {
        if (!Instance) Instance = this;

        if (ArtWorks.Length == 0) return;

    }

    public static void OpenWheel(List<ArtWork> artWorks = null)
    {
        if (artWorks != null)
            Instance.ArtWorks = artWorks.ToArray();

        Instance.Open();

    }

    public static void CloseWheel()
    {
        Instance.Close();
    }

    public static void DestroySaleObject()
    {
        if(Instance.PieceForSale)
            Destroy(Instance.PieceForSale.gameObject);
    }

    void Update()
    {
        if (!Holder.activeInHierarchy)
            return;

        if (Input.GetMouseButton(0))
        {
            //ArtSelectUI.Disable();

            MoveDelta = (lastMousePos - Input.mousePosition.x) / 5;
        }
        else if (SelectedPiece)
        {
            MoveDelta *= 0.90f;

            //TODO: only update if not already selected
            ArtSelectUI.SetArt(SelectedPiece.ArtWork);

            float rotation = SelectedPiece.transform.localEulerAngles.y;
            if (rotation > 180) rotation -= 360;

            if (rotation < -1f)
            {
                WheelPosition += 0.08f;
            }
            else if (rotation > 1f)
            {
                WheelPosition -= 0.08f;
            }
        }


        WheelPosition += MoveDelta;

        lastMousePos = Input.mousePosition.x;
    }

    private void Open()
    {
        if (PieceForSale)
            Destroy(PieceForSale.gameObject);

        var rotationDifference = 360 / ArtWorks.Length;

        MaxSelectRotation = rotationDifference / 2;
        MinSelectRotation = -MaxSelectRotation;

        var nextRot = 0;

        foreach (var art in ArtWorks)
        {
            var next = Instantiate(PaintingWheelPieceInstance, PaintingWheelPieceInstance.transform.parent);

            next.gameObject.SetActive(true);

            next.SetRotation(nextRot);

            nextRot += rotationDifference;
            next.SetArt(art);

            InstantiatedPaintings.Add(next.gameObject);
        }

        Holder.SetActive(true);

        PaintingWheelPieceInstance.gameObject.SetActive(false);
    }

    private void Close()
    {

        Debug.Log("closing wheel");
        Holder.SetActive(false);

        //TODO: use a selection Object instead
        if (SelectedPiece)
        {
            PieceForSale = Instantiate(SelectedPiece);
            PieceForSale.transform.localScale = SelectedPiece.transform.lossyScale;
            PieceForSale.transform.SetPositionAndRotation(SelectedPiece.transform.position,SelectedPiece.transform.rotation);
            PieceForSale.PieceForSale = true;
        }

        foreach (var p in InstantiatedPaintings)
        {
            Destroy(p);
        }
        InstantiatedPaintings.Clear();
    }
}
