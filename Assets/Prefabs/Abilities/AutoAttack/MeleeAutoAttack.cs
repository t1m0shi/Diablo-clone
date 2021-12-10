using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//goes on the melee prefab to deal damage to enemies
public class MeleeAutoAttack : MonoBehaviour
{
    public PlayerStats origin;
    public Weapon weapon;
    public int damage;
    Timer timmy;
    public new Animation animation;
    //private Animator animator;

    private void Awake()
    {
        //Debug.Log("meleed");
        //animation.Play();
        timmy = gameObject.AddComponent<Timer>();
        timmy.purpose = TimerPurpose.duration;
        timmy.SetTimer(origin.CalcAttackSpeed(weapon), End);
    }
    private void Update()
    {
        /*
        if (!animation.isPlaying)
        {
            End();
        }*/
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("hit " + other.name);
        //if (other.tag == "Enemy")
        EnemyStats enemy = other.gameObject.GetComponent<EnemyStats>();
        if (enemy != null)
        {
            //EnemyStats enemy = other.gameObject.GetComponent<EnemyStats>();
            int damage = UnityEngine.Random.Range(weapon.damage.min, weapon.damage.max + 1);
            origin.DealDamage(enemy, damage, weapon.damage.type);
        }
        
        
    }

    void End()
    {
        PlayerManager.instance.state = PlayerState.idle;
        Destroy(this.gameObject);
    }
}
