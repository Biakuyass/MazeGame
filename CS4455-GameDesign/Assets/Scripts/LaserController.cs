using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Timers;

#if UNITY_EDITOR
using UnityEditor;
#endif

//require some things the bot control needs
public class LaserController : MonoBehaviour
{
    public AudioClip playerHitAudio;
    public AudioClip playerHurtAudio;

    public Rigidbody player;
    private Animator anim;

    private LineRenderer laserLine;

    private Vector3 LASER_START;
    private Vector3 LASER_END;

    private Vector3 laserDirection;

    private float laserLength;
    private float pushForce = 500f;
    private bool canBeHitByLaser = true;

    private bool isHittingCrystal = false;

    private PulsingLightScript lightScript;

    void Awake()
    {
        
    }


    // Use this for initialization
    void Start()
    {
        laserLine = GetComponent<LineRenderer>();
        LASER_START = laserLine.GetPosition(0);
        LASER_END = GetComponentsInChildren<Transform>()[1].localPosition;

        laserDirection = (LASER_END - LASER_START).normalized;
        laserLength = (LASER_END - LASER_START).magnitude;

        lightScript = GameObject.Find("crystal_light").GetComponent<PulsingLightScript>();

        anim = player.GetComponent<Animator>();
    }

    void Update()
    {
    }

    //Update whenever physics updates with FixedUpdate()
    //Updating the animator here should coincide with "Animate Physics"
    //setting in Animator component under the Inspector
    void FixedUpdate()
    {
        ShootLaser();
    }

    void ShootLaser() {
        const float ZBufFix = 0.01f;
        const float edgeSize = 0.2f;
        Color col = Color.black;

        GameObject laserStart = gameObject;

        float rayOriginOffset = 0f;
        float rayDepth = laserLength; //how far will we look?
        float totalRayLen = rayOriginOffset + rayDepth;

        float reflLength = 500f;

        Ray ray = new Ray(laserStart.transform.position + Vector3.forward * rayOriginOffset, laserDirection);
        RaycastHit hit;
        Vector3 endPoint, reflPoint;

        if (Physics.Raycast(ray, out hit, totalRayLen))
        {
            endPoint = (hit.point - laserStart.transform.position) * 5f;

            //Vector3 diff = (hit.point - laserStart.transform.position)*5;
            //endPoint = diff;
            reflPoint = LASER_START;

            if (hit.collider.gameObject.CompareTag("Player"))
            {
                HitByLaser(hit);
                showCrystalLight(false);
            }
            else if (hit.collider.gameObject.name.Contains("box_piece") || hit.collider.gameObject.name.Contains("reflector"))
            {
                reflPoint = endPoint + hit.normal * reflLength;

                Ray rayRefl = new Ray(hit.point, hit.normal);
                RaycastHit hitRefl;

                //Debug.DrawLine(rayRefl.origin, rayRefl.origin + rayRefl.direction * totalRayLen, Color.green);

                if (Physics.Raycast(rayRefl, out hitRefl, totalRayLen))
                {
                    if (hitRefl.collider.gameObject.name.Contains("crystal_go"))
                    {
                        reflPoint = hitRefl.collider.transform.position;
                        showCrystalLight(true);
                    }
                    else
                    {
                        showCrystalLight(false);
                    }
                }
                else {
                    showCrystalLight(false);
                }
            }
            else if (hit.collider.gameObject.name.Contains("crystal")) {
                reflPoint = hit.collider.gameObject.transform.position;
                showCrystalLight(true);
            } else
            {
                showCrystalLight(false);
            }

        }
        else {
            endPoint = LASER_END;
            reflPoint = LASER_START;
            showCrystalLight(false);
        }

        laserLine.SetPosition(1, endPoint);
        laserLine.SetPosition(2, reflPoint);
        //Debug.DrawLine(ray.origin, ray.origin + ray.direction * totalRayLen, Color.green);

        /*
        Debug.DrawRay(hit.point + Vector3.forward * ZBufFix, Vector3.forward * edgeSize, col);
        Debug.DrawRay(hit.point + Vector3.forward * ZBufFix, Vector3.left * edgeSize, col);
        Debug.DrawRay(hit.point + Vector3.forward * ZBufFix, Vector3.right * edgeSize, col);
        Debug.DrawRay(hit.point + Vector3.forward * ZBufFix, Vector3.back * edgeSize, col);
        */
    }

    void showCrystalLight(bool e) {
        if (this.isHittingCrystal) {
            lightScript.setLight(e);
        }

        this.isHittingCrystal = e;
    }

    void HitByLaser(RaycastHit hit) {
        Debug.Log("Hit by laser");

        if (!canBeHitByLaser) {
            return;
        }
        canBeHitByLaser = false;

        // Push back from laser
        float playerSpeed = anim.GetFloat("vely");
        Vector3 playerForward = player.transform.forward;
        Vector3 pushDirection = new Vector3(-playerForward.x * playerSpeed * pushForce * 1.5f, 0, playerForward.z * playerSpeed / 2f * pushForce);
        player.GetComponent<Rigidbody>().AddForce(pushDirection, ForceMode.Impulse);
        if (player.GetComponent<YBotSimpleControlScript>().getHeldObject().getGameObject() != null) {
            Debug.Log(player.GetComponent<YBotSimpleControlScript>().getHeldObject().getGameObject());

            float scaledForce = player.GetComponent<YBotSimpleControlScript>().getHeldObject().getGameObject().GetComponent<Rigidbody>().mass / player.GetComponent<Rigidbody>().mass;
            player.GetComponent<YBotSimpleControlScript>().getHeldObject().getGameObject().GetComponent<Rigidbody>().AddForce(pushDirection * scaledForce, ForceMode.Impulse);
            player.GetComponent<YBotSimpleControlScript>().EndDrag();
        }

        // Damage from laser
        player.GetComponent<PlayerHealthPoint>().Hurt();
        EventManager.TriggerEvent<PlayerHurtEvent, Vector3>(hit.point);


        Debug.Log("Hurt and pushed back");

        Invoke("allowHitting", .15f);
    }

    void allowHitting() {
        Debug.Log("Allow hitting");
        canBeHitByLaser = true;
    }

    void DeadByLaser(Vector3 hit) {
        AudioSource.PlayClipAtPoint(playerHitAudio, hit);
        AudioSource.PlayClipAtPoint(playerHurtAudio, hit);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
