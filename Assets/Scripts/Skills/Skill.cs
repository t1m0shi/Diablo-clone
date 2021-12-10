using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Skill", menuName = "Skill")]
public class Skill : ScriptableObject
{
    public new string name;
    public string description;
    public Modifier mod;
    public Ability ability;

    public void UnlockSkill()
    {
        if (ability != null)
        {
            SpellBook.instance.AddSpell(this.ability);

        }
        if (mod != null)
        {
            switch (mod.mtype)
            {
                case ModType.Primary:
                    PlayerStats.instance.primary[mod.ptype].AddModifier(mod);
                    break;
                case ModType.Secondary:
                    PlayerStats.instance.secondary[mod.stype].AddModifier(mod);
                    break;
                case ModType.Resistance:
                    PlayerStats.instance.defenses[mod.dtype].AddModifier(mod);
                    break;
                case ModType.Damage:
                    PlayerStats.instance.damages[mod.dtype].AddModifier(mod);
                    break;
            }
        }
    }

    public void RefundSkill()
    {
        SpellBook.instance.RemoveSpell(this.ability);
        switch (mod.mtype)
        {
            case ModType.Primary:
                PlayerStats.instance.primary[mod.ptype].RemoveModifier(mod);
                break;
            case ModType.Secondary:
                PlayerStats.instance.secondary[mod.stype].RemoveModifier(mod);
                break;
            case ModType.Resistance:
                PlayerStats.instance.defenses[mod.dtype].RemoveModifier(mod);
                break;
            case ModType.Damage:
                PlayerStats.instance.damages[mod.dtype].RemoveModifier(mod);
                break;
        }
    }
}
