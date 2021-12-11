using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

/* Base class that player and enemies can derive from to include stats. */
public class CharacterStats : MonoBehaviour {

	// Health
	public int maxHealth;// = 100;
	public int baseHealth = 100;
	public int currentHealth;
	public int level { get; set; } = 1;
	//public float[] baseAttackSpeed = new float[] { 1f, 1f };
	public float baseAttackSpeed = 1f; //base 1 attack per second
	public int handsFull = 0;
	//players have mana
	public PlayerCombat combat;

	/*
	public Stat health = new Stat(StatType.Health);
	public Stat mana = new Stat(StatType.Mana);
	public Stat armor = new Stat(StatType.Armor);
	public Stat intelligence = new Stat(StatType.Intelligence);
	public Stat strength = new Stat(StatType.Strength);
	public Stat agility = new Stat(StatType.Agility);
	public Stat critchance = new Stat(StatType.CritChance);
	public Stat critdmg = new Stat(StatType.CritDamage);
	public Stat cdreduc = new Stat(StatType.CooldownReduction);
	public Stat atkspd = new Stat(StatType.AttackSpeed);
	public Stat movespd = new Stat(StatType.MoveSpeed);
	*/
	[SerializeField]
	public Dictionary<PrimaryStatType, Stat> primary = new Dictionary<PrimaryStatType, Stat>();
	[SerializeField]
	public Dictionary<SecondaryStatType, Stat> secondary = new Dictionary<SecondaryStatType, Stat>();
	[SerializeField]
	//public Dictionary<DmgType, Resistance> defenses;
	public Dictionary<DmgType, Stat> defenses = new Dictionary<DmgType, Stat>();
	public Stat BlockChance;// = new Stat(0,new List<int>(), new List<float>() { 2f });
	[SerializeField]
	//public Dictionary<DmgType, Damage> damages;
	public Dictionary<DmgType, Stat> damages = new Dictionary<DmgType, Stat>();

	protected bool init;

	public delegate void OnStatsChanged();
	public OnStatsChanged onStatsChanged;

	public delegate void OnHealthChange(int change); //can use this to update UI, or maybe if int is > certain value, can do screen shake, red around edge
	public OnHealthChange onHealthChange;

	// Set current health to max health
	// when starting the game.
	void Awake()
	{
		if (!init) Init();
	}
    protected void Init()
    {
		init = true;

		primary = new Dictionary<PrimaryStatType, Stat>();
		secondary = new Dictionary<SecondaryStatType, Stat>();
		defenses = new Dictionary<DmgType, Stat>();
		damages = new Dictionary<DmgType, Stat>();
		//currentHealth = maxHealth;
		foreach (PrimaryStatType type in Enum.GetValues(typeof(PrimaryStatType)))
		{
			primary[type] = new Stat(0);
			//stats[type] = 0;
		}
		foreach (SecondaryStatType type in Enum.GetValues(typeof(SecondaryStatType)))
		{
			secondary[type] = new Stat(0);
			//stats[type] = 0;
		}
		foreach (DmgType type in Enum.GetValues(typeof(DmgType)))
		{
			defenses[type] = new Stat(0);
			//defenses[type] = 0;
		}
		foreach (DmgType type in Enum.GetValues(typeof(DmgType)))
		{
			damages[type] = new Stat(0);
			//defenses[type] = 0;
		}

		//primary[PrimaryStatType.Health].AddModifier(new Modifier(); //set base health stat to 100
		primary[PrimaryStatType.Health].baseValue = baseHealth;
		maxHealth = (int)primary[PrimaryStatType.Health].GetValue();
		secondary[SecondaryStatType.AttackSpeed].baseValue = baseAttackSpeed;
		currentHealth = maxHealth;
		BlockChance = new Stat(0, new List<int>(), new List<float>());// { 2f });
		BlockChance.type = ModType.Block;
		combat = GetComponent<PlayerCombat>();
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
		{
			TakeDamage(10, DmgType.Physical);
        }
		
    }


