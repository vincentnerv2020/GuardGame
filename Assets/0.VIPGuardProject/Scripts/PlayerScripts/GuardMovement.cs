using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class GuardMovement : MonoBehaviour
{
    public Animator anim;
    public CharacterController controller;
    public bool groundedPlayer;
    public bool run;
    public bool stop;
    public float kickDelay;

    public float rayLength;
    public LayerMask layer;

    public Transform rayStart;

    public bool isAttacking;
    Fan fanScript;
    public float animationDelay;
    public float distanceToAttack;
    public MMFeedbacks kick;
    public Transform hand;
    private TouchMovement movementScript;


    private void Awake()
    {
        stop = false;
        controller = gameObject.GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        movementScript = GetComponent<TouchMovement>();
    }

    void Update()
    {
        if (stop)
        {
            movementScript.enabled = false;
        }
        else
        {
            movementScript.enabled = true;
        }
        if(Time.frameCount % GuardGM.instance.interval == 0)
        {
            CheckAngryInFront();
        }
       
    }
    void CheckAngryInFront()
    {
        if (isAttacking)
        {
            return;
        }
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(rayStart.position, transform.TransformDirection(Vector3.forward), out hit, rayLength, layer))
        {
            if (isAttacking)
            {
                return;
            }

            if (hit.collider.GetComponent<Fan>() != null)
            {
                fanScript = hit.collider.GetComponent<Fan>();
            }
            else
            {
                fanScript = hit.collider.GetComponentInParent<Fan>();
            }


            if(fanScript.currentFanType == Fan.FanTypes.ANGRY && fanScript.isAlive)
            {
                if (hit.distance <= distanceToAttack)
                {
                    StartCoroutine(Kick());
                }
            }

           // Debug.DrawRay(rayStart.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
           // Debug.Log("Did Hit");
        }
        else
        {
           // Debug.DrawRay(rayStart.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.white);
            //Debug.Log("Did not Hit");
        }
    }
    public IEnumerator Kick()
    {
        //Debug.Log("Kick");
        isAttacking = true;
        stop = true;
        anim.SetTrigger("Kick");
        yield return new WaitForSeconds(animationDelay);
        if (fanScript != null)
        {
            kick.PlayFeedbacks();
            fanScript.ToogleRagdoll(true);
            fanScript.spine.AddForce(new Vector3(0, 250f, 700f), ForceMode.Impulse);
            GameActions.OnFanKicked();
        }
        yield return new WaitForSeconds(kickDelay);
        stop = false;
        isAttacking = false;
    }


   

}
