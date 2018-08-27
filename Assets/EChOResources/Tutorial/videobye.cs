using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;

public class videobye : MonoBehaviour {

    public RawImage image;
    // Use this for initialization
    void Start () {
        //image.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        VideoPlayer vp = GetComponent<VideoPlayer>();
        if ((ulong)vp.frame >= vp.frameCount-2)
        {
            //Debug.Log("Ending video");
            image.enabled = false;
        }
    }
}
