using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpBarUI : MonoBehaviour
{
    private Image ExpBar;
    public PlayerStats player;

    void Start()
    {
        player.onExpGain += UpdateExpUI;
        ExpBar = GetComponent<Image>();
        UpdateExpUI();
    }

    void UpdateExpUI()
    {
        //Debug.Log("updating exp");
        ExpBar.fillAmount = (float)player.currentExp / (float)player.levelExp;
    }
}
