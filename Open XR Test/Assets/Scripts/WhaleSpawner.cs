using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleSpawner : MonoBehaviour
{

    public float value;
    public float moveSpeed = 5f; // speed in units per second

    private bool isMoving = false;

    public FishingBobber bobber;

    public AnimationCurve easingCurve;

    void Update () {

        if (bobber.fishScore == 1) {
            StartCoroutine(MoveUp());
            isMoving = true;
            bobber.fishScore += 1;

        }

    }

    IEnumerator MoveUp() {
        float startY = transform.position.y;
        float endY = startY + 30f;
        
        float startZ = transform.position.z;
        float endZ = startZ + 80f;

        float distance = endY - startY;
        float duration = distance / moveSpeed;


        float elapsedTime = 0f;

        while (elapsedTime < duration) {

            float t = easingCurve.Evaluate(elapsedTime / duration); // evaluate the easing curve at the elapsed time

            float newY = Mathf.Lerp(startY, endY, t);
            float newZ = Mathf.Lerp(startZ, endZ, t);
            transform.position = new Vector3(transform.position.x, newY, newZ);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isMoving = false;
    }
}

