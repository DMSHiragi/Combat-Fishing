using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGarbage : MonoBehaviour
{

    public Grappling grappling;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (grappling.swinging == true && other.gameObject.CompareTag("Whale") && gameObject.GetComponent<SpringJoint>() != null){
        grappling.StopSwing();
        Destroy(gameObject);
    }
}

}



