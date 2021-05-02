using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// The top level class responsible for guiding the vehicle along an existing
// road network. There are a few dependencies that are required to get this 
// thing working.

// 1: you need a navmesh that paints the existing road network
// 2: There needs to be a single RoadArchitect RoadNetwork in the scene with a 
//    TrafficManager component attached to it.

public class AIVehicleController : MonoBehaviour
{
    public int target_idx;
    public OnRoadController orc;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        var next_state = orc.next_state(target_idx);
    }
}
