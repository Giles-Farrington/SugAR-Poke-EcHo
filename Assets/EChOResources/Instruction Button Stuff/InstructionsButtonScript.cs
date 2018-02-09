using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsButtonScript : MonoBehaviour {

	// Use this for initialization


	public GameObject instructions;

	
	// Update is called once per frame
	public void Button_Click() {
		

		if (instructions.activeInHierarchy || instructions.activeSelf) {
			instructions.SetActive (false);
		} else {
			instructions.SetActive (true);
		}

	}

}
