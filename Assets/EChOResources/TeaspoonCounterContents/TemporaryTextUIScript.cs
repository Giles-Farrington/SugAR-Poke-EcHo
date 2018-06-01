using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TemporaryTextUIScript : MonoBehaviour {

    public Transform cam;

    private void Start()
    {
        foreach (Transform child in cam)
        {
            string fileName = child.GetComponent<Renderer>().material.name;

            char[] fileChars = fileName.ToCharArray();
            int index = fileChars.Length - 1;
            LinkedList<char> reverse = new LinkedList<char>();

            //Debug.Log("----File Name: + " + fileName);

            int k = 0;
            while (!fileChars[index].Equals("-"))
            {
                if (!System.Char.IsDigit(fileChars[index]))
                {
                    continue;
                }
                reverse.AddLast(fileChars[index]);
                k++;
                if(k == 10)
                {
                    Debug.Log("ERROR");
                    return;
                }
            }

            //Debug.Log("Product: " + fileName + " || " + reverse.ToString());
        }
    }
}
