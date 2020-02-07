﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopbarUI : MonoBehaviour
{
    [SerializeField]
    private Text SecurityValueText;
    [SerializeField]
    private Text VisibilityValueText;
    [SerializeField]
    private Text FoodValueText;

    void Awake()
    {
        Player.OnVisibilityChange.AddListener(v => VisibilityValueText.text = v.ToString("N0"));
        Player.OnFoodChange.AddListener(v => FoodValueText.text = v.ToString("N0"));
        Player.OnSecurityChange.AddListener(v => SecurityValueText.text = v.ToString("N0"));
    }

}
