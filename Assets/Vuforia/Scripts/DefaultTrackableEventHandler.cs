/* /* /*==============================================================================
Copyright (c) 2017 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/
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
	protected TrackableBehaviour.Status mStatus;
	protected TrackableBehaviour.Status nStatus;
	private string imgTargetName = " ";
	private double distance = 0.5;
	private double[] prevDist = new double[10];
	private int count = 0;
	private int flag = 0;
	

    #endregion // PRIVATE_MEMBER_VARIABLES

    #region UNTIY_MONOBEHAVIOUR_METHODS

    protected virtual void Start()
    {
		
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
		mStatus = TrackableBehaviour.Status.DETECTED;
		nStatus = TrackableBehaviour.Status.NOT_FOUND;
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
    }
	protected virtual void Update(){
		Vector3 delta = Camera.main.transform.position - mTrackableBehaviour.transform.position; //Resets distance to new current image target distance
		distance = delta.magnitude;
	
		if(distance < 0.09){
			distance = 0.5;
		}
		if (distance > 0.6){
			count = 0;
			OnTrackingLostTwo();
		}
		if (distance < 0.3){
			OnTrackableStateChanged(nStatus, mStatus);
		}	
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
		if(newStatus == TrackableBehaviour.Status.TRACKED && flag == 1){
			mStatus = TrackableBehaviour.Status.DETECTED;
			Debug.Log("SHOULD BE RESETTING AFTE RIMAGE SHWONE");
		}
        if ((newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) &&
			distance < 0.3)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
			imgTargetName = mTrackableBehaviour.TrackableName;
			transform.Find("TeaspoonCounter 2 1").GetComponent<CounterScript>().GetTeaspoonValue(imgTargetName);
            OnTrackingFound();
			prevDist[count] = distance;
			if (count == 9){
				if(prevDist[0] == prevDist[9]){
					Debug.Log("FLAG SET OFF!!!");
					flag = 1;
					mStatus = TrackableBehaviour.Status.NOT_FOUND;
				}
				count = 0;
			}
			count = count + 1;
			
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NOT_FOUND)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
			count = 0;
            OnTrackingLost();
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
			count = 0;
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
	
	protected virtual void OnTrackingLostTwo(){
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