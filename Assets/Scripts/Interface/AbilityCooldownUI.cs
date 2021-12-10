using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownUI : MonoBehaviour
{
    public PlayerAbilityHolder holder;
    public float initialcd;
    public Timer cooldown;
    //Color onCooldownColor;
    Image cdImage;
    public bool onCooldown = false;

    void Start()
    {
        cdImage = this.GetComponent<Image>();
        Color og = cdImage.color;
        og.a = 0.8f;
        cdImage.color = og;

        //cooldown = holder.cooldownTimer;
        cdImage.fillAmount = 0;
    }

    void Update()
    {
        if (onCooldown)
        {
            if (initialcd != 0)
                cdImage.fillAmount = cooldown.timer / initialcd;
            else
                cdImage.fillAmount = 0;
            if (cooldown.timer <= 0)//cooldown.GetSecondsLeft() == 0)//
            {
                GoOffCooldown();
            }
        }
    }

    void GoOffCooldown()
    {
        onCooldown = false;
        //maybe play an animation to show it's ready
    }
}
