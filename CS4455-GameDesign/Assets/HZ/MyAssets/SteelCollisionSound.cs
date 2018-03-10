using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelCollisionSound : MonoBehaviour {


    public C_SOUNDS materialType;
    public float mag = 2;
    public bool isBarrel = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision c)
    {
       // if(isBarrel)
       // Debug.Log(c.impulse.magnitude);
        if (c.impulse.magnitude > mag)
            EventManager.TriggerEvent<CollisionSound, Vector3, C_SOUNDS>(c.contacts[0].point, materialType);
 
    }
    void OnCollisionStay(Collision c)
    {
        if (c.impulse.magnitude > mag && isBarrel)
            EventManager.TriggerEvent<CollisionSound, Vector3, C_SOUNDS>(c.contacts[0].point, materialType);
    }
}
