using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    public int curHealth = 0;
    public int maxHealth = 100;
    public ParticleSystem psys;

    // Start is called before the first frame update
    void Start()
    {
        curHealth = maxHealth;
        psys = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public void Damage(int d)
    {
        curHealth -= d;
        
        if (curHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Rod"))
        {
            Damage(10);
            psys.Play();
        }
    }
}