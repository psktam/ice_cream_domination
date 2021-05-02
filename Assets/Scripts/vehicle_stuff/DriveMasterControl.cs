using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public struct DynamicParams 
{
    public Vector3 goal;
}


public class DriveMasterControl : MonoBehaviour
{
    public DriveState state;
    public Vector3 goal;
    public TrafficManager traffic;
    DrivingSSM drive_ssm;
    DynamicParams env;

    // Start is called before the first frame update
    void Start()
    {
        state = DriveState.on_road;
        drive_ssm = new DrivingSSM(this);
        env = new DynamicParams();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        env.goal = goal;
        switch(state)
        {
            case DriveState.on_road:
                state = drive_ssm.next_state(env);
                break;
        }
    }
}

