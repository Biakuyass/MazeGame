using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPoints : MonoBehaviour {

    public GameObject gameManager;
    public GameObject[] areaWalls;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Player")
        {
            gameManager.GetComponent<GameManager>().Damaged();
            /*for (int i = 0; i < areaWalls.Length; i++)
            {
                Destroy(areaWalls[i]);
            }*/

            EventManager.TriggerEvent<ClearEvent, Vector3>(transform.position);

             Destroy(gameObject);
        }

    }
}
