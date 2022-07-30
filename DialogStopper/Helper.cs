using System.Reflection;

namespace DialogStopper
{
    public static class Helper
    {
        public static byte[] GetResource(Assembly assembly, string name)
        {
            using var stream = assembly.GetManifestResourceStream(name);
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }
    }
}