using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class FanInteraction : MonoBehaviour
{
    public bool touched;
    public MMFeedbacks slapFeed;
    private Fan fanScript;


    private void Start()
    {
        touched = false;
        fanScript = GetComponentInParent<Fan>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(fanScript.currentFanType == Fan.FanTypes.GIMMEFIVE)
        {
            if (other.gameObject.CompareTag("FiveHand"))
            {
                if (touched)
                {
                    return;
                }

                slapFeed.PlayFeedbacks();
                //Debug.Log("Touch");
                GetComponentInParent<Animator>().SetTrigger("Five");

                StartCoroutine(GuardGM.instance.AddLikes(
                     1, //Count
                     GuardGM.instance.likePrefab, //Prefab
                     GuardGM.instance.guest.position + new Vector3(Random.Range(-0.25f, 0.25f), -0.1f, -0.4f),  //Position
                     GuardGM.instance.fxOffset,  //Offset
                     GuardGM.instance.likesLifeTime //Lifetime
                 ));

                touched = true;
                GetComponentInParent<Fan>().BecomeFolower();

            }

        }



    }
}
