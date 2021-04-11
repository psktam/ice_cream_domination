using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelControl : MonoBehaviour
{

    public Rigidbody wheel_rb;
    public Collider wheel_collider;
    public HashSet<Collider> contacts;

    void OnTriggerEnter(Collider other)
    {
        contacts.Add(other);
    }

    void OnTriggerExit(Collider other)
    {
        contacts.Remove(other);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool is_colliding()
    {
        return contacts.Count > 0;
    }
}
