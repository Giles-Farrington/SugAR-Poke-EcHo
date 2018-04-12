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
	private float[] distance = new float[10];
	private int count = 0;
	private float prevDist = 0;
	private float prevDist2 = 0;
	

    #endregion // PRIVATE_MEMBER_VARIABLES

    #region UNTIY_MONOBEHAVIOUR_METHODS

	//On start, creates trackable behaviour event handler
    protected virtual void Start()
    {
		distance[0] = 0;
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
    }
	
	//On update gets current distance of the current image target and depending on distance of current image target, acts appropiately. 
	
	//Essentially 
	protected virtual void Update(){
		if(distance[0] == 1){
			Vector3 delta = Camera.main.transform.position - mTrackableBehaviour.transform.position; //Resets distance to new current image target distance
			prevDist2 = delta.magnitude;
			Debug.Log("IMPORTANT IMPORTANT LOOK AT THIS!!!!!!!: " + prevDist);
			if(prevDist != prevDist2 && prevDist2 != 0.0007){
				distance[0] = prevDist2;
			}
		}
		Debug.Log("UPDATING!!! DIstance:" + distance[0]);
		if(mTrackableBehaviour && distance[0] <= 0.7 && distance[0] != 0.005 && distance[0] != 0.0007){   //Only looks for new distance if current image target distance is less than the maximum distance threshold.
			if(distance[0] < 0.3 && distance[0] != 0 && distance[0] != 0.0007) {	//If distance is below 0.1m, most likely a new image target and gets the name, displays the added sugar value and calls OnTrackingFound()function
				Debug.Log("DISTANCE STILL LESS THAN 0.2!! " + distance[0] );
				imgTargetName = mTrackableBehaviour.TrackableName;
				transform.Find("TeaspoonCounter 2 1").GetComponent<CounterScript>().GetTeaspoonValue(imgTargetName);
				OnTrackingFound();
			}
			else if(distance[0] >= 0.6){	//If distance is greater than maximum threshold (Currently 0.3m), Calls OnTrackingLost() Function
				Debug.Log("Trackable LOST" + mTrackableBehaviour.TrackableName + " lost");
				OnTrackingLost();
				//While utilizing the cloud database scene, uncomment line of code below. When switching back to local database scenes, make sure to re-comment 
				//GameObject.Find("CloudRecognition").GetComponent<SimpleCloudHandler>().ResetTarget();
			}
		}
		if(distance[0] != 1){
			Vector3 delta2 = Camera.main.transform.position - mTrackableBehaviour.transform.position; //Resets distance to new current image target distance
			distance[0] = delta2.magnitude;
			Debug.Log("NEW Trackable DISTANCE IS: " + distance[0]);
		}
		if (count < 10){
			distance[count] = distance[0];
		}
		if ((distance[9] == distance[0]) && (distance[0] == distance[3]) && (distance[0] == distance[6]) && distance[0] != 0 && distance[0] != 1){
			Debug.Log("DISTANCE IS SAME, LOSING TRACKING" + distance[0] + distance[1] + distance[2] + distance[9]);
			OnTrackingLost();
			count = -1;
			prevDist = distance[0];
			distance[0] = 1;
		}
		if(count >= 10 && count != -1){
			count = -1;
		}
		else if(count >= 10){
			count = 0;
		}
		count = count + 1;
		
		
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
