using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class Location : ScriptableObject
{
    public new string name;
    [TextArea]
    public string Description;
    [Header("Possible Player location, if empty")]
    public Community Community;
    public Community[] ClosestNeighbours;
    [Range(0,9)]
    public int Visibility;
    [Range(0, 9)] 
    public int Security;
    [Range(0, 9)] 
    public int Food;
    [SerializeField]
    private string LocationSceneName;

    public void SelectLocation()
    {
        SceneManager.LoadScene(LocationSceneName);
        UIManager.OpenTradeUI();
        Player.Location = this;

    }
}
