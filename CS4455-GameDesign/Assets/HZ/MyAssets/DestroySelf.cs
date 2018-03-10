using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DestroySelf : MonoBehaviour {

    public float lifetime = 0;
	// Use this for initialization
	void Start () {
        StartCoroutine(Death());
		
	}
	
	// Update is called once per frame
	void Update () {
        
		
	}

    IEnumerator Death()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
