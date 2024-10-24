using DDSS_LobbyGuard.Components;
using Il2Cpp;
using Il2CppMirror;
using Il2CppProps.FireEx;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class FireExSecurity
    {
        private static Dictionary<FireExController, FireExHolder> _allFireExs = new();

        internal static void OnSceneLoad()
            => _allFireExs.Clear();

        internal static void SpawnFireEx(FireExHolder holder)
        {
            if ((holder == null)
                || holder.WasCollected
                || !holder.isServer)
                return;

            GameObject gameObject = GameObject.Instantiate(holder.fireExControllerPrefab, holder.transform.position, holder.transform.rotation);
            NetworkServer.Spawn(gameObject);
            gameObject.AddComponent<FireExDestructionCallback>();

            FireExController fireEx = gameObject.GetComponent<FireExController>();
            holder.CmdPlaceCollectible(fireEx.netIdentity, fireEx.label);
            _allFireExs[fireEx] = holder;
        }

        internal static void SpawnFireEx(FireExController fireEx)
        {
            if ((fireEx == null)
                || fireEx.WasCollected
                || !fireEx.isServer
                || !_allFireExs.ContainsKey(fireEx))
                return;

            FireExHolder holder = _allFireExs[fireEx];
            _allFireExs.Remove(fireEx);

            SpawnFireEx(holder);
        }
    }
}
