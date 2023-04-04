using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinder
{
    public static class TConnector
    {
        public static List<Transform> Nodes = new List<Transform>();
        public static Vector3 centerPoint = new Vector3(0, 0, 0);

        // Define a static method that organizes and reorders a list of nodes based on their distance from a center point
        public static List<Transform> OragnizeAndReorder(List<Transform> _nodes, Vector3 _C)
        {
            centerPoint = _C;
            if (_nodes.Count > 0)
            {
                if (_nodes.Count < 4)
                {
                    Nodes = _nodes;
                }
                else
                {
                    Nodes = GetFurthestSectorPoint(_nodes);
                    Nodes = ReorderNodes(Nodes);
                    Nodes = CheckForCrossingPaths(Nodes);
                }
            }
            return Nodes;
        }

        // Define a private method that finds the furthest node from the center point
        private static List<Transform> GetFurthestSectorPoint(List<Transform> _nodes)
        {
            int furthestNodeIndex = 0;
            float furthestSecDist = 0;

            // Loop through the nodes to find the furthest one
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (Vector3.Distance(_nodes[i].transform.position, centerPoint) > furthestSecDist)
                {
                    furthestNodeIndex = i;
                    furthestSecDist = Vector3.Distance(_nodes[i].transform.position, centerPoint);
                }
            }

            // Swap the furthest node with the first node in the list
            Transform tmp = _nodes[0];
            _nodes[0] = _nodes[furthestNodeIndex];
            _nodes[furthestNodeIndex] = tmp;

            return _nodes;
        }
        // ReorderNodes reorganizes the list of nodes based on the shortest distance between each node.
        private static List<Transform> ReorderNodes(List<Transform> _nodes)
        {
            List<Transform> path = new List<Transform>();
            path.Clear();

            // Initialize the variables for finding the shortest distance between nodes
            int visits = _nodes.Count;
            int nextNode = 0;
            int thisNode = 0;

            // Iterate through the nodes and find the closest node to the current node
            while (visits > 0)
            {
                path.Add(_nodes[thisNode]);
                float minDist = float.MaxValue;

                for (int j = 0; j < _nodes.Count; j++)
                {
                    if (thisNode == j || path.Contains(_nodes[j])) { continue; }

                    if (Vector3.Distance(_nodes[thisNode].transform.position, _nodes[j].transform.position) < minDist)
                    {
                        minDist = Vector3.Distance(_nodes[thisNode].transform.position, _nodes[j].transform.position);
                        nextNode = j;
                    }
                }
                visits--;
                thisNode = nextNode;
            }

            // Adjust the indexing of the reordered path to have the two furthest points at the beginning and end
            path = AdjustIndexing(path);
            return path;
        }

        // AdjustIndexing reorders the list of nodes to have the two consecutive nodes with the greatest distance between them at the beginning and end of the list.
        public static List<Transform> AdjustIndexing(List<Transform> _nodes)
        {
            int maxIndex = 0;
            float maxDistance = 0f;

            // Find the two consecutive subsectors with the greatest distance between them
            for (int i = 0; i < _nodes.Count - 1; i++)
            {
                float distance = Vector3.Distance(_nodes[i].transform.position, _nodes[i + 1].transform.position);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxIndex = i;
                }
            }

            // Reorder the list of subsectors so that the two furthest points are first and last
            List<Transform> reorderedSubsectors = new List<Transform>();
            reorderedSubsectors.Add(_nodes[maxIndex]);
            for (int i = maxIndex + 1; i < _nodes.Count; i++)
            {
                reorderedSubsectors.Add(_nodes[i]);
            }
            for (int i = 0; i < maxIndex; i++)
            {
                reorderedSubsectors.Add(_nodes[i]);
            }

            // Return the updated list of reordered subsectors
            return reorderedSubsectors;
        }

        // CheckForCrossingPaths detects and corrects any crossing paths in the given list of nodes.
        public static List<Transform> CheckForCrossingPaths(List<Transform> _nodes)
        {
            int count = _nodes.Count;

            // Iterate through the nodes and check if any two lines cross
            for (int i = 0; i < count; i++)
            {
                int prevIndex = (i + count - 1) % count;
                int nextIndex = (i + 1) % count;

                for (int j = i + 2; j < count; j++)
                {
                    int nextNextIndex = j % count;

                    Vector3 a0 = _nodes[i].position;
                    Vector3 a1 = _nodes[nextIndex].position;
                    Vector3 b0 = _nodes[j - 1].position;
                    Vector3 b1 = _nodes[nextNextIndex].position;

                    // If the lines cross, swap the positions of the corresponding nodes to correct the crossing
                    if (DoLinesCross(a0, a1, b0, b1))
                    {
                        Transform temp = _nodes[nextIndex];
                        _nodes[nextIndex] = _nodes[j - 1];
                        _nodes[j - 1] = temp;
                    }
                }
            }
            return _nodes;
        }

        // DoLinesCross determines if two line segments (a0-a1 and b0-b1) intersect.
        private static bool DoLinesCross(Vector3 a0, Vector3 a1, Vector3 b0, Vector3 b1)
        {
            float denominator = ((b1.z - b0.z) * (a1.x - a0.x)) - ((b1.x - b0.x) * (a1.z - a0.z));

            // If the denominator is 0, the lines are parallel and do not intersect
            if (denominator == 0)
            {
                return false;
            }

            float ua = (((b1.x - b0.x) * (a0.z - b0.z)) - ((b1.z - b0.z) * (a0.x - b0.x))) / denominator;
            float ub = (((a1.x - a0.x) * (a0.z - b0.z)) - ((a1.z - a0.z) * (a0.x - b0.x))) / denominator;

            // If ua and ub are within the range [0, 1], the line segments intersect
            if (ua < 0 || ua > 1 || ub < 0 || ub > 1)
            {
                return false;
            }
            return true;
        }
    }
}