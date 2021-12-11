using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentEntryUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public EquipmentUI Owner;
    //[HideInInspector]
    public Equipment selected;
    public Image icon;
    public int slot;
    private Color ogcolor;

    public void Awake()
    {
        string s = gameObject.transform.parent.name.ToString();
        if (s == "Weapon2")
            slot = (int)(EquipmentSlot)Enum.Parse(typeof(EquipmentSlot), "Weapon") + 1;
        else
            slot = (int)(EquipmentSlot)Enum.Parse(typeof(EquipmentSlot), s);
        selected = Owner.equipped.currentEquipment[slot];
        ogcolor = transform.parent.GetComponent<Image>().color;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        Owner.CurrentlyDragged = new EquipmentUI.DragData();
        Owner.CurrentlyDragged.DraggedEntry = this;
        Owner.CurrentlyDragged.OriginalParent = (RectTransform)transform.parent;

        transform.SetParent(Owner.DragCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        transform.localPosition = transform.localPosition + UnscaleEventDelta(eventData.delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        Owner.HandleDroppedEntry(eventData.position);//, prevDrag);

        RectTransform t = transform as RectTransform;
        float ogx = t.rect.width;
        float ogy = t.rect.height;

        transform.SetParent(Owner.CurrentlyDragged.OriginalParent, true);

        t.offsetMax = -Vector2.one * 4;
        t.offsetMin = Vector2.one * 4;
        t.sizeDelta = new Vector2(ogx, ogy);
    }

    Vector3 UnscaleEventDelta(Vector3 vec)
    {

        Vector2 referenceResolution = Owner.DragCanvasScaler.referenceResolution;
        Vector2 currentResolution = new Vector2(Screen.width, Screen.height);

        float widthRatio = currentResolution.x / referenceResolution.x;
        float heightRatio = currentResolution.y / referenceResolution.y;
        float ratio = Mathf.Lerp(widthRatio, heightRatio, Owner.DragCanvasScaler.matchWidthOrHeight);

        return vec / ratio;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //right click an item in equipment, should attempt to unequip
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (selected != null)
                Owner.ObjectRightClicked(selected, slot);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //UpdateEntry();
        Owner.ObjectHoveredEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //selected = null;
        Owner.ObjectHoverExited(this);
    }

    public void UpdateEntry()
    {
        selected = Owner.equipped.currentEquipment[slot];
        GameObject s = transform.parent.gameObject;
        //if there is an item in the equipped slot, display in the equipmentUI in the correct slot with the rarity changing the color of the inventoryEntry
        if (selected != null)
        {
            //get the correct invEntry parent (ex. head, shoulder)
            //GameObject s = Owner.equippedObj.Find(e.equipSlot.ToString()).gameObject;
            //change the color
            string hex = Item.RarityColor[selected.rarity];
            s.GetComponent<Image>().color = Item.ConvertColor(hex);

            //change the Icon
            //s.GetComponentInChildren<EquipmentEntryUI>().icon.gameObject.SetActive(true);
            //s.GetComponentInChildren<EquipmentEntryUI>().icon.sprite = e.icon;
            icon.gameObject.SetActive(true);
            icon.sprite = selected.icon;
        }
        else
        {
            s.GetComponent<Image>().color = ogcolor;
            icon.sprite = null;
            icon.gameObject.SetActive(false);
        }
    }
}
