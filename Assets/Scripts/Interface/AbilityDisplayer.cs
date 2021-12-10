using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityDisplayer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Ability ability;
    PlayerAbilityHolder p;

    void Start()
    {
        p = new PlayerAbilityHolder();
        p.ability = this.ability;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        AbilityTooltip.instance.hovered = p;

        if (AbilityTooltip.instance.hovered.ability != null)
        {
            AbilityTooltip.instance.UpdateText();
            AbilityTooltip.instance.headerObj.SetActive(true);
            AbilityTooltip.instance.contentObj.SetActive(true);
        }
        else
        {
            AbilityTooltip.instance.UpdateText();
            AbilityTooltip.instance.headerObj.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AbilityTooltip.instance.hovered = null;
        AbilityTooltip.instance.headerObj.SetActive(false);
        AbilityTooltip.instance.contentObj.SetActive(false);
    }
}
