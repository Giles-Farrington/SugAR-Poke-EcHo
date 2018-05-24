using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RetrieveIngredients : MonoBehaviour {

	// Use this for initialization
	public void SugarList(string productName)
	{
		Text text_mesh_label;
		text_mesh_label = GameObject.Find ("Text").GetComponent<Text> ();
		text_mesh_label.text = "NONE";

		switch (productName) {
		case "CapeCod-FortyPercentRFCapeCod-twentyeightOZ-0":
			text_mesh_label.text = "1. Sugar" + "\n" + "2. Syrup" + "\n" + "3. Syrup" + "\n" + "4. Syrup" + "\n" + "5. Syrup";
			break;
		}
	}
}
