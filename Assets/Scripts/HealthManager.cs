using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthManager : MonoBehaviour
{

    public float max_health;
    public float starting_health;
    public float health;

    // Start is called before the first frame update
    void Start()
    {
        health = starting_health;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void inflict_damage(float damage, DamageTypes type)
    {
        health = Mathf.Max(health - damage, 0.0f);
    }

    public float get_health()
    {
        return health;
    }

    public static HealthManager find_helth(GameObject obj)
    {
        // Try to find the HealthManager attached to the given gameobject. It 
        // traverses up the hierarchy until it finds one. If none exist, returns
        // null instead.
        HealthManager manager = obj.GetComponent<HealthManager>();
        while (manager == null)
        {
            if (obj.transform == obj.transform.root)
            {
                break;
            }
            obj = obj.transform.parent.gameObject;
            manager = obj.GetComponent<HealthManager>();
        }
        return manager;
    }
}
