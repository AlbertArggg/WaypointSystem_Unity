using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Pathfinder
{
    public class Manager : MonoBehaviour    // This class only used to display / view pathways
    {
        // Variables
        public Camera Main;
        public AIPatrolSystem[] aIPatrolSystems;
        public Transform MapViewPos;
        private int currIndex = 0;
        Transform followThis;
        bool follow = false;
        Color off;
        Color on;
        public Image WorldViewSprite;
        public Image SubSectorPath;
        public Image SectorPOIPath;
        public Image SubSectorPOIPath;

        [HideInInspector] public bool ShowSubsectorConnections = false;
        [HideInInspector] public bool ShowSubsectorPoiPaths = false;
        [HideInInspector] public bool ShowSectorPoiPath = false;
        [HideInInspector] public bool ShowOtherPoiPath = false;

        [Header("Line Renderer View Variables")]
        public Material[] materials;
        public Transform SubsectorConnections;
        public Transform POIconnectionsInSubsector;
        public Transform Poiconnections;
        public Transform other;

        private void Awake()
        {
            // get AI Patrol Systems
            aIPatrolSystems = FindObjectsOfType<AIPatrolSystem>();

            //Colors
            off = ColorUtility.TryParseHtmlString("#FF8383", out off) ? off : Color.red;
            on = ColorUtility.TryParseHtmlString("#B7FF83", out on) ? on : Color.green;

            // Create or get transforms for displaying connections
            SubsectorConnections = CreateOrGetTransform(SubsectorConnections, "Subsector Connections", Vector3.zero, null);
            POIconnectionsInSubsector = CreateOrGetTransform(POIconnectionsInSubsector, "POI Connections In Subsector", Vector3.zero, null);
            Poiconnections = CreateOrGetTransform(Poiconnections, "POI Connections", Vector3.zero, null);
            other = CreateOrGetTransform(other, "Other Connections", Vector3.zero, null);
        }
        private void Update()
        {
            // camera follow logic
            if (follow)
            {
                Main.transform.position = followThis.position;
                Main.transform.rotation = followThis.rotation;
            }

            // turn on / off liner renders
            SubsectorConnections.gameObject.SetActive(ShowSubsectorConnections);
            POIconnectionsInSubsector.gameObject.SetActive(ShowSubsectorPoiPaths);
            Poiconnections.gameObject.SetActive(ShowSectorPoiPath);
            other.gameObject.SetActive(ShowOtherPoiPath);
        }
        private Transform CreateOrGetTransform(Transform transform, string name, Vector3 position, Type componentType)
        {
            // create object if does not exist
            if (transform == null)
            {
                transform = new GameObject(name).transform;
                transform.SetParent(this.transform);
                transform.position = position;
                if (componentType != null) { transform.gameObject.AddComponent(componentType); }
            }
            return transform;
        }
        public void EnableSubsectorPath()
        {
            // display subsector path
            if (ShowSubsectorConnections == true)
            {
                ShowSubsectorConnections = false;
                SubSectorPath.color = off;
            }
            else
            {
                ShowSubsectorConnections = true;
                ShowSubsectorPoiPaths = false;
                ShowSectorPoiPath = false;
                SubSectorPath.color = on;
                SubSectorPOIPath.color = off;
                SectorPOIPath.color = off;
            }
        }
        public void EnableSectorPOIPath()
        {
            // display sector POI path
            if (ShowSectorPoiPath == true)
            {
                ShowSectorPoiPath = false;
                SectorPOIPath.color = off;
            }
            else
            {
                ShowSubsectorConnections = false;
                ShowSubsectorPoiPaths = false;
                ShowSectorPoiPath = true;
                SectorPOIPath.color = on;
                SubSectorPath.color = off;
                SubSectorPOIPath.color = off;
            }
        }
        public void EnableSubsectorPOIPath()
        {
            // display subsector POI path
            if (ShowSubsectorPoiPaths == true)
            {
                ShowSubsectorPoiPaths = false;
                SubSectorPOIPath.color = off;
            }
            else
            {
                ShowSubsectorConnections = false;
                ShowSubsectorPoiPaths = true;
                ShowSectorPoiPath = false;
                SubSectorPOIPath.color = on;
                SectorPOIPath.color = off;
                SubSectorPath.color = off;
            }
        }
        public void SetWorldView()
        {
            // camera set to original world view
            if (follow == true)
            {
                follow = false;
                Main.orthographic = true;
                Main.transform.position = MapViewPos.position;
                Main.transform.rotation = MapViewPos.rotation;
                WorldViewSprite.color = off;
            }
            else
            {
                follow = true;
                Main.orthographic = false;
                followThis = aIPatrolSystems[currIndex].camPos;
                WorldViewSprite.color = on;
            }
        }
        public void PrevAIView()
        {
            // camera follow previous AI character
            follow = true;
            Main.orthographic = false;
            currIndex--;
            if (currIndex < 0) { currIndex = aIPatrolSystems.Length - 1; }
            followThis = aIPatrolSystems[currIndex].camPos;
            WorldViewSprite.color = off;
        }
        public void NextAIView()
        {
            // camera follow next AI character
            follow = true;
            Main.orthographic = false;
            currIndex++;
            if (currIndex == aIPatrolSystems.Length) { currIndex = 0; }
            followThis = aIPatrolSystems[currIndex].camPos;
            WorldViewSprite.color = off;
        }
        public void DrawPaths(Path p)
        {
            // calls DrawPathLineRenderer DisplayPaths to create line renders for pathways
            Transform t = DrawPathLineRenderer.DisplayPaths(p.pathway, p.type, materials[Random.Range(0, materials.Length)]);
            switch (p.type)
            {
                case "Sector":
                    t.SetParent(Poiconnections);
                    break;

                case "Subsector":
                    t.SetParent(POIconnectionsInSubsector);
                    break;

                case "SubsectorPath":
                    t.SetParent(SubsectorConnections);
                    break;

                default:
                    t.SetParent(other);
                    break;
            }
        }
    }
}
