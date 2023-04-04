using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinder
{
    public class AIPatrolSystem : MonoBehaviour
    {
        // The speed at which the AI should move between points
        public float moveSpeed = 5f;

        // The time to wait before starting patrol
        public float patrolTime = 5f;

        // The type of path to follow
        public string pathType; // Subsector, Sector, other

        // The current index of the Transform position being patrolled towards
        private int currentPatrolPointIndex = 0;

        // The path object to follow
        private Path path;

        // The camera position to move towards
        public Transform camPos;

        // A boolean to indicate if the AI is patrolling
        bool patrol = false;

        // Function to initialize the AI patrol system
        public void Start()
        {
            if (pathType == null) { pathType = "Subsector"; }
            StartCoroutine(Initialize());
        }

        // Coroutine to initialize the AI patrol system
        IEnumerator Initialize()
        {
            // Wait for the patrol time
            yield return new WaitForSeconds(patrolTime);

            // Find the closest path to the AI's position
            path = FindObjectOfType<Pathways>().GetClosestPath(this.transform.position, pathType);

            // Start patrolling
            patrol = true;
        }

        // Coroutine to handle the behaviour of the AI while patrolling
        IEnumerator PatrolBehaviour()
        {
            // Stop patrolling
            patrol = false;

            // Wait for the patrol time
            yield return new WaitForSeconds(patrolTime);

            // Start patrolling again
            patrol = true;
        }

        // Function to update the AI's position
        void Update()
        {
            if (patrol)
            {
                // Check if the AI has reached its current patrol point
                if (Vector3.Distance(transform.position, path.pathway[currentPatrolPointIndex].position) < 2f)
                {
                    // Increment the index of the current patrol point, wrapping around to 0 when the end of the list is reached
                    currentPatrolPointIndex = (currentPatrolPointIndex + 1) % path.pathway.Count;

                    // Start coroutine for patrol behaviour
                    StartCoroutine(PatrolBehaviour());
                }

                // Move the AI towards its current patrol point
                transform.position = Vector3.MoveTowards(transform.position, path.pathway[currentPatrolPointIndex].position, moveSpeed * Time.deltaTime);
            }
        }
    }
}