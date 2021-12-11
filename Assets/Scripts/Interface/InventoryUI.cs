using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// Handle all the UI code related to the inventory (drag'n'drop of object, using objects, equipping object etc.)
/// </summary>
public class InventoryUI : MonoBehaviour
{
    public class DragData
    {
        public InventoryEntryUI DraggedEntry;
        public RectTransform OriginalParent;
    }

    //public RectTransform[] ItemSlots;

   // public InventoryEntryUI ItemEntryPrefab;
    public ItemTooltip Tooltip;

   // public EquipmentUI equipmentUI;

    public Inventory inventory;

    public Canvas DragCanvas;

    public DragData CurrentlyDragged { get; set; }
    public CanvasScaler DragCanvasScaler { get; private set; }

    //public CharacterData Character
    //{
    //    get { return m_Data; }
    //}

    public InventoryEntryUI[] m_ItemEntries;
    public InventoryEntryUI m_HoveredItem;
    //CharacterData m_Data;
    public EquipmentUI equipmentParent;
    public EquipmentEntryUI[] equipEntries;

    void Awake()
    {
        Init();
        /*
        for (int i = 0; i < m_ItemEntries.Length; ++i)
        {
            m_ItemEntries[i] = Instantiate(ItemEntryPrefab, ItemSlots[i]);
            m_ItemEntries[i].gameObject.SetActive(false);
            m_ItemEntries[i].Owner = this;
            m_ItemEntries[i].InventoryEntry = i;
        }*/

        //equipmentUI.Init(this);
    }


    void OnEnable()
    {
        m_HoveredItem = null;
        Tooltip.gameObject.SetActive(false);
    }

    void Init()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;	// Subscribe to the onItemChanged callback

        CurrentlyDragged = null;

        DragCanvasScaler = DragCanvas.GetComponentInParent<CanvasScaler>();

