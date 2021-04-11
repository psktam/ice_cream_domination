using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Start is called before the first frame update
    public Component transform_comp;
    public truck_control truck;
    public int smoothing_window;

    private Queue<Vector3> truck_headings;

    void Start()
    {
        truck_headings = new Queue<Vector3>(smoothing_window);
    }

    // Update is called once per frame
    void Update()
    {
        truck_headings.Enqueue(truck.transform.forward);
        while (truck_headings.Count > smoothing_window)
        {
            truck_headings.Dequeue();
        }

        Vector3 smoothed_heading = new Vector3(0.0f, 0.0f, 0.0f);
        foreach (Vector3 heading in truck_headings)
        {
            smoothed_heading += heading;
        }
        smoothed_heading /= Mathf.Max(1.0f, truck_headings.Count);

        // Project the truck's negative Z vector to the global X/Z plane, 
        // and find out how much you need to rotate the global Z axis in order 
        // to line up with it.
        float rotate_angle = Mathf.Atan2(smoothed_heading.x, 
                                         smoothed_heading.z);

        // doing is rotating about the Y axis
        Quaternion rot = new Quaternion(
            0.0f, 
            Mathf.Sin(rotate_angle * 0.5f), 
            0.0f, 
            Mathf.Cos(rotate_angle * 0.5f));

        transform_comp.transform.SetPositionAndRotation(
            truck.transform.position, rot);
    }
}
