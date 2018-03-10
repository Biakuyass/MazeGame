using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMove : MonoBehaviour {

 //   public float maxspeed = 0;
    Rigidbody rigid;
	// Use this for initialization
    bool upflag = true;
    float timerecord = 0;
	void Start () {
        rigid = this.GetComponent<Rigidbody>();
        rigid.velocity = new Vector3(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
        /*timerecord += Time.deltaTime;


        if (timerecord >= 3)
        {
            Debug.Log(rigid.velocity);
            timerecord = 0;
            rigid.velocity *= -1;
        }*/

        if (rigid.velocity.y < 3 && upflag)
            rigid.velocity += new Vector3(0, 9.8f * Time.deltaTime, 0);
        if (rigid.velocity.y > -3 && !upflag)
            rigid.velocity += new Vector3(0, -9.8f * Time.deltaTime, 0);

        if (transform.position.y > 8)
            upflag = false;
        if (transform.position.y < 0.6f)
            upflag = true;
      
		
	}
}
