using System;
using System.Text.RegularExpressions;
using System.IO;
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

    public Vector3 direction = Vector3.forward;  // This is our direction we're travelling in.
    public float velocity = 5.0f;
    public float avoidVelocity = 10.0f;
    private float velocityValue;

    private Regex rgxRock = new Regex(@"^rock.*$", RegexOptions.IgnoreCase);
    private Regex rgxWeed = new Regex(@"^weed.*$", RegexOptions.IgnoreCase);
    private Regex rgxWeedRobot = new Regex(@"^weedrobot.*$", RegexOptions.IgnoreCase);

    private int removalCounter = 0;
    private int detectedCounter = 0;

    private int startLife;

    private string logPath = "Assets/log";
    private string batteryPath = "Assets/battery";

    [SerializeField] private int id = 1;
    [SerializeField] private int life = 20;

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

    [SerializeField] private float numberOfRays = 17; // n + 1
    [SerializeField] private float rayRange = 10;
    [SerializeField] private float angle = 180;
   
    [SerializeField] private int destroyingTime = 100;

    [SerializeField] private int removalProbability = 80;

    private float lastInterval;

    void Start() 
    {
        // Init
        timer = destroyingTime;
        lastInterval = Time.realtimeSinceStartup;
        startLife = life;

        File.WriteAllText(logPath + id + ".txt", "Time                            Romoved weeds    Battery    Encountered weeds\n");
        File.WriteAllText(batteryPath + id + ".txt", "Time    Battery\n");
    }

    void Update()
    {
        var deltaPosition = velocityValue * direction;
        for(int i = 0; i < numberOfRays; ++i) 
        {
            var rotation = this.transform.rotation;
            var rotationMod = Quaternion.AngleAxis((i / ((float)numberOfRays - 1)) * angle * 2 - angle, this.transform.up + new Vector3(0, 0.125f, 0));
            var avoidDirection = rotation * rotationMod * direction;
            var ray = new Ray(this.transform.position + new Vector3(0, 0.125f, 0), avoidDirection * rayRange);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, rayRange))
            {
                // Debug.Log("hitInfo: " + hitInfo.collider.gameObject.name);
                if (rgxRock.IsMatch(hitInfo.collider.gameObject.name) || rgxWeedRobot.IsMatch(hitInfo.collider.gameObject.name))
                {
                    Debug.Log("Inside of");
                    deltaPosition -= (1.0f / numberOfRays) * velocityValue * avoidDirection * avoidVelocity; // avoidVelocity 10.0f - scale factor (selected individually)
                }
            }
            else
            {
                deltaPosition += (1.0f / numberOfRays) * velocityValue * avoidDirection;

            }
        }

        transform.Translate(deltaPosition * Time.deltaTime);

        float timeNow = Time.realtimeSinceStartup;
        // Debug.Log("Simulation: " + (timeNow - lastInterval));
        if ((timeNow - lastInterval) > 15)
        {
            ChargeBattery();
            lastInterval = timeNow;
        }
    }
    private void FixedUpdate()
    {
        GetInput();   
        HandleMotor();

        if (timer < destroyingTime) {
            ++timer;
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
        if (life > 0) {
            if (horizontalInput != 0 || verticalInput != 0 || isBreaking)
            {
                frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
                frontRightWheelCollider.motorTorque = verticalInput * motorForce;
                currentbreakForce = isBreaking || (timer < destroyingTime) ?  breakForce : 0f;
                ApplyBreakingManually();
            }
            else
            {
                currentbreakForce = (timer < destroyingTime) ? 0.0f : velocity; // velocity
                ApplyBreaking();
            }
            
        }
    }

    private void ApplyBreaking()
    {
        velocityValue = currentbreakForce;
    }

    private void ApplyBreakingManually()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void OnDrawGizmos() 
    {
        for(int i = 0; i < numberOfRays; ++i) 
        {
            var rotation = this.transform.rotation;
            var rotationMod = Quaternion.AngleAxis((i / ((float)numberOfRays - 1)) * angle * 2 - angle, this.transform.up + new Vector3(0, 0.125f, 0));
            var avoidDirection = rotation * rotationMod * direction;
            Gizmos.DrawRay(this.transform.position + new Vector3(0, 0.125f, 0), avoidDirection * rayRange);
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

    private void OnTriggerEnter(Collider other) 
    {
        bool isSure = UnityEngine.Random.Range(0, 100) < removalProbability;
        bool isEncountered = rgxWeed.IsMatch(other.gameObject.name);
        
        if (isEncountered) {
            IncreaseEncounteredWeeds();
            if (isSure && timer == destroyingTime) 
            {
                targetedWeed = other;

                timer = 0;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("Name: " + collision.gameObject.name);
        if (collision.gameObject.name == "FencePanel")
        {
            direction.z *= -1;
        }      
    }

    private void DestroyWeed() 
    {
        Destroy(targetedWeed.gameObject);
        IncreaseRemovedWeeds();
        DischargeBattery();
        WriteString(logPath, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "                            " + removalCounter + "    " + GetBatteryStatus() + "%" + "    " + detectedCounter);
    }

    private void WriteString(string path, string str)
    {
        Debug.Log("Write to file...");
        StreamWriter writer = new StreamWriter(path + id + ".txt", true);
        writer.WriteLine(str);
        writer.Close();
    }

    private string GetLifeInfo(string info)
    {
        string message = (GetBatteryStatus()).ToString() + "%";
        Debug.Log(info + message);
        return message;
    }

    private void ChargeBattery()
    {
        if (life < startLife)
        {
            ++life;
        }
        WriteString(batteryPath, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "     " + GetLifeInfo("Remaining battery +: "));
    }

    private void DischargeBattery()
    {
        --life;
        WriteString(batteryPath, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "     " + GetLifeInfo("Remaining battery -: "));
    }

    private void IncreaseEncounteredWeeds()
    {
        ++detectedCounter;
    }

    private void IncreaseRemovedWeeds()
    {
        ++removalCounter;
    }

    private float GetBatteryStatus()
    {
        return (float)life / (float)startLife * 100;
    }
}