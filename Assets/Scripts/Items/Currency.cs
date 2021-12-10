using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currency : Item
{
    public CurrencyType type;
    public int value;
}

public enum CurrencyType { Gold }
