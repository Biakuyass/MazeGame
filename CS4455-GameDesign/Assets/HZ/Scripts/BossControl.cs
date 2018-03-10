using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossControl : MonoBehaviour {

    public GameObject missile;
    public float rotatespeed = 20;
    public float maxImpulse;
    public float minImpulse;
    public Transform gunPosition;
    private float timerecord = 0;
    private float limittime = 2;

    bool alive = true;
    private Animator anim;
	// Use this for initialization
	void Start () {

        anim = GetComponent<Animator>();
		
	}
	
	// Update is called once per frame
	void Update () {
        if (alive)
        {
            timerecord += Time.deltaTime;
            if (timerecord >= limittime)
            {
                timerecord = 0;
                Attack();

            }

            transform.Rotate(new Vector3(0, 1, 0), Time.deltaTime * rotatespeed);
 
        }
       
        
	}
    void Attack()
    {


        GameObject m = Instantiate(missile, gunPosition.position, gunPosition.rotation);

        Vector3 impulse = transform.forward * Random.Range(minImpulse,maxImpulse) ;

        m.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        m.GetComponent<Rigidbody>().AddForce(impulse, ForceMode.Impulse);
 
    }

    public void Death()
    {
        anim.SetBool("Dead",true);
        alive = false;
 
    }
}
