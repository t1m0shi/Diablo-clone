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
		if (mainH != null && attackCooldownMainH <= 0)
		{
			Debug.Log("attacking with mh");
			attackCooldownMainH = myStats.CalcAttackSpeed(mainH);
			//StartCoroutine(DoDamage(target, attackCooldownMainH, myStats.mainHand));
			int damage = UnityEngine.Random.Range((int)mainH.damage.min, (int)mainH.damage.max + 1);
			myStats.DealDamage(target, damage, mainH.damage.type);
			lastHit = Time.time;
			//inCombat = true;
		}
		if (offH != null && attackCooldownOffH <= 0)
		{
			Debug.Log("attacking with oh");
			attackCooldownOffH = myStats.CalcAttackSpeed(offH);
			//StartCoroutine(DoDamage(target, attackCooldownOffH, myStats.offHand));
			int damage = UnityEngine.Random.Range((int)offH.damage.min, (int)offH.damage.max + 1);
			myStats.DealDamage(target, damage, mainH.damage.type);
			lastHit = Time.time;
			//inCombat = true;
		}
	}
}
