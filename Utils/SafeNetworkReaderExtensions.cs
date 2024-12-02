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
        internal static sbyte SafeReadSByte(this NetworkReader reader)
            => TryRunRead(reader.ReadSByte);
        internal static char SafeReadChar(this NetworkReader reader)
            => TryRunRead(reader.ReadChar);
        internal static bool SafeReadBool(this NetworkReader reader)
            => TryRunRead(reader.ReadBool);
        internal static short SafeReadShort(this NetworkReader reader)
            => TryRunRead(reader.ReadShort);
        internal static ushort SafeReadUShort(this NetworkReader reader)
            => TryRunRead(reader.ReadUShort);
        internal static int SafeReadInt(this NetworkReader reader)
            => TryRunRead(reader.ReadInt);
        internal static uint SafeReadUInt(this NetworkReader reader)
            => TryRunRead(reader.ReadUInt);
        internal static long SafeReadLong(this NetworkReader reader)
            => TryRunRead(reader.ReadLong);
        internal static ulong SafeReadULong(this NetworkReader reader)
            => TryRunRead(reader.ReadULong);
        internal static float SafeReadFloat(this NetworkReader reader)
            => TryRunRead(reader.ReadFloat);
        internal static double SafeReadDouble(this NetworkReader reader)
            => TryRunRead(reader.ReadDouble);
        internal static Decimal SafeReadDecimal(this NetworkReader reader)
            => TryRunRead(reader.ReadDecimal);
        internal static string SafeReadString(this NetworkReader reader)
            => TryRunRead(reader.ReadString);
        internal static Il2CppStructArray<byte> SafeReadBytes(this NetworkReader reader, int count)
            => TryRunRead(() => reader.ReadBytes(count));
        internal static Il2CppStructArray<byte> SafeReadBytes(this NetworkReader reader, Il2CppStructArray<byte> arr, int count)
            => TryRunRead(() => reader.ReadBytes(arr, count));
        internal static Il2CppStructArray<byte> SafeReadBytesAndSize(this NetworkReader reader)
            => TryRunRead(reader.ReadBytesAndSize);
        internal static ArraySegment<byte> SafeReadBytesSegment(this NetworkReader reader, int count)
            => TryRunRead(() => reader.ReadBytesSegment(count));
        internal static ArraySegment<byte> SafeReadArraySegmentAndSize(this NetworkReader reader)
            => TryRunRead(reader.ReadArraySegmentAndSize);
        internal static ArraySegment<byte> SafeReadBytesSegmentAndSize(this NetworkReader reader)
            => TryRunRead(reader.ReadArraySegmentAndSize);
        internal static Vector2 SafeReadVector2(this NetworkReader reader)
            => TryRunRead(reader.ReadVector2);
        internal static Vector2Int SafeReadVector2Int(this NetworkReader reader)
            => TryRunRead(reader.ReadVector2Int);
        internal static Vector3 SafeReadVector3(this NetworkReader reader)
            => TryRunRead(reader.ReadVector3);
        internal static Vector3Int SafeReadVector3Int(this NetworkReader reader)
            => TryRunRead(reader.ReadVector3Int);
        internal static Vector4 SafeReadVector4(this NetworkReader reader)
            => TryRunRead(reader.ReadVector4);
        internal static Color SafeReadColor(this NetworkReader reader)
            => TryRunRead(reader.ReadColor);
        internal static Color32 SafeReadColor32(this NetworkReader reader)
            => TryRunRead(reader.ReadColor32);
        internal static NetworkIdentity SafeReadNetworkIdentity(this NetworkReader reader)
            => TryRunRead(reader.ReadNetworkIdentity);

        internal static Nullable<byte> SafeReadByteNullable(this NetworkReader reader)
            => TryRunRead(reader.ReadByteNullable);
        internal static Nullable<sbyte> SafeReadSByteNullable(this NetworkReader reader)
            => TryRunRead(reader.ReadSByteNullable);
        internal static Nullable<char> SafeReadCharNullable(this NetworkReader reader)
            => TryRunRead(reader.ReadCharNullable);
        internal static Nullable<bool> SafeReadBoolNullable(this NetworkReader reader)
            => TryRunRead(reader.ReadBoolNullable);
        internal static Nullable<short> SafeReadShortNullable(this NetworkReader reader)
            => TryRunRead(reader.ReadShortNullable);
        internal static Nullable<ushort> SafeReadUShortNullable(this NetworkReader reader)
            => TryRunRead(reader.ReadUShortNullable);
        internal static Nullable<int> SafeReadIntNullable(this NetworkReader reader)
            => TryRunRead(reader.ReadIntNullable);
        internal static Nullable<uint> SafeReadUIntNullable(this NetworkReader reader)
            => TryRunRead(reader.ReadUIntNullable);
        internal static Nullable<long> SafeReadLongNullable(this NetworkReader reader)
            => TryRunRead(reader.ReadLongNullable);
        internal static Nullable<ulong> SafeReadULongNullable(this NetworkReader reader)
            => TryRunRead(reader.ReadULongNullable);
        internal static Nullable<float> SafeReadFloatNullable(this NetworkReader reader)
            => TryRunRead(reader.ReadFloatNullable);
        internal static Nullable<double> SafeReadDoubleNullable(this NetworkReader reader)
            => TryRunRead(reader.ReadDoubleNullable);
        internal static Nullable<Decimal> SafeReadDecimalNullable(this NetworkReader reader)
            => TryRunRead(reader.ReadDecimalNullable);
        internal static Nullable<Vector2> SafeReadVector2Nullable(this NetworkReader reader)
            => TryRunRead(reader.ReadVector2Nullable);
        internal static Nullable<Vector2Int> SafeReadVector2IntNullable(this NetworkReader reader)
            => TryRunRead(reader.ReadVector2IntNullable);
        internal static Nullable<Vector3> SafeReadVector3Nullable(this NetworkReader reader)
            => TryRunRead(reader.ReadVector3Nullable);
        internal static Nullable<Vector3Int> SafeReadVector3IntNullable(this NetworkReader reader)
            => TryRunRead(reader.ReadVector3IntNullable);
        internal static Nullable<Vector4> SafeReadVector4Nullable(this NetworkReader reader)
            => TryRunRead(reader.ReadVector4Nullable);
        internal static Nullable<Color> SafeReadColorNullable(this NetworkReader reader)
            => TryRunRead(reader.ReadColorNullable);
        internal static Nullable<Color32> SafeReadColor32Nullable(this NetworkReader reader)
            => TryRunRead(reader.ReadColor32Nullable);
    }
}
