using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataRectifier : MonoBehaviour {

    [HideInInspector]
    public bool detectedImageValidity;
    [HideInInspector]
    public string correctedFoodName;
    [HideInInspector]
    Transform targetTransform;

    public void CorrectInformation(Transform trans, string foodName)
    {
        targetTransform = trans;
        correctedFoodName = foodName;
    }

    public void CorrectDisplayedTeaspoonValue()
    {
        if(correctedFoodName == null)
        {
            Debug.LogError("Food name shouldn't be null");
            detectedImageValidity = false;
            targetTransform = null;
            return;
        }
        CounterScript cs = targetTransform.Find("TeaspoonCounter 2 1").GetComponent<CounterScript>();
        string tspValue = cs.GetTeaspoonValue(correctedFoodName);
    }

    public string GetCorrectedName()
    {
        return correctedFoodName;
    }

    public bool GetImageValidity()
    {
        return detectedImageValidity;
    }

    /// <summary>
    /// Sets the value for whether of the image recognized is correct of if the color detector detected it's something different.
    /// </summary>
    /// <param name="valid"></param>
    public void SetImageValidity(bool valid)
    {
        detectedImageValidity = valid;
        targetTransform = null;
        correctedFoodName = null;
    }

    private void LateUpdate()
    {
        if (detectedImageValidity)
        {
            CorrectDisplayedTeaspoonValue();
        }
    }

}
