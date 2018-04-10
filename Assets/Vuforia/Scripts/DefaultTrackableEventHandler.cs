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

	//On start, creates trackable behaviour event handler
    protected virtual void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
    }
	
	//On update gets current distance of the current image target and depending on distance of current image target, acts appropiately. 
	protected virtual void Update(){
		Debug.Log("UPDATING!!! DIstance:" + distance);
		if(mTrackableBehaviour && distance < 0.3){   //Only looks for new distance if current image target distance is less than the maximum distance threshold
			Vector3 delta = Camera.main.transform.position - mTrackableBehaviour.transform.position; //Finds the distance
			distance = delta.magnitude;
			Debug.Log("Trackable DISTANCE IS: " + distance);
			if(distance < 0.1){	//If distance is below 0.1m, most likely a new image target and gets the name, displays the added sugar value and calls OnTrackingFound()function
								//This ensures that a target has to be close to camera in order start tracking. Thus we know for sure it has a good view of the target.
				imgTargetName = mTrackableBehaviour.TrackableName;
				transform.Find("TeaspoonCounter 2 1").GetComponent<CounterScript>().GetTeaspoonValue(imgTargetName);
				OnTrackingFound();
			}
			else if(distance >= 0.3){	//If distance is greater than maximum threshold (Currently 0.3m), Calls OnTrackingLost() Function
										//This ensures that if the current image target is far enough away from camera, then it is dropped.
				Debug.Log("Trackable LOST" + mTrackableBehaviour.TrackableName + " lost");
				OnTrackingLost();
				//While utilizing the cloud database scene, uncomment line of code below. When switching back to local database scenes, make sure to re-comment 
				//GameObject.Find("CloudRecognition").GetComponent<SimpleCloudHandler>().ResetTarget();
			}
		}
		Vector3 delta2 = Camera.main.transform.position - mTrackableBehaviour.transform.position; //Resets distance to new current image target distance
		distance = delta2.magnitude;
		
		
	}


    #endregion // UNTIY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(		//Function is called on a trackable state change
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
		Debug.Log("DISTANCE: " + distance);
		Vector3 delta = Camera.main.transform.position - mTrackableBehaviour.transform.position;  //Finds current distance of current image target
		distance = delta.magnitude;
		
        if ((newStatus == TrackableBehaviour.Status.DETECTED ||		//If a target is detected, tracked, or extended tracked, and the distance is lower than minimum distance threshold
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) &&
			distance < 0.1)
        {
			imgTargetName = mTrackableBehaviour.TrackableName;		//Gets image target name
			transform.Find("TeaspoonCounter 2 1").GetComponent<CounterScript>().GetTeaspoonValue(imgTargetName); //Displays added sugar value
            OnTrackingFound();	//Calls OnTrackingFound()
        }
		//Both else statements are essentially the same, if target is not found, stops tracking current image by calling OnTrackingLost()
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NOT_FOUND)
        {
            Debug.Log("Trackable LOST" + mTrackableBehaviour.TrackableName + " lost");
			//While utilizing the cloud database scene, uncomment line of code below. When switching back to local database scenes, make sure to re-comment 
			//GameObject.Find("CloudRecognition").GetComponent<SimpleCloudHandler>().ResetTarget();
            OnTrackingLost();
        }
        else
        {
			//While utilizing the cloud database scene, uncomment line of code below. When switching back to local database scenes, make sure to re-comment 
			//GameObject.Find("CloudRecognition").GetComponent<SimpleCloudHandler>().ResetTarget();
			
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }
	
    #endregion // PUBLIC_METHODS

    #region PRIVATE_METHODS

    protected virtual void OnTrackingFound()	//Called when image target is found
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


    protected virtual void OnTrackingLost()		//Called when image target is lost
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
