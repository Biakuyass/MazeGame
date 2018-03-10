//#define USE_CHARACTER_CONTROLLER


using UnityEngine;
using System.Collections;
using UnityEngine.AI;


/// <summary>
/// AI script that integrates NavMeshAgent with a Mecanim Animator with two params (forward+turn left/right).
/// Author: Jeff Wilson (jeff@imtc.gatech.edu)
/// Interactive Media Technology Center
/// Georgia Insitute of Technology
/// </summary>


// Require these components when using this script
[RequireComponent (typeof(Animator))]
#if USE_CHARACTER_CONTROLLER
[RequireComponent (typeof(CharacterController))]
#else
[RequireComponent (typeof(CapsuleCollider))]
#endif
[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(NavMeshAgent))]
public class AIAgentController : MonoBehaviour
{

	//Component Refs
	public Transform[] points;
	private int destPoint = 0;
	private NavMeshAgent agent;

	void Start () {
		agent = GetComponent<NavMeshAgent>();

		// Disabling auto-braking allows for continuous movement
		// between points (ie, the agent doesn't slow down as it
		// approaches a destination point).
		agent.autoBraking = false;

		GotoNextPoint();
	}


	void GotoNextPoint() {
		// Returns if no points have been set up
		if (points.Length == 0)
			return;

		// Set the agent to go to the currently selected destination.
		agent.destination = points[destPoint].position;

		// Choose the next point in the array as the destination,
		// cycling to the start if necessary.
		destPoint = (destPoint + 1) % points.Length;
	}


	void Update () {
		// Choose the next destination point when the agent gets
		// close to the current one.
		if (!agent.pathPending && agent.remainingDistance < 0.5f)
			GotoNextPoint();
	}
}
