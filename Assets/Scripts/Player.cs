using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


//TODO: maybe rename to something more manager ish
public class Player : MonoBehaviour
{
    public static List<ArtWork> ArtWorks;
    public static Dictionary<Commodity, int> Inventory = new Dictionary<Commodity, int>();

    private static int day = 0;
    public static int Day
    {
        get => day; set
        {
            if (value != day)
                OnDayChange.Invoke(); //TODO: invoke several times for more days
            day = value;
        }
    }

    public static UnityEvent OnDayChange = new UnityEvent();

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
            Visibility = location.Visibility ;
            Security = location.Security ;
            Food = location.Food * 10;
        }
    }




    //TODO: use parent class for community and player for shared fields

    void Awake()
    {
        Visibility = 10;

#if UNITY_EDITOR
        ArtWorks = Resources.FindObjectsOfTypeAll<ArtWork>().ToList();
#endif
        DontDestroyOnLoad(gameObject);

        OnDayChange.AddListener(() => Player.Food--);
    }

    public static void NextDay()
    {
        Day++;
    }

}
