using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    
    private static Image hbar;
    public float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        hbar = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.Find("FishingPlayer");
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        currentHealth = health.curHealth;
        hbar.fillAmount = currentHealth;

        Color greenHealth = new Color(0.6f, 1, 0.6f, 1); 

        if (currentHealth >= 0.3f) {
            hbar.color = greenHealth;
        } else {
           hbar.color = Color.red;
       }
    }
}
