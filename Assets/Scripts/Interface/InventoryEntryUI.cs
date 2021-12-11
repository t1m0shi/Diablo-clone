using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class InventoryEntryUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image icon;
    public Text ItemCount;

    public int InventoryEntry;
    //public Item item;// { get; private set; } //should be moving entrys??

    public InventoryUI Owner;// { get; set; }
    //public int Index { get; set; }

    //public InventoryEntryUI prevEntry;

    void Start()
    {
        //Owner = GetComponentInParent<InventoryUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //right click an item
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //if (Owner.inventory.items[InventoryEntry] != null)
            if (this.icon.gameObject.activeSelf)
                    Owner.ObjectRightClicked(Owner.inventory.items[InventoryEntry]);
            
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Owner.ObjectHoveredEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        Owner.ObjectHoverExited(this);
    }

    public void UpdateEntry()
    {
        var entry = Owner.inventory.items[InventoryEntry];
        bool isEnabled = entry != null && entry.item != null;

        //icon.SetActive(isEnabled);
        icon.gameObject.SetActive(isEnabled);
        ItemCount.gameObject.SetActive(isEnabled);

        if (isEnabled)
        {
            if (entry.item != null) icon.sprite = entry.item.icon;
            ItemCount.text = entry.count.ToString();
            if (entry.count > 1)
            {
                ItemCount.gameObject.SetActive(true);
                
            }
            else
            {
                ItemCount.gameObject.SetActive(false);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        Owner.CurrentlyDragged = new InventoryUI.DragData();
        Owner.CurrentlyDragged.DraggedEntry = this;
        Owner.CurrentlyDragged.OriginalParent = (RectTransform)transform.parent;

        transform.SetParent(Owner.DragCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        transform.localPosition = transform.localPosition + UnscaleEventDelta(eventData.delta);
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
}