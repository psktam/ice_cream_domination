using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class truck_control : MonoBehaviour
{

    public Rigidbody rb;

    public float turn_rate = 1.0f;
    public float lock_angle = 45.0f;
    public float cg_offset = -20.0f;
    public GameObject wheel_model;

    public List<WheelCollider> drive_wheels;
    public List<WheelCollider> idle_wheels;

    public float throttle_gain = 1.0f;
    public int throttle_reaction_frames = 10;

    public List<float> downforce_coeff;

    private float current_throttle = 0.0f;
    private Queue<float> throttle_vals = new Queue<float>();
    
    // Start is called before the first frame update
    void Start()
    {
    }

    private float modulo(float dividend, float divisor)
    {
        return dividend - divisor * Mathf.Floor(dividend / divisor);
    }

    // Update is called once per frame
    void Update()
    {
        manage_steering();
    }

    void manage_braking()
    {
        float braking = Input.GetAxis("Brake");
        foreach (WheelCollider wheel in idle_wheels)
        {
            wheel.brakeTorque = 1000.0f * braking;
        }
    }

    void manage_throttle()
    {
        throttle_vals.Enqueue(
            Input.GetAxis("Vertical") / throttle_reaction_frames);
        if (throttle_vals.Count >= throttle_reaction_frames)
        {
            throttle_vals.Dequeue();
        }

        current_throttle = 0.0f;
        foreach (float val in throttle_vals)
        {
            current_throttle += val;
        }

        current_throttle *= throttle_gain;
        // Debug.Log("Current throttle: " + current_throttle);
        
        foreach (WheelCollider wheel in drive_wheels)
        {
            // Negative sign because I put the wheels on backwards
            wheel.motorTorque = -current_throttle;
        }
    }

    /**
     * Apply downforce to the truck as it moves faster in order to help keep it
     * from rolling over.
     * Downforce scales as a polynomial, defined by the downforce_coeff vector.
     * downforce_coeff[0] is the 0th-order coefficient. downforce_coeff[1] is 
     * the 1st-order coefficient, and so on.
     */
    void manage_downforce()
    {
        float downforce = 0.0f;
        float power = 1.0f;
        float current_vel = rb.velocity.magnitude;

        foreach (float coeff in downforce_coeff)
        {
            downforce += coeff * power;
            power *= current_vel;
        }

        Vector3 force = -downforce * transform.up;
        // Now that we have total downforce, apply it to the truck
        Debug.DrawRay(transform.position, -force, Color.green, 1.0f);
        rb.AddForce(force);
    }

    void manage_steering()
    {
        float steering_input = Input.GetAxis("Horizontal") * turn_rate;

        foreach (WheelCollider wheel in drive_wheels)
        {
            float curr_angle = modulo((wheel.steerAngle - 180.0f), 360.0f) - 
                               180.0f;
            // Do one of two things here. If steering input detected, then 
            // steer the wheel if we're in lock. If no steering input, however,
            // center the wheel;
            if ((Mathf.Abs(steering_input) < 0.01f))
            {
                steering_input = Mathf.Clamp(
                    -Mathf.Sign(curr_angle) * turn_rate, 
                    Mathf.Min(curr_angle, -curr_angle), 
                    Mathf.Max(curr_angle, -curr_angle));
            }
            float next_angle = curr_angle + steering_input;
            if (Mathf.Abs(next_angle) < lock_angle)
            {
                wheel.steerAngle = next_angle;
            }
        }
    }

    void FixedUpdate()
    {
        rb.centerOfMass = new Vector3(rb.centerOfMass.x, 
                                      cg_offset,
                                      rb.centerOfMass.z);
        manage_throttle();
        manage_braking();
        manage_downforce();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(rb.worldCenterOfMass, 0.5f);
    }
}
