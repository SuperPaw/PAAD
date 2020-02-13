using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RumourManager : MonoBehaviour
{
    private static RumourManager Instance;
    public int LeaderRumourChance = 2;
    public int SupplyRumourChance = 2; 
    public int StoryRumourChance = 1;

    private void Start()
    {
        if (!Instance) Instance = this;
    }

    public static string GetRumourSpeech(Community teller)
    {

        //Rumour types: Leader Trait info. Community Wants / abundance. Community stories (changes names?)

        //teller will talk about neighbours

        return Instance.GenerateRumour(teller);
    }

    private string GenerateRumour(Community source)
    {

        int x = Random.Range(0,SupplyRumourChance + LeaderRumourChance + StoryRumourChance);

        if(x < SupplyRumourChance)
        {
            var about = GetRandom(Database.Instance.AllCommunities, source);

            return "Did you know... the others are stupid";

        }
        else if (x < LeaderRumourChance + SupplyRumourChance)
        {

            var about = GetRandom(Database.Instance.AllCommunities, source);

            return $"Did you know that {about.LeaderName} from {about.name} is very stupid";

        }
        else //Story rumour
        {
            var about = GetRandom(Database.Instance.AllCommunities, source);

            return "Did you know... the others are stupid";
        }

    }

    private static T GetRandom<T>(IEnumerable<T> from, T except) 
    {
        return from.Where(i => !EqualityComparer<T>.Default.Equals(i,except)).ToArray()[Random.Range(0, from.Count())];
    }
}
