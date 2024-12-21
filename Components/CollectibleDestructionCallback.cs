using DDSS_LobbyGuard.Security;
using Il2Cpp;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using Il2CppProps;
using Il2CppProps.Keys;
using Il2CppProps.Scripts;
using Il2CppProps.Shelf;
using Il2CppProps.Stereo;
using System;
using UnityEngine;

namespace DDSS_LobbyGuard.Components
{
    [ClassInjectionAssemblyTarget("DDSS.LobbyGuard")]
    public class CollectibleDestructionCallback : MonoBehaviour
    {
        internal static void Register()
            => ClassInjector.RegisterTypeInIl2Cpp<CollectibleDestructionCallback>();
        public CollectibleDestructionCallback(IntPtr ptr) : base(ptr) { }

        internal enum eCollectibleType
        {
            CD,
            KEYS,
            BINDER,
            FIRE_EX,
            STORAGE_BOX,

            NO_CALLBACK,
        }
        internal eCollectibleType collectibleType = eCollectibleType.NO_CALLBACK;
        internal int extraIndex;

        public void OnDestroy()
        {
            if ((collectibleType == eCollectibleType.NO_CALLBACK)
                || (gameObject == null)
                || gameObject.WasCollected
                || !gameObject.scene.isLoaded
                || !CollectibleSecurity._holderSpawnCache.TryGetValue(gameObject, out CollectibleHolder holder))
                return;

            CollectibleSecurity._holderSpawnCache.Remove(gameObject);

            switch (collectibleType)
            {
                case eCollectibleType.CD:
                    CollectibleSecurity.SpawnCD(holder.TryCast<CDHolder>(), extraIndex);
                    break;

                case eCollectibleType.KEYS:
                    CollectibleSecurity.SpawnKey(holder.TryCast<KeyHolder>());
                    break;

                case eCollectibleType.BINDER:
                    CollectibleSecurity.SpawnBinder(holder.TryCast<OfficeShelf>(), extraIndex);
                    break;

                case eCollectibleType.FIRE_EX:
                    CollectibleSecurity.SpawnFireEx(holder.TryCast<FireExHolder>());
                    break;

                case eCollectibleType.STORAGE_BOX:
                    CollectibleSecurity.SpawnStorageBox(holder.TryCast<ShelfController>(), extraIndex);
                    break;

                default:
                    break;

            }
        }
    }
}
