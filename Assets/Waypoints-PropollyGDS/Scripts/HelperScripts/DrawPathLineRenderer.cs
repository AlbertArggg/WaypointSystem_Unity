using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinder
{
    public static class DrawPathLineRenderer
    {
        static Transform parent; // holds the parent transform for the path
        public static Transform DisplayPaths(List<Transform> _nodes, string Type, Material mat)
        {
            // create a new transform to hold the path
            parent = new GameObject(Type + " path").transform;

            // loop through each node in the list of nodes
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (i == _nodes.Count - 1) // if last node, connect it to the first node
                {
                    _nodes[i].name = Type + " " + (i + 1); // set node name
                    DrawPathLines(_nodes[i], _nodes[0], mat, Type); // draw line between last and first node
                }
                else // otherwise, connect it to the next node
                {
                    _nodes[i].name = Type + " " + (i + 1); // set node name
                    _nodes[i + 1].name = Type + " " + (i + 2); // set next node name
                    DrawPathLines(_nodes[i], _nodes[i + 1], mat, Type); // draw line between current node and next node
                }
            }
            return parent;
        }
        private static void DrawPathLines(Transform node1, Transform node2, Material mat, string type)
        {
            // create a new game object for the line renderer and set its position and parent
            GameObject lineRend = new GameObject("LR[" + node1.name + " - " + node2.name + "]");
            lineRend.transform.position = node2.transform.position;
            lineRend.transform.parent = parent;
            lineRend.AddComponent<LineRenderer>();

            // get the line renderer component and set the start and end positions
            LineRenderer line = lineRend.GetComponent<LineRenderer>();
            Vector3 pos = node1.transform.position;
            pos.y = 20;
            line.SetPosition(0, pos);
            pos = node2.transform.position;
            pos.y = 20;
            line.SetPosition(1, pos);

            // set the start and end width of the line renderer and the material
            line.startWidth = 2;
            line.endWidth = 2;
            line.material = mat;
        }
    }
}