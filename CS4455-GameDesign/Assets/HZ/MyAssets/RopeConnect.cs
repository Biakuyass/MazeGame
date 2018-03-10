using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeConnect : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<ConfigurableJoint>().connectedBody = transform.parent.GetComponent<Rigidbody>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
