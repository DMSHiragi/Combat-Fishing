using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class Footsteps : MonoBehaviour {

    // declaring the AudioSource and the Clips required for this function
    public AudioSource footstepsAud;
    public AudioClip[] footstepsClips;
    private InputDevice handR;
    private InputDevice handL;

    // Use this for initialization
    void Start () {
        // fetching the AudioSource, which is attached to this gameObject
        footstepsAud = gameObject.GetComponent<AudioSource>();
        // setting the first [0] AudioClip from the "footstepsClips" array as default at the start of your game
        footstepsAud.clip = footstepsClips[0];
    }
	
	// Update is called once per frame
	void Update () {
        handR = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        handL = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        handR.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 positionR);
        handL.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 positionL);

        if(positionL.y == 0){
            footstepsAud.Play();
        }
    }

    // the Feet gameObject is colliding with something...
    private void OnTriggerEnter(Collider col)
    {
        // ...which is tagged as "Deep Water"
        if (col.CompareTag("Deep Water"))
        {
            // change the AudioClip (to be played) into the second from the "footstepsClips" array 
            footstepsAud.clip = footstepsClips[1];
        } 
        else if (col.CompareTag("Water"))
        {
            footstepsAud.clip = footstepsClips[2];  
        }
        else if (col.CompareTag("Flesh"))
        {
            footstepsAud.clip = footstepsClips[0];  
        }
        else if (col.CompareTag("Sand"))
        {
            footstepsAud.clip = footstepsClips[3];  
        }
        else if (col.CompareTag("Cave"))
        {
            footstepsAud.clip = footstepsClips[4];  
        }

        footstepsAud.Play();
    }

    // the Feet gameObject isn't colliding anymore, with something...
    private void OnTriggerExit(Collider col)
    {
        // ...which is tagged as "Deep Water", etc
        if ((col.CompareTag("Deep Water")) || (col.CompareTag("Water")) || (col.CompareTag("Sand")) || (col.CompareTag("Cave")))
        {
            footstepsAud.Stop();
        }
    }
}
