using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public static List<ArtWork> ArtWorks;
    public static Dictionary<Commodity, int> Inventory = new Dictionary<Commodity, int>();

    private static Location location;

    public class ValueChange : UnityEvent<int> { }
    public static ValueChange OnVisibilityChange = new ValueChange();
    public static ValueChange OnSecurityChange = new ValueChange();
    public static ValueChange OnFoodChange = new ValueChange();


    private static int visibility;
    public static int Visibility
    {
        get => visibility; set
        {
            visibility = value;
            OnVisibilityChange.Invoke(value);
        }
    }
    private static int security;
    public static int Security
    {
        get => security; set
        {
            security = value;
            OnSecurityChange.Invoke(value);
        }
    }
    private static int food;
    public static int Food
    {
        get => food; set
        {
            food = value;
            OnFoodChange.Invoke(value);
        }
    }


    public static Location Location
    {
        get => location;
        set
        {
            location = value;
            Visibility = location.Visibility;
            Security = location.Security;
            Food = location.Food;
        }
    }



    //TODO: use parent class for community and player for shared fields

    void Awake()
    {
#if UNITY_EDITOR
        ArtWorks = Resources.FindObjectsOfTypeAll<ArtWork>().ToList();
#endif
    }


}
