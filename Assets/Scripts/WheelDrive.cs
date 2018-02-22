using UnityEngine;
using System;



[Serializable]
public enum DriveType
{
	RearWheelDrive,
	FrontWheelDrive,
	AllWheelDrive
}

public class WheelDrive : MonoBehaviour
{
    public GameObject bone;
    Rigidbody carRB;
    float boneRotationSpeed;
    public int maxBoneRotation = 20;
    public int boneSpeed = 8;
    public float rotateTorque;
    bool wheelGrounded;
    float turnUp;
    float turnLR;
    Rigidbody rb;
    float Speed;

    [Tooltip("Maximum steering angle of the wheels")]
	public float maxAngle = 30f;
	[Tooltip("Maximum torque applied to the driving wheels")]
	public float maxTorque = 300f;
    [Tooltip("Maximum speed sets torque to 0 when reaches this")]
    public float maxSpeed = 50f;               // MAX SPEED
	[Tooltip("Maximum brake torque applied to the driving wheels")]
	public float brakeTorque = 30000f;
	[Tooltip("If you need the visual wheels to be attached automatically, drag the wheel shape here.")]
	public GameObject wheelShape;

	[Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
	public float criticalSpeed = 5f;
	[Tooltip("Simulation sub-steps when the speed is above critical.")]
	public int stepsBelow = 5;
	[Tooltip("Simulation sub-steps when the speed is below critical.")]
	public int stepsAbove = 1;

	[Tooltip("The vehicle's drive type: rear-wheels drive, front-wheels drive or all-wheels drive.")]
	public DriveType driveType;

    private WheelCollider[] m_Wheels;

    // Find all the WheelColliders down in the hierarchy.
	void Start()
	{
        carRB = gameObject.GetComponent<Rigidbody>();
        m_Wheels = GetComponentsInChildren<WheelCollider>();

		for (int i = 0; i < m_Wheels.Length; ++i) 
		{
			var wheel = m_Wheels [i];

			// Create wheel shapes only when needed.
			if (wheelShape != null)
			{
				var ws = Instantiate (wheelShape);
				ws.transform.parent = wheel.transform;

                if (wheel.transform.localPosition.x > 0f)
                {
                    ws.transform.localScale = new Vector3(ws.transform.localScale.x * -1f, ws.transform.localScale.y, ws.transform.localScale.z);
                }
			}
		}

        rb = GetComponent<Rigidbody>();
	}

	// This is a really simple approach to updating wheels.
	// We simulate a rear wheel drive car and assume that the car is perfectly symmetric at local zero.
	// This helps us to figure our which wheels are front ones and which are rear.
	void Update()
	{
        Speed = rb.velocity.magnitude * 3.6f;


        m_Wheels[0].ConfigureVehicleSubsteps(criticalSpeed, stepsBelow, stepsAbove);

		float angle = maxAngle * Input.GetAxis("Horizontal");

		float torque = maxTorque * Input.GetAxis("Vertical");

        boneRotationSpeed = Input.GetAxis("Horizontal") * boneSpeed;        // speed of bone rotation


            bone.transform.Rotate(new Vector3(0, 0, boneRotationSpeed * -1) * Time.deltaTime);

        if (bone.transform.localEulerAngles.z > 20f && Input.GetAxis("Horizontal") < 0)
        {
            bone.transform.localRotation = Quaternion.Euler(0, 0, maxBoneRotation);
        }
        else
        {
            if (bone.transform.localEulerAngles.z > -20f)
            {
                bone.transform.localRotation = Quaternion.Euler(0, 0, -maxBoneRotation);
            }
        }

        if (Input.GetAxis("Horizontal") == 0)
        {
            bone.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        float handBrake = Input.GetKey(KeyCode.X) ? brakeTorque : 0;

		foreach (WheelCollider wheel in m_Wheels)
		{
			// A simple car where front wheels steer while rear ones drive.
			if (wheel.transform.localPosition.z > 0)
				wheel.steerAngle = angle;

			if (wheel.transform.localPosition.z < 0)
			{
				wheel.brakeTorque = handBrake;
			}

			if (wheel.transform.localPosition.z < 0 && driveType != DriveType.FrontWheelDrive && Speed < maxSpeed)
			{
				wheel.motorTorque = torque;
			}

			if (wheel.transform.localPosition.z >= 0 && driveType != DriveType.RearWheelDrive && Speed < maxSpeed)
			{
				wheel.motorTorque = torque;
			}

            if (Speed > maxSpeed)
            {
                wheel.motorTorque = 0;
                //Debug.Log("MAX SPEED: " + Speed);
            }
            else { wheel.motorTorque = torque; }

            // Update visual wheels if any.
            if (wheelShape) 
			{
				Quaternion q;
				Vector3 p;
				wheel.GetWorldPose (out p, out q);

				// Assume that the only child of the wheelcollider is the wheel shape.
				Transform shapeTransform = wheel.transform.GetChild (0);
				shapeTransform.position = p;
				shapeTransform.rotation = q;
			}
		}
	}

    void FixedUpdate()
    {
        for (int i = 0; i < m_Wheels.Length; ++i)
        {
            var wheel = m_Wheels[i];

            wheelGrounded = wheel.isGrounded;
        }

        turnUp = Input.GetAxis("RotateUpDown");
        turnLR = Input.GetAxis("Horizontal");

        if (wheelGrounded == false)
        {
            carRB.AddTorque(transform.right * rotateTorque * turnUp);
            carRB.AddTorque(new Vector3(0, 0, -1) * rotateTorque * turnLR);
        }
    }
}
