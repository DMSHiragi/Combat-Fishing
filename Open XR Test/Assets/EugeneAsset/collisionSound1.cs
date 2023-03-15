using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionSound1 : MonoBehaviour
{

    public AudioSource audioSource;

    // Start is called before the first frame update
    // Update is called once per frame
        void onTriggerEnter (Collider other) {
                    audioSource.Play ();
        }
    }
