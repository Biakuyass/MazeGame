using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(AINavSteeringController))]
[RequireComponent(typeof(NavMeshAgent))]
public class AIDemoControllerSimple2 : MonoBehaviour
{

    public Transform waypointA;
    public Transform waypointB;
    public Transform waypointC;
    public Transform waypointD;

    public Vector3 waypointF;

    public enum State
    {

        A, // path planning
        B, // stationary waypoint
        C, // intercept
        D, // aimed throw
        R  // goes to random state

    }



    public State state = State.A;

    public float waitTime = 5f;

    protected float beginWaitTime;


    AINavSteeringController aiSteer;
    NavMeshAgent agent;
    TextMesh text;


    // Use this for initialization
    void Start()
    {
        aiSteer = GetComponent<AINavSteeringController>();
        aiSteer.mecanimInputForwardSpeedCap = 0.75f;

        agent = GetComponent<NavMeshAgent>();

        text = GetComponent<TextMesh>();

        Debug.Log("NavMesh:avoidancePredictionTime(default): " + NavMesh.avoidancePredictionTime);

        //NavMesh.avoidancePredictionTime = 4f;

        aiSteer.Init();

        aiSteer.waypointLoop = false;
        aiSteer.stopAtNextWaypoint = false;

        transitionToStateA();

    }

    void transitionToStateR()
    {
        print("Transition to state R");
        text.text = "State: R";

        state = State.R;
    }


    void transitionToStateA()
    {

        print("Transition to state A");
        text.text = "State: A";

        state = State.A;

        aiSteer.setWaypoint(waypointA);

        aiSteer.useNavMeshPathPlanning = true;


    }
    void transitionToStateB()
    {

        print("Transition to state B");
        text.text = "State: B";

        state = State.B;

        aiSteer.setWaypoint(waypointB);

        aiSteer.useNavMeshPathPlanning = true;


    }
    void transitionToStateC()
    {

        print("Transition to state C");
        text.text = "State: C";

        state = State.C;

        aiSteer.setWaypoint(waypointC);

        aiSteer.useNavMeshPathPlanning = true;


    }
    void transitionToStateD()
    {

        print("Transition to state D");
        text.text = "State: D";

        state = State.D;

        aiSteer.setWaypoint(waypointD);

        aiSteer.useNavMeshPathPlanning = true;


    }

    public void FootStep()
    {

    }



    // Update is called once per frame
    void Update()
    {

        switch (state)
        {
            case State.R:
                int r = Random.Range(0, 4);
                if (r == 0)
                {
                    transitionToStateA();
                }
                else if (r == 1)
                {
                    transitionToStateB();
                }
                else if (r == 2)
                {
                    transitionToStateC();
                }
                else if (r == 3)
                {
                    transitionToStateD();
                }
                break;
            case State.A:
                if (aiSteer.waypointsComplete())
                {
                    EventManager.TriggerEvent<NpcEvent, Vector3, int>(this.transform.position, 1);
                    transitionToStateR();
                }
                break;

            case State.B:
                if (aiSteer.waypointsComplete())
                {
                    EventManager.TriggerEvent<NpcEvent, Vector3, int>(this.transform.position, 1);
                    transitionToStateR();
                }
                break;

            case State.C:
                if (aiSteer.waypointsComplete())
                {
                    EventManager.TriggerEvent<NpcEvent, Vector3, int>(this.transform.position, 1);
                    transitionToStateR();
                }
                break;

            case State.D:
                if (aiSteer.waypointsComplete())
                {
                    EventManager.TriggerEvent<NpcEvent, Vector3, int>(this.transform.position, 1);
                    transitionToStateR();
                }
                break;

            default:

                print("Weird?");
                break;
        }


    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.ToString().Contains("Sphere"))
        {
            EventManager.TriggerEvent<NpcEvent, Vector3, int>(this.transform.position, 0);

        }
    }
}
