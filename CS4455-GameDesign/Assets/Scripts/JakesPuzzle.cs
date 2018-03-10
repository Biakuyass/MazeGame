﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JakesPuzzle : MonoBehaviour {
    public Rigidbody player;

    List<MovingBox> boxes;

    // Use this for initialization
    void Start () {
        player.transform.position = new Vector3(16, 0, 16);

        boxes = new List<MovingBox>();

        for (int i = 0; i < 128; i++) {
            boxes.Add(new MovingBox(false, Random.Range(0, 32), Random.Range(1, 2), Random.Range(0, 32)));
        }
        boxes.Add(new MovingBox(true, Random.Range(0, 32), Random.Range(1, 2), Random.Range(0, 32)));
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate() {
        /*print(boxes[0].cube.transform.ToString());
        print(boxes[0].velocity.ToString());
        print((boxes[0].cube.transform.position + boxes[0].velocity).ToString());*/
       
        foreach (MovingBox b0 in boxes) {
            Vector3 oldVelocity = b0.velocity;
            Vector3 newVelocity = new Vector3(0, 0, 0);
            Vector3 repulsiveVelocity = new Vector3(0, 0, 0);
            Vector3 avgVelocity = new Vector3(0, 0, 0);
            Vector3 avgPosition = new Vector3(0, 0, 0);
            Vector3 boundsVelocity = new Vector3(0, 0, 0);
            if (b0.cube.transform.position.x < 0) {
                boundsVelocity.x += 1;
            } else if (b0.cube.transform.position.x > 32) {
                boundsVelocity.x -= 1;
            }
            if (b0.cube.transform.position.y < 1) {
                boundsVelocity.y += 1;
            }
            else if (b0.cube.transform.position.y > 2) {
                boundsVelocity.y -= 1;
            }
            if (b0.cube.transform.position.z < 0) {
                boundsVelocity.z += 1;
            }
            else if (b0.cube.transform.position.z > 32) {
                boundsVelocity.z -= 1;
            }
            int numNeighbors = 0;
            foreach (MovingBox b1 in boxes) {
                double distance = Vector3.Distance(b0.cube.transform.position, b1.cube.transform.position);
                if (b0 != b1 && distance < 1) {
                    numNeighbors++;
                    Vector3 toOther = b0.cube.transform.position - b1.cube.transform.position;
                    repulsiveVelocity += Vector3.Normalize(toOther) / toOther.magnitude;                
                    avgVelocity += b1.velocity;
                    avgPosition += b1.cube.transform.position;
                }
            }
            if (numNeighbors > 0) {
                avgPosition /= numNeighbors;
                newVelocity += Vector3.Normalize(repulsiveVelocity);
                newVelocity += 0.75f * Vector3.Normalize(avgPosition - b0.cube.transform.position);
                newVelocity += Vector3.Normalize(avgVelocity / numNeighbors);
            } else {

            }
            newVelocity += 2f * Vector3.Normalize(oldVelocity);
            newVelocity += 0.1f * Vector3.Normalize(boundsVelocity);

            b0.velocity = Vector3.Normalize(newVelocity);
            b0.cube.transform.position = b0.cube.transform.position + b0.velocity / 20;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);

        if (collision.gameObject.tag == "special") {
            SceneManager.LoadScene("Maze");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }
}
