using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    //public float MotorForce, SteerForce, BrakeForce;
    //public WheelCollider FR_L_Wheel, FR_R_Wheel, RE_L_Wheel, RE_R_Wheel, R2E_L_Wheel, R2E_R_Wheel;
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    float v = Input.GetAxis("Vertical") * MotorForce;
    //    float h = Input.GetAxis("Horizontal") * SteerForce;


    //    RE_L_Wheel.motorTorque = v;
    //    RE_R_Wheel.motorTorque = v;
    //    R2E_L_Wheel.motorTorque = v;
    //    R2E_R_Wheel.motorTorque = v;

    //    FR_L_Wheel.steerAngle = h;
    //    FR_R_Wheel.steerAngle = h;

    //    if (Input.GetKey(KeyCode.Space))
    //    {
    //        RE_L_Wheel.brakeTorque = BrakeForce;
    //        RE_R_Wheel.brakeTorque = BrakeForce;
    //        R2E_L_Wheel.brakeTorque = BrakeForce;
    //        R2E_R_Wheel.brakeTorque = BrakeForce;
    //    }
    //    if (Input.GetKeyUp(KeyCode.Space))
    //    {
    //        RE_L_Wheel.brakeTorque = 0;
    //        RE_R_Wheel.brakeTorque = 0;
    //        R2E_L_Wheel.brakeTorque = 0;
    //        R2E_R_Wheel.brakeTorque = 0;
    //    }

    //    if (Input.GetAxis("Vertical") == 0)
    //    {
    //        RE_L_Wheel.brakeTorque = BrakeForce;
    //        RE_R_Wheel.brakeTorque = BrakeForce;
    //        R2E_L_Wheel.brakeTorque = BrakeForce;
    //        R2E_R_Wheel.brakeTorque = BrakeForce;
    //    }
    //    else
    //    {
    //        RE_L_Wheel.brakeTorque = 0;
    //        RE_R_Wheel.brakeTorque = 0;
    //        R2E_L_Wheel.brakeTorque = 0;
    //        R2E_R_Wheel.brakeTorque = 0;
    //    }


    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.layer == 6) Destroy(other.gameObject);
    //}

    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;
    private int timer;
    private Collider targetedWeed;

    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;
    [SerializeField] private int life = 20;
    [SerializeField] private int destroyingTime = 100;
    [SerializeField] private int removalProbability = 80; // %

    void Start() {
        timer = destroyingTime;
    }
    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        //UpdateWheels();
        if (timer<destroyingTime) {
            timer++;
            if (timer==destroyingTime) DestroyWeed();
        }
        //Debug.Log(timer.ToString());
    }


    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        if (life>0) {
            frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
            frontRightWheelCollider.motorTorque = verticalInput * motorForce;
            currentbreakForce = (isBreaking || (timer<destroyingTime))? breakForce:0f;
            ApplyBreaking();
        }
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer==6 && UnityEngine.Random.Range(0, 100)<removalProbability && timer==destroyingTime) {
            timer = 0;
            targetedWeed = other;
        }
    }

    private void DestroyWeed() {
        Destroy(targetedWeed.gameObject);
        Debug.Log("Remaining life points: "+(--life).ToString());
    }
}