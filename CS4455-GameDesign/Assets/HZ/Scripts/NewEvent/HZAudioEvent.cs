using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HZAudioEvent : MonoBehaviour {

    private UnityAction<Vector3> clearEventListener;
    private UnityAction<Vector3> explosionEventListener;
    private UnityAction<Vector3> robotExploEventListener;
    private UnityAction<Vector3> bossAttackExploEventListener;


    public AudioClip clearAudio;
    public AudioClip explosionAudio;
    public AudioClip robotExplosionAudio;
    public AudioClip bossATExplosionAudio;

    void Awake()
    {
        clearEventListener = new UnityAction<Vector3>(clearEventHandler);
        explosionEventListener = new UnityAction<Vector3>(explosionEventHandler);
        robotExploEventListener = new UnityAction<Vector3>(robotExploEventHandler);
        bossAttackExploEventListener = new UnityAction<Vector3>(bossAttackExploEventHandler);

    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnEnable()
    {

        EventManager.StartListening<ClearEvent, Vector3>(clearEventListener);
        EventManager.StartListening<Explosion1, Vector3>(explosionEventListener);
        EventManager.StartListening<RobotExplosion, Vector3>(robotExploEventListener);
        EventManager.StartListening<BossAttackEvent, Vector3>(bossAttackExploEventListener);

    }

    void OnDisable()
    {

        EventManager.StopListening<ClearEvent, Vector3>(clearEventListener);
        EventManager.StopListening<Explosion1, Vector3>(explosionEventListener);
        EventManager.StopListening<RobotExplosion, Vector3>(robotExploEventListener);
        EventManager.StopListening<BossAttackEvent, Vector3>(bossAttackExploEventListener);
    }

    void explosionEventHandler(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(explosionAudio, position);
    }

    void robotExploEventHandler(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(robotExplosionAudio, position);
    }

    void bossAttackExploEventHandler(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(bossATExplosionAudio,position);
    }

    void clearEventHandler(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clearAudio, position);
    }
}
