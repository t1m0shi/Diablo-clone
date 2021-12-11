using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerAbilityHolder : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler//,
        //IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private PlayerStats player;
    public Ability ability;
    public Timer activeTimer;
    public Timer cooldownTimer;
    public Timer durationTimer;
    //public CharacterStats target;
    public AbilityCooldownUI cooldownUI;
    public AbilityBarUI Owner;
    public KeyCode key;
    private Transform attackPoint;
    private Image icon;
    public AbilitySelector selector;
    public AbilityState currentState = AbilityState.ready;

    bool mouseOverUI = false;

    private float recentlyPickedUp = 0.1f;//can't cast for this

    public enum AbilityState
    {
        ready,
        active,
        cooldown
    }
    

    private void Start()
    {
        //player = GetComponentInParent<PlayerStats>();
        icon = GetComponent<Image>();
        cooldownTimer = gameObject.AddComponent<Timer>();
        cooldownTimer.purpose = TimerPurpose.cooldown;
        if (cooldownUI != null)
            cooldownUI.cooldown = cooldownTimer;
        else
            Debug.LogWarning("cooldownUI not set");
        activeTimer = gameObject.AddComponent<Timer>();
        activeTimer.purpose = TimerPurpose.active;
        durationTimer = gameObject.AddComponent<Timer>();
        durationTimer.purpose = TimerPurpose.duration;
        if (ability != null)
        {
            AttachAbility();
        }
        attackPoint = GameObject.FindGameObjectWithTag("AttackPoint").transform;
        //timer = new Timer();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && EventSystem.current.IsPointerOverGameObject() || EventSystem.current.IsPointerOverGameObject())
            mouseOverUI = true;
        else if (Input.GetKeyUp(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject())
            mouseOverUI = false;
        if (ability != null && currentState == AbilityState.ready && Input.GetKey(key) && !mouseOverUI)
        {
            /**
            //it will cast if you're holding shift or you're moused over an enemy
            Type t = ability.effect.GetType();
            bool r = t.IsSubclassOf(typeof(Ability.PlayerAbilityEffect.Attack));
            bool isEnemy = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                //var target = hitInfo.point;
                var target = hitInfo.collider.gameObject;
                isEnemy = target.CompareTag("Enemy");
            }
            if ((r && (Input.GetKey(KeyCode.LeftShift) || isEnemy)) || !r )
            */
            bool isItem = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                var target = hitInfo.collider.gameObject;
                isItem = target.CompareTag("Item");
            }
            if (isItem)
            {
                recentlyPickedUp = 0.1f;
            }
            if (recentlyPickedUp <= 0)
            {
                bool casted = player.UseMana(ability.cost);//.onSpellCast.Invoke(ability.cost);
                if (casted)
                {
                    //Debug.Log("Casted " + ability.name);
                    ability.effect.attackPoint = attackPoint;
                    ability.effect.Activate(player);
                    currentState = AbilityState.active;
                    PlayerManager.instance.state = PlayerState.attacking;
                    player.gameObject.GetComponent<PlayerAnimator>().OnAttack();
                    activeTimer.SetTimer(ability.activeTime, BeginCooldown); //after active period is over, start cooldown
                }
            }
            else
            {
                recentlyPickedUp -= Time.deltaTime;
            }
        }


        /**
        switch (currentState)
        {
            case AbilityState.ready:
                if (Input.GetKeyDown(key))
                {
                    origin = gameObject.GetComponentInParent<CharacterStats>();

                    ability.Activate(origin, target);
                    currentState = AbilityState.active;
                    timer.SetTimer(ability.activeTime, BeginCooldown); //after active period is over, start cooldown
                   // activeTime = ability.activeTime;
                }
                break;
            case AbilityState.active:
                if (activeTime > 0)
                {
                    activeTime -= Time.deltaTime;
                }
                else
                {
                    currentState = AbilityState.cooldown;
                    ability.BeginCooldown();
                }
                break;
            case AbilityState.cooldown:
                if (cooldownTime > 0)
                {
                    cooldownTime -= Time.deltaTime;
                }
                else
                {
                    currentState = AbilityState.cooldown;
                }
                break;
        }
        */
    }

    public void AttachAbility()
    {
        if (ability != null)
        {
            ability.originator = player;
            ability.effect.holder = this;
        }
    }
    
    public void BeginCooldown()
    {
        //Debug.Log(ability.name +" going on cooldown");
        PlayerManager.instance.state = PlayerState.idle;
        currentState = AbilityState.cooldown;
        cooldownTimer.SetTimer(GetCooldownTime(ability.GetCooldownTime()), EndCooldown);
        cooldownUI.onCooldown = true;
        cooldownUI.initialcd = ability.GetCooldownTime();
    }
    private void EndCooldown()
    {
        //Debug.Log(ability.name + "is now ready.");
        currentState = AbilityState.ready;
        PlayerManager.instance.state = PlayerState.idle;
    }
    private float GetCooldownTime(float cooldown)
    {
        return (1 - player.secondary[SecondaryStatType.CooldownReduction].GetValue()) * cooldown;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //select ability from list of avaialble spells
        if (selector != null && currentState == AbilityState.ready)
        {
            selector.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 90, this.transform.position.z);//this.transform.position;
            selector.clicked = this;
            selector.gameObject.SetActive(!selector.gameObject.activeSelf);
        }
        else if (currentState != AbilityState.ready)
        {
            Debug.LogWarning("Still on cooldown!");
        }
        //what if I want to swap spells??
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AbilityTooltip.instance.hovered = this;
        if (AbilityTooltip.instance.hovered.ability != null)
        {
            AbilityTooltip.instance.UpdateText();
            AbilityTooltip.instance.headerObj.SetActive(true);
            AbilityTooltip.instance.contentObj.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AbilityTooltip.instance.hovered = null;
        AbilityTooltip.instance.headerObj.SetActive(false);
        AbilityTooltip.instance.contentObj.SetActive(false);
    }
    /*
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentState == AbilityState.ready)
        {
            Owner.CurrentlyDragged = new AbilityBarUI.DragData();
            Owner.CurrentlyDragged.DraggedEntry = this;
            //Owner.CurrentlyDragged.OriginalParent = (RectTransform)transform.parent.parent;
            Owner.CurrentlyDragged.OriginalParent = (RectTransform)transform.parent;

            //transform.parent.transform.SetParent(Owner.DragCanvas.transform, true);
            transform.SetParent(Owner.DragCanvas.transform, true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentState == AbilityState.ready)
            transform.localPosition = transform.localPosition + UnscaleEventDelta(eventData.delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentState == AbilityState.ready)
        {
            Owner.HandledDroppedEntry(eventData.position);//, prevDrag);

            RectTransform t = transform as RectTransform;
            float ogx = t.rect.width;
            float ogy = t.rect.height;

            //transform.parent.transform.SetParent(Owner.CurrentlyDragged.OriginalParent, true);
            transform.SetParent(Owner.CurrentlyDragged.OriginalParent, true);

            t.offsetMax = -Vector2.one * 4;
            t.offsetMin = Vector2.one * 4;
            t.sizeDelta = new Vector2(ogx, ogy);
        }
    }

    Vector3 UnscaleEventDelta(Vector3 vec)
    {

        Vector2 referenceResolution = Owner.DragCanvasScaler.referenceResolution;
        Vector2 currentResolution = new Vector2(Screen.width, Screen.height);

        float widthRatio = currentResolution.x / referenceResolution.x;
        float heightRatio = currentResolution.y / referenceResolution.y;
        float ratio = Mathf.Lerp(widthRatio, heightRatio, Owner.DragCanvasScaler.matchWidthOrHeight);

        return vec / ratio;
    }*/

    public void UpdateHolder()
    {
        icon.sprite = ability.icon;
    }
}
