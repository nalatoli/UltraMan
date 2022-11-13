using System;
using System.IO;

namespace Launcher
{
    internal static class Paths
    {
        public static string UltraManRootFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UltraMan");

        public static string UltraManLauncherFolder => Path.Combine(UltraManRootFolder, "Launcher");

        public static string UltraManGameFolder => Path.Combine(UltraManRootFolder, "Game");

        public static string UltraManExeFile => Path.Combine(UltraManGameFolder, "MegaManTrueNet.exe");

        public static string UltraManConfigFile => Path.Combine(UltraManLauncherFolder, "gameconfig.json");

        public static string UltraManLogFile => Path.Combine(UltraManRootFolder, "launcherlog.txt");

        public static string UltraManTempDownloadFile => Path.Combine(UltraManLauncherFolder, "gameinstaller.exe");
    }
}
