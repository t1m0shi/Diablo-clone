using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// Handle all the UI code related to the Abilities (drag'n'drop of object, using objects, equipping object etc.)
/// </summary>
public class AbilityBarUI : MonoBehaviour
{
    public class DragData
    {
        public PlayerAbilityHolder DraggedEntry;
        public RectTransform OriginalParent;
    }
    public Canvas DragCanvas;

    public DragData CurrentlyDragged;
    public CanvasScaler DragCanvasScaler { get; private set; }

    protected PlayerAbilityHolder[] m_ItemEntries;
    //CharacterData m_Data;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        CurrentlyDragged = null;
        DragCanvasScaler = DragCanvas.GetComponentInParent<CanvasScaler>();
        m_ItemEntries = GetComponentsInChildren<PlayerAbilityHolder>();
    }
    /*
    public void ObjectHoveredEnter(InventoryEntryUI hovered)
    {
        m_HoveredItem = hovered;

        Item itemUsed = inventory.items[m_HoveredItem.InventoryEntry].item;
        if (itemUsed != null)
        {
            Tooltip.gameObject.SetActive(true);
            Tooltip.UpdateText(itemUsed);
        }
    }

    public void ObjectHoverExited(InventoryEntryUI exited)
    {
        if (m_HoveredItem == exited && Tooltip.gameObject.activeSelf)
        {
            m_HoveredItem = null;
            Tooltip.gameObject.SetActive(false);
        }
    }*/

    
    public void HandledDroppedEntry(Vector3 position)
    {
        for (int i = 0; i < m_ItemEntries.Length; ++i)
        {
            RectTransform t = (RectTransform)m_ItemEntries[i].transform;
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position)) //only care once it gets to the next position of i
            {
                if (CurrentlyDragged.DraggedEntry.ability != m_ItemEntries[i].ability && m_ItemEntries[i].currentState == PlayerAbilityHolder.AbilityState.ready)
                {
                    Ability prevItem = CurrentlyDragged.DraggedEntry.ability;
                    Ability nextItem = m_ItemEntries[i].ability;

                    CurrentlyDragged.DraggedEntry.ability = nextItem;
                    m_ItemEntries[i].ability = prevItem;

                    CurrentlyDragged.DraggedEntry.UpdateHolder();
                    m_ItemEntries[i].UpdateHolder();
                    AbilityTooltip.instance.UpdateText();
                    break;
                }
            }
        }
    }
}