using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.XR;

public class Dialogue : MonoBehaviour


{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    public bool startTime;

    public bool endTime;
    private bool isButtonPressed = false;

    private InputDevice hand;
    private bool inputEnabled;
    private int index;
    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = string.Empty;
    }

    // Update is called once per frame
    void Update()
    {
        if(startTime){
            inputEnabled = true;
            StartDialogue();
            startTime = false;
        }

        hand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        hand.TryGetFeatureValue(CommonUsages.secondaryButton, out bool bPressed);
        hand.TryGetFeatureValue(CommonUsages.primaryButton, out bool aPressed);

        if(inputEnabled){
            if (aPressed || bPressed || Input.GetMouseButtonDown(0)) {
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

            else {
            isButtonPressed = false;
            }
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
        else{
            gameObject.SetActive(false);
            endTime = true;
            inputEnabled = false;
        }
    }
}
