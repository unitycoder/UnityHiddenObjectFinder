﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UObject = UnityEngine.Object;

namespace GBG.HiddenObjectFinder.Editor
{
    public static class HiddenObjectFinder
    {
        public static List<GameObject> FindGameObjects(HideFlags hideFlagsFilter = HideFlags.HideInHierarchy)
        {
            return FindAll<GameObject>(hideFlagsFilter);
        }

        public static List<GameObject> FindGameObjects(this Scene scene, HideFlags hideFlagsFilter = HideFlags.HideInHierarchy)
        {
            if (scene.isLoaded)
            {
                return new List<GameObject>(0);
            }

            List<GameObject> hiddenGoList = new List<GameObject>();
            foreach (GameObject rootGo in scene.GetRootGameObjects())
            {
                rootGo.FindHiddenGameObjectInHierarchy(hiddenGoList, hideFlagsFilter);
            }

            return hiddenGoList;
        }

        public static void FindHiddenGameObjectInHierarchy(this Transform rootGo, List<GameObject> resultForAppend, HideFlags hideFlagsFilter = HideFlags.HideInHierarchy)
        {
            if (resultForAppend == null)
            {
                throw new ArgumentNullException(nameof(resultForAppend));
            }

            if (!rootGo)
            {
                return;
            }

            FindHiddenGameObjectInHierarchy(rootGo.gameObject, resultForAppend, hideFlagsFilter);
        }

        public static void FindHiddenGameObjectInHierarchy(this GameObject rootGo, List<GameObject> resultForAppend, HideFlags hideFlagsFilter = HideFlags.HideInHierarchy)
        {
            if (resultForAppend == null)
            {
                throw new ArgumentNullException(nameof(resultForAppend));
            }

            if (!rootGo)
            {
                return;
            }

            if (rootGo.MatchHideFlags(hideFlagsFilter))
            {
                resultForAppend.Add(rootGo);
            }

            Transform rootTransform = rootGo.transform;
            for (int i = 0; i < rootTransform.childCount; i++)
            {
                GameObject childGo = rootTransform.GetChild(i).gameObject;
                FindHiddenGameObjectInHierarchy(childGo, resultForAppend, hideFlagsFilter);
            }
        }

        public static List<UObject> FindAll(HideFlags hideFlagsFilter = HideFlags.HideInHierarchy)
        {
            return FindAll<UObject>(hideFlagsFilter);
        }

        public static List<T> FindAll<T>(HideFlags hideFlagsFilter = HideFlags.HideInHierarchy) where T : UObject
        {
            T[] allObjects = UObject.FindObjectsOfType<T>();
            List<T> hiddenObjects = allObjects.Where(obj => obj.MatchHideFlags(hideFlagsFilter))
                .ToList();
            return hiddenObjects;
        }

        public static bool MatchHideFlags(this UObject obj, HideFlags hideFlagsFilter)
        {
            return obj && (obj.hideFlags & hideFlagsFilter) != 0;
        }

        public static string BuildHierarchyPath(this Transform transform)
        {
            if (!transform)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(transform.name);
            transform = transform.parent;
            while (transform)
            {
                builder.Insert(0, '/').Insert(0, transform.name);
                transform = transform.parent;
            }

            return builder.ToString();
        }

        public static string BuildHierarchyPath(this GameObject go)
        {
            if (!go)
            {
                throw new ArgumentNullException(nameof(go));
            }

            return BuildHierarchyPath(go.transform);
        }
    }
}