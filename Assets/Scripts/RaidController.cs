using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidController : MonoBehaviour
{
    private static RaidController Instance;

    void Start()
    {
        if (!Instance) Instance = this;
    }

    public static void StartFight(Community attacker)
    {
        SoundController.PlayStinger(SoundBank.Stinger.Raid);

        int attackStr = attacker.GetAttackStrength();

        if (attackStr > Player.Security)
        {
            //      Attack value higher than security means success
            //          deals trampling damage to security and half to food(min 5.)
            //steals art
            var diff = Mathf.Max(5, attackStr - Player.Security);
            Player.Security -= diff;
            Player.Food -= diff;
            var stolen = Player.ArtWorks[0];

            Player.ArtWorks.RemoveAt(0);

            Debug.Log($"{attacker} raided your shop. Destroying your defences for {diff} and stealing {diff} food and the {stolen.name}");
        }
        else if (attackStr > Player.Security / 2f)
        {
            //  Attack value <= Security / 2
            //      Attack deals damage to security based on difference from 0 - 5
            int diff =(int) (((attackStr - Player.Security / 2f) / (Player.Security / 2f)) * 5);
            Player.Security -= diff;

            Debug.Log($"{attacker} tried to raid your shop. They did {diff} damage to your defences!");

        }
        else
        {
            Debug.Log($"{attacker} tried to raid your shop, but were severely beaten. They will behave better from now on.");

            attacker.Attitude = 15;

        }
    }
}
