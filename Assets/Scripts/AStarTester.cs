using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


# if UNITY_EDITOR
[CustomEditor(typeof(AStarTester))]
public class PathExplorer : Editor {
    protected AStarTester tester
    {
        get
        {
            return (AStarTester) target;
        }
    }

    private void updateCallback()
    {
        var path = tester.mgr.calculate_path(tester.start_idx, tester.stop_idx);
        tester.path = path;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Update"))
        {
            updateCallback();
        }
    }
}
#endif

public class AStarTester : MonoBehaviour
{
    public List<int> path;
    public TrafficManager mgr;
    public int start_idx;
    public int stop_idx;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnDrawGizmosSelected()
    {
        var start = mgr.nodes[path[0]];
        var stop = mgr.nodes[path[path.Count - 1]];
        Gizmos.color = Color.green;
        for (int i = 0; i < path.Count - 1; i++)
        {
            var curr = mgr.nodes[path[i]];
            var next = mgr.nodes[path[i + 1]];
            Gizmos.DrawLine(curr.position, next.position);
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(start.position, 1.0f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(stop.position, 1.0f);
    }
}
