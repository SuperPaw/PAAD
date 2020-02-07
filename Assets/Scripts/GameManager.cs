using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance;
    
    void Awake()
    {
        if (!Instance) Instance = this;

        DontDestroyOnLoad(this);
    }
}
