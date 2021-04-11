using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveTrainControl : MonoBehaviour
{
    // Start is called before the first frame update
    // Should be bounded from -1 to 1, like GetAxis is.
    public float steering_signal;
    public float lock_angle;
    public bool brake_signal;
    public List<WheelCollider> steering_wheels;
    public List<WheelCollider> drive_wheels;
    public List<WheelCollider> idle_wheels;
    public EngineControl engine;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void manage_power()
    {
        float brake_f = System.Convert.ToSingle(brake_signal);
        float n_brake_f = System.Convert.ToSingle(!brake_signal);

        foreach (WheelCollider wheel in drive_wheels)
        {
            wheel.motorTorque = (engine.torque / drive_wheels.Count) * 
                                n_brake_f;
            wheel.brakeTorque = 1000.0f * brake_f;
        }
        foreach (WheelCollider wheel in idle_wheels)
        {
            wheel.brakeTorque = 1000.0f * brake_f;
        }
    }

    void manage_steering()
    {
        steering_signal = Mathf.Max(-1.0f, Mathf.Min(1.0f, steering_signal));
        float angle = lock_angle * steering_signal;
        foreach (WheelCollider wheel in steering_wheels)
        {
            wheel.steerAngle = angle;
        }
    }

    void FixedUpdate()
    {
        manage_steering();
        manage_power();
    }
}
