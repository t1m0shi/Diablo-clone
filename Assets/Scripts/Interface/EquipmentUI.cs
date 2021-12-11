using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keep reference and update the Equipment entry (the icons around the character in the Inventory)
/// </summary>
public class EquipmentUI : MonoBehaviour
{
    public class DragData
    {
        public EquipmentEntryUI DraggedEntry;
        public RectTransform OriginalParent;
    }
    public EquipmentManager equipped;
    public Transform equippedObj;

    public ItemTooltip tooltip;
    public EquipmentEntryUI m_HoveredItem;
    public EquipmentEntryUI[] equipEntries;

    public InventoryUI inventoryParent;
    public InventoryEntryUI[] itemEntries;

    public Canvas DragCanvas;
    public DragData CurrentlyDragged { get; set; }
    public CanvasScaler DragCanvasScaler { get; private set; }


    private void Awake()
    {
        Init();
    }

    void Init()
    {
        equipped = EquipmentManager.instance;
        equipped.onEquipmentChanged += UpdateUI;// (null, null);
        equipped.onWeaponChanged += UpdateUI;
        //equippedObj = gameObject.transform.Find("Equipped").gameObject; //object that stores all of the equipped items ui's
        equipEntries = GetComponentsInChildren<EquipmentEntryUI>();
        //UpdateUI(null, null);
        inventoryParent.equipEntries = equipEntries;

        CurrentlyDragged = null;
        DragCanvasScaler = DragCanvas.GetComponentInParent<CanvasScaler>();
    }

    public void UpdateUI(Equipment t, Equipment s)
    {
        if (equipped == null) {
            Debug.Log("reinitializing equipment");
            Init();
        }
        //Debug.Log("updating equip ui");
        foreach (var i in equipEntries)
        {
            i.UpdateEntry();
        }
    }

    public void ObjectRightClicked(Equipment usedItem, int slot)
    {
        //if (inventory.UseItem(usedItem))
        //SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = usedItem.Item is EquipmentItem ? SFXManager.ItemEquippedSound : SFXManager.ItemUsedSound });
        //Debug.Log("unequipped");
        if (PlayerManager.instance.state != PlayerState.attacking)
        {
            equipped.Unequip(usedItem, slot);
            //ObjectHoverExited(m_HoveredItem);
            ObjectHoveredEnter(m_HoveredItem);
            UpdateUI(null, usedItem);
        }
    }

    public void ObjectHoveredEnter(EquipmentEntryUI hovered)
    {
        m_HoveredItem = hovered;

        Equipment itemUsed = hovered.selected;
        if (itemUsed != null)
        {
            tooltip.gameObject.SetActive(true);
            tooltip.UpdateText(itemUsed);
            
        }
        
    }

    public void ObjectHoverExited(EquipmentEntryUI exited)
    {
        if (m_HoveredItem == exited && tooltip.gameObject.activeSelf)
        {
            m_HoveredItem = null;
            tooltip.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        ObjectHoverExited(m_HoveredItem);
    }
    
    public void HandleDroppedEntry(Vector3 position, InventoryEntryUI inv=null)
    {
        //dragging from equipment to somewhere
        if (inv == null)
        {
            //if we are dragging from equipment onto inventory, inInventory will become true
            bool inInventory = false;
            for (int j = 0; j < itemEntries.Length; ++j)
            {
                RectTransform t = (RectTransform)itemEntries[j].transform;
                if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
                {
                    inventoryParent.CurrentlyDragged = new InventoryUI.DragData
                    {
                        DraggedEntry = itemEntries[j],
                        OriginalParent = t
                    };
                    inventoryParent.HandleDroppedEntry(position, CurrentlyDragged.DraggedEntry);
                    //this.CurrentlyDragged = null;
                    inInventory = true;
                    break;
                }
            }
            //dragging inside equipment
            if (!inInventory)
            {
                for (int i = 0; i < equipEntries.Length; ++i)
                {
                    RectTransform t = (RectTransform)equipEntries[i].transform;
                    if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
                    {
                        if (CurrentlyDragged.DraggedEntry.slot != i) //exclude itself
                        {
                            Equipment prevItem = equipped.currentEquipment[CurrentlyDragged.DraggedEntry.slot];
                            Equipment nextItem = equipped.currentEquipment[equipEntries[i].slot];
                            //swap the item if they're the same type (should only happen with weapons)
                            if (prevItem.equipSlot == (EquipmentSlot)equipEntries[i].slot || prevItem.equipSlot  == EquipmentSlot.Weapon)
                            {
                                //if (nextItem != null)
                                Weapon p = (Weapon)prevItem;
                                Weapon n = (Weapon)nextItem;
                                if (n == null || n.wtype != WeaponType.Shield)
                                {
                                    equipped.SwapWeapons(p, n, CurrentlyDragged.DraggedEntry.slot, equipEntries[i].slot);

                                    //equipped.currentEquipment[CurrentlyDragged.DraggedEntry.slot] = n;
                                    //equipped.currentEquipment[equipEntries[i].slot] = p;
                                    //CurrentlyDragged.DraggedEntry.UpdateEntry();
                                    equipEntries[i].UpdateEntry();
                                    break;
                                }
                            }
                        }

                    }
                }
            }
            
        }
        //dragging from somewhere into equipment
        else
        {
            for (int i = 0; i < equipEntries.Length; ++i)
            {
                RectTransform t = (RectTransform)equipEntries[i].transform;
                if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
                {
                    Equipment prevItem = (Equipment)inventoryParent.inventory.items[inv.InventoryEntry].item;
                    Equipment nextItem = equipped.currentEquipment[equipEntries[i].slot];

                    
                    bool matches = false;
                    //try
                    //{
                    if (prevItem.equipSlot == (EquipmentSlot)equipEntries[i].slot || prevItem.equipSlot == EquipmentSlot.Weapon)
                    {
                        matches = true;
                    }
                //}
                //catch
                //{
                    //else if (prevItem.equipSlot == EquipmentSlot.Weapon && (int) == equipEntries[i].slot)
                    //{
                    //    matches = true;
                    //}
                    //}
                    if (matches)
                    {
                        if (nextItem != null)
                        {
                            EquipmentManager.instance.SwapEquipment(prevItem, nextItem, inv.InventoryEntry, equipEntries[i].slot);
                        }
                        else
                        {
                            EquipmentManager.instance.Equip(prevItem, inv.InventoryEntry, equipEntries[i].slot);
                        }
                        inv.UpdateEntry();
                        //equipEntries[i].UpdateEntry();
                        break;
                    }
                    
                   
                }
            }
            return;
        }
    }
    
}
