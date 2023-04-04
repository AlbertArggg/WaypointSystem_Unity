using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinder
{
    public class Path
    {
        public string name;
        public string type;
        public Vector3 position;
        public List<Transform> pathway = new List<Transform>();

        // Constructor for the Path class, initializes the name, type, and pathway variables and calculates the middle point
        public Path(string _name, string _type, List<Transform> _pathway)
        {
            name = _name;
            type = _type;
            pathway = _pathway;
            position = CalculateMiddlePoint(pathway);
        }

        // Calculates the middle point of the path
        public Vector3 CalculateMiddlePoint(List<Transform> transforms)
        {
            Vector3 sum = Vector3.zero;
            foreach (Transform transform in transforms)
            {
                sum += transform.position;
            }
            Vector3 middlePoint = sum / transforms.Count;
            return middlePoint;
        }
    }   
}