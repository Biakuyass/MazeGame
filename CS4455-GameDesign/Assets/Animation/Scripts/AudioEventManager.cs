using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerFootEvent: UnityEvent<Vector3, Material> { }
public class PlayerCollisionEvent : UnityEvent<Vector3, Material> { }
public class NpcEvent : UnityEvent<Vector3, int> { }

public class AudioEventManager : MonoBehaviour
{

    public AudioClip boxAudio;
    public AudioClip playerLandsAudio;
    public AudioClip playerFootAudio;
    public AudioClip grassAudio;
    public AudioClip snowAudio;
    public AudioClip mooAudio;
    public AudioClip crateAudio;
    public AudioClip woodAudio;
    public AudioClip ballAudio;
    public AudioClip honkAudio;
    public AudioClip collectibleAudio;
    public AudioClip enemyHitAudio;
    public AudioClip playerHurtAudio;

    private UnityAction<Vector3, Material> playerCollisionEventListener;

    private UnityAction<Vector3> playerLandsEventListener;

    private UnityAction<Vector3, Material> playerFootEventListener;

    private UnityAction<Vector3, int> npcEventListener;

    private UnityAction<Vector3> collectibleEventListener;

    private UnityAction<Vector3> enemyHitEventListener;

    private UnityAction<Vector3> playerHurtEventListener;


    public ParticleSystem particles;

    void Awake()
    {

        playerCollisionEventListener = new UnityAction<Vector3, Material>(playerCollisionEventHandler);

        playerLandsEventListener = new UnityAction<Vector3>(playerLandsEventHandler);

        playerFootEventListener = new UnityAction<Vector3, Material>(playerFootEventHandler);
        npcEventListener = new UnityAction<Vector3, int>(npcEventHandler);

        collectibleEventListener = new UnityAction<Vector3>(collectibleEventHandler);

        enemyHitEventListener = new UnityAction<Vector3>(enemyHitEventHandler);
        playerHurtEventListener = new UnityAction<Vector3>(playerHurtEventHandler);
    }


    // Use this for initialization
    void Start()
    {


        			
    }


    void OnEnable()
    {

        EventManager.StartListening<PlayerCollisionEvent, Vector3, Material>(playerCollisionEventListener);
        EventManager.StartListening<PlayerLandsEvent, Vector3>(playerLandsEventListener);
        EventManager.StartListening<PlayerFootEvent, Vector3, Material>(playerFootEventListener);
        EventManager.StartListening<NpcEvent, Vector3, int>(npcEventListener);
        EventManager.StartListening<CollectibleEvent, Vector3>(collectibleEventListener);
        EventManager.StartListening<EnemyHitEvent, Vector3>(enemyHitEventListener);
        EventManager.StartListening<PlayerHurtEvent, Vector3>(playerHurtEventListener);
    }

    void OnDisable()
    {

        EventManager.StopListening<PlayerCollisionEvent, Vector3, Material>(playerCollisionEventListener);
        EventManager.StopListening<PlayerLandsEvent, Vector3>(playerLandsEventListener);
        EventManager.StopListening<PlayerFootEvent, Vector3, Material>(playerFootEventListener);
        EventManager.StopListening<NpcEvent, Vector3, int>(npcEventListener);
        EventManager.StopListening<CollectibleEvent, Vector3>(collectibleEventListener);
        EventManager.StopListening<EnemyHitEvent, Vector3>(enemyHitEventListener);
        EventManager.StopListening<PlayerHurtEvent, Vector3>(playerHurtEventListener);
    }


	
    // Update is called once per frame
    void Update()
    {
    }


 

    void playerCollisionEventHandler(Vector3 worldPos, Material mat)
    {
        //AudioSource.PlayClipAtPoint(this.boxAudio, worldPos);
        if (mat.ToString().Contains("Unlit"))
        {
            AudioSource.PlayClipAtPoint(this.mooAudio, worldPos);
        }
        if (mat.ToString().Contains("crate"))
        {
            AudioSource.PlayClipAtPoint(this.crateAudio, worldPos);
        }
        if (mat.ToString().Contains("Plywood"))
        {
            AudioSource.PlayClipAtPoint(this.woodAudio, worldPos);
        }
        if (mat.ToString().Contains("Plywood"))
        {
            AudioSource.PlayClipAtPoint(this.woodAudio, worldPos);
        }
        if (mat.ToString().Contains("asdf"))
        {
            AudioSource.PlayClipAtPoint(this.ballAudio, worldPos);
        }
        if (mat.ToString().Contains("Cone"))
        {
            AudioSource.PlayClipAtPoint(this.honkAudio, worldPos);
        }
    }

    void playerLandsEventHandler(Vector3 worldPos)
    {
        AudioSource.PlayClipAtPoint(this.playerLandsAudio, worldPos);
    }

    void playerFootEventHandler(Vector3 worldPos, Material mat)
    {

        //Debug.Log("Player foot event");

        //AudioSource.PlayClipAtPoint(this.playerFootAudio, worldPos);


        //print(mat.ToString());
        if (mat == null) {
            AudioSource.PlayClipAtPoint(this.snowAudio, worldPos);
        }
        else if (mat.ToString().Contains("Snow"))
        {
            //particles.startColor = Color.white;
            AudioSource.PlayClipAtPoint(this.snowAudio, worldPos);
        }
        else if (mat.ToString().Contains("Grass"))
        {
            //particles.startColor = Color.green;
            AudioSource.PlayClipAtPoint(this.grassAudio, worldPos);
        }
        else
        {
            //particles.startColor = Color.red;
            AudioSource.PlayClipAtPoint(this.grassAudio, worldPos);
        }
        //particles.startColor = mat;
        //particles.Play();
    }

    void collectibleEventHandler(Vector3 worldPos)
    {
        AudioSource.PlayClipAtPoint(this.collectibleAudio, worldPos);
    }

    void enemyHitEventHandler(Vector3 worldPos)
    {
        AudioSource.PlayClipAtPoint(this.enemyHitAudio, worldPos);
    }

    void playerHurtEventHandler(Vector3 worldPos)
    {
        AudioSource.PlayClipAtPoint(this.playerHurtAudio, worldPos);
    }

    void npcEventHandler(Vector3 worldPos, int type)
    {
        if (type == 0)
        {
            AudioSource.PlayClipAtPoint(this.honkAudio, worldPos);
        }
        if (type == 1)
        {
            AudioSource.PlayClipAtPoint(this.woodAudio, worldPos);
        }
        if (type == 2)
        {
            AudioSource.PlayClipAtPoint(this.ballAudio, worldPos);
        }
    }

}
