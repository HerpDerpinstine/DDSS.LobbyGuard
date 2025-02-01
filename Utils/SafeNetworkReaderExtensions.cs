using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMirror;
using Il2CppSystem;
using UnityEngine;

namespace DDSS_LobbyGuard.Utils
{
    internal static class SafeNetworkReaderExtensions
    {
        private delegate T dTryRunRead<T>();
        private static T TryRunRead<T>(dTryRunRead<T> method)
        {
            try
            {
                return method();
            }
            catch
            {
                return default;
            }
        }

        internal static T SafeRead<T>(this NetworkReader reader)
            => TryRunRead(reader.Read<T>);

        internal static byte SafeReadByte(this NetworkReader reader)
            => TryRunRead(reader.ReadByte);
        internal static bool SafeReadBool(this NetworkReader reader)
            => TryRunRead(reader.ReadBool);
        internal static int SafeReadInt(this NetworkReader reader)
            => reader.SafeReadByte() / 2;
        internal static float SafeReadFloat(this NetworkReader reader)
            => TryRunRead(reader.ReadFloat);
        internal static string SafeReadString(this NetworkReader reader)
            => TryRunRead(reader.ReadString);
        internal static Il2CppStructArray<byte> SafeReadBytesAndSize(this NetworkReader reader)
            => TryRunRead(reader.ReadBytesAndSize);
        internal static Vector3 SafeReadVector3(this NetworkReader reader)
            => TryRunRead(reader.ReadVector3);
        internal static Quaternion SafeReadQuaternion(this NetworkReader reader)
            => TryRunRead(reader.ReadQuaternion);
        internal static NetworkIdentity SafeReadNetworkIdentity(this NetworkReader reader)
            => TryRunRead(reader.ReadNetworkIdentity);
        internal static T SafeReadNetworkBehaviour<T>(this NetworkReader reader)
            where T : NetworkBehaviour
            => TryRunRead(reader.ReadNetworkBehaviour<T>);
        internal static GameObject SafeReadGameObject(this NetworkReader reader)
            => TryRunRead(reader.ReadGameObject);
    }
}
