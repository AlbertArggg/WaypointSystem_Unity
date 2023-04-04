using System.Collections.Generic;
using UnityEngine;

namespace Pathfinder
{
    // GetTypeInChildren is a static class used for searching and returning a list of Transform objects
    // containing a specified component type within a given Transform and its child hierarchy.
    public static class GetTypeInChildren
    {
        // GetType is a generic method that takes a Transform and returns a list of Transforms
        // containing the specified component type T in the given Transform and its child hierarchy.
        public static List<Transform> GetType<T>(Transform transform) where T : Component
        {
            List<Transform> list = new List<Transform>();
            TraverseChildren<T>(transform, list);
            return list;
        }

        // TraverseChildren is a private recursive helper method that traverses the child hierarchy
        // of a given Transform, adding any Transform objects containing the specified component
        // type T to the provided list.
        private static void TraverseChildren<T>(Transform currentTransform, List<Transform> list) where T : Component
        {
            if (currentTransform.GetComponent<T>() != null)
            {
                list.Add(currentTransform);
            }

            for (int i = 0; i < currentTransform.childCount; i++)
            {
                Transform childTransform = currentTransform.GetChild(i);
                TraverseChildren<T>(childTransform, list);
            }
        }
    }
}