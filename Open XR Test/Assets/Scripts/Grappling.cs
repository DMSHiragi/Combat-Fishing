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
    public LayerMask everything;
    public LineRenderer lr;
    public LineRenderer lr2;
    public GameObject fishingLineBallPrefab;
    public GameObject fishingLineBall;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    private float grappleNoDelay = 0f;
    public float overShootYAxis;
    private bool currentlyGrappling;

    [Header("Swinging")]
    private Vector3 swingPoint;
    private SpringJoint joint;
    public bool swinging;
    private GameObject swingObject;

    private Vector3 grapplePoint;

    [Header("Prediction")]
    public RaycastHit predictionHit;
    public RaycastHit predictionHit2;
    public RaycastHit predictionHit3;
    public float predictionSphereCastRadius;
    public Transform predictionPoint;
    public Transform predictionPoint2;
    public Transform predictionPoint3;
    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]

    private InputDevice hand;
    private bool grappling;
    private bool isButtonPressed;
    private bool isButtonPressed2;
    private bool isButtonPressed3;

    public AudioSource sfx;
    public AudioClip[] sfxClips;


    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();

        lr2.startWidth = 0.007f;
        lr2.endWidth = 0.010f;
        lr.startWidth = 0.007f;
        lr.endWidth = 0.010f;

        sfx = gameObject.GetComponent<AudioSource>();
        sfx.clip = sfxClips[0];
        Material lineMaterial = new Material(Shader.Find("Unlit/Color"));
        lineMaterial.color = Color.black;
        lr.material = lineMaterial;
        lr2.material = lineMaterial;

    }

    // Update is called once per frame
    void Update()
    {
        
        hand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        hand.TryGetFeatureValue(CommonUsages.triggerButton, out bool isPressed);
        hand.TryGetFeatureValue(CommonUsages.gripButton, out bool isGripped);
        hand.TryGetFeatureValue(CommonUsages.primaryButton, out bool isPushed);

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

        if (isPushed){
            if (!isButtonPressed3) 
            {

                YeetObject();

            }
            isButtonPressed3 = true;
            }
        else
        {
            isButtonPressed3 = false;
        }

        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }else{lr.enabled = false;}

        lr.SetPosition(0, rodEnd.position);
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
            


            StartCoroutine(CastFishingLine(grapplePoint, lr));

            
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        

        else
        {       // If not looking at grapplable or swingable object, stop swinging
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleNoDelay);    // Please not have delay when doing this ._.
        }

        if (swinging == false)
        {
            // lr.SetPosition(1, grapplePoint);
        }
    }



    IEnumerator CastFishingLine(Vector3 targetPosition, LineRenderer myRenderer)
    {
        float duration = grappleDelayTime;
        float elapsedTime = 0f;
        currentlyGrappling = true;

        Vector3 startPosition = rodEnd.transform.position;

        while (elapsedTime < duration)
        {
            lr.enabled = true;
            elapsedTime += Time.deltaTime;
            fishingLineBall.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);


            // Update line renderer positions
            lr.SetPosition(0, rodEnd.position);
            lr.SetPosition(1, fishingLineBall.transform.position);

            yield return null;
        }


        lr.enabled = true;
        lr2.enabled = false;

        // Set the ball's position to the target position at the end of the animation

        // Remove the ball and line renderer after reaching the goal
    }

    private void ExecuteGrapple()
    {
        Debug.Log("ExecuteGrapple");

         sfx.clip = sfxClips[0];
         sfx.Play();

        pc.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overShootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overShootYAxis;

        pc.JumpToPosition(grapplePoint, highestPointOnArc);



        Invoke(nameof(StopGrapple), 0);
   }

   


    public void StopGrapple()
    {   

        Debug.Log("StopGraple");

        grappling = false;

        pc.freeze = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
        
        currentlyGrappling = false;
    }

    private void StartSwing(RaycastHit h){
        Debug.Log("StartSwing");

         sfx.clip = sfxClips[1];
         sfx.Play();

        pc.freeze = false;
        lr2.enabled = true;
        lr2.positionCount = 2;
        predictionPoint2.gameObject.SetActive(false);

        swingPoint = h.point;
        swingObject = h.rigidbody.gameObject;
        joint = h.rigidbody.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        //joint.connectedAnchor = player.position;
        joint.connectedBody = player.gameObject.GetComponent<Rigidbody>();

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

        joint.maxDistance = distanceFromPoint * .5f;
        joint.minDistance = distanceFromPoint * 0f;

        joint.spring = 1f;
        joint.damper = 1f;
        joint.massScale = 2.5f;


    Debug.Log("Start swing");
}

    

    private void YeetObject(){
        if(swinging){
            if(swingObject != null){
                Rigidbody swingRb = swingObject.GetComponent<Rigidbody>();
                swingRb.AddForce((rodEnd.position - swingObject.transform.position).normalized * 20f, ForceMode.Impulse);
            }
        }
    }




    public void StopSwing(){
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

        RaycastHit sphereCastHit3;
        Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, out sphereCastHit3, maxGrappleDistance);

        RaycastHit rayCastHit3;
        Physics.Raycast(cam.position, cam.forward, out rayCastHit3, maxGrappleDistance, everything);

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
            predictionPoint.gameObject.SetActive(false);
            predictionPoint3.gameObject.SetActive(false);
            return;
        }

        else
        {
            predictionPoint2.gameObject.SetActive(false);
        }

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
            predictionPoint2.gameObject.SetActive(false);
            predictionPoint3.gameObject.SetActive(false);
            return;
        }

        else
        {
            predictionPoint.gameObject.SetActive(false);
        }

  

        Vector3 realHitPoint3;

        if (rayCastHit3.point != Vector3.zero)
        {
            realHitPoint3 = rayCastHit3.point;
        }

        else if (sphereCastHit2.point != Vector3.zero)
        {
            realHitPoint3 = sphereCastHit3.point;
        }

        else
        {
            realHitPoint3 = Vector3.zero;
        }

        if (realHitPoint3 != Vector3.zero)
        {
            predictionPoint3.gameObject.SetActive(true);
            predictionPoint3.position = realHitPoint3;
            predictionPoint.gameObject.SetActive(false);
            predictionPoint2.gameObject.SetActive(false);
            return;
        }

        else
        {
            predictionPoint3.gameObject.SetActive(false);
        }

        predictionHit = rayCastHit.point == Vector3.zero ? sphereCastHit : rayCastHit;
        predictionHit2 = rayCastHit2.point == Vector3.zero ? sphereCastHit2 : rayCastHit2;
        predictionHit3 = rayCastHit3.point == Vector3.zero ? sphereCastHit3 : rayCastHit3;
    }
}

