using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackSpeedIncreaseEffect : Ability.PlayerAbilityEffect.Buff
{
    public override void Activate(PlayerStats originator)
    {
        base.Activate(originator);
        //Debug.Log("increasing atk speed");
        this.target = originator;
        change = new Modifier(SecondaryStatType.AttackSpeed, percent, true);
        originator.secondary[SecondaryStatType.AttackSpeed].AddModifier(change);
        holder.durationTimer.SetTimer(duration, RemoveEffect);
    }
    protected override void RemoveEffect()
    {
        //Debug.Log("removing atk speed");
        if (target != null)
            target.secondary[SecondaryStatType.AttackSpeed].RemoveModifier(change);
    }
}
