using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Handles the players stats and adds/removes modifiers when equipping items. */
public class PlayerStats : CharacterStats {

	public static PlayerStats instance;

	public int currentMana;
	public int maxMana;
	public int baseMana = 100;
	public int baseMoveSpeed = 2;
	public StatsUI statsUI;
	public int currentExp = 20;
	public int levelExp = 100;
	public int skillPoints = 0;
	public int refundPoints = 0;
	public int maxHands = 2;

	public delegate void OnSpellCast();
	public OnSpellCast onSpellCast;
	public delegate void OnEnemyDeath(int exp);
	public OnEnemyDeath onEnemyDeath;
	public delegate void OnExpGain();
	public OnExpGain onExpGain;

    private void Awake()
    {
		instance = this;
	}
    // Use this for initialization
    void Start () {
		Init();
		EquipmentManager.instance.onEquipmentChanged += OnEquipmentChanged;
		EquipmentManager.instance.onWeaponChanged += OnWeaponChanged;

		combat = GetComponent<PlayerCombat>();

		//primary[PrimaryStatType.Mana].AddModifier(new Modifier(PrimaryStatType.Mana, baseMana)); 
		primary[PrimaryStatType.Mana].baseValue = baseMana;
		maxMana = (int)primary[PrimaryStatType.Mana].GetValue();
		currentMana = maxMana;
		secondary[SecondaryStatType.MoveSpeed].baseValue = baseMoveSpeed;

		onStatsChanged += statsUI.UpdateUI;
		onEnemyDeath += GainExp;
	}

	// Called when an item gets equipped/unequipped
	void OnEquipmentChanged (Equipment newItem, Equipment oldItem)
	{
		//Debug.Log("changing modifiers");
		// Add new modifiers
		if (newItem != null)
		{
            foreach (var item in newItem.modifiers)
            {
                switch (item.mtype)
                {
					case ModType.Primary:
						primary[item.ptype].AddModifier(item);
						break;
					case ModType.Secondary:
						secondary[item.stype].AddModifier(item);
						break;
					case ModType.Resistance:
						defenses[item.dtype].AddModifier(item);
						break;
					case ModType.Damage:
						damages[item.dtype].AddModifier(item);
						break;
                }
            }
		}

		// Remove old modifiers
		if (oldItem != null)
		{
			foreach (var item in oldItem.modifiers)
			{
				switch (item.mtype)
				{
					case ModType.Primary:
						primary[item.ptype].RemoveModifier(item);
						break;
					case ModType.Secondary:
						secondary[item.stype].RemoveModifier(item);
						break;
					case ModType.Resistance:
						defenses[item.dtype].RemoveModifier(item);
						break;
					case ModType.Damage:
						damages[item.dtype].RemoveModifier(item);
						break;
				}
			}
		}
	}

	void OnWeaponChanged(Weapon newItem, Weapon oldItem)
    {
		OnEquipmentChanged(newItem, oldItem);
		if (oldItem != null)
		{
			handsFull -= oldItem.handed;
			/*
			if (oldItem.equipSlot == EquipmentSlot.MainHand)
			{
				mainHand = null;
				//baseAttackSpeed[0] = 0;
			}
			else
			{
				offHand = null;
				//baseAttackSpeed[1] = 0;
			}*/
		}
		if (newItem != null)
		{
			handsFull += newItem.handed;
			/*
			if (newItem.equipSlot == EquipmentSlot.MainHand)
			{
				mainHand = newItem;
				//baseAttackSpeed[0] = newItem.damage.attacksPerSecond;
			}
			else
			{
				offHand = newItem;
				//baseAttackSpeed[1] = newItem.damage.attacksPerSecond;
			}*/
		}
	}

	public override void Die()
	{
		base.Die();
		PlayerManager.instance.KillPlayer();
	}

	public bool UseMana(int cost)
    {
		bool casted = false;
		if (cost <= currentMana)
		{
			//Debug.Log("Lost " + cost + " mana.");
			cost = Mathf.Clamp(cost, 0, cost);
			currentMana -= cost;
			casted = true;
		}
        else
        {
			Debug.Log("Not enough mana!");
        }
		return casted;
    }

	private void GainExp(int exp)
    {
		int diff = levelExp - currentExp;
		if (exp >= diff)
        {
			LevelUp();
			exp -= diff;
        }
		currentExp += exp;
		onExpGain.Invoke();
	}

	private void LevelUp()
    {
		Debug.Log("leveled up!");
		level += 1;
		currentExp = 0;
		levelExp = Mathf.RoundToInt(Mathf.Pow((float)levelExp, 1.1f));
		skillPoints += 1;

    }

	public override void DealDamage(CharacterStats target, int damage, DmgType type)
    {
		target = (EnemyStats)target;
		base.DealDamage(target, damage, type);
		//PlayerManager.instance.onCombatEnter.Invoke();
	}

	public bool AreHandsFull()
    {
		return handsFull >= maxHands;
    }
	public bool WillHandsBeTooFull(int hands)
    {
		return handsFull + hands > maxHands;
    }

    public override void EnterCombat()
    {
        base.EnterCombat();
		PlayerManager.instance.onCombatEnter.Invoke();
	}
}
