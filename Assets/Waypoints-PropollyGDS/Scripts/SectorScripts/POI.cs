using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinder
{
    public class POI : MonoBehaviour
    {
        // Declare public variables for the POI name and coordinates
        public string Name = "POI";
        [HideInInspector] public float x, y, z;

        // Awake method is called when the script instance is being loaded
        private void Awake()
        {
            // Set the x and y variables to match the POI's transform position
            x = this.transform.position.x;
            y = this.transform.position.z;
        }
    }
}