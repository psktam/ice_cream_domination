using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Use this to control direction and speed
public class VehicleSpeedDirectionControl : MonoBehaviour
{

    // Units are degrees. 0 degrees corresponds to the x-axis, 90 degrees 
    // corresponds to the z-axis
    public float target_direction;
    // This needs to be a non-negative number
    public float target_speed;
    public EngineControl engine;
    public DriveTrainControl drivetrain;
    Vector3 last_position;
    Utils.TON idle_timer;

    public Vector3 velocity()
    {
        return (transform.position - last_position) / Time.fixedDeltaTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        last_position = transform.position;
        idle_timer = new Utils.TON(false, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        float forward_speed = Vector3.Dot(velocity(), transform.forward);
        
        speedControl();
        directionControl();
        last_position = transform.position;
    }

    void speedControl()
    {
        float forward_speed = Vector3.Dot(velocity(), transform.forward);
        float kp = 0.5f;

        engine.throttle = kp * (target_speed - forward_speed);

        // If we've been idling for a second or longer, apply the brakes
        idle_timer.update((engine.rpm - engine.idle) < 1.0f);
        drivetrain.brake_signal = idle_timer.q();
    }

    void directionControl()
    {
        float current_angle = Mathf.Atan2(
            transform.forward.x, transform.forward.z) * 180.0f / Mathf.PI;
        float delta_angle = target_direction - current_angle;
        float kp = 0.1f;
        drivetrain.steering_signal = kp * (delta_angle);
    }
}
