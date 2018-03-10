//#define USE_CHARACTER_CONTROLLER

using UnityEngine;
using System.Collections;
using UnityEngine.AI;

// Require these components when using this script
[RequireComponent (typeof(Animator))]
#if USE_CHARACTER_CONTROLLER
[RequireComponent (typeof(CharacterController))]
#else
[RequireComponent (typeof(CapsuleCollider))]
#endif
[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(NavMeshAgent))]
public class ChaseController : MonoBehaviour
{

	//Component Refs
	public GameObject chase;
	private bool toChase = true;
	private NavMeshAgent agent;

	void Start () {
		agent = GetComponent<NavMeshAgent>();

		// Disabling auto-braking allows for continuous movement
		// between points (ie, the agent doesn't slow down as it
		// approaches a destination point).
		agent.autoBraking = false;

	}

	void Update () {
		// move towards
		if (toChase)
			agent.destination = chase.transform.position;
	}
}

