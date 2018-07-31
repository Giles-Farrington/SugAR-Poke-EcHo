﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour {

    public float waitTime;
    //Goes to the next scene TODO: Still hardcoded
	public void NextScene()
    {
        SceneManager.LoadScene(1);
    }

    public void Update()
    {
        if(Time.time > waitTime)
        {
            Debug.Log("Next Scene");
            NextScene();
        }
    }
}
