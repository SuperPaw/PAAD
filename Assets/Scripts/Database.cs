using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Database : MonoBehaviour
{
    public static Commodity[] AllComodities;

    public static Community[] AllCommunities;

    private void Awake()
    {
        LoadScriptableObjects();
    }


    /// <summary>
    /// Loads all scriptable objects of type Commodity into AllCommodities. 
    /// </summary>
    void LoadScriptableObjects()
    {
#if UNITY_EDITOR
        AllComodities = Resources.FindObjectsOfTypeAll<Commodity>().Where(c => !(c as ArtWork)).ToArray();

        AllCommunities = Resources.FindObjectsOfTypeAll<Community>();
#endif
    }

}
