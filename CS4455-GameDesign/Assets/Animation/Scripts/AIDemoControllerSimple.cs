using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(AINavSteeringController))]
[RequireComponent(typeof(NavMeshAgent))]
public class AIDemoControllerSimple : MonoBehaviour
{

    public enum State {
        None,
        Greeting
    }

    public State state = State.None;

    public float waitTime = 5f;

    protected float beginWaitTime;


    AINavSteeringController aiSteer;
    NavMeshAgent agent;
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


        aiSteer = GetComponent<AINavSteeringController>();
        aiSteer.mecanimInputForwardSpeedCap = 0.75f;

        agent = GetComponent<NavMeshAgent>();

        text = GetComponentInChildren<TextMesh>();

        Debug.Log("NavMesh:avoidancePredictionTime(default): " + NavMesh.avoidancePredictionTime);

        //NavMesh.avoidancePredictionTime = 4f;

        aiSteer.Init();

        aiSteer.waypointLoop = false;
        aiSteer.stopAtNextWaypoint = false;

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

        //GameObject target = GameObject.Find("Player");
        //aiSteer.setWaypoint(target.transform);

        //aiSteer.useNavMeshPathPlanning = true;
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
