using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    public float curHealth = 0;
    public float maxHealth = 1;
    public Image overlay;
    public float duration;
    public float fadeSpeed;
    private float durationTimer;
    // Start is called before the first frame update
    void Start()
    {
         curHealth = maxHealth;
         overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
         if (Input.GetMouseButtonDown(0)) {
                Damage(0.1f);
          }
          if(overlay.color.a > 0){
                durationTimer += Time.deltaTime;
                if(durationTimer > duration){
                    float tempAlpha = overlay.color.a;
                    tempAlpha -= Time.deltaTime * fadeSpeed;
                    overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
                }
          }
    }

     public void Damage(float d)
    {
        curHealth -= d;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1);
        durationTimer = 0;

    }

    private void OnTriggerEnter(Collider other)
    {
       
        } 
}
