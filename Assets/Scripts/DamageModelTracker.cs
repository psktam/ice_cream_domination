using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageModelTracker: MonoBehaviour
{
    // Class that loads the models and colliders necessary with health.

    // damage_models and health_setpoints must be the same length. 
    // Additionally, health_setpoints must be strictly decreasing, starting
    // at 1.
    // These two variables represent what model should be shown for what 
    // fraction of health remains for the building
    public HealthManager helth;
    public List <GameObject> damage_models;
    public List <float> health_setpoints;
    public bool force_convex = true;

    // Index of health setpoint/model that is currently relevant
    private int active_idx = 0;
    private Collider active_collider;
    private GameObject current_model;

    // Determine which model/setpoint to use
    private int determine_health_idx()
    {
        float health_fraction = helth.get_health() / helth.max_health;
        int idx = 0;
        for (int test_idx=0; test_idx < health_setpoints.Count; test_idx++)
        {
            if (health_fraction > health_setpoints[test_idx])
            {
                break;
            }
            idx = test_idx;
        }
        return idx;
    }

    // Start is called before the first frame update
    void Start()
    {
        active_idx = determine_health_idx();
        setModel(active_idx);
    }
    
    /**
     * Set the model. Handles changing the model if necessary and enforcing 
     * collider settings.
     */
    private void setModel(int idx)
    {
        // Destroy the model if it's not set already
        if (current_model) Destroy(current_model.gameObject);
        current_model = Instantiate(damage_models[idx], transform);
        // Add a collider to the model;
        
        MeshCollider coll = current_model.GetComponent<MeshCollider>();
        if ((coll == null) && (idx < (health_setpoints.Count - 1)))
        {
            coll = current_model.AddComponent<MeshCollider>() as MeshCollider;
            coll.convex = force_convex;
        }
        active_collider = coll;
    }

    // Invoke this when we hit 0 health.
    private void Die()
    {
    }

    // Update is called once per frame
    void Update()
    {
        int new_idx = determine_health_idx();
        if (new_idx != active_idx)
        {
            setModel(new_idx);
        }
        active_idx = new_idx;
        if (helth.get_health() == 0.0f)
        {
            Die();
        }
    }
}
