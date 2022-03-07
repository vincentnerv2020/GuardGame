using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
   public enum FanTypes {CROWD, GIMMEFIVE, GIMMESIGN, ANGRY}
   public FanTypes currentFanType;

    public Animator fanAnim;
    public Material[] fanMats;

    public bool rightSide;
    
    SkinnedMeshRenderer smr;

    public LayerMask angryLayer;

    public Rigidbody spine;
    public Vector3 force;

    public bool isAlive;
    public Transform gloveR;
    public Transform gloveL;

    [SerializeField] Rigidbody[] ragdollBodies;
    [SerializeField] Collider[] ragdollColliders;

    public Collider angryFanTrigger;
    public Collider rightSideTriggers;
    public Collider leftSideTriggers;
    public Collider head;

    public Rigidbody triggerBody;

    private void OnEnable()
    {
        GameActions.OnPlayerTrapped += FanReaction;
    }
    private void OnDisable()
    {
        GameActions.OnPlayerTrapped -= FanReaction;
    }
    public void Start()
    {
        FanInitialization();
    }

    void FanReaction()
    {
        //Debug.Log("Oh...");
    }
    void FanInitialization()
    {
        isAlive = true;
        smr = GetComponentInChildren<SkinnedMeshRenderer>();
        switch (currentFanType)
        {
            case FanTypes.CROWD:
                {
                    gloveR.gameObject.SetActive(false);
                    gloveL.gameObject.SetActive(false);
                    smr.material = fanMats[0];
                    fanAnim.SetBool("Crowd", true);
                    break;
                }
            case FanTypes.GIMMEFIVE:
                {
                    if (rightSide)
                    {
                        gloveR.gameObject.SetActive(true);
                    }
                    else
                    {
                        gloveL.gameObject.SetActive(true);
                    }
                   
                    smr.material = fanMats[1];
                    fanAnim.SetBool("GimmeFive", true);
                    fanAnim.SetBool("RightSide", rightSide);
                    break;
                }
            case FanTypes.GIMMESIGN:
                {
                    gloveR.gameObject.SetActive(false);
                    gloveL.gameObject.SetActive(false);
                    smr.material = fanMats[1];
                    fanAnim.SetBool("GimmeSign", true);
                    break;
                }
            case FanTypes.ANGRY:
                {
                    gameObject.tag = "Angry";
                    gloveR.gameObject.SetActive(false);
                    gloveL.gameObject.SetActive(false);
                    smr.material = fanMats[2];
                    fanAnim.SetBool("Angry", true);
                    ragdollBodies = GetComponentsInChildren<Rigidbody>();
                    ragdollColliders = GetComponentsInChildren<Collider>();
                    ToogleRagdoll(false);
                    break;

                }
        }

       

        if (currentFanType == FanTypes.ANGRY)
        {
            angryFanTrigger.enabled = true;
            triggerBody.isKinematic = true;
        }
        if (currentFanType == Fan.FanTypes.GIMMEFIVE)
        {
            if (rightSide)
            {
                rightSideTriggers.enabled = true;
                leftSideTriggers.enabled = false;
            }
            else
            {

                rightSideTriggers.enabled = false;
                leftSideTriggers.enabled = true;
            }
        }
        head.enabled = true;
    }

    public void BecomeFolower()
    {
        smr.material = fanMats[3];
    }

    public void BecomeAngry()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach(Collider col in colliders)
        {
            col.enabled = false;
        }
        gloveR.gameObject.SetActive(false);
        gloveL.gameObject.SetActive(false);
        smr.material = fanMats[2];
        fanAnim.SetTrigger("Point");
        fanAnim.SetBool("Angry", true);
        currentFanType = Fan.FanTypes.ANGRY;
    }


    public void ToogleRagdoll(bool state)
    {
        isAlive = !state;
        
        fanAnim.enabled = !state;
        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = !state;
        }
        foreach (Collider col in ragdollColliders)
        {
            col.enabled = state;
        }
    }
}
