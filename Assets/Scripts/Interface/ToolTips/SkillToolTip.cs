using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillToolTip : Tooltip <SkillHolder>
{
    public static SkillToolTip instance;
    protected override void Init()
    {
        base.Init();
        instance = this;
    }

    public override void UpdateText()
    {
        header.text = hovered.skill.name;
        content.text = hovered.skill.description;
    }
}
