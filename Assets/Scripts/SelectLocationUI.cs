using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;

public class SelectLocationUI : MonoBehaviour
{
    private LocationObject[] Locations;
    private LocationObject CurrentSelectedLocation;
    public Button SelectLocationButton, NextLocationButton, PreviousLocationButton;
    public Text VisibilityText, SecurityText, FoodText, NeighboursText, LocationName;

    void Start()
    {
        Locations = FindObjectsOfType<LocationObject>().Where(l => !l.Location.Community).ToArray();
        UpdateLocation(Locations[0]);
        NextLocationButton.onClick.AddListener(NextLocation);
        PreviousLocationButton.onClick.AddListener(PreviousLocation);
        SelectLocationButton.onClick.AddListener(SelectLocation);
    }

    void NextLocation()
    {
        var i = Array.IndexOf(Locations, CurrentSelectedLocation) +1;

        if (i == Locations.Length) i = 0;

        UpdateLocation(Locations[i]);
    }

    void PreviousLocation()
    {
        var i = Array.IndexOf(Locations, CurrentSelectedLocation) -1;

        if (i < 0) i = Locations.Length-1;

        UpdateLocation(Locations[i]);
    }

    void SelectLocation()
    {
        CurrentSelectedLocation.Location.SelectLocation();
    }

    void UpdateLocation(LocationObject loc)
    {
        CurrentSelectedLocation = loc;
        var l = loc.Location;
        VisibilityText.text = $"Visibility: {l.Visibility}";
        SecurityText.text = $"Security: {l.Security}";
        FoodText.text = $"Food: {l.Food}";
        LocationName.text = l.name;

    }
}
