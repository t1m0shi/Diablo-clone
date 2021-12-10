using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject explosion;
    public int damage;
    public PlayerStats player;
    void OnCollisionEnter(Collision c)
    {
        //Debug.Log(c.gameObject.name);
        if (!(c.collider.CompareTag("Player") || c.collider.CompareTag("Attack")))
        {
            GameObject e = Instantiate(explosion, this.transform.position, Quaternion.identity);
            e.GetComponent<Explosion>().damage = this.damage;
            e.GetComponent<Explosion>().player = this.player;
            Destroy(this.gameObject);
        }
    }
}
