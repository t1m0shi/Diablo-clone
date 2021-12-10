using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AbilityTooltip  : Tooltip<PlayerAbilityHolder>
{
    public static AbilityTooltip instance;

    protected override void Init()
    {
        base.Init();
        instance = this;
    }
    public override void UpdateText()
    {
        if (hovered.ability != null)
        {
            header.text = hovered.ability.name;
            content.text = hovered.ability.effect.description;
        }
        else
        {
            header.text = "None";
        }
    }
}
