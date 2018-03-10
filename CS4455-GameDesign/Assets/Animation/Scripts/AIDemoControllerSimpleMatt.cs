using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(AINavSteeringController))]
[RequireComponent(typeof(NavMeshAgent))]
public class AIDemoControllerSimpleMatt : MonoBehaviour
{

    public enum State {
        None,
        Greeting
    }

    public State state = State.None;


    public GameObject player;
    public float waitTime = 5f;

    protected float beginWaitTime;

    GameObject npc0Object;
    AIDemoControllerSimple npc0Control;

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
            "Ah, it's you again!",
            "I'm glad to see you're still in one piece.",
            "What's that? Of course, why wouldn't you be!",
            "You see that? That there's a Percamore laser.",
            "It'll roast you right through in a second if you're not careful!",
            "Don't worry though, there's a secret door to the next room.",
            "Then it's just a matter of...well, no one's gotten that far.",
            "Knowing the bunny boss though, it'll probably\nbe some kind of shrine or picture of him",
            "Good luck friend! And be careful!",
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

        npc0Object = this.gameObject;
        npc0Control = npc0Object.GetComponent<AIDemoControllerSimple>();

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

    private void Update()
    {
        if ((npc0Object.transform.position - player.transform.position).sqrMagnitude < 15f) {
            Interact();
        }
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
