using DDSS_LobbyGuard.Components;
using Il2Cpp;
using Il2CppMirror;
using Il2CppProps.Keys;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class KeySecurity
    {
        private static Dictionary<KeyController, KeyHolder> _allKeys = new();

        internal static void OnSceneLoad()
            => _allKeys.Clear();

        internal static void SpawnKey(KeyHolder holder)
        {
            if ((holder == null)
                || holder.WasCollected
                || !holder.isServer)
                return;

            GameObject gameObject = GameObject.Instantiate(holder.keyPrefab, holder.transform.position, holder.transform.rotation);
            NetworkServer.Spawn(gameObject);
            gameObject.AddComponent<KeyDestructionCallback>();

            KeyController key = gameObject.GetComponent<KeyController>();
            holder.CmdPlaceCollectible(key.netIdentity, key.label);
            _allKeys[key] = holder;
        }

        internal static void SpawnKey(KeyController key)
        {
            if ((key == null)
                || key.WasCollected
                || !key.isServer
                || !_allKeys.ContainsKey(key))
                return;

            KeyHolder holder = _allKeys[key];
            _allKeys.Remove(key);

            SpawnKey(holder);
        }
    }
}
