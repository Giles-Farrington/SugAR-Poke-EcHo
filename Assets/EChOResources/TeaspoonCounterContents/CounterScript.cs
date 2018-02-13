using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterScript : MonoBehaviour {

    // Use this for initialization
    public Material[] backgrounds = new Material[2];
    void Start () { 

        //Lines 12-14 get the TextMesh component and set the proper font size and font color
        TextMesh text_mesh = this.GetComponentInChildren<TextMesh>();
        text_mesh.color = new Vector4(1, 1, 1, 1);
        text_mesh.fontSize = 200;

        //Lines 19-20 gets the string name of the ImageTarget file and calls a method that returns the # of teaspoons and sets it as
        //the text.
        string fileName = transform.parent.name;
        text_mesh.text = GetTeaspoonValue(fileName);

        //Calls the function that selects the right background (green or black label) based on the number of teaspoons.
        SetBackground(GetTeaspoonValue(fileName));

    }

    /*The following function makes an empty string, and then loops through the file name until it finds the # of teaspoons. It then 
    appends each digit of the number to the empty string. If name contains no numbers, the empty string it will be replaced with 
    an error message.*/
    private string GetTeaspoonValue(string fileName)
    {
        string output = "";
        char[] fileChars = fileName.ToCharArray();

        for(int i = 0; i < fileChars.Length; i++)
        {
            if (System.Char.IsDigit(fileChars[i]))
            {
                output += fileChars[i];
            }
        }
        if(output == "")
        {
            output = "Error! \\n Teaspoon value not found!";
        }

        return output;
    }

    //The following function takes in the number of teaspoons found in the file name. If it is zero it sets the background material 
    //to be the green label. Otherwise it will be set to the black label.
    private void SetBackground(string output)
    {
        if (output == "0")
        {
            
            this.GetComponent<MeshRenderer>().material = backgrounds[0];
        }
        else
        {
            this.GetComponent<MeshRenderer>().material = backgrounds[1];
        }
    }

	
}
