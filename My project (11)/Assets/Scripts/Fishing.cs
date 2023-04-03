using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishing : MonoBehaviour
{
    public float minWaitTime = 3f;
    public float maxWaitTime = 6f;

    public float maxXPosition = 510f;


    private float timer;
    private bool fishing;
    private int score;

    private bool bitten;

    private void Start() {
        // Debug.Log("Start");
    }

    private void Update()
    {

        if(Input.GetMouseButtonDown(0)){

            if (!fishing)
            {
                fishing = true;
                timer = Time.time + Random.Range(minWaitTime, maxWaitTime);
                // Debug.Log("Timer Started: " + timer);
            }

            else 
            {
                float clickTime = Time.time - timer;
                if (clickTime <= 1f && clickTime > 0)
                {
                    score++;
                    // Debug.Log("Success: " + score);
                }
                else if (clickTime <= 0)
                {
                    // Debug.Log("Early");
                }
                else
                {
                    // Debug.Log("Late");
                }
                ResetTimer();
            }

        }

        else if (fishing && Time.time >= timer && !bitten)
        {
            // Debug.Log("Bite");
            bitten = true;
        }
    }

    private void ResetTimer()
    {
        fishing = false;
        bitten = false;
        timer = Time.time + Random.Range(minWaitTime, maxWaitTime);
    }
}