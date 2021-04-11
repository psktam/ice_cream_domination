using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class NavMeshPreviewer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject for_navmesh;
    bool playing = false;
    void Start()
    {
        playing = true;
        Destroy(for_navmesh);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #if UNITY_EDITOR

    void OnDrawGizmos()
    {
        DamageModelTracker tracker = gameObject.GetComponent<DamageModelTracker>();
        if (playing)
        {
            return;
        }
        if (for_navmesh == null)
        {
            for_navmesh = Instantiate(tracker.damage_models[0], transform);
        }

        var flags = StaticEditorFlags.NavigationStatic;
        GameObjectUtility.SetStaticEditorFlags(for_navmesh, flags);
    }
    #endif
}
