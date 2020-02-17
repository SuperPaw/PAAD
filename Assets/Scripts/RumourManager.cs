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
            var firstAbout = GetRandom(Database.Instance.AllCommunities, source);

            Community secondAbout = null;

            while (!secondAbout || secondAbout == firstAbout)
                secondAbout = GetRandom(Database.Instance.AllCommunities, source);

            var firstResource = GetRandom(firstAbout.PrimaryResources, null);
            var secondRes = GetRandom(secondAbout.PrimaryResources, firstResource);

            var amountOfSec = firstResource.InitialValue / (float) secondRes.InitialValue;
            var amountOfFirst = 1f;

            while( amountOfSec %1 > 0.1f && amountOfSec %1 < 0.9f)
            {
                amountOfSec *= (amountOfFirst + 1) / amountOfFirst;
                amountOfFirst++;
            }

            //ROUND up
            if (amountOfSec % 1 >= 0.9f)
                amountOfSec += 0.1f;


            return $"I heard the <b>{firstAbout.name}</b>  is trading <b> {amountOfFirst.ToString("N0")} {firstResource.name}'s for {amountOfSec.ToString("N0")} {secondRes.name}'s</b> with the <b>{secondAbout.name}</b>";


        }
        else if (x < LeaderRumourChance + SupplyRumourChance)
        {

            var about = GetRandom(Database.Instance.AllCommunities, source);

            return $"Did you know that <b>{about.LeaderName}</b>  from the <b>{about.name}</b>  is very <b>{about.LeaderTraits[Random.Range(0,about.LeaderTraits.Length)]}</b>";

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
