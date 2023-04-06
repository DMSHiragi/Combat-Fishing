using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Health : MonoBehaviour
{
    public Grappling grappling;
    public int curHealth = 0;
    public int maxHealth = 100;
    public ParticleSystem psys;
    public AudioSource hitSfx;

    // Start is called before the first frame update
    void Start()
    {
        curHealth = maxHealth;
        psys = GetComponent<ParticleSystem>();
        hitSfx = gameObject.GetComponent<AudioSource>();

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
            // Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (grappling.swinging == true && other.gameObject.layer == 18 && other.gameObject.GetComponent<SpringJoint>() != null){
                Damage(100);
                hitSfx.Play();
                psys.Play();
                Debug.Log("Damage 100");
        }else if (other.gameObject.CompareTag("Rod")) {
                Damage(10);
                psys.Play();
                hitSfx.Play();
          }
        }
    }
