using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionEventManager : MonoBehaviour {

    public GameObject player;
   // public Transform handPosition;
    Rigidbody rig;

    public Transform lefthand;
    public Transform ballposition;
    public GameObject Ball;

    //avoid creating two balls
    bool ishold = false;

    private UnityAction<Vector3> jumpEventListener;
    private UnityAction throwEventListener;
    private UnityAction createBallEventListener;
    private UnityAction<GrabableType> pickUpEventListener;

    //private GrabableType grabbedType;

   // public GameObject grabbedObject;

    public static GameObject tempball;
    void Awake()
    {
        jumpEventListener = new UnityAction<Vector3>(jumpEventHandler);
        throwEventListener = new UnityAction(throwEventHandler);
        createBallEventListener = new UnityAction(createBallEventHandler);

        pickUpEventListener = new UnityAction<GrabableType>(PickupEventHandler);

        rig = player.GetComponent<Rigidbody>();
        
    }
	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {

        //Debug.DrawRay(Ball.transform.position, handPosition.forward);
        //  Debug.DrawRay(tempball.transform.position, tempball.GetComponent<Rigidbody>().velocity);

        
	}

    void OnEnable()
    {

        EventManager.StartListening<JumpEvent, Vector3>(jumpEventListener);
       // Debug.Log("1");
        EventManager.StartListening<ThrowEvent>(throwEventListener);
        EventManager.StartListening<CreateBallEvent>(createBallEventListener);

        EventManager.StartListening<PickUpEvent,GrabableType>(pickUpEventListener);
        

    }

    void OnDisable()
    {

        EventManager.StopListening<JumpEvent, Vector3>(jumpEventListener);
        EventManager.StopListening<ThrowEvent>(throwEventListener);
        EventManager.StopListening<CreateBallEvent>(createBallEventListener);

        EventManager.StopListening<PickUpEvent, GrabableType>(pickUpEventListener);

    }

    void jumpEventHandler(Vector3 impulse)
    {
       // Debug.Log("jump");
        rig.AddForce(impulse, ForceMode.Impulse);
        
 
    }
    void createBallEventHandler()
    {
        if (!ishold)
        {
            tempball = Instantiate(Ball, ballposition.transform.position, ballposition.transform.rotation, lefthand) as GameObject;
            // tempball.transform.position = Ball.transform.position;
            Debug.Log("CreateBall");
            ishold = true;
 
        }
        

    }

    void PickUpBall()
    {
        /*if (!ishold)
        {
            tempball = Instantiate(Ball, ballposition.transform.position, ballposition.transform.rotation, lefthand) as GameObject;
            Debug.Log("CreateBall");
            ishold = true;

        }*/
    }
    void PickupEventHandler(GrabableType type_)
    {

        //Debug.Log("Event: " + type_);
        //PickUpBall();
        if (!ishold)
        {
           // player.GetComponent<YBotSimpleControlScript>().GrabObject();
           // player.GetComponent<YBotSimpleControlScript>().ChangeGrabState(true);
           // ishold = true;
 
        }
        
 
    }
    void throwEventHandler()
    {

       
        //
       // Ball.transform.parent = 
        if(ishold)
        {
            Vector3 impulse = player.transform.forward * 70;
            tempball.transform.parent = null;
            tempball.GetComponent<Rigidbody>().isKinematic = false;
            tempball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            tempball.GetComponent<Rigidbody>().AddForce(impulse, ForceMode.Impulse);
            //StartCoroutine(ChangeLayer());
            
            ishold = false;
          //  player.GetComponent<YBotSimpleControlScript>().ChangeGrabState(false);

           
            Debug.DrawRay(tempball.transform.position, impulse);
           // Debug.Log("throw");

        }


    }
    IEnumerator ChangeLayer()
    {
        yield return new WaitForSeconds(3);
        tempball.layer = LayerMask.NameToLayer("Default");
    }
}
