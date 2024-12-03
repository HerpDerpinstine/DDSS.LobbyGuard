using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps;
using Il2CppProps.FireEx;
using Il2CppProps.Keys;
using Il2CppProps.Misc.PaperTray;
using Il2CppProps.PaperShredder;
using Il2CppProps.Printer;
using Il2CppProps.Scripts;
using Il2CppProps.Shelf;
using Il2CppProps.StickyNote;
using Il2CppProps.TrashBin;
using Il2CppProps.WC.Toilet;
using Il2CppProps.WorkStation.Mouse;
using Il2CppProps.WorkStation.Scripts;
using Il2CppSystem;
using System.Collections.Generic;

namespace DDSS_LobbyGuard.Security
{
    internal static class CollectibleHolderSecurity
    {
        private static Type _trashBinType = Il2CppType.Of<TrashBin>();

        internal static bool CanPlace(NetworkIdentity player, CollectibleHolder holder, Collectible collectible)
        {
            // Validate Drop
            if (!InteractionSecurity.CanDropUsable(player, false))
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
