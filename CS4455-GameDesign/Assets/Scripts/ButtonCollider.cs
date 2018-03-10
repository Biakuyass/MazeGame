using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

//require some things the bot control needs
[RequireComponent(typeof(BoxCollider))]
public class ButtonCollider : MonoBehaviour
{

    private float button2StartTime = 0f;
    private bool button2Pressed = false;
    private bool button2Returning = false;
    private Vector3 button2Start;
    private Vector3 button2End;

    private Quaternion DOOR_POS_OPEN;
    private Quaternion DOOR_POS_CLOSED;

    private Vector3 BUTTON2_START_POS;
    private Vector3 BUTTON2_END_POS;

    void Awake()
    {

    }


    // Use this for initialization
    void Start()
    {
        DOOR_POS_CLOSED = GameObject.Find("door1").transform.rotation;
        DOOR_POS_OPEN = new Quaternion(0, 1, 0, 0);

        BUTTON2_START_POS = GameObject.Find("button_openclose").transform.position;
        BUTTON2_END_POS = BUTTON2_START_POS;
        BUTTON2_END_POS.y -= .05f;
    }
    

    //Update whenever physics updates with FixedUpdate()
    //Updating the animator here should coincide with "Animate Physics"
    //setting in Animator component under the Inspector
    void FixedUpdate()
    {
        if (DoorCollider.canMove)
        {
            if (button2Pressed)
            {
                GameObject.Find("button_openclose").transform.position = Vector3.Lerp(BUTTON2_START_POS, BUTTON2_END_POS, ((Time.time - button2StartTime) * 15f) / 5.5f);
                GameObject.Find("door1").transform.rotation = Quaternion.Lerp(DOOR_POS_CLOSED, DOOR_POS_OPEN, ((Time.time - button2StartTime) * 15f) / 5.5f);
            }
            if (button2Returning)
            {
                GameObject.Find("button_openclose").transform.position = Vector3.Lerp(BUTTON2_END_POS, BUTTON2_START_POS, ((Time.time - button2StartTime) * 15f) / 5.5f);
                GameObject.Find("door1").transform.rotation = Quaternion.Lerp(DOOR_POS_OPEN, DOOR_POS_CLOSED, ((Time.time - button2StartTime) * 15f) / 5.5f);
            }
            if (GameObject.Find("button_openclose").transform.position == BUTTON2_START_POS)
            {
                button2Returning = false;
            }
        } else {
            Debug.Log("door jammed");
        }
    }

    //This is a physics callback
    void OnCollisionEnter(Collision collision)
    {
        if (button2Pressed) {
            return;
        }

        button2Pressed = true;
        button2Returning = false;
        button2StartTime = Time.time;
    }

    //This is a physics callback
    void OnCollisionExit(Collision collision)
    {
        if (button2Returning) {
            return;
        }

        button2Pressed = false;
        button2Returning = true;
        button2StartTime = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
       
    }

}
