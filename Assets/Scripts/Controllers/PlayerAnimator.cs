using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimator : MonoBehaviour {

    const float locomationAnimationSmoothTime = 1f;

    //NavMeshAgent agent;
    //PlayerControls agent;
    //Rigidbody body;
    public Animator animator;
    public float speedPercent;
    public float accel;
    float maxSpeed = 10f;

    AnimatorOverrideController aoc;
    public AnimationClip[] defaultAtkClips;
    private AnimationClip[] currentAtkClips;
    public AnimationClip replaceableAttack;
    public WeaponAnimations[] weaponAnimations;
    Dictionary<WeaponType, AnimationClip[]> weaponAnimsDict;

    Vector3 input;

    public Transform mainH;
    public Transform offH;
    public bool sheathed = true;

    protected virtual void Start () {
        //agent = GetComponent<NavMeshAgent>();
        //agent = GetComponent<PlayerControls>();//GetComponent<Rigidbody>();
        //body = agent.GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        //maxSpeed = animator.GetFloat("maxSpeed"); //need to manually make sure the thresholds are capped at this value in the animator
        EquipmentManager.instance.onWeaponChanged += WeaponEquipped;
        PlayerManager.instance.onCombatEnter += EnterCombat;
        PlayerManager.instance.onCombatExit += ExitCombat;

        aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = aoc;
        currentAtkClips = defaultAtkClips;
        weaponAnimsDict = new Dictionary<WeaponType, AnimationClip[]>();
        foreach (var item in weaponAnimations)
        {
            weaponAnimsDict.Add(item.weapon, item.clips);
        }
	}
    protected virtual void FixedUpdate()
    {
        if (accel == 0 || Input.GetKey(KeyCode.LeftShift) || PlayerManager.instance.state == PlayerState.attacking)
        {
            speedPercent -= locomationAnimationSmoothTime;
        }
        else
        {
            speedPercent += accel;
        }
        speedPercent = Mathf.Clamp(speedPercent, 0, maxSpeed);
        animator.SetFloat("speedPercent", speedPercent);
        if (animator.GetBool("InCombat") && speedPercent <= 0.1f && sheathed)
        {
            UnSheathAll();
        }
        else if (speedPercent > 0.1f && !sheathed)
        {
            SheathAll();
        }
        //animator.SetFloat("speedPercent", speedPercent, locomationAnimationSmoothTime, Time.deltaTime);

    }
    protected virtual void Update () {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        accel = input.normalized.magnitude;
        /*
        if (accel > 0)
        {
            //put away weapon if running
            //animator.SetBool("Sheathed", true);
            animator.SetTrigger("Sheath");
        }*/
    }

    void WeaponEquipped(Weapon newW, Weapon oldW)
    {
        if (newW == null)
        {
            animator.SetBool("WeaponEquipped", false);
            currentAtkClips = defaultAtkClips;
        }
        else
        {
            animator.SetBool("WeaponEquipped", true);
            if (weaponAnimsDict.ContainsKey(newW.wtype)){
                currentAtkClips = weaponAnimsDict[newW.wtype];
            }
        }
    }
    public void OnAttack()
    {
        if (currentAtkClips.Length > 0)
        {
            animator.SetTrigger("attack");
            if (sheathed)
            {
                UnSheathAll();
            }
            int i = Random.Range(0, currentAtkClips.Length);
            aoc[replaceableAttack.name] = currentAtkClips[i];
        }
    }

    void EnterCombat()
    {
        animator.SetBool("InCombat", true);
        if (sheathed)
            UnSheathAll();
    }
    void ExitCombat()
    {
        animator.SetBool("InCombat", false);
        if (!sheathed)
            SheathAll();
    }
    public void UnSheathAll()
    {
        if (PlayerStats.instance.mainHand != null)
        {
            UnSheath(true);
        }
        if (PlayerStats.instance.offHand != null)
        {
            UnSheath(false);
        }
        sheathed = false;
    }
    public void SheathAll()
    {
        if (PlayerStats.instance.mainHand != null)
        {
            Sheath(true);
        }
        if (PlayerStats.instance.offHand != null)
        {
            Sheath(false);
        }
        sheathed = true;
    }
    public void Sheath(bool mainhand)
    {
        if (mainhand && PlayerStats.instance.mainHand != null)
        {
            EquipmentManager.instance.RemoveFromBone(PlayerStats.instance.mainHand, mainH);
            EquipmentManager.instance.AttachToBone(PlayerStats.instance.mainHand);
        }
        else if (!mainhand && PlayerStats.instance.offHand != null)
        {
            EquipmentManager.instance.RemoveFromBone(PlayerStats.instance.offHand, offH);
            EquipmentManager.instance.AttachToBone(PlayerStats.instance.offHand);
        }
        animator.SetTrigger("sheath");
    }
    public void UnSheath(bool mainhand)
    {
        if (mainhand && PlayerStats.instance.mainHand != null)
        {
            EquipmentManager.instance.RemoveFromBone(PlayerStats.instance.mainHand);
            EquipmentManager.instance.AttachToBone(PlayerStats.instance.mainHand, mainH);
        }
        else if (!mainhand && PlayerStats.instance.offHand != null)
        {
            EquipmentManager.instance.RemoveFromBone(PlayerStats.instance.offHand);
            EquipmentManager.instance.AttachToBone(PlayerStats.instance.offHand, offH);
        }
        animator.SetTrigger("unsheath");
    }

    [System.Serializable]
    public struct WeaponAnimations
    {
        public WeaponType weapon;
        public AnimationClip[] clips;
    }

}
