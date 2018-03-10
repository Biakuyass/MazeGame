﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCEController : MonoBehaviour
{

    Vector3 velocity;
    Vector3 wander;

    Rigidbody playerRB;
    Maze mazeController;
    GameObject[] markers;
    GameObject[] walls;

    // Use this for initialization
    void Start()
    {
        velocity = new Vector3(0, 0, 0);
        float x = Random.Range(-1, 1);
        float y = Mathf.Sqrt(1.0f - Mathf.Pow(x, 2));
        wander = new Vector3(x, 0, y);
        playerRB = GameObject.Find("Player").GetComponent<Rigidbody>();
        mazeController = GameObject.Find("Terrain").GetComponent<Maze>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 toPlayer = playerRB.transform.position - this.transform.position;
        velocity += Vector3.Normalize(toPlayer);

        markers = GameObject.FindGameObjectsWithTag("marker");
        foreach (GameObject marker in markers)
        {
            if (marker.GetComponent<Renderer>().material.color == this.GetComponent<MeshRenderer>().material.color)
            {
                Vector3 awayMarker = this.transform.position - marker.transform.position;
                velocity += 12 * Vector3.Normalize(awayMarker) / awayMarker.magnitude;
            }
        }

        if (walls == null)
        {
            walls = GameObject.FindGameObjectsWithTag("wall");
        }
        foreach (GameObject wall in walls)
        {
            Vector3 awayMarker = this.transform.position - wall.transform.position;
            velocity += Vector3.Normalize(awayMarker) / awayMarker.magnitude;
        }

        wander += new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
        wander = Vector3.Normalize(wander);
        velocity += wander * (toPlayer.magnitude / 256); // remember about scaling
        //velocity = wander;

        if (transform.position.x <= 1)
        {
            velocity.x += 1;
        }
        else if (transform.position.x > 255)
        {
            velocity.x -= 1;
        }
        if (transform.position.z <= 1)
        {
            velocity.z += 1;
        }
        else if (transform.position.z >= 255)
        {
            velocity.z -= 1;
        }

        velocity.y = 0;
        velocity = Vector3.Normalize(velocity);
        this.transform.position = this.transform.position + velocity / 20;
    }

    void OnCollisionEnter(Collision collision) {
        print(collision.transform.gameObject.tag);
        if (collision.transform.gameObject.tag == "Ball") {
            mazeController.enemyCount -= 1;
            Destroy(transform.parent.gameObject);
        }
    }
}
