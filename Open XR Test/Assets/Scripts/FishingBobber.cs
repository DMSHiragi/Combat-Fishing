using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class FishingBobber : MonoBehaviour
{
    public float throwForce = 10f;
    public float maxPosZ = 510f;
    public Vector3 ballScale = new Vector3(0.2f, 0.2f, 0.2f);

    private Camera mainCamera;
    private GameObject ball;
    public GameObject fish1;
    public GameObject fish2;

    public int fishRequired = 3;
    public float minWaitTime = 3f;
    public float maxWaitTime = 6f;

    public int coolDownTimer = 0;

    public float ballReturnSpeed = 20f;
    private bool isBallReturning;
    private bool fishCaught;
    public float heightLimit = 30;
    public float maxYSpeed = 5;
    public float maxXPosition = 510f;
    public FishingTracker myTracker;
    public LineRenderer fishingLine;
    public GameObject waterSplashPrefab;
public AudioClip sfxClip;
private AudioSource audioSource;

    private float timer;
    private bool fishing;
    public int fishScore;

    private bool bitten;

    public bool redFish;
    public float fishSize;

    private InputDevice hand;


    public bool whaleTime;


    void Start()
    {
        mainCamera = Camera.main;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 0.2f;

        //Initialise fishing line
        fishingLine.positionCount = 2;
        fishingLine.SetPosition(0, Vector3.zero);
        fishingLine.SetPosition(1, Vector3.zero);
        Material lineMaterial = fishingLine.material;
        lineMaterial.color = Color.black;
        fishingLine.startWidth = 0.007f;
        fishingLine.endWidth = 0.010f;

    }

    void Update()
    {

        hand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        hand.TryGetFeatureValue(CommonUsages.triggerButton, out bool isPressed);

        if (ball != null)   //update fishing ball if any
        {
            Vector3 playerOffset = new Vector3(0.3f, -0.3f, 0f);

            if(fishingLine != null){
                fishingLine.SetPosition(0, transform.position);
                fishingLine.SetPosition(1, ball.transform.position);
                fishingLine.enabled = true;
            }
        }
        else{   // only show line when ball active
            fishingLine.enabled = false;
        }

        if(coolDownTimer > 0){
            coolDownTimer--;
        }

        if (ball != null && ball.transform.position.y <= 20.7f) // If ball is at sea level
        {
            if(!fishing && !isBallReturning){   // When ball first hits water
            
                startFishing();
            }

            else if(fishing && isPressed){  // When player reels
                stopFishing();
            }
            
            if (fishing && Time.time >= timer && !bitten)   // When a fish initially bites
            {
                // Debug.Log("BITE TIME: " + Time.time);   // start timer to catch
                bitten = true;

                Rigidbody ballRb = ball.GetComponent<Rigidbody>();
                if (ballRb != null)
                {
                    //Plays animation
                    GameObject splash3 = Instantiate(waterSplashPrefab, ball.transform.position, Quaternion.identity);
                    splash3.transform.rotation = Quaternion.LookRotation(Vector3.up);
                    splash3.transform.localScale *= 2f;
                    Destroy(splash3, 1f);


                    //Move ball down & disable gravity to act like water
                    Vector3 forceDirection = Vector3.down * 4f;
                    ballRb.AddForce(forceDirection, ForceMode.Impulse);
                    ballRb.detectCollisions = false;
                    ballRb.useGravity = false;

                    //Rotate fish so it faces the player
                    Vector3 directionToPlayer = transform.position - ball.transform.position;
                    Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
                    ball.transform.rotation = targetRotation;
                }
            }
        }


        if (isPressed && coolDownTimer == 0)    // When the player can press fishing rod
        {
            //If there is a ball, reel it towards us (Up + towards player)
            if(ball != null){
                Rigidbody ballRb = ball.GetComponent<Rigidbody>();
                if (ball.transform.position.y < heightLimit && Mathf.Abs(ballRb.velocity.y) < maxYSpeed) {
                    Vector3 forceDirection = Vector3.up * 1.5f;
                    ballRb.AddForce(forceDirection, ForceMode.Impulse);
                }
                ballRb.AddForce((transform.position - ball.transform.position).normalized * ballReturnSpeed * 0.1f, ForceMode.Impulse);
                isBallReturning = true;
            }

            //If there isn't one, cast out a ball
            else{
                createBall();
                coolDownTimer = 40;
            }
        }

        if (isBallReturning)    //If reeling in
        {
            // Move the ball towards the player
            ball.transform.position = Vector3.MoveTowards(ball.transform.position, transform.position, ballReturnSpeed * Time.deltaTime);
            
            // Check if the ball has reached the player
            if (Vector3.Distance(ball.transform.position, transform.position) < 0.5f)
            {
                // destroy it
                isBallReturning = false;
                Destroy(ball);
                fishing = false;
                if(fishCaught){
                    myTracker.getFish(fishSize, redFish);
                }
            }
        }


    }

    private void createBall(){

    // If a ball already exists, destroy it
        if (ball != null)
        {
            Destroy(ball);
        }
        else    //Create sphere, Disable collision with bounding boxes & throw in direction of player cam
        {
            // Create a new ball object
            ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            // Set the position of the ball
            Vector3 ballStartPosition = transform.position;
            ball.transform.position = ballStartPosition;
            ball.transform.localScale = ballScale;

            // Rigid body
            Rigidbody ballRb = ball.AddComponent<Rigidbody>();

            // (Try to) Prevent collision between player and ball
            Collider ballCollider = ball.GetComponent<Collider>();
            GameObject fishingPlayer = GameObject.Find("FishingPlayer");
            CapsuleCollider capsuleCollider = fishingPlayer.GetComponent<CapsuleCollider>();
            Physics.IgnoreCollision(ballCollider, capsuleCollider, true);

            // Throw based on where the player is looking
            Vector3 throwDirection = mainCamera.transform.forward;
            ballRb.AddForce(throwDirection * throwForce, ForceMode.Impulse);

        }

    }

    private void startFishing(){    // Called when ball first hits water
        fishing = true;
        // Set random delay for fish to appear
        timer = Time.time + Random.Range(minWaitTime, maxWaitTime); 
        
        // Splash effect
        GameObject splash = Instantiate(waterSplashPrefab, ball.transform.position, Quaternion.identity);
        audioSource.PlayOneShot(sfxClip);
        splash.transform.rotation = Quaternion.LookRotation(Vector3.up);
        splash.transform.localScale *= 2f;
        Destroy(splash, 1f); 

        // slow bobber down
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();    
        ballRb.drag = 3f; 
    }

    private void stopFishing(){     // Called when you first reel in bobber
        //Splash effect
        GameObject splash2 = Instantiate(waterSplashPrefab, ball.transform.position, Quaternion.identity);

        splash2.transform.rotation = Quaternion.LookRotation(Vector3.up);
        splash2.transform.localScale *= 2f;
        Destroy(splash2, 1f);
        
        // How long it took to react to catch fish
        float clickTime = Time.time - timer;
        // Debug.Log("CLICK TIME: " + clickTime);
        // Debug.Log("TIMER: " + timer);
        // Debug.Log("END: " + Time.time);

        // If within 1 second, reward, otherwise miss
        if (clickTime <= 1f && clickTime > 0)
        {
            // Replace ball with fish model if success
            ReplaceBallMesh();
            fishScore++;
            if(fishScore >= fishRequired){
                whaleTime = true;
            }
            fishCaught = true;
        }

        else if (clickTime <= 0){   //Too early
            fishCaught = false;}

        else{  fishCaught = false;}     //Too late

        // Reset the LineRenderer positions
        fishingLine.SetPosition(0, Vector3.zero);    
        fishingLine.SetPosition(1, Vector3.zero);

        ResetTimer();
    }

    private void ResetTimer()
    {           //Resets to start fishing again.
        fishingLine.enabled = false;    //Hide fishing line
        fishing = false;
        bitten = false;
        timer = Time.time + Random.Range(minWaitTime, maxWaitTime);     // I think this might not be needed but I don't want to test removing it since it works as is

    }


    //Replace bobber with fish model
    public void ReplaceBallMesh()
    {
        if (ball == null)
        {
            return;
        }

        MeshFilter prefabMeshFilter = fish1.GetComponent<MeshFilter>();
        MeshRenderer prefabMeshRenderer = fish1.GetComponent<MeshRenderer>();
        MeshFilter prefabMeshFilter2 = fish2.GetComponent<MeshFilter>();
        MeshRenderer prefabMeshRenderer2 = fish2.GetComponent<MeshRenderer>();

        MeshFilter ballMeshFilter = ball.GetComponent<MeshFilter>();
        MeshRenderer ballMeshRenderer = ball.GetComponent<MeshRenderer>();


        ballMeshFilter.mesh = prefabMeshFilter2.sharedMesh;
        ballMeshRenderer.materials = prefabMeshRenderer2.sharedMaterials;


        redFish = false;

        // Randomly selects different model 50% of the time
        // I wanted 5 random fish models but if there's too many then it takes too long to load and breaks it. Probably solvable with a mesh array but too much work
        if(Random.Range(1,10) > 5){
            ballMeshFilter.mesh = prefabMeshFilter.sharedMesh;
            ballMeshRenderer.materials = prefabMeshRenderer.sharedMaterials;
            redFish = true;
        }

        //Random size fish
        fishSize = Random.Range(0.5f, 2.5f);
        Vector3 randomScale = new Vector3(fishSize, fishSize, fishSize);
        ball.transform.localScale = randomScale;

    }

}
