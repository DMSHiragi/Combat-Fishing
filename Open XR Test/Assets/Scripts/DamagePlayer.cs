using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamagePlayer : MonoBehaviour
{
    private Collider myCollider;
    public PlayerHealth phealth;
    public float damageValue;


    // If this script is placed onto something, it will damage the player
    // Must have a rigid body, mesh collider, and Is Trigger must be checked
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Damage");
        }
    }
}
