using DDSS_LobbyGuard.Modules.Security.Door;
using Il2CppProps.Door;
using MelonLoader;
using System;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Fixes.DoorRework
{
    internal class ModuleMain : ILobbyModule
    {
        private const float _colliderSizeMin = 0.5f;

        public override string Name => "DoorRework";
        public override eModuleType ModuleType => eModuleType.Fixes;
        public override Type ConfigType => typeof(ModuleConfig);

        internal static void DoorStart()
        {
            foreach (var volume in UnityEngine.Object.FindObjectsByType<PlayerDetectionVolume>(FindObjectsSortMode.None))
            {
                if (volume == null
                    || volume.WasCollected)
                    continue;
                FixColliderSize(volume);
            }
        }

        internal static void FixColliderSize(PlayerDetectionVolume volume)
        {
            if (volume == null
                || volume.WasCollected)
                return;

            BoxCollider collider = volume.GetComponent<BoxCollider>();
            if (collider == null
                || collider.WasCollected)
                return;

            Vector3 size = collider.size;
            MelonLogger.Msg("OldSize: " + size.x + ", " + size.y + ", " + size.z);

            if (size.x < _colliderSizeMin)
                size.x = _colliderSizeMin;
            if (size.y < _colliderSizeMin)
                size.y = _colliderSizeMin;
            if (size.z < _colliderSizeMin)
                size.z = _colliderSizeMin;

            MelonLogger.Msg("NewSize: " + size.x + ", " + size.y + ", " + size.z);

            collider.size = size;
        }
    }
}