using DDSS_LobbyGuard.Components;
using DDSS_LobbyGuard.Utils;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.Tasks;
using Il2CppProps;
using Il2CppProps.CameraProp;
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
using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.SecurityExtension
{
    internal static class CollectibleSecurity
    {
        private static Type _trashBinType = Il2CppType.Of<TrashBin>();

        internal static Dictionary<GameObject, CollectibleHolder> _holderSpawnCache = new();

        internal static GameObject _cameraPrefab;
        internal static Vector3 _cameraSpawnPos;
        internal static Quaternion _cameraSpawnRot;

        internal static void OnSceneLoad()
        {
            _cameraPrefab = null;
            _cameraSpawnPos = Vector3.zero;
            _cameraSpawnRot = Quaternion.identity;
            _holderSpawnCache.Clear();
        }

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

            // Handle TrashBin Specifically
            if (holderType == _trashBinType)
                return collectible.isTrashable;

            // Validate Collectible
            if (allowedCollectibleTypes.ContainsKey(collectibleType))
                return true;

            // Return False
            return false;
        }

        internal static void SpawnMug(GameObject prefab, MugHolder holder)
            => SpawnAndPlace<Mug, MugHolder>(prefab,
                holder,
                CollectibleSecurityHandler.eCollectibleType.NO_CALLBACK,
                null);
        
        internal static void SpawnMouse(GameObject prefab, MouseHolder holder)
            => SpawnAndPlace<Mouse, MouseHolder>(prefab,
                holder,
                CollectibleSecurityHandler.eCollectibleType.NO_CALLBACK,
                null);

        internal static void SpawnKey(KeyHolder holder)
            => SpawnAndPlace<KeyController, KeyHolder>(holder.keyPrefab,
                holder,
                CollectibleSecurityHandler.eCollectibleType.KEYS,
                null);

        internal static void SpawnFireEx(FireExHolder holder)
            => SpawnAndPlace<FireExController, FireExHolder>(holder.fireExControllerPrefab,
                holder,
                CollectibleSecurityHandler.eCollectibleType.FIRE_EX,
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
                CollectibleSecurityHandler.eCollectibleType.STORAGE_BOX,
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
                CollectibleSecurityHandler.eCollectibleType.BINDER,
                (Binder binder) =>
                {
                    if (BinderManager.instance.binders == null)
                        BinderManager.instance.binders = new();
                    if (!BinderManager.instance.binders.ContainsKey(shelf.shelfCategory))
                        BinderManager.instance.binders[shelf.shelfCategory] = new();
                    BinderManager.instance.binders[shelf.shelfCategory].Add(binder);

                    string catName = System.Enum.GetName(shelf.shelfCategory);
                    string name = $"{catName} {index}";
                    string localizedName = $"{LocalizationManager.instance.GetLocalizedValue(catName)} {index}";

                    binder.Networklabel = localizedName;
                    binder.NetworkinteractableName = name;
                    binder.Networkcolor = binder.colors[UnityEngine.Random.Range(0, binder.colors.Count)];

                    int randomIndex = UnityEngine.Random.Range(0, Task.documents.Count);
                    string item = Task.documents[randomIndex].Item1;
                    string text = Resources.Load<TextAsset>("files/" + item).text;
                    binder.ServerAddDocument(item, text);
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
                CollectibleSecurityHandler.eCollectibleType.CD,
                (CDCase cd) => cd.NetworksongIndex = index,
                index);

        internal static void SpawnCamera(MonoBehaviour coroutineParent)
            => SpawnAndPlace(coroutineParent,
                _cameraPrefab,
                _cameraSpawnPos,
                _cameraSpawnRot,
                CollectibleSecurityHandler.eCollectibleType.CAMERA,
                (CameraPropController camera) => camera.gameObject.SetActive(true));

        private delegate void dSpawnAndPlace<T>(T obj) 
            where T : Collectible;
        private static void SpawnAndPlace<T, Z>(GameObject prefab, Z holder, CollectibleSecurityHandler.eCollectibleType type, dSpawnAndPlace<T> afterSpawn, int extraIndex = 0)
            where T : Collectible
            where Z : CollectibleHolder
            => holder.StartCoroutine(SpawnAndPlaceCoroutine(prefab, holder, type, afterSpawn, extraIndex));

        private static void SpawnAndPlace<T>(MonoBehaviour coroutineParent, GameObject prefab, Vector3 position, Quaternion rotation, CollectibleSecurityHandler.eCollectibleType type, dSpawnAndPlace<T> afterSpawn, int extraIndex = 0)
            where T : Collectible
            => coroutineParent.StartCoroutine(SpawnAndPlaceCoroutine(prefab, position, rotation, type, afterSpawn, extraIndex));

        private static IEnumerator SpawnAndPlaceCoroutine<T, Z>(GameObject prefab, Z holder, CollectibleSecurityHandler.eCollectibleType type, dSpawnAndPlace<T> afterSpawn, int extraIndex = 0)
            where T : Collectible
            where Z : CollectibleHolder
        {
            while (GameManager.instance.currentGameState <= (int)GameStates.WaitingForPlayerConnections)
                yield return null;
            
            bool flag = false;
            while (!flag)
            {
                flag = true;
                foreach (NetworkIdentity networkIdentity in LobbyManager.instance.GetAllPlayers())
                    if (networkIdentity == null || networkIdentity.GetComponent<LobbyPlayer>().NetworkplayerController == null)
                    {
                        flag = false;
                        break;
                    }
                if (!flag)
                    yield return null;
            }

            yield return new WaitForSeconds(1f);

            GameObject gameObject = GameObject.Instantiate(prefab, holder.transform.position, holder.transform.rotation);
            gameObject.transform.position = holder.transform.position;
            gameObject.transform.rotation = holder.transform.rotation;

            yield return new WaitForSeconds(1f);

            NetworkServer.Spawn(gameObject);

            if (type != CollectibleSecurityHandler.eCollectibleType.NO_CALLBACK)
            {
                _holderSpawnCache[gameObject] = holder;

                CollectibleSecurityHandler callback = gameObject.AddComponent<CollectibleSecurityHandler>();
                callback.collectibleType = type;
                callback.extraIndex = extraIndex;
            }

            yield return new WaitForSeconds(3f);

            gameObject.transform.position = holder.transform.position;
            gameObject.transform.rotation = holder.transform.rotation;

            T obj = gameObject.GetComponent<T>();
            holder.ServerPlaceCollectible(obj.netIdentity, obj.Networklabel);

            yield return new WaitForSeconds(1f);

            if (afterSpawn != null)
                afterSpawn(obj);

            yield break;
        }

        private static IEnumerator SpawnAndPlaceCoroutine<T>(GameObject prefab, Vector3 position, Quaternion rotation, CollectibleSecurityHandler.eCollectibleType type, dSpawnAndPlace<T> afterSpawn, int extraIndex = 0)
            where T : Collectible
        {
            while (GameManager.instance.currentGameState <= (int)GameStates.WaitingForPlayerConnections)
                yield return null;

            bool flag = false;
            while (!flag)
            {
                flag = true;
                foreach (NetworkIdentity networkIdentity in LobbyManager.instance.GetAllPlayers())
                    if (networkIdentity == null || networkIdentity.GetComponent<LobbyPlayer>().NetworkplayerController == null)
                    {
                        flag = false;
                        break;
                    }
                if (!flag)
                    yield return null;
            }

            yield return new WaitForSeconds(1f);

            GameObject gameObject = GameObject.Instantiate(prefab, position, rotation);
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;

            yield return new WaitForSeconds(1f);

            NetworkServer.Spawn(gameObject);

            if (type != CollectibleSecurityHandler.eCollectibleType.NO_CALLBACK)
            {
                CollectibleSecurityHandler callback = gameObject.AddComponent<CollectibleSecurityHandler>();
                callback.collectibleType = type;
                callback.extraIndex = extraIndex;
            }

            yield return new WaitForSeconds(3f);

            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;

            yield return new WaitForSeconds(1f);

            if (afterSpawn != null)
            {
                T obj = gameObject.GetComponent<T>();
                afterSpawn(obj);
            }

            yield break;
        }

        private static Dictionary<Type, Dictionary<Type, bool>> _whitelist 
            = new Dictionary<Type, Dictionary<Type, bool>>()
            {
                {
                    _trashBinType,
                    null
                },

                {
                    Il2CppType.Of<CDHolder>(),
                    new Dictionary<Type, bool>()
                    {
                        { Il2CppType.Of<CDCase>(), true },
                    }
                },

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
