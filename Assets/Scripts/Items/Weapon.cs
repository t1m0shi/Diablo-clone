using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class Weapon : Equipment
{
    public WeaponDamage damage;
	//below set in individual class init
    public WeaponType wtype;
    public int handed;
	public WeaponMode wmode;
	//public Mesh mesh; //replaces the actual mesh with gameobject so I can have lights on it and stuff easily
	public GameObject representation;
	[HideInInspector]
	public Transform grip;


    #region Random Generation
    /*
    public override void InitWeapon(DmgType dtype, int min, int max, Rarity rarity=Rarity.Common)
    {
		Debug.Log("Creating a weapon");
        base.Init(rarity);
        this.damage = new WeaponDamage(dtype, min, max);
		//this.wtype = wtype;
		/**
		switch (wtype)
        {
			case WeaponType.Sword1h:
				this.hands = 1;
				break;
			case WeaponType.Sword2h:
				this.hands = 2;
				break;
			case WeaponType.Gun1h:
				this.hands = 1;
				break;
			case WeaponType.Gun2h:
				this.hands = 2;
				break;
			case WeaponType.Dagger:
				this.hands = 1;
				break;
			case WeaponType.Chainsaw:
				this.hands = 2;
				break;
			case WeaponType.Shield:
				this.hands = 1;
				break;
					
        }
	}
		*/


    public override void InitWeapon(Rarity rarity = Rarity.Epic)//, WeaponType wtyp = WeaponType.Sword2h)
    {
		Debug.Log("Creating a weapon by rarity");
		base.InitWeapon(rarity);//, wtyp);
		this.equipSlot = EquipmentSlot.Weapon;
		//random slot
		handed = UnityEngine.Random.Range(1, 3);
		if (handed == 2)
        {
			//this.equipSlot = EquipmentSlot.MainHand; //2handed always mainhand
			this.wtype = (WeaponType)UnityEngine.Random.Range((int)WeaponType.Sword2h, Enum.GetValues(typeof(WeaponType)).Length);
		}
        else
        {
			//this.equipSlot = (EquipmentSlot)UnityEngine.Random.Range((int)EquipmentSlot.MainHand, (int)EquipmentSlot.OffHand + 1);
			this.wtype = (WeaponType)UnityEngine.Random.Range(0, (int)WeaponType.Sword2h);
		}
		//WeaponType wslot = (WeaponType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(WeaponType)).Length);
		//this.wtype = wtyp;
		string path = "Assets/Resources/Prefabs/Weapons/" + wtype.ToString() + "/" + rarity.ToString();
		DirectoryInfo d = new DirectoryInfo(path);
		int r = UnityEngine.Random.Range(0, d.GetFiles().Length);
		representation = Resources.Load<GameObject>(path +"/"+r.ToString());
        

		SetModifiers();
		GenerateName();
	}

    protected new void SetModifiers()
    {
		int num_modifiers = base.SetNumModifiers() - 1;

		if (this.rarity != Rarity.Legendary)
		{
            #region possiblility setup
            //how many primary/secondary stats an item can have at maximum/optimal rarity roll, depends on equipSlot type
            //they don't need to include all, and they don't all have to be primary/secondary


            //Modifier hp, mana, armor, strength, agility, intelligence, critchance, critdmg, atkspd, movespd, cooldown, fireres, coldres, voidres;
            List<PrimaryStatType> plist = Enum.GetValues(typeof(PrimaryStatType)).Cast<PrimaryStatType>().ToList();
			List<SecondaryStatType> slist = Enum.GetValues(typeof(SecondaryStatType)).Cast<SecondaryStatType>().ToList();
			List<DmgType> reslist = Enum.GetValues(typeof(DmgType)).Cast<DmgType>().ToList();
			List<DmgType> dmglist = Enum.GetValues(typeof(DmgType)).Cast<DmgType>().ToList();

			//make modifier lists to pull from
			//generic list of the primaries (flat and percentage)
			List<Modifier> all_primaries = new List<Modifier>();
			List<Modifier> all_primaries_p = new List<Modifier>();
			foreach (var p in plist)
			{
				all_primaries.Add(new Modifier(p, lt.Roll(p.ToString())));
				all_primaries_p.Add(new Modifier(p, lt.Roll(p.ToString(), true), true));
			}
			//generic list of all secondaries
			//List<Modifier> all_secondaries = new List<Modifier>();
			List<Modifier> all_secondaries_p = new List<Modifier>();
			foreach (var s in slist)
			{
				//all_secondaries.Add(new Modifier(s, lt.Roll(s.ToString())));
				all_secondaries_p.Add(new Modifier(s, lt.Roll(s.ToString(), true), true));
			}
			//generic list of all resistances
			List<Modifier> all_reses = new List<Modifier>();
			List<Modifier> all_reses_p = new List<Modifier>();
			foreach (var r in reslist)
			{
				all_reses.Add(new Modifier(r, lt.Roll(r.ToString() + "Res"), ModType.Resistance));
				all_reses_p.Add(new Modifier(r, lt.Roll(r.ToString() + "Res", true), ModType.Resistance, true));
			}
			//generic list of all damages
			List<Modifier> all_dmgs = new List<Modifier>();
			List<Modifier> all_dmgs_p = new List<Modifier>();
			foreach (var d in dmglist)
			{
				all_dmgs.Add(new Modifier(d, lt.Roll(d.ToString() + "Dmg"), ModType.Damage));
				all_dmgs_p.Add(new Modifier(d, lt.Roll(d.ToString() + "Dmg", true), ModType.Damage, true));
			}

			//any possible mix of different stats that I want to assign to each equip type
			List<Modifier> possible_primary = new List<Modifier>();
			List<Modifier> possible_secondary = new List<Modifier>();
			List<Modifier> possible_dmg = new List<Modifier>();
			List<Modifier> possible_res = new List<Modifier>();
            #endregion

            #region define allowed rolls
            
			//all weapons except shield get a WeaponDmg (dmg type, atk speed, min, max)
			if (this.wtype != WeaponType.Shield)
            {
				//random damage roll
				DmgType d = (DmgType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(DmgType)).Length);
				int min = lt.Roll(wtype + "WeaponMin");
				int max = lt.Roll(wtype + "WeaponMax");
				//speed is added onto player's attack speed to get more attacks per second
				float speed;
				if (this.handed ==1)
					speed = UnityEngine.Random.Range(1f, 2f); //* ilvl?
				else
					speed = UnityEngine.Random.Range(0f, 1f);
				this.damage = new WeaponDamage(d, speed, min, max);
			}

			//define what rolls/stats each item slot can get in each of its slots
			//equipDef[index, [primary, secondary, etc]
			ModType[][] equipDef = new ModType[5][];
			if (this.wtype == WeaponType.Shield)
            {
				this.wmode = WeaponMode.Blocking;
				//this.handed = 1;
				#region shield
				int blockMin = 10 * ilevel; // or something?
				int blockMax = 2 * blockMin;
				Modifier blockChance = new Modifier(ModType.Block, UnityEngine.Random.Range(blockMin, blockMax + 1));
				modifiers.Add(blockChance);
				equipDef[0] = new ModType[] { ModType.Primary };
				equipDef[1] = new ModType[] { ModType.Secondary, ModType.Resistance };
				equipDef[2] = new ModType[] { ModType.Secondary, ModType.Resistance, ModType.Primary };
				equipDef[3] = new ModType[] { ModType.Secondary, ModType.Resistance };
				equipDef[4] = new ModType[] { ModType.Primary, ModType.Resistance };

				MoveTo(possible_secondary, all_secondaries_p, (int)SecondaryStatType.CooldownReduction);
				MoveTo(possible_secondary, all_secondaries_p, (int)SecondaryStatType.CritChance);
				possible_primary.AddRange(all_primaries);
				possible_primary.AddRange(all_primaries_p);
				possible_res.AddRange(all_reses);
				possible_res.AddRange(all_reses_p);
				foreach (var item in possible_res)
				{
					item.value = Mathf.RoundToInt(item.value * 1.5f);
				}

				#endregion
			}
            else
            {

				switch (this.wtype)
				{
					case WeaponType.Sword1h:
					case WeaponType.Dagger:
					case WeaponType.Sword2h:
					//case WeaponType.Chainsaw:
						//this.handed = 1;
						this.wmode = WeaponMode.Melee;
						break;
					case WeaponType.Gun1h:
					case WeaponType.Gun2h:
						//this.handed = 1;
						this.wmode = WeaponMode.Ranged;
						break;
					/*
						//this.handed = 2;
						this.wmode = WeaponMode.Ranged;
						break;
					
						//this.handed = 2;
						this.wmode = WeaponMode.Melee;
						break;
					*/
				}
				if (handed == 1)
				{
					// can roll (in order):
					equipDef[0] = new ModType[] { ModType.Primary, ModType.Secondary };
					equipDef[1] = new ModType[] { ModType.Secondary };
					equipDef[2] = new ModType[] { ModType.Primary };
					equipDef[3] = new ModType[] { ModType.Secondary};
					equipDef[4] = new ModType[] { ModType.Damage, ModType.Secondary };

					possible_primary.AddRange(all_primaries);
					possible_primary.AddRange(all_primaries_p);
					possible_primary.RemoveAll(r => r.ptype == PrimaryStatType.Health || r.ptype == PrimaryStatType.Mana); //core stats
					possible_secondary.AddRange(all_secondaries_p);
					possible_dmg.AddRange(all_dmgs_p);
				}
                else
                {
					// can roll (in order):
					equipDef[0] = new ModType[] { ModType.Primary, ModType.Secondary };
					equipDef[1] = new ModType[] { ModType.Secondary };
					equipDef[2] = new ModType[] { ModType.Primary };
					equipDef[3] = new ModType[] { ModType.Secondary, ModType.Primary };
					equipDef[4] = new ModType[] { ModType.Damage, ModType.Secondary };

					possible_primary.AddRange(all_primaries);
					possible_primary.AddRange(all_primaries_p);
					possible_primary.RemoveAll(r => r.ptype == PrimaryStatType.Mana); //core stats and hp
					possible_secondary.AddRange(all_secondaries_p);
					possible_dmg.AddRange(all_dmgs_p);
				}
			}
			
			//if (possible_modifiers.Count > 6) Debug.LogWarning("Equipment has too many possible modifiers");
			#endregion

			#region assign modifiers
			//assign a modifier for each slot into our modifier list by selecting from the allowed rolls
			for (int i = 0; i < num_modifiers; i++)
			{
				//pick which type
				int roll = UnityEngine.Random.Range(0, equipDef[i].Length);
				ModType t = equipDef[i][roll];
				//grab a random stat from the allowed list
				bool rolled = false;
				while (!rolled)
				{
					try
					{
						int roll2;
						switch (t)
						{
							case ModType.Primary:
								roll2 = UnityEngine.Random.Range(0, possible_primary.Count);
								modifiers.Add(possible_primary[roll2]);
								rolled = true;
								break;
							case ModType.Secondary:
								roll2 = UnityEngine.Random.Range(0, possible_secondary.Count);
								modifiers.Add(possible_secondary[roll2]);
								rolled = true;
								break;
							case ModType.Resistance:
								roll2 = UnityEngine.Random.Range(0, possible_res.Count);
								modifiers.Add(possible_res[roll2]);
								rolled = true;
								break;
							case ModType.Damage:
								roll2 = UnityEngine.Random.Range(0, possible_dmg.Count);
								modifiers.Add(possible_dmg[roll2]);
								rolled = true;
								break;
						}
					}
                    catch (IndexOutOfRangeException e)
                    {
						rolled = false;
                    }
				}
			}
			#endregion

			#region set up possible_modifiers for crafting
			//for each slot
			for (int i = 0; i < equipDef.Length; i++)
			{
				possible_modifiers[i] = new List<Modifier>();
				//for each possible type
				for (int j = 0; j < equipDef[i].Length; j++)
				{
					//check what I'm allowed to roll for that type
					switch (equipDef[i][j])
					{
						case ModType.Primary:
							foreach (var item in possible_primary)
							{
								possible_modifiers[i].Add(new Modifier(item.ptype, 0, item.percentage)); //maybe change 0 to be the range?
							}
							break;
						case ModType.Secondary:
							foreach (var item in possible_secondary)
							{
								possible_modifiers[i].Add(new Modifier(item.ptype, 0, item.percentage)); //maybe change 0 to be the range?
							}
							break;
						case ModType.Resistance:
							foreach (var item in possible_res)
							{
								possible_modifiers[i].Add(new Modifier(item.ptype, 0, item.percentage)); //maybe change 0 to be the range?
							}
							break;
						case ModType.Damage:
							foreach (var item in possible_dmg)
							{
								possible_modifiers[i].Add(new Modifier(item.ptype, 0, item.percentage)); //maybe change 0 to be the range?
							}
							break;
					}
				}
			}
			#endregion
			/** old
			//roll the modifiers
			while (num_modifiers > 0)
			{
				num_modifiers -= 1;
				int which = UnityEngine.Random.Range(1, 3);
				if (max_num_primary > 0 && possible_primary.Count > 0)
				{
					if ((which == 1) || (max_num_secondary == 0))// && which == 2))
					{
						int i = UnityEngine.Random.Range(0, possible_primary.Count);
						possible_modifiers.Add(possible_primary[i]);
						possible_primary.RemoveAt(i);
						max_num_primary -= 1;
					}
				}
				if (max_num_secondary > 0 && possible_secondary.Count > 0)
				{
					if (which == 2 || (max_num_primary == 0))// && which == 1))
					{
						int i = UnityEngine.Random.Range(0, possible_secondary.Count);
						possible_modifiers.Add(possible_secondary[i]);
						possible_secondary.RemoveAt(i);
						max_num_secondary -= 1;
					}
				}
				if (possible_primary.Count == 0 && possible_secondary.Count == 0) break;
			}
			*/
		}
		else
        {

        }


		//increase the values based on the quality of the item, the rarity of the item, and the item level

		foreach (Modifier mod in this.modifiers)
		{
			//if (mod.percentage)
			mod.value = Mathf.RoundToInt((1f + (quality_rarity / 100)) * mod.value);
			mod.value = Mathf.RoundToInt((1f + (quality / 100)) * mod.value);
			mod.value = Mathf.RoundToInt(ilevel * mod.value);
		}
	}

    public override void GenerateName()
    {
		name = GetDescription(wtype);
		for (int i = 1; i < this.modifiers.Count; i++) //skip first modifier since that is innate
		{
			//look up the prefix/suffix based on modifiers?
		}
	}

    public virtual string GetDescription(WeaponType value)
    {
		Type type = value.GetType();
		string name = Enum.GetName(type, value);
		if (name != null)
		{
			FieldInfo field = type.GetField(name);
			if (field != null)
			{
				DescriptionAttribute attr =
					   Attribute.GetCustomAttribute(field,
						 typeof(DescriptionAttribute)) as DescriptionAttribute;
				if (attr != null)
				{
					return attr.Description;
				}
			}
		}
		return null;
	}
    #endregion

    public override bool Use(int index)
    {
		return EquipmentManager.instance.Equip(this, index);
		//return true;
    }
}

public enum WeaponMode { Ranged, Melee, Blocking }
public enum WeaponType
{ //1 handeds first, 2 handeds last
	[Description("1-Handed Gun")] Gun1h,
	[Description("1-Handed Sword")] Sword1h,
	Shield, Dagger,
	[Description("2-Handed Sword")] Sword2h,
	[Description("2-Handed Gun")] Gun2h
	//Chainsaw}
}

[Serializable]
public class WeaponDamage
{
    public DmgType type;
	public float attacksPerSecond;
    public int min;
    public int max;
	
    public WeaponDamage(DmgType type, float speed, int min, int max)
    {
        this.type = type;
		this.attacksPerSecond = speed;
        this.min = min;
        this.max = max;
    }
}

/*
#if UNITY_EDITOR
[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
		
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Weapon.wmode)));
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Weapon.wtype)));
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Weapon.handed)));
		//EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Weapon.wmode)));
	}
}
[CustomEditor(typeof(WeaponDamage))]
public class WeaponDamageEditor : WeaponEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(WeaponDamage.type)));
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(WeaponDamage.min)));
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(WeaponDamage.max)));
	}
}
#endif
*/
