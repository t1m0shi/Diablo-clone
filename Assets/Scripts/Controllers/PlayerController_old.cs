using UnityEngine.EventSystems;
using UnityEngine;

/* Controls the player. Here we choose our "focus" and where to move. */

[RequireComponent(typeof(PlayerMotor_old))]
public class PlayerController_old : MonoBehaviour {
	/*

	public Interactable focus;	// Our current focus: Item, Enemy etc.

	public LayerMask movementMask;	// Filter out everything not walkable

	Camera cam;			// Reference to our camera
	PlayerMotor motor;  // Reference to our motor

	private Highlightable highlightable = null;
	private Highlightable prev_highlight = null;


	// Get references
	void Start () {
		cam = Camera.main;
		motor = GetComponent<PlayerMotor>();
	}
	
	// Update is called once per frame
	void Update () {

		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}

		// We create a ray
		RaycastHit hit;
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		
		//hovering
		if (Physics.Raycast(ray, out hit))
		{
			highlightable = hit.collider.GetComponent<Highlightable>();
			//if it's highlightable and you mouse over it
			if (highlightable != null)
			{
				if (highlightable != prev_highlight)
				{
					if (prev_highlight != null) prev_highlight.OnHoverExit();
					highlightable.OnHoverEnter();
					prev_highlight = highlightable;
				}
			}
			else if (prev_highlight != null)
			{
				prev_highlight.OnHoverExit();
				prev_highlight = null;
			}
        }
        

		// If we press left mouse
		//if (Input.GetMouseButtonDown(0))
		if (Input.GetMouseButton(0))
		{
			
			// If the ray hits
			if (Physics.Raycast(ray, out hit, 100, movementMask))
			{
				motor.MoveToPoint(hit.point);   // Move to where we hit
				RemoveFocus();
			}
			if (Physics.Raycast(ray, out hit, 100))
            {
				Interactable interactable = hit.collider.GetComponent<Interactable>();
				if (interactable != null)
				{
				SetFocus(interactable);
				}
			}
		}


		
		/*
		// If we press right mouse
		if (Input.GetMouseButtonDown(1))
		{
			// We create a ray
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			// If the ray hits
			if (Physics.Raycast(ray, out hit, 100))
			{
				Interactable interactable = hit.collider.GetComponent<Interactable>();
				if (interactable != null)
				{
					SetFocus(interactable);
				}
			}
		}
	}

	// Set our focus to a new focus
	void SetFocus (Interactable newFocus)
	{
		// If our focus has changed
		if (newFocus != focus)
		{
			// Defocus the old one
			if (focus != null)
				focus.OnDefocused();

			focus = newFocus;	// Set our new focus
			motor.FollowTarget(newFocus);	// Follow the new focus
		}
		
		newFocus.OnFocused(transform);
	}

	// Remove our current focus
	void RemoveFocus ()
	{
		if (focus != null)
			focus.OnDefocused();

		focus = null;
		motor.StopFollowingTarget();
	}
*/
}
