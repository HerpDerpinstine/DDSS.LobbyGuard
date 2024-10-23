using Il2Cpp;
using System.Collections.Generic;

namespace DDSS_LobbyGuard.Security
{
    internal static class StereoSecurity
    {
        private static Dictionary<StereoController, bool> _stereoStates = new();

        internal static void OnSceneLoad()
            => _stereoStates.Clear();
        
        internal static bool GetState(StereoController stereo)
        {
            if (!_stereoStates.TryGetValue(stereo, out bool state))
                return false;
            return state;
        }

        internal static void ApplyState(StereoController stereo, bool state)
            => _stereoStates[stereo] = state;
    }
}
