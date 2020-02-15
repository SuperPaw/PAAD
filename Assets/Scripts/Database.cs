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
    public static Database Instance;
    public Commodity[] AllComodities;

    public Community[] AllCommunities;

    public ArtWork[] AllArtWorks;

    private void Awake()
    {
        if (!Instance) Instance = this;

        //LoadScriptableObjects();
    }


    /// <summary>
    /// Loads all scriptable objects of type Commodity into AllCommodities. 
    /// </summary>
    void LoadScriptableObjects()
    {
#if UNITY_EDITOR
        AllComodities = Resources.FindObjectsOfTypeAll<Commodity>().Where(c => !(c as ArtWork)).ToArray();

        AllCommunities = Resources.FindObjectsOfTypeAll<Community>();

        AllArtWorks = Resources.FindObjectsOfTypeAll<ArtWork>();
#endif
    }

}
