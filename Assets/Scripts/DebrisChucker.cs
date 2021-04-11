using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This hurls debris after getting killed, or as damage gets inflicted
public enum state{
    waiting,
    triggered,
    timing,
    done
}


public class DebrisChucker : MonoBehaviour
{
    public HealthManager helth;
    public List<GameObject> debris_models;
    public state current_state;
    public int chunks_to_spawn;
    private float timer;
    private List<GameObject> spawned;

    // Start is called before the first frame update
    void Start()
    {
        current_state = state.waiting;
        spawned = new List<GameObject>();
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {}

    void FixedUpdate()
    {
        Collider coll = GetComponentInChildren<Collider>();

        if((helth.get_health() == 0) && (current_state == state.waiting))
        {
            spawned = spawn_debris();
            current_state = state.triggered;
            timer = 0.0f;
        }
        else if (current_state == state.triggered)
        {
            timer += Time.deltaTime;
            // Spawn an explosive force
            foreach (GameObject debris in spawned)
            {
                debris.GetComponent<Rigidbody>().AddExplosionForce(
                    50.0f, coll.bounds.center, 
                    0.0f, 0.1f);
            }
            if (timer > 0.1f)
            {
                current_state = state.timing;
                timer = 0.0f;
            }
        }
        else if (current_state == state.timing)
        {
            timer += Time.deltaTime;
            if (timer >= 11.0f)
            {
                foreach(GameObject debris in spawned)
                {
                    Destroy(debris);
                }
                current_state = state.done;
            }
        }
    }


    // Spawn debris models and start chucking them about the place.
    // Returns references to the models spawned.
    private List<GameObject> spawn_debris()
    {
        Collider coll = GetComponentInChildren<Collider>();
        Transform tform = GetComponent<Transform>();
        List<GameObject> spawned = new List<GameObject>();

        for (int i = 0; i < chunks_to_spawn; i++)
        {
            int choice = Mathf.RoundToInt(
                Random.Range(0.0f, debris_models.Count - 1));
            GameObject debris = debris_models[choice];
            Quaternion debris_rot = Random.rotationUniform;
            float spawn_x = Random.Range(-0.5f, 0.5f) * coll.bounds.size.x;
            float spawn_y = Random.Range(-0.5f, 0.5f) * coll.bounds.size.y;
            float spawn_z = Random.Range(-0.5f, 0.5f) * coll.bounds.size.z;

            Vector3 debris_pos = spawn_x * tform.right + 
                                 spawn_y * tform.up + 
                                 spawn_z * tform.forward + 
                                 coll.bounds.center;
            GameObject new_debris = Instantiate(debris, debris_pos, debris_rot);
            // Make sure the spawned collider is convex
            new_debris.GetComponent<MeshCollider>().convex = true;
            spawned.Add(new_debris);
        }
        return spawned;
    }

}
