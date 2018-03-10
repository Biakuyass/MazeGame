using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterCaveScript : MonoBehaviour {

    float speed = 6.5f;
    float maxIntensity = 15f;

    // Use this for initialization
    void Start () {
        GameObject.Find("cave_light").GetComponent<Light>().intensity = 0f;
    }
	
	// Update is called once per frame
	void Update () {
        if (GameObject.Find("cave_light").GetComponent<Light>().enabled) {
            GameObject.Find("cave_light").GetComponent<Light>().intensity = Mathf.PingPong(Time.time * speed, maxIntensity);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "YBot" && GameObject.Find("pieces").GetComponent<PuzzleCheckerScript>().puzzleCompleted) {
            SceneManager.LoadScene("Maze");
        }
    }
}
