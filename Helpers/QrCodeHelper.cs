using QRCoder;
using System;

namespace VideojuegosStore.Helpers
{
    public static class QrCodeHelper
    {
        public static string GenerateQrCodeBase64(string text)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            var qrBytes = qrCode.GetGraphic(10);
            return Convert.ToBase64String(qrBytes);
        }
    }
}