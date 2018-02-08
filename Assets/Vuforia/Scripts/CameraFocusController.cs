using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class CameraFocusController : MonoBehaviour {
	void Start ()
	{
		Debug.Log ("I'm alive");
		//var vuforia = VuforiaARController.Instance;
		//vuforia.RegisterVuforiaStartedCallback(OnVuforiaStarted);
		//vuforia.RegisterOnPauseCallback(OnPaused);

		VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted); 
		VuforiaARController.Instance.RegisterOnPauseCallback(OnPaused);
	
	}

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
