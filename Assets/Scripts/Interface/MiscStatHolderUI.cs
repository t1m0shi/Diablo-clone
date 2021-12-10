using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System;

public class MiscStatHolderUI : StatHolderUI
{
    
    public string toDisplay; //clean for UI
    private string ogDisplay; //remove spaces to search variables
    public Stat stat;

    public override void UpdateHolder()
    {
        base.UpdateHolder();
        
        ogDisplay = string.Join("", toDisplay.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        FieldInfo p = inst.GetType().GetField(ogDisplay);
        stat = (Stat)p.GetValue(inst);
        if (ogDisplay.Contains("Chance"))
            this.text.text = toDisplay + ": " + stat.GetChance().ToString()+"%";
        else
            this.text.text = toDisplay +": "+ stat.GetValue().ToString();
    }
}
