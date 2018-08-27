using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWebpage : MonoBehaviour {

	public void OpenPage(string page)
    {
        Application.OpenURL(page);
    }
}
