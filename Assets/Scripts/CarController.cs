using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;

    private int timer;
      
    private Collider targetedWeed;

    public float numberOfRays = 17;
    public float angle = 180;
    public float rayRange = 10;
    public float avoidVelocity = 10.0f;

    // This is our direction we're travelling in.
    public Vector3 direction = Vector3.forward;
    public float velocity = 5.0f;
    public float velocityValue;

    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;
    // 
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;
    //
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;
    // 
    [SerializeField] private int life = 20;
    [SerializeField] private int destroyingTime = 100;
    [SerializeField] private int removalProbability = 80;

   

    void Start() {
        timer = destroyingTime;
    }

    void Update() {
        HandleMotor();

        var deltaPosition = velocityValue * direction;
        for(int i = 0; i < numberOfRays; ++i) 
        {
            var rotation = this.transform.rotation;
            var rotationMod = Quaternion.AngleAxis((i / ((float)numberOfRays - 1)) * angle * 2 - angle, this.transform.up + new Vector3(0, 0.25f, 0));
            var avoidDirection = rotation * rotationMod * direction;
            var ray = new Ray(this.transform.position + new Vector3(0, 0.25f, 0), avoidDirection * rayRange);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, rayRange))
            {
                Debug.Log("hitInfo: " + hitInfo.collider.gameObject.name);
                if (hitInfo.collider.gameObject.name == "Rock")
                {
                    Debug.Log("Inside of");
                    deltaPosition -= (1.0f / numberOfRays) * velocityValue * avoidDirection * 10.0f; // 10.0f - scale factor (selected individually)
                }
            }
            else
            {
                deltaPosition += (1.0f / numberOfRays) * velocityValue * avoidDirection;

            }
        }

        transform.Translate(deltaPosition * Time.deltaTime);
    }
    private void FixedUpdate()
    {
        // GetInput();
        // HandleMotor();
        // HandleSteering();
        //UpdateWheels();

        if (timer < destroyingTime) {
            timer++;
            if (timer == destroyingTime) 
            {
                DestroyWeed();
            }    
        }
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
            // frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
            // frontRightWheelCollider.motorTorque = verticalInput * motorForce;
            // currentbreakForce = (isBreaking || (timer < destroyingTime)) ?  breakForce : 0f;
            currentbreakForce = (timer < destroyingTime) ? 0.0f : velocity; // velocity
            ApplyBreaking();
        }
    }

    private void ApplyBreaking()
    {
        // Debug.Log("Prędkość: " + currentbreakForce);
        // Debug.Log("Timer: " + timer);
        velocityValue = currentbreakForce;

        // frontRightWheelCollider.brakeTorque = currentbreakForce;
        // frontLeftWheelCollider.brakeTorque = currentbreakForce;
        // rearLeftWheelCollider.brakeTorque = currentbreakForce;
        // rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void OnDrawGizmos() 
    {
        for(int i = 0; i < numberOfRays; ++i) 
        {
            var rotation = this.transform.rotation;
            var rotationMod = Quaternion.AngleAxis((i / ((float)numberOfRays - 1)) * angle * 2 - angle, this.transform.up + new Vector3(0, 0.25f, 0));
            var avoidDirection = rotation * rotationMod * direction;
            Gizmos.DrawRay(this.transform.position + new Vector3(0, 0.25f, 0), avoidDirection * rayRange);
        }
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
        if (other.gameObject.layer == 6 && UnityEngine.Random.Range(0, 100) < removalProbability && timer == destroyingTime) {
            timer = 0;
            targetedWeed = other;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Name: " + collision.gameObject.name);
        if (collision.gameObject.name == "FencePanel")
        {
            direction.z *= -1;
        }      
    }

    private void DestroyWeed() {
        Destroy(targetedWeed.gameObject);
        Debug.Log("Remaining life points: " + (--life).ToString());
    }
}