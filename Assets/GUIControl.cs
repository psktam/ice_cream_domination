using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIControl : MonoBehaviour
{
    public truck_control truck;
    public Text speed_text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float speed = truck.rb.velocity.magnitude * 2.237f;
        speed_text.text = "Speed: " + speed + " mph";
    }
}
