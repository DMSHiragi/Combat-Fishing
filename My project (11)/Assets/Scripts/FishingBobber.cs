using UnityEngine;

public class FishingBobber : MonoBehaviour
{
    public float throwForce = 10f;
    public float maxPosZ = 510f;
    public Vector3 ballScale = new Vector3(0.2f, 0.2f, 0.2f);

    private Camera mainCamera;
    private GameObject ball;



    public float minWaitTime = 3f;
    public float maxWaitTime = 6f;

    public float maxXPosition = 510f;

    public LineRenderer fishingLine;

    public GameObject waterSplashPrefab;

    private float timer;
    private bool fishing;
    private int score;

    private bool bitten;


    void Start()
    {
        // Get a reference to the main camera
        mainCamera = Camera.main;

        fishingLine.positionCount = 2;
        fishingLine.SetPosition(0, Vector3.zero);
        fishingLine.SetPosition(1, Vector3.zero);
        // Initialize the LineRenderer

    }

    void Update()
    {

        
        if (ball != null)
        {
            Vector3 playerOffset = new Vector3(0.3f, -0.3f, 0f);

            if(fishingLine != null){
                fishingLine.SetPosition(0, transform.position + playerOffset);
                fishingLine.SetPosition(1, ball.transform.position);
                fishingLine.enabled = true;
            }
        }
        else{
            fishingLine.enabled = false;
        }


        if (Input.GetMouseButtonDown(0))
        {
            // Check if the player is looking in the +Z direction and their X position is less than 510
            if (transform.forward.z < 0f && transform.position.z < maxPosZ)
            {
                createBall();
            }
            else if(transform.forward.z > 0f){
                Debug.Log("Not facing sea");
            }
            else{
                Debug.Log("Not close enough");
            }
        }

        

        if (ball != null && ball.transform.position.y <= 20.6f)
        {
            if(!fishing){
                startFishing();
            }

            else if(fishing && Input.GetMouseButtonDown(0)){
                stopFishing();
            }

            
            if (fishing && Time.time >= timer && !bitten)
            {
                Debug.Log("Bite");
                bitten = true;

                Rigidbody ballRb = ball.GetComponent<Rigidbody>();
                if (ballRb != null)
                {
                    ballRb.detectCollisions = false;
                }
            }
        }

        // Update the LineRenderer positions when the ball is not null
        

    }

    private void createBall(){

    // If the ball already exists, destroy it
        if (ball != null)
        {
            Destroy(ball);
        }
        else
        {

            // Add a LineRenderer component to create the fishing line

            // Otherwise, create a new ball object
            ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            // Set the position of the ball slightly lower than the player position
            Vector3 ballStartPosition = transform.position + new Vector3(0f, -0.3f, 0f);
            ball.transform.position = ballStartPosition;

            ball.transform.localScale = ballScale;

            // Add a Rigidbody component to the ball
            Rigidbody ballRb = ball.AddComponent<Rigidbody>();


            // Calculate the throw direction based on where the player is looking
            Vector3 throwDirection = mainCamera.transform.forward;

            // Apply the throw force to the ball
            ballRb.AddForce(throwDirection * throwForce, ForceMode.Impulse);


        }

    }

    private void startFishing(){
        fishing = true;
        timer = Time.time + Random.Range(minWaitTime, maxWaitTime);
        Debug.Log("Start Fishing");

            // Instantiate the water splash Particle System at the bobber's position
        GameObject splash = Instantiate(waterSplashPrefab, ball.transform.position, Quaternion.identity);
        splash.transform.rotation = Quaternion.LookRotation(Vector3.up);

        Destroy(splash, 1f); // Destroy the Particle System after 5 seconds to clean up the scene

    }

    private void stopFishing(){        
        float clickTime = Time.time - timer;

        if (clickTime <= 1f && clickTime > 0)
        {
            score++;
            Debug.Log("Fish Caught: " + score);
        }
        else if (clickTime <= 0)
        {
            Debug.Log("Reeled too early");
        }
        else
        {
            Debug.Log("Reeled too late");
        }

        
    // Reset the LineRenderer positions
    
        fishingLine.SetPosition(0, Vector3.zero);
        fishingLine.SetPosition(1, Vector3.zero);

        ResetTimer();
    }

    private void ResetTimer()
    {
        Debug.Log("Reset");

        fishingLine.enabled = false;



        Destroy(ball);
        fishing = false;
        bitten = false;
        timer = Time.time + Random.Range(minWaitTime, maxWaitTime);

    }

}
