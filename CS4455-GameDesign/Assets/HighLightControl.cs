using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighLightControl : MonoBehaviour {

    //public UnityEngine.UI.Button button2;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}
    public void SelectButton()
    {
       // Debug.Log(1);
        gameObject.GetComponent<UnityEngine.UI.Button>().Select();
    }
}
