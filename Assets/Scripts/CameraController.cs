using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private static CameraController Instance;

    public Vector3 DesiredCamPos;
    public float DesiredOrtographicSize;
    private Camera Cam;
    public float PanSpeed = 1;
    public float ZoomSpeed = 1;
    public int LocationViewSize = 8;

    void Awake()
    {
        Cam = GetComponent<Camera>();
        DesiredCamPos = Cam.transform.position;
        DesiredOrtographicSize = Cam.orthographicSize;

        if (!Instance) 
            Instance = this;
    }

    void Update()
    {
        if (!Instance)
            return;

        Cam.transform.position = Vector3.Lerp(Cam.transform.position, DesiredCamPos, PanSpeed * Time.unscaledDeltaTime);

        Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, DesiredOrtographicSize, ZoomSpeed * Time.unscaledDeltaTime);

    }

    public static void MoveToLocation(LocationObject locationObject)
    {
        Instance.DesiredCamPos = locationObject.transform.position;
        Instance.DesiredOrtographicSize = Instance.LocationViewSize;
    }
}
