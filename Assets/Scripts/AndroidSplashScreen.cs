using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AndroidSplashScript : MonoBehaviour
{

    public float waitTime;
    //Goes to the next scene TODO: When Vuforia DB is Loaded
    public void NextScene()
    {
        SceneManager.LoadScene(2);
    }

    public void Update()
    {
        if (Time.time > waitTime)
        {
            Debug.Log("Next Scene");
            NextScene();
        }
    }
}
