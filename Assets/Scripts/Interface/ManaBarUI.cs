using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBarUI : MonoBehaviour
{

    private Image ManaBar;
    public PlayerStats player;

    void Start()
    {
        player.onSpellCast += UpdateManaUI;
        ManaBar = GetComponent<Image>();
    }

    void UpdateManaUI()
    {
        ManaBar.fillAmount = (float)player.currentMana / (float)player.maxMana;
    }
}
