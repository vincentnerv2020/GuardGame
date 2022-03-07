using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using DG.Tweening;
public class Paparazzi : MonoBehaviour
{
    public Transform cam;
    public Transform from;
    public Transform to;
    public MMFeedbacks cameraFlash;


    private void OnEnable()
    {
        GameActions.OnPhotographMakePhoto += CameraShot;
    }
    private void OnDisable()
    {
        GameActions.OnPhotographMakePhoto -= CameraShot;
    }
    private void Start()
    {
        EndPhotoSession();
    }
    public void CameraShot()
    {            

        cameraFlash.PlayFeedbacks();

        StartCoroutine(GuardGM.instance.AddLikes(
                10,
                GuardGM.instance.likePrefab,
                GuardGM.instance.guest.transform.position + new Vector3(-1.5f,1.5f, Random.Range(1f,3.5f)),
                GuardGM.instance.fxOffset,
                GuardGM.instance.likesLifeTime
                ));

        GuardGM.instance.ChangePoseFeedBack.PlayFeedbacks();


    }

    public void PrepareToShot()
    {
        cam.DOMove(to.position, 0.5f);
        cam.DORotateQuaternion(to.rotation, 0.5f);
    }

    public void EndPhotoSession()
    {
        cam.DOMove(from.position, 0.5f);
        cam.DORotateQuaternion(from.rotation, 0.5f);
    }
}
