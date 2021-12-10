using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

/* Class used for all stats where we want to be able to add/remove modifiers */

[System.Serializable]
public class Stat {

	[SerializeField]
	public float baseValue;  // Starting value
	
	public ModType type;

	// List of modifiers that change the baseValue
	//only used for player
	public List<int> flatModifiers { get; private set; } = new List<int>(); 
	public List<float> percentModifiers { get; private set; } = new List<float>();


	public Stat() { }
	public Stat(int amt)
    {
		baseValue = amt;
    }
	public Stat(int amt, List<int> fmods, List<float> pmods)
	{
		baseValue = amt;
		flatModifiers = fmods;
		percentModifiers = pmods;
	}
	// Get the final value after applying modifiers
	public float GetValue ()
	{
		float finalValue = baseValue;
		flatModifiers.ForEach(x => finalValue += x);
		float percents = 0f;
		percentModifiers.ForEach(x => percents += x);
		finalValue = finalValue * (1f + percents/100f);
		return finalValue;
	}

	public float GetChance()
    {
		float final = baseValue;
		percentModifiers.ForEach(x => final += x);
		return final;
    }
	/*
	public int GetBonus()
    {

    }*/

	// Add new modifier
	public void AddModifier (Modifier modifier)
	{
		if (!modifier.percentage)
			flatModifiers.Add(modifier.value);
		else
			percentModifiers.Add((float)modifier.value);// / 100f);
		PlayerStats.instance.onStatsChanged.Invoke();
	}

	// Remove a modifier
	public void RemoveModifier (Modifier modifier)
	{
		if (!modifier.percentage)
			flatModifiers.Remove(modifier.value);
		else
			percentModifiers.Remove((float)modifier.value);// / 100f);
		PlayerStats.instance.onStatsChanged.Invoke();
	}

    public override string ToString()
    {
		return this.GetValue().ToString();
    }

	
}

public enum PrimaryStatType { Health, Mana, Strength, Intellect, Agility }
public enum SecondaryStatType { CritChance, CritDamage, CooldownReduction, AttackSpeed, MoveSpeed }
public enum DmgType { [Description("Armor")]Physical, Fire, Cold, Void } //True

public static class DmgTypeExtension
{
	public static string GetDescription(this DmgType value)
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