using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class ManualTextSystem : MonoBehaviour
{

   public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    public bool startTime;

    public bool endTime;
    private bool isButtonPressed = false;

    private InputDevice hand;

    private int index;
    // Start is called before the first frame update
    void Start()
    {
         textComponent.text = string.Empty;
         StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
         if(startTime){
            StartDialogue();
            startTime = false;
        }

        hand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        hand.TryGetFeatureValue(CommonUsages.secondaryButton, out bool yPressed);
        hand.TryGetFeatureValue(CommonUsages.primaryButton, out bool xPressed);

        
        if (yPressed || Input.GetMouseButtonDown(0)) {
            if (!isButtonPressed) {
                if (textComponent.text == lines[index]) {
                    NextLine();
                } else {
                    StopAllCoroutines();
                    textComponent.text = lines[index];
                }
            }

            isButtonPressed = true;
        }

        else if (xPressed || Input.GetMouseButtonDown(0)) {
            if (!isButtonPressed) {
                if (textComponent.text == lines[index]) {
                    PrevLine();
                } else {
                    StopAllCoroutines();
                    textComponent.text = lines[index];
                }
            }

            isButtonPressed = true;
        } 
        else {
            isButtonPressed = false;
        }


    }

        void StartDialogue(){
        index = 0;
        StartCoroutine(TypeLine());

    }

    IEnumerator TypeLine(){
        foreach(char c in lines[index].ToCharArray()){
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine(){
        if(index < lines.Length - 1){
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
    }

        void PrevLine(){
        Debug.Log("Running");
        if(index < lines.Length && index > 0){
            index--;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
    }
}
