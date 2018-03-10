using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthPoint : MonoBehaviour {

    public int MaxHealth = 5;

    [HideInInspector]
    public int healthPoint;

	// Use this for initialization
	void Start () {
        healthPoint = MaxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Hurt()
    {
        Hurt(1);
    }

    public void Hurt(int damage)
    {
        healthPoint -= damage;
        if (healthPoint <= 0)
            Death();

    }

    void Death()
    {
        gameObject.GetComponent<YBotSimpleControlScript>().PlayerDeath();
    }
}
