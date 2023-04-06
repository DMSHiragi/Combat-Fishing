using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public Grappling grappling;
    public int curHealth = 0;
    public int maxHealth = 100;
    public ParticleSystem psys;
    public Animator animator;
    public Rigidbody rb;
    

    // Start is called before the first frame update
    void Start()
    {
        curHealth = maxHealth;
        psys = GetComponent<ParticleSystem>();
        rb = GetComponent<Rigidbody>();
        grappling = GameObject.FindGameObjectWithTag("Player").GetComponent<Grappling>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Damage(int d)
    {
        curHealth -= d;
        rb.AddForce((transform.position - grappling.gameObject.transform.position).normalized * 10f, ForceMode.Impulse);
        //AudioManager.instance.Play("SharkDamage");
        if (curHealth <= 0)
        {
            animator.SetTrigger("Defeat");
            StartCoroutine(waiter());
        }

        else{
            animator.SetTrigger("Damage");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (grappling.swinging == true && other.gameObject.layer == 18){
                Damage(100);
                psys.Play();
                Debug.Log("Damage 100");
        }else if (other.gameObject.CompareTag("Rod")) {
                Damage(10);
                psys.Play();
          } 
    }

    IEnumerator waiter(){
        yield return new WaitForSeconds(1);

        Destroy(gameObject);
    }
}
