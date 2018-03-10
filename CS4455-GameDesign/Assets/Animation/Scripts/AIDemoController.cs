using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(AINavSteeringController))]
[RequireComponent(typeof(NavMeshAgent))]
public class AIDemoController : MonoBehaviour
{


    public Transform[] waypointSetA;

    public Transform[] waypointSetB;

    public Vector3 waypointC;

    public Transform waypointE;

    public Vector3 waypointF;

    private Transform leftHand;
    private Animator anim;
    private Rigidbody rbody;

    public enum State
    {

        A, // path planning
        B, // stationary waypoint
        C, // intercept
        D, // aimed throw
        E, //
        R  // goes to random state

    }



    public State state = State.A;

    public float waitTime = 2f;

    protected float beginWaitTime;


    AINavSteeringController aiSteer;
    NavMeshAgent agent;
    TextMesh text;
    public void FootStep()
    {
    }

        // Use this for initialization
        void Start()
    {
        leftHand = this.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand");
        anim = GetComponent<Animator>();
        if (anim == null)
            Debug.Log("Animator could not be found");
        rbody = GetComponent<Rigidbody>();
        if (rbody == null)
            Debug.Log("Rigid body could not be found");

        aiSteer = GetComponent<AINavSteeringController>();

        agent = GetComponent<NavMeshAgent>();

        text = GetComponent<TextMesh>();

        Debug.Log("NavMesh:avoidancePredictionTime(default): " + NavMesh.avoidancePredictionTime);

        //NavMesh.avoidancePredictionTime = 4f;

        aiSteer.Init();

        aiSteer.waypointLoop = false;
        aiSteer.stopAtNextWaypoint = false;

        transitionToStateD();
		
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

        aiSteer.setWaypoints(waypointSetA);

        aiSteer.useNavMeshPathPlanning = true;


    }


    void transitionToStateB()
    {

        print("Transition to state B");
        text.text = "State: B";

        state = State.B;

        aiSteer.setWaypoints(waypointSetB);

        aiSteer.useNavMeshPathPlanning = true;
    }


    void transitionToStateC()
    {

        print("Transition to state C");
        text.text = "State: C";

        state = State.C;

        //aiSteer.setWaypoints(waypointC);
        GameObject target = GameObject.Find("NPC_1");
        float d = Vector3.Distance(this.transform.position, target.transform.position);
        Vector3 targetPosition = target.transform.position + target.transform.forward * (float)((target.GetComponent<Rigidbody>().velocity.magnitude) * System.Math.Sqrt(d));
        aiSteer.setWaypoint(targetPosition);
        //aiSteer.clearWaypoints();

        aiSteer.useNavMeshPathPlanning = true;

    }

    void transitionToStateD()
    {

        print("Transition to state D");
        text.text = "State: D";

        state = State.D;

        aiSteer.clearWaypoints();

        beginWaitTime = Time.timeSinceLevelLoad;

        aiSteer.useNavMeshPathPlanning = true;
        

    }

    void transitionToStateE()
    {
        print("Transition to state D");
        text.text = "State: D";

        state = State.E;

        aiSteer.clearWaypoints();

        beginWaitTime = Time.timeSinceLevelLoad;

        aiSteer.useNavMeshPathPlanning = true;

        
        anim.SetTrigger("throw");
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.transform.position = leftHand.position;
        ball.transform.position += Vector3.down * 0.2f;
        ball.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        Rigidbody ballRBody = ball.AddComponent<Rigidbody>();
        ballRBody.isKinematic = true;
        ballRBody.useGravity = false;
        ball.GetComponent<Collider>().enabled = false;
        ball.transform.parent = leftHand;
        
    }

