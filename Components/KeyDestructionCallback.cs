using DDSS_LobbyGuard.Security;
using Il2Cpp;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using System;
using UnityEngine;

namespace DDSS_LobbyGuard.Components
{
    [ClassInjectionAssemblyTarget("DDSS.LobbyGuard")]
    public class KeyDestructionCallback : MonoBehaviour
    {
        internal static void Register()
            => ClassInjector.RegisterTypeInIl2Cpp<KeyDestructionCallback>();
        public KeyDestructionCallback(IntPtr ptr) : base(ptr) { }
        public void OnDestroy()
            => KeySecurity.SpawnKey(gameObject.GetComponent<KeyController>());
    }
}
