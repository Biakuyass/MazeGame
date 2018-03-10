﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MovingBox
{
    public bool isSpecial;
    public GameObject cube;
    public Vector3 velocity;
    public Vector3 wander;

    public MovingBox(bool isSpecial, float x, float y, float z)
    {
        this.isSpecial = isSpecial;
        this.cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //this.cube.GetComponent<Renderer>().material.color = new Color(.7f, 0.3f, 0f, 1f);
        this.cube.GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f, 1f);
        this.cube.transform.localScale = new Vector3(.2f, .2f, .2f);
        this.cube.transform.position = new Vector3(x,y,z);
        if (isSpecial)
        {
            this.cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            this.cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f) * 3;
            this.cube.GetComponent<Renderer>().material.color = new Color(0f, 1f, 0f, 1f);
            this.cube.AddComponent<SphereCollider>();
            this.cube.GetComponent<SphereCollider>().radius = 1f;
            this.cube.GetComponent<SphereCollider>().isTrigger = true;
            this.cube.transform.tag = "special";
            this.cube.transform.name = "specialCube";
        }
        else
        {
            this.cube.transform.tag = "Ball";
        }

        velocity = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100));
        velocity = velocity.normalized;
        wander = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100));
        wander = wander.normalized;
    }
}

public class Cell : MonoBehaviour
{
    protected int y;
    protected int x;
    public bool on;
    public bool alreadyMade;
    private GameObject cube;
    private GameObject cylinder;
    private GameObject portal;

    public Cell(int y, int x, int scale) {
        this.y = y;
        this.x = x;
        this.on = false;
        this.alreadyMade = false;
        this.cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        this.cylinder.transform.Rotate(90, 0, 0);
        this.cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        this.cube.layer = 13;
        this.cube.GetComponent<Renderer>().material.color = new Color(.7f, 0.3f, 0f, 1f);
        cube.transform.localScale = new Vector3(scale, 4, scale);
        cube.transform.position = new Vector3(x*scale+scale/2,0,y*scale+scale/2);
        this.cube.SetActive(false);
    }

    public void TurnOn() {
        this.on = true;
        this.cube.SetActive(true);
    }

    public void TurnOff() {
        this.on = false;
        this.cube.SetActive(false);
    }

    public void Toggle() {
        if (this.on) {
            this.TurnOff();
        } else {
            this.TurnOff();
        }
    }

    public void MakeMarker(Color c, int scale) {
        this.on = false;
        this.cube.SetActive(true);
        this.cube.tag = "marker";
        this.cube.GetComponent<Renderer>().material.color = c;
        this.cube.transform.localScale = new Vector3(0.1f*scale,4f*scale,0.1f*scale);
        this.cube.GetComponent<BoxCollider>().enabled = false;
    }

    public void MakeWall(int scale) {
        this.on = false;
        this.cube.SetActive(true);
        this.cube.tag = "wall";
        this.cube.layer = 14;
        this.cube.GetComponent<Renderer>().material.color = new Color(0.0f,0.0f,0.0f);
        this.cube.transform.localScale = new Vector3(1.0f * scale, 1f * scale, 1.0f * scale);
        this.cube.GetComponent<BoxCollider>().enabled = true;
    }

    public void MakeCoin(int scale) {
        this.on = false;
        this.cylinder.SetActive(true);
        this.cylinder.GetComponent<Renderer>().material.color = new Color(1.0f,1.0f,0.0f);
        this.cylinder.transform.localScale = new Vector3(0.1f * scale, 0.01f * scale, 0.1f * scale);
        this.cylinder.transform.position = new Vector3(x * scale + scale / 2, 1.0f, y * scale + scale / 2);
        this.cylinder.transform.Rotate(0, 0, 2, Space.Self);
    }

    public void MakeEmpty(int scale) {
        this.on = false;
        this.cube.tag = "Untagged";
        this.cube.SetActive(false);
        this.cylinder.SetActive(false);
    }

    public void MakePortal(int scale) {
        this.on = false;
        this.cube.SetActive(false);
        if (!alreadyMade) {
            alreadyMade = true;
            portal = Instantiate(GameObject.Find("Portal"));
            portal.transform.position = this.cube.transform.position;
        }

        
        //this.cube = Instantiate(GameObject.Find("NPCE"));
        //this.cube.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
        //this.cube.transform.localScale = new Vector3(1.0f * scale, 1f * scale, 1.0f * scale);
        //this.cube.GetComponent<BoxCollider>().enabled = false;
    }

