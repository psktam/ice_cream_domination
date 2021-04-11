using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FireSpawner : MonoBehaviour
{

    public HealthManager helth;
    // List of particle objects we can spawn at random
    public List<GameObject> fire_particles;
    // Start is called before the first frame update

    // When health reaches these percentages of maximum health, spawns a new
    // fire instance. This must be ordered from 1.0 -> 0.0
    public List<float> spawn_points;

    // Indicate how long the fires are allowed to last after death
    public float duration_after_death;

    private List<GameObject> current_fires;
    public int active_idx;

    private int determine_health_idx()
    {
        float health_fraction = helth.get_health() / helth.max_health;
        int idx = -1;
        for (int test_idx=0; test_idx < spawn_points.Count; test_idx++)
        {
            if (health_fraction > spawn_points[test_idx])
            {
                break;
            }
            idx = test_idx;
        }
        return idx;
    }

    void Start()
    {
        active_idx = -1;
        current_fires = new List<GameObject>();
    }

    void extinguish()
    {
        // Call this to put out fires.
    }

    // Update is called once per frame
    void Update()
    {
        int new_idx = determine_health_idx();
        if (new_idx != active_idx)
        {
            spawn_flames();
        }
        active_idx = new_idx;

        if (helth.get_health() == 0.0f)
        {
            foreach (GameObject fire in current_fires)
            {
                if (fire != null)
                {
                    Destroy(fire, duration_after_death);
                }
            }
        }
    }

    private void spawn_flames()
    {
        // Spawn one new fire
        // First pick which fire to spawn
        GameObject to_spawn = fire_particles[Random.Range(
            0, fire_particles.Count)];
        // Then pick a place to put it.
        Bounds bounds = GetComponentInChildren<MeshRenderer>().bounds;
        float dx = Random.Range(-bounds.extents.x, bounds.extents.x);
        float dy = Random.Range(-bounds.extents.y, bounds.extents.y);
        float dz = Random.Range(-bounds.extents.z, bounds.extents.z);
        Transform render_tform = GetComponentInChildren<Transform>();
        Vector3 displacement = render_tform.right * dx + 
                               render_tform.up * dy +
                               render_tform.forward * dz;
        Vector3 spawn_pos = bounds.center + displacement;
        current_fires.Add(Instantiate(to_spawn, spawn_pos, new Quaternion()));
    }
}
