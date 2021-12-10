using UnityEngine;
using UnityEngine.UI;

public class DefenseStatHolderUI : StatHolderUI
{
    public DmgType stype;

    public override void UpdateHolder()
    {
        base.UpdateHolder();
        if (stype == DmgType.Physical)
            this.text.text = stype.GetDescription() + ": " + inst.defenses[stype].GetValue().ToString();
        else
            this.text.text = stype.ToString() + " Resistance: " + inst.defenses[stype].GetValue().ToString();
    }
}
