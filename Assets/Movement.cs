using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MoreMountains.Feedbacks;
public class Movement : MonoBehaviour
{
    private Animator anim;

    public Transform ground; //The ground player stays on
    public Transform ceiling; //The ground player stays on
    public bool isCeiling; //If above the player is ceiling





    Tween jumpTween;
    Tween fallTween;


    //Raycasting 
    public LayerMask layer;
    [Space]
    //Jump
    [SerializeField] float lowJump;
    [SerializeField] float highJump;
    [SerializeField] float ultraJump;

    [SerializeField] Vector3 jumpStartPos;
    [SerializeField] Vector3 jumpPower;
    [SerializeField] Vector3 jumpTo;
    [SerializeField] float jumpDuration;
    [SerializeField] Ease jumpEase;
    [SerializeField] MMFeedbacks jumpFeedBack;

    //After Jump
    [SerializeField] Vector3 diffrence;

    //Platform
    [SerializeField] Vector3 hitPos;

  

    [Space]
    //Falling
    [SerializeField] float fallDuration;
    [SerializeField] Vector3 fallPos;
    [SerializeField] Ease fallEase;

  

    //Calculation
    public bool fall;
    [SerializeField] float distanceToFallPos;
    [SerializeField] float distanceToGround;
    [SerializeField] float distanceToTarget;

    [SerializeField] float feedBackOffset;
    [SerializeField] float targetOffset;
    [SerializeField] float groundOffset;



    private void Start()
    {
        anim = GetComponent<Animator>();
        Jump();
    }


    private void Update()
    {
        //Always check if something is above the player
        CheckCeiling();
    }

    void Jump()
    {
        //Start jumping by tween
             jumpTween = transform.DOMove(jumpTo, jumpDuration)
            .SetEase(jumpEase)
            .OnStart(OnStartJumping)
            .OnUpdate(OnJumpUpdate)
            .OnComplete(OnJumpComplete);
    }
    void Fall()
    {
      
        Vector3 GroundPos = new Vector3(transform.position.x, ground.position.y + groundOffset, transform.position.z);
        fallTween = transform.DOMove(GroundPos, fallDuration)
            .SetEase(fallEase)
            .OnStart(OnStartFalling)
            .OnUpdate(OnFallUpdate)
            .OnComplete(OnCompleteFalling);
    }
    #region JumpingState

    void OnStartJumping()
    {
        //Set feedback world position to a curent fall position
        jumpFeedBack.gameObject.transform.position = new Vector3(jumpFeedBack.gameObject.transform.position.x, ground.transform.position.y + groundOffset/2, jumpFeedBack.gameObject.transform.position.z);
        fall = false;
        jumpFeedBack.PlayFeedbacks();
    }
    void OnJumpUpdate()
    {

    }
    void OnJumpComplete()
    {
        CheckGround();
        Fall();
    }

    #endregion JumpingState

    #region FallingState
    void OnStartFalling()
    {
        anim.SetBool("Jump", false);
        fall = true;
    }
    void OnFallUpdate()
    {
       
    }
    void OnCompleteFalling()
    {
        RestartJump();
    }
    #endregion FallingState

    public void JumpFeedBack()
    {
        jumpFeedBack.PlayFeedbacks();
    }
    public void CheckGround()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layer))
        {
            //from = hit.point;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
            ground = hit.transform;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
    }
    public void RestartJump()
    {
        if (fall)
        {
            Debug.Log("RestartJump");
            fallTween.Kill();
            fall = false;
            anim.SetBool("Jump", true);
            jumpTween.Kill();
            Jump();
        }
    }
    public void CheckCeiling()
    {

        RaycastHit ceilingHit;

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out ceilingHit, 10f, layer))
        {
            //from = hit.point;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * ceilingHit.distance, Color.green);
            Debug.Log("Did Hit");

            ceiling = ceilingHit.transform;
            isCeiling = true;
        }
        else
        {
            isCeiling = false;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 10f, Color.red);
            Debug.Log("Did not Hit");
        }

        //Recalculate jumpPower if Ceiling is above the player
        if (isCeiling)
        {
            jumpTo = new Vector3(transform.position.x, ground.position.y + groundOffset + lowJump, transform.position.z);
        }
        else
        {
            jumpTo = new Vector3(transform.position.x, ground.position.y + groundOffset + highJump, transform.position.z);
        }
    }



}
