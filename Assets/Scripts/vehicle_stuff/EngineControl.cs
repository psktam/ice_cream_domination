using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineControl : MonoBehaviour
{
    // Start is called before the first frame update
    // Max torque in ft lbs
    public float max_torque = 150.0f;
    public float inertia = 1.0f;
    public float throttle;
    public float idle = 800.0f;
    public float redline = 3000.0f;
    private float _rpm;
    float rate;
    public float rpm {
        get
        {
            return _rpm;
        }
        private set
        {
            _rpm = value;
        }
    }
    public float torque {
        get 
        {
            return rpm / redline * max_torque;
        }
    }
    void Start()
    {
        rpm = idle;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Current RPM: " + rpm);
    }

    void FixedUpdate()
    {
        float asymp = redline;
        throttle = Mathf.Max(-1.0f, Mathf.Min(throttle, 1.0f));
        asymp *= throttle;
        if (throttle <= 0.0f)
        {
            asymp = idle;
        }
        rpm += (asymp - rpm) * Time.fixedDeltaTime / inertia;
    }
}

