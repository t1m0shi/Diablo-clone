using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddManaOnUse : Consumable.UsageEffect
{
    public int manaAmt = 10;

    public override bool Consume(PlayerStats player)
    {
        bool consumed = false;

        //heal the difference up to heal amount
        int diff = player.maxMana - player.currentMana;
        player.currentHealth += Mathf.Clamp(diff, 0, manaAmt);

        if (diff > 0) consumed = true;
        return consumed;
    }
}
