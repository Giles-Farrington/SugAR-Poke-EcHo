using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TemporaryUIScript : MonoBehaviour {

    /// <summary>
    /// Attach the AR Cam after placing this script in the scene
    /// </summary>
    public Transform camera;
    public GameObject replacer;
    [SerializeField]
    public static int x = 0;

    private void Update()
    {
        if(x == 2)
        {
            Debug.Log("x == 0");
            return;
        }

        Debug.Log("Started Script");
        foreach (Transform child in camera)
        {
            /*
            foreach(Transform c in child)
            {
                DestroyImmediate(c.gameObject);
            }
            */

            //Transform tsp = Instantiate(replacer, child).transform;
            //tsp.name = "TeaspoonCounter 2 1";

            Transform tsp = child.Find("TeaspoonCounter 2 1");
			tsp.localPosition = new Vector3(.086f, -.031f, -.007f);
            
        }
        x = 2;
        Debug.Log("Complete");
        DestroyImmediate(this.gameObject);
    }
}
