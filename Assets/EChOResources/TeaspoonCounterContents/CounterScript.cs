﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using System;

public class CounterScript : MonoBehaviour{

    // Use this for initialization
    public Material[] backgrounds = new Material[2];
	public string fileName = "";
	//
	Quaternion rotation;
	//
    /*The following function makes an empty string, and then loops through the file name until it finds the # of teaspoons. It then
    appends each digit of the number to the empty string. If name contains no numbers, the empty string it will be replaced with
    an error message.*/


	//Get the original rotation values
	void Awake() 
	{
		rotation = Quaternion.Euler(new Vector3(-90,0,0));
	}

	//fix rotation
	void LateUpdate() 
	{
		transform.rotation = rotation;
	}	
	//
    public void GetTeaspoonValue(string fileName)
    {
        //Lines get the TextMesh component and set the proper font size and font color
        TextMesh text_mesh = this.GetComponentInChildren<TextMesh>();
        text_mesh.color = new Vector4(1, 1, 1, 1);
        text_mesh.fontSize = 200;
        string reverseOutput = "";
        string finalOutput = "";
        char[] fileChars = fileName.ToCharArray();
        int index = fileChars.Length - 1;

        while(fileChars[index] != '-')
        {
            if (System.Char.IsDigit(fileChars[index]))
            {
                reverseOutput += fileChars[index];
            }
            index--;
        }

		for(int j = reverseOutput.Length - 1; j >= 0; j--){
          finalOutput += reverseOutput[j];
        }
        if (finalOutput == "")
        {
            finalOutput = "Error! \\n Teaspoon value not found!";
        }

        text_mesh.text = finalOutput;
        //Determine label shape by the teaspoon number

        int finalOutputNum = Convert.ToInt32(finalOutput);

        if (finalOutputNum == 0) this.GetComponent<MeshRenderer>().material = backgrounds[0];

        else if (finalOutputNum >= 1 && finalOutputNum <= 6) this.GetComponent<MeshRenderer>().material = backgrounds[1];

        else if (finalOutputNum >= 7 && finalOutputNum <= 12) this.GetComponent<MeshRenderer>().material = backgrounds[2];

        else this.GetComponent<MeshRenderer>().material = backgrounds[3];

       
		//position for text of the number of teaspoon
		if (int.Parse (text_mesh.text) < 10) {
			this.gameObject.transform.GetChild (0).localPosition = new Vector3 (-1.59f, 0f, 4.7f); 
		} else {
			this.gameObject.transform.GetChild (0).localPosition = new Vector3 (-3.5f, 0f, 4.7f); 
		}
		//Debug.Log (text_mesh.text);
			
    }


}
