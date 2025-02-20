using DDSS_LobbyGuard.Config;
using System;

namespace DDSS_LobbyGuard.Modules
{
    public abstract class ILobbyModule
    {
        public abstract string Name { get; }
        public virtual int Priority { get; }
        public virtual Type ConfigType { get; }
        public HarmonyLib.Harmony HarmonyInstance { get; internal set; }
        public ConfigCategory Config { get; internal set; }

        public virtual bool OnLoad() => true;
        public virtual void OnQuit() { }
        public virtual void OnSceneInit(int buildIndex, string sceneName) { }
    }
}
