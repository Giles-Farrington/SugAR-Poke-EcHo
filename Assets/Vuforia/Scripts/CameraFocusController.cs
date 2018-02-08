/**
 * Created by An Nguyen 
 * Date: February 8, 2018
**/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class CameraFocusController : MonoBehaviour {

	//Set ARCamera focus mode when camera starts
	void Start ()
	{
		Debug.Log ("I'm alive");

		VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted); 
		VuforiaARController.Instance.RegisterOnPauseCallback(OnPaused);
	
	}

	//Setting the focus mode to continuously auto focusing
	private void OnVuforiaStarted()
	{
		CameraDevice.Instance.SetFocusMode(
			CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
	}

	private void OnPaused(bool paused)
	{
		if (!paused) // resumed
		{
			// Set again autofocus mode when app is resumed
			CameraDevice.Instance.SetFocusMode(
				CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
		}
	}

}
