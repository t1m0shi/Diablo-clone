using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    private Image HealthBar;
    public CharacterStats player;

    void Start()
    {
        player.onHealthChange += UpdateHealthUI;
        HealthBar = GetComponent<Image>();
    }

    protected virtual void UpdateHealthUI(int change)
    {
        HealthBar.fillAmount = (float) player.currentHealth / (float)player.maxHealth;
    }
}
