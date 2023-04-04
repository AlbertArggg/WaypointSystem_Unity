using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Pathfinder
{
    public class Pathways : MonoBehaviour
    {
        public List<Path> paths = new List<Path>(); // Declare public list of 'Path' objects
        public Manager manager;                     // Declare public 'Manager' object
        bool ManagerExists = false;                 // Declare boolean flag to check if 'Manager' object exists

        // 'Start' method is called when the script instance is being loaded
        public void Start()
        {
            // Find 'Manager' object in the scene
            manager = FindObjectOfType<Manager>();

            // Set 'ManagerExists' flag to true if 'Manager' object is found
            if (manager != null) { ManagerExists = true; }
        }

        // Method to find the closest 'Path' object to a given position and type
        public Path GetClosestPath(Vector3 pos, string type)
        {
            // Initialize variables to store minimum distance and corresponding path
            float minDistance = float.MaxValue;
            Path path = null;

            // Iterate through each 'Path' object in the list
            foreach (Path p in paths)
            {
                // Check if the path type matches the required type
                if (p.type != type) { continue; }

                // Update minimum distance and corresponding path if current path is closer
                if (Vector3.Distance(pos, p.position) < minDistance)
                {
                    minDistance = Vector3.Distance(pos, p.position);
                    path = p;
                }
            }

            // Return the closest 'Path' object
            return path;
        }

        public void AddNewPath(Path p)
        {
            // Add new 'Path' object to the list
            paths.Add(p);

            // Draw paths using the 'Manager' object if it exists
            if (ManagerExists) { manager.DrawPaths(p); }
        }
    }
}   
