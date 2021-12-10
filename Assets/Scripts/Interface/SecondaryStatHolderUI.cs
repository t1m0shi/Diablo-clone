using UnityEngine;
using UnityEngine.UI;

public class SecondaryStatHolderUI : StatHolderUI
{
    //public PlayerStats inst;
    public SecondaryStatType stype;

    public override void UpdateHolder()
    {
        base.UpdateHolder();
        //not all percents
        switch (stype)
        {
            case SecondaryStatType.AttackSpeed:
                this.text.text = stype.ToString() + ": " + inst.secondary[stype].GetValue().ToString();
                break;
            default:
                this.text.text = stype.ToString() + ": " + inst.secondary[stype].GetChance().ToString() + "%";
                break;
        }
        
    }
}
