using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Inputs;



public class Grappling : MonoBehaviour
{

    [Header("References")]
    private PlayerController pc;
    private Rigidbody rb;
    public Transform cam;
    public Transform rodEnd;
    public Transform player;
    public LayerMask grappleable;
    public LayerMask swingable;
    public LineRenderer lr;
    public LineRenderer lr2;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    private float grappleNoDelay = 0f;
    public float overShootYAxis;

    [Header("Swinging")]
    private Vector3 swingPoint;
    private SpringJoint joint;
    private bool swinging;

    private Vector3 grapplePoint;

    [Header("Prediction")]
    public RaycastHit predictionHit;
    public RaycastHit predictionHit2;
    public float predictionSphereCastRadius;
    public Transform predictionPoint;
    public Transform predictionPoint2;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]

    private InputDevice hand;
    private bool grappling;
    private bool isButtonPressed;
    private bool isButtonPressed2;



    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
        hand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        hand.TryGetFeatureValue(CommonUsages.triggerButton, out bool isPressed);
        hand.TryGetFeatureValue(CommonUsages.gripButton, out bool isGripped);

        CheckForHitPoints();

        //  Only take input from button when initially pressed. Holding down the trigger is not something we can do.
        if (isPressed){
            if (!isButtonPressed) 
            {
                StartGrapple();
            }
            isButtonPressed = true;
            }
        else
        {
            isButtonPressed = false;
        }

        if (isGripped){
            if (!isButtonPressed2) 
            {
                StopSwing();
            }
            isButtonPressed2 = true;
            }
        else
        {
            isButtonPressed2 = false;
        }

        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0, rodEnd.position);
        }

        DrawRope();
    }

    private void StartGrapple()     //When you press button
    {
        Debug.Log("StartGrapple");
        RaycastHit hit2;
        if(Physics.Raycast(cam.position, cam.forward, out hit2, maxGrappleDistance, swingable) && swinging == false)
        {       // If looking at swingable object and not swinging, Swing
           swinging = true;
           StartSwing(hit2);
        }

        if (grapplingCdTimer > 0) return;

        grappling = true;

        pc.freeze = true;

        RaycastHit hit;

        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, grappleable) && swinging == false)
        {       // If looking at grapplable object and not swinging, Grapple
            grapplePoint = hit.point;
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }


        else
        {       // If not looking at grapplable or swingable object, stop swinging
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleNoDelay);    // Please not have delay when doing this ._.
        }


        if (swinging == false)
        {
            lr.enabled = true;
            lr.SetPosition(1, grapplePoint);
        }
    }


    private void ExecuteGrapple()
    {
        Debug.Log("ExecuteGrapple");

        pc.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overShootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overShootYAxis;

        pc.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
   }

    public void StopGrapple()
    {   

        Debug.Log("StopGraple");

        grappling = false;

        pc.freeze = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }

    private void StartSwing(RaycastHit h){
        Debug.Log("StartSwing");

        pc.freeze = false;
        lr2.enabled = true;
        lr2.positionCount = 2;
        predictionPoint2.gameObject.SetActive(false);

        swingPoint = h.point;
        joint = h.rigidbody.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        //joint.connectedAnchor = player.position;
        joint.connectedBody = player.gameObject.GetComponent<Rigidbody>();

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

        joint.maxDistance = distanceFromPoint * 0.5f;
        joint.minDistance = distanceFromPoint * 0.25f;

        joint.spring = 10f;
        joint.damper = 3f;
        joint.massScale = 2.5f;

        Debug.Log("Start swing");
    }


    private void StopSwing(){
        Debug.Log("StopSwing");

        lr2.positionCount = 0;
        lr2.enabled = false;
        swinging = false;
        Destroy(joint);

    }

    private void DrawRope(){
        if(!joint) return;

        lr2.SetPosition(1, rodEnd.position);
        lr2.SetPosition(0, joint.gameObject.transform.position);
    }

    private void CheckForHitPoints()
    {
        if (grappling == true || swinging == true) return;

        RaycastHit sphereCastHit;
        Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, out sphereCastHit, maxGrappleDistance, grappleable);

        RaycastHit rayCastHit;
        Physics.Raycast(cam.position, cam.forward, out rayCastHit, maxGrappleDistance, grappleable);

        RaycastHit sphereCastHit2;
        Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, out sphereCastHit2, maxGrappleDistance, swingable);

        RaycastHit rayCastHit2;
        Physics.Raycast(cam.position, cam.forward, out rayCastHit2, maxGrappleDistance, swingable);


        Vector3 realHitPoint;

        if (rayCastHit.point != Vector3.zero)
        {
            realHitPoint = rayCastHit.point;
        }

        else if (sphereCastHit.point != Vector3.zero)
        {
            realHitPoint = sphereCastHit.point;
        }

        else
        {
            realHitPoint = Vector3.zero;
        }
        
        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }

        else
        {
            predictionPoint.gameObject.SetActive(false);
        }

        Vector3 realHitPoint2;

        if (rayCastHit2.point != Vector3.zero)
        {
            realHitPoint2 = rayCastHit2.point;
        }

        else if (sphereCastHit2.point != Vector3.zero)
        {
            realHitPoint2 = sphereCastHit2.point;
        }

        else
        {
            realHitPoint2 = Vector3.zero;
        }

        if (realHitPoint2 != Vector3.zero)
        {
            predictionPoint2.gameObject.SetActive(true);
            predictionPoint2.position = realHitPoint2;
        }

        else
        {
            predictionPoint2.gameObject.SetActive(false);
        }

        predictionHit = rayCastHit.point == Vector3.zero ? sphereCastHit : rayCastHit;
        predictionHit2 = rayCastHit2.point == Vector3.zero ? sphereCastHit2 : rayCastHit2;
    }
}


