using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/* Keep track of equipment. Has functions for adding and removing items. */

public class EquipmentManager : MonoBehaviour {

	#region Singleton
	public static EquipmentManager instance;
	
	

	void Awake ()
	{
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of EquipManager found!");
			return;
		}
		instance = this;

		//inventory = Inventory.instance;     // Get a reference to our inventory

		// Initialize currentEquipment based on number of equipment slots
		int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length+1;
		//currentEquipment = new List<Equipment>();
		currentEquipment = new Equipment[numSlots];
		//currentWeapons = new Weapon[2];
		currentMeshes = new SkinnedMeshRenderer[numSlots];
	}
    private void Start()
    {
		inventory = Inventory.instance;
		sheathL = pa.sheathL;
		sheathR = pa.sheathR;
		sheath2R = pa.sheath2R;
		sheath2L = pa.sheath2L;
	}

	#endregion
	
	//public Equipment[] defaultEquipment;

	public SkinnedMeshRenderer targetMesh; //should be player's mesh
	public SkinnedMeshRenderer[] currentMeshes;

	private Transform sheathL;
	private Transform sheathR;
	private Transform sheath2L;
	private Transform sheath2R;

	//[SerializeField]
	public Equipment[] currentEquipment;   // Items we currently have equipped
										   //public List<Equipment> currentEquipment;
	//public Weapon[] currentWeapons;

	// Callback for when an item is equipped/unequipped
	public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
	public OnEquipmentChanged onEquipmentChanged;
	public delegate void OnWeaponChanged(Weapon newItem, Weapon oldItem);
	public OnWeaponChanged onWeaponChanged;

	[SerializeField]
	PlayerAnimator pa;

	Inventory inventory;	// Reference to our inventory

	// Equip a new item from inventory
	public bool Equip (Equipment newItem, int inventoryIndex, int equipIndex)//, bool dragged = true)
	{
		if (newItem.GetType() == typeof(Weapon))
        {
			return Equip((Weapon)newItem, inventoryIndex, equipIndex);//, dragged);
        }
		// Find out what slot the item fits in
		//int equipIndex = (int)newItem.equipSlot;
		//check what's currently equpped in that slot
		Equipment oldItem = currentEquipment[equipIndex];

		//there's already an item in the slot, so they swap
		if (oldItem != null)
		{
			SwapEquipment(newItem, oldItem, inventoryIndex, equipIndex);
		}
		//if there's nothing in that slot, then we just equip and we're done (after cleanup)
		else
		{
			//put on new item
			currentEquipment[equipIndex] = newItem;
			//take it out of inventory
			inventory.RemoveItem(inventoryIndex);
		}
		//cleanup
		// An item has been equipped so we trigger the callback
		if (onEquipmentChanged != null)
		{
			onEquipmentChanged.Invoke(newItem, oldItem);
		}
        AttachToMesh(newItem, equipIndex);
		return true;
	}
	// Drag or click an item from inventory
	public bool Equip(Weapon newItem, int inventoryIndex, int equipIndex)//, bool dragged=true)
	{
		PlayerStats player = PlayerStats.instance;
		if (player.WillHandsBeTooFull(newItem.handed))
		{
			return false;
		}
		Weapon oldItem = (Weapon)currentEquipment[equipIndex];
		//hands are empty
		if (player.handsFull <= 1)
		{
			//put on new item in that slot
			currentEquipment[equipIndex] = newItem;
			if (newItem.handed == 2)
			{
				if (equipIndex == (int)EquipmentSlot.Weapon)
					newItem.sheath = sheath2R;
				else
					newItem.sheath = sheath2L;
			}
			else
			{
				if (equipIndex == (int)EquipmentSlot.Weapon)
					newItem.sheath = sheathL;
				else
					newItem.sheath = sheathR;
			}
			//take it out of inventory
			inventory.RemoveItem(inventoryIndex);
			
		}
		//currently have a 2 hander, or 2 1 handers
		else if (player.handsFull == 2 && player.maxHands == 2)
		{
			 //check what's in main hand or offhand
			//replacing a 2h with another 2h or a 1h
			if (newItem.handed <= oldItem.handed)
			{
				//SwapWeapon(newItem, oldItem, inventoryIndex);
				//Unequip(oldItem, equipIndex);
				//Equip(newItem, inventoryIndex, equipIndex, dragged);
				SwapEquipment(newItem, oldItem, inventoryIndex, equipIndex);
			}
			//replacing a 1h with a 2h, gotta unequip the other hand first
			else
			{
				if (equipIndex == (int)EquipmentSlot.Weapon && Unequip((Weapon)currentEquipment[equipIndex + 1], equipIndex+1)) //mainhand
				{
					SwapEquipment(newItem, oldItem, inventoryIndex, equipIndex);
				}
				else if (equipIndex == (int)EquipmentSlot.Weapon+1 && Unequip((Weapon)currentEquipment[equipIndex - 1], equipIndex - 1)) //offhand
				{
					SwapEquipment(newItem, oldItem, inventoryIndex, equipIndex);
				}
				else
					return false;
			}
		}
		//currently have 2 2h's or a 2h and a shield?
		else
		{
			//swap the offhand only
			if (newItem.wtype == WeaponType.Shield)// && currentEquipment[(int)EquipmentSlot.Weapon + 1] == null)
			{
				SwapEquipment(newItem, currentEquipment[(int)EquipmentSlot.Weapon + 1], inventoryIndex, (int)EquipmentSlot.Weapon + 1);
            }
		}
		
		
		/**
		//there's already an item in the slot, so they swap if they're the same weight or switching in a 1h for a 2h
		if (oldItem != null && (newItem.handed == oldItem.handed || newItem.handed < oldItem.handed))
		{
			SwapWeapon(newItem, oldItem, inventoryIndex);
		}
		//else if (oldItem != null 
		//equipping a 2h over a 1h, always going to be mainHand (or is it???)
		else if (oldItem != null && newItem.handed > oldItem.handed)
        {

			
			if (newItem.equipSlot == EquipmentSlot.MainHand)
            {
				if ((Weapon)currentEquipment[(int)EquipmentSlot.OffHand] != null)
                {
					if (PlayerStats.instance.AreHandsFull()) //if hands are full, gotta unequip the offhand 
                    {
						if (Unequip((Weapon)currentEquipment[(int)EquipmentSlot.OffHand])) //if there's room in the inventory to also unequip offhand
						{
							SwapWeapon(newItem, oldItem, inventoryIndex);
						}
						else
							return false;
                    }
                }
                else //no offhand equipped
                {
					SwapWeapon(newItem, oldItem, inventoryIndex);
                }
            }
            else //2h in offhand??
            {

            }
			
        }
		//if there's nothing in that slot, then we just equip and we're done (after cleanup)
		else
		{
			//put on new item
			currentEquipment[equipIndex] = newItem;
			//take it out of inventory
			inventory.RemoveItem(inventoryIndex);
		}
		*/
		//cleanup
		// An item has been equipped so we trigger the callback
		if (onWeaponChanged != null)
		{
			onWeaponChanged.Invoke(newItem, oldItem);
		}
		//AttachToMesh(newItem, equipIndex);
		AttachToBone(newItem, newItem.sheath);
		return true;

	}

	//remove an item from equipped items, invIndex = -1 if just rightclicked
	public bool Unequip(Equipment toUnequip, int equipIndex, int invIndex=-1)
    {
		if (toUnequip != null)
        {
			//weapons have different ways of taking care of mesh
			if (toUnequip.GetType() == typeof(Weapon))
			{
				return Unequip((Weapon)toUnequip, equipIndex, invIndex);
			}
			if (invIndex != -1 && inventory.items[invIndex].item == null)
			{
				inventory.items[invIndex].item = toUnequip;

				SetBlendShapeWeight(toUnequip, 0);
				//destroy the mesh
				if (currentMeshes[equipIndex] != null)
				{
					Destroy(currentMeshes[equipIndex].gameObject);
				}
				currentEquipment[equipIndex] = null;
				// Equipment has been removed so we trigger the callback
				if (onEquipmentChanged != null)
				{
					onEquipmentChanged.Invoke(null, toUnequip);
				}
				//oldItem = toUnequip;
				return true;
			}
			//if there's room to unequip
			else if (inventory.AddItem(toUnequip, 1))
			{
				SetBlendShapeWeight(toUnequip, 0);
				//destroy the mesh
				if (currentMeshes[equipIndex] != null)
				{
					Destroy(currentMeshes[equipIndex].gameObject);
				}
				currentEquipment[equipIndex] = null;
				// Equipment has been removed so we trigger the callback
				if (onEquipmentChanged != null)
				{
					onEquipmentChanged.Invoke(null, toUnequip);
				}
				//oldItem = toUnequip;
				return true;
			}
		}
		return false;
    }
	public bool Unequip(Weapon toUnequip, int equipIndex, int invIndex)
	{
		if (toUnequip != null)
		{
			//drag it into an open slot in inventory
			if (invIndex != -1 && inventory.items[invIndex].item == null)
			{
				inventory.items[invIndex].item = toUnequip;
				currentEquipment[equipIndex] = null;
				RemoveFromBone(toUnequip.sheath);
				// Equipment has been removed so we trigger the callback
				if (onWeaponChanged != null)
				{
					onWeaponChanged.Invoke(null, toUnequip);
				}
				return true;
			}
			//if there's room to unequip (right clicked)
			else if (invIndex == -1 && inventory.AddItem(toUnequip, 1))
			{
				currentEquipment[equipIndex] = null;
				RemoveFromBone(toUnequip.sheath);
				// Equipment has been removed so we trigger the callback
				if (onWeaponChanged != null)
				{
					onWeaponChanged.Invoke(null, toUnequip);
				}
				return true;
			}
		}
		return false;
	}
	//moving from equipment to inventory
	public void SwapEquipment(Equipment newItem, Equipment oldItem, int inventoryIndex, int equipIndex)
    {
		if (newItem.GetType() == typeof(Weapon))
        {
			SwapWeapon((Weapon)newItem, (Weapon)oldItem, inventoryIndex, equipIndex);
			return;
        }
		//Debug.Log("swapping equipment");
		SetBlendShapeWeight(oldItem, 0);
		//destroy the mesh
		
		if (currentMeshes[(int)oldItem.equipSlot] != null)
		{
			Destroy(currentMeshes[(int)oldItem.equipSlot].gameObject);
		}
		currentEquipment[(int)newItem.equipSlot] = newItem;
		inventory.items[inventoryIndex].item = oldItem;
		AttachToMesh(newItem, (int)newItem.equipSlot);
		if (onEquipmentChanged != null)
		{
			onEquipmentChanged.Invoke(newItem, oldItem);
		}
	}
	//moving from equipment to inventory
	public void SwapWeapon(Weapon newItem, Weapon oldItem, int inventoryIndex, int equipIndex)
    {
		RemoveFromBone(oldItem.sheath);
		currentEquipment[equipIndex] = newItem;
		inventory.items[inventoryIndex].item = oldItem;
		newItem.sheath = oldItem.sheath;
		AttachToBone(newItem, newItem.sheath);
		if (onWeaponChanged != null)
		{
			onWeaponChanged.Invoke(newItem, oldItem);
		}
	}
	//swapping while in equipment
	public void SwapWeapons(Weapon current, Weapon other, int curIndex, int oIndex)
    {
		currentEquipment[curIndex] = other;
		currentEquipment[oIndex] = current;
		RemoveFromBone(current.sheath);
		RemoveFromBone(other.sheath);
		Transform temp = current.sheath;
		current.sheath = other.sheath;
		other.sheath = temp;
		AttachToBone(current, current.sheath);
		AttachToBone(current, other.sheath);
	}

	public void RemoveFromBone(Transform bone)
    {
		Destroy(bone.GetChild(0).gameObject);
	}

	void AttachToMesh(Equipment item, int slotIndex)
	{
		SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(item.smesh);
		newMesh.BakeMesh(item.smesh.sharedMesh);
		newMesh.transform.parent = targetMesh.transform.parent;

		newMesh.rootBone = targetMesh.rootBone;
		newMesh.bones = targetMesh.bones;

		currentMeshes[slotIndex] = newMesh;

		SetBlendShapeWeight(item, 100);
		
	}
	public void AttachToBone(Weapon item, Transform bone)
    {
		Quaternion q = Quaternion.identity;
		GameObject g = Instantiate(item.representation, bone.position, q);
		g.transform.SetParent(bone);

		if (bone == sheath2R)
		{
			var v = new Vector3(10, 145, 110);
			g.transform.localRotation = Quaternion.Euler(v);
		}
		else if (bone == sheath2L)
        {
			var v = new Vector3(10, -145, 110);
			g.transform.localRotation = Quaternion.Euler(v);
		}
		else if (bone == sheathL)
        {

        }
		else if (bone == sheathR)
        {

        }
		
		else
		{
			var v = new Vector3(90, 0, 0);
			g.transform.localRotation = Quaternion.Euler(v);
		}
	}

    void SetBlendShapeWeight(Equipment item, int weight)
    {
		
		foreach (MeshBlendShape blendshape in item.coveredMeshRegions)
		{
			int shapeIndex = (int)blendshape;
            targetMesh.SetBlendShapeWeight(shapeIndex, weight);
		}
		
    }

	public enum MeshBlendShape { Head, Shoulders, Torso, Hands, Legs, Feet };
	/*
    void EquipDefaults()
    {
		foreach (Equipment e in defaultEquipment)
		{
			//Equip(e);
		}
    }
	*/



	#region gui
#if UNITY_EDITOR

	[CustomPropertyDrawer(typeof(Equipment))]
	public class EquipmentDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			try
			{
				int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
				EditorGUI.ObjectField(rect, property, new GUIContent(((EquipmentSlot)pos).ToString()));
			}
            catch
            {
				EditorGUI.ObjectField(rect, property, label);
			}
		}
	}
	#endif
	#endregion
	
}
