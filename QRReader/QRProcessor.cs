using IronBarCode;

namespace QRReader
{
    public class QRProcessor
    {
        // Allow the use to upload the QR file

        public static string[] Process(byte[] bytes)
        {
            BarcodeResults QR = BarcodeReader.Read(bytes);
            string[] qrs = QR.Values();
            if (qrs.Length == 0)
            {
                return new string[] { "No QR Code found" };
            }
            else
            {
                return qrs;
            }
        }
        
    }
}
