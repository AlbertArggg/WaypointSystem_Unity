using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Pathfinder
{
    public class Sector : MonoBehaviour
    {
        [Header("AutoCreate")]
        public bool AutoCreateSubsectors;
        public bool AutoFindPathways;

        [HideInInspector] public bool ShowSubsectorConnections = false;
        [HideInInspector] public bool ShowSubsectorPoiPaths = false;
        [HideInInspector] public bool ShowSectorPoiPath = false;

        [Header("Pois and Subsectors")]
        public List<Transform> Pois = new List<Transform>();
        public List<Transform> Subsectors = new List<Transform>();

        [Header("Sector Values [Used in Auto-Create]")]
        public int desiredSectorPoiLength;
        public List<Transform> minMaxPoints = new List<Transform>();
        public const int DefaultRange = 250;
        [HideInInspector] public float x0, x1, y0, y1;
        [HideInInspector] public Transform Pathways;

        private void Start()
        {
            // Check if Pathways is not set, then create or get the Pathways object
            if (Pathways == null)
            {
                Transform pathway = FindObjectOfType<Pathways>().transform;
                Pathways = CreateOrGetTransform(pathway, "Pathway", Vector3.zero, typeof(Pathways));
            }
            // Call the initial setup method to set up the Sector
            InitialSetup();
        }
        private void InitialSetup()
        {
            if (AutoCreateSubsectors) { AutoFindPathways = true; }
            // Set the desiredSectorPoiLength to at least 6
            desiredSectorPoiLength = Math.Max(desiredSectorPoiLength, 6);

            // populate Pois list from transform Hierarchy if Poi List is empty
            if (Pois.Count < 1) { Pois = GetTypeInChildren.GetType<POI>(this.transform); }

            // If autocreate subsectors is true, set the min/max range
            if (AutoCreateSubsectors)
            {
                var minMaxPointsArray = minMaxPoints.ToArray();
                (x0, y0) = minMaxPointsArray.Length > 0 ? (minMaxPointsArray.Min(p => p.position.x), minMaxPointsArray.Min(p => p.position.z)) : (-DefaultRange, -DefaultRange);
                (x1, y1) = minMaxPointsArray.Length > 0 ? (minMaxPointsArray.Max(p => p.position.x), minMaxPointsArray.Max(p => p.position.z)) : (DefaultRange, DefaultRange);
                CreateSubSectors();
            }
            else
            {
                // Populate Subsectors list from transform hierarchy if empty
                if (Subsectors.Count < 1) { Subsectors = GetTypeInChildren.GetType<Subsector>(this.transform); }

                // Get each subsector's Pois from hierarchy if empty //int dspl, float x0, float x1, float y0, float y1, Transform p
                foreach (Transform t in Subsectors) { t.GetComponent<Subsector>().Init(0, 0, 0, 0, 0, this.transform, null); }
            }
            StartCoroutine(ConnectSubsectors());
        }
        IEnumerator ConnectSubsectors()
        {
            // Wait for 0.2 seconds before executing the rest of the method
            yield return new WaitForSeconds(0.2f);

            // If AutoFindPathways is true, organize and reorder the Subsectors
            if (AutoFindPathways)
            {
                Subsectors = TConnector.OragnizeAndReorder(Subsectors, this.transform.position);
            }

            // Add a new path connecting the subsectors to the Pathways object
            Pathways.GetComponent<Pathways>().AddNewPath(new Path(this.transform.name, "SubsectorPath", Subsectors));

            // Create the Point of Interest (POI) pathway
            CreatePoiPathway();
        }
        private void CreateSubSectors()
        {
            // Create or get a new Subsector Transform object with the given name, position, and component type
            Transform obs = CreateOrGetTransform(null, "Subsector",
                new Vector3((x0 + x1) / 2, this.transform.position.y, (y0 + y1) / 2), typeof(Subsector));

            // Set the parent of each Point of Interest (POI) to the newly created Subsector Transform
            Pois.ForEach(poi => poi.transform.SetParent(obs.transform));

            // Initialize the new Subsector with the provided parameters
            obs.GetComponent<Subsector>().Init(desiredSectorPoiLength, x0, x1, y0, y1, this.transform, Pois);
        }
        private Transform CreateOrGetTransform(Transform transform, string name, Vector3 position, Type componentType)
        {
            // Check if the provided Transform is null
            if (transform == null)
            {
                // If null, create a new GameObject with the given name and set its Transform component
                transform = new GameObject(name).transform;

                // Set the parent of the new Transform to the current object's Transform
                transform.SetParent(this.transform);

                // Set the position of the new Transform to the given position
                transform.position = position;

                // If a component type is provided, add the component to the new GameObject
                if (componentType != null) { transform.gameObject.AddComponent(componentType); }
            }

            // Return the created or existing Transform
            return transform;
        }
        private void CreatePoiPathway()
        {
            // Create a new list of POI Transforms
            List<Transform> POIs = new List<Transform>();

            // If AutoFindPathways is enabled
            if (AutoFindPathways)
            {
                // Iterate through all Subsectors
                for (int i = 0; i < Subsectors.Count; i++)
                {
                    // If a Subsector has only one POI, add it to the POIs list
                    if (Subsectors[i].GetComponent<Subsector>().Pois.Count == 1)
                    {
                        POIs.Add(Subsectors[i].GetComponent<Subsector>().Pois[0]);
                    }
                    else
                    {
                        // Calculate the indices of the previous and next Subsectors
                        int prevIndex = (i + Subsectors.Count - 1) % Subsectors.Count;
                        int nextIndex = (i + 1) % Subsectors.Count;

                        // Find the closest POI in the current Subsector to the last POI in the POIs list
                        Transform firstPOI = null;
                        float minDistance = float.MaxValue;
                        foreach (Transform poi in Subsectors[i].GetComponent<Subsector>().Pois)
                        {
                            float distance = Vector3.Distance(poi.position, Subsectors[prevIndex].transform.position);
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                firstPOI = poi;
                            }
                        }

                        // Find the closest POI in the current Subsector to the next Subsector
                        Transform lastPOI = null;
                        minDistance = float.MaxValue;
                        foreach (Transform poi in Subsectors[i].GetComponent<Subsector>().Pois)
                        {
                            if (poi == firstPOI) { continue; }
                            float distance = Vector3.Distance(poi.position, Subsectors[nextIndex].transform.position);
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                lastPOI = poi;
                            }
                        }

                        // Add POIs consecutively to the POIs list, starting from firstPOI and ending with lastPOI
                        POIs.Add(firstPOI);
                        foreach (Transform poi in Subsectors[i].GetComponent<Subsector>().Pois)
                        {
                            if (poi == firstPOI || poi == lastPOI) continue;
                            POIs.Add(poi);
                        }
                        POIs.Add(lastPOI);
                    }

                    // Check for crossing paths and update the Pois list
                    Pois = TConnector.CheckForCrossingPaths(POIs);
                }
            }
            // If AutoFindPathways is disabled
            else
            {
                // Add all POIs from each Subsector to the POIs list
                for (int i = 0; i < Subsectors.Count; i++)
                {
                    for (int j = 0; j < Subsectors[i].GetComponent<Subsector>().Pois.Count; j++)
                    {
                        POIs.Add(Subsectors[i].GetComponent<Subsector>().Pois[j]);
                    }
                }
                // Update the Pois list
                Pois = POIs;
            }

            // Create a new Path object with the POIs list and add it to the Pathways component
            Path p = new Path(this.transform.name, "Sector", Pois);
            Pathways.GetComponent<Pathways>().AddNewPath(p);
        }
    }
}