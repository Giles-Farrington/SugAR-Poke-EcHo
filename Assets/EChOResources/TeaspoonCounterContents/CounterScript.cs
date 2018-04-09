using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class CounterScript : MonoBehaviour{

    // Use this for initialization
    public Material[] backgrounds = new Material[2];
	public string fileName = "";

    /*The following function makes an empty string, and then loops through the file name until it finds the # of teaspoons. It then
    appends each digit of the number to the empty string. If name contains no numbers, the empty string it will be replaced with
    an error message.*/

    public void GetTeaspoonValue(string fileName)
    {
        //Lines 12-14 get the TextMesh component and set the proper font size and font color
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

        if(finalOutput == "")
        {
            finalOutput = "Error! \\n Teaspoon value not found!";
        }
		//Debug.LogError ("Final output is " + finalOutput);
        text_mesh.text = finalOutput;

		if (finalOutput == "0")
        {

            this.GetComponent<MeshRenderer>().material = backgrounds[0];
        }
        else
        {
            this.GetComponent<MeshRenderer>().material = backgrounds[1];
        }
    }
}
