using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCombat : Combat {

	//public float attackSpeed;// = 1f; //don't use, so we can keep attack speed updated without another method call

	//public float attackDelay = .6f;

	PlayerStats myStats;
	public Transform atkPoint;
	[SerializeField]
	private GameObject[] autoPrefabs;
	private GameObject currentAuto;
	[SerializeField]
	private GameObject defaultAuto;
	int mainSlot = (int)EquipmentSlot.Weapon;
	
	/*
	[Serializable]
	class Autos
    {
		public GameObject[] list;
    }*/

    protected override void Start ()
	{
		base.Start();
		myStats = GetComponent<PlayerStats>();
		//autoPrefabs = new GameObject[Enum.GetNames(typeof(WeaponType)).Length];
		mainH = (Weapon)EquipmentManager.instance.currentEquipment[mainSlot];
		offH = (Weapon)EquipmentManager.instance.currentEquipment[mainSlot+1];
		EquipmentManager.instance.onWeaponChanged += ChangeWeapons;
	}

	protected override void Update ()
	{
		base.Update();
		//exit combat if no attacks in 3 seconds
		if (inCombat && Time.time - lastHit > 3f)
        {
			//inCombat = false;
			//PlayerManager.instance.state = PlayerState.idle;
			PlayerManager.instance.onCombatExit.Invoke();
        }
	}

	public override void AutoAttack (CharacterStats target=null)
	{
		/*
		if (!inCombat)
        {
			PlayerManager.instance.onCombatEnter.Invoke();
        }*/
		//target = (EnemyStats)target;
		if (mainH != null && attackCooldownMainH <= 0)
        {
			Debug.Log("attacking with mh");
			attackCooldownMainH = myStats.CalcAttackSpeed(mainH);
			//StartCoroutine(DoDamage(target, attackCooldownMainH, myStats.mainHand));
			ChooseAttack(mainH);
			//int damage = UnityEngine.Random.Range((int)myStats.mainHand.damage.min, (int)myStats.mainHand.damage.max + 1);
			//myStats.DealDamage(target, damage, myStats.mainHand.damage.type);
			lastHit = Time.time;
			//inCombat = true;
		}
		if (offH != null && attackCooldownOffH <= 0)
        {
			Debug.Log("attacking with oh");
			attackCooldownOffH = myStats.CalcAttackSpeed(offH);
			//StartCoroutine(DoDamage(target, attackCooldownOffH, myStats.offHand));
			ChooseAttack(offH);
			//int damage = UnityEngine.Random.Range((int)myStats.offHand.damage.min, (int)myStats.offHand.damage.max + 1);
			//myStats.DealDamage(target, damage, myStats.offHand.damage.type);
			lastHit = Time.time;
			//inCombat = true;
		}
	}

	//instantiate the attack based on the weapon you're attacking with, which will take care of itself how it interacts with enemies
	private void ChooseAttack(Weapon weapon)
    {
		//atk = autoPrefabs[(int)WeaponType]
		if (weapon != null)
		{
			//in order of the weapon type enum
			//int a = UnityEngine.Random.Range(0, currentAuto.Length);
			//GameObject atk = currentAuto;
			if (weapon.wmode == WeaponMode.Melee)
            {
				MeleeAutoAttack m = currentAuto.GetComponent<MeleeAutoAttack>();
				m.weapon = weapon;
				m.origin = this.myStats;
			}
			else if (weapon.wmode == WeaponMode.Ranged)
            {
				RangedAutoAttack m = currentAuto.GetComponent<RangedAutoAttack>();
				m.weapon = weapon;
				m.origin = this.myStats;
			}
			
			//Quaternion q = atkPoint.rotation;
			//float r = atkPoint.position.z + q.normalized.z;
			//Vector3 v = new Vector3(atkPoint.position.x, atkPoint.position.y, r);
			Vector3 v = atkPoint.forward.normalized;
			GameObject b = Instantiate(currentAuto, atkPoint.position + v, atkPoint.rotation);
			b.transform.SetParent(atkPoint);
		}
	}

	private void ChangeWeapons(Weapon newItem, Weapon oldItem)
    {
		if (newItem != null)
        {
			if (newItem.wmode == WeaponMode.Melee)
			{
				currentAuto = autoPrefabs[(int)WeaponMode.Melee];
			}
			else if (newItem.wmode == WeaponMode.Ranged)
			{
				currentAuto = autoPrefabs[(int)WeaponMode.Ranged];
			}
		}
        else
        {
			currentAuto = defaultAuto;
        }
		mainH = (Weapon)EquipmentManager.instance.currentEquipment[mainSlot];
		offH = (Weapon)EquipmentManager.instance.currentEquipment[mainSlot + 1];
	}
	/*
	IEnumerator DoDamage (CharacterStats target, float delay, Weapon weap)
	{
		yield return new WaitForSeconds(delay);

		
		int damage = UnityEngine.Random.Range((int)weap.damage.min, (int)weap.damage.max + 1);
		Debug.Log("dealing" +damage+" dmg");
		myStats.DealDamage(target, damage, weap.damage.type);
	}
	*/
}
