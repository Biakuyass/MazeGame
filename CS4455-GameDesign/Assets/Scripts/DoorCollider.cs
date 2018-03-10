using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

//require some things the bot control needs
[RequireComponent(typeof(BoxCollider))]
public class DoorCollider : MonoBehaviour
{

    public static bool canMove = true;

    void Awake()
    {

    }


    // Use this for initialization
    void Start()
    {
       
    }
    

    //Update whenever physics updates with FixedUpdate()
    //Updating the animator here should coincide with "Animate Physics"
    //setting in Animator component under the Inspector
    void FixedUpdate()
    {
        
    }

    //This is a physics callback
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.name.Contains("blockade")) {
            canMove = false;
        }
    }

    //This is a physics callback
    void OnCollisionExit(Collision collision)
    {
        if (collision.transform.gameObject.name.Contains("blockade"))
        {
            canMove = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       
    }

}
