using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Pathfinder
{
    public class Subsector : MonoBehaviour
    {
        // Variables
        public bool finalFormSector;
        public List<Transform> Pois = new List<Transform>();
        Transform subSectorLineRenderedPath;
        Transform parent;
        int desiredSectorPoiLength;
        float x0, x1, y0, y1;

        // Initialize Subsector
        public void Init(int dspl, float x0, float x1, float y0, float y1, Transform p, List<Transform> pois)
        {
            // Set variables
            Pois = pois;
            parent = p;
            this.transform.parent = parent;
            subSectorLineRenderedPath = CreateOrGetTransform(subSectorLineRenderedPath,
                                    "Subsector POI Connections", this.transform.position, null);

            // If AutoCreateSubsectors is enabled, divide subsector
            if (parent.GetComponent<Sector>().AutoCreateSubsectors)
            {
                desiredSectorPoiLength = dspl;
                this.x0 = x0; this.x1 = x1;
                this.y0 = y0; this.y1 = y1;
                BaseCaseOrRecursionLogic();
            }
            // If AutoFindPathways is enabled, find POIs and draw paths
            else if (parent.GetComponent<Sector>().AutoFindPathways)
            {
                Pois = GetTypeInChildren.GetType<POI>(this.transform);
                Pois = TConnector.OragnizeAndReorder(Pois, transform.position);
                StartCoroutine(AddPathsToPathways());
            }
            // If neither AutoCreateSubsectors nor AutoFindPathways are enabled, just draw paths
            else
            {
                Pois = GetTypeInChildren.GetType<POI>(this.transform);
                StartCoroutine(AddPathsToPathways());
            }
        }

        // Create or get transform
        private Transform CreateOrGetTransform(Transform transform, string name, Vector3 position, Type componentType)
        {
            if (transform == null)
            {
                transform = new GameObject(name).transform;
                transform.SetParent(this.transform);
                transform.position = position;
                if (componentType != null) { transform.gameObject.AddComponent(componentType); }
            }
            return transform;
        }

        // Divide the sector if there are too many POIs
        public void BaseCaseOrRecursionLogic()
        {
            if (Pois.Count < 1) { Destroy(gameObject); }
            else if (Pois.Count < desiredSectorPoiLength)
            {
                finalFormSector = true;
                parent.GetComponent<Sector>().Subsectors.Add(this.transform);
                Pois = TConnector.OragnizeAndReorder(Pois, transform.position);
                StartCoroutine(AddPathsToPathways());
            }
            else { DivideSector(); }
        }

        // Divide the sector
        public void DivideSector()
        {
            List<Transform> newPois = Pois;
            for (int i = 1; i <= 4; i++) { HandleQuadrant(i, newPois); }
            Destroy(gameObject);
        }

        // Handle each quadrant of the divided sector
        private void HandleQuadrant(int quad, List<Transform> newPois)
        {
            float[] q = CalculateNextSectorVals(quad, x0, x1, y0, y1);

            Vector3 sectorPos = new Vector3((q[0] + q[1]) / 2, 18, (q[2] + q[3]) / 2);
            Transform obs = CreateOrGetTransform(null, "Subsector", sectorPos, typeof(Subsector));

            List<Transform> inSector = newPois
                .Where(p => p.GetComponent<POI>().x > q[0] && p.GetComponent<POI>().x <= q[1]
                         && p.GetComponent<POI>().y > q[2] && p.GetComponent<POI>().y <= q[3])
                .ToList();

            foreach (Transform poi in inSector)
            {
                poi.transform.SetParent(obs, true);
            }

            obs.GetComponent<Subsector>().Init(desiredSectorPoiLength, q[0], q[1], q[2], q[3], parent, inSector);
        }

        // get next sector quadrants values
        public float[] CalculateNextSectorVals(int sector, float x0, float x1, float y0, float y1)
        {
            float midX = (x0 + x1) / 2;
            float midY = (y0 + y1) / 2;

            float[] Q_vals = new float[4];
            switch (sector)
            {
                case 1: // Q1
                    Q_vals[0] = x0;
                    Q_vals[1] = midX;
                    Q_vals[2] = midY;
                    Q_vals[3] = y1;
                    break;
                case 2: // Q2
                    Q_vals[0] = midX;
                    Q_vals[1] = x1;
                    Q_vals[2] = midY;
                    Q_vals[3] = y1;
                    break;
                case 3: // Q3
                    Q_vals[0] = x0;
                    Q_vals[1] = midX;
                    Q_vals[2] = y0;
                    Q_vals[3] = midY;
                    break;
                default: // Q4
                    Q_vals[0] = midX;
                    Q_vals[1] = x1;
                    Q_vals[2] = y0;
                    Q_vals[3] = midY;
                    break;
            }
            return Q_vals;
        }
        
        // Add paths to pathway
        IEnumerator AddPathsToPathways()
        {
            yield return new WaitForSeconds(0.1f);
            Sector parentSector = parent.GetComponent<Sector>();
            parentSector.Pathways.GetComponent<Pathways>().AddNewPath(new Path(this.transform.name, "Subsector", Pois));
        }
    }
}