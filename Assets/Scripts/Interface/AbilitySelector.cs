using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelector : MonoBehaviour
{
    public GameObject pickPrefab;
    public List<Ability> availableSpells;
    public SpellBook sb;
    public PlayerAbilityHolder clicked;

    void OnEnable()
    {
        availableSpells = sb.availableSpells;
        UpdateSelections();
        if (clicked.ability != null)
        {
            GameObject f = Instantiate(pickPrefab);
            f.transform.SetParent(this.transform);
            f.transform.GetChild(0).GetComponent<AbilitySelection>().ability = null;
            f.transform.GetChild(0).GetComponent<AbilitySelection>().selector = this;
            f.transform.GetChild(0).GetComponent<Image>().sprite = null;
        }
    }
    void OnDisable()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    

    //clicked on an empty slot, so we don't want to give the option for a null selection since it's already empty
    void UpdateSelections()
    {
        foreach (var item in availableSpells)
        {
            if (item != null)
            {
                GameObject g = Instantiate(pickPrefab);
                g.transform.SetParent(this.transform);
                g.transform.GetChild(0).GetComponent<AbilitySelection>().ability = item;
                g.transform.GetChild(0).GetComponent<AbilitySelection>().selector = this;
                g.transform.GetChild(0).GetComponent<Image>().sprite = item.icon;
            }
        }

    }

    //sets the new ability or empties it so you can switch
    public void SwapAbilities(Ability a)
    {
        if (a != null)
        {
            availableSpells.Remove(a);
        }
        if (clicked.ability == null)
        {
            clicked.ability = a;

        }
        else
        {
            availableSpells.Add(clicked.ability);
            clicked.ability = a;
        }
        clicked.AttachAbility();
        
    }
}
