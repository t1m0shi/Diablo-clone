using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    public GameObject parent;
    public StatHolderUI[] statHolders;
    public delegate void OnStatChange();
    public OnStatChange onStatChange;
    //public PlayerStats inst;

    void Awake()
    {
        //onStatChange += UpdateUI;
    }
    void OnEnable()
    {
        statHolders = GetComponentsInChildren<StatHolderUI>();

        UpdateUI();
    }

    public void UpdateUI()
    {
        //Debug.Log("updating stats ui");
        foreach (var item in statHolders)
        {
            item.UpdateHolder();
        }
    }
}
