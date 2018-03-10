using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rbody;

    private Transform leftFoot;
    private Transform rightFoot;


    public int groundContacts = 0;

    public float groundRayOffset = 1;

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


    }




    //Update whenever physics updates with FixedUpdate()
    //Updating the animator here should coincide with "Animate Physics"
    //setting in Animator component under the Inspector
    void Update()
    {

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

        if (Mathf.Abs(anim.GetFloat("Footland")) > 8.5f && footstepTimeRecord > footstepFilter)
        {
            //   Debug.Log(footstepTimeRecord);
            if (anim.GetFloat("Footland") < 0)
                leftorRight = 0;
            else
                leftorRight = 1;

            EventManager.TriggerEvent<FootStepEvent, Vector3, int, int>(transform.position, footstepChoice, leftorRight);
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

        if (anim.GetFloat("JumpCurve") < -4.1f)
        {
            isjumping = false;
            Debug.Log("No jumping");
        }

        if (anim.GetFloat("JumpCurve") > 4.5f && jumpTimeRecord > jumpFilter)
        {
            EventManager.TriggerEvent<JumpEvent, Vector3>(transform.forward * forwardSpeedLimit * 200 + new Vector3(0, 450, 0));
            jumpTimeRecord = 0;
            isjumping = true;

        }

        if (anim.GetFloat("ThrowCurve") > 4.5f && throwTimeRecord > throwFilter)
        {
            EventManager.TriggerEvent<ThrowEvent>();
            throwTimeRecord = 0;
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

            if (Physics.Raycast(transform.position + transform.up * groundRayOffset, groundedVector, out hit, 2.0f, hitmask))
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

        anim.SetBool("Throw", false);

        //ori
        /*if (Input.GetButtonDown("Fire1") && grabableState) //normally left-ctrl on keyboard
        {
            anim.SetBool("Throw", true);
            EventManager.TriggerEvent<CreateBallEvent>();
        }*/

        if (Input.GetButtonDown("Fire1")) //normally left-ctrl on keyboard
        {
            anim.SetBool("Throw", true);
            EventManager.TriggerEvent<CreateBallEvent>();
        }


        anim.SetBool("Jump", false);
        if (Input.GetKeyDown(KeyCode.T) && IsGrounded)
            anim.SetBool("Jump", true);

        anim.SetBool("Pickup", false);



        if (Input.GetKeyDown(KeyCode.F))
            StartCoroutine(Death());

        // Debug.Log(anim.GetCurrentAnimatorClipInfo(0).);


    }


    //This is a physics callback
    void OnCollisionEnter(Collision collision)
    {

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

}

