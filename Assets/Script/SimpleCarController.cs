using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    public void GetInput()
    {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = Input.GetAxis("Vertical");
    }

    private void Steer()
    {
        m_steeringAngle = maximumSteerAngle * m_horizontalInput;
        w_frontLeft.steerAngle = m_steeringAngle;
        w_frontRight.steerAngle = m_steeringAngle;
    }

    private void Accelerate()
    {
        w_RearLeft.motorTorque = m_verticalInput * motorForce;
        w_RearRight.motorTorque = m_verticalInput * motorForce;
    }

    private void UpdateWheelPoses()
    {
        UpdateWheelPose(w_frontLeft, t_frontLeft);
        UpdateWheelPose(w_frontRight, t_frontRight);
        UpdateWheelPose(w_RearLeft, t_RearLeft);
        UpdateWheelPose(w_RearRight, t_RearRight);
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

    private float m_horizontalInput;
    private float m_verticalInput;
    private float m_steeringAngle;
    public float maximumSteerAngle = 30;
    public float motorForce = 50;

    public WheelCollider w_frontLeft, w_frontRight;
    public WheelCollider w_RearLeft, w_RearRight;
    public Transform t_frontLeft, t_frontRight;
    public Transform t_RearLeft, t_RearRight;
    
}
