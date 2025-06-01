using System;

/*
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
*/

namespace DDSS_LobbyGuard.Modules.Security.Object
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Object";
        public override eModuleType ModuleType => eModuleType.Security;
        public override Type ConfigType => typeof(ModuleConfig);

        /*
        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            var scene = SceneManager.GetActiveScene();

            var rootObjs = scene.GetRootGameObjects();
            List<Collider> allColliders = new();
            foreach (var obj in rootObjs)
                FindAllColliders(ref allColliders, obj);

            Vector3 minPos = new();
            Vector3 maxPos = new();
            foreach (var collider in allColliders)
            {
                Vector3 colliderPosition = collider.transform.position;
                CompareMinMaxVec(ref minPos, colliderPosition, true);
                CompareMinMaxVec(ref maxPos, colliderPosition, false);

                var colliderBounds = collider.bounds;

            }
        }

        private static void CompareMinMaxVec(ref Vector3 input,
            Vector3 targetPos,
            bool lessThan)
        {
            if (lessThan)
            {
                if (targetPos.x < input.x)
                    input.x = targetPos.x;
                if (targetPos.y < input.y)
                    input.y = targetPos.y;
                if (targetPos.z < input.z)
                    input.z = targetPos.z;
            }
            else
            {
                if (targetPos.x > input.x)
                    input.x = targetPos.x;
                if (targetPos.y > input.y)
                    input.y = targetPos.y;
                if (targetPos.z > input.z)
                    input.z = targetPos.z;
            }
        }

        private static void FindAllColliders(ref List<Collider> allColliders, GameObject target)
        {
            if ((target == null)
                || target.WasCollected)
                return;

            Collider[] foundColliders = target.GetComponents<Collider>();
            if (foundColliders.Length > 0)
                foreach (var collider in foundColliders)
                {
                    if ((collider == null)
                        || collider.WasCollected)
                        continue;
                    allColliders.Add(collider);
                }

            int childCount = target.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = target.transform.GetChild(i);
                if ((child == null)
                    || child.WasCollected)
                    continue;
                FindAllColliders(ref allColliders, child.gameObject);
            }
        }
        */
    }
}