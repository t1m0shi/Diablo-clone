using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Modifier
{
	public ModType mtype;
	public int value;
	public PrimaryStatType ptype;
	public SecondaryStatType stype;
	public DmgType dtype;
	public bool percentage;// = false;

	public Modifier(int value, bool percent = false) 
	{
		this.percentage = percent;
		this.value = value;
	}
	public Modifier(ModType m, int value) : this(value)
	{
		if (m == ModType.Block) this.percentage = true;
		this.mtype = m;
	}
	public Modifier(PrimaryStatType type, int value, bool percent = false) : this(value, percent)
	{
		this.mtype = ModType.Primary;
		this.ptype = type;
	}
	public Modifier(SecondaryStatType type, int value, bool percent = false) : this(value, percent)
	{
		this.mtype = ModType.Secondary;
		this.stype = type;
	}
	public Modifier(DmgType type, int value, ModType mtype, bool percent = false) : this(value, percent)
	{
		this.mtype = mtype;
		this.dtype = type;
	}

	public override string ToString()
	{
		if (percentage)
		{
			switch (mtype)
			{
				case ModType.Primary:
					return value.ToString() + "% " + Enum.GetName(typeof(PrimaryStatType), ptype);
				case ModType.Secondary:
					return value.ToString() + "% " + Enum.GetName(typeof(SecondaryStatType), stype);
				case ModType.Damage:
					return value + "% " + Enum.GetName(typeof(DmgType), dtype) + " Damage";
				case ModType.Resistance:
					if (dtype == DmgType.Physical) return value + "% Armor";
					else return value + "% " + Enum.GetName(typeof(DmgType), dtype) + " Resistance";
				default:
					return "";
			}
		}
		else
		{
			switch (mtype)
			{
				case ModType.Primary:
					return value.ToString() + " " + Enum.GetName(typeof(PrimaryStatType), ptype);
				case ModType.Secondary:
					return value.ToString() + " " + Enum.GetName(typeof(SecondaryStatType), stype);
				case ModType.Damage:
					return value + " " + Enum.GetName(typeof(DmgType), dtype) + " Damage";
				case ModType.Resistance:
					if (dtype == DmgType.Physical) return value + " Armor";
					else return value + " " + Enum.GetName(typeof(DmgType), dtype) + " Resistance";
				default:
					return "";
			}
		}
	}
}

public enum ModType { Primary, Secondary, Resistance, Damage, Block }
//public enum ModMode { Flat, Percentage }
