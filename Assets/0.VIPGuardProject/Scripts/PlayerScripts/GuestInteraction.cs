using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
using MoreMountains.Feedbacks;

public class GuestInteraction : MonoBehaviour
{
    //References
    public Transform guard;
    private AgentFolower agentScript;
    public GuardMovement guardMovement;
    public Vector3 guardOffsetPosition;
    Paparazzi paparazzi;

    public float fallTime;
    public float delayBeforeStand;
    public float standUpSpeed;

    //Photosession settings
    public Camera paparazziCam;
    public CinemachineVirtualCamera guestCM;
    public CinemachineVirtualCamera guardCM;
    public CinemachineVirtualCamera finishCM;
    public CinemachineVirtualCamera congratsCM;
    public CinemachineVirtualCamera startCM;

    public GameObject posePanel;

    //Hand rig settings
    public Rig rigLayer;
    public TwoBoneIKConstraint leftHand;
    public TwoBoneIKConstraint rightHand;

    public Collider[] handTriggers;
   
    Tween floating;

    public float detectionRadius;
    public LayerMask layer;


    public MMFeedbacks congratsFeedBack;

    public bool touchTriggers;

   public  bool enterRightZone;
   public  bool enterLefttZone;


    bool catched = false;

    [SerializeField] float distanceToCheck;

    private void Awake()
    {
        agentScript = GetComponent<AgentFolower>();
        
    }
    private void Start()
    {
        startCM.Priority = 1;
    }

