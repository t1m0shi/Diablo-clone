using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Pulls from the lootTable to drop loot for an enemy
public class LootTable
{
    
    public List<LootDrop> lootdrops;
    private float totalWeight = 0;
    private bool isInitialized = false;

    public LootTable()
    {
        lootdrops = new List<LootDrop>();
    }
    //must initialize after creating a new one and populating the lootdrops
    public void Init()
    {
        if (!isInitialized)
        {
            totalWeight = lootdrops.Sum(lootdrop => lootdrop.weight);
            isInitialized = true;
        }
    }
    //attaches loot drops to a loot object, may be called more than once when an enemy is killed
    public Item GetDrop()
    {
        if (!isInitialized)
        {
            Debug.LogError("DropTable is not initialized");
            return null;
        }

        //bool dropped = false;

        float roll_forRarity = UnityEngine.Random.Range(0f, totalWeight);
        //float roll_forRarity = 4f; //for testing
        Item item = null;
        
        foreach (LootDrop drop in lootdrops) //might need to check order on lootdrops to make sure it checks for the rarest first?? not sure on math here
        {
            
            //weightSum += drop.weight;
            
            //if (roll_forRarity < weightSum)
            if (roll_forRarity <= drop.weight)
            {
                //item.rarity = drop.rarity;
                //roll for type of item dropped
                //int roll2 = UnityEngine.Random.Range(1, 3);
                int roll_forItemType = 2; //for testing, always spawn an equipment //set as 3 separate rolls??
                if (roll_forItemType == 1)
                {
                    //spawn consumable
                    //loot.item = new Consumable(drop.rarity);
                    item = ScriptableObject.CreateInstance<Consumable>();
                    //loot.item.Init(drop.rarity);
                }
                else if (roll_forItemType == 2)
                {
                    //spawn equipment
                    //item = new Equipment(drop.rarity);
                    //int roll_forEquipType = UnityEngine.Random.Range(2, System.Enum.GetNames(typeof(EquipmentSlot)).Length + System.Enum.GetNames(typeof(WeaponType)).Length + 1);
                    //int roll_forEquipType = 15; //fixed for testing
                    int roll_forEquipType = 2;
                    if (roll_forEquipType > System.Enum.GetNames(typeof(WeaponType)).Length)
                    {
                        item = ScriptableObject.CreateInstance<Equipment>();
                        item.Init(drop.rarity);
                    }
                    else
                    {
                        item = ScriptableObject.CreateInstance<Weapon>();
                        //get random weapon
                        item.InitWeapon(drop.rarity);//, (WeaponType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(WeaponType)).Length));
                    }
                }
                else
                {
                    //item = new Currency();
                }
                //return new Item(drop.rarity);
                //dropped = true;
                break;
            }
            roll_forRarity -= drop.weight;
        }
        //Debug.Log(roll_forRarity.ToString());
        return item;
    }
}

public class LootDrop: System.IComparable<LootDrop>
{
    //public Item item;
    public Rarity rarity;
    public float weight;

    public LootDrop(float weight)
    {
        //this.item = new Item();
        this.weight = weight;
    }
    //public LootDrop(Item item, int weight)
    public LootDrop(Rarity rarity, float weight)
    {
        //this.item = item;
        this.rarity = rarity;
        this.weight = weight;
    }

    public int CompareTo(LootDrop other)
    {
        if (other == null) return 1;
        else return this.weight.CompareTo(other.weight);
    }
}
