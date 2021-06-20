using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverThrusters : MonoBehaviour
{
    [SerializeField]
    private Rigidbody hoverbikeBody;
    private Vector3 m_EulerAngleVelocity;

    public float Speed = 5f;
    public float BrakeForce = 5f;
    public float speedRotation = 5f;
    private float deadZone = 0.1f;

    public float forwardAcl = 100f;
    public float BackwardAcl = 25f;
    float currThrust = 0f;

    public float turnStrength = 10f;
    float currTurn = 0f;

    private Vector3 _inputs = Vector3.zero;

    // Start is called before the first frame update
    void Awake()
    {
        hoverbikeBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //Set the axis the Rigidbody rotates in (100 in the y axis)
        m_EulerAngleVelocity = new Vector3(0, 100, 0);

        hoverbikeBody.GetComponent<Rigidbody>();
        //Fetch the Rigidbody from the GameObject with this script attached

    }

    // Update is called once per frame
    void Update()
    {
        //_inputs.z = Input.GetAxis("Vertical");

        //m_EulerAngleVelocity = new Vector3(0, Input.GetAxis("Horizontal"), 0);


        //NEW STUFF//
        //MainThrust

        //Forward and BackWard Thrusters
        currThrust = 0.0f;
        float aclAxis = Input.GetAxis("Vertical");

        if (aclAxis > deadZone)
            currThrust = aclAxis * forwardAcl;
        else if(aclAxis < -deadZone)
            currThrust = aclAxis * BackwardAcl;

        //Turning Vector
        currTurn = 0.0f;
        float turnAxis = Input.GetAxis("Horizontal");
        if (Mathf.Abs(turnAxis) > deadZone)
            currTurn = turnAxis;




    }

    void FixedUpdate()
    {
        //if (Input.GetKey(KeyCode.Z))
        //{
        //    //transform.forward = _inputs;
        //    Vector3 directionPush = transform.forward;
        //    hoverbike.AddForce(directionPush * Speed);
        //}

        //if (Input.GetKey(KeyCode.S))
        //{
        //    //transform.forward = _inputs;
        //    hoverbike.AddForce(-hoverbike.velocity * BrakeForce);

        //}

        //Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.deltaTime * speedRotation);
        //hoverbike.MoveRotation(deltaRotation * hoverbike.rotation);
        //Debug.Log(Vector3.Angle(transform.forward, hoverbike.velocity));

        //NEW STUFF//

        //Forward
        if(Mathf.Abs(currThrust)>0)
        {
            hoverbikeBody.AddForce(transform.forward * currThrust);
        }

        //Turn
        //if(currTurn > 0)
        //{
        //    hoverbikeBody.AddRelativeTorque(Vector3.up * currTurn * turnStrength);
        //}
        //else if(currTurn < 0)
        //{
        //    hoverbikeBody.AddRelativeTorque(Vector3.up * currTurn * turnStrength);
        //}
        
    }

    private Vector3 VelocityAssistVector(Vector3 vectorToCorrect)
    {
        vectorToCorrect.z *= -1;
        return vectorToCorrect;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, hoverbikeBody.velocity, Color.red);
        Debug.DrawRay(transform.position, transform.forward * 3, Color.green);
        //Debug.DrawRay(transform.position, VelocityAssistVector(hoverbike.velocity).normalized, Color.yellow);
    }
}
