/*==============================================================================
Copyright (c) 2017 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;
using Vuforia;

/// <summary>
///     A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class DefaultTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    #region PRIVATE_MEMBER_VARIABLES

    protected TrackableBehaviour mTrackableBehaviour;
	private string imgTargetName = " ";
	private float distance = 0;
	

    #endregion // PRIVATE_MEMBER_VARIABLES

    #region UNTIY_MONOBEHAVIOUR_METHODS

    protected virtual void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
    }
	
	protected virtual void Update(){
		if(mTrackableBehaviour && distance < 0.6){
			Vector3 delta = Camera.main.transform.position - mTrackableBehaviour.transform.position;
			distance = delta.magnitude;
			Debug.Log("Trackable DISTANCE IS: " + distance);
			if(distance >= 0.6){
				Debug.Log("Trackable LOST" + mTrackableBehaviour.TrackableName + " lost");
				OnTrackingLost();
				GameObject.Find("CloudRecognition").GetComponent<SimpleCloudHandler>().ResetTarget();
			}
		}
		Vector3 delta2 = Camera.main.transform.position - mTrackableBehaviour.transform.position;
		distance = delta2.magnitude;
		
		
		
	}


    #endregion // UNTIY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
		Debug.Log("DISTANCE: " + distance);
		
        if ((newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) &&
			distance < 0.35)
        {
			imgTargetName = mTrackableBehaviour.TrackableName;
            Debug.Log("Trackable FILENAME " + imgTargetName + " found");
			transform.Find("TeaspoonCounter 2 1").GetComponent<CounterScript>().GetTeaspoonValue(imgTargetName);
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NOT_FOUND)
        {
            Debug.Log("Trackable LOST" + mTrackableBehaviour.TrackableName + " lost");
			//transform.Find("TeaspoonCounter 2 1").GetComponent<CounterScript>().DestroyTsp();
			GameObject.Find("CloudRecognition").GetComponent<SimpleCloudHandler>().ResetTarget();
            OnTrackingLost();
        }
        else
        {
			GameObject.Find("CloudRecognition").GetComponent<SimpleCloudHandler>().ResetTarget();
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }
	
    #endregion // PUBLIC_METHODS

    #region PRIVATE_METHODS

    protected virtual void OnTrackingFound()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = true;

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Enable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;
    }


    protected virtual void OnTrackingLost()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = false;
    }
	
	
	

    #endregion // PRIVATE_METHODS
}
