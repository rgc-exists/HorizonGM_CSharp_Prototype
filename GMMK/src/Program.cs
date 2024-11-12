using UndertaleModLib;
using UndertaleModLib.Models;

namespace GMMK
{
    public static class GMMK_Program
    {
        public static GMMK_Patcher patcher = new GMMK_Patcher();

        static void Main(string[] args)
        {
            var gmslDir = Path.GetDirectoryName(Environment.CurrentDirectory);
            var modDir = Path.Combine(gmslDir!, "mods");
            var baseDir = Path.GetDirectoryName(gmslDir);
            var modDirs = Directory.GetDirectories(modDir);

            var stream = File.OpenRead(Path.Combine(baseDir!, "data.win"));
            var data = UndertaleIO.Read(stream, UMT_LoaderError, _ => { });
            stream.Dispose();

            if(File.Exists(Path.Combine(baseDir!, "cache.win"))) {
                File.Delete(Path.Combine(baseDir!, "cache.win"));
            }

            /*
            stream = File.OpenWrite(Path.Combine(baseDir!, "cache.win"));
            UndertaleIO.Write(stream, data);
            stream.Dispose();

            GMMK_Patcher.StartGame(true, new string[]{ });
            */
            patcher.Load(data, args);
        }

        static void UMT_LoaderError(object message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  [ERROR] {message}");
        }
    }
}