using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PrefabPreviewer : MonoBehaviour
{
    // Set this to pick which model you want to preview
    public int preview_idx = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnDrawGizmos()
    {
        DamageModelTracker tracker = gameObject.GetComponent<DamageModelTracker>();

        MeshFilter[] meshes = tracker.damage_models[preview_idx].GetComponentsInChildren<MeshFilter>(true);
        if (meshes.Length == 0)
        {
            return;
        }

        Gizmos.color = Color.gray;
        Transform tform = meshes[0].GetComponent<Transform>();
        Vector3 pos = transform.TransformPoint(tform.position);
        Quaternion rot = transform.rotation * tform.rotation;

        Gizmos.DrawMesh(meshes[0].sharedMesh, pos, rot, Vector3.one);
    }
}
