using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class EnemyHealthBarUI : HealthBarUI
{
    private Slider health;
    void Start()
    {
        player.onHealthChange += UpdateHealthUI;
        health = GetComponent<Slider>();
        health.maxValue = (float)player.maxHealth;
        health.value = (float)player.maxHealth;
        health.minValue = 0;
    }
    protected override void UpdateHealthUI(int change)
    {
        //health. = (float)player.currentHealth / (float)player.maxHealth;
        health.value = (float)player.currentHealth;
        //health.value = (float)player.currentHealth / (float)player.maxHealth;
    }
}
