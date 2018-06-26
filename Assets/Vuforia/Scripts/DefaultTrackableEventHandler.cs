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
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;




/// <summary>
///     A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class DefaultTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    #region PRIVATE_MEMBER_VARIABLES

    protected TrackableBehaviour mTrackableBehaviour;	//Gives access to the trackable behaviour of image targets
	protected TrackableBehaviour.Status mStatus;		//An enum used specifically for calling the OnTrackableStateChanged method, status can be set to different values. Used for newStatus within the OnTrackableState Changed method.
	protected TrackableBehaviour.Status nStatus;		//Same as above ^^, although rather than being used for newStatus within the OnTrackableStateChanged method, it's used for previousStatus
	private string imgTargetName = " ";					//String that is consistently set to current image target name
	private double distance = 0.4;						//Value consistently updated with current distance 
	private double[] prevDist = new double[4];			//Used for determining whether or not distance is same multiple frames in a row within OnTrackableStateChanged method
	private int count = 0;

	//Text text_mesh_label;


	//Keeps track of prevDist current array value
	

    #endregion // PRIVATE_MEMBER_VARIABLES

    #region UNTIY_MONOBEHAVIOUR_METHODS

    protected virtual void Start()
    {
		//text_mesh_label = GameObject.Find ("Text").GetComponent<Text> ();
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
		mStatus = TrackableBehaviour.Status.DETECTED;		//mStatus initialized as DETECTED so first image target may be found
		nStatus = TrackableBehaviour.Status.NOT_FOUND;		//nStatus initialized as NOT_FOUND and never changes, since previousStatus doesn't really matter when finding new image target
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
	
    }
	protected virtual void Update(){
		
		Vector3 delta = Camera.main.transform.position - mTrackableBehaviour.transform.position; //Gets updated current image target distance every update frame
		distance = delta.magnitude;
		//Debug.Log("DISTANCE: " + distance);
	
		if(distance < 0.073){		//If distance is below 0.073, that is lower than the camera can track. This is mostly for initialization since distance defaults to a really low number.
			distance = 0.4;			//thus we set the distance in between the minimum and maximum threshold, which as of now is 0.3 and 0.5, so it's set to 0.4
		}
		if (distance > 0.5){		//If distance is above maximum threshold (at the moment it is 0.5)
			count = 0;				//count resets
			//transform.Find("CloudRecognition").GetComponent<SimpleCloudHandler>().Reset();  //UNCOMMENT this line if using cloud database (WARNING: I HAVE NOT TESTED cloud database since script was rewritten so may not work or receive errors)
			OnTrackingLostTwo();	//OnTrackingLostTwo() is called. Which removes the added sugar AR display on screen. 
									//OnTrackingLostTwo() is called rather than OnTrackingLost() because if OnTrackingLost() is called outside of OnTrackableStateChanged method it messes with stuff.
		}
		else if (distance < 0.3){	//If distance is below minimum threshold, OnTrackableStateChanged() is called so new image target can be tracked and added sugar displayed on screen
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
		//if new image target is ready to be tracked by camera and last image target was cleared by having same distance for multiple frames in row, thus mStatus = TrackableBehaviour.Status.NOT_FOUND;
		if(newStatus == TrackableBehaviour.Status.TRACKED && mStatus == TrackableBehaviour.Status.NOT_FOUND){  
			mStatus = TrackableBehaviour.Status.DETECTED;				//mStatus is reset to DETECTED so new image target can be tracked and added sugar value displayed
			Debug.Log("SHOULD BE RESETTING AFTER IMAGE SHOWN");
		}
        if ((newStatus == TrackableBehaviour.Status.DETECTED ||			//if new target is just found and currently being detected or tracked and the distance is less than the minimum threshold 
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) &&
			distance < 0.3)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
			imgTargetName = mTrackableBehaviour.TrackableName;			//Gets current image target name 
			transform.Find("TeaspoonCounter 2 1").GetComponent<CounterScript>().GetTeaspoonValue(imgTargetName);	//Calls teaspoon counter script to get added sugar info
			OnTrackingFound();				//OnTrackingFound() is called in order to display added sugar info on screen and tracks current image target
			
			//Next 8 lines specifically written to determine whether or not image target left screen by checking to see if distance is exactly the same for multiple frames in a row.
			prevDist[count] = distance;				//Sets prevDist of current count to current distance 	
			if (count == 3){						//If count makes it to 3, then most likely distance has been exactly the same for 4 frames in a row (0,1,2,3)
				if(prevDist[0] == prevDist[3]){		//If the first set prevDist is equal to newest set prevDist, then distance has definitely been exactly the same for 4 frames in a row
					Debug.Log("FLAG SET OFF!!!");
					mStatus = TrackableBehaviour.Status.NOT_FOUND;	//mStatus is set to NOT_FOUND so next time OnTrackableStateChanges is called through update function it won't display added sugar info and stops tracking
				}
				count = -1; 	//Since maximum count value has been reached, resets to -1 due to next line incrementing by 1 and setting it back to 0
			}
			count = count + 1;	//increments count tracker
			
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&		//if previousStatus TRACKED and newStatus NOT_FOUND 
                 newStatus == TrackableBehaviour.Status.NOT_FOUND)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
			count = 0;				//count reset to 0
			//transform.Find("CloudRecognition").GetComponent<SimpleCloudHandler>().Reset();  //UNCOMMENT this line if using cloud database (WARNING: I HAVE NOT TESTED cloud database since script was rewritten so may not work or receive errors)
			OnTrackingLost();		//Removes added sugar value from screen and drops tracking of current image target
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
			count = 0;		//count reset to 0
			//transform.Find("CloudRecognition").GetComponent<SimpleCloudHandler>().Reset();  //UNCOMMENT this line if using cloud database (WARNING: I HAVE NOT TESTED cloud database since script was rewritten so may not work or receive errors)
            OnTrackingLost();		//Removes added sugar value from screen and drops tracking of current image target
        }
		
    }

    #endregion // PUBLIC_METHODS

    #region PRIVATE_METHODS

    protected virtual void OnTrackingFound()	//Starts tracking and displays added sugar values
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


		//Read "Sugar Poke.txt" and retrieve sugar ingredients for the tracked product
		/*TextAsset txt = (TextAsset)Resources.Load("Sugar Poke", typeof(TextAsset));
		List<string> productName = new List<string> ();
		List<string> sugarIngredients = new List<string> ();
		List<string> targets = new List<string> ();
		string content = txt.text;
		string[] product = content.Split (new char[] { '\n' });
		for (int i = 0; i < product.Length - 1; i++) {
			string[] itemInProduct = product [i].Split (new char[]{ '\t' });
			productName.Add (itemInProduct [1]);
			targets.Add (itemInProduct [3]);
			sugarIngredients.Add (itemInProduct [7]);
		}

		//ckeck if the tracked product exists in the list
		if (targets.Contains(imgTargetName)) {
			text_mesh_label.text = sugarIngredients[targets.IndexOf (imgTargetName)];
		}
		*/


		//text_mesh_label.text = sugarIngredients[targets.IndexOf (imgTargetName)];

		//


    }


    protected virtual void OnTrackingLost()		//Stops tracking and displaying added sugar values (Called inside OnTrackableStateChanged method)
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
	
	protected virtual void OnTrackingLostTwo(){		//Stops tracking and displaying added sugar values without messing with OnTrackableStateChanged method (Called outside of the OnTrackableStateChanged method)
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