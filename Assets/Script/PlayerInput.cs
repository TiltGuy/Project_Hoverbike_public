
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    public string verticalAxisName = "Vertical";
    public string horizontalAxisName = "Horizontal";
    public string brakingKey = "Brake";
    public string activationBikeKey = "Jump";

    [HideInInspector] public float thruster;
    [HideInInspector] public float rudder;
    [HideInInspector] public bool isBraking;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel")&& !Application.isEditor)
            Application.Quit();

        thruster = Input.GetAxis(verticalAxisName);
        rudder = Input.GetAxis(horizontalAxisName);
        isBraking = Input.GetButton(brakingKey);
        
    }
}
