using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellBook : MonoBehaviour
{
    public static SpellBook instance;
    public List<Ability> spells;// = new List<Ability>(); //all spells learned by the player
    public List<Ability> availableSpells; //spells you can choose from in the hud bar
    //public Ability[] currentSpells = new Ability[6];
    //public AbilityDisplayer[] slots;// = new List<PlayerAbilityHolder>();
    public List<GameObject> slots;// = new List<GameObject>(); //keep track of all of the ability displayers
    public GameObject slotPrefab;
    public Transform abilitiesParent; //beginning of ability displayers
    public Transform abilityHoldersParent; //beginning of currently equipped abilities
    private List<PlayerAbilityHolder> holders; //holders for the currently eqippped abilities

    void Awake()
    {
        instance = this;
        Init();
    }

    void Init()
    {
        slots = new List<GameObject>();
        availableSpells = new List<Ability>(spells);
        holders = new List<PlayerAbilityHolder>(abilityHoldersParent.GetComponentsInChildren<PlayerAbilityHolder>());
        foreach (var item in holders)
        {
            availableSpells.Remove(item.ability);
        }
        foreach (var item in spells)
        {
            GameObject g = Instantiate(slotPrefab);
            g.transform.SetParent(abilitiesParent.transform);
            g.transform.GetChild(0).GetComponent<AbilityDisplayer>().ability = item;
            //g.transform.GetChild(0).GetComponent<AbilitySelection>().selector = this;
            g.transform.GetChild(0).GetComponent<Image>().sprite = item.icon;
        }
    }

    public void AddSpell(Ability a)
    {
        if (a != null)
        {
            spells.Add(a);
            availableSpells.Add(a);
            GameObject g = Instantiate(slotPrefab);
            g.transform.SetParent(abilitiesParent);
            g.GetComponentInChildren<AbilityDisplayer>().ability = a;
            slots.Add(g);
        }
    }

    public void RemoveSpell(Ability a)
    {
        if (a != null)
        {
            int i = spells.IndexOf(a);
            spells.RemoveAt(i);
            PlayerAbilityHolder p = holders.Find(r => r.ability == a);
            int j = holders.IndexOf(p);
            if (!holders.Contains(p))
                availableSpells.Remove(a);
            else
                holders[j].ability = null;
            Destroy(slots[i].gameObject);
        }
    }
}
