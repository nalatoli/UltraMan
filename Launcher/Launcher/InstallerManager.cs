using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
    public static class InstallerManager
    {
        public static async Task RunInstaller(string installerPath, string arguments = null)
        {
            /* Throw error if installer path does not exist */
            if (!File.Exists(installerPath))
                throw new FileNotFoundException("This installer path does NOT exist: '" + installerPath + "'");

            /* Create new process start info for Python script */
            var startInfo = new ProcessStartInfo()
            {
                FileName = installerPath,       // Set file name to installer path
                Arguments = arguments,
                UseShellExecute = true,         // Use OS shell for elevated priveleges
                CreateNoWindow = true,         // Set process to NOT create a console
                WorkingDirectory = Path.GetDirectoryName(installerPath),
                WindowStyle = ProcessWindowStyle.Hidden
            };

            if (!string.IsNullOrEmpty(arguments))
                startInfo.Arguments = arguments;

            /* On a new thread */
            await Task.Run(() =>
            {
                try
                {
                    /* Create and record new disposable process with start info */
                    using (var process = new Process() { StartInfo = startInfo })
                    {
                        /* Start process and wait for exit */
                        process.Start();
                        process.WaitForExit();
                    }
                }

                catch (Win32Exception)
                {
                    /* Rethrow Win32Exception (user denied install) as OperationCancelledException */
                    throw new OperationCanceledException("The installer operation has been cancelled.");
                }
            });
        }
    }
}
