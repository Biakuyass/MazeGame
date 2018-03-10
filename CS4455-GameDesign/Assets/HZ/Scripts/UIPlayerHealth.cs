using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIPlayerHealth : MonoBehaviour {

    public GameObject player;
    private UnityEngine.UI.Text HpText;
	// Use this for initialization
	void Start () {

        HpText = gameObject.GetComponent<UnityEngine.UI.Text>();
        
		
	}
	
	// Update is called once per frame
	void Update () {

        HpText.text = "HP: " + player.GetComponent<PlayerHealthPoint>().healthPoint;
		
	}
}
