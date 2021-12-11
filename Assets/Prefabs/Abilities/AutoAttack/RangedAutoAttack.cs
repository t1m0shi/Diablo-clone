using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAutoAttack : MonoBehaviour
{
    public PlayerStats origin;
    public Weapon weapon;
    public int damage;
    Timer timmy;
    public new Animation animation;
    public float speed = 25f;
    Rigidbody body;
    //private Animator animator;

    private void Awake()
    {
        //Debug.Log("meleed");
        //animation.Play();
        timmy = gameObject.AddComponent<Timer>();
        timmy.purpose = TimerPurpose.duration;
        timmy.SetTimer(5f, End); //disappears after at most 5 sec
        body = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        body.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log("hit " + other.name);
        //if (other.tag == "Enemy")
        EnemyStats enemy = other.gameObject.GetComponent<EnemyStats>();
        if (enemy != null)
        {
            //EnemyStats enemy = other.gameObject.GetComponent<EnemyStats>();
            int damage = UnityEngine.Random.Range(weapon.damage.min, weapon.damage.max + 1);
            origin.DealDamage(enemy, damage, weapon.damage.type);
            End();
        }
    }

    void End()
    {
        PlayerManager.instance.state = PlayerState.idle;
        Destroy(this.gameObject);
    }
}
