using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	#region Singleton

	public static Inventory instance;

	void Awake ()
	{
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of Inventory found!");
			return;
		}
		instance = this;
		space = slots;
        for (int i = 0; i < items.Length; i++)
        {
			items[i].inventory_index = i;
        }
	}
	#endregion

	public int slots = 24;  // Amount of slots in inventory
	public int space;
	public InventoryEntry[] items;// = new InventoryEntry[20];

    // Callback which is triggered when
    // an item gets added/removed.
    public delegate void OnItemChanged();
	public OnItemChanged onItemChangedCallback;
	
	[Serializable]
	public class InventoryEntry
    {
		public int count;
		public Item item;
		public int inventory_index;
    }

	/// <summary>
	/// Add an item to the inventory. This will look if this item already exist in one of the slot and increment the
	/// stack counter there instead of using another slot.
	/// </summary>
	/// <param name="item">The item to add to the inventory</param>
	public bool AddItem(Item item, int count)
	{
		InventoryEntry e = new InventoryEntry();
		e.item = item;
		e.count = count;
		//is the item stackable? (equipment vs consumable)
		if (item.stackSize > 1)
		{
			//does the inventory already contain an item of this type already with available stacks?
			for (int i = 0; i < slots; i++)
			{
				InventoryEntry d = items[i];
				if (d != null)
				{
					if (d.item == item)// && e.count + count <= e.item.stackSize)
					{
						//what if there's only enough for part of the stack? should fill the stack, then split and search again
						if (MergeStack(e, d))
                        {
							//is there still more in e's stack?
							if (e.count > 0)
                            {
								return AddItem(e.item, e.count);
                            }
							if (onItemChangedCallback != null) onItemChangedCallback.Invoke();
							return true;
						}
					}
				}
			}
		}
		//is there more room?
		if (space > 0)
        {
			//look for the first empty slot
			for (int i=0; i < slots; i++)
            {
				//slot is empty
				if (items[i].item == null)
                {
					//replace the entry in the inventory with the new item
					e.inventory_index = i;
					items[i] = e;
					space -= 1;
					if (onItemChangedCallback != null) onItemChangedCallback.Invoke();
					return true;
				}
            }
        }
		Debug.Log("Inventory full!");
		return false;
	}

	/// <summary>
	/// This will *try* to use the item. If the item return true when used, this will decrement the stack count and
	/// if the stack count reach 0 this will free the slot. If it return false, it will just ignore that call.
	/// (e.g. a potion will return false if the user is at full health, not consuming the potion in that case)
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	public bool UseItem(InventoryEntry item, int equipIndex=-1)
	{
        bool wasUsed = item.item.Use(item.inventory_index, equipIndex);
        if (wasUsed) 
		{
			if (onItemChangedCallback != null) onItemChangedCallback.Invoke();
		}
		return wasUsed;
	}

	public void RemoveItem(int index)
    {
		items[index].item = null;
		items[index].count = 0;
		space++;
		// Trigger callback
		if (onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
	}

	public bool AddItemAt(InventoryEntry entry, int index)
    {
		bool added = false;
		//if the slot is empty, or we can add into the stack
		if (items[index].item == null || (items[index].item == entry.item && items[index].count + entry.count <= items[index].item.stackSize))
        {
			items[index].item = entry.item;
			items[index].count += entry.count;
			added = true;
			if (onItemChangedCallback != null) onItemChangedCallback.Invoke();
        }
		return added;
    }

	//merge two stacks of the same item, return false if unable to add even one more into the second stack
	public bool MergeStack(InventoryEntry stack1, InventoryEntry stack2)
    {
		bool merged = false;

		//move stack1 into stack2
		//check how many more can fit into the stack
		int fit = stack2.item.stackSize - stack2.count;
		//if there's room, add some in
		if (fit > 0)
        {
			int toAdd = Mathf.Clamp(stack1.count, 1, fit);
			stack2.count += toAdd;
			stack1.count -= toAdd;
			merged = true;
			if (onItemChangedCallback != null) onItemChangedCallback.Invoke();
        }

		return merged;
    }

	/* old //////////////
    // Current list of items in inventory
    //public List<Item> items = new List<Item>();


	// Remove an item
	public void Remove(Item item)
	{
		items.Remove(item);     // Remove item from list

		// Trigger callback
		if (onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
	}

    // Add a new item. If there is enough room we
    // return true. Else we return false.
    public bool Add (Item item)
	{
		// Don't do anything if it's a default item
		if (!item.isDefaultItem)
		{
			// Check if out of space
			if (items.Count >= slots)
			{
				Debug.Log("Not enough room.");
				return false;
			}

			items.Add(item);	// Add item to list

			// Trigger callback
			if (onItemChangedCallback != null)
				onItemChangedCallback.Invoke();
		}

		return true;
	}
	*/


}
