using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackLegendaryEffect : Equipment.ILegendary.LegendaryEffect
{
    public override string description { get => description; protected set => description = "You have 20% increased Move Speed"; }

    public override void Effect(PlayerStats player)
    {
        player.secondary[SecondaryStatType.MoveSpeed].AddModifier(new Modifier(SecondaryStatType.MoveSpeed, 20, true));
    }
}