    private void OnTriggerEnter(Collider other)
    {

        touchTriggers = true;
        if (other.gameObject.CompareTag("PhotoZone"))
        {
            

            if (paparazzi == null)
            {
                PauseGuardPhotosesion();
                paparazzi = FindObjectOfType<Paparazzi>();
                paparazzi.PrepareToShot();
                guardCM.Priority = 5;
                guestCM.Priority = 10;
                paparazziCam.enabled = true;
                posePanel.SetActive(true);
                AgentFolower af = GetComponent<AgentFolower>();
                af.photoSesison = true;
                
            }
            else
            {
                return;
            }
           
        }
        if (other.gameObject.CompareTag("RightZone"))
        {
            if (!enterRightZone)
            {
                enterRightZone = true;
                foreach (Collider handTrigger in handTriggers)
                {
                    handTrigger.enabled = true;
                }
                DOTween.To(() => rightHand.weight, x => rightHand.weight = x, 1, 0.5f);
            }
          

        }
        else if (other.gameObject.CompareTag("LeftZone"))
        {
            if (!enterLefttZone)
            {
                enterLefttZone = true;
                foreach (Collider handTrigger in handTriggers)
                {
                    handTrigger.enabled = true;
                }
                DOTween.To(() => leftHand.weight, x => leftHand.weight = x, 1, 0.5f);
            }
        }
        if (other.gameObject.CompareTag("Barier"))
        {

            //Call TheAction
            //GameActions.OnPlayerTrapped();
            StartCoroutine(Fall());
        }
        if (other.gameObject.CompareTag("Money"))
        {
            Destroy(other.gameObject);
            GuardGM.instance.ExtraMoney(Random.Range(150,350));
        }
        if (other.gameObject.CompareTag("Turd"))
        {
       
            //Call TheAction
           // GameActions.OnPlayerTrapped();

            Destroy(other.gameObject);
            GuardGM.instance.TurdFeedBack.PlayFeedbacks();
            CheckCrowd();
        }
        if (other.gameObject.CompareTag("Finish"))
        {
            PauseGuard();
            finishCM.Priority = 30;
            //SetUpCountLikesAnimation
            GuardGM.instance.StartCoroutine("FinishGame");
        }
        if (other.gameObject.CompareTag("Vehicle"))
        {
            transform.DOLookAt(congratsCM.transform.position, 1f, AxisConstraint.Y, Vector3.up).OnComplete(OnReachTheCar);

            congratsCM.Priority = 35;
            congratsFeedBack.PlayFeedbacks();

        }
        if (other.gameObject.CompareTag("Angry"))
        {
            if (!catched)
            {
                catched = true;
                PauseGuard();
                CatchedState();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        touchTriggers = false;
        if (other.gameObject.CompareTag("RightZone"))
        {
            if (enterRightZone)
            {
                enterRightZone = false;
                foreach (Collider handTrigger in handTriggers)
                {
                    handTrigger.enabled = false;
                }
                DOTween.To(() => rightHand.weight, x => rightHand.weight = x, 0, 0.5f);
            }
           


        }else if (other.gameObject.CompareTag("LeftZone"))
        {
            if (enterLefttZone)
            {
                enterLefttZone = false;
                foreach (Collider handTrigger in handTriggers)
                {
                    handTrigger.enabled = false;
                }
                DOTween.To(() => leftHand.weight, x => leftHand.weight = x, 0, 0.5f);
            }
           
        }


        if (other.gameObject.CompareTag("PhotoZone"))
        {
            if (paparazzi != null)
            {
                paparazzi.EndPhotoSession();
            }
            
        }
      
    }

    void PauseGuardPhotosesion()
    {
       
       // Debug.Log("Move guard right side");
        guard.DOMove(guard.position+ guardOffsetPosition, 0.4f);


        guardMovement.stop = true;
        guardMovement.anim.SetBool("Waiting", true);
    }
    void PauseGuard()
    {
        guardMovement.anim.SetBool("Waiting", true);
        guardMovement.stop = true;
    }
    IEnumerator Fall()
    {
       
        agentScript.guestAnim.SetTrigger("Fall");
        
        yield return new WaitForSeconds(fallTime);
        PauseGuard();
        agentScript.agent.speed = 0;
        GuardGM.instance.FallFeedBack.PlayFeedbacks();
       
        CheckCrowd();
        yield return new WaitForSeconds(delayBeforeStand);
        agentScript.guestAnim.SetTrigger("Stand");
        yield return new WaitForSeconds(standUpSpeed);
        guardMovement.stop = false;
        guardMovement.anim.SetBool("Waiting", false);
        yield return new WaitForSeconds(0.25f);
        agentScript.agent.speed = GuardLM.LMinstance.agentSpeed;
      

    }
    public void ChangePose(string poseName)
    {
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger(poseName);
    }
    void OnReachTheCar()
    {
        GuardUI.UIinstance.ShowButton(1);
    }
    public void EndPhotoSession()
   {

        guardMovement.anim.SetBool("Waiting", false);
        guardMovement.stop = false;
        guardCM.Priority = 10;
        guestCM.Priority = 5;
        posePanel.SetActive(false);
        AgentFolower af = GetComponent<AgentFolower>();
        af.photoSesison = false;
        af.guestAnim.SetBool("Continue", true);
        
    }
    public void CheckCrowd()
    {
        
        Collider[] Crowd = Physics.OverlapSphere(transform.position+Vector3.forward * distanceToCheck, detectionRadius, layer);

        for (int i = 0; i < Crowd.Length; i++)
        {
        
            if (Crowd[i].GetComponentInParent<Fan>().currentFanType == Fan.FanTypes.ANGRY)
            {
                //Debug.Log("I`m angry already!");
            }
            else
            {
                StartCoroutine(GuardGM.instance.AddDislikes(
              1,
              GuardGM.instance.dislikePrefab,
              Crowd[i].transform.position,
              GuardGM.instance.fxOffset,
              GuardGM.instance.likesLifeTime
              ));

                Crowd[i].GetComponentInParent<Fan>().BecomeAngry();
            }

          
        }
      
    }
    public void CatchedState()
    {
        agentScript.agent.speed = 0;
        agentScript.guestAnim.SetTrigger("Catched");
        if (GuardGM.instance !=null)
        {
            GuardGM.instance.GameOver(true);
        }
       
    }
}
