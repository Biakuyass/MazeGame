using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParticleEventManager : MonoBehaviour
{
    private UnityAction<Vector3, Material> playerFootEventListener;
    public ParticleSystem particles;
	private ParticleSystem tempFootParticle;

    void Awake()
    {
        //playerFootEventListener = new UnityAction<Vector3>(playerFootEventHandler);
        //playerFootEventListener = new UnityAction<Vector3, Material>(playerFootEventHandler);
    }


    // Use this for initialization
    void Start()
    {


        			
    }


    void OnEnable()
    {
        //EventManager.StartListening<PlayerFootEvent, Vector3>(playerFootEventListener);
        //EventManager.StartListening<PlayerFootEvent, Vector3, Material>(playerFootEventListener);
    }

    void OnDisable()
    {
        //EventManager.StopListening<PlayerFootEvent, Vector3>(playerFootEventListener);
        //EventManager.StopListening<PlayerFootEvent, Vector3, Material>(playerFootEventListener);
    }


	
    // Update is called once per frame
    void Update()
    {
    }

    //void playerFootEventHandler(Vector3 worldPos, Material mat)
    //{
    //    //tempFootParticle = Instantiate(particles, particles.transform.position, Quaternion.identity) as ParticleSystem;
    //    //tempFootParticle.Play();
    //   print(mat.ToString());
    //    if (mat.ToString().Contains("Default")) {
    //        print ("!");
    //    }
    //    //particles.startColor = mat;
    //    particles.Play();
    //}
}
