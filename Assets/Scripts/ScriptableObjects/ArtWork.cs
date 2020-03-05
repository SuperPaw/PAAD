using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ArtWork : Commodity
{
    [Header("Art work specific")]
    public string Artist;
    [TextArea]
    public string SalesPitch;
    [TextArea]
    public string Info;
    public int Year;
    public string EstimatedValue;

    //TODO: HashSet
    public Property[] Properties;
    public Color[] Colors;
    public string Movement = "e.g. Surrealist";
    public string Type = "e.g. Painting"; //TODO: types could have inferred properties, which is added automatically

    public enum Property {Any, Big,Small,Burnable,Stone,Wood,Heavy,Light,Metal,Expensive,Famous,Violent,Serene,Educational,Humoristic,Modern,Antique,Beautiful,Ugly,
        Provocative, Sexy}

    public enum Color {Any, Red,Blue,Green,Yellow,Pink,White,Black,Gray,Brown}
}
