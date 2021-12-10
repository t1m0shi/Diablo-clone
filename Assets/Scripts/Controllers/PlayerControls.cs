using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Timers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControls : MonoBehaviour
{
    public static PlayerControls instance;

    private float moveSpeed;
    [HideInInspector]
    public float speed;
    private PlayerStats player;
    private PlayerManager manager;
    Vector3 input, mouse;
    //public PlayerState state = PlayerState.idle;
    float rotateSpeed = 15f;
    Rigidbody mybody;
    //Ray mouseRay;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of EquipManager found!");
            return;
        }
        instance = this;
        player = GetComponent<PlayerStats>();
        //player = PlayerStats.instance;
    }
    void Start()
    {
        player.onSpellCast += LookAtSpell;
        //player.onSpellCast += SetStateToCasting;

        manager = GameManager.instance.GetComponent<PlayerManager>();
        mybody = GetComponent<Rigidbody>();
    }
    void Update()
    {
        moveSpeed = player.secondary[SecondaryStatType.MoveSpeed].GetValue();
        GatherInput();
        if (manager.state != PlayerState.talking)// && manager.state != PlayerState.attacking)// && manager.state != PlayerState.inCombat)
            LookAtInput();
        
    }

    private void FixedUpdate()
    {
        if (manager.state != PlayerState.talking && !Input.GetKey(KeyCode.LeftShift))// && manager.state != PlayerState.attacking)// && manager.state != PlayerState.inCombat)
        {
            Move();
        }
    }

    void GatherInput()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        mouse = Input.mousePosition;
    }
    private void Move()
    {
        speed = moveSpeed * Time.deltaTime;
        input = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0) * input;
        input = Vector3.Normalize(input);
        //transform.position += input * speed;
        mybody.MovePosition(transform.position + (input * speed));
    }
    private void LookAtInput()
    {
        if (input.magnitude != 0)
        {
            var rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0) * Quaternion.LookRotation(input);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed);
        }
        //if you cast a spell, turn towards it?
        /**
        if (onAbilityCast != null)
        {
            onAbilityCast.Invoke();
        }

        
        //if you're moused over inventory/UI follow input
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (input.magnitude != 0)
            {
                var rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0) * Quaternion.LookRotation(input);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed);
            }
        }
        //else follow the mouse
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(mouse);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                var target = hitInfo.point;
                target = new Vector3(target.x, transform.position.y, target.z);
                //transform.LookAt(target);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, rotateSpeed);
            }
        }
        */
    }
    public void LookAtSpell()
    {
        //Debug.Log("looking at spell");
        Ray ray = Camera.main.ScreenPointToRay(mouse);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            var target = hitInfo.point;
            target = new Vector3(target.x, transform.position.y, target.z);
            transform.LookAt(target);
            //Quaternion q = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0) * Quaternion.LookRotation(target);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotateSpeed);
        }
    }
}


