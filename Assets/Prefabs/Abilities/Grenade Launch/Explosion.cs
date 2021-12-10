using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public int damage;
    Timer timmy;
    public PlayerStats player;

    private void Awake()
    {
        //play animation
        //change timmy for the animation stuff
        timmy = gameObject.AddComponent<Timer>();
        timmy.purpose = TimerPurpose.duration;
        timmy.SetTimer(2f, Explode);
    }
    void OnTriggerEnter(Collider collision)
    {
        EnemyStats enemy = collision.gameObject.GetComponent<EnemyStats>();
        if (enemy != null)
        {
            //enemy.TakeDamage(damage, DmgType.Fire);
            player.DealDamage(enemy, damage, DmgType.Fire);
        }
    }

    void Explode()
    {
        Destroy(this.gameObject);
    }
}
