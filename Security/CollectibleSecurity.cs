using DDSS_LobbyGuard.Components;
using DDSS_LobbyGuard.Utils;
using Il2Cpp;
using Il2CppGameManagement;
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
            if (!BinderManager.instance.binders.ContainsKey(shelf.shelfCategory))
                BinderManager.instance.binders[shelf.shelfCategory] = new();
            for (int i = 0; i < 10; i++)
                SpawnBinder(shelf, i);
        }
        internal static void SpawnBinder(OfficeShelf shelf, int index)
            => SpawnAndPlace(shelf.binderPrefab,
                shelf,
                CollectibleDestructionCallback.eCollectibleType.BINDER,
                (Binder binder) =>
                {
                    if (!BinderManager.instance.binders.ContainsKey(shelf.shelfCategory))
                        BinderManager.instance.binders[shelf.shelfCategory] = new();
                    BinderManager.instance.binders[shelf.shelfCategory].Add(binder);

                    string title = LocalizationManager.instance.GetLocalizedValue($"{System.Enum.GetName(shelf.shelfCategory)} {index}");
                    binder.Networklabel = title;
                    binder.NetworkinteractableName = title;
                    binder.Networkcolor = binder.colors[UnityEngine.Random.Range(0, binder.colors.Count)];

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
            => holder.StartCoroutine(SpawnAndPlaceCoroutine(prefab, holder, type, afterSpawn, extraIndex));

        private static IEnumerator SpawnAndPlaceCoroutine<T, Z>(GameObject prefab, Z holder, CollectibleDestructionCallback.eCollectibleType type, dSpawnAndPlace<T> afterSpawn, int extraIndex = 0)
            where T : Collectible
            where Z : CollectibleHolder
        {
            while (GameManager.instance.currentGameState < 1)
                yield return null;

            yield return new WaitForSeconds(0.2f);

            GameObject gameObject = GameObject.Instantiate(prefab, holder.transform.position, holder.transform.rotation);
            gameObject.transform.position = holder.transform.position;
            gameObject.transform.rotation = holder.transform.rotation;

            yield return new WaitForSeconds(0.2f);

            NetworkServer.Spawn(gameObject);

            if (type != CollectibleDestructionCallback.eCollectibleType.NO_CALLBACK)
            {
                _holderSpawnCache[gameObject] = holder;

                CollectibleDestructionCallback callback = gameObject.AddComponent<CollectibleDestructionCallback>();
                callback.collectibleType = type;
                callback.extraIndex = extraIndex;
            }

            yield return new WaitForSeconds(0.2f);

            T obj = gameObject.GetComponent<T>();
            holder.CmdPlaceCollectible(obj.netIdentity, obj.Networklabel);

            yield return new WaitForSeconds(0.2f);

            if (afterSpawn != null)
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
