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
	}
	void Update()
	{
		// Unequip all items if we press U
		/*
		if (Input.GetKeyDown(KeyCode.U))
			UnequipAll();
		*/
	}

	#endregion
	
	//public Equipment[] defaultEquipment;

	public SkinnedMeshRenderer targetMesh; //should be player's mesh
	public SkinnedMeshRenderer[] currentMeshes;

	public Transform sheathL;
	public Transform sheathR;
	public Transform sheath2L;
	public Transform sheath2R;

	//[SerializeField]
	public Equipment[] currentEquipment;   // Items we currently have equipped
										   //public List<Equipment> currentEquipment;
	//public Weapon[] currentWeapons;

	// Callback for when an item is equipped/unequipped
	public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
	public OnEquipmentChanged onEquipmentChanged;
	public delegate void OnWeaponChanged(Weapon newItem, Weapon weapon);
	public OnWeaponChanged onWeaponChanged;

	[SerializeField]
	PlayerAnimator pa;

	Inventory inventory;	// Reference to our inventory


	// Equip a new item from inventory
	public void Equip (Equipment newItem, int inventoryIndex)
	{
		// Find out what slot the item fits in
		int equipIndex = (int)newItem.equipSlot;
		//check what's currently equpped in that slot
		Equipment oldItem = currentEquipment[equipIndex];

		//there's already an item in the slot, so they swap
		if (oldItem != null)
		{
			SwapEquipment(newItem, oldItem, inventoryIndex);
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
	}

	//remove an item from equipped items
	public bool Unequip(Equipment toUnequip, int equipIndex)
    {
		//Equipment oldItem = null;
		if (toUnequip != null)
        {
			if (toUnequip.GetType() == typeof(Weapon))
			{
				return Unequip((Weapon)toUnequip, equipIndex);
			}
			//int equipIndex = (int)toUnequip.equipSlot;
			//if there's room to unequip
			if (inventory.AddItem(toUnequip, 1))
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
		//return oldItem;
    }
	public bool Unequip(Equipment toUnequip, int equipIndex, int invIndex)
	{
		//Equipment oldItem = null;
		if (toUnequip != null)
		{
			if (toUnequip.GetType() == typeof(Weapon))
			{
				return Unequip((Weapon)toUnequip, equipIndex, invIndex);
			}
			//int equipIndex = (int)toUnequip.equipSlot;
			//if there's room to unequip
			if (inventory.items[invIndex].item == null)
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
		}
		return false;
		//return oldItem;
	}

	public void SwapEquipment(Equipment newItem, Equipment oldItem, int inventoryIndex)
    {
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
	}
	public void SwapWeapon(Weapon newItem, Weapon oldItem, int inventoryIndex, int mainhand)
    {
		RemoveFromBone(oldItem);
		int newSlot = (int)newItem.equipSlot + mainhand;
		currentEquipment[newSlot] = newItem;
		if (mainhand == 0)
        {
			PlayerStats.instance.mainHand = newItem;
			if (newItem.handed == 2)
			{
				newItem.grip = sheath2R;
			}
			else
				newItem.grip = sheathR;
        }
        else
        {
			PlayerStats.instance.offHand = newItem;
			if (newItem.handed == 2)
			{
				newItem.grip = sheath2L;
			}
			else
				newItem.grip = sheathL;
		}
		
		inventory.items[inventoryIndex].item = oldItem;
		//AttachToMesh(newItem, (int)newItem.equipSlot);
		AttachToBone(newItem);
	}
	public void SwapWeapon(Weapon current, Weapon other)
    {
		RemoveFromBone(current);
		RemoveFromBone(other);
		if (current.grip == PlayerStats.instance.mainHand)
		{
			PlayerStats.instance.mainHand = other;
			PlayerStats.instance.offHand = current;
		}
		else
		{
			PlayerStats.instance.mainHand = current;
			PlayerStats.instance.offHand = other;
		}
		AttachToBone(current);
		AttachToBone(other);
	}

	public void RemoveFromBone(Weapon oldItem, Transform bone=null)
    {
		if (oldItem != null)
		{
			if (bone == null)
			{
				if (!pa.sheathed)
					pa.SheathAll();
				Destroy(oldItem.grip.GetChild(0).gameObject);
				
				/*
				if (oldItem.equipSlot == EquipmentSlot.MainHand)
				{
					if (oldItem.handed == 1)
					{
						Destroy(sheathL.GetChild(0).gameObject);
					}
					else
					{
						Destroy(sheath2R.GetChild(0).gameObject);
					}
				}
				else
				{
					if (oldItem.handed == 1)
					{
						Destroy(sheathR.GetChild(0).gameObject);
					}
					else
					{
						Destroy(sheath2L.GetChild(0).gameObject);
					}
				}
				*/

			}
			else
			{
				Destroy(bone.GetChild(0).gameObject);
			}
		}
	}

	public bool Equip(Weapon newItem, int inventoryIndex, int mainHand=0)
    {
		PlayerStats player = PlayerStats.instance;
		if (player.WillHandsBeTooFull(newItem.handed))
		{
			return false;
		}
		// Find out what slot the item fits in
		int equipIndex = (int)newItem.equipSlot;
		Weapon oldItem = null;

		//hands are empty
		if (player.handsFull == 0)
        {
			//put on new item in mainhand
			currentEquipment[equipIndex] = newItem;
			if (newItem.handed == 2)
				newItem.grip = sheath2R;
			else
				newItem.grip = sheathL;
			player.mainHand = newItem;
			//take it out of inventory
			inventory.RemoveItem(inventoryIndex);
		}
        else
        {
			//currently have a weapon equipped
			//only have 1 hander equipped, so we equip in offhand (should only allow 1h here)
			if (player.handsFull == 1)
            {
				currentEquipment[equipIndex + 1] = newItem;
				newItem.grip = sheathR;
				player.offHand = newItem;
				inventory.RemoveItem(inventoryIndex);
            }
			//currently have a 2 hander, or 2 1 handers
			else if (player.handsFull == 2 && player.maxHands == 2)
            {
				oldItem = (Weapon)currentEquipment[equipIndex+mainHand]; //check what's in main hand or offhand
				//replacing a 2h with another 2h or a 1h
				if (newItem.handed <= oldItem.handed)
				{ 
					SwapWeapon(newItem, oldItem, inventoryIndex, 0);
					if (newItem.handed == 2)
                    {
						newItem.grip = sheath2L;
                    }
                    else
                    {
						newItem.grip = sheathR;
                    }
                }
				//replacing a 1h with a 2h, gotta unequip the offhand first
				else
				{
					if (Unequip((Weapon)currentEquipment[equipIndex + 1], equipIndex +1))
					{
						SwapWeapon(newItem, oldItem, inventoryIndex, 0);
					}
					else
						return false;
                }
            }
			//currently have 2 2h's or a 2h and a shield?
            else
            {

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
		AttachToBone(newItem);
		return true;
	}

	public bool Unequip(Weapon toUnequip, int equipIndex)
    {
		//Weapon oldItem = null;
		if (toUnequip != null)
		{
			
			//int equipIndex = (int)toUnequip.equipSlot+mainhand;
			//if there's room to unequip
			if (inventory.AddItem(toUnequip, 1))
			{
				/*
				SetBlendShapeWeight(toUnequip, 0);
				//destroy the mesh
				if (currentMeshes[equipIndex] != null)
				{
					Destroy(currentMeshes[equipIndex].gameObject);
				}
				*/
				RemoveFromBone(toUnequip);
				if (equipIndex == (int)EquipmentSlot.Weapon)
				{
					PlayerStats.instance.mainHand = null;
				}
				else
				{
					PlayerStats.instance.offHand = null;
				}
				toUnequip.grip = null;
				currentEquipment[equipIndex] = null;
				// Equipment has been removed so we trigger the callback
				if (onWeaponChanged != null)
				{
					onWeaponChanged.Invoke(null, toUnequip);
				}
				//oldItem = toUnequip;
				return true;
			}
		}
		return false;
		//return oldItem;
	}
	public bool Unequip(Weapon toUnequip, int equipIndex, int invIndex)
	{
		//Weapon oldItem = null;
		if (toUnequip != null)
		{

			//int equipIndex = (int)toUnequip.equipSlot+mainhand;
			//if there's room to unequip
			if (inventory.items[invIndex].item == null)
			{
				inventory.items[invIndex].item = toUnequip;
				/*
				SetBlendShapeWeight(toUnequip, 0);
				//destroy the mesh
				if (currentMeshes[equipIndex] != null)
				{
					Destroy(currentMeshes[equipIndex].gameObject);
				}
				*/
				RemoveFromBone(toUnequip);
				if (equipIndex == (int)EquipmentSlot.Weapon)
                {
					PlayerStats.instance.mainHand = null;
                }
                else
                {
					PlayerStats.instance.offHand = null;
				}
				toUnequip.grip = null;
				currentEquipment[equipIndex] = null;
				// Equipment has been removed so we trigger the callback
				if (onWeaponChanged != null)
				{
					onWeaponChanged.Invoke(null, toUnequip);
				}
				//oldItem = toUnequip;
				return true;
			}
		}
		return false;
		//return oldItem;
	}
	// Unequip all items
	/*
	public void UnequipAll ()
	{
		for (int i = 0; i < currentEquipment.Length; i++)
		{
			Unequip(currentEquipment[i]);
		}

        //EquipDefaults();
	}*/

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
	public void AttachToBone(Weapon item, Transform bone=null)
    {
		GameObject g;// = Instantiate(item.representation, mainH.position, Quaternion.identity);
		Quaternion q = Quaternion.identity;//Quaternion.Euler(0, 0, 0);

		if (bone == null)
		{
			bone = item.grip;
			g = Instantiate(item.representation, bone.position, q);//Quaternion.identity);
			g.transform.SetParent(bone);
			/*
			if (item.equipSlot == EquipmentSlot.MainHand)
			{
				if (item.handed == 1)
				{
					g = Instantiate(item.representation, sheathL.position, q);//Quaternion.identity);
					g.transform.SetParent(sheathL);
				}
                else
                {
					g = Instantiate(item.representation, sheath2R.position, q);//Quaternion.identity);
                    g.transform.SetParent(sheath2R);
					var v = new Vector3(10, 145, 110);
					g.transform.localRotation = Quaternion.Euler(v);
				}
			}
			else
			{
				if (item.handed == 1)
				{
					g = Instantiate(item.representation, sheathR.position, q);
					g.transform.SetParent(sheathR);
				}
                else
                {
					g = Instantiate(item.representation, sheath2L.position, q);//Quaternion.identity);
					g.transform.SetParent(sheath2L);
					var v = new Vector3(10, 145, 110);
					g.transform.localRotation = Quaternion.Euler(v);
				}
			}
			*/
			if (bone == sheath2R)
			{
				//g.transform.SetParent(sheath2R);
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
		}
		else
		{
			//q = Quaternion.Euler(90, 0, 0);


			g = Instantiate(item.representation, bone.position, q);//Quaternion.identity);
			g.transform.SetParent(bone);
			var v = new Vector3(0, 0, 180);
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
