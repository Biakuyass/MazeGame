using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour {

    public GameObject Explosion;
    public float rotateSpeed = 0.3f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(1,0,0), rotateSpeed);
		
	}

    /*void OnCollisionEnter(Collision collision)
    {
        Instantiate(Explosion, transform.position,transform.rotation);
        Destroy(gameObject);
    }*/
    void OnTriggerEnter(Collider collision)
    {
        Instantiate(Explosion, transform.position, transform.rotation);

        if (collision.tag == "Player")
            collision.GetComponent<PlayerHealthPoint>().Hurt();

        EventManager.TriggerEvent<BossAttackEvent, Vector3>(transform.position);

        Destroy(gameObject);
    }
}
