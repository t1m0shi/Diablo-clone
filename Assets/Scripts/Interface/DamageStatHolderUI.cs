using UnityEngine;
using UnityEngine.UI;

public class DamageStatHolderUI : StatHolderUI
{
    public DmgType stype;

    
    public override void UpdateHolder()
    {
        base.UpdateHolder();
        this.text.text = stype.ToString() + " Damage: " + inst.damages[stype].GetValue().ToString();
    }
}
