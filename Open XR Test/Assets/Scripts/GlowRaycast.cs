using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class GlowRaycast : MonoBehaviour
{

    public Transform cam;
    public float hitDistance;
    private RaycastHit hit;

    public LayerMask grappleable;
    public LayerMask swingable;
    public LayerMask breakable;

    private Outline gc;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(cam.position, cam.forward, out hit, hitDistance, grappleable)){
            gc = hit.collider.gameObject.GetComponent<Outline>();
            if(gc != null){
            gc.enabled  = true;}
        }

        else if(Physics.Raycast(cam.position, cam.forward, out hit, hitDistance, swingable)){
            gc = hit.collider.gameObject.GetComponent<Outline>();
        
            gc.enabled  = true;
        }

        else if(Physics.Raycast(cam.position, cam.forward, out hit, hitDistance, breakable)){
            gc = hit.collider.gameObject.GetComponent<Outline>();
        
            gc.enabled  = true;
        }

        else if(gc != null){
            gc.enabled = false;
        }
    }
}
