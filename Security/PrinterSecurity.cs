using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppProps.Printer;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class PrinterSecurity
    {
        private const int MAX_WIDTH = 87;
        private const int MAX_HEIGHT = 60;

        internal static bool VerifyImage(Printer printer, Il2CppStructArray<byte> bytes)
        {
            // Parse Texture
            Texture2D _tempTexture = new(MAX_WIDTH, MAX_HEIGHT);
            Il2CppImageConversionManager.LoadImage(_tempTexture, bytes);

            // Validate
            bool returnVal = VerifyImage(printer, _tempTexture);
            GameObject.Destroy(_tempTexture);
            return returnVal;
        }

        internal static bool VerifyImage(Printer printer, PrintedImage image)
            => VerifyImage(printer, image.renderer.material.mainTexture);
        internal static bool VerifyImage(Printer printer, Texture texture)
            => VerifyImage(printer, texture.TryCast<Texture2D>());
        internal static bool VerifyImage(Printer printer, Texture2D texture)
        {
            // Validate
            if ((texture.width > MAX_WIDTH)
                || (texture.height > MAX_HEIGHT)
                || !printer.VerifyImageColors(texture))
            {
                // Image is Not Allowed
                return false;
            }

            // Image is Allowed
            return true;
        }
    }
}
