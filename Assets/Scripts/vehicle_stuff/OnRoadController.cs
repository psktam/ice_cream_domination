using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnRoadController : MonoBehaviour
{
    // This is to only be used when the car is on the road
    // Start is called before the first frame update
    public VehicleSpeedDirectionControl ctrl;
    public float update_period;
    float last_update_time;
    public List<TrafficNode> current_path;

    void Start()
    {
        last_update_time = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void updatePath(int target_idx)
    {
        last_update_time = Time.time;
        var start_idx = TrafficManager.Instance.query(
            transform.position)[0];
        var path = TrafficManager.Instance.calculate_path(
            start_idx, target_idx);
        current_path = new List<TrafficNode>(path.Count);
        foreach (int idx in path)
        {
            var current_node = TrafficManager.Instance.nodes[idx];
            Vector3 to_node = current_node.position - transform.position;
            current_path.Add(TrafficManager.Instance.nodes[idx]);
        }
    }

    DriveState calculate_target(out Vector3 target)
    {
        // There are quite a few edge cases that need to be considered here.
        // We'll enumerate them all

        // We're already at the target
        if (current_path.Count <= 1)
        {
            target = transform.position;
            return DriveState.at_target;
        }
        // Not at the target. See if we're already on the right path, or need
        // to turn around
        // Find the first point in front of the car
        int target_idx = -1;
        for (int i = 0; i < current_path.Count; i++)
        {
            var node = current_path[i];
            var to_node = node.position - transform.position;
            if (Vector3.Dot(to_node, transform.forward) > 0.0f)
            {                        
                target_idx = i;
                break;
            }
        }

        Vector3 dpath = new Vector3();
        Vector3 raw_target = new Vector3();
        // We need to turn around. Set the target as the first position.
        if ((target_idx == -1) || 
            (current_path[target_idx].position - 
                transform.position).magnitude > 10.0f)
        {
            Vector3 next_pos = current_path[1].position;
            Vector3 this_pos = current_path[0].position;
            dpath = next_pos - this_pos;
            raw_target = this_pos;
        }
        // We don't need to turn around. 
        else if (target_idx == 0)
        {
            dpath = current_path[1].position - current_path[0].position;
            raw_target = current_path[0].position;
        }
        else if (target_idx == (current_path.Count - 1))
        {
            dpath = current_path[target_idx].position - 
                    current_path[target_idx - 1].position;
            raw_target = current_path[target_idx].position;
        }
        else
        {
            dpath = current_path[target_idx + 1].position - 
                    current_path[target_idx - 1].position;
            raw_target = current_path[target_idx].position;
        }
        dpath.Normalize();

        Vector3 offset_vector = Vector3.Cross(
            new Vector3(0.0f, 1.0f, 0.0f), dpath) * 1.75f;
        target = raw_target + offset_vector;
        return DriveState.on_road;
    }

    public DriveState next_state(int target_idx)
    {
        if ((Time.time - last_update_time) >= update_period)
        {
            updatePath(target_idx);
        }
        Vector3 target;
        DriveState to_return = calculate_target(out target);
        Vector3 to_target = target - transform.position;
        ctrl.target_direction = Mathf.Atan2(to_target.x, to_target.z) * 
                                180.0f / Mathf.PI;
        ctrl.target_speed = Mathf.Min(50.0f, to_target.magnitude);
        return to_return;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < current_path.Count - 1; i++)
        {
            Gizmos.DrawLine(current_path[i].position, 
                            current_path[i + 1].position);
        }
        if (current_path.Count > 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(current_path[0].position, 0.5f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(
                current_path[current_path.Count - 1].position,
                0.5f);
        }
    }
}
