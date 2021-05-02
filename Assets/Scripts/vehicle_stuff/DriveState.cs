using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Sub-state machine to handle things that we need while driving.
public class DrivingSSM
{
    public DriveMasterControl master;
    float update_period;
    float t_last_calc;

    public DrivingSSM(DriveMasterControl _master)
    {
        master = _master;
        update_period = 1.0f;
        t_last_calc = 0.0f;
    }

    public DriveState next_state(DynamicParams dp)
    {
        return DriveState.on_road;
    }

    bool check_obstacles(Vector3 goal)
    {
        // Walk along the path, performing a capsule cast between every pair of
        // adjacent vertices. Once we get a hit, break immediately and calcualte
        // the distance to the obstacle.
        // float distance_downroad = 0.0f;
        // for (int i = 0; i < path.corners.Length - 2; i++)        
        // {
        //     Vector3 corner = path.corners[i];
        //     Vector3 next_corner = path.corners[i + 1];

        //     Physics.BoxCast()
        // }
        return false;
    }
}