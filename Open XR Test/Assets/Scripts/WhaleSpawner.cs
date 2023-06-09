using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WhaleSpawner : MonoBehaviour
{

    public AudioClip rainSound;
    public AudioClip emergeSound;
    private AudioSource audioSource;


    public float value;
    public float moveSpeed = 3f; // speed in units per second
    public float moveSpeed2 = 3f; // speed in units per second


    public FishingBobber bobber;

    public AnimationCurve easingCurve;
    public AnimationCurve easingCurve2;
    public AnimationCurve easingCurve3;
    public Canvas myCanvas;

    public Dialogue myDialogue;
    public GameObject jaw;

    public ParticleSystem myParticleSystem;
    public ParticleSystem myRain;
    public ParticleSystem myMist;
    public float delayTime = 5f;
    public Image image;


    void Start() {
        myCanvas.enabled = false;
        audioSource = gameObject.AddComponent<AudioSource>();
    }


    void Update () {

        // If enough fish caught
        if (bobber.fishScore == 3 ) {

            // When enough fish caught, start water effects
            var emission = myParticleSystem.emission;
            emission.rateOverTime = 10f;
            var rain = myRain.emission;
            rain.rateOverTime = 3000f;
            audioSource.PlayOneShot(rainSound);

            // Delay whale appearing 3s
            StartCoroutine(DelayedMoveUp());
            bobber.fishScore += 1;
        }

        // When finished dialogue, start swallow animation
        if(myDialogue.endTime == true){
            StartCoroutine(Swallow());
            myDialogue.endTime = false;
        }
    }

    IEnumerator DelayedMoveUp() {
        yield return new WaitForSeconds(delayTime);
        var mistEmission = myMist.emission;
        mistEmission.rateOverTime = 30f;
        audioSource.PlayOneShot(emergeSound);
        StartCoroutine(MoveUp());
        StartCoroutine(DelayedEmission());

    }

    IEnumerator DelayedEmission(){
        yield return new WaitForSeconds(5);
        var emission = myParticleSystem.emission;
        emission.rateOverTime = 0f;
        var mistEmission = myMist.emission;
        mistEmission.rateOverTime = 0f;
    }




    IEnumerator MoveUp() {      // Emerges from water
            yield return new WaitForSeconds(1);
        float startY = transform.position.y;
        float endY = startY + 30f;
        
        float startZ = transform.position.z;
        float endZ = startZ + 80f;

        float distance = endY - startY;
        float duration = distance / moveSpeed;


        float elapsedTime = 0f;

        while (elapsedTime < duration) {

            float t = easingCurve3.Evaluate(elapsedTime / duration); // evaluate the easing curve at the elapsed time

            float newY = Mathf.Lerp(startY, endY, t);
            float newZ = Mathf.Lerp(startZ, endZ, t);
            transform.position = new Vector3(transform.position.x, newY, newZ);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        
        myCanvas.enabled = true;
        myDialogue.startTime = true;
        Debug.Log(myDialogue.startTime);
    }
IEnumerator Swallow() {
    myCanvas.enabled = false;
    float startY = transform.position.y;
    float endY = startY;
    float startZ = transform.position.z;
    float endZ = startZ + 70f;

    float jawRotate = 5f;

    Quaternion startRotation = jaw.transform.localRotation;
    Quaternion endRotation = startRotation * Quaternion.Euler(0f, 0f, jawRotate);

    float distance = endZ - startZ;
    float halfwayPoint = startZ + distance / 2f;

    float halfwayTime = (halfwayPoint - startZ) / moveSpeed2;

    float elapsedTime = 0f;

    // First coroutine moves halfway and opens jaw
    while (transform.position.z < halfwayPoint) {
        float t = easingCurve.Evaluate(elapsedTime / halfwayTime);
        float newY = Mathf.Lerp(startY, endY, t);
        float newZ = Mathf.Lerp(startZ, halfwayPoint, t);

        transform.position = new Vector3(transform.position.x, newY, newZ);

        float newJawRotation = Mathf.Lerp(0f, jawRotate, t);
        Quaternion newRotation = Quaternion.Slerp(startRotation, endRotation, t);
        jaw.transform.localRotation = Quaternion.Euler(0f, 0f, newJawRotation) * newRotation;

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Second coroutine moves the rest of the way and closes jaw
    startY = transform.position.y;
    startZ = transform.position.z;
    endZ = startZ + distance / 2f;

    startRotation = jaw.transform.localRotation;
    endRotation = startRotation * Quaternion.Euler(0f, 0f, -jawRotate * 2);

    distance = endZ - startZ;
    float duration2 = distance / moveSpeed2;

    elapsedTime = 0f;

    while (elapsedTime < duration2) {
        float t = easingCurve2.Evaluate(elapsedTime / duration2);
        float newY = Mathf.Lerp(startY, endY, t);
        float newZ = Mathf.Lerp(startZ, endZ, t);

        float newAlpha = Mathf.Lerp(0, 2, t);
        image.color = new Color(0, 0, 0, newAlpha);


        transform.position = new Vector3(transform.position.x, newY, newZ);

        float newJawRotation = Mathf.Lerp(jawRotate, 0f, t);
        Quaternion newRotation = Quaternion.Slerp(startRotation, endRotation, t);
        jaw.transform.localRotation = Quaternion.Euler(0f, 0f, newJawRotation) * newRotation;

        elapsedTime += Time.deltaTime;
        yield return null;

    }

    yield return new WaitForSeconds(3);

    SceneManager.LoadScene("Stomach");


        // CODE TO LOAD NEXT SCENE GOES HERE


}








}

