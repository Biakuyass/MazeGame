using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum AIState
{
    Patrol,
    Chase

};
public enum RobotType
{
    Blue,
    Yellow,
    Green,
    Grey
};

public class AIController : MonoBehaviour {

    public AIState state;
    public RobotType robottype;
    private AINavSteeringController ainav;
    private NavMeshAgent agent;
    private Animator anim;


    public GameObject Explosion;
    public GameObject Explosion_killed;
    public Transform[] WayPointsA;
    public Transform prey;



    //public Transform[] WayPointsB;
    //public Transform ThrowStartPoint;
   // public Transform lefthand;
   // public Transform preyBornPoint;
   // public GameObject preyPrefab;

   // private float throwFilter = 0.5f;
   // private float throwTimeRecord = 0;


   // private GameObject prey;


    public bool notHit = true;
    public bool notCatch = true;
   // private bool predictChase = false;

    //public Transform preyStartPoint;
    //public Transform preyEndPoint;
    //public float preyMoveTime;
	// Use this for initialization
	void Start () {

        ainav = gameObject.GetComponent<AINavSteeringController>();
        anim = gameObject.GetComponent<Animator>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        Patrol();
       // Chase();
        //PrepareThrow();
		
	}
	
	// Update is called once per frame
	void Update () {
        switch (state)
        {
            case AIState.Patrol:
                if (FoundPrey())
                    Chase();
                break;
            case AIState.Chase:
                //ainav.mecanimInputForwardSpeedCap = 1.0f;
                ainav.setWaypoint(prey.transform.position);
                break;
            default:
                break;

        }
        //Debug.Log(prey.GetComponent<NavMeshAgent>().velocity);
        /*if (ainav.waypointsComplete() && state != AIState.Chase)
        {
            ChangeState();
        }*/

      //  EventCheck();


	}
    void OnCollisionEnter(Collision collision)
    {
        /*if (collision.transform.tag == "Player")
        {
            collision.transform.GetComponent<PlayerHealthPoint>().Hurt();
            Destroy(gameObject);
            Debug.Log("Catch Player");
        }*/
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
           // collider.
            collision.transform.GetComponent<PlayerHealthPoint>().Hurt();

            Instantiate(Explosion, (collision.transform.position + transform.position ) / 2.0f, transform.rotation);
            EventManager.TriggerEvent<Explosion1, Vector3>(transform.position);

            Destroy(gameObject);

            Debug.Log("Catch Player");

        }
        else if (collision.tag == "Robot")
        {
            RobotType touchedType = collision.GetComponent<AIController>().robottype;
            if (touchedType == robottype)
            {

                Instantiate(Explosion_killed, (collision.transform.position + transform.position) / 2.0f, transform.rotation);
                EventManager.TriggerEvent<RobotExplosion, Vector3>((collision.transform.position + transform.position) / 2.0f);

                Destroy(collision.gameObject);
                Destroy(gameObject);
            }

        }
    }

    bool FoundPrey()
    {
        Vector3 dir = prey.transform.position - transform.position;
        float angle = Vector3.Angle(dir, transform.forward);
        //Debug.Log(angle);
        if (Mathf.Abs(angle) < 30 && dir.magnitude < 10)
            return true;
        else
            return false;
 
    }
    void EventCheck()
    {
       /* throwTimeRecord += Time.deltaTime;

        if (anim.GetFloat("ThrowCurve") > 4.5f && throwTimeRecord > throwFilter && anim.GetBool("Throw") == true)
        {
            //float predictTime = 1.0f;
            //Vector3 predictPos = prey.transform.position + prey.GetComponent<NavMeshAgent>().velocity * predictTime;

            //without gravity
            Vector3 dir = (predictPos - lefthand.transform.position).normalized;
            float distance = (predictPos - lefthand.transform.position).magnitude;
            float speed = distance / predictTime;

            EventManager.TriggerEvent<ThrowEvent,Vector3>(dir * speed);
            throwTimeRecord = 0;

            //gravity
            Vector3 planexz = predictPos - lefthand.transform.position;
            Vector3 perpendicularDistance = new Vector3(0, planexz.y, 0);
            planexz.y = 0;
            float speed = planexz.magnitude / predictTime;

            Vector3 dir = (predictPos - lefthand.transform.position).normalized;
            Vector3 xzvelocity = speed * new Vector3(dir.x, 0, dir.z);

            Vector3 yvelocity = perpendicularDistance / predictTime - new Vector3(0,-4.9f,0) * predictTime;

            EventManager.TriggerEvent<ThrowEvent, Vector3>(xzvelocity + yvelocity);
            throwTimeRecord = 0;

           // 
            

           

            


            //isjumping = true;

        }*/
    }

    void Patrol()
    {
        ainav.setWaypoints(WayPointsA);
        state = AIState.Patrol;
        //AIstateText.text = "AIState: Fixed Points A";
       // choicerecord = 0;
 
    }
   
    void Chase()
    {
        state = AIState.Chase;
        ainav.mecanimInputForwardSpeedCap = 0.96f;
        ainav.setWaypoint(prey.transform.position);
        Debug.Log("Start Chase");
       
    }

    float CalculateDistance()
    {
        float distance = 0;
        var path = agent.path;
        if (path.status != NavMeshPathStatus.PathInvalid && path.corners.Length > 1)
        {
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                distance += Vector3.Distance(path.corners[i + 1], path.corners[i]);
 
            }
 
        }
        return distance;
    }

    /*void OnCollisionEnter(Collision collision)
    {
        if (collision.tag == "Player")
        {
 
        }
    }*/


    void ChangeState(){
       
           /* int choice = Random.Range(0, 4);
            if (choice == choicerecord)
                choice = (choice + 1) % 1;*/


            int choice = 0;
            switch (choice)
            {
                case 0:
                    Patrol();
                    break;
                case 1:
                    Chase();
                    break;
                default:
                    break;
            }
    }
}
