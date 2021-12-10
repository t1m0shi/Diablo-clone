using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class Combat : MonoBehaviour
{
    public float attackCooldownMainH = 0f;
    public float attackCooldownOffH = 0f;
    public bool inCombat = false;
    public float lastHit;

	protected virtual void Start()
	{
		//myStats = GetComponent<CharacterStats>();
	}

	protected virtual void Update()
	{
		if (attackCooldownMainH > 0)
			attackCooldownMainH -= Time.deltaTime;
		if (attackCooldownOffH > 0)
			attackCooldownOffH -= Time.deltaTime;
		//exit combat if no attacks in 3 seconds
		
	}

	public virtual void AutoAttack(CharacterStats target)
	{
		
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
