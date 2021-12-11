using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{
    private PlayerStats player;
    [SerializeField]
    public List<UsageEffect> UsageEffects = new List<UsageEffect>();


    public abstract class UsageEffect : ScriptableObject
    {
        public string description;
        //return true if could be used, false otherwise.
        public abstract bool Consume(PlayerStats player);
    }

    public Consumable(Rarity rarity) : base()
    {
        //depending on rarity, maybe add more usage effects?
        //icon = potion.png;
        this.rarity = rarity;
        //WorldObjectPrefab = potion;
    }
    public Consumable(PlayerStats player, List<UsageEffect> usageEffects) : base()
    {
        this.player = player;
        UsageEffects = usageEffects;
    }

    public override bool Use(int index, int equipIndex=-1)
    {
        base.Use(index);
        Debug.Log("used a consumable");

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        bool wasUsed = false;
        foreach (var effect in UsageEffects)
        {
            wasUsed |= effect.Consume(player);
        }
        if (wasUsed)
        {
            Inventory inv = GameObject.FindGameObjectWithTag("GM").GetComponent<Inventory>();
            inv.items[index].count -= 1;
            if (inv.items[index].count == 0)
            {
                inv.RemoveItem(index);
            }
            if (inv.onItemChangedCallback != null) inv.onItemChangedCallback.Invoke();
        }
        return wasUsed;
    }

    public override string GetDescription()
    {
        string description = base.GetDescription();

        if (!string.IsNullOrWhiteSpace(description))
            description += "\n";
        else
            description = "";


        foreach (var effect in UsageEffects)
        {
            description += effect.description + "\n";
        }

        return description;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Consumable))]
public class ConsumableEditor : Editor
{
    Consumable m_Target;

    ItemEditor m_ItemEditor;

    List<string> m_AvailableUsageType;
    SerializedProperty m_UsageEffectListProperty;

    void OnEnable()
    {
        m_Target = target as Consumable;
        m_UsageEffectListProperty = serializedObject.FindProperty(nameof(Consumable.UsageEffects));

        m_ItemEditor = new ItemEditor();
        m_ItemEditor.Init(serializedObject);

        var lookup = typeof(Consumable.UsageEffect);
        m_AvailableUsageType = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(lookup))
            .Select(type => type.Name)
            .ToList();
    }

    public override void OnInspectorGUI()
    {
        m_ItemEditor.GUI();

        int choice = EditorGUILayout.Popup("Add new Effect", -1, m_AvailableUsageType.ToArray());

        if (choice != -1)
        {
            var newInstance = ScriptableObject.CreateInstance(m_AvailableUsageType[choice]);

            AssetDatabase.AddObjectToAsset(newInstance, target);

            m_UsageEffectListProperty.InsertArrayElementAtIndex(m_UsageEffectListProperty.arraySize);
            m_UsageEffectListProperty.GetArrayElementAtIndex(m_UsageEffectListProperty.arraySize - 1).objectReferenceValue = newInstance;
        }

        Editor ed = null;
        int toDelete = -1;
        for (int i = 0; i < m_UsageEffectListProperty.arraySize; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            var item = m_UsageEffectListProperty.GetArrayElementAtIndex(i);
            SerializedObject obj = new SerializedObject(item.objectReferenceValue);

            Editor.CreateCachedEditor(item.objectReferenceValue, null, ref ed);

            ed.OnInspectorGUI();
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("-", GUILayout.Width(32)))
            {
                toDelete = i;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (toDelete != -1)
        {
            var item = m_UsageEffectListProperty.GetArrayElementAtIndex(toDelete).objectReferenceValue;
            DestroyImmediate(item, true);

            //need to do it twice, first time just nullify the entry, second actually remove it.
            m_UsageEffectListProperty.DeleteArrayElementAtIndex(toDelete);
            //m_UsageEffectListProperty.DeleteArrayElementAtIndex(toDelete);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
