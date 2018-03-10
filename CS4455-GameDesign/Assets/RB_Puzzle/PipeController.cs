using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PipeController : MonoBehaviour
{
	public Material ConnectedMaterial;
	public Material DisconnectedMaterial;
	public Boolean IsSource;

	public Boolean IsConnectedToSource;

	private LayerMask _pipeLayer;

	// Use this for initialization
	void Start ()
	{
		//IsConnectedToSource = false;
		_pipeLayer = LayerMask.GetMask("pipe");
		InvokeRepeating("findConnected", 0, 1f);

	}
	
	// Update is called once per frame
	void Update () {

		if (IsSource)
		{
			IsConnectedToSource = true;
		}

		if (IsConnectedToSource)
		{
			gameObject.GetComponent<Renderer>().material = ConnectedMaterial;
		
		}
		else
		{
			gameObject.GetComponent<Renderer>().material = DisconnectedMaterial;
		}
		
		
	}

	void findConnected()
	{
		if (IsConnectedToSource)
		{
			Collider[] nearbyPipes = Physics.OverlapSphere(gameObject.transform.position, 0.5f, _pipeLayer);
			foreach (Collider pipe in nearbyPipes)
			{
				if (pipe == gameObject.GetComponent<BoxCollider>())
				{
					continue;
				}
				if (pipe.name.Contains("Pipe"))
				{
					print("pipe found");
					PipeController otherController = pipe.gameObject.GetComponentInChildren<PipeController>();
					if (otherController != null)
					{
						print("connected");
						otherController.IsConnectedToSource = true;
					}


				}
			}
		}
	}

}
