using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Keeps track of enemy stats, loosing health and dying. */

public class EnemyStats : CharacterStats {

	private int baseExp = 50;
	public float baseSpeed = 4f;
	private EnemyController aggro;

    private void Start()
    {
		level *= GameManager.instance.DifficultyTier;
		baseExp = GameManager.instance.DifficultyTier * baseExp;
		secondary[SecondaryStatType.MoveSpeed].baseValue = baseSpeed;
		aggro = GetComponent<EnemyController>();
	}
    public override void Die()
	{
		base.Die();

		PlayerStats.instance.onEnemyDeath.Invoke(baseExp);
		GetComponent<Enemy>().Die();
	}

    public override void DealDamage(CharacterStats target, int damage, DmgType type)
    {
		target = (PlayerStats)target;
        base.DealDamage(target, damage, type);
    }

    public override void EnterCombat()
    {
        base.EnterCombat();
		//possibly aggro directly to the player
		if (aggro.isActiveAndEnabled)
			aggro.combat.inCombat = true;
    }
}
