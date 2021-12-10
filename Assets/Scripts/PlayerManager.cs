using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/* Keeps track of the player */

public class PlayerManager : MonoBehaviour {

	#region Singleton

	public static PlayerManager instance;

	void Awake ()
	{
		instance = this;
		s_ui_parent = player.GetComponent<PlayerStats>().statsUI.parent;
	}

	#endregion

	public GameObject player;

	[SerializeField]
	private InventoryUI inv_ui;
	[SerializeField]
	private EquipmentUI eq_ui;
	[SerializeField]
	private StatsUI s_ui;
	private GameObject s_ui_parent;
	public PlayerState state = PlayerState.idle;
	public GameObject spellBook;// = new List<Ability>();
	[SerializeField]
	private GameObject skillTree;
	private PlayerCombat combat;
	public PlayerAnimator pa;

	public delegate void OnCombatEnter();
	public OnCombatEnter onCombatEnter;
	public delegate void OnCombatExit();
	public OnCombatEnter onCombatExit;

	//private Highlightable highlightable;
	//private Highlightable prev_highlight;

	private void Start()
    {
		onCombatEnter += CombatEnter;
		onCombatExit += CombatExit;
		combat = player.GetComponent<PlayerCombat>();
    }

    private void Update()
    {
		//open/close inventory and char stats window
		if (Input.GetButtonDown("Inventory"))
		{
			//isBrowsing = !isBrowsing;
			bool up = !inv_ui.gameObject.activeSelf;
			inv_ui.gameObject.SetActive(up);
			eq_ui.gameObject.SetActive(up);
			if (s_ui_parent.gameObject.activeSelf && !up) s_ui_parent.gameObject.SetActive(false);
			if (up) 
			{
				inv_ui.UpdateUI();
				eq_ui.UpdateUI(null, null);
				//s_ui.UpdateUI();
			}
		}

		//open/close char stats 
		if (Input.GetKeyDown(KeyCode.C))
        {
			bool up = !s_ui_parent.gameObject.activeSelf;
			s_ui_parent.gameObject.SetActive(up);
			//s_ui.UpdateUI();
		}

		//click on interactable thing (enemy, loot)
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (EventSystem.current.IsPointerOverGameObject()) return;
			// If the ray hits
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				Interactable interactable = hit.collider.GetComponent<Interactable>();
				if (interactable != null)
				{
					float distance = Vector3.Distance(player.transform.position, interactable.transform.position);
					if (distance <= interactable.interactDistance) interactable.Interact();
				}
			}
		}

		if (Input.GetButtonDown("Skills"))
        {
			skillTree.gameObject.SetActive(!skillTree.gameObject.activeSelf);
        }
		
		if (Input.GetButtonDown("Abilities"))
        {
			spellBook.SetActive(!spellBook.gameObject.activeSelf);
        }
	}


    public void KillPlayer ()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	void CombatEnter()
    {
		if (!combat.inCombat)
		{
			//state = PlayerState.inCombat;
			combat.inCombat = true;
			combat.lastHit = Time.time;
		}
    }
	void CombatExit()
    {
		//state = PlayerState.idle;
		combat.inCombat = false;
	}
}
public enum PlayerState { idle, talking, attacking }
