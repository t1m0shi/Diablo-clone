using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilitySelection : AbilityDisplayer, IPointerClickHandler
{
    public AbilitySelector selector;
    public void OnPointerClick(PointerEventData eventData)
    {
        selector.SwapAbilities(this.ability);
        selector.clicked.OnPointerExit(eventData);
        selector.gameObject.SetActive(false);
    }
}