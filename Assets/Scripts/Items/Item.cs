#if UNITY_EDITOR
using System.Collections.Generic;
using System;
using UnityEditor;
#endif
using UnityEngine;

/* The base item class. All items should derive from this. */

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject {

	new public string name = "New Item";    // Name of the item
	public Rarity rarity;
	public Sprite icon;// = null;				// Item icon
	public int stackSize = int.MaxValue;
	public string description;
	//public GameObject WorldObjectPrefab;
	public int ilevel = 1;
	public float quality_rarity = 0f; //% more than normal
	public float quality = 0f;

	[SerializeField]
	protected List<Modifier>[] possible_modifiers; //used for crafting later
	public List<Modifier> modifiers; //used for item generation

	public virtual void Init(Rarity rarity=Rarity.Common) { 
		//Debug.Log("created an item");
		//rarity = (Rarity)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Rarity)).Length);
		icon = Resources.Load("Sprites/Placeholder01") as Sprite;
		possible_modifiers = new List<Modifier>[6];
		modifiers = new List<Modifier>();
		this.rarity = rarity;
	}
	public virtual void InitWeapon(Rarity rarity = Rarity.Epic)//, WeaponType w = WeaponType.Chainsaw)
    {
		icon = Resources.Load("Sprites/Placeholder01") as Sprite;
		possible_modifiers = new List<Modifier>[6];
		modifiers = new List<Modifier>();
		this.rarity = rarity;
	}

	/*
	void Start()
    {
		icon = Resources.Load("Sprites/Placeholder01") as Sprite;
	}
    public Item(string name, Rarity rarity, Sprite icon, int stackSize, string description, GameObject WorldObjectPrefab)
    {
		this.rarity = rarity;
		this.icon = icon;
		this.name = name;
		this.stackSize = stackSize;
		this.description = description;
		this.WorldObjectPrefab = WorldObjectPrefab;
    }

    public Item() 
	{
		//level = difficulty * player level?;
		level = 1;
	}
	*/
    public static Dictionary<Rarity, string> RarityColor = new Dictionary<Rarity, string>() //stores the color associated with a rarity
	{
		{ Rarity.Common, "FEFDFC" }, //white
		{ Rarity.Uncommon, "B7DF53" }, //green
		{ Rarity.Rare, "53B2DF" }, //blue
		{ Rarity.Epic, "AE53DF" }, //purple
		{ Rarity.Legendary, "DF8C53" } //orange
	};

	public static Color ConvertColor(string hex)
    {
		byte a = 255;
		byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
		if (hex.Length == 8)
		{
			a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
		}
		return new Color32(r, g, b, a);
    }
 

	// Called when the item is pressed in the inventory
	public virtual bool Use(int index, int equipIndex=-1)
	{
		// Use the item
		// Something might happen

		//Debug.Log("Using " + name);
		return true;
	}

	
	public void RemoveFromInventory (int index)
	{
		Inventory.instance.RemoveItem(index);
	}
	
	public virtual string GetDescription()
    {
		return description;
    }

	
}

public enum Rarity { Common, Uncommon, Rare, Epic, Legendary }

#if UNITY_EDITOR
public class ItemEditor
{
	SerializedObject m_Target;

	SerializedProperty m_NameProperty;
	SerializedProperty m_IconProperty;
	SerializedProperty m_DescriptionProperty;
	//SerializedProperty m_WorldObjectPrefabProperty;
	SerializedProperty m_RarityProperty;
	SerializedProperty m_StackSizeProperty;
	//SerializedProperty statsProperty;
	//SerializedProperty dmgProperty;
	//SerializedProperty resProperty;
	SerializedProperty modifiersProperty;
	//SerializedProperty pos_modifiersProperty;

	public void Init(SerializedObject target)
	{
		m_Target = target;

		m_NameProperty = m_Target.FindProperty(nameof(Item.name));
		m_IconProperty = m_Target.FindProperty(nameof(Item.icon));
		m_DescriptionProperty = m_Target.FindProperty(nameof(Item.description));
		//m_WorldObjectPrefabProperty = m_Target.FindProperty(nameof(Item.WorldObjectPrefab));
		m_RarityProperty = m_Target.FindProperty(nameof(Item.rarity));
		m_StackSizeProperty = m_Target.FindProperty(nameof(Item.stackSize));
		//statsProperty = m_Target.FindProperty(nameof(Item.statModifiers));
		//dmgProperty = m_Target.FindProperty(nameof(Item.dmgModifiers));
		//resProperty = m_Target.FindProperty(nameof(Item.resModifiers));
		//pos_modifiersProperty = m_Target.FindProperty(nameof(Equipment.possible_modifiers));
		modifiersProperty = m_Target.FindProperty(nameof(Equipment.modifiers));
	}

	public void GUI()
	{
		EditorGUILayout.PropertyField(m_IconProperty);
		EditorGUILayout.PropertyField(m_NameProperty);
		EditorGUILayout.PropertyField(m_DescriptionProperty, GUILayout.MinHeight(128));
		//EditorGUILayout.PropertyField(m_WorldObjectPrefabProperty);
		EditorGUILayout.PropertyField(m_RarityProperty);
		EditorGUILayout.PropertyField(m_StackSizeProperty);
		//EditorGUILayout.PropertyField(statsProperty);
		//EditorGUILayout.PropertyField(resProperty);
		//EditorGUILayout.PropertyField(dmgProperty);
		EditorGUILayout.PropertyField(modifiersProperty);
	}
}
#endif