﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum Inventory
{
    ball,
    mR,
    mG,
    mB
}

//require some things the bot control needs
[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
public class YBotSimpleControlScript : MonoBehaviour
{
    

    public Canvas ui;
    public Text generalText;
    public Text inventoryText;
    public int score = 0;
    public int health = 10;
    public int healthCap = 10;
    //public bool hasMarkerR = false;
    //public bool hasMarkerG = false;
    //public bool hasMarkerB = true;
    public List<Inventory> inventory;
    public int inventoryIdx = 0;
    private float inputDelay = 0.025f;
    private float lastInput = 0f;
    private float enemyTouchDelay = 2f;
    private float lastEnemyTouch = 0f;

    private Animator anim;	
    private Rigidbody rbody;

    private Transform leftFoot;
    private Transform rightFoot;

	private int ballCount = 0;

    public int groundContacts = 0;

    public float groundRayOffset = 1;

	public GameObject ball; 
	public GameObject wall; 

    private float filteredForwardInput = 0f;
    private float filteredTurnInput = 0f;

    public float forwardInputFilter = 5f;
    public float turnInputFilter = 5f;
    private float forwardSpeedLimit = 1f;

    private float footstepFilter = 0.15f;
    private float footstepFilterWalk = 0.3f;
    private float footstepTimeRecord = 0;

    private float jumpFilter = 0.5f;
    private float jumpTimeRecord = 0;

    private float throwFilter = 0.5f;
    private float throwTimeRecord = 0;

    private bool isjumping = false;
    private bool startingDrag = false;


    //private bool isDragging = false;
    private DraggingObject currentHeldObject;
    private HashSet<GameObject> possibleObjectsToDrag;
    HashSet<Collider> ignoreColliders;

    public Terrain terrain;


    public bool IsGrounded
    {
        get { return groundContacts > 0; }
    }
   // bool isFalling = false;


    void Awake()
    {

        anim = GetComponent<Animator>();

        if (anim == null)
            Debug.Log("Animator could not be found");

        rbody = GetComponent<Rigidbody>();

        if (rbody == null)
            Debug.Log("Rigid body could not be found");

    }


    // Use this for initialization
    void Start()
    {
		//example of how to get access to certain limbs
        leftFoot = this.transform.Find("mixamorig:Hips/mixamorig:LeftUpLeg/mixamorig:LeftLeg/mixamorig:LeftFoot");
        rightFoot = this.transform.Find("mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg/mixamorig:RightFoot");

        if (leftFoot == null || rightFoot == null)
            Debug.Log("One of the feet could not be found");
        if (ui != null) {
            if (SceneManager.GetActiveScene().name != "Maze")
            {
                ui.enabled = false;
            }
            else
            {
                ui.enabled = true;
            }
        }

        ignoreColliders = new HashSet<Collider>();
        foreach (Collider mybody in GetComponentsInChildren<Collider>())
        {
            ignoreColliders.Add(mybody);
        }

        possibleObjectsToDrag = new HashSet<GameObject>();
        currentHeldObject = new DraggingObject(null, false, -1f);
    }

    //Update whenever physics updates with FixedUpdate()
    //Updating the animator here should coincide with "Animate Physics"
    //setting in Animator component under the Inspector
    void Update()
    {
        if ((Time.timeSinceLevelLoad - lastInput) > inputDelay) {
            lastInput = Time.timeSinceLevelLoad;
            if (Input.GetKeyUp(KeyCode.Space)) {
                terrain.GetComponent<Maze>().BreakGrid();
            }
            else if (Input.GetKeyUp(KeyCode.P)) {
                print("Pickup?");
                terrain.GetComponent<Maze>().Pickup();
            }
            else if (Input.GetKeyUp("joystick 1 button 1")) {
                terrain.GetComponent<Maze>().BreakGrid();
            }
            else if (Input.GetKeyUp("joystick 1 button 3")) {
                terrain.GetComponent<Maze>().Pickup();
            }
            else if (Input.GetKeyUp("joystick 1 button 4")) {
                print("button4");
                if (inventoryIdx <= 0) {
                    inventoryIdx = inventory.Count - 1;
                }
                else {
                    inventoryIdx--;
                }
                print("inv idx" + inventoryIdx.ToString());
            }
            else if (Input.GetKeyUp("joystick 1 button 5")) {
                print("button5");
                if (inventoryIdx >= inventory.Count - 1) {
                    inventoryIdx = 0;
                }
                else {
                    inventoryIdx++;
                }
                //print("inv idx" + inventoryIdx.ToString());
            }
            else if (Input.GetKeyUp("joystick 1 button 6")) {
                ui.enabled = !ui.enabled;
            }
            else if (Input.GetKeyUp("joystick 1 button 0")) {
                if (inventory[inventoryIdx] == Inventory.ball) {
                    print("plzthrow");
                    anim.SetBool("Throw", true);
                    EventManager.TriggerEvent<CreateBallEvent>();
                }
                else if (inventory[inventoryIdx] == Inventory.mR
                      || inventory[inventoryIdx] == Inventory.mG
                      || inventory[inventoryIdx] == Inventory.mB) {
                    terrain.GetComponent<Maze>().Place(inventory[inventoryIdx]);
                }
            }

            for (int i = 0; i < 20; i++)
            {
                if (Input.GetKeyDown("joystick 1 button " + i))
                {
                    print("joystick 1 button " + i);
                }
            }
        }

        footstepTimeRecord += Time.deltaTime;
        jumpTimeRecord += Time.deltaTime;
        throwTimeRecord += Time.deltaTime;

        int leftorRight = 0;
        int footstepChoice = 0;
        Vector3 groundedVector = Vector3.down;
        RaycastHit hit;
        int hitmask = LayerMask.GetMask("ground");


        /*if (Physics.Raycast(transform.position + transform.up * groundRayOffset, groundedVector, out hit, 10, hitmask))
        {
            if (hit.transform.name == "GrassTerrain")
                footstepChoice = 0;
            else if(hit.transform.name == "SnowTerrain")
                footstepChoice = 1;
        }*/

        if (Mathf.Abs(anim.GetFloat("Footland")) > 8.5f && footstepTimeRecord > footstepFilter ) {
            leftorRight = anim.GetFloat("Footland") < 0 ? 0 : 1;

            Material mat = null;
            if (Physics.Raycast(transform.position + transform.up * groundRayOffset, groundedVector, out hit, 2.0f, hitmask))
            {
                mat = hit.transform.GetComponent<MeshRenderer>().material;
            }


            EventManager.TriggerEvent<PlayerFootEvent, Vector3, Material>(transform.position, mat);
            footstepTimeRecord = 0;

        }
        
        /*else if (Mathf.Abs(anim.GetFloat("Footland")) > 6.0f && footstepTimeRecord > footstepFilterWalk && forwardSpeedLimit < 0.6f)
            {
                if(anim.GetFloat("Footland") < 0)
                    leftorRight = 0;
                else
                    leftorRight = 1;
                EventManager.TriggerEvent<FootStepEvent, Vector3, int,int>(transform.position, footstepChoice,leftorRight);
                footstepTimeRecord = 0;
 
            }*/

         if (anim.GetFloat("JumpCurve") < -4.1f) {
                    isjumping = false;
                    Debug.Log("No jumping");
         }

         if (anim.GetFloat("JumpCurve") > 4.5f && jumpTimeRecord > jumpFilter) {
                EventManager.TriggerEvent<JumpEvent, Vector3>(transform.forward * forwardSpeedLimit * 200 + new Vector3(0, 450, 0));
                jumpTimeRecord = 0;
                isjumping = true;
               
         }

         if (anim.GetFloat("ThrowCurve") > 4.5f && throwTimeRecord > throwFilter ) {
                EventManager.TriggerEvent<ThrowEvent>();
                throwTimeRecord = 0;
                anim.SetBool("Throw", false);
            //isjumping = true;
        }

    }

    void FixedUpdate()
    {

        //GetAxisRaw() so we can do filtering here instead of the InputManager
        float h = Input.GetAxisRaw("Horizontal");// setup h variable as our horizontal input axis
        float v = Input.GetAxisRaw("Vertical");	// setup v variables as our vertical input axis


        //enforce circular joystick mapping which should coincide with circular blendtree positions
        Vector2 vec = Vector2.ClampMagnitude(new Vector2(h, v), 1.0f);

        h = vec.x;
        v = vec.y;


        //BEGIN ANALOG ON KEYBOARD DEMO CODE
        if (Input.GetKey(KeyCode.Q))
            h = -0.5f;
        else if (Input.GetKey(KeyCode.E))
            h = 0.5f;

        if (Input.GetKeyUp(KeyCode.Alpha1))
            forwardSpeedLimit = 0.5f;
        else if (Input.GetKeyUp(KeyCode.Alpha2))
            forwardSpeedLimit = 0.5f;
        else if (Input.GetKeyUp(KeyCode.Alpha3))
            forwardSpeedLimit = 0.5f;
        else if (Input.GetKeyUp(KeyCode.Alpha4))
            forwardSpeedLimit = 0.5f;
        else if (Input.GetKeyUp(KeyCode.Alpha5))
            forwardSpeedLimit = 0.5f;
        else if (Input.GetKeyUp(KeyCode.Alpha6))
            forwardSpeedLimit = 0.9f;
        else if (Input.GetKeyUp(KeyCode.Alpha7))
            forwardSpeedLimit = 0.9f;
        else if (Input.GetKeyUp(KeyCode.Alpha8))
            forwardSpeedLimit = 0.9f;
        else if (Input.GetKeyUp(KeyCode.Alpha9))
            forwardSpeedLimit = 0.9f;
        else if (Input.GetKeyUp(KeyCode.Alpha0))
        {
            forwardSpeedLimit = 1.0f;
        }
        //END ANALOG ON KEYBOARD DEMO CODE  

        //do some filtering of our input as well as clamp to a speed limit
        filteredForwardInput = Mathf.Clamp(Mathf.Lerp(filteredForwardInput, v, 
                Time.deltaTime * forwardInputFilter), -forwardSpeedLimit, forwardSpeedLimit);
        
        filteredTurnInput = Mathf.Lerp(filteredTurnInput, h, 
            Time.deltaTime * turnInputFilter);
                                                    
        //finally pass the processed input values to the animator
        anim.SetFloat("velx", filteredTurnInput);	// set our animator's float parameter 'Speed' equal to the vertical input axis				
        anim.SetFloat("vely", filteredForwardInput); // set our animator's float parameter 'Direction' equal to the horizontal input axis		


        if (!IsGrounded && !isjumping)
        {

            Vector3 groundedVector = Vector3.down;

            Debug.DrawLine(transform.position + transform.up * groundRayOffset, transform.position + transform.up * groundRayOffset + groundedVector * 2.0f);

            RaycastHit hit;

            int hitmask = LayerMask.GetMask("ground");

            if (Physics.Raycast(transform.position + transform.up * groundRayOffset, groundedVector, out hit, 2.0f,hitmask))
            {
                if (hit.transform.tag == "ground")
                    anim.SetBool("isFalling", false);
            }
            else
            {
                anim.SetBool("isFalling", true);
            }
 
        }

        //Debug.Log(gameObject.GetComponent<Rigidbody>().velocity);

        //anim.SetBool("Throw", false);

        //ori
        /*if (Input.GetButtonDown("Fire1") && grabableState) //normally left-ctrl on keyboard
        {
            anim.SetBool("Throw", true);
            EventManager.TriggerEvent<CreateBallEvent>();
        }*/

        /*if (Input.GetButtonDown("Fire1")) //normally left-ctrl on keyboard
        {
            anim.SetBool("Throw", true);
            //EventManager.TriggerEvent<CreateBallEvent>();
        }*/
            

        anim.SetBool("Jump",false);
        if (Input.GetKeyDown(KeyCode.T) && IsGrounded)
            anim.SetBool("Jump",true);

        anim.SetBool("Pickup", false);

            

        if (Input.GetKeyDown(KeyCode.F))
            StartCoroutine(Death());

       // Debug.Log(anim.GetCurrentAnimatorClipInfo(0).);


		if (ballCount >= 3) {
			
			ball.GetComponent<MeshRenderer> ().enabled = false; 
			wall.GetComponent<MeshRenderer> ().enabled = false; 
			wall.GetComponent<BoxCollider> ().enabled = false;
		}


        if (Input.GetKeyDown(KeyCode.J) && !startingDrag)
        {
            startingDrag = true;
            if (!currentHeldObject.isDragging())
            {
                BeginDrag();
            }
            else
            {
                EndDrag();
            }
            startingDrag = false;
        }
    }

    void doDrag(RaycastHit hit) {
        if (hit.transform.gameObject.name.Contains("box_piece"))
        {
            currentHeldObject.setDragging(true);
            currentHeldObject.setGameObject(hit.transform.gameObject);
            currentHeldObject.setDistanceFromBody((this.transform.position-hit.transform.position).magnitude);
        }
        else
        {
            EndDrag();
        }
    }

    void BeginDrag() {
        Debug.Log("--- Begin dragging ---");

        RaycastHit hit = raycastWithHeight(.25f);
        if (hit.transform) // if grab at lower waist
        {
            doDrag(hit);
        }
        else
        {
            hit = raycastWithHeight(.5f);
            if (hit.transform) // if grab at waist
            {
                doDrag(hit);
            }
            else // no grab
            {
                EndDrag();
            }
        }
    }

    private RaycastHit raycastWithHeight(float h) {
        float rayOriginOffset = 0f;
        float rayDepth = 1f; //how far will we look?
        float totalRayLen = rayOriginOffset + rayDepth;

        Vector3 startPoint = new Vector3(this.transform.position.x, this.transform.position.y + h, this.transform.position.z);
        RaycastHit hit;
        Ray ray = new Ray(startPoint, this.transform.forward);

        //Debug.DrawRay(ray.origin, ray.direction * totalRayLen, Color.white, 5f);
        //Debug.Log(this.transform.forward);

        if (Physics.Raycast(ray, out hit, totalRayLen)) {
            return hit;
        } else {
            return hit;
        }
    }

    public void EndDrag() {
        Debug.Log("--- End dragging ---");

        currentHeldObject.setDragging(false);
        currentHeldObject.setGameObject(null);
    }

    void BeginDragOld()
    {
        Debug.Log("--- Begin dragging ---");

        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);
        if (colliders.Length == 0)
        {
            EndDrag();
            return;
        }

        Collider closestCollider = null;

        foreach (Collider hit in colliders)
        {
            if (!hit.gameObject.name.Contains("box_piece"))
            {
                continue;
            }
            if (closestCollider == null)
            {
                closestCollider = hit;
                continue;
            }
            if (Vector3.Distance(hit.transform.position, transform.position) < Vector3.Distance(closestCollider.transform.position, transform.position))
            {
                closestCollider = hit;
            }
        }

        if (closestCollider == null)
        {
            EndDrag();
            return;
        }

        GameObject objToDrag = closestCollider.gameObject;

        Debug.Log("Dragging " + closestCollider.gameObject.name);

        currentHeldObject.setDragging(true);
        currentHeldObject.setGameObject(objToDrag);
    }

    //This is a physics callback
    void OnCollisionEnter(Collision collision)
    {
        //print(collision.transform.gameObject.tag);
        if (collision.transform.gameObject.tag == "ground")
        {
            ++groundContacts;

            //Debug.Log("Player hit the ground at: " + collision.impulse.magnitude);

            //Debug.Log(collision.impulse.magnitude);
            if (collision.impulse.magnitude > 300f)
            {
                EventManager.TriggerEvent<PlayerLandsEvent, Vector3>(collision.contacts[0].point);
            }
        }	
        if (collision.transform.gameObject.tag == "special") {
            SceneManager.LoadScene("Maze");
        }
        if (collision.transform.gameObject.tag == "enemy")
        {
            if ((Time.timeSinceLevelLoad - lastEnemyTouch) > enemyTouchDelay)
            {
                lastEnemyTouch = Time.timeSinceLevelLoad;
                health -= 1;
                if (health <= 0)
                {
                    Death();
                }
            }
        }
    }
    
    void OnTriggerEnter(Collider collision)
    {

            /*if (collision.transform.tag == "Ball")
            {
                grabableObject = collision.gameObject;
                if (!grabableState)
                pickupText.color = new Color(pickupText.color.r, pickupText.color.g, pickupText.color.b, 255); 
                grabableType = GrabableType.Ball;
  
            }
            else if (collision.transform.tag == "smallbox")
            {
                grabableObject = collision.gameObject;
                if(!grabableState)
                pickupText.color = new Color(pickupText.color.r, pickupText.color.g, pickupText.color.b, 255); 
                grabableType = GrabableType.smallBox;
 
            }
            else if (collision.transform.tag == "smalllog")
            {
                grabableObject = collision.gameObject;
                if (!grabableState)
                pickupText.color = new Color(pickupText.color.r, pickupText.color.g, pickupText.color.b, 255); 
                grabableType = GrabableType.smalllog;

            }*/

           // Debug.Log("Grab");
        

    }
    void OnTriggerExit(Collider collision)
    {


    }

    //This is a physics callback
    void OnCollisionExit(Collision collision)
    {

        if (collision.transform.gameObject.tag == "ground")
            --groundContacts;
    }

    void OnAnimatorMove()
    {
        // Don't use root motion when jumping
        if (!isjumping)
        {
            Vector3 previousPosition = transform.position;
            if (IsGrounded)
            {
                //use root motion as is if on the ground		
                this.transform.position = anim.rootPosition;

            }
            else
            {
                //Simple trick to keep model from climbing other rigidbodies that aren't the ground
                this.transform.position = new Vector3(anim.rootPosition.x, this.transform.position.y, anim.rootPosition.z);
            }

            //use rotational root motion as is
            this.transform.rotation = anim.rootRotation;

            if (currentHeldObject.isDragging()) {
                //Vector3 delta = transform.position - previousPosition;
                //currentHeldObject.getGameObject().transform.position += delta;

                Debug.Log(currentHeldObject.getDistanceFromBody());

                Vector3 boxPlacement = new Vector3(transform.position.x, currentHeldObject.getGameObject().transform.position.y, transform.position.z) + transform.forward * currentHeldObject.getDistanceFromBody();
                currentHeldObject.getGameObject().transform.position = boxPlacement;
            }

        }
        
        				
    }

    public void PlayerDeath()
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        
        anim.enabled = false;

        yield return new WaitForSeconds(3);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public DraggingObject getHeldObject() {
        return this.currentHeldObject;
    }

    public class DraggingObject {

        GameObject gameObject;
        bool active;
        float distanceFromBody;

        public DraggingObject(GameObject obj, bool active, float dist) {
            this.gameObject = obj;
            this.active = active;
            this.distanceFromBody = dist;
        }

        public void setDragging(bool drag) {
            this.active = drag;
        }

        public bool isDragging() {
            return this.active;
        }

        public void setGameObject(GameObject obj) {
            this.gameObject = obj;
        }

        public GameObject getGameObject() {
            return this.gameObject;
        }

        public void setDistanceFromBody(float d) {
            this.distanceFromBody = d;
        }

        public float getDistanceFromBody() {
            return this.distanceFromBody;
        }

    }
}
