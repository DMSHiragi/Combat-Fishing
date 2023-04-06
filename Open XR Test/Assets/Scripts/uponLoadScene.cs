using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uponLoadScene : MonoBehaviour
{
    public Image image;
    public float fadeDuration = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeImageTo(0.0f, fadeDuration));

    }
    IEnumerator FadeImageTo(float targetAlpha, float duration)
    {
        Color startColor = image.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startColor.a, 0, elapsedTime / duration);
            image.color = new Color(0, 0, 0, 0);
            yield return null;
        }
        // Set the image's alpha to the target alpha at the end of the animation
        image.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
    }

}
