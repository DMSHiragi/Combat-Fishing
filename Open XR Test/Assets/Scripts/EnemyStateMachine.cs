using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{

    public Transform target;
    public int moveSpeed;
    public int rotationSpeed;
    public int maxDistance;
    public bool trigger;
    // Start is called before the first frame update
    private Transform myTransform;

    float distanceBetweenObjects;
    public GameObject hitBox;

    int timer = 0;
    int attackCooldwonTimer = 0;
    bool timerOn = false;

    void Awake()
    {
        myTransform = transform;

    }

    void Start()
    {
        moveSpeed = 3;
        rotationSpeed = 3;

    }

    // Update is called once per frame
    void Update()
    {

        if (trigger == true)
        {
            Debug.DrawLine(target.position, myTransform.position, Color.yellow);

            //look at target
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(target.position - myTransform.position), rotationSpeed * Time.deltaTime);
            myTransform.rotation *= Quaternion.Euler(0, 90, 0); // Correct for initial rotation

            //move towards target
            if (Vector3.Distance(target.position, myTransform.position) > maxDistance)
            {
                myTransform.position += myTransform.forward * moveSpeed * Time.deltaTime;

            }
        }

        if (timerOn){
            timer++;
        }

        if (timer > 60){
            timer = 0;
            timerOn = false;
            hitBox.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            target = go.transform;

            maxDistance = 0;

            trigger = true;
        }
    }

    private void OnTriggerStay(Collider other){
        if(other.tag == "Player")
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            distanceBetweenObjects = Vector3.Distance(transform.position, go.transform.position);
            if(distanceBetweenObjects < 5){
                Attack();
            }
        }
    }



    private void Attack(){
        hitBox.SetActive(true);
        timerOn = true;
    }
}
