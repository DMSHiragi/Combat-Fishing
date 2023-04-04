using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattle : MonoBehaviour
{
    [Header("Configurable Stats")]
    public int health = 100;
    public float waitTime;
    public int battleStage = 0;

    [Header("References")]
    public GameObject player;
    public GameObject playerVR;
    public Dialogue myDialogue;
    public Canvas myCanvas;
    public GameObject myWater;
    public GameObject myJaw;
    public GameObject waterHitbox;

    private int attackPhase = 3;
    private bool idleOnce;
    private int HPcount = 0;
    private bool splashPlayed;
    private bool spoutPlayed;
    private bool waterRise;

    private Vector3 startPos;
    private Quaternion startRot;

    [Header("Jump Animation")]
    public float down = 40f;
    public float up = 20f;
    public float rotate = 30f;
    public float diveTime = 2f;
    public float jumpTime = 4f;
    public float fallTime = 5f;
    public float resetTime = 6f;
    public AnimationCurve slowOut;
    public AnimationCurve slowIn;
    public AnimationCurve falling;
    public AnimationCurve inOut;
    public AnimationCurve jawCloseGraph;
    public float riseDelay = 1f;
    public float riseDuration = 1f;
    public float fallDuration = 1f;
    public float riseHeight = 10f;

    [Header("Swallow Animation")]
    private bool swallowPlayed;
    public float moveDown = 5f;
    public float moveBack = 10f;
    public float moveBackTime = 2f;
    public float attackingSpeed = 1f;
    public float attackTime = 2f;
    public float jawOpen = 30f;
    public float returnToStartTime = 3f;
    public float attackDistance = 1f;

    [Header("Spout Animation")]
    public float waterDuration;

    void Start()  // Disable canvas, log start position
    {
        myCanvas.enabled = false;
        startPos = transform.position;
        startRot = transform.rotation;
        waterHitbox.SetActive(false);
    }

    void Update()
    {
        switch ((int)battleStage)
        {
            case 0: // Start dialogue when approached
                if(player.transform.position.z < -560f)
                {
                    StartConversation();
                }
                break;

            case 1: // Whale stops talking, battle begins
                if(myDialogue.endTime)
                {
                    EndConversation();
                }
                break;


            case 2: // idle state, do nothing

                // Do this once every X seconds

                if(!idleOnce){ 
                    StartCoroutine(PerformIdleState());
                }

                // Do this every update
                CheckHealth(); // Check if health drops below threshold

                break;


            case 3: // Splash attack

                if (!splashPlayed)
                {
                    StartCoroutine(SplashAnimation());
                }

                break;

            case 4: // Swallow attack
                
                if (!swallowPlayed)
                {
                    StartCoroutine(SwallowAnimation());

                }

                break;


            case 5: // Blow hole attack
                
                if(!spoutPlayed){
                    StartCoroutine(SpoutAnimation());
                }


                break;


            case 6: // Spawn small enemies
                //I can't do this part

                break;


            case 7: // Boss hits half HP
                

                break;


            case 8: // Death outro
                

                break;


            case 9: // Death 
                

                break;
        }
    }

    // CASE 0
                                                // StartConversation TODO
    private void StartConversation(){
        myCanvas.enabled = true;
        myDialogue.startTime = true;

        // Lock player movement

        Debug.Log("1 - EndDialogue");
        battleStage = 1;
    }


    // CASE 1
                                                // EndConversation TODO
    private void EndConversation(){
        myCanvas.enabled = false;

        // Unlock player movement

        // Spawn a bunch of enemies

        Debug.Log("Battle Start");
        battleStage = 3;
    }

    // CASE 2


                                                // PerformIdleState
    IEnumerator PerformIdleState()
    {
        // Code here is called when idle starts
        Debug.Log("Idle Start");

        idleOnce = true;

        // Say a random voiceline sometimes
        Debug.Log("Voice");

        // Wait for 2 seconds
        yield return new WaitForSeconds(waitTime);


        // Hide voiceline
        Debug.Log("Hide Voice");


        // Cycle new attack
        attackPhase += 1;
        if (attackPhase > 5) { // Repeat attack cycle
            attackPhase = 3;
        }
        battleStage = attackPhase;
        idleOnce = false;

        Debug.Log("Idle End");
    }

                                                // CheckHealth
    // When health goes down, change phase if true
    private void CheckHealth(){
        if (health < 50) {
            if (HPcount == 0) { // Only do when first reaching 50%
                battleStage = 7;
                HPcount = 1;
            }
            if (health < 1) { // When killed
                battleStage = 8;
                HPcount = 2;
            }
        }
    }


    

    //CASE 3
        // Animate the whale jump, hell if you don't have a rig to animate
    private IEnumerator SplashAnimation(){
        splashPlayed = true;
        attackPhase = 3;

        float originalY = up - down;
        // Calculate target position and rotation
        Vector3 targetPos = new Vector3(startPos.x, startPos.y - down, startPos.z);
        Quaternion targetRot = Quaternion.Euler(startRot.eulerAngles.x, startRot.eulerAngles.y, startRot.eulerAngles.z + rotate);
        Quaternion midRot = Quaternion.Euler(startRot.eulerAngles.x, startRot.eulerAngles.y, startRot.eulerAngles.z + rotate/4);

        // Move object down and rotate
        float elapsedTime = 0f;
        AnimationCurve diveCurve = AnimationCurve.EaseInOut(0f, 0f, diveTime, 1f);


        //Dive down
        while (elapsedTime < diveTime)
        {
            float t = elapsedTime / diveTime;
            float curveT = slowIn.Evaluate(t);
            transform.position = new Vector3(startPos.x, Mathf.Lerp(startPos.y, targetPos.y, curveT),  startPos.z);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Vector3 newTargetPos = new Vector3(startPos.x, startPos.y + up, startPos.z);
        targetRot = Quaternion.Euler(startRot.eulerAngles.x, startRot.eulerAngles.y, startRot.eulerAngles.z - 45f);

        transform.rotation = targetRot;

        //Jump up
        while (elapsedTime >= diveTime && elapsedTime < jumpTime)
        {
            float t = (elapsedTime - diveTime) / (jumpTime - diveTime);
            float curveT = slowOut.Evaluate(t);
            transform.position = new Vector3(targetPos.x, Mathf.Lerp(targetPos.y, newTargetPos.y, curveT), targetPos.z);
            transform.rotation = Quaternion.Lerp(targetRot, startRot, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Vector3 newPos = new Vector3(startPos.x, startPos.y - 10f, startPos.z);
        if(!waterRise)
        {
            StartCoroutine(MoveWaterCoroutine());
        }

        //Fall & sink
        while (elapsedTime >= jumpTime && elapsedTime < fallTime)
        {
            float t = (elapsedTime - jumpTime) / (fallTime - jumpTime);
            float curveT = falling.Evaluate(t);  // Linear easing
            transform.position = Vector3.Lerp(newTargetPos, newPos, curveT);
            transform.rotation = Quaternion.Lerp(startRot, midRot, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Return to start
        while (elapsedTime >= fallTime && elapsedTime <= resetTime)
        {
            float t = (elapsedTime - fallTime) / (resetTime - fallTime);
            float curveT = inOut.Evaluate(t);  // Linear easing
            transform.position = Vector3.Lerp(newPos, startPos, curveT);
            transform.rotation = Quaternion.Lerp(midRot, startRot, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Set the splashPlayed flag to true so the animation doesn't play again
        yield return new WaitForSeconds(8f);
        splashPlayed = false;
    }

        // Move water up then down, simple.
    private IEnumerator MoveWaterCoroutine()
    {
        waterRise = true;
        Vector3 firstPos = myWater.transform.position;
        Vector3 endPos = new Vector3(firstPos.x, firstPos.y + riseHeight, firstPos.z);

        yield return new WaitForSeconds(riseDelay);

        float elapsedTime = 0f;
        while (elapsedTime < riseDuration) {
            float t = slowOut.Evaluate(elapsedTime / riseDuration);
            myWater.transform.position = Vector3.Lerp(firstPos, endPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < fallDuration) {
            float t = inOut.Evaluate(elapsedTime / fallDuration);
            myWater.transform.position = Vector3.Lerp(endPos, firstPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        waterRise = false;
        battleStage = 2;
        Debug.Log("Battlestage Set");
    }



    //CASE 4

//Animate swallowing
    private IEnumerator SwallowAnimation(){
        swallowPlayed = true;
        attackPhase = 4;

        float key0 = 1f; // When to end rotation
        float key1 = moveBackTime + key0; // when to end move back
        float key2 = attackingSpeed + key1;  // when to end waiting
        float key3 = attackTime + key2; // when to end attack
        float key4 = key3 + attackingSpeed;  // how long to hold
        float key5 = returnToStartTime + key4;  // how long to return
        
        float originalZ = transform.position.z;
        float targetZ = transform.position.z - moveBack;

        float originalY = transform.position.y;
        float targetY = transform.position.y - moveDown;

        float elapsedTime = 0f;
        Quaternion origRot = transform.rotation;

        //Initially slowly rotate towards player
        while (elapsedTime < key0){  
            Vector3 targetDir = player.transform.position - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(targetDir, Vector3.up);
            targetRot *= Quaternion.Euler(0, 90, 0); // Correct for initial rotation

            // Rotate whale towards target
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime);

            targetDir = player.transform.position - transform.position;

            elapsedTime += Time.deltaTime;
            yield return null;

        }


        //Rotate instantly to player, and move back over time
        while (elapsedTime >= key0 && elapsedTime < key1)
        {
            float t = (elapsedTime - key0) / (key1 - key0);
            float curveT = slowIn.Evaluate(t);
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(originalY, targetY, curveT), Mathf.Lerp(originalZ, targetZ, curveT));

            float angleUp = Mathf.Lerp(0f, -5f, t);
            // Calculate target rotation
            Vector3 targetDir = player.transform.position - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(targetDir, Vector3.up);
            targetRot *= Quaternion.Euler(0, 90, angleUp); 


            float currentJawOpen = Mathf.Lerp(0f, jawOpen, t);
            
            Quaternion jawRot = Quaternion.LookRotation(targetDir, Vector3.up);
            jawRot *= Quaternion.Euler(0, 90f, currentJawOpen);


            // Rotate whale towards target and open jaw
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, t);
            myJaw.transform.rotation = Quaternion.Slerp(myJaw.transform.rotation, jawRot, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }


        Quaternion startRot = transform.rotation;
        Vector3 targetPos = player.transform.position - (player.transform.forward);

        Quaternion whaleClose = transform.rotation; // stores target destination
        whaleClose *= Quaternion.Euler(0, 0, 5f);

        Quaternion jawClose = myJaw.transform.rotation; // stores target destination
        jawClose *= Quaternion.Euler(0, 0, -jawOpen);

        float currentTime = elapsedTime;
        Vector3 curPos = transform.position;

        Quaternion curRotation = transform.rotation;
        Quaternion curJawRotation = myJaw.transform.rotation;

        // Move towards player
        while (elapsedTime >= key1 && elapsedTime < key2){

            float t = (elapsedTime - key1) / ((key2 - key1) * 2f);
            float u = (elapsedTime - key1) / ((key2 - key1));

            float curveT = slowOut.Evaluate(t);
            float curveU = jawCloseGraph.Evaluate(u);

            transform.position = Vector3.Lerp(curPos, targetPos, curveT);
            
            transform.rotation = Quaternion.Slerp(curRotation, whaleClose, curveU);
            myJaw.transform.rotation = Quaternion.Slerp(curJawRotation, jawClose, curveU);

            elapsedTime += Time.deltaTime;
            yield return null;
        }


        //close jaw + hold keyframe
        while (elapsedTime >= key2 && elapsedTime < key3){

            float t = (elapsedTime - key2) / (key3 - key2);
            float curveT = slowOut.Evaluate(t);


            elapsedTime += Time.deltaTime;
            yield return null;
        }

        float passedTime = 0f;
        Vector3 currentPos = transform.position;
        Quaternion currentRot = transform.rotation;

        //return to original position
        while (elapsedTime >= key3 && elapsedTime < key4){
            float t = (passedTime) / (2f);
            float curveT = slowOut.Evaluate(t);

            transform.position = Vector3.Lerp(currentPos, startPos, curveT);
            transform.rotation = Quaternion.Slerp(currentRot, origRot, curveT);

            passedTime += Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        battleStage = 2;
        yield return new WaitForSeconds(4f);
        swallowPlayed = false;
    }

    
    //CASE 5


    private IEnumerator SpoutAnimation(){
        spoutPlayed = true;
        attackPhase = 5;

        float spoutDown = 10f;
        float length = 3f;
        float rotateS = 40f;

        bool leftToRight = false;
                int LR = Random.Range(0,2);

        if(LR == 1){
            leftToRight = true;
        }

        float moveSpout = -40f;;

        if(leftToRight){
            moveSpout *= -1;
        }
        Vector3 endPos = new Vector3(startPos.x + moveSpout, startPos.y - spoutDown, startPos.z + 5f);
        Quaternion midRot = Quaternion.Euler(startRot.eulerAngles.x, startRot.eulerAngles.y, startRot.eulerAngles.z + rotateS/4);

        float elapsedTime = 0f;
        
        while(elapsedTime < length){    //Move to one side
            float t = inOut.Evaluate(elapsedTime / length);
            float curveT = slowIn.Evaluate(t);
            transform.position = Vector3.Lerp(startPos, endPos, curveT);
            transform.rotation = Quaternion.Slerp(startRot, midRot, curveT);

            elapsedTime += Time.deltaTime;
            yield return null;

        }

        Vector3 nextPos = new Vector3(startPos.x - moveSpout, startPos.y - spoutDown, startPos.z + 5f);

        elapsedTime = 0f;
        float length2 = 4f;
        
        while(elapsedTime < length2){   //Move side to side
            float t = inOut.Evaluate(elapsedTime / length2);
            float curveT = slowIn.Evaluate(t);
            waterHitbox.SetActive(true);
            transform.position = Vector3.Lerp(endPos, nextPos, curveT);

            elapsedTime += Time.deltaTime;
            yield return null;

        }

        elapsedTime = 0f;

        
        while(elapsedTime < length){    // Return to start
            float t = inOut.Evaluate(elapsedTime / length);
            float curveT = slowIn.Evaluate(t);
            waterHitbox.SetActive(false);
            transform.position = Vector3.Lerp(nextPos, startPos, curveT);
            transform.rotation = Quaternion.Slerp(midRot, startRot, curveT);

            elapsedTime += Time.deltaTime;
            yield return null;

        }

        battleStage = 2;
        yield return new WaitForSeconds(6f);
        spoutPlayed = false;
    }


    
    //CASE 6






    
    //CASE 7




    
    //CASE 8




    
    //CASE 9

}
