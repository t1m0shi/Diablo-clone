using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class StatsTooltip : Tooltip<StatHolderUI>
{
    public static StatsTooltip instance;
    //public TextMeshProUGUI content;
    //public StatHolderUI hovered;
    //public GameObject contentObj;
    //RectTransform m_RectTransform;

    protected override void Init()
    {
        base.Init();
        instance = this;
    }

    public override void UpdateText()
    {
        Type t = hovered.GetType();
        if (t == typeof(PrimaryStatHolderUI))
        {
            PrimaryStatHolderUI p = (PrimaryStatHolderUI)hovered;
            switch (p.stype)
            {
                case PrimaryStatType.Health:
                    content.text = "This increases your maximum health";
                    break;
                case PrimaryStatType.Mana:
                    content.text = "This increases your maximum mana";
                    break;
                case PrimaryStatType.Strength:
                    content.text = "This increases something";
                    break;
                case PrimaryStatType.Agility:
                    content.text = "This increases your sdf";
                    break;
                case PrimaryStatType.Intellect:
                    content.text = "This increases your maxisdfsdh";
                    break;
            }
        }
        else if (t == typeof(SecondaryStatHolderUI))
        {
            SecondaryStatHolderUI s = (SecondaryStatHolderUI)hovered;
            switch (s.stype)
            {
                case SecondaryStatType.AttackSpeed:
                    content.text = "This increases your attacks per second.";
                    break;
                case SecondaryStatType.CooldownReduction:
                    content.text = "This reduces the time your abilities are unusable.";
                    break;
                case SecondaryStatType.CritChance:
                    content.text = "This increases the chance your attacks and abilities deal critical damage.";
                    break;
                case SecondaryStatType.CritDamage:
                    content.text = "This increases extra critical damage that your attacks and abilities do.";
                    break;
                case SecondaryStatType.MoveSpeed:
                    content.text = "This increases how fast you move.";
                    break;
            }
        }
    }
}
