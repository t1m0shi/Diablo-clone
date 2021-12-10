using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/* Controls the Enemy AI */
[RequireComponent(typeof(EnemyStats))]
public class EnemyController : MonoBehaviour {

	public float lookRadius = 10f;	// Detection range for player

	Transform target;	// Reference to the player
	NavMeshAgent agent; // Reference to the NavMeshAgent
	public PlayerCombat combat;

	// Use this for initialization
	void Start () {
		target = GameObject.FindGameObjectWithTag("Player").transform;
		agent = GetComponent<NavMeshAgent>();
		agent.speed = GetComponent<EnemyStats>().secondary[SecondaryStatType.MoveSpeed].GetValue();
		combat = GetComponent<PlayerCombat>();
	}
	
	// Update is called once per frame
	void Update () {
		// Distance to the target
		float distance = Vector3.Distance(target.position, transform.position);

		// If inside the lookRadius
		if (distance <= lookRadius || combat.inCombat)
		{
			// Move towards the target
			agent.SetDestination(target.position);

			// If within attacking distance
			if (distance <= agent.stoppingDistance)
			{
				PlayerStats targetStats = target.GetComponent<PlayerStats>();
				if (targetStats != null)
				{
					//Debug.Log("attacking");
					combat.AutoAttack(targetStats);
				}

				FaceTarget();	// Make sure to face towards the target
			}
		}
	}

	// Rotate to face the target
	void FaceTarget ()
	{
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
	}

	// Show the lookRadius in editor
	void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, lookRadius);
	}
}
