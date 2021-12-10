using UnityEngine;
using UnityEngine.UI;

public class PrimaryStatHolderUI : StatHolderUI
{
    public PrimaryStatType stype;

    public override void UpdateHolder()
    {
        base.UpdateHolder();
        this.text.text = stype.ToString() +": "+ inst.primary[stype].GetValue().ToString();
    }
}