    // Damage the character
    public void TakeDamage (int damage, DmgType type)
	{
		//reduce damage from armor
		//damage -= armor.GetValue();
		//reduce damage from resistances
		//reduce damage by the resistance of the same type
		damage -= (int)defenses[type].GetValue();
		int roll = UnityEngine.Random.Range(1, 101);
		if (roll <= BlockChance.GetChance())
        {
			damage = Mathf.RoundToInt((float)damage * 0.5f);
        }
		damage = Mathf.Clamp(damage, 0, int.MaxValue);

		// Damage the character
		currentHealth -= damage;
		//Debug.Log(transform.name + " takes " + damage + " damage.");
		if (onHealthChange != null) onHealthChange.Invoke(damage);

		// If health reaches zero
		if (currentHealth <= 0)
		{
			Die();
		}
	}

	public virtual void DealDamage(CharacterStats target, int damage, DmgType type)
    {
		bool crit = UnityEngine.Random.Range(0, 101) <= secondary[SecondaryStatType.CritChance].GetValue();
		if (crit)
		{
			damage = Mathf.RoundToInt((float)damage * (1 + (secondary[SecondaryStatType.CritDamage].GetValue() / 100)));
		}
		//damage *= UnityEngine.Random.Range(weapon.damage.min, weapon.damage.max + 1);
		damage += (int)damages[type].GetValue();
		target.TakeDamage(damage, type);
		EnterCombat();
		target.EnterCombat();
	}

	public float CalcAttackSpeed(Weapon weap)
    {
		float aps = secondary[SecondaryStatType.AttackSpeed].GetValue() + weap.damage.attacksPerSecond;
		return 1f / aps;
    }

	public virtual void Die ()
	{
		// Die in some way
		// This method is meant to be overwritten
		Debug.Log(transform.name + " died.");
	}

	public virtual void EnterCombat()
    {
		
    }

#if UNITY_EDITOR

	[CustomPropertyDrawer(typeof(CharacterStats))]
	public class StatsDrawer : PropertyDrawer
	{

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = new VisualElement();

			foreach (var i in Enum.GetValues(typeof(PrimaryStatType)))
            {
				var j = new PropertyField(property.FindPropertyRelative(nameof(i)));
				container.Add(j);
            }

			return container;
		}

		/*
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			int enumTypesCount = Enum.GetValues(typeof(StatType)).Length;
			int lineCount = enumTypesCount + 7;
			float extraHeight = 6f;
			float propertyHeight = lineCount * EditorGUIUtility.singleLineHeight + extraHeight;

			return propertyHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.alignment = TextAnchor.MiddleCenter;

			var currentRect = position;
			currentRect.height = EditorGUIUtility.singleLineHeight;

			EditorGUI.DropShadowLabel(currentRect, property.displayName);

			currentRect.y += EditorGUIUtility.singleLineHeight + 6f;
			EditorGUI.PropertyField(currentRect, property.FindPropertyRelative(nameof(StatType.Health)));

			currentRect.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(currentRect, property.FindPropertyRelative(nameof(StatSystem.Stats.strength)));

			currentRect.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(currentRect, property.FindPropertyRelative(nameof(StatSystem.Stats.defense)));

			currentRect.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(currentRect, property.FindPropertyRelative(nameof(StatSystem.Stats.agility)));

			currentRect.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.LabelField(currentRect, "Elemental Protection/Boost", style);

			currentRect.y += EditorGUIUtility.singleLineHeight;
			currentRect.width *= 0.3f;

			currentRect.x += currentRect.width;
			EditorGUI.LabelField(currentRect, "Protection (%)", style);
			currentRect.x += currentRect.width;
			EditorGUI.LabelField(currentRect, "Boost (%)", style);

			var names = Enum.GetNames(typeof(StatSystem.DamageType));

			var elementalProtectionProp = property.FindPropertyRelative(nameof(StatSystem.Stats.elementalProtection));
			var elementalBoostProp = property.FindPropertyRelative(nameof(StatSystem.Stats.elementalBoosts));

			for (int i = 0; i < names.Length; ++i)
			{
				currentRect.x -= currentRect.width * 2;
				currentRect.y += EditorGUIUtility.singleLineHeight;
				EditorGUI.LabelField(currentRect, names[i]);

				currentRect.x += currentRect.width;
				EditorGUI.PropertyField(currentRect, elementalProtectionProp.GetArrayElementAtIndex(i), GUIContent.none);

				currentRect.x += currentRect.width;
				EditorGUI.PropertyField(currentRect, elementalBoostProp.GetArrayElementAtIndex(i), GUIContent.none);

			}

			EditorGUI.EndProperty();
		}*/
	}
#endif
}