    /*public void Throw()
    {
        EventManager.TriggerEvent<NpcEvent, Vector3, int>(this.transform.position, 2);
        for (int i = 0; i < leftHand.childCount; i++)
        {
            //print(leftHand.GetChild(i));
            if (leftHand.GetChild(i).ToString().Contains("Sphere"))
            {
                leftHand.GetChild(i).GetComponent<Rigidbody>().isKinematic = false;
                leftHand.GetChild(i).GetComponent<Rigidbody>().useGravity = true;
                leftHand.GetChild(i).GetComponent<Collider>().enabled = true;
                GameObject target = GameObject.Find("NPC_1");
                

                float p = 15;
                float d = Vector3.Distance(this.transform.position, target.transform.position);
                print("magnitude:" + (targetVel.magnitude).ToString());
                //Vector3 targetPosition = target.transform.position + target.transform.forward * (float) ((targetVel.magnitude) * d);

                float ballToTargetTime = d / p;
                print("ballToTarget" + ballToTargetTime.ToString());
                print("a" + (targetVel.magnitude).ToString());
                print("b" + (targetVel.magnitude * ballToTargetTime).ToString());
                Vector3 interceptPoint = target.transform.position + target.transform.forward * targetVel.magnitude * 50 * ballToTargetTime;

                Vector3 targetPosition = interceptPoint;
                print("d" + d.ToString());
                print("fuck" + ((System.Math.Pow(p,2) + System.Math.Sqrt(System.Math.Pow(p, 4) - 10 * (10 * System.Math.Pow(d, 2)))) / (10 * d)).ToString());
                double myTheta0 = System.Math.Atan((System.Math.Pow(p,2) + System.Math.Sqrt(System.Math.Pow(p, 4) - 10 * (10 * System.Math.Pow(d, 2)))) / (10 * d));
                print("myTheta0" + myTheta0.ToString());
                double myTheta1 = System.Math.Atan((System.Math.Pow(p,2) - System.Math.Sqrt(System.Math.Pow(p, 4) - 10 * (10 * System.Math.Pow(d, 2)))) / (10 * d));
                print("myTheta1" + myTheta1.ToString());
                double myTheta = System.Math.Min(myTheta0, myTheta1);
                print("myTheta" + myTheta.ToString());
                double myThetaDegree = (myTheta) * (180 / 3.14);
                print("myThetaDegree"+myThetaDegree.ToString());

                Vector3 imconfused = targetPosition - this.transform.position;
                imconfused = Vector3.Normalize(imconfused);
                Vector3 cross = Vector3.Cross(imconfused, Vector3.up);
                imconfused = Quaternion.AngleAxis((float) myThetaDegree, cross) * imconfused;
                imconfused = imconfused * 1000;
                imconfused = Vector3.ClampMagnitude(imconfused, p - 1f);

                //Vector3 imconfused = targetPosition - this.transform.position;
                //imconfused.z = (float) myTheta;
                //imconfused = Vector3.ClampMagnitude(imconfused, 15);

                //leftHand.GetChild(i).GetComponent<Rigidbody>().velocity = imconfused + this.transform.up * (float) myTheta;
                leftHand.GetChild(i).GetComponent<Rigidbody>().velocity = imconfused;

                leftHand.GetChild(i).parent = null;
            }
        }
    }*/

    Vector3 targetVel;
    Vector3 targetLastPos;
    private void FixedUpdate()
    {
        GameObject target = GameObject.Find("NPC_1");
        targetVel = target.GetComponent<Rigidbody>().position - targetLastPos;
        targetLastPos = target.GetComponent<Rigidbody>().position;
    }

    // Update is called once per frame
    void Update()
    {

        GameObject target = GameObject.Find("NPC_1");
        float p = 15;
        float d = Vector3.Distance(this.transform.position, target.transform.position);
        float ballToTargetTime = d / p;
        Vector3 interceptPoint = target.transform.position + target.transform.forward * targetVel.magnitude * 50 * ballToTargetTime;

                Vector3 targetPosition = interceptPoint;

        switch (state)
        {
            case State.R:
                int r = Random.Range(0, 4);
                //r = 3;
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
                if (aiSteer.waypointsComplete()) {
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
                if (d <= 1)
                {
                    aiSteer.clearWaypoints();
                    transitionToStateR();
                }
                if (aiSteer.waypointsComplete())
                {
                    EventManager.TriggerEvent<NpcEvent, Vector3, int>(this.transform.position, 1);
                    transitionToStateR();
                }                    
                else
                {
                    aiSteer.setWaypoint(targetPosition);
                }
                break;

            case State.D:
                aiSteer.setWaypoint(targetPosition);

                float minD = (float) System.Math.Sqrt(System.Math.Pow(15, 4) / 100);
                if (Vector3.Distance(this.transform.position, target.transform.position) < minD - 5)
                //if (Time.timeSinceLevelLoad - beginWaitTime > waitTime)
                //if (true)
                {
                    transitionToStateE();
                }
                break;

            case State.E:
                aiSteer.mecanimInputForwardSpeedCap = 0;
                aiSteer.setWaypoint(targetPosition);
                if (Time.timeSinceLevelLoad - beginWaitTime > waitTime)
                {
                    print("y tho");
                    aiSteer.mecanimInputForwardSpeedCap = 1;
                    transitionToStateR();
                }
                    
                break;

            default:

                print("Weird?");
                break;
        }


    }

    
}
