using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverAntiGravReactor : MonoBehaviour
{
    [SerializeField]
    private Rigidbody hoverbikeBody;
    private bool isBikeActive = false;

    private Vector3 velocityVec;
    private Vector3 forwardVec;
    private Vector3 eulerAngles;
    private Vector3 parkingVector;

    PlayerInput input;

    [SerializeField]
    private float maxAngleLeaning = 45f;
    [SerializeField]
    private float antiLeaningFactor = 1f;

    public float angle;
    private int cross;

    [SerializeField] private float verticalSpeedMultiplier = 1f;
    [SerializeField] private float hoverHeight = 10f;
    [SerializeField] private float hoverTolerance = 0.5f;
    [SerializeField] private float maximumVerticalVelocity = 10f;
    [SerializeField] private float leanAssistForceTorque = 1f;

    public LayerMask layerMask;
    [SerializeField]
    private float hoverForce = 9f;
    [SerializeField]
    private float hoverGravity = 20f;
    [SerializeField]
    private float fallGravity = 50f;
    [SerializeField]
    private float nbHoverPointsSlots;
    [SerializeField]
    private GameObject[] hoverPoints;
    [SerializeField]
    private Vector3[] hoverPointsNormals;

    public float deadZone = 0.1f;
    private bool isMoving = false;
    [SerializeField]
    private bool SlopeAssistStatue = true;
    [SerializeField]
    private float hoverAssistForce = 15f;

    [Header("Drive Settings")]
    public float driveForce = 17f;
    public float slowingVelFactor = .99f;
    public float brakingVelFactor = .95f;
    public float angleOfRoll = 30f;
    public float sideWaysFactor = 1f;
    [SerializeField]
    private float reactivitySuspensionFactor = 5f;

    public Transform shipBody;

    public PIDController hoverPID;

    private void Start()
    {
        hoverbikeBody = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();
        hoverPID = GetComponent<PIDController>();

        layerMask = 1 << LayerMask.NameToLayer("Character");
        layerMask = ~layerMask;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        { 
            ToggleBike();
        }

        float aclAxis = Input.GetAxis("Vertical");

        if (aclAxis > deadZone)
        {
            isMoving = true;
        }
        else
            isMoving = false;


        //if (isBikeActive)
        //{
        //    Physics.Raycast(hoverbikeBody.transform.position, Vector3.down, out RaycastHit hit);

        //    Vector3 modifiedVelocity = hoverbikeBody.velocity;

        //    if ((hit.distance > hoverHeight - hoverTolerance) && (hit.distance < hoverHeight + hoverTolerance))
        //    {
        //        modifiedVelocity.y = 0f;
        //    }
        //    else
        //    {
        //        modifiedVelocity.y = -(hit.distance - hoverHeight) * verticalSpeedMultiplier;
        //        modifiedVelocity.y = Mathf.Clamp(modifiedVelocity.y, -maximumVerticalVelocity, maximumVerticalVelocity);
        //    }

        //    //Debug.Log($"Distance from ground: {hit.distance}, hoverHeight - hoverTolerance: {hoverHeight - hoverTolerance}");

        //    hoverbikeBody.velocity = modifiedVelocity;


        //    //LeanBike();


        //}


    }

    private void FixedUpdate()
    {
        //Hover SHIT
        if (isBikeActive)
        {
            HoverSuspension();
            HoverTurnAssist();
            HoverYawRotation();
            HoverSlopeBrakeAssist();
            hoverbikeBody.useGravity = false;
        }
        else
            hoverbikeBody.useGravity = true;

        //float turn = Input.GetAxis("Horizontal");
        //hoverbikeBody.AddTorque(transform.forward * -leanAssistForceTorque * turn);


    }

    private void HoverTurnAssist()
    {
        //LEAN ASSIST

        Vector3 groundNormal;

        Ray ray = new Ray(transform.position, -transform.up);

        RaycastHit hitInfo;

        bool isGround = Physics.Raycast(ray, out hitInfo, hoverHeight, layerMask);

        //if (isGround)
        //{
        //    groundNormal = hitInfo.normal.normalized;
        //}
        //else
        //{
        //    groundNormal = Vector3.up;
        //}

        groundNormal = CalculateAverageHoverPointNormals(hoverPointsNormals);

        Vector3 projection = Vector3.ProjectOnPlane(transform.forward, groundNormal);
        Quaternion rotation = Quaternion.LookRotation(projection, groundNormal);

        hoverbikeBody.MoveRotation(Quaternion.Slerp(hoverbikeBody.rotation, rotation, Time.deltaTime * reactivitySuspensionFactor));

        float angle = angleOfRoll * -input.rudder;

        Quaternion bodyRotation = transform.rotation * Quaternion.Euler(0f, 0f, angle);

        shipBody.rotation = Quaternion.Slerp(shipBody.rotation, bodyRotation, Time.deltaTime * leanAssistForceTorque);

        //VECTOR DIRECTION CORRECTION
        float rotationTorque = input.rudder - hoverbikeBody.angularVelocity.y;

        hoverbikeBody.AddRelativeTorque(0f, rotationTorque, 0f, ForceMode.Force);

        float sidewaysSpeed = Vector3.Dot(hoverbikeBody.velocity, transform.right);

        Vector3 sideFriction = -transform.right * (sidewaysSpeed / Time.deltaTime/ sideWaysFactor);

        hoverbikeBody.AddForce(sideFriction, ForceMode.Acceleration);

    }

    private void ToggleBike()
    {
        isBikeActive = !isBikeActive;
    }

    private void HoverSuspension()
    {
        RaycastHit hit;
        for (int i = 0; i < hoverPoints.Length; i++)
        {

            Vector3 groundNormal;



            var hoverPoint = hoverPoints[i];

            Ray ray = new Ray(hoverPoint.transform.position, -hoverPoint.transform.up);

            bool isGround = Physics.Raycast(ray, out hit, hoverHeight, layerMask);

            if (isGround)
            {
                float height = hit.distance;

                groundNormal = hit.normal.normalized;
                hoverPointsNormals[i] = groundNormal;

                float forcePercent = hoverPID.GetOutput(hoverHeight, height);

                Vector3 force = groundNormal * hoverForce * forcePercent;

                Vector3 gravity = -groundNormal * hoverGravity * height;

                hoverbikeBody.AddForceAtPosition(force, hoverPoint.transform.position,ForceMode.Acceleration);
                hoverbikeBody.AddForceAtPosition(gravity , hoverPoint.transform.position, ForceMode.Acceleration);
            }
            else
            {

                groundNormal = Vector3.up;
                hoverPointsNormals[i] = groundNormal;

                Vector3 gravity = -groundNormal * fallGravity;
                hoverbikeBody.AddForceAtPosition(gravity, hoverPoint.transform.position, ForceMode.Acceleration);

                ////Rotational Correction if gravity center is higher than
                ////hoverpoint position in global space
                //if (transform.position.y > hoverPoint.transform.position.y)
                //{
                //    hoverbikeBody.AddForceAtPosition(hoverPoint.transform.up * hoverForce,
                //        hoverPoint.transform.position);


                //}
                ////Same to up but in the opposite
                ////if gravity center is lower than hoverpoint 
                //else
                //{
                //    hoverbikeBody.AddForceAtPosition(hoverPoint.transform.up * hoverForce,
                //        hoverPoint.transform.position);


                //}
            }

            //Debug.Log("Hover point " + i + " stored vector = " + StoredVector);
        }

    }

    private void HoverYawRotation()
    {
        float rotationTorque = input.rudder - hoverbikeBody.angularVelocity.y;

        hoverbikeBody.AddRelativeTorque(0f, rotationTorque, 0f, ForceMode.VelocityChange);
    }

    private void HoverSlopeBrakeAssist()
    {
        if (!SlopeAssistStatue)
        {
            float dotGravity = Vector3.Dot(transform.forward, Vector3.up);
            //Vector3 slopeFallVector = new Vector3(0f, 0f, -dotGravity);

            hoverbikeBody.AddForce(transform.forward * -dotGravity * hoverbikeBody.mass / 50, ForceMode.Acceleration);
            Debug.Log(transform.forward * -dotGravity * hoverbikeBody.mass/50);
        }
    }

    private Vector3 CalculateAverageHoverPointNormals(Vector3[] array)
    {
        Vector3 averageVector = Vector3.zero;
        for(int i =0; i < array.Length;i++)
        {
            Vector3 currentVector = array[i];
            averageVector += currentVector;
        }
        averageVector /= array.Length;
        return averageVector;
    }

    private void OnDrawGizmos()
    {
        //  Hover Force
        RaycastHit hit;

        

        for (int i = 0; i < hoverPoints.Length; i++)
        {
            var hoverPoint = hoverPoints[i];

            Ray ray = new Ray(hoverPoint.transform.position, -hoverPoint.transform.up);

            if (Physics.Raycast(ray, out hit,
                                hoverHeight,
                                layerMask))
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(hoverPoint.transform.position, hit.point);
                Gizmos.DrawSphere(hit.point, 0.1f);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(hoverPoint.transform.position,
                               hoverPoint.transform.position - Vector3.up *  hoverHeight);
            }
        }

        //Hover Central Correction
        if (Physics.Raycast(transform.position,-transform.up,
            out hit, hoverHeight,
            layerMask))
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(hit.point, CalculateAverageHoverPointNormals(hoverPointsNormals));
            Gizmos.DrawSphere(hit.point, 0.1f);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position,
                           transform.position - transform.up
                           * hoverHeight);
        }

        Debug.DrawRay(transform.position, hoverbikeBody.velocity, Color.red);
        Debug.DrawRay(transform.position, parkingVector, Color.cyan);
    }









}
