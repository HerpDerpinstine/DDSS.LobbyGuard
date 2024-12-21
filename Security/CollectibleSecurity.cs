using DDSS_LobbyGuard.Components;
using DDSS_LobbyGuard.Utils;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppPlayer.Tasks;
using Il2CppProps;
using Il2CppProps.FireEx;
using Il2CppProps.Keys;
using Il2CppProps.Misc.PaperTray;
using Il2CppProps.PaperShredder;
using Il2CppProps.Printer;
using Il2CppProps.Scripts;
using Il2CppProps.Shelf;
using Il2CppProps.Stereo;
using Il2CppProps.StickyNote;
using Il2CppProps.TrashBin;
using Il2CppProps.WC.Toilet;
using Il2CppProps.WorkStation.Mouse;
using Il2CppProps.WorkStation.Scripts;
using Il2CppSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class CollectibleSecurity
    {
        private static Type _trashBinType = Il2CppType.Of<TrashBin>();

        internal static Dictionary<GameObject, CollectibleHolder> _holderSpawnCache = new();
        
        internal static void OnSceneLoad()
            => _holderSpawnCache.Clear();

        internal static bool CanPlace(NetworkIdentity player, CollectibleHolder holder, Collectible collectible)
        {
            // Validate Drop
            if (!InteractionSecurity.CanStopUseUsable(player, false))
                return false;

            // Get Holder Type
            Type holderType = holder.GetIl2CppType();

            // Check if Holder is Restricted
            if (!_whitelist.TryGetValue(holderType,
                out Dictionary<Type, bool> allowedCollectibleTypes))
                return false;

            // Get Collectible Type
            Type collectibleType = collectible.GetIl2CppType();

            // Validate Collectible
            if (allowedCollectibleTypes.ContainsKey(collectibleType))
                return true;

            // Handle TrashBin Specifically
            if (holderType == _trashBinType)
                return collectible.isTrashable;

            // Return False
            return false;
        }

        internal static void SpawnMug(GameObject prefab, MugHolder holder)
            => SpawnAndPlace<Mug, MugHolder>(prefab,
                holder,
                CollectibleDestructionCallback.eCollectibleType.NO_CALLBACK,
                null);
        
        internal static void SpawnMouse(GameObject prefab, MouseHolder holder)
            => SpawnAndPlace<Mouse, MouseHolder>(prefab,
                holder,
                CollectibleDestructionCallback.eCollectibleType.NO_CALLBACK,
                null);

        internal static void SpawnKey(KeyHolder holder)
            => SpawnAndPlace<KeyController, KeyHolder>(holder.keyPrefab,
                holder,
                CollectibleDestructionCallback.eCollectibleType.KEYS,
                null);

        internal static void SpawnFireEx(FireExHolder holder)
            => SpawnAndPlace<FireExController, FireExHolder>(holder.fireExControllerPrefab,
                holder,
                CollectibleDestructionCallback.eCollectibleType.FIRE_EX,
                null);

        internal static void SpawnStorageBoxStart(ShelfController shelf)
        {
            StorageBox prefabBox = shelf.storageBoxPrefab.GetComponent<StorageBox>();
            int boxCount = prefabBox.collectiblePrefabs.Count;
            for (int i = 0; i < boxCount; i++)
                SpawnStorageBox(shelf, i);
        }
        internal static void SpawnStorageBox(ShelfController shelf, int index)
            => SpawnAndPlace(shelf.storageBoxPrefab,
                shelf,
                CollectibleDestructionCallback.eCollectibleType.STORAGE_BOX,
                (StorageBox box) => box.NetworkprefabIndex = index,
                index);

        internal static void SpawnBinderStart(OfficeShelf shelf)
        {
            for (int i = 0; i < 10; i++)
                SpawnBinder(shelf, i);
        }
        internal static void SpawnBinder(OfficeShelf shelf, int index)
            => SpawnAndPlace(shelf.binderPrefab,
                shelf,
                CollectibleDestructionCallback.eCollectibleType.BINDER,
                (Binder binder) =>
                {
                    string title = $"{System.Enum.GetName(shelf.shelfCategory)} {index}";
                    binder.SetBinder(title, index);
                    binder.Networklabel = title;

                    int randomIndex = UnityEngine.Random.Range(0, Task.documents.Count);
                    string item = Task.documents[randomIndex].Item1;
                    string text = Resources.Load<TextAsset>("files/" + item).text;
                    binder.CmdAddDocumentServer(item, text);
                },
                index);

        internal static void SpawnCDStart(CDHolder holder)
        {
            for (int i = 0; i < 2; i++)
                SpawnCD(holder, i);
        }
        internal static void SpawnCD(CDHolder holder, int index)
            => SpawnAndPlace(holder.cdCasePrefab,
                holder,
                CollectibleDestructionCallback.eCollectibleType.CD,
                (CDCase cd) => cd.NetworksongIndex = index,
                index);


        private delegate void dSpawnAndPlace<T>(T obj) 
            where T : Collectible;
        private static void SpawnAndPlace<T, Z>(GameObject prefab, Z holder, CollectibleDestructionCallback.eCollectibleType type, dSpawnAndPlace<T> afterSpawn, int extraIndex = 0)
            where T : Collectible
            where Z : CollectibleHolder
            => holder.StartCoroutine(SpawnAndPlaceCoroutine(prefab,
                holder,
                (T obj) =>
                {
                    if (type != CollectibleDestructionCallback.eCollectibleType.NO_CALLBACK)
                    {
                        _holderSpawnCache[obj.gameObject] = holder;

                        CollectibleDestructionCallback callback = obj.gameObject.AddComponent<CollectibleDestructionCallback>();
                        callback.collectibleType = type;
                        callback.extraIndex = extraIndex;
                    }

                    if (afterSpawn != null)
                        afterSpawn(obj);
                }));
        private static IEnumerator SpawnAndPlaceCoroutine<T, Z>(GameObject prefab, Z holder, dSpawnAndPlace<T> afterSpawn)
            where T : Collectible
            where Z : CollectibleHolder
        {
            GameObject gameObject = GameObject.Instantiate(prefab, holder.transform.position, holder.transform.rotation);
            NetworkServer.Spawn(gameObject);

            yield return new WaitForSeconds(0.1f);

            T obj = gameObject.GetComponent<T>();
            holder.CmdPlaceCollectible(obj.netIdentity, obj.Networklabel);

            yield return new WaitForSeconds(0.1f);

            afterSpawn(obj);

            yield break;
        }

        private static Dictionary<Type, Dictionary<Type, bool>> _whitelist 
            = new Dictionary<Type, Dictionary<Type, bool>>()
            {
                {
                    Il2CppType.Of<BulletinBoard>(),
                    new Dictionary<Type, bool>()
                    {
                        { Il2CppType.Of<Document>(), true },
                        { Il2CppType.Of<PrintedImage>(), true }
                    }
                },

                {
                    Il2CppType.Of<CoffeeMachine>(),
                    new Dictionary<Type, bool>()
                    {
                        { Il2CppType.Of<Mug>(), true }
                    }
                },

                {
                    Il2CppType.Of<FireExHolder>(),
                    new Dictionary<Type, bool>()
                    {
                        { Il2CppType.Of<FireExController>(), true }
                    }
                },

                {
                    Il2CppType.Of<KeyHolder>(),
                    new Dictionary<Type, bool>()
                    {
                        { Il2CppType.Of<KeyController>(), true }
                    }
                },

                {
                    Il2CppType.Of<MouseHolder>(),
                    new Dictionary<Type, bool>()
                    {
                        { Il2CppType.Of<Mouse>(), true }
                    }
                },

                {
                    Il2CppType.Of<MugHolder>(),
                    new Dictionary<Type, bool>()
                    {
                        { Il2CppType.Of<Mug>(), true } 
                    }
                },

                {
                    Il2CppType.Of<OfficeShelf>(),
                    new Dictionary<Type, bool>()
                    {
                        { Il2CppType.Of<Binder>(), true },
                    }
                },

                {
                    Il2CppType.Of<PaperTray>(),
                    new Dictionary<Type, bool>()
                    {
                        { Il2CppType.Of<Document>(), true },
                        { Il2CppType.Of<PrintedImage>(), true }
                    }
                },
                
                {
                    Il2CppType.Of<PaperShredder>(),
                    new Dictionary<Type, bool>()
                    {
                        { Il2CppType.Of<Document>(), true },
                        { Il2CppType.Of<PrintedImage>(), true }
                    }
                },

                {
                    Il2CppType.Of<Printer>(),
                    new Dictionary<Type, bool>()
                    {
                        { Il2CppType.Of<Document>(), true },
                        { Il2CppType.Of<PrintedImage>(), true }
                    }
                },

                {
                    Il2CppType.Of<ShelfController>(),
                    new Dictionary<Type, bool>()
                    {
                        { Il2CppType.Of<StorageBox>(), true }
                    }
                },

                { 
                    Il2CppType.Of<StickyNoteHolder>(),
                    new Dictionary<Type, bool>()
                    {
                        { Il2CppType.Of<StickyNoteController>(), true } 
                    }
                },
                
                { 
                    Il2CppType.Of<StickyNoteDoorHolder>(),
                    new Dictionary<Type, bool>()
                    {
                        { Il2CppType.Of<StickyNoteController>(), true } 
                    }
                },

                {
                    Il2CppType.Of<ToiletPaperHolder>(),
                    new Dictionary<Type, bool>()
                    {
                        { Il2CppType.Of<ToiletPaper>(), true }
                    }
                },
            };
    }
}
