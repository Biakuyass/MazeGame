using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

   // public GameObject[] weak_points;
    public int totalhealth = 4;

    public GameObject Boss;
    public GameObject puzzleA;
    public GameObject[] puzzleACubes;
    public GameObject player;
    public Transform pizzleA_Pos;
   // public GameObject[] Enemies;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
       // Debug.Log(1);
       // LastPuzzle();
		if(totalhealth == 0)
        {
          //  LastPuzzle();
            ClearStage();
            totalhealth--;
        }
        /*if (totalhealth == -1)
        {
            int count = 0;
            for (int i = 0; i < puzzleACubes.Length; i++)
            {
                PuzzleBoxState tempState = puzzleACubes[i].GetComponent<PuzzleA>().state;
                if (PuzzleBoxState.Yellow == tempState)
                {
                    count++;
                }

            }
            if (count == puzzleACubes.Length)
            {

                ClearStage();
                totalhealth--;

            }
        }*/

	}

    public void Damaged()
    {
        totalhealth--;
    }
    void ClearStage()
    {

        EventManager.TriggerEvent<ClearEvent, Vector3>(puzzleA.transform.position);
        Boss.GetComponent<BossControl>().Death();
        Destroy(puzzleA);

        string tips = "You have cleared all puzzles and will return to the maze soon";
        gameObject.GetComponent<TipsManager>().ChangeTips(tips);

        StartCoroutine(AfterClearState());
        /*foreach(GameObject i in Enemies)
        {
            Destroy(i);
        }*/

        
        Debug.Log("ClearStage");
 
    }

    IEnumerator AfterClearState()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Maze");
        //Destroy(gameObject);
    }
    void LastPuzzle()
    {
        puzzleA.SetActive(true);

        string tips = "Final Puzzle appears in the middle of the map: make all the boxes Yellow";
        gameObject.GetComponent<TipsManager>().ChangeTips(tips);

        player.transform.position = pizzleA_Pos.position;
        player.transform.rotation = pizzleA_Pos.rotation;

        Debug.Log("last puzzle");
    }




}
