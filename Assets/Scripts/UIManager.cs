using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager Instance;
    [SerializeField]
    private GameObject TradeUI;
    [SerializeField]
    private GameObject SelectLocationUI;

    void Awake()
    {
        if (!Instance) Instance = this;

        DontDestroyOnLoad(this);
    }

    internal static void OpenTradeUI()
    {
        Instance.TradeUI.SetActive(true);
        Instance.SelectLocationUI.SetActive(false);

    }
}
