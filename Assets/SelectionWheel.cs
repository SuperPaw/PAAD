using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionWheel : MonoBehaviour
{
    public static SelectionWheel Instance;
    public static float WheelPosition = 0;
    public ArtWork[] ArtWorks;
    //How much the x position should move out
    public AnimationCurve HighlightPositionCurve;
   // public static bool MovingWheel;
    public PaintingWheelPiece PaintingWheelPieceInstance;
    private float lastMousePos;
    public static PaintingWheelPiece SelectPiece;
    public static float MinSelectRotation;
    public static float MaxSelectRotation;
    private float MoveDelta;

    void Awake()
    {
        if (!Instance) Instance = this;

        if (ArtWorks.Length == 0) return;

        var rotationDifference = 360 / ArtWorks.Length;

        MaxSelectRotation = rotationDifference /2;
        MinSelectRotation = -MaxSelectRotation;


        var nextRot = 0;

        foreach(var art in ArtWorks)
        {
            var next = Instantiate(PaintingWheelPieceInstance,PaintingWheelPieceInstance.transform.parent);

            next.SetRotation(nextRot);

            nextRot += rotationDifference;
            next.SetArt(art);
        }

        PaintingWheelPieceInstance.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ArtSelectUI.Disable();

            MoveDelta = (lastMousePos - Input.mousePosition.x) / 5;
        }
        else if(SelectPiece)
        {
            MoveDelta *= 0.90f;

            //TODO: only update if not already selected
            ArtSelectUI.SetArt(SelectPiece.ArtWork);

            float rotation = SelectPiece.transform.localEulerAngles.y;
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
}
