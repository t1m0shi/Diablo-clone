using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : Combat
{
    EnemyStats myStats;
	// Start is called before the first frame update
	protected override void Start()
    {
		base.Start();
		myStats = GetComponent<EnemyStats>();
    }

    // Update is called once per frame
    protected override void Update()
    {
		base.Update();
		if (inCombat && Time.time - lastHit > 3f)
		{
			inCombat = false;
		}
	}

    public override void AutoAttack(CharacterStats target)
    {
        base.AutoAttack(target);
		if (!inCombat)
		{
			inCombat = true;
		}
		target = (PlayerStats)target;
		if (myStats.mainHand != null && attackCooldownMainH <= 0)
		{
			Debug.Log("attacking with mh");
			attackCooldownMainH = myStats.CalcAttackSpeed(myStats.mainHand);
			//StartCoroutine(DoDamage(target, attackCooldownMainH, myStats.mainHand));
			int damage = UnityEngine.Random.Range((int)myStats.mainHand.damage.min, (int)myStats.mainHand.damage.max + 1);
			myStats.DealDamage(target, damage, myStats.mainHand.damage.type);
			lastHit = Time.time;
			//inCombat = true;
		}
		if (myStats.offHand != null && attackCooldownOffH <= 0)
		{
			Debug.Log("attacking with oh");
			attackCooldownOffH = myStats.CalcAttackSpeed(myStats.offHand);
			//StartCoroutine(DoDamage(target, attackCooldownOffH, myStats.offHand));
			int damage = UnityEngine.Random.Range((int)myStats.offHand.damage.min, (int)myStats.offHand.damage.max + 1);
			myStats.DealDamage(target, damage, myStats.offHand.damage.type);
			lastHit = Time.time;
			//inCombat = true;
		}
	}
}
