using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class wheel_control : MonoBehaviour
{
    public GameObject model;
    public WheelCollider wheel_coll;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 col_pos;
        Quaternion col_rot;
        wheel_coll.GetWorldPose(out col_pos, out col_rot);

        model.transform.position = col_pos;
        model.transform.rotation = col_rot;
    }
}
