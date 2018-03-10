﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    public UnityEngine.UI.Button startbutton;
    public UnityEngine.UI.Button creditbutton;
    public UnityEngine.UI.MaskableGraphic[] menu_UI;
    //public UnityEngine.UI.MaskableGraphic[] pause_UI;
    public UnityEngine.UI.MaskableGraphic background;
    //private UnityEngine.UI.MaskableGraphic[] currUI;

    public float fadeSpeed = 2.5f;
    //public UnityEngine.UI.Text[] menu_UI;

    private bool fade_flag = false;
    private bool return_flag = false;
    private bool isPaused = false;
    private float colorAlphaRecord = 1.0f;
    private float colorAlphaRecord_background;
    // private Color highlightColor_Start;
    // private Color highlightColor_Credit;
    // private Color selectedColor_Start;
    // private Color selectedColor_Credit;

    private GameObject pauseMenu;
    private Canvas pauseMenuCanvas;


    // Use this for initialization
    void Start() {
        pauseMenu = this.gameObject;
        pauseMenuCanvas = this.gameObject.GetComponent<Canvas>();
        if (GameObject.Find("CreditMenu") != null) {
            GameObject.Find("CreditMenu").GetComponent<Canvas>().enabled = false;
        }

        if (startbutton != null)
        {
            startbutton.Select();
        }
        else {
            Debug.Log("Start button null");
        }
        colorAlphaRecord_background = background.color.a;
        //currUI = menu_UI;
        //Debug.Log(background.color.a);
        //creditbutton.Select();
        // highlightColor_Start = startbutton.colors.highlightedColor;
        // highlightColor_Credit = creditbutton.colors.highlightedColor;

        // selectedColor_Start = (startbutton.colors.highlightedColor + startbutton.colors.normalColor) / 2.0f;

        // startbutton.colors.highlightedColor = selectedColor_Start;   
        //startbutton.select

    }


    // Update is called once per frame
    void Update() {

        //startbutton;
        // Debug.Log();

        //Test for return to this menu
        if (Input.GetKeyDown(KeyCode.A))
            return_flag = true;

        if (fade_flag)
            Fade();

        if (return_flag)
            ColorReturn();


        if (//Input.GetKeyDown(KeyCode.Joystick1Button9) 
             Input.GetKeyUp(KeyCode.Escape) 
            || Input.GetKeyUp(KeyCode.Joystick1Button7)) // start, escape, other start
        {
            isPaused = !isPaused;
         
            Debug.Log("Button = " + Input.GetButton("Submit") + ", " + Input.GetButton("Cancel"));
            switch (SceneManager.GetActiveScene().name)
            {
                case "Menu":
                    GameObject.Find("GameMenu").GetComponent<Canvas>().enabled = true;
                    GameObject.Find("CreditMenu").GetComponent<Canvas>().enabled = false;
                    break;
                //case "Credits":
                //    SceneManager.LoadScene("Menu");
                //    break;
                case "Maze":
                case "JessiePuzzleRoom":
                case "HanPuzzleRoom":
                case "MattPuzzleRoom":
                case "JakesPuzzleRoom":
                case "RyanPuzzleRoom":
                    if (pauseMenu != null)
                    {
                        pauseMenuCanvas.enabled = !pauseMenuCanvas.enabled;
                        Time.timeScale = Time.timeScale == 1? 0 : 1;
                    }

                    break;
                default:
                    break;
            }
        }

    }

    void Fade()
    {
        colorAlphaRecord -= fadeSpeed * Time.deltaTime;
        foreach (UnityEngine.UI.MaskableGraphic i in menu_UI)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, colorAlphaRecord);
        }
        // for the transparent background
        background.color = new Color(background.color.r, background.color.g, background.color.b, colorAlphaRecord * colorAlphaRecord_background);

        if (colorAlphaRecord <= 0)
        {
            colorAlphaRecord = 0;
            fade_flag = false;
        }
    }

    void ColorReturn()
    {
        colorAlphaRecord += fadeSpeed * Time.deltaTime;
        foreach (UnityEngine.UI.MaskableGraphic i in menu_UI)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, colorAlphaRecord);
        }

        // for the transparent background
        
        background.color = new Color(background.color.r, background.color.g, background.color.b, Mathf.Clamp(background.color.a, 0, colorAlphaRecord_background));
       // Debug.Log(background.color.a);

        if (colorAlphaRecord >= 1)
        {
            colorAlphaRecord = 1;
            return_flag = false;

        }
    }

    //Menu will be transparent gradually,for start button
    public void ClickEvent_Start()
    {
        print("isPaused" + isPaused);
        if (SceneManager.GetActiveScene().name == "Menu") // menu -> maze
        {
            fade_flag = true;
            colorAlphaRecord = 1.0f;
            Debug.Log("Start Clicked");
            SceneManager.LoadScene("Maze");
        }
        else if (isPaused) { // game -> resume
            fade_flag = false;
            colorAlphaRecord = 0.0f;
            Debug.Log("Start Clicked");
            //print("enabled pause menu?" + pauseMenuCanvas.enabled);
            //Time.timeScale = (int) Time.timeScale ^ 0x1;
            Time.timeScale = Time.timeScale == 1 ? 0 : 1;
        }

        
    }
    //Menu will be transparent gradually,for credit button
    public void ClickEvent_Credit()
    {
        if (SceneManager.GetActiveScene().name == "Menu") // menu -> credits
        {
            fade_flag = false; // true
            colorAlphaRecord = 1.0f;
            //SceneManager.LoadScene("Credits");
            GameObject.Find("GameMenu").GetComponent<Canvas>().enabled = false;
            GameObject.Find("CreditMenu").GetComponent<Canvas>().enabled = true;
        }
        else { // maze or room -> menu
            fade_flag = true;
            colorAlphaRecord = 1.0f;
            SceneManager.LoadScene("Menu");
        }
    }

    //Menu will be transparent gradually,for resume button
    public void ClickEvent_Resume()
    {
        //fade_flag = true;
        // colorAlphaRecord = 1.0f;
        //this.enabled = false;
        pauseMenuCanvas.enabled = !pauseMenuCanvas.enabled;
        Time.timeScale = Time.timeScale == 1 ? 0 : 1;
        Debug.Log("Resume Clicked");
    }

    //Menu will be transparent gradually,for exit button
    public void ClickEvent_Exit()
    {
        if (isPaused)
        {
            fade_flag = true;
            colorAlphaRecord = 1.0f;
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("Menu");
        }
    }

    //Menu will appear gradually,used when you want to return this menu
    public void ReturnEvent()
    {
        colorAlphaRecord = 0;
        return_flag = true;
    }

}
