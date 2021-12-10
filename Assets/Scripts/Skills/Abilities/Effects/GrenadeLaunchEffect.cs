using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLaunchEffect : Ability.PlayerAbilityEffect.Attack
{
    [Range(20f, 75f)]
    public float alpha;

    public override void Activate(PlayerStats originator)
    {
        base.Activate(originator);
        //instantiate grenade and launch it
        //attackPoint = originator.gameObject.transform;
        GameObject l = Instantiate(attackPrefab, attackPoint.position, Quaternion.identity);
        l.GetComponent<Grenade>().damage = this.damage;
        l.GetComponent<Grenade>().player = originator;
        LobIt(l);
    }

    private void LobIt(GameObject grenade)
    {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            //make it face the mouse
            var target = hitInfo.point;
            var look = new Vector3(target.x, 0, target.z);
            grenade.transform.LookAt(look);
            //grenade.transform.position = grenade.transform.position + grenade.transform.forward;
            //grenade.transform.position = new Vector3(grenade.transform.position.x, 1, grenade.transform.position.z);
            Vector3 XZpos = new Vector3(grenade.transform.position.x, 0, grenade.transform.position.z);

            float R = Vector3.Distance(XZpos, look);
            float G = Physics.gravity.y;
            float tanAlpha = Mathf.Tan(alpha * Mathf.Deg2Rad);
            float H = target.y - grenade.transform.position.y;
            float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
            float Vy = tanAlpha * Vz;
            Vector3 localVelocity = new Vector3(0f, Vy, Vz);
            Vector3 globalVelocity = grenade.transform.TransformDirection(localVelocity);

            grenade.GetComponent<Rigidbody>().velocity = globalVelocity;
        }
    }
}
