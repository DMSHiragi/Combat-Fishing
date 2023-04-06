using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR;
using System;

public class FishingTracker : MonoBehaviour
{

    //Used in beach scene
    //This script updates the paper with what fish were caught

    private bool newFish;
    private bool fishColour;
    private float fishSize;
    public TextMeshProUGUI textComponent;    

    public void getFish(float Size, bool red){
        string colour;
        if(red){
            colour = "Blue Fin - ";
        }else{
            colour = "Red Emperor - ";
        }
        float roundedFloat = (float) Math.Round(Size, 2);

        textComponent.text += "\n" + colour + roundedFloat.ToString() + "m";
    }


    
}
