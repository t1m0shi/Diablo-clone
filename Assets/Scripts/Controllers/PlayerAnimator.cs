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
    const float maxSpeed = 10f;

    AnimatorOverrideController aoc;
    public AnimationClip[] defaultAtkClips;
    private AnimationClip[] currentAtkClips;
    public AnimationClip replaceableAttack;
    public WeaponAnimations[] weaponAnimations;
    Dictionary<WeaponType, AnimationClip[]> weaponAnimsDict;

    Vector3 input;

    public Transform mainH;
    public Transform offH;
    public Transform sheathL;
    public Transform sheathR;
    public Transform sheath2L;
    public Transform sheath2R;

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
        accel = input.normalized.magnitude;
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
        //animator.SetFloat("speedPercent", speedPercent, locomationAnimationSmoothTime, Time.deltaTime);

    }
    protected virtual void Update () {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        
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
            int i = Random.Range(0, currentAtkClips.Length);
            aoc[replaceableAttack.name] = currentAtkClips[i];
        }
    }

    void EnterCombat()
    {
        animator.SetBool("InCombat", true);
    }
    void ExitCombat()
    {
        animator.SetBool("InCombat", false);
    }

    [System.Serializable]
    public struct WeaponAnimations
    {
        public WeaponType weapon;
        public AnimationClip[] clips;
    }

}
