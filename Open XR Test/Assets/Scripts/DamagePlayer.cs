using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamagePlayer : MonoBehaviour
{
    private Collider myCollider;
    public PlayerHealth phealth;
    public float damageValue;
    void Start()
    {
    }

    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            phealth.Damage(damageValue);
            Debug.Log("Damage");
        }
    }
}
