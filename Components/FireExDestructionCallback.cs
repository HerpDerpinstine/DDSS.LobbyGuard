using DDSS_LobbyGuard.Security;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using Il2CppProps.FireEx;
using System;
using UnityEngine;

namespace DDSS_LobbyGuard.Components
{
    [ClassInjectionAssemblyTarget("DDSS.LobbyGuard")]
    public class FireExDestructionCallback : MonoBehaviour
    {
        internal static void Register()
            => ClassInjector.RegisterTypeInIl2Cpp<FireExDestructionCallback>();
        public FireExDestructionCallback(IntPtr ptr) : base(ptr) { }
        public void OnDestroy()
            => FireExSecurity.SpawnFireEx(gameObject.GetComponent<FireExController>());
    }
}
