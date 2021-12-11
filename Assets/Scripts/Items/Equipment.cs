using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.ComponentModel;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

/* An Item that can be equipped. */

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item 
{
	public EquipmentSlot equipSlot; // Slot to store equipment in

    public SkinnedMeshRenderer smesh; //actual mesh of the item
    public EquipmentManager.MeshBlendShape[] coveredMeshRegions; //where the item will attach

	public RollTable lt;

	
	//
	public override void Init(Rarity rarity = Rarity.Common)
    {
		//if(this.GetType() == typeof(Weapon)) return;
		base.Init(rarity);
		//Debug.Log("created an equipment");
		//modifiers = new List<Modifier>();
		this.stackSize = 1;
		lt = GameManager.instance.GetComponent<RollTable>();//GameObject.FindGameObjectWithTag("GM").GetComponent<RollTable>();
		//switch for another init based on equipslot?
		List<EquipmentSlot> notWeapons = new List<EquipmentSlot>(Enum.GetValues(typeof(EquipmentSlot)).Cast<EquipmentSlot>());
		//notWeapons.Remove(EquipmentSlot.MainHand);
		//notWeapons.Remove(EquipmentSlot.OffHand);
		//equipSlot = notWeapons[UnityEngine.Random.Range(0, notWeapons.Count)];
		equipSlot = EquipmentSlot.Feet; //for testing


		//set the mesh
		switch (equipSlot)
		{
			case EquipmentSlot.Head:
				name = "Frickin helmet";
				icon = Resources.Load<Sprite>("Sprites/Helmet icon"); //could also depend on rarity?
				description = "yep it's a helmet";
				//WorldObjectPrefab = ItemTable.Helmet;
				//mesh = ItemTable[name].mesh;
				//level; //assigned in base
				coveredMeshRegions = new EquipmentManager.MeshBlendShape[1]; //covers 1 area
				coveredMeshRegions[0] = EquipmentManager.MeshBlendShape.Head; //covers the head
				smesh = Resources.Load<SkinnedMeshRenderer>("Meshes/Helmet");
				break;
			case EquipmentSlot.Belt:

				break;
			case EquipmentSlot.Chest:
				//icon = Resources.Load<Sprite>("Sprites/Chest icon");
				name = "garbo chest";
				description = "poop chest";
				coveredMeshRegions = new EquipmentManager.MeshBlendShape[1]; //covers 1 area
				coveredMeshRegions[0] = EquipmentManager.MeshBlendShape.Torso; //covers the head
				smesh = Resources.Load<SkinnedMeshRenderer>("Meshes/Chest");
				break;
			case EquipmentSlot.Feet:
				name = "garbo feet";
				description = "poop feet";
				coveredMeshRegions = new EquipmentManager.MeshBlendShape[1]; //covers 1 area
				coveredMeshRegions[0] = EquipmentManager.MeshBlendShape.Feet; //covers the head
				smesh = Resources.Load<SkinnedMeshRenderer>("Meshes/Boots");
				break;
			case EquipmentSlot.Hands:
				name = "garbo gloves";
				description = "poop gloves";
				coveredMeshRegions = new EquipmentManager.MeshBlendShape[1]; //covers 1 area
				coveredMeshRegions[0] = EquipmentManager.MeshBlendShape.Hands; //covers the head
				smesh = Resources.Load<SkinnedMeshRenderer>("Meshes/Gloves");
				break;
			case EquipmentSlot.Legs:
				name = "garbo legs";
				description = "poop legs";
				coveredMeshRegions = new EquipmentManager.MeshBlendShape[1]; //covers 1 area
				coveredMeshRegions[0] = EquipmentManager.MeshBlendShape.Legs; //covers the head
				smesh = Resources.Load<SkinnedMeshRenderer>("Meshes/Legs");
				break;
			case EquipmentSlot.Shoulders:
				name = "garbo shoulders";
				description = "poop shoulders";
				coveredMeshRegions = new EquipmentManager.MeshBlendShape[1]; //covers 1 area
				coveredMeshRegions[0] = EquipmentManager.MeshBlendShape.Shoulders; //covers the head
				smesh = Resources.Load<SkinnedMeshRenderer>("Meshes/Shoulders");
				break;
			//case EquipmentSlot.MainHand:
				
				//break;
			//case EquipmentSlot.OffHand:
				//break;
			case EquipmentSlot.Neck:
				break;
			case EquipmentSlot.Ring:
				break;
		}

		SetModifiers();
		GenerateName();
	}

	public override void InitWeapon(Rarity rarity = Rarity.Common)//, WeaponType w = WeaponType.Chainsaw)
    {
		//base.InitWeapon(rarity);
		base.InitWeapon(rarity);//, w);
		//Debug.Log("created a weapon");
		this.stackSize = 1;
		//this.equipSlot = (EquipmentSlot)UnityEngine.Random.Range((int)EquipmentSlot.MainHand, (int)EquipmentSlot.OffHand + 1);
		lt = GameObject.FindGameObjectWithTag("GM").GetComponent<RollTable>();
	}
	protected void SetModifiers() 
	{

		int num_modifiers = SetNumModifiers() - 1;

        if (this.rarity != Rarity.Legendary)
		{
            #region possibility setup
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
			Modifier movespeed = all_secondaries_p[(int)SecondaryStatType.MoveSpeed];
			all_secondaries_p.RemoveAt((int)SecondaryStatType.MoveSpeed);
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
            //define what rolls/stats each item slot can get in each of its slots
            //equipDef[index, [primary, secondary, etc]
            ModType[][] equipDef = new ModType[5][];
			switch (this.equipSlot)
			{
				case EquipmentSlot.Head:
					#region helmet
					//always rolls armor, add directly to modifiers, remove an extra mod roll
					//roll for percent or flat only, remove opposite from allowing to be rolled
					int roll1 = UnityEngine.Random.Range(1, 3);
					if (roll1 == 1)
					{
						//MoveToPossible(modifiers, all_reses, new int[] { (int)DmgType.Physical });
						MoveResTo(modifiers, all_reses, DmgType.Physical);
						all_reses_p.RemoveAll(r => r.dtype == DmgType.Physical);
					}
					else
					{
						MoveResTo(modifiers, all_reses_p, DmgType.Physical);
						all_reses.RemoveAll(r => r.dtype == DmgType.Physical);
					}

					//helmet can roll (in order):
					equipDef[0] = new ModType[] { ModType.Primary }; 
					equipDef[1] = new ModType[] { ModType.Secondary, ModType.Resistance};
					equipDef[2] = new ModType[] { ModType.Primary, ModType.Resistance};
					equipDef[3] = new ModType[] { ModType.Secondary, ModType.Resistance};
					equipDef[4] = new ModType[] { ModType.Damage };

					//add possible modifiers to the list, then depending on rarity will have some more modifiers

					//can roll any primary, percent or flat
					possible_primary.AddRange(all_primaries);
					possible_primary.AddRange(all_primaries_p);

					//can roll a random resistance that isn't armor
					possible_res.AddRange(all_reses);
					possible_res.AddRange(all_reses_p);

					//can roll any secondary stat
					//possible_secondary.AddRange(all_secondaries);
					possible_secondary.AddRange(all_secondaries_p);

					//can only roll % dmg
					possible_dmg.AddRange(all_dmgs_p);

					#endregion
					break;
                case EquipmentSlot.Belt:
                    #region belt
                    //always rolls only health %
                    MovePrimaryTo(modifiers, all_primaries_p, PrimaryStatType.Health);
					all_primaries.RemoveAll(r => r.ptype == PrimaryStatType.Health);

					equipDef[0] = new ModType[] { ModType.Secondary };
					equipDef[1] = new ModType[] { ModType.Secondary };
					equipDef[2] = new ModType[] { ModType.Secondary, ModType.Resistance };
					equipDef[3] = new ModType[] { ModType.Secondary, ModType.Resistance };
					equipDef[4] = new ModType[] { ModType.Secondary, ModType.Resistance };

					possible_res.AddRange(all_reses_p);
					possible_res.AddRange(all_reses);
					possible_secondary.AddRange(all_secondaries_p);
					possible_secondary.Add(movespeed);
					#endregion
					break;
				case EquipmentSlot.Chest:
                    #region chest
                    //always rolls flat high health, but can double dip on %
                    MovePrimaryTo(modifiers, all_primaries, PrimaryStatType.Health);
					modifiers[0].value = Mathf.RoundToInt(modifiers[0].value * 1.3f);

					equipDef[0] = new ModType[] { ModType.Primary };
					equipDef[1] = new ModType[] { ModType.Primary, ModType.Secondary };
					equipDef[2] = new ModType[] { ModType.Secondary, ModType.Primary };
					equipDef[3] = new ModType[] { ModType.Secondary, ModType.Resistance };
					equipDef[4] = new ModType[] { ModType.Resistance };

					possible_primary.AddRange(all_primaries);
					possible_primary.AddRange(all_primaries_p);
					possible_primary.RemoveAll(r => r.ptype == PrimaryStatType.Mana); //except mana
					possible_secondary.AddRange(all_secondaries_p);
					possible_res.AddRange(all_reses);
					possible_res.AddRange(all_reses_p);

                    #endregion
                    break;
				case EquipmentSlot.Feet:
                    #region feet
                    //always rolls movespeed
                    //MoveTo(modifiers, all_secondaries_p, (int)SecondaryStatType.MoveSpeed);
                    modifiers.Add(movespeed);
					equipDef[0] = new ModType[] { ModType.Primary };
					equipDef[1] = new ModType[] { ModType.Secondary };
					equipDef[2] = new ModType[] { ModType.Secondary, ModType.Resistance };
					equipDef[3] = new ModType[] { ModType.Secondary, ModType.Resistance };
					equipDef[4] = new ModType[] { ModType.Resistance };

					MovePrimaryTo(possible_primary, all_primaries, PrimaryStatType.Agility);
					MovePrimaryTo(possible_primary, all_primaries, PrimaryStatType.Strength);
					MovePrimaryTo(possible_primary, all_primaries, PrimaryStatType.Intellect);
					//can roll higher core stat
					foreach (var item in possible_primary)
					{
						item.value = Mathf.RoundToInt(item.value * 1.2f);
					}
					possible_secondary.AddRange(all_secondaries_p);
					possible_res.AddRange(all_reses);
					possible_res.AddRange(all_reses_p);

                    #endregion
                    break;
				case EquipmentSlot.Hands:
                    #region gloves
                    //always rolls atkspeed
                    MoveSecondaryTo(modifiers, all_secondaries_p, SecondaryStatType.AttackSpeed);

					equipDef[0] = new ModType[] { ModType.Secondary };
					equipDef[1] = new ModType[] { ModType.Primary }; //core stat
					equipDef[2] = new ModType[] { ModType.Secondary, ModType.Resistance };
					equipDef[3] = new ModType[] { ModType.Secondary, ModType.Damage };
					equipDef[4] = new ModType[] { ModType.Resistance, ModType.Damage };

					MovePrimaryTo(possible_primary, all_primaries, PrimaryStatType.Agility);
					MovePrimaryTo(possible_primary, all_primaries, PrimaryStatType.Strength);
					MovePrimaryTo(possible_primary, all_primaries, PrimaryStatType.Intellect);
					//can roll higher core stat
                    foreach (var item in possible_primary)
                    {
						item.value = Mathf.RoundToInt(item.value * 1.2f);
                    }

					possible_secondary.AddRange(all_secondaries_p);
					possible_res.AddRange(all_reses);
					possible_res.AddRange(all_reses_p);
					possible_dmg.AddRange(all_dmgs);
					possible_dmg.AddRange(all_dmgs_p);

					#endregion
					break;
				case EquipmentSlot.Legs:
                    #region legs
                    //always rolls one of the core stats but random and can double dip on either flat or percent
                    int roll2 = UnityEngine.Random.Range(1, 4);
					int roll3 = UnityEngine.Random.Range(1, 3);
					if (roll2 == 1)
                    {
						if (roll3 == 1)
						{
							MovePrimaryTo(modifiers, all_primaries, PrimaryStatType.Strength);
						}
						else
							MovePrimaryTo(modifiers, all_primaries_p, PrimaryStatType.Strength);
					}
					else if ( roll2 == 2)
                    {
						if (roll3 == 1)
							MovePrimaryTo(modifiers, all_primaries, PrimaryStatType.Intellect);
						else
							MovePrimaryTo(modifiers, all_primaries_p, PrimaryStatType.Intellect);
					}
                    else
                    {
						if (roll3 == 1)
							MovePrimaryTo(modifiers, all_primaries, PrimaryStatType.Agility);
						else
							MovePrimaryTo(modifiers, all_primaries_p, PrimaryStatType.Agility);
					}

					equipDef[0] = new ModType[] { ModType.Primary };
					equipDef[1] = new ModType[] { ModType.Resistance };
					equipDef[2] = new ModType[] { ModType.Primary, ModType.Resistance };
					equipDef[3] = new ModType[] { ModType.Secondary, ModType.Resistance };
					equipDef[4] = new ModType[] { ModType.Resistance, ModType.Secondary, ModType.Damage };

					possible_primary.AddRange(all_primaries);
					possible_primary.AddRange(all_primaries_p);
					possible_secondary.AddRange(all_secondaries_p);
					possible_res.AddRange(all_reses);
					possible_res.AddRange(all_reses_p);
					possible_dmg.AddRange(all_dmgs);
					possible_dmg.AddRange(all_dmgs_p);

					#endregion
					break;
				case EquipmentSlot.Shoulders:
                    #region shoulders
                    //always rolls cooldown
                    MoveSecondaryTo(modifiers, all_secondaries_p, SecondaryStatType.CooldownReduction);
					equipDef[0] = new ModType[] { ModType.Primary };
					equipDef[1] = new ModType[] { ModType.Secondary, ModType.Primary, ModType.Resistance };
					equipDef[2] = new ModType[] { ModType.Primary, ModType.Resistance };
					equipDef[3] = new ModType[] { ModType.Secondary, ModType.Resistance };
					equipDef[4] = new ModType[] { ModType.Secondary };

					possible_primary.AddRange(all_primaries);
					possible_primary.AddRange(all_primaries_p);
					possible_secondary.AddRange(all_secondaries_p);
					possible_res.AddRange(all_reses);
					possible_res.AddRange(all_reses_p);
                    #endregion
                    break;
				//case EquipmentSlot.MainHand:
					//break;
				//case EquipmentSlot.OffHand:
					//break;
				case EquipmentSlot.Neck:
                    #region neck
                    //always rolls mana but can double dip
                    //MoveTo(modifiers, all_primaries, (int)PrimaryStatType.Mana);
                    int roll4 = UnityEngine.Random.Range(1, 3);
					if (roll4 == 1)
						modifiers.Add(all_primaries.Find(r => r.mtype == ModType.Primary && r.ptype == PrimaryStatType.Mana));
					else
						modifiers.Add(all_primaries_p.Find(r => r.mtype == ModType.Primary && r.ptype == PrimaryStatType.Mana));
					//can roll extra high crit damage
					//all_secondaries_p[(int)SecondaryStatType.CritDamage].value *= 5;//new Modifier(SecondaryStatType.CritDamage, )
					all_secondaries_p.Single(r => r.stype == SecondaryStatType.CritDamage).value *= 5;


					equipDef[0] = new ModType[] { ModType.Primary };
					equipDef[1] = new ModType[] { ModType.Secondary, ModType.Resistance };
					equipDef[2] = new ModType[] { ModType.Secondary, ModType.Resistance };
					equipDef[3] = new ModType[] { ModType.Secondary, ModType.Damage };
					equipDef[4] = new ModType[] { ModType.Damage, ModType.Secondary };

					possible_primary.AddRange(all_primaries);
					possible_primary.AddRange(all_primaries_p);
					possible_primary.RemoveAll(r => r.ptype == PrimaryStatType.Health); //except health

					possible_secondary.AddRange(all_secondaries_p);

					possible_res.AddRange(all_reses);
					possible_res.AddRange(all_reses_p);
					possible_res.RemoveAll(r => r.dtype == DmgType.Physical); // except armor

					possible_dmg.AddRange(all_dmgs);
					possible_dmg.AddRange(all_dmgs_p);


					#endregion
					break;
				case EquipmentSlot.Ring:
                    #region ring
                    //always rolls crit chance
                    MoveSecondaryTo(modifiers, all_secondaries_p, SecondaryStatType.CritChance);

					//ring can roll (in order):
					equipDef[0] = new ModType[] { ModType.Secondary };
					equipDef[1] = new ModType[] { ModType.Secondary, ModType.Primary, ModType.Resistance};
					equipDef[2] = new ModType[] { ModType.Primary, ModType.Resistance };
					equipDef[3] = new ModType[] { ModType.Secondary, ModType.Damage, ModType.Resistance };
					equipDef[4] = new ModType[] { ModType.Damage, ModType.Secondary };

					possible_primary.AddRange(all_primaries);
					possible_primary.AddRange(all_primaries_p);

					//all_secondaries_p.Remove(all_secondaries_p.Single(r => r.stype == SecondaryStatType.MoveSpeed));//except movespeed
					possible_secondary.AddRange(all_secondaries_p); 

					possible_res.AddRange(all_reses);
					possible_res.AddRange(all_reses_p);

					possible_dmg.AddRange(all_dmgs);
					possible_dmg.AddRange(all_dmgs_p);

					#endregion
					break;
            }
            //if (possible_modifiers.Count > 6) Debug.LogWarning("Equipment has too many possible modifiers");
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
            #endregion

            #region assign modifiers
            //assign a modifier for each slot into our modifier list by selecting from the allowed rolls
            for (int i = 0; i < num_modifiers; i++)
            {
				//pick which type from pre-determined list
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
								//modifiers.Add(possible_primary[roll2]);
								MoveTo(modifiers, possible_primary, roll2);
								rolled = true;
								break;
							case ModType.Secondary:
								roll2 = UnityEngine.Random.Range(0, possible_secondary.Count);
								//modifiers.Add(possible_secondary[roll2]);
								MoveTo(modifiers, possible_secondary, roll2);
								rolled = true;
								break;
							case ModType.Resistance:
								roll2 = UnityEngine.Random.Range(0, possible_res.Count);
								//modifiers.Add(possible_res[roll2]);
								MoveTo(modifiers, possible_res, roll2);
								rolled = true;
								break;
							case ModType.Damage:
								roll2 = UnityEngine.Random.Range(0, possible_dmg.Count);
								//modifiers.Add(possible_dmg[roll2]);
								MoveTo(modifiers, possible_dmg, roll2);
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
			//look up which legendary item it spawned
        }

        //increase the values based on the quality of the item, the rarity of the item, and the item level
        foreach (Modifier mod in this.modifiers)
        {
			//if (mod.percentage)
			mod.value = Mathf.RoundToInt((1f + (quality_rarity/100)) * mod.value);
			mod.value = Mathf.RoundToInt((1f + (quality / 100)) * mod.value);
			mod.value = Mathf.RoundToInt(ilevel * mod.value);
		}

		

		//very random items//
		/**
		List<StatType> possible_modifiers1 = new List<StatType>((IEnumerable<StatType>)Enum.GetValues(typeof(StatType)));
		List<DmgType> possible_modifiers2 = new List<DmgType>((IEnumerable<DmgType>)Enum.GetValues(typeof(DmgType)));
		List<DmgType> possible_modifiers3 = new List<DmgType>((IEnumerable<DmgType>)Enum.GetValues(typeof(DmgType)));

		for (int i = 1; i <= num_modifiers; i++)
		{
			//pick which "bucket" of stat to modify
			int which_modifier = Random.Range(1, 3); //1 = stat, 2 = dmg, 3 = res
			switch (which_modifier)
			{
				case 1:
					//pick which stat to modify out of the remaining possible modifiers
					int j = Random.Range(0, possible_modifiers1.Count);
					//create a new statmodifier using the table to roll min and max of that stat (possibly modify the number here based on rarity or difficulty?)
					int st = Random.Range(Stat.StatRoll[(StatType)j].Item1, Stat.StatRoll[(StatType)j].Item2);
					statModifiers.Add(new StatModifier(possible_modifiers1[j], st));
					//remove the possibility of rolling this stat again
					possible_modifiers1.RemoveAt(j);
					break;
				case 2:
					int k = Random.Range(0, possible_modifiers2.Count);
					int dm = Random.Range(Stat.DmgRoll[(DmgType)k].Item1, Stat.DmgRoll[(DmgType)k].Item2);
					dmgModifiers.Add(new DmgModifier(possible_modifiers2[k], dm));
					possible_modifiers1.RemoveAt(k);
					break;
				case 3:
					int l = Random.Range(0, possible_modifiers3.Count);
					int re = Random.Range(Stat.ResRoll[(DmgType)l].Item1, Stat.ResRoll[(DmgType)l].Item2);
					resModifiers.Add(new ResModifier(possible_modifiers3[l], re));
					possible_modifiers1.RemoveAt(l);
					break;
			}
		}
		*/
	}

	protected int SetNumModifiers()
    {
		//rarity affects number of modifiers, quality?,
		int num_modifiers = 1;
		switch (this.rarity)
		{
			//1 modifiers?
			case Rarity.Common:
				//num_modifiers = UnityEngine.Random.Range(1, 3);
				break;
			//2?
			case Rarity.Uncommon:
				//num_modifiers = UnityEngine.Random.Range(2, 4);
				quality_rarity = 10f;
				num_modifiers = 2;
				break;
			//3-4?
			case Rarity.Rare:
				quality_rarity = 50f;
				num_modifiers = UnityEngine.Random.Range(3, 5);
				break;
			//5-6?
			case Rarity.Epic:
				quality_rarity = 80f;
				num_modifiers = UnityEngine.Random.Range(5, 7);
				break;
			//fixed?
			case Rarity.Legendary:
				quality_rarity = 60f;
				num_modifiers = 6;
				break;
		}
		/*
		//get a new modifier list for each extra mod rolled
		for (int i = 0; i < num_modifiers; i++)
		{
			//this.modifiers[i] = new List<Modifier>();
			this.possible_modifiers.Add(new List<Modifier>());
		}*/
		return num_modifiers;
	}

    #region Modifier Movers
    public void MoveTo(List<Modifier> addTo, List<Modifier> from, int i)
    {
		addTo.Add(from[i]);
		from.RemoveAt(i);
	}
	public void MoveResTo(List<Modifier> addTo, List<Modifier> from, DmgType d)
    {
		
		Modifier m = from.Find(r => r.mtype == ModType.Resistance && r.dtype == d);
		addTo.Add(m);
		from.Remove(m);
    }
	public void MoveDmgTo(List<Modifier> addTo, List<Modifier> from, DmgType d)
	{
		Modifier m = from.Find(r => r.mtype == ModType.Damage && r.dtype == d);
		addTo.Add(m);
		from.Remove(m);
	}
	public void MovePrimaryTo(List<Modifier> addTo, List<Modifier> from, PrimaryStatType d)
	{
		Modifier m = from.Find(r => r.mtype == ModType.Primary && r.ptype == d);
		addTo.Add(m);
		from.Remove(m);
	}
	public void MoveSecondaryTo(List<Modifier> addTo, List<Modifier> from, SecondaryStatType d)
	{
		Modifier m = from.Find(r => r.mtype == ModType.Secondary && r.stype == d);
		addTo.Add(m);
		from.Remove(m);
	}
    #endregion

    //create a name based on the mods
    public virtual void GenerateName()
    {
		//name = this.equipSlot.GetDescrip();//this.equipSlot.ToString();//"Helmet";
		name = equipSlot.GetDescription();//GetDescription(equipSlot);
        for (int i = 1; i < this.modifiers.Count; i++) //skip first modifier since that is innate
        {
			//look up the prefix/suffix based on modifiers?
        }
    }

    // When pressed in inventory
    public override bool Use(int index, int equipIndex)
	{
		base.Use(index);
		//Debug.Log("used an equipment");
		EquipmentManager.instance.Equip(this, index, equipIndex);
		return true;
	}


	public interface ILegendary// : Equipment
    {
		//public string legendaryName;
		//public LegendaryEffect legendaryEffect;

		public abstract void Init(string legendaryName = "Legendary Equipment", LegendaryEffect effect = null);
        //{
			//base.Init(Rarity.Legendary);
			//this.legendaryName = legendaryName;
			//this.legendaryEffect = effect;
        //}

		public abstract class LegendaryEffect : ScriptableObject
		{
			public abstract string description { get; protected set; }
			//return true if could be used, false otherwise.
			public abstract void Effect(PlayerStats player);
		}
	}
}

public enum EquipmentSlot 
{	
	[Description("Helmet")]
	Head, 
	[Description("Shoulder Pads")]
	Shoulders, 
	[Description("Chassis")]
	Chest,
	[Description("Gloves")]
	Hands,
	Legs, 
	 
	[Description("Boots")]
	Feet, 
	[Description("Enhancement")]
	Ring, 
	[Description("Augment")]
	Neck,
	Belt,
	Weapon
	//MainHand, 
	//OffHand, 
}

public static class EquipmentSlotExtension
{
	public static string GetDescription(this EquipmentSlot value)
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
}


