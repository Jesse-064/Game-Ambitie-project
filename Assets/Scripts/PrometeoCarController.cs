using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class SimpleCarController : MonoBehaviour
{
    [Header("Motor & Sturen")]
    public float motorForce = 2000f;
    public float brakeForce = 3000f;
    public float maxSteerAngle = 30f;

    [Header("Versnellingsbak")]
    public int maxGear = 5;
    public float[] gearRatios = { 10f, 20f, 40f, 60f, 80f };
    private int currentGear = 0;
    private float rpm;
    private float shiftRPM = 3000f;

    [Header("Wielen")]
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;

    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    [Header("Rigidbody Setup")]
    public Vector3 centerOfMass = new Vector3(0f, -0.5f, 0f);

    [Header("Geluid")]
    [SerializeField] private AudioSource engineSound;
    [SerializeField] private AudioClip engineClip;
    [SerializeField] private float minPitch = 1f;
    [SerializeField] private float maxPitch = 2f;

    [Header("Speedometer")]
    public Transform needleTransform;
    public float maxSpeed = 200f; // Max snelheid in km/u
    public float minNeedleAngle = -130f;
    public float maxNeedleAngle = 130f;
    [SerializeField] private float needleSmoothSpeed = 5f;

    [Header("Tekst (optioneel)")]
    public Text speedText;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass;

        SetupGrip(frontLeftCollider);
        SetupGrip(frontRightCollider);
        SetupGrip(rearLeftCollider);
        SetupGrip(rearRightCollider);

        SetupEngineSound();
    }

    void FixedUpdate()
    {
        float throttle = Input.GetAxis("Vertical");
        float steer = Input.GetAxis("Horizontal");

        HandleMotor(throttle);
        HandleSteering(steer);
        HandleBraking(Input.GetKey(KeyCode.Space));
        UpdateWheelMeshes();
        SimulateGears();
        UpdateEngineSound();
        UpdateSpeedUI();
        UpdateSpeedometerNeedle();
    }

    void HandleMotor(float input)
    {
        float torque = input * motorForce;
        frontLeftCollider.motorTorque = torque;
        frontRightCollider.motorTorque = torque;
    }

    void HandleSteering(float input)
    {
        float angle = input * maxSteerAngle;
        frontLeftCollider.steerAngle = angle;
        frontRightCollider.steerAngle = angle;
    }

    void HandleBraking(bool isBraking)
    {
        float brake = isBraking ? brakeForce : 0f;
        ApplyBrake(brake);
    }

    void ApplyBrake(float brake)
    {
        frontLeftCollider.brakeTorque = brake;
        frontRightCollider.brakeTorque = brake;
        rearLeftCollider.brakeTorque = brake;
        rearRightCollider.brakeTorque = brake;
    }

    void UpdateWheelMeshes()
    {
        UpdateWheel(frontLeftCollider, frontLeftMesh);
        UpdateWheel(frontRightCollider, frontRightMesh);
        UpdateWheel(rearLeftCollider, rearLeftMesh);
        UpdateWheel(rearRightCollider, rearRightMesh);
    }

    void UpdateWheel(WheelCollider col, Transform mesh)
    {
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);
        mesh.position = pos;
        mesh.rotation = rot;
    }

    void SetupGrip(WheelCollider col)
    {
        WheelFrictionCurve fwd = col.forwardFriction;
        fwd.stiffness = 6f;
        col.forwardFriction = fwd;

        WheelFrictionCurve side = col.sidewaysFriction;
        side.stiffness = 6f;
        col.sidewaysFriction = side;
    }

    void SimulateGears()
    {
        float speedKmh = rb.linearVelocity.magnitude * 3.6f;
        if (currentGear < gearRatios.Length - 1 && speedKmh > gearRatios[currentGear])
        {
            currentGear++;
        }
        else if (currentGear > 0 && speedKmh < gearRatios[currentGear - 1] - 5f)
        {
            currentGear--;
        }

        float gearRange = currentGear < gearRatios.Length - 1 ? gearRatios[currentGear + 1] - gearRatios[currentGear] : 20f;
        float currentGearSpeed = Mathf.Clamp(speedKmh - gearRatios[currentGear], 0, gearRange);
        rpm = (currentGearSpeed / gearRange) * shiftRPM;
    }

    void UpdateEngineSound()
    {
        if (engineSound == null) return;

        float rpmRatio = Mathf.InverseLerp(0f, shiftRPM, rpm);
        float pitch = Mathf.Lerp(minPitch, maxPitch, rpmRatio);
        engineSound.pitch = pitch;
    }

    void SetupEngineSound()
    {
        if (engineSound == null)
        {
            engineSound = gameObject.AddComponent<AudioSource>();
        }

        if (engineClip != null)
        {
            engineSound.clip = engineClip;
            engineSound.loop = true;
            engineSound.playOnAwake = true;
            engineSound.Play();
        }
        else
        {
            Debug.LogWarning("Geen engineClip toegewezen!");
        }
    }

    void UpdateSpeedUI()
    {
        if (speedText != null)
        {
            float speedKmh = rb.linearVelocity.magnitude * 3.6f;
            speedText.text = Mathf.RoundToInt(speedKmh).ToString() + " km/u";
        }
    }

    void UpdateSpeedometerNeedle()
    {
        if (needleTransform == null) return;

        float speedKmh = rb.linearVelocity.magnitude * 3.6f;
        float speedPercent = Mathf.Clamp01(speedKmh / maxSpeed);
        float targetAngle = Mathf.Lerp(minNeedleAngle, maxNeedleAngle, speedPercent);
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        needleTransform.localRotation = Quaternion.Lerp(needleTransform.localRotation, targetRotation, Time.deltaTime * needleSmoothSpeed);
    }
}