    public void NameHelper() {
        this.cube.SetActive(true);
        this.cube.GetComponent<Renderer>().material.color = new Color(0f, 1f, 0f, 0.5f);
    }
}

public class Maze : MonoBehaviour {
    GameObject playerObject;
    YBotSimpleControlScript playerControl;
    GameObject npcSObject;
    NPCSController npcSControl;
    AudioSource wallBreak;
    
    public int height;
    public int width;
    public int scale;
    public List<List<Cell>> grid;
    public List<List<string>> locationGrid;

    public List<List<List<GameObject>>> brokenCubes;
    bool everyOther;
    public List<List<List<MovingBox>>> boxes;

    public float updateGridDelay;
    public float lastUpdate;
    public float mutateGridDelay;
    public float lastMutate;
    public float breakGridDelay;
    public float lastBreak;
    public float enemySpawnDelay;
    public float lastEnemySpawn;
    public float boidsUpdateDelay;
    public float lastBoidsUpdate;
    public int boidsGridWidth;
    public int enemyCap;
    public int enemyCount;

    bool leftCenter;

    List<List<int>> GetNeighbors(List<int> coordinates) {
        int coordY = coordinates[0];
        int coordX = coordinates[1];
        List<List<int>> neighbors = new List<List<int>>();
        for (int y = -1; y <= 1; y++) {
            for (int x = -1; x <= 1; x++) {
                if (coordY + y >= 0 && coordY + y < height
                        && coordX + x >= 0 && coordX + x < width
                        && (coordY + y != 0 || coordX + x != 0)) {
                    neighbors.Add(new List<int>() {coordY+y, coordX+x});
                }
            }
        }
        return neighbors;
    }

    int GetNumLivingNeighbors(List<int> coordinates) {
        int count = 0;
        foreach (List<int> n in GetNeighbors(coordinates)) {
            if (grid[n[0]][n[1]].on) {
                count++;
            }
        }
        return count;
    }

