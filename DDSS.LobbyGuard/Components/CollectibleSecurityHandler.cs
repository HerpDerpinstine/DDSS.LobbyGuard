using DDSS_LobbyGuard.SecurityExtension;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppInterop.Runtime.Attributes;
using Il2CppMirror;
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
    public class CollectibleSecurityHandler : MonoBehaviour
    {
        internal enum eCollectibleType
        {
            CD,
            KEYS,
            BINDER,
            FIRE_EX,
            STORAGE_BOX,
            CAMERA,

            NO_CALLBACK,
        }
        internal eCollectibleType collectibleType = eCollectibleType.NO_CALLBACK;
        internal int extraIndex;

        internal static bool Register()
            => MelonMain.RegisterComponent<CollectibleSecurityHandler>();
        public CollectibleSecurityHandler(IntPtr ptr) : base(ptr) { }

        /*
        public void Update()
        {

        }
        */

        private bool HolderCheck(out CollectibleHolder holder)
        {
            holder = null;

            if ((gameObject == null)
                || gameObject.WasCollected
                || !gameObject.scene.isLoaded
                || !CollectibleSecurity._holderSpawnCache.TryGetValue(gameObject, out holder)
                || (holder == null)
                || holder.WasCollected)
                return false;

            CollectibleSecurity._holderSpawnCache.Remove(gameObject);
            return true;
        }

        public void OnDestroy()
        {
            if (!NetworkServer.activeHost)
                return;
            if (collectibleType == eCollectibleType.NO_CALLBACK)
                return;

            CollectibleHolder holder = null;
            switch (collectibleType)
            {
                case eCollectibleType.CD:
                    if (HolderCheck(out holder))
                    {
                        CDHolder cdholder = holder.TryCast<CDHolder>();
                        if ((cdholder != null)
                            && !cdholder.WasCollected)
                            CollectibleSecurity.SpawnCD(cdholder, extraIndex);
                    }
                    break;

                case eCollectibleType.KEYS:
                    if (HolderCheck(out holder))
                    {
                        KeyHolder keyholder = holder.TryCast<KeyHolder>();
                        if ((keyholder != null)
                            && !keyholder.WasCollected)
                            CollectibleSecurity.SpawnKey(keyholder);
                    }
                    break;

                case eCollectibleType.BINDER:
                    if (HolderCheck(out holder))
                    {
                        OfficeShelf binderholder = holder.TryCast<OfficeShelf>();
                        if ((binderholder != null)
                            && !binderholder.WasCollected)
                            CollectibleSecurity.SpawnBinder(binderholder, extraIndex);
                    }
                    break;

                case eCollectibleType.FIRE_EX:
                    if (HolderCheck(out holder))
                    {
                        FireExHolder fireexholder = holder.TryCast<FireExHolder>();
                        if ((fireexholder != null)
                            && !fireexholder.WasCollected)
                            CollectibleSecurity.SpawnFireEx(fireexholder);
                    }
                    break;

                case eCollectibleType.STORAGE_BOX:
                    if (HolderCheck(out holder))
                    {
                        ShelfController boxHolder = holder.TryCast<ShelfController>();
                        if ((boxHolder != null)
                            && !boxHolder.WasCollected)
                            CollectibleSecurity.SpawnStorageBox(boxHolder, extraIndex);
                    }
                    break;

                case eCollectibleType.CAMERA:
                    if ((CollectibleSecurity._cameraPrefab != null)
                        && !CollectibleSecurity._cameraPrefab.WasCollected)
                        CollectibleSecurity.SpawnCamera(GameManager.instance);
                    break;

                default:
                    break;

            }
        }
    }
}
