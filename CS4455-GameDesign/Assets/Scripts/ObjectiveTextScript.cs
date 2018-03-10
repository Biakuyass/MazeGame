using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveTextScript : MonoBehaviour {

    private GameObject player;
    private Text objectiveText;

    bool inSecondRoom = false;

    // Use this for initialization
    void Start () {
        objectiveText = GetComponent<Text>();
        player = GameObject.Find("YBot");

        objectiveText.text = "Objective: Open the hidden door by pressing the right buttons";
    }
	
	// Update is called once per frame
	void Update () {
        if (player.transform.position.x > -20f) {
            inSecondRoom = true;
        }

        if (inSecondRoom) {
            objectiveText.text = "Objective: Move the boxes into an image of the ferocious bunny zombie to earn your escape";
        }
    }
}
