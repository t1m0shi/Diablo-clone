using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//is activated through the UI and uses combat to instantiate an autoattack based on the weapon type
public class AutoAttack : Ability.PlayerAbilityEffect.Attack
{
    public override void Activate(PlayerStats originator)
    {
        //Debug.Log("autod");
        base.Activate(originator);
		originator.combat.AutoAttack(); //changes based on weapon type equipped
		//then the prefab can check collisions to see who got hit
	}

}
