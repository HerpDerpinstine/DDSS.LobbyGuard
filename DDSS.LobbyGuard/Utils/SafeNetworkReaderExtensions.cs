using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMirror;
using UnityEngine;

namespace DDSS_LobbyGuard.Utils
{
    public static class SafeNetworkReaderExtensions
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

        public static T SafeRead<T>(this NetworkReader reader)
            => TryRunRead(reader.Read<T>);

        public static byte SafeReadByte(this NetworkReader reader)
            => TryRunRead(reader.ReadByte);
        public static bool SafeReadBool(this NetworkReader reader)
            => TryRunRead(reader.ReadBool);
        public static int SafeReadInt(this NetworkReader reader)
            => reader.SafeReadByte() / 2;
        public static float SafeReadFloat(this NetworkReader reader)
            => TryRunRead(reader.ReadFloat);
        public static string SafeReadString(this NetworkReader reader)
            => TryRunRead(reader.ReadString);
        public static Il2CppStructArray<byte> SafeReadBytesAndSize(this NetworkReader reader)
            => TryRunRead(reader.ReadBytesAndSize);
        public static Vector3 SafeReadVector3(this NetworkReader reader)
            => TryRunRead(reader.ReadVector3);
        public static Quaternion SafeReadQuaternion(this NetworkReader reader)
            => TryRunRead(reader.ReadQuaternion);
        public static NetworkIdentity SafeReadNetworkIdentity(this NetworkReader reader)
            => TryRunRead(reader.ReadNetworkIdentity);
        public static T SafeReadNetworkBehaviour<T>(this NetworkReader reader)
            where T : NetworkBehaviour
            => TryRunRead(reader.ReadNetworkBehaviour<T>);
        public static GameObject SafeReadGameObject(this NetworkReader reader)
            => TryRunRead(reader.ReadGameObject);
    }
}
