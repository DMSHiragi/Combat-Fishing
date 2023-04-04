using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public Animator animator;
    private int levelToLoad;

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        FadeToLevel(2);
    }

    public void FadeToLevel(int levelIndex)
    {
        levelToLoad = levelIndex;   
        animator.SetTrigger("FadeOut");
        // Debug.Log(levelToLoad);

    }

    public void OnFadeComplete(){
        // Debug.Log("Loading Scene: " + levelToLoad);
        SceneManager.LoadScene(2);
    }
}
