using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCSController : MonoBehaviour
{

    public enum State {
        None,
        Greeting
    }

    public State state = State.None;

    TextMesh text;
    float speakDelay;
    float lastSpeak;
    int greetingIdx;
    List<string> greetingDialog;

    // Use this for initialization
    void Start() {

        speakDelay = 4f;
        lastSpeak = 0f;
        greetingIdx = 0;
        greetingDialog = new List<string>
        {
            "Oh hello there",
            "...",
            "You may be wondering where you are.",
            "You are trapped so it doesn't matter",
            "but I'll tell you anyway, I've nothing else to do",
            "Outside these black walls is maze",
            "it changes occasionally and is full of monsters.",
            "I think I remember hearing something...",
            "Ah yes, if you beat four puzzles found in the maze",
            "you can escape... but no ones ever done it before.",
            ""
        };

        text = GetComponentInChildren<TextMesh>();
        TransitionToStateNone();
    }

    public void Interact() {
        if (state == State.None) {
            TransitionToStateGreeting();
        }
    }
    
    void TransitionToStateNone() {

        //print("Transition to state A");
        text.text = "";
        state = State.None;
    }

    void TransitionToStateGreeting() {
        text.text = greetingDialog[0];
        state = State.Greeting;
    }

    void FixedUpdate() {
		
        switch (state) {
            case State.None:
                break;
            case State.Greeting:
                GameObject target = GameObject.Find("Player");
                //https://answers.unity.com/questions/161053/making-an-object-rotate-to-face-another-object.html
                var lookPos = target.transform.position - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                float damping = 1f;
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping); 
                if ((Time.timeSinceLevelLoad - lastSpeak) > speakDelay 
                        && greetingIdx < greetingDialog.Count) {
                    lastSpeak = Time.timeSinceLevelLoad;
                    text.text = greetingDialog[greetingIdx];
                    greetingIdx += 1;
                }
                break;

            default:
                print("Weird?");
                break;
        }
    }
}
