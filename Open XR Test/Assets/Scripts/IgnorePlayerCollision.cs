using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnorePlayerCollision : MonoBehaviour
{    private Collider objectCollider;
    private Collider playerCollider;

    private void Start()
    {
        objectCollider = GetComponent<Collider>();

        // Make sure you have the player GameObject in your scene with the "Player" tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerCollider = player.GetComponent<Collider>();
            if (playerCollider != null)
            {
                Physics.IgnoreCollision(objectCollider, playerCollider, true);
            }
        }

    }

}
