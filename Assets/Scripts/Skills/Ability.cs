using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="New Ability", menuName ="Ability")]
public class Ability : ScriptableObject
{
    public new string name;
	public Sprite icon;
	//public GameObject spellPrefab;
    public float cooldownTime;
    public float activeTime = 0.1f;
    public int cost;
    public CharacterStats originator;
    [SerializeField]
    public PlayerAbilityEffect effect;

    //this incorporates the player's cooldown reduction
    public float GetCooldownTime()
    {
        return cooldownTime * (1 - originator.secondary[SecondaryStatType.CooldownReduction].GetValue() / 100);
    }

    //[CreateAssetMenu(fileName = "New Effect", menuName = "Ability/Effect")]
    public class PlayerAbilityEffect : ScriptableObject
    {
        public string description;
        public PlayerAbilityHolder holder;
        public Transform attackPoint;
        public virtual void Activate(PlayerStats originator)
        {
            if (originator.onSpellCast != null)//originator.GetType() == typeof(PlayerStats) && originator.onSpellCast != null)
            {
                originator.onSpellCast.Invoke();
            }
        }

        //subclass for buffs and debuffs
        public class Buff : PlayerAbilityEffect
        {
            public float duration;
            public int percent;
            protected Modifier change;
            protected PlayerStats target;

            protected virtual void RemoveEffect() { }//override to remove buff after it expires (if it does)
        }
        //subclass for attacks
        public class Attack : PlayerAbilityEffect
        {
            public int damage;
            public GameObject attackPrefab;

            public override void Activate(PlayerStats originator)
            {
                base.Activate(originator);
                //PlayerManager.instance.onCombatEnter.Invoke();
                //create the attack object in attack

            }
        }
    }

    

    
}


#region editor stuff
#if UNITY_EDITOR
public class AbilityEditorThing
{
	SerializedObject m_Target;

	SerializedProperty m_NameProperty;
	SerializedProperty m_IconProperty;
	//SerializedProperty m_SpellPrefabProperty;
	SerializedProperty m_CooldownProperty;
	SerializedProperty m_ActiveTimeProperty;
	SerializedProperty m_CostProperty;
    //SerializedProperty m_AffectsStatsProperty;
    //SerializedProperty m_EffectProperty;


    public void Init(SerializedObject target)
	{
		m_Target = target;

		m_NameProperty = m_Target.FindProperty(nameof(Ability.name));
		m_IconProperty = m_Target.FindProperty(nameof(Ability.icon));
		//m_SpellPrefabProperty = m_Target.FindProperty(nameof(Ability.spellPrefab));
		m_CooldownProperty = m_Target.FindProperty(nameof(Ability.cooldownTime));
		m_CostProperty = m_Target.FindProperty(nameof(Ability.cost));
		//m_EffectProperty = m_Target.FindProperty(nameof(Ability.effect));
        m_ActiveTimeProperty = m_Target.FindProperty(nameof(Ability.activeTime));
        //m_AffectsStatsProperty = m_Target.FindProperty(nameof(Ability.affectsStats));
    }

	public void GUI()
	{
		EditorGUILayout.PropertyField(m_IconProperty);
		EditorGUILayout.PropertyField(m_NameProperty);
		//EditorGUILayout.PropertyField(m_SpellPrefabProperty);
		EditorGUILayout.PropertyField(m_CooldownProperty);
		EditorGUILayout.PropertyField(m_CostProperty);
        EditorGUILayout.PropertyField(m_ActiveTimeProperty);
        //EditorGUILayout.PropertyField(m_AffectsStatsProperty);
        //EditorGUILayout.PropertyField(m_EffectProperty);
    }
}

[CustomEditor(typeof(Ability))]
public class AbilityEditor : Editor
{

    AbilityEditorThing m_AbilityEditor;

    List<string> m_AvailableEffects;
    SerializedProperty m_EffectProperty;

    void OnEnable()
    {
        
        m_EffectProperty = serializedObject.FindProperty(nameof(Ability.effect));
        //m_DescriptionProperty = serializedObject.FindProperty(nameof(Ability.PlayerAbilityEffect.description));

        m_AbilityEditor = new AbilityEditorThing();
        m_AbilityEditor.Init(serializedObject);

        var lookup = typeof(Ability.PlayerAbilityEffect);
        m_AvailableEffects = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(lookup))
            .Select(type => type.Name)
            .ToList();
        
    }

    public override void OnInspectorGUI()
    {
        m_AbilityEditor.GUI();

        if (m_EffectProperty.objectReferenceValue == null)
        {
            int choice = EditorGUILayout.Popup("Add an Effect", -1, m_AvailableEffects.ToArray());
            
            if (choice != -1)
            {
                var newInstance = ScriptableObject.CreateInstance(m_AvailableEffects[choice]);
                AssetDatabase.AddObjectToAsset(newInstance, target);
                m_EffectProperty.objectReferenceValue = newInstance;
            }
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            var effect = m_EffectProperty.objectReferenceValue as Ability.PlayerAbilityEffect;
            Editor ed = null;
            Editor.CreateCachedEditor(effect, typeof(AbilityEffectEditor), ref ed);
            Editor ad = Editor.CreateEditor(effect, typeof(AbilityEffectEditor));
            //ad.DrawDefaultInspector();
            //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Ability.PlayerAbilityEffect.description)));
            ed.OnInspectorGUI();
            //ad.OnInspectorGUI();

            //Editor.CreateCachedEditor(effect, typeof(AbilityEffectEditor), ref ed);
            //ad.OnInspectorGUI();
            //ed.OnInspectorGUI();
            //Editor.CreateCachedEditor(effect, null, ref ed);
            ///ed.OnInspectorGUI();
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("-", GUILayout.Width(32)))
            {
                DestroyImmediate(effect, true);
            }
            EditorGUILayout.EndHorizontal();
        }
        serializedObject.ApplyModifiedProperties();
        
    }
}

[CustomEditor(typeof(Ability.PlayerAbilityEffect))]
public class AbilityEffectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SerializedProperty prop = serializedObject.GetIterator();
        if (prop.NextVisible(true))
        {
            do
            {
                // Draw property manually.
                if (prop.name == "description")
                {
                    EditorGUILayout.PropertyField(prop, GUILayout.MinHeight(128));
                    GUI.skin.textField.wordWrap = true;
                }
                else if (prop.name == "m_Script")
                {
                    //EditorGUI.BeginDisabledGroup(true);
                    using (new EditorGUI.DisabledScope(true)) { EditorGUILayout.PropertyField(prop); }
                    //EditorGUI.EndDisabledGroup();
                }
                // Draw default property field.
                else
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.name), true);
                }
            }
            while (prop.NextVisible(false));
        }

        serializedObject.ApplyModifiedProperties();

    }
}

#endif
#endregion