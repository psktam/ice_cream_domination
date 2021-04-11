using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DataStructures.ViliWonka.KDTree;


[System.Serializable]
public struct TrafficNode
{
    public Vector3 position;
    public HashSet<TrafficNode> connected_to;

    public static TrafficNode from_pos(Vector3 pos)
    {
        TrafficNode to_return = new TrafficNode();
        to_return.position = pos;
        to_return.connected_to = new HashSet<TrafficNode>();
        return to_return;
    }

    public static void link(TrafficNode node1, TrafficNode node2)
    {
        node1.connected_to.Add(node2);
        node2.connected_to.Add(node1);
    }

    // Merge the two nodes together, provided that their positions are 
    // close enough together
    public static TrafficNode merge(TrafficNode node1, TrafficNode node2, 
                                    HashSet<TrafficNode> node_set,
                                    float eps=1.0e-9f)
    {
        if ((node1.position - node2.position).magnitude > eps)
        {
            throw new System.ArgumentException(
                "The provided nodes are not close enough " + 
                "to each other to be merged");
        }
        TrafficNode merged = TrafficNode.from_pos(
            (node1.position + node2.position) * 0.5f);
        merged.connected_to.UnionWith(node1.connected_to);
        merged.connected_to.UnionWith(node2.connected_to);
        // Remove the original nodes
        merged.connected_to.Remove(node1);
        merged.connected_to.Remove(node2);
        // Run through the adjacency list and replace all references to the 
        // original nodes with a reference to the new merged node
        foreach (TrafficNode neighbor in merged.connected_to)
        {
            neighbor.connected_to.Remove(node1);
            neighbor.connected_to.Remove(node2);
            neighbor.connected_to.Add(merged);
        }
        node_set.Remove(node1);
        node_set.Remove(node2);
        node_set.Add(merged);

        return merged;
    }
}


# if UNITY_EDITOR
[CustomEditor(typeof(TrafficManager))]
public class TrafficEditor : Editor {
    protected TrafficManager manager 
    {
        get 
        {
            return (TrafficManager) target;
        }
    }

    private Dictionary<GSDSplineN, GSDSplineC> mark_nodes_in_road(
        List<GSDSplineC> splines)
    {
        var spline_map = new Dictionary<GSDSplineN, GSDSplineC>();

        foreach (GSDSplineC spline in splines)
        {
            bool in_spline = false;
            bool just_left = false;
            foreach (GSDSplineN node in spline.mNodes)
            {
                if (node.bIsEndPoint && !in_spline)
                {
                    in_spline = true;
                }
                else if (node.bIsEndPoint && in_spline)
                {
                    just_left = true;
                }
                else if (just_left)
                {
                    in_spline = false;
                    just_left = false;
                }
                if (in_spline)
                {
                    spline_map[node] = spline;
                }
            }
        }
        return spline_map;
    }

    private HashSet<TrafficNode> reinterpolate(
        Dictionary<GSDSplineN, GSDSplineC> spline_map)
    {
        HashSet<GSDSplineN> in_road = new HashSet<GSDSplineN>(spline_map.Keys);
        var old_to_new_map = new Dictionary<GSDSplineN, TrafficNode>();
        var old_nodes_to_merge = new HashSet<(GSDSplineN, GSDSplineN)>();

        foreach (GSDSplineN old_node in in_road)
        {
            foreach (GSDSplineN nother_node in in_road)
            {
                if (old_node == nother_node)
                {
                    continue;
                }
                if (old_nodes_to_merge.Contains((nother_node, old_node)))
                {
                    continue;
                }
                if ((old_node.pos - nother_node.pos).magnitude <= 1e-3f)
                {
                    old_nodes_to_merge.Add((old_node, nother_node));
                }
            }
        }

        var all_nodes = new HashSet<TrafficNode>();
        var splines = new HashSet<GSDSplineC>(spline_map.Values);

        foreach (GSDSplineC spline in splines)
        {
            List<TrafficNode> nodes_this_spline = new List<TrafficNode>();
            // We will process the road segment by segment. Each segment is 
            // defined as the chunk of road between two nodes.
            // We reinterpolate the road in segments, keeping track of which
            // node will be connected to which node.
            GSDSplineN next_node = null;
            for (int i = 0; i < spline.mNodes.Count - 1; i++)
            {
                GSDSplineN node = spline.mNodes[i];
                if (!in_road.Contains(node)){ continue; }

                // We can at least include this node
                TrafficNode start_of_segment = TrafficNode.from_pos(node.pos);
                old_to_new_map[node] = start_of_segment;
                nodes_this_spline.Add(start_of_segment);

                next_node = spline.mNodes[i + 1];
                // Check if we need to interpolate. If the next node is not part
                // of the road, themn no need to do so.
                if (!in_road.Contains(next_node)){ continue ;}

                // Reinterpolate every 10 meters
                float p_start = node.tDist / spline.distance;
                float p_stop = next_node.tDist / spline.distance;
                float p_step = 10.0f / spline.distance;

                for (float p = p_start + p_step; p < p_stop; p += p_step)
                {
                    Vector3 cpos = spline.GetSplineValue(p);
                    nodes_this_spline.Add(TrafficNode.from_pos(cpos));
                }
            }
            if (in_road.Contains(next_node))
            {
                var new_node = TrafficNode.from_pos(next_node.pos);
                nodes_this_spline.Add(TrafficNode.from_pos(next_node.pos));
                old_to_new_map[next_node] = new_node;
            }

            // Now that we've built up the chain of nodes on this particular
            // spline, we'll pass through it one more time to link everything
            // together in a straight line.
            for (int i = 0; i < nodes_this_spline.Count - 1; i++)
            {
                var this_node = nodes_this_spline[i];
                var the_next_node = nodes_this_spline[i + 1];
                TrafficNode.link(this_node, the_next_node);
            }
            all_nodes.UnionWith(nodes_this_spline);
        }

        // Now merge everything. Typically intersections
        foreach ((var node1, var node2) in old_nodes_to_merge)
        {
            TrafficNode.merge(old_to_new_map[node1], old_to_new_map[node2],
                              all_nodes, 1e-3f);
        }
        return all_nodes;
    }

    private void updateCallback()
    {
        // Called when update button is pressed
        var splines = new List<GSDSplineC>(
            GameObject.FindObjectsOfType<GSDSplineC>());

        var node_spline_map = mark_nodes_in_road(splines);
        var interpolated_nodes = reinterpolate(node_spline_map);

        // Stick the interpolated nodes into a KD tree. We might need to 
        // map vectors to raw indices.
        var vecarray = new Vector3[interpolated_nodes.Count];
        int idx = 0;
        foreach (var node in interpolated_nodes)
        {
            vecarray[idx] = node.position;
            idx++;
        }
        manager.nodes = new List<TrafficNode>(interpolated_nodes);
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
#endif // UNITY_EDITOR

public class TrafficManager : MonoBehaviour
{
    public List<TrafficNode> nodes;
    public KDTree kdtree;
    public Dictionary<Vector3, TrafficNode> nodemap;

    // Start is called before the first frame update
    void Start()
    {
        nodemap = new Dictionary<Vector3, TrafficNode>(nodes.Count);
        var vecarray = new Vector3[nodes.Count];
        int idx = 0;
        foreach (var node in nodes)
        {
            vecarray[idx] = node.position;
            nodemap[node.position] = node;
            idx++;
        }
        kdtree = new KDTree(vecarray);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Return a list of all TrafficNodes that are in the sphere defined by the 
    // given inputs.
    public List<TrafficNode> query (Vector3 center, float radius)
    {
        KDQuery q = new KDQuery();
        List<int> indices = new List<int>();
        q.Radius(kdtree, center, radius, indices);
        var found_nodes = new List<TrafficNode>(indices.Count);
        for (int i = 0; i < indices.Count; i++)
        {
            found_nodes[i] = nodes[indices[i]];
        }
        return found_nodes;
    }

    # if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (nodes == null) { return; }
        var drawn = new HashSet<(TrafficNode, TrafficNode)>();
        var intersections = new HashSet<TrafficNode>();
        foreach (var node in nodes)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(node.position, 1.0f);
            Gizmos.color = Color.green;

            if (node.connected_to.Count > 2) { intersections.Add(node); }
            foreach (TrafficNode neighbor in node.connected_to)
            {
                if (drawn.Contains((neighbor, node)))
                {
                    continue;
                }
                drawn.Add((node, neighbor));
                Gizmos.DrawLine(node.position, neighbor.position);
            }
        }

        Gizmos.color = Color.red;
        foreach (var node in intersections)
        {
            foreach (var neighbor in node.connected_to)
            {
                Gizmos.DrawLine(node.position, neighbor.position);
            }
        }
    }
    # endif // UNITY_EDITOR
}
