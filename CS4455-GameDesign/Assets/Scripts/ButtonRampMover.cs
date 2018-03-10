using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

//require some things the bot control needs
public class ButtonRampMover : MonoBehaviour
{
    private GameObject reflectorOne;

    private bool mirrorOneMoving = false;
    private float mirrorStartTime = 0f;
    private Vector3 mirrorStart;
    private Vector3 mirrorEnd;

    private float buttonStartTime = 0f;
    private bool buttonReturning = false;
    private Vector3 buttonStart;
    private Vector3 buttonEnd;

    private Vector3 MIRROR_POS_ONE;
    private Vector3 MIRROR_POS_TWO;

    private Vector3 BUTTON_START_POS;
    private Vector3 BUTTON_END_POS;

    private float BASE_MOVE_TIME = .25f;
    private float remainingMoveTime = .25f;
    private float moveTimePassed = 0f;

    void Awake() {

    }


    // Use this for initialization
    void Start()
    {
        mirrorStart = GameObject.Find("endpiece1").transform.position;
        mirrorEnd = GameObject.Find("endpiece2").transform.position;
        MIRROR_POS_ONE = GameObject.Find("endpiece1").transform.position;
        MIRROR_POS_TWO = GameObject.Find("endpiece2").transform.position;

        BUTTON_START_POS = GameObject.Find("button_startstop").transform.position;
        BUTTON_END_POS = BUTTON_START_POS;
        BUTTON_END_POS.y -= .1f;

        reflectorOne = GameObject.Find("reflector1");
    }


    void Update()
    {
    }

    //Update whenever physics updates with FixedUpdate()
    //Updating the animator here should coincide with "Animate Physics"
    //setting in Animator component under the Inspector
    void FixedUpdate()
    {
        if (mirrorOneMoving) {
            
            if (reflectorOne.transform.position == mirrorEnd) {
                Vector3 temp = mirrorStart;
                mirrorStart = mirrorEnd;
                mirrorEnd = temp;
                mirrorStartTime = Time.time;

                if (mirrorStart == MIRROR_POS_ONE)
                {
                    mirrorEnd = MIRROR_POS_TWO;
                }
                else
                {
                    mirrorEnd = MIRROR_POS_ONE;
                }
                remainingMoveTime = BASE_MOVE_TIME;
                moveTimePassed = 0f;

                Debug.Log(mirrorStart + " -- " + mirrorEnd);

            }

            GameObject.Find("button_startstop").transform.position = Vector3.Lerp(BUTTON_START_POS, BUTTON_END_POS, ((Time.time - buttonStartTime) * 15f) / 5.5f);
            reflectorOne.transform.position = Vector3.Lerp(mirrorStart, mirrorEnd, (Time.time - mirrorStartTime) * remainingMoveTime);
            moveTimePassed += Time.deltaTime;
        }

        if (buttonReturning) {
            GameObject.Find("button_startstop").transform.position = Vector3.Lerp(BUTTON_END_POS, BUTTON_START_POS, ((Time.time - buttonStartTime) * 15f) / 5.5f);
        }
        if (GameObject.Find("button_startstop").transform.position == BUTTON_START_POS) {
            buttonReturning = false;
        }

    }

    //This is a physics callback
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.name == "YBot" || collision.transform.gameObject.name.Contains("box_piece")) {
            mirrorOneMoving = true;
            buttonReturning = false;
            mirrorStartTime = Time.time;
            buttonStartTime = Time.time;
            mirrorStart = reflectorOne.transform.position;

            remainingMoveTime = BASE_MOVE_TIME + moveTimePassed / 6f;
        }
    }

    //This is a physics callback
    void OnCollisionExit(Collision collision)
    {
        if (collision.transform.gameObject.name== "YBot" || collision.transform.gameObject.name.Contains("box_piece")) {
            mirrorOneMoving = false;
            buttonReturning = true;
            mirrorStartTime = Time.time;
            buttonStartTime = Time.time;
        }


    }
}
