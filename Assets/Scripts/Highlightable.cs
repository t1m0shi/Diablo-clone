using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Outline))]
public class Highlightable : Interactable
{
    [SerializeField]
    private Outline outline;
    private Ray mouse;
    private Transform player;
    

    private void Start()
    {
        outline = GetComponent<Outline>();
        
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        mouse = Camera.main.ScreenPointToRay(Input.mousePosition);
        //hovering
        
        if (Physics.Raycast(mouse, out RaycastHit hit))//, interactDistance, mask))
        {
            if (hit.collider != null && hit.collider == gameObject.GetComponent<Collider>()
                && (Vector3.Distance(transform.position, player.position) <= interactDistance))
                OnHoverEnter();
            else
                OnHoverExit();
        }
    }

    public void OnHoverEnter()
    {
        outline.enabled = true;
    }

    public void OnHoverExit()
    {
        outline.enabled = false;
    }
}
