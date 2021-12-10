using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class determines what items can spawn in a level
//Items dropped should be based on difficulty, player level, quality of enemy?
public class RollTable : MonoBehaviour
{
	public static RollTable instance;
	//need list of items (or randomly generate them) and a chance for that item to drop
	//so maybe just a chance per rarity, then it creates an item of that rarity
	private Dictionary<string, int> lootTable_flats;
	private Dictionary<string, int> lootTable_pcts;
	private int difficulty;
	private bool initialized = false;

    private void Awake()
    {
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of Roll Table found!");
			return;
		}
		
		instance = this;
		//lootTable = new Dictionary<string, int>();
		//difficulty = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>().Difficulty;
		Init();
	}

	private void Init()
    {
		difficulty = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>().DifficultyTier;
		//difficulty = 1;
		lootTable_flats = new Dictionary<string, int>();
		lootTable_pcts = new Dictionary<string, int>();
		//System.Random r = new System.Random();
		foreach (var item in PrimaryStatRoll_flat)
		{
			lootTable_flats[item.Key.ToString()] = UnityEngine.Random.Range(item.Value.Item1, item.Value.Item2+1) * difficulty; //maybe only multiply item2 for more rng
		}
		/*
		foreach (var item in SecondaryStatRoll_flat)
		{
			lootTable_flats[item.Key.ToString()] = UnityEngine.Random.Range(item.Value.Item1, item.Value.Item2 + 1) * difficulty; //maybe only multiply item2 for more rng
		}*/
		foreach (var item in DmgRoll_flat)
		{
			lootTable_flats[item.Key.ToString() + "Dmg"] = UnityEngine.Random.Range(item.Value.Item1, item.Value.Item2+1) * difficulty;
		}
		foreach (var item in ResRoll_flat)
		{
			lootTable_flats[item.Key.ToString() + "Res"] = UnityEngine.Random.Range(item.Value.Item1, item.Value.Item2+1) * difficulty;
		}

		foreach (var item in PrimaryStatRoll_pct)
		{
			lootTable_pcts[item.Key.ToString()] = UnityEngine.Random.Range(item.Value.Item1, item.Value.Item2 + 1);// * difficulty; //maybe only multiply item2 for more rng
		}			  
		foreach (var item in SecondaryStatRoll_pct)
		{
			lootTable_pcts[item.Key.ToString()] = UnityEngine.Random.Range(item.Value.Item1, item.Value.Item2 + 1);// * difficulty; //maybe only multiply item2 for more rng
		}			  
		foreach (var item in DmgRoll_pct)
		{
			lootTable_pcts[item.Key.ToString() + "Dmg"] = UnityEngine.Random.Range(item.Value.Item1, item.Value.Item2 + 1);// * difficulty;
		}			  
		foreach (var item in ResRoll_pct)
		{
			lootTable_pcts[item.Key.ToString() + "Res"] = UnityEngine.Random.Range(item.Value.Item1, item.Value.Item2 + 1);// * difficulty;
		}
		foreach (var item in WeaponDamageRoll)
		{
			lootTable_flats[item.Key.ToString() + "WeaponMin"] = UnityEngine.Random.Range(item.Value.Item1.Item1, item.Value.Item1.Item2 + 1);// * difficulty;
			lootTable_flats[item.Key.ToString() + "WeaponMax"] = UnityEngine.Random.Range(item.Value.Item2.Item1, item.Value.Item2.Item2 + 1);
		}
	}

    #region Tables
    //tables of stat numbers that the Equipment can roll from Tuple(min,max)
    public static Dictionary<PrimaryStatType, Tuple<int, int>> PrimaryStatRoll_flat = new Dictionary<PrimaryStatType, Tuple<int, int>>()
	{
		{ PrimaryStatType.Health, Tuple.Create(1,10) },
		{ PrimaryStatType.Mana, Tuple.Create(1,10) },
		{ PrimaryStatType.Strength, Tuple.Create(1,10) },
		{ PrimaryStatType.Agility, Tuple.Create(1,10) },
		{ PrimaryStatType.Intellect, Tuple.Create(1,10) }
	};
	/*
	public static Dictionary<SecondaryStatType, Tuple<int, int>> SecondaryStatRoll_flat = new Dictionary<SecondaryStatType, Tuple<int, int>>()
	{
		{ SecondaryStatType.CritChance, Tuple.Create(1,10) },
		{ SecondaryStatType.CritDamage, Tuple.Create(1,10) },
		{ SecondaryStatType.CooldownReduction, Tuple.Create(1,10) },
		{ SecondaryStatType.MoveSpeed, Tuple.Create(1,10) },
		{ SecondaryStatType.AttackSpeed, Tuple.Create(1,10) }
	};*/

	public static Dictionary<DmgType, Tuple<int, int>> DmgRoll_flat = new Dictionary<DmgType, Tuple<int, int>>()
	{
		{ DmgType.Physical, Tuple.Create(10,100) },
		{ DmgType.Cold, Tuple.Create(10,100) },
		{ DmgType.Fire, Tuple.Create(10,100) },
		{ DmgType.Void, Tuple.Create(10,100) }
	};

	public static Dictionary<DmgType, Tuple<int, int>> ResRoll_flat = new Dictionary<DmgType, Tuple<int, int>>()
	{
		{ DmgType.Physical, Tuple.Create(1,10) },
		{ DmgType.Cold, Tuple.Create(1,10) },
		{ DmgType.Fire, Tuple.Create(1,10) },
		{ DmgType.Void, Tuple.Create(1,10) }
	};

	//tables of stat numbers that the Equipment can roll from Tuple(min,max)
	public static Dictionary<PrimaryStatType, Tuple<int, int>> PrimaryStatRoll_pct = new Dictionary<PrimaryStatType, Tuple<int, int>>()
	{
		{ PrimaryStatType.Health, Tuple.Create(1,10) },
		{ PrimaryStatType.Mana, Tuple.Create(1,10) },
		{ PrimaryStatType.Strength, Tuple.Create(1,10) },
		{ PrimaryStatType.Agility, Tuple.Create(1,10) },
		{ PrimaryStatType.Intellect, Tuple.Create(1,10) }
	};
	public static Dictionary<SecondaryStatType, Tuple<int, int>> SecondaryStatRoll_pct = new Dictionary<SecondaryStatType, Tuple<int, int>>()
	{
		{ SecondaryStatType.CritChance, Tuple.Create(1,10) },
		{ SecondaryStatType.CritDamage, Tuple.Create(1,10) },
		{ SecondaryStatType.CooldownReduction, Tuple.Create(1,10) },
		{ SecondaryStatType.MoveSpeed, Tuple.Create(1,10) },
		{ SecondaryStatType.AttackSpeed, Tuple.Create(1,10) }
	};

	public static Dictionary<DmgType, Tuple<int, int>> DmgRoll_pct = new Dictionary<DmgType, Tuple<int, int>>()
	{
		{ DmgType.Physical, Tuple.Create(1,10) },
		{ DmgType.Cold, Tuple.Create(1,10) },
		{ DmgType.Fire, Tuple.Create(1,10) },
		{ DmgType.Void, Tuple.Create(1,10) }
	};

	public static Dictionary<DmgType, Tuple<int, int>> ResRoll_pct = new Dictionary<DmgType, Tuple<int, int>>()
	{
		{ DmgType.Physical, Tuple.Create(1,10) },
		{ DmgType.Cold, Tuple.Create(1,10) },
		{ DmgType.Fire, Tuple.Create(1,10) },
		{ DmgType.Void, Tuple.Create(1,10) }
	};

	public static Dictionary<WeaponType, Tuple<Tuple<int, int>, Tuple<int, int>>> WeaponDamageRoll = new Dictionary<WeaponType, Tuple<Tuple<int, int>, Tuple<int, int>>>()
	{
		//{ WeaponType.Chainsaw, Tuple.Create(Tuple.Create(100,200), Tuple.Create(200,300)) },
		{ WeaponType.Dagger, Tuple.Create(Tuple.Create(50,100), Tuple.Create(100,150)) },
		{ WeaponType.Gun1h, Tuple.Create(Tuple.Create(50,100), Tuple.Create(100,150)) },
		{ WeaponType.Gun2h, Tuple.Create(Tuple.Create(100,200), Tuple.Create(200,300)) },
		{ WeaponType.Sword1h, Tuple.Create(Tuple.Create(50,150), Tuple.Create(150,250)) },
		{ WeaponType.Sword2h, Tuple.Create(Tuple.Create(100,200), Tuple.Create(200,300)) }
	};


	#endregion

	public int Roll(string toRoll, bool percentage=false)
    {
		if (!initialized) Init();
		if (percentage)
			return lootTable_pcts[toRoll];
		else
			return lootTable_flats[toRoll];
    }

}
