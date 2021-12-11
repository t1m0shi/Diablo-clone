using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Handles interaction with the Enemy */

[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EnemyController))]
[RequireComponent(typeof(EnemyCombat))]
public class Enemy : MonoBehaviour{//Interactable {

	[SerializeField]
	private EnemyStats myStats;

	private LootTable dropTable;
	//public List<GameObject> LootPrefabs;
	public GameObject LootPrefab;

	void Start ()
	{
		//playerManager = PlayerManager.instance;
		myStats = GetComponent<EnemyStats>();
		dropTable = new LootTable();
		dropTable.lootdrops = new List<LootDrop>()
			{
				new LootDrop(Rarity.Common, 100f),
				new LootDrop(Rarity.Uncommon, 40f),
				new LootDrop(Rarity.Rare, 20f),
				new LootDrop(Rarity.Epic, 5f),
				new LootDrop(Rarity.Legendary, 0.01f)
			};
		dropTable.Init();
		dropTable.lootdrops.Sort();
		//LootPrefabs = new List<GameObject>();
	}

	/* use with old movement system
	public override void Interact()
	{
		base.Interact();
		EnemyCombat playerCombat = playerManager.player.GetComponent<EnemyCombat>();
		if (playerCombat != null)
		{
			playerCombat.Attack(myStats);
		}
	}*/

	public void Die()
    {
		int num_loot = UnityEngine.Random.Range(1, myStats.level+1); //randomly drop x number of items based on level/difficulty
        for (int i = 0; i < num_loot; i++)
        {
			DropLoot();
		}
		//myStats.Die();

		// Add ragdoll effect / death animation

		//Destroy(gameObject);
	}

	public void DropLoot()
	{
		//create piece(s) of loot
		Item dropped =  dropTable.GetDrop();
		if (dropped != null) {
			dropped.ilevel = this.myStats.level;
			Vector3 spawn = this.GetComponent<Transform>().position;
			//drop it at a random point around me
			float randomangle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
			spawn = spawn + (new Vector3(Mathf.Sin(randomangle), 0.3f, Mathf.Cos(randomangle))).normalized;
			GameObject l = Instantiate(LootPrefab, spawn, Quaternion.Euler(-90, 0, 0));
			l.GetComponent<Loot>().item = dropped;
			if (dropped.GetType().IsSubclassOf(typeof(Equipment)) || dropped.GetType() == typeof(Equipment)) //dropped.GetType() == typeof(Equipment)) 
				l.GetComponent<Loot>().count = 1;
			else 
				l.GetComponent<Loot>().count = UnityEngine.Random.Range(1, Mathf.Clamp(GameManager.instance.DifficultyTier, 2, GameManager.instance.DifficultyTier)+1);
			if (dropped.GetType() == typeof(Equipment))
			{
				Equipment dropped_equipment = (Equipment)dropped;
				SkinnedMeshRenderer skinnedMeshRenderer = dropped_equipment.smesh;
				MeshFilter filter = l.AddComponent<MeshFilter>();
				MeshRenderer renderer = l.AddComponent<MeshRenderer>();
				filter.mesh = skinnedMeshRenderer.sharedMesh;
				renderer.sharedMaterials = skinnedMeshRenderer.sharedMaterials;
			}
			l.GetComponent<Outline>().renderer = l.GetComponent<MeshRenderer>();
		}
		else { Debug.Log("No loot dropped :("); } //shouldn't get here because of loot table I think
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.L)) Die();
	}
}


