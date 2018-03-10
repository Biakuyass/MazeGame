using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum PuzzleBoxState
{
    Blue,
    Yellow
}
public class PuzzleA : MonoBehaviour {


    public PuzzleBoxState state;
    public GameObject[] neighbours;

    private UnityEngine.Object mat_yellow;
    private UnityEngine.Object mat_blue;
    private Material mat_y;
    private Material mat_b;

    

	// Use this for initialization
	void Start () {
        #if UNITY_EDITOR
		mat_yellow = AssetDatabase.LoadMainAssetAtPath("Assets/HZ/Material/Yellow.mat");
        mat_y = mat_yellow as Material;

        mat_blue = AssetDatabase.LoadMainAssetAtPath("Assets/HZ/Material/Blue.mat");
        mat_b = mat_blue as Material;
        #endif

        //gameObject.GetComponent<Renderer>().material = mat_y;

	}
	
	// Update is called once per frame
	void Update () {


		
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            ChangeState();
            for (int i = 0; i < neighbours.Length; i++)
            {
                // neighbours[i].GetComponent<Renderer>().material = mat_y;
                neighbours[i].GetComponent<PuzzleA>().ChangeState();
            }
        }

    }

    void ChangeState()
    {
        if (PuzzleBoxState.Blue == state)
        {
            gameObject.GetComponent<Renderer>().material = mat_y;
            state = PuzzleBoxState.Yellow;
            
        }
        else if (PuzzleBoxState.Yellow == state)
        {
            gameObject.GetComponent<Renderer>().material = mat_b;
            state = PuzzleBoxState.Blue;
        }

    }
}
