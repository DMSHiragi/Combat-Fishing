using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public Animator animator;
    private int levelToLoad;
    public string sceneName;

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene ();
        sceneName = currentScene.name;
    }

    private void OnTriggerEnter(Collider other)
    {
        FadeToLevel(1);
    }

    public void FadeToLevel(int levelIndex)
    {
        levelToLoad = levelIndex;   
        animator.SetTrigger("FadeOut");

    }

    public void OnFadeComplete(){
        if (sceneName == "FishingScene")
        {
            SceneManager.LoadScene("Stomach");
        }
        else if (sceneName == "Stomach")
        {
            SceneManager.LoadScene("CaveScene");
        }
    }
}