        //m_ItemEntries = new InventoryEntryUI[inventory.slots];
        m_ItemEntries = GetComponentsInChildren<InventoryEntryUI>();
        for (int i = 0; i < m_ItemEntries.Length; i++)
        {
            m_ItemEntries[i].InventoryEntry = i;
        }
        equipmentParent.itemEntries = m_ItemEntries;
    }

    // Update the inventory UI by:
    //		- Adding items
    //		- Clearing empty slots
    // This is called using a delegate on the Inventory.
    public void UpdateUI()
    {
        if (inventory == null)
        {
            Debug.Log("reinitializing inventory");
            Init();
        }
        //Debug.Log("updating ui");
        // Loop through all the slots
        for (int i = 0; i <m_ItemEntries.Length; i++)
        {
            m_ItemEntries[i].UpdateEntry();
        }
    }

    public void ObjectRightClicked(Inventory.InventoryEntry usedItem)
    {
        //if (inventory.UseItem(usedItem))
        //SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = usedItem.Item is EquipmentItem ? SFXManager.ItemEquippedSound : SFXManager.ItemUsedSound });
        if (Input.GetKey(KeyCode.LeftShift) && usedItem.item.GetType() == typeof(Weapon))
        {
            Weapon w = (Weapon)usedItem.item;
            inventory.UseItem(usedItem, (int)w.equipSlot + 1);
        }
        else if (usedItem.item.GetType() == typeof(Equipment) || usedItem.item.GetType().IsSubclassOf(typeof(Equipment)))
        {
            Equipment e = (Equipment)usedItem.item;
            inventory.UseItem(usedItem, (int)e.equipSlot);
        }
        else
        {
            inventory.UseItem(usedItem);
        }
        //ObjectHoverExited(m_HoveredItem);
        ObjectHoveredEnter(m_HoveredItem);
        UpdateUI();
    }

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
    }

    
    public void HandleDroppedEntry(Vector3 position, EquipmentEntryUI equ=null)//, InventoryEntryUI prevEntry)
    {
        //dragging from inventory to somewhere
        if (equ == null)
        {
            //if we are dragging from inventory onto equipment, inEquipment will become true
            bool inEquipment = false;

            for (int j = 0; j < equipEntries.Length; ++j)
            {
                RectTransform t = (RectTransform)equipEntries[j].transform;
                if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
                {
                    equipmentParent.CurrentlyDragged = new EquipmentUI.DragData()
                    {
                        DraggedEntry = equipEntries[j],
                        OriginalParent = t
                    };
                    equipmentParent.HandleDroppedEntry(position, this.CurrentlyDragged.DraggedEntry);
                    //this.CurrentlyDragged = null;
                    inEquipment = true;
                    break;
                }
            }
            //dragging inside the inventory
            if (!inEquipment)
            {
                bool inBox = false;
                for (int i = 0; i < m_ItemEntries.Length; ++i)
                {
                    RectTransform t = (RectTransform)m_ItemEntries[i].transform;
                    if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
                    {
                        if (CurrentlyDragged.DraggedEntry.InventoryEntry != i) //to not include itself
                        {
                            Item prevItem = inventory.items[CurrentlyDragged.DraggedEntry.InventoryEntry].item;
                            Item nextItem = inventory.items[m_ItemEntries[i].InventoryEntry].item;
                            //swap the item (if they're different)and how many
                            if (nextItem != prevItem)
                            {
                                int prevCount = inventory.items[CurrentlyDragged.DraggedEntry.InventoryEntry].count;

                                inventory.items[CurrentlyDragged.DraggedEntry.InventoryEntry].item = inventory.items[i].item;
                                inventory.items[CurrentlyDragged.DraggedEntry.InventoryEntry].count = inventory.items[i].count;
                                inventory.items[i].item = prevItem;
                                inventory.items[i].count = prevCount;

                                CurrentlyDragged.DraggedEntry.UpdateEntry();
                                m_ItemEntries[i].UpdateEntry();
                                inBox |= true;
                                break;
                            }
                            //otherwise if they're the same item and not null
                            else if (nextItem != null && nextItem == prevItem)
                            {
                                //try to fit the stack from previous (currently dragged) to new slot (i)
                                inventory.MergeStack(inventory.items[CurrentlyDragged.DraggedEntry.InventoryEntry], inventory.items[i]);
                                CurrentlyDragged.DraggedEntry.UpdateEntry();
                                m_ItemEntries[i].UpdateEntry();
                                inBox |= true;
                                break;
                            }

                        }
                        else
                        {
                            inBox = true;
                        }

                    }
                }
                if (!inBox) //the boundaries are a little jacked up, can drop an item in between slots or on the same slot
                {
                    Debug.Log("dropping item");
                    //inventory.RemoveItem(CurrentlyDragged.DraggedEntry.InventoryEntry);
                }
            }
        }
        //dragging from somewhere else into inventory
        else
        {
            for (int i = 0; i < m_ItemEntries.Length; ++i)
            {
                RectTransform t = (RectTransform)m_ItemEntries[i].transform;
                if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
                {
                    Item prevItem = equipmentParent.equipped.currentEquipment[equ.slot];
                    Item nextItem = inventory.items[m_ItemEntries[i].InventoryEntry].item;
                    //if it's the same equipment type, then swap from inventory into the equipment
                    if (nextItem != null && nextItem.GetType() == prevItem.GetType())
                    {
                        EquipmentManager.instance.SwapEquipment((Equipment)nextItem, (Equipment)prevItem, m_ItemEntries[i].InventoryEntry, equ.slot);
                        /*
                        if (prevItem.GetType() == typeof(Weapon))
                        {
                            Weapon p = (Weapon)prevItem;
                            Weapon n = (Weapon)nextItem;
                            if (p.grip == PlayerStats.instance.mainHand)
                                EquipmentManager.instance.SwapWeapon(n, p, m_ItemEntries[i].InventoryEntry, 0);
                            else
                            {
                                EquipmentManager.instance.SwapWeapon(n, p, m_ItemEntries[i].InventoryEntry, 1);
                            }
                        }
                        else
                            EquipmentManager.instance.SwapEquipment((Equipment)nextItem, (Equipment)prevItem, m_ItemEntries[i].InventoryEntry);
                        */
                    }
                    //nothing in that slot
                    else
                    {
                        equ.transform.SetParent(equipmentParent.CurrentlyDragged.OriginalParent);
                        EquipmentManager.instance.Unequip((Equipment)prevItem, equ.slot, m_ItemEntries[i].InventoryEntry);
                        //equ.UpdateEntry();
                        m_ItemEntries[i].UpdateEntry();
                    }
                    break;
                }
            }
            return;
        }
    }
}



/*
using UnityEngine;

// This object updates the inventory UI. 

public class InventoryUI : MonoBehaviour {

	public Transform itemsParent;	// The parent object of all the items
	public GameObject inventoryUI;	// The entire UI

	Inventory inventory;	// Our current inventory

	//Inventory.InventoryEntry[] slots;	// List of all the slots

	void Start () {
		inventory = Inventory.instance;
		inventory.onItemChangedCallback += UpdateUI;	// Subscribe to the onItemChanged callback

		// Populate our slots array
		//slots = itemsParent.GetComponentsInChildren<Inventory.InventoryEntry>();
	}
	
	

	// Update the inventory UI by:
	//		- Adding items
	//		- Clearing empty slots
	// This is called using a delegate on the Inventory.
	void UpdateUI ()
	{
        Debug.Log("updating ui");
		// Loop through all the slots
		for (int i = 0; i < inventory.items.Length; i++)
		{
			
			if (i < inventory.items.Length)	// If there is an item to add
			{
				//slots[i].AddItem(inventory.items[i]);	// Add it
			} else
			{
				// Otherwise clear the slot
				//slots[i].ClearSlot();
			}
		}
	}
}

*/