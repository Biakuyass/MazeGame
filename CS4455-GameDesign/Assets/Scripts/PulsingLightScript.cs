using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsingLightScript : MonoBehaviour {

    Light mLight;
    float speed = 5f;
    float maxIntensity = 5f;
    float maxRange = 10f;

    private Quaternion DOOR_POS_OPEN;
    private Quaternion DOOR_POS_CLOSED;

    private float startTime = 0f;


    // Use this for initialization
    void Start () {
        mLight = gameObject.GetComponent<Light>();
        mLight.enabled = false;

        GameObject.Find("crystal_go").GetComponent<AudioSource>().enabled = false;

        DOOR_POS_CLOSED = GameObject.Find("door1").transform.rotation;
        DOOR_POS_OPEN = new Quaternion(0, 1, 0, 0);
    }
	
	// Update is called once per frame
	void Update () {
        if (mLight.enabled)
        {
            mLight.intensity = Mathf.PingPong(Time.time * speed, maxIntensity);
            GameObject.Find("door2").transform.rotation = Quaternion.Lerp(DOOR_POS_CLOSED, DOOR_POS_OPEN, ((Time.time - startTime) * 15f) / 5.5f);
            //light.range = Mathf.PingPong(Time.time * speed, maxRange);
        }
        else {
            GameObject.Find("door2").transform.rotation = Quaternion.Lerp(DOOR_POS_OPEN, DOOR_POS_CLOSED, ((Time.time - startTime) * 15f) / 5.5f);
            mLight.intensity = 0;
            //light.range = 0;
        }
    }

    public void setLight(bool e) {
        if (e != mLight.enabled) {
            startTime = Time.time;
        }

        GameObject.Find("crystal_go").GetComponent<AudioSource>().enabled = e;

        mLight.enabled = e;
    }

    public bool isEnabled() {
        return mLight.enabled;
    }
}
