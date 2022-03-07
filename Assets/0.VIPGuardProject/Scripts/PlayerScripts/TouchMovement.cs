using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchMovement : MonoBehaviour
{
    public float forwardSpeed;
    private float _lastFrameFingerPositionX;
    private float _moveFactorX;
    public float MoveFactorX => _moveFactorX;

    [SerializeField] private float swerveSpeed = 0.5f;
    [SerializeField] private float maxSwerveAmount = 1f;

    CharacterController controller;
    Vector3 move;
    public bool useTouch;
    public bool useDeltaTouch;
    public Toggle toggle;
    float swerveAmount;

    GuardMovement guardManager;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        guardManager = GetComponent<GuardMovement>();
      
    }

    private void OnEnable()
    {
        swerveAmount = 0;
    }
    private void OnDisable()
    {
        swerveAmount = 0;
    }

    void Update()
    {
        useDeltaTouch = toggle.isOn;
        //If NOT stoping
            if (!useDeltaTouch)
            {   //On Down
                if (Input.GetMouseButtonDown(0))
                {
                    _lastFrameFingerPositionX = Input.mousePosition.x;
                } //On Move
                else if (Input.GetMouseButton(0))
                {
                    _moveFactorX = Input.mousePosition.x - _lastFrameFingerPositionX;
                    _lastFrameFingerPositionX = Input.mousePosition.x;
                }//OnEnd
                else if (Input.GetMouseButtonUp(0))
                {
                    _moveFactorX = 0f;
                }
                swerveAmount = Time.deltaTime * swerveSpeed * MoveFactorX;
                swerveAmount = Mathf.Clamp(swerveAmount, -maxSwerveAmount, maxSwerveAmount);
                controller.Move(new Vector3(swerveAmount, 0, forwardSpeed * Time.deltaTime));
            }
            if (useDeltaTouch)
            {
                //Count imput position on X axis
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    //On Down
                    if (touch.phase == TouchPhase.Began)
                    {
                        _lastFrameFingerPositionX = touch.position.x;
                    } //On Move or stay
                    else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        _moveFactorX = touch.position.x - _lastFrameFingerPositionX;
                        _lastFrameFingerPositionX = touch.position.x;
                    }//OnEnd
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        _moveFactorX = 0f;
                    }

                   
                    swerveAmount = Time.deltaTime * swerveSpeed * MoveFactorX;
                    swerveAmount = Mathf.Clamp(swerveAmount, -maxSwerveAmount, maxSwerveAmount);
                    controller.Move(new Vector3(swerveAmount, 0, 0));
                }
                controller.Move(new Vector3(0, 0, forwardSpeed * Time.deltaTime));
            }
    }
}
