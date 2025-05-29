using DDSS_LobbyGuard.SecurityExtension;
using Il2Cpp;
using Il2CppInterop.Runtime.Attributes;
using Il2CppProps;
using Il2CppProps.Keys;
using Il2CppProps.Scripts;
using Il2CppProps.Shelf;
using Il2CppProps.Stereo;
using MelonLoader;
using System;
using UnityEngine;

namespace DDSS_LobbyGuard.Components
{
    [ClassInjectionAssemblyTarget("DDSS.LobbyGuard")]
    public class CollectibleSecurityHandler : MonoBehaviour
    {
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

        internal static bool Register()
            => MelonMain.RegisterComponent<CollectibleSecurityHandler>();
        public CollectibleSecurityHandler(IntPtr ptr) : base(ptr) { }

        public void Update()
        {

        }

        public void OnDestroy()
        {
            if ((collectibleType == eCollectibleType.NO_CALLBACK)
                || (gameObject == null)
                || gameObject.WasCollected
                || !gameObject.scene.isLoaded
                || !CollectibleSecurity._holderSpawnCache.TryGetValue(gameObject, out CollectibleHolder holder)
                || (holder == null)
                || holder.WasCollected)
                return;

            CollectibleSecurity._holderSpawnCache.Remove(gameObject);

            switch (collectibleType)
            {
                case eCollectibleType.CD:
                    CDHolder cdholder = holder.TryCast<CDHolder>();
                    if ((cdholder != null)
                        && !cdholder.WasCollected)
                        CollectibleSecurity.SpawnCD(cdholder, extraIndex);
                    break;

                case eCollectibleType.KEYS:
                    KeyHolder keyholder = holder.TryCast<KeyHolder>();
                    if ((keyholder != null)
                        && !keyholder.WasCollected)
                        CollectibleSecurity.SpawnKey(keyholder);
                    break;

                case eCollectibleType.BINDER:
                    OfficeShelf binderholder = holder.TryCast<OfficeShelf>();
                    if ((binderholder != null)
                        && !binderholder.WasCollected)
                        CollectibleSecurity.SpawnBinder(binderholder, extraIndex);
                    break;

                case eCollectibleType.FIRE_EX:
                    FireExHolder fireexholder = holder.TryCast<FireExHolder>();
                    if ((fireexholder != null)
                        && !fireexholder.WasCollected)
                        CollectibleSecurity.SpawnFireEx(fireexholder);
                    break;

                case eCollectibleType.STORAGE_BOX:
                    ShelfController boxHolder = holder.TryCast<ShelfController>();
                    if ((boxHolder != null)
                        && !boxHolder.WasCollected)
                        CollectibleSecurity.SpawnStorageBox(boxHolder, extraIndex);
                    break;

                default:
                    break;

            }
        }
    }
}