    void UpdateGrid() {
        //print("Time.time" + Time.time.ToString());
        int playerY = ((int)playerObject.GetComponent<Rigidbody>().transform.position.z) / scale;
        int playerX = ((int)playerObject.GetComponent<Rigidbody>().transform.position.x) / scale;
        List<List<bool>> tempGrid = new List<List<bool>>();
        for (int y = 0; y < height; y++) {
            tempGrid.Add(new List<bool>());
            for (int x = 0; x < width; x++) {
                tempGrid[y].Add(grid[y][x].on);
                int lN = GetNumLivingNeighbors(new List<int>() { y, x });
                if (lN == 3) {
                    tempGrid[y][x] = true;
                }
                else if (lN < 1 || lN > 5) {
                    tempGrid[y][x] = false;
                }
            }
        }
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (grid[y][x].on && !tempGrid[y][x] && locationGrid[y][x] != "!") {
                    grid[y][x].TurnOff();
                }
                if (!grid[y][x].on && tempGrid[y][x] && locationGrid[y][x] == "0" && (x != playerX || y != playerY)) {
                    grid[y][x].TurnOn();
                }
            }
        }
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (locationGrid[y][x] == "!") {
                    grid[y][x].TurnOn();
                }
                if (locationGrid[y][x] == "w") {
                    grid[y][x].MakeWall(scale);
                }
                if (locationGrid[y][x] == "mR") {
                    grid[y][x].MakeMarker(new Color(1.0f, 0.0f, 0.0f), scale);
                }
                if (locationGrid[y][x] == "mG") {
                    grid[y][x].MakeMarker(new Color(0.0f, 1.0f, 0.0f), scale);
                }
                if (locationGrid[y][x] == "mB") {
                    grid[y][x].MakeMarker(new Color(0.0f, 0.0f, 1.0f), scale);
                }
                if (locationGrid[y][x] == "c") {
                    grid[y][x].MakeCoin(scale);
                }
            }
        }
    }

    void MutateGrid() {
        int playerY = ((int)playerObject.GetComponent<Rigidbody>().transform.position.z) / scale;
        int playerX = ((int)playerObject.GetComponent<Rigidbody>().transform.position.x) / scale;
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (locationGrid[y][x] == "0" 
                        && Mathf.Sqrt(Mathf.Pow(y-playerY,2) + Mathf.Pow(x-playerX,2)) > 8) {
                    if (Random.Range(0, 256) == 0) {
                        print("!");
                        grid[y][x].Toggle();
                    }
                }
            }
        }
    }

    public void BreakGrid() {
        if ((Time.timeSinceLevelLoad - lastBreak) > breakGridDelay) {
            lastUpdate = Time.timeSinceLevelLoad;
            lastMutate = Time.timeSinceLevelLoad;
            //print(player.transform.position);
            //print(player.transform.position + player.transform.forward);
            Vector3 targetPos = playerObject.GetComponent<Rigidbody>().transform.position 
                    + (scale / 2) * playerObject.GetComponent<Rigidbody>().transform.forward;
            //int targetX = ((int)targetPos.x + (scale / 2)) / scale;
            //int targetY = ((int)targetPos.z + (scale / 2)) / scale;'
            int targetX = ((int)targetPos.x) / scale;
            int targetY = ((int)targetPos.z) / scale;
            if ((locationGrid[targetY][targetX] == "0" || locationGrid[targetY][targetX] == "!") && grid[targetY][targetX].on) {
                if (locationGrid[targetY][targetX] == "!") {
                    locationGrid[targetY][targetX] = "0";
                    leftCenter = true;
                }
                lastBreak = Time.timeSinceLevelLoad;            
                grid[targetY][targetX].TurnOff();
                wallBreak.transform.position = targetPos;
                wallBreak.Play();

                float initX = targetX * scale + (scale / 8);
                float initY = 0.5f;
                float initZ = targetY * scale + (scale / 8);
                for (int x = 0; x < 8; x++) {
                    for (int y = 0; y < 4; y++) {
                        for (int z = 0; z < 8; z++) {
                            brokenCubes[x][y][z].SetActive(true);
                            brokenCubes[x][y][z].transform.position = new Vector3(initX + 0.5f * x, initY + 0.5f * y, initZ + 0.5f * z);
                        }
                    }
                }
            }
        }
               
    }

    public void Pickup() {
        int playerY = ((int)playerObject.GetComponent<Rigidbody>().transform.position.z) / scale;
        int playerX = ((int)playerObject.GetComponent<Rigidbody>().transform.position.x) / scale;
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if ((playerY == y && playerX == x) && locationGrid[y][x] == "mR") {
                    locationGrid[y][x] = "0";
                    grid[y][x].MakeEmpty(scale);
                    playerControl.inventory.Add(Inventory.mR);
                }
                if ((playerY == y && playerX == x) && locationGrid[y][x] == "mG") {
                    locationGrid[y][x] = "0";
                    grid[y][x].MakeEmpty(scale);
                    playerControl.inventory.Add(Inventory.mG);
                }
                if ((playerY == y && playerX == x) && locationGrid[y][x] == "mB") {
                    locationGrid[y][x] = "0";
                    grid[y][x].MakeEmpty(scale);
                    playerControl.inventory.Add(Inventory.mB);
                }
            }
        }
    }

    public void Place(Inventory i) {
        int playerY = ((int)playerObject.GetComponent<Rigidbody>().transform.position.z) / scale;
        int playerX = ((int)playerObject.GetComponent<Rigidbody>().transform.position.x) / scale;
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if ((playerY == y && playerX == x) && (locationGrid[y][x] == "0"
                        || locationGrid[y][x] == "e")) {
                    if (i == Inventory.mR) {
                        locationGrid[y][x] = "mR";
                    } else if (i == Inventory.mG)
                    {
                        locationGrid[y][x] = "mG";
                    } else if (i == Inventory.mB)
                    {
                        locationGrid[y][x] = "mB";
                    } 
                    playerControl.inventory.Remove(i);
                    UpdateGrid();
                }
            }
        }
    }

 

    // Use this for initialization
    void Start () {
        playerObject = GameObject.Find("Player");
        playerControl = playerObject.GetComponent<YBotSimpleControlScript>();
        npcSObject = GameObject.Find("NPCS");
        npcSControl = npcSObject.GetComponent<NPCSController>();
        wallBreak = GameObject.Find("WallBreak").GetComponent<AudioSource>();

        updateGridDelay = 4f;
        lastUpdate = 0f;
        mutateGridDelay = 8f;
        lastMutate = 0f;
        breakGridDelay = 0f;
        lastBreak = 0f;
        enemySpawnDelay = 30f;
        lastEnemySpawn = 0f;
        enemyCap = 8;
        boidsUpdateDelay = 0f;
        lastBoidsUpdate = 0f;
        boidsGridWidth = 32;

        height = 64;
        width = 64;
        scale = 4;

        brokenCubes = new List<List<List<GameObject>>>();
        for (int x = 0; x < 8; x++) {
            brokenCubes.Add(new List<List<GameObject>>());
            for (int y = 0; y < 4; y++) {
                brokenCubes[x].Add(new List<GameObject>());
                for (int z = 0; z < 8; z++) {
                    GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    temp.GetComponent<Renderer>().material.color = new Color(.7f, 0.3f, 0f, 1f);
                    temp.AddComponent<Rigidbody>();
                    temp.GetComponent<Rigidbody>().mass = 1f;
                    temp.transform.localScale = new Vector3(0.425f, 0.425f, 0.425f);
                    temp.SetActive(false);
                    brokenCubes[x][y].Add(temp);
                }
            }
        }

        everyOther = false;
        boxes = new List<List<List<MovingBox>>>();
        for (int y = 0; y < 256/boidsGridWidth; y++) {
            boxes.Add(new List<List<MovingBox>>());
            for (int x = 0; x < 256/boidsGridWidth; x++) {
                boxes[y].Add(new List<MovingBox>());
                for (int n = 0; n < 1; n++) {
                    boxes[y][x].Add(new MovingBox(false, Random.Range(x * boidsGridWidth, (x + 1) * boidsGridWidth),
                            5, Random.Range(y * boidsGridWidth, (y + 1) * boidsGridWidth)));
                }
            }
        }

        leftCenter = false;

        locationGrid = new List<List<string>>()
        {
            new List<string>() {"w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},// 3
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","e","e","e","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","n0","e","e","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","e","p2","e","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","e","p0","e","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","e","e","e","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},// 7
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","e","e","e","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","c","c","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","o","0","0","0","0","0","0","0","0","0","0","c","c","c","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","c","c","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},// 11
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","c","c","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","c","c","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","c","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","c","c","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","c","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},// 15
            new List<string>() {"w","0","0","0","0","0","0","c","c","c","c","c","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","c","c","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","c","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","c","c","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","c","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","c","c","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","e","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},// 19
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","e","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","e","e","e","e","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},// 23
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","e","e","mG","e","e","0","0","0","0","0","0","0","0","0","0","0","0","e","e","e","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","e","mR","c","mB","e","0","0","0","0","0","0","0","0","0","0","0","0","e","p3","e","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","e","e","e","c","c","c","e","e","e","0","0","0","0","0","0","0","0","0","0","e","e","e","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w","w","w","w","!","w","w","w","w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},// 27
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","c","0","0","0","0","0","0","0","0","0","0","0","0","w","e","e","e","e","e","e","e","w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","c","c","c","0","0","0","0","0","0","0","0","0","0","0","w","e","e","e","e","e","e","e","w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","c","c","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","c","c","c","c","c","0","0","0","0","0","0","0","0","0","0","w","e","e","e","e","e","nS","e","w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","c","c","c","c","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","c","c","c","0","0","0","0","0","0","0","0","0","0","0","w","e","e","e","e","e","e","e","w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","c","c","0","0","0","0","0","0","0","w"},// 31
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","c","0","0","0","0","0","0","0","0","0","0","0","0","w","e","e","e","s","e","e","e","w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w","e","e","e","e","e","e","e","w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w","p0","p1","p2","p3","p4","p5","e","w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w","w","w","w","w","w","w","w","w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},// 35
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},// 39
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","e","e","e","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","e","p1","e","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},// 43
            new List<string>() {"w","0","0","0","0","0","0","0","0","e","e","e","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},// 47
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","e","0","0","0","0","0","0","0","0","w"},// 51
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","e","e","e","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","e","p4","e","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","c","c","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","e","e","e","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","c","c","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","c","c","0","0","0","0","0","0","0","0","w"},// 55
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","c","c","c","c","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","c","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},// 59
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","w"},
            new List<string>() {"w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w","w"}
        };
        locationGrid.Reverse();
        
        grid = new List<List<Cell>>();
        for (int y = 0; y < height; y++) {
            grid.Add(new List<Cell>());
            for (int x = 0; x < width; x++) {
                Cell c = new Cell(y,x,scale);
                grid[y].Add(c);
            }
        }
        int u = height/2 - height/4;
        int d = height/2 + height/4;
        int l = width/2 - width/4;
        int r = width/2 + width/4;
        /*int u = 0;
        int d = height;
        int l = 0;
        int r = width;*/

        for (int y = u; y < d; y++) {
            for (int x = l; x < r; x++) {
                if (Random.Range(0,2) == 1 && locationGrid[y][x] == "0") {
                    grid[y][x].TurnOn();
                }
            }
        }

        for (int i = 0; i < 80; i++) {
            UpdateGrid();
        }

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (locationGrid[y][x] == "0" && !grid[y][x].on) {
                    if (Random.Range(0, 10) == 0) {
                        locationGrid[y][x] = "c";
                    }
                }
            }
        }
     
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (locationGrid[y][x] == "nS") {
                    npcSObject.GetComponent<Rigidbody>().transform.position = new Vector3(x * scale, 1, y * scale);
                }
                if (locationGrid[y][x] == "s") {
                    playerObject.GetComponent<Rigidbody>().transform.position = new Vector3(x * scale, 0, y * scale);
                }
                if (locationGrid[y][x].Contains("p")) {
                    GameObject temp = Instantiate(GameObject.Find("PortalLure"));
                    temp.transform.position = new Vector3(x * scale + scale / 2, 0, y * scale + scale /2);
                }
            }
        }
        playerControl.inventory.Add(Inventory.ball);
        //playerControl.inventory.Add(Inventory.mG);
        if (playerControl.ui.isActiveAndEnabled == false) {
            playerControl.ui.enabled = true;
        }
    }

    // Update is called once per frame
    void Update() {
        
    }

    void FixedUpdate() {
        
        int playerY = ((int)playerObject.GetComponent<Rigidbody>().transform.position.z) / scale;
        int playerX = ((int)playerObject.GetComponent<Rigidbody>().transform.position.x) / scale;
        //print("pGridY" + playerY.ToString() + "pGridX" + playerX.ToString());
        //print("location at player: " + locationGrid[playerY][playerX]);
        int markerRD = 0;
        int markerGD = 0;
        int markerBD = 0;
        int npcSD = 0;
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (locationGrid[y][x] == "mR") {
                    markerRD = (int) Mathf.Sqrt(Mathf.Pow(playerY - y, 2) + Mathf.Pow(playerX - x, 2));
                }
                if (locationGrid[y][x] == "mG") {
                   markerGD = (int) Mathf.Sqrt(Mathf.Pow(playerY - y, 2) + Mathf.Pow(playerX - x, 2));
                }
                if (locationGrid[y][x] == "mB") {
                    markerBD = (int) Mathf.Sqrt(Mathf.Pow(playerY - y, 2) + Mathf.Pow(playerX - x, 2));
                }
                if (locationGrid[y][x] == "nS") {
                    npcSD = (int)Mathf.Sqrt(Mathf.Pow(playerY - y, 2) + Mathf.Pow(playerX - x, 2));
                    //npc0Control.Interact();
                    //print("should interact");
                }
                if (locationGrid[y][x] == "c" && (playerY == y && playerX == x)) {
                    locationGrid[y][x] = "e";
                    grid[y][x].MakeEmpty(scale);
                    playerControl.score += 1;
                }
                if (locationGrid[y][x] == "c") {
                    grid[y][x].MakeCoin(scale);
                }
                if (locationGrid[y][x] == "p0") {
                    grid[y][x].MakePortal(scale);
                }
                if (locationGrid[y][x] == "p1") {
                    grid[y][x].MakePortal(scale);
                }
                if (locationGrid[y][x] == "p2") {
                    grid[y][x].MakePortal(scale);
                }
                if (locationGrid[y][x] == "p3") {
                    grid[y][x].MakePortal(scale);
                }
                if (locationGrid[y][x] == "p4") {
                    grid[y][x].MakePortal(scale);
                }

            }
        }
        // print("npcd:" + npc0D.ToString());
        if (npcSD <= 1) {
            npcSControl.Interact();
        }
        string uiText = "";
        uiText += "Health: ";
        for (int i = 0; i < playerControl.health; i++) {
            uiText += "*";
        }
        uiText += "\n";
        uiText += "Score: " + playerControl.score.ToString() + "\n";
        if (!playerControl.inventory.Contains(Inventory.mR)) {
            uiText += "Distance to Red Marker: " + markerRD.ToString() + "\n";
        }
        if (!playerControl.inventory.Contains(Inventory.mG)) {
            uiText += "Distance to Green Marker: " + markerGD.ToString() + "\n";
        }
        if (!playerControl.inventory.Contains(Inventory.mB)) {
            uiText += "Distance to Blue Marker: " + markerBD.ToString() + "\n";
        }
        playerControl.generalText.text = uiText;

        /*string uiInventoryText = "";
        uiInventoryText += "Inventory: \n";
        if (playerControl.hasMarkerR)
        {
            uiInventoryText += "Red Marker\n";
        }
        if (playerControl.hasMarkerG)
        {
            uiInventoryText += "Green Marker\n";
        }
        if (playerControl.hasMarkerB)
        {
            uiInventoryText += "Blue Marker\n";
        }
        playerControl.inventoryText.text = uiInventoryText;*/
        string uiInventoryText = "";
        uiInventoryText += "Inventory: \n";
        for (int i = 0; i < playerControl.inventory.Count; i++) {
            if (i == playerControl.inventoryIdx) {
                uiInventoryText += ">";
            }
            if (playerControl.inventory[i] == Inventory.ball) {
                uiInventoryText += "Ball\n";
            } else if (playerControl.inventory[i] == Inventory.mR) {
                uiInventoryText += "Red Marker\n";
            } else if (playerControl.inventory[i] == Inventory.mG) {
                uiInventoryText += "Green Marker\n";
            } else if (playerControl.inventory[i] == Inventory.mB) {
                uiInventoryText += "Blue Marker\n";
            }
        }
        //print("uiInventoryText: " + uiInventoryText);
        playerControl.inventoryText.text = uiInventoryText;
        breakGridDelay = 5.0f - playerControl.score * 0.1f;
        //print(breakGridDelay);
        if ((Time.timeSinceLevelLoad - lastUpdate) > updateGridDelay) {
            lastUpdate = Time.timeSinceLevelLoad;
            UpdateGrid();
        }
        if ((Time.timeSinceLevelLoad - lastMutate) > mutateGridDelay) {
            lastMutate = Time.timeSinceLevelLoad;
            MutateGrid();
        }
        if ((Time.timeSinceLevelLoad - lastEnemySpawn) > enemySpawnDelay
                && enemyCount < enemyCap && leftCenter) {
            lastEnemySpawn = Time.timeSinceLevelLoad;
            enemyCount += 1;
            GameObject temp = Instantiate(GameObject.Find("NPCE"));
            int r = Random.Range(0, 3);
            if (r == 0) {
                temp.GetComponent<MeshRenderer>().material.color = Color.red;
            }
            else if (r == 1) {
                temp.GetComponent<MeshRenderer>().material.color = Color.green;
            }
            else if (r == 2) {
                temp.GetComponent<MeshRenderer>().material.color = Color.blue;
            }
            r = Random.Range(0, 4);
            if (r == 0) {
                temp.transform.position = new Vector3(scale + scale / 2, 1, scale + scale / 2 );
            } else if (r == 1) {
                temp.transform.position = new Vector3(256 - scale - scale/2, 1, scale + scale/2);
            } else if (r == 2) {
                temp.transform.position = new Vector3(256 - scale - scale/2, 1, 256 - scale - scale/2);
            } else if (r == 3) {
                temp.transform.position = new Vector3(256 - scale - scale/2, 1, scale + scale / 2);
            }
        }
        if (locationGrid[playerY][playerX] == "p0") {
            SceneManager.LoadScene("JakesPuzzleRoom");
        } else if (locationGrid[playerY][playerX] == "p1") {
            SceneManager.LoadScene("JessiePuzzleRoom");
        } else if (locationGrid[playerY][playerX] == "p2") {
            SceneManager.LoadScene("MattPuzzleRoom");
        } else if (locationGrid[playerY][playerX] == "p3") {
            SceneManager.LoadScene("HanPuzzleRoom");
        } else if (locationGrid[playerY][playerX] == "p4") {
            SceneManager.LoadScene("RyanPuzzleRoom");
        }

        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 4; y++) {
                for (int z = 0; z < 8; z++) {
                    if (brokenCubes[x][y][z].activeInHierarchy) {
                        if (Random.Range(0f,1f) < 0.01) {
                            brokenCubes[x][y][z].SetActive(false);
                        }
                    }
                }
            }
        }

        List<List<List<MovingBox>>> newBoxes = new List<List<List<MovingBox>>>();
        for (int y = 0; y < 256 / boidsGridWidth; y++) {
            newBoxes.Add(new List<List<MovingBox>>());
            for (int x = 0; x < 256 / boidsGridWidth; x++) {
                newBoxes[y].Add(new List<MovingBox>());
            }
        }
        for (int y = 0; y < 256 / boidsGridWidth; y++) {
            for (int x = 0; x < 256 / boidsGridWidth; x++) {
                foreach (MovingBox b0 in boxes[y][x]) {
                    int boundedY = Mathf.Min((int)b0.cube.transform.position.z / boidsGridWidth, 256 / boidsGridWidth - 1);
                    boundedY = Mathf.Max(boundedY, 0);
                    int boundedX = Mathf.Min((int)b0.cube.transform.position.x / boidsGridWidth, 256 / boidsGridWidth - 1);
                    boundedX = Mathf.Max(boundedX, 0);
                    List<MovingBox> tempBoxes = new List<MovingBox>();
                    for (int yN = -1; yN <= 1; yN++) {
                        for (int xN = -1; xN <= 1; xN++) {
                            if ((boundedY + yN) >= 0 && boundedY + yN < (256 / boidsGridWidth)
                                    && (boundedX + xN) >= 0 && (boundedX + xN) < (256 / boidsGridWidth)) {
                                if (boxes[boundedY + yN][boundedX + xN].Count > 0) {
                                    tempBoxes.AddRange(boxes[boundedY + yN][boundedX + xN]);
                                }
                            }
                        }
                    }
                    Vector3 oldVelocity = b0.velocity;
                    Vector3 newVelocity = new Vector3(0, 0, 0);
                    Vector3 repulsiveVelocity = new Vector3(0, 0, 0);
                    Vector3 avgVelocity = new Vector3(0, 0, 0);
                    Vector3 avgPosition = new Vector3(0, 0, 0);
                    Vector3 boundsVelocity = new Vector3(0, 0, 0);
                    if (b0.cube.transform.position.x < 4) {
                        boundsVelocity.x += 1;
                    }
                    else if (b0.cube.transform.position.x > 252) {
                        boundsVelocity.x -= 1;
                    }
                    if (b0.cube.transform.position.y < 5) {
                        boundsVelocity.y += 1;
                    }
                    else if (b0.cube.transform.position.y > 10) {
                        boundsVelocity.y -= 1;
                    }
                    if (b0.cube.transform.position.z < 4) {
                        boundsVelocity.z += 1;
                    }
                    else if (b0.cube.transform.position.z > 252) {
                        boundsVelocity.z -= 1;
                    }
                    int numNeighbors = 0;
                    foreach (MovingBox b1 in tempBoxes) {
                        double distance = Vector3.Distance(b0.cube.transform.position, b1.cube.transform.position);
                        if (b0 != b1 && distance < 5) {
                            numNeighbors++;
                            Vector3 toOther = b0.cube.transform.position - b1.cube.transform.position;
                            repulsiveVelocity += Vector3.Normalize(toOther) / toOther.magnitude;
                            avgVelocity += b1.velocity;
                            avgPosition += b1.cube.transform.position;
                        }
                    }
                    newVelocity += oldVelocity;
                    b0.wander += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
                    b0.wander = Vector3.Normalize(b0.wander);
                    newVelocity += 0.25f * b0.wander;
                    if (numNeighbors > 0) {
                        avgPosition /= numNeighbors;
                        newVelocity += Vector3.Normalize(repulsiveVelocity);
                        newVelocity += Vector3.Normalize((avgPosition - b0.cube.transform.position));
                        newVelocity += 0.5f * Vector3.Normalize(avgVelocity / numNeighbors);
                    }
                    newVelocity += 0.25f * boundsVelocity;
                    b0.velocity = Vector3.Normalize(newVelocity);

                    b0.cube.transform.position = b0.cube.transform.position + b0.velocity / 4;
                    boundedY = Mathf.Min((int)b0.cube.transform.position.z / boidsGridWidth, 256 / boidsGridWidth - 1);
                    boundedY = Mathf.Max(boundedY, 0);
                    boundedX = Mathf.Min((int)b0.cube.transform.position.x / boidsGridWidth, 256 / boidsGridWidth - 1);
                    boundedX = Mathf.Max(boundedX, 0);
                    newBoxes[boundedY][boundedX].Add(b0);
                }
            }
        }
        boxes = newBoxes;
    }
}
