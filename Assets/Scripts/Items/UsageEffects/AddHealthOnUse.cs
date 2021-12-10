using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddHealthOnUse : Consumable.UsageEffect
{ 
    public int healAmt = 10;
    
    public override bool Consume(PlayerStats player)
    {
        bool consumed = false;
        
        //heal the difference up to heal amount
        int diff = player.maxHealth - player.currentHealth;
        player.currentHealth += Mathf.Clamp(diff, 0, healAmt);

        if (diff > 0)
        {
            consumed = true;
        }
        else
        {

            Debug.Log("Full health already!");
        }
        return consumed;
    }
}
