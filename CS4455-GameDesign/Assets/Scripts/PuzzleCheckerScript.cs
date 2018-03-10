using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PuzzleCheckerScript : MonoBehaviour {

    Transform[] pieces;
    public bool puzzleCompleted = false;
    float distCheck = 16f;

    public AudioClip success;
    private bool playedSuccessSound = false;


    private bool AllPiecesInPlace {
        get { return PieceZeroInPlace && PieceOneInPlace && PieceTwoInPlace && PieceThreeInPlace && PieceFourInPlace && PieceFiveInPlace; }
    }

    private bool PieceZeroInPlace
    {
        get {
            Transform zero = pieces[0];
            Transform one = pieces[1];
            Transform three = pieces[3];

            return zero.position.z < one.position.z && zero.position.x > three.position.x && (zero.position - three.position).sqrMagnitude < distCheck && (zero.position - one.position).sqrMagnitude < distCheck;
        }
    }

    private bool PieceOneInPlace
    {
        get
        {
            Transform zero = pieces[0];
            Transform one = pieces[1];
            Transform two = pieces[2];
            Transform four = pieces[4];

            return one.position.z < two.position.z && one.position.z > zero.position.z && one.position.x > four.position.x && (one.position - zero.position).sqrMagnitude < distCheck && (one.position - two.position).sqrMagnitude < distCheck && (one.position - four.position).sqrMagnitude < distCheck;
        }
    }

    private bool PieceTwoInPlace
    {
        get
        {
            Transform one = pieces[1];
            Transform two = pieces[2];
            Transform five = pieces[5];

            return one.position.z < two.position.z && two.position.x > five.position.z && (two.position - one.position).sqrMagnitude < distCheck && (two.position - five.position).sqrMagnitude < distCheck;
        }
    }

    private bool PieceThreeInPlace
    {
        get
        {
            Transform zero = pieces[0];
            Transform three = pieces[3];
            Transform four = pieces[4];

            return three.position.z < four.position.z && three.position.x < zero.position.x && (three.position - zero.position).sqrMagnitude < distCheck && (three.position - four.position).sqrMagnitude < distCheck;
        }
    }

    private bool PieceFourInPlace
    {
        get
        {
            Transform one = pieces[1];
            Transform three = pieces[3];
            Transform four = pieces[4];
            Transform five = pieces[5];

            return four.position.z < five.position.z && four.position.z > three.position.z && four.position.x < one.position.x && (four.position - one.position).sqrMagnitude < distCheck && (four.position - three.position).sqrMagnitude < distCheck && (four.position - five.position).sqrMagnitude < distCheck;
        }
    }

    private bool PieceFiveInPlace
    {
        get
        {
            Transform two = pieces[2];
            Transform four = pieces[4];
            Transform five = pieces[5];

            return five.position.z > four.position.z && five.position.x < two.position.x && (five.position - four.position).sqrMagnitude < distCheck && (five.position - two.position).sqrMagnitude < distCheck;
        }
    }

    // Use this for initialization
    void Start () {
        pieces = new Transform[this.transform.childCount];

        for (int i = 0; i < this.transform.childCount; i++) {
            pieces[i] = transform.GetChild(i);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (AllPiecesInPlace) {
            puzzleCompleted = true;
        }

        if (puzzleCompleted) {
            GameObject.Find("cave_light").GetComponent<Light>().enabled = true;
        }

        if (puzzleCompleted && !playedSuccessSound) {
            AudioSource.PlayClipAtPoint(success, GameObject.Find("YBot").transform.position);
            playedSuccessSound = true;
        }
    }
}
