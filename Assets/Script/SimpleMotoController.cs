using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMotoController : MonoBehaviour
{
    [Header("GLOBAL SETTINGS")]
    private float m_horizontalInput;
    private float m_verticalInput;
    private float m_steeringAngle;
    public float maximumSteerAngle = 30;
    public float motorForce = 50;
    public float f_leanForce = 20;

    public WheelCollider w_front;
    public WheelCollider w_Rear;
    public Transform t_front;
    public Transform t_Rear;

    public GameObject go_Corps;
    public GameObject go_WheelColliders;

    public void GetInput()
    {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = Input.GetAxis("Vertical");
    }

    private void Steer()
    {
        m_steeringAngle = maximumSteerAngle * m_horizontalInput;
        w_front.steerAngle = m_steeringAngle;
        
    }

    private void Accelerate()
    {
        w_Rear.motorTorque = -m_verticalInput * motorForce;
        
    }

    private void UpdateWheelPoses()
    {
        UpdateWheelPose(w_front, t_front);
        UpdateWheelPose(w_Rear, t_Rear);
    }

    private void UpdateWheelPose( WheelCollider _collider, Transform _transform)
    {
        Vector3 _pos = transform.position;
        Quaternion _quat = _transform.rotation;

        _collider.GetWorldPose(out _pos, out _quat);

        _transform.position = _pos;
        _transform.rotation = _quat;
    }

    private void FixedUpdate()
    {
        GetInput();
        Steer();
        Accelerate();
        UpdateWheelPoses();
    }
    /*
    void OnCollisionEnter(Collision collision)
    {
        go_WheelColliders.SetActive(false);


    }*/



}
