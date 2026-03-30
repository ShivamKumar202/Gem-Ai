namespace Gem.BLL.Common.Utility
{
    public static class GeminiExtensionMethods
    {
        public static string GetFileExtension(string mimeType)
        {
            return mimeType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "audio/wav" => ".wav",
                "audio/mpeg" => ".mp3",
                _ => ".bin"
            };
        }

        public static void SaveBinaryFile(string fileName, byte[] data)
        {
            File.WriteAllBytes(fileName, data);
            Console.WriteLine($"File saved to: {fileName}");
        }
    }
}
