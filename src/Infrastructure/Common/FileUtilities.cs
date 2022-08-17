using SixLabors.ImageSharp;


namespace Infrastructure.Common;


public static class FileUtilities
{
    private static readonly IReadOnlyDictionary<string, byte[]> Signatures = new Dictionary<string, byte[]>
    {
        [".jpeg"] = new byte[] { 0xFF, 0xD8, 0xFF },
        [".jpg"] = new byte[] { 0xFF, 0xD8, 0xFF },
        [".png"] = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
    };


    public static bool IsSignatureMatchToExtension(BinaryReader fileStream, string extension)
    {
        var key = Signatures[extension];
        var header = fileStream.ReadBytes(key.Length);
        fileStream.BaseStream.Seek(0, SeekOrigin.Begin);

        return key.SequenceEqual(header);
    }


    public static bool ValidExtension(string extension, params string[] possibleExtensions) =>
        possibleExtensions.Contains(extension);


    public static bool SignatureMatchToExtension(string extension, Stream fileStream)
    {
        var stream = new BinaryReader(fileStream);

        return IsSignatureMatchToExtension(stream, extension);
    }


    public static bool MaximumResolution(Stream fileStream, int width, int height)
    {
        var imageInfo = Image.Identify(fileStream);

        return imageInfo.Width <= width && imageInfo.Height <= height;
    }
}