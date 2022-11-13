using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary> Http client for downloading installers. </summary>
        private static readonly HttpClient httpClient = new();

        /// <summary> Backed status text. </summary>
        private string statusText = "";

        /// <summary> Status text. </summary>
        public string StatusTest
        {
            get => statusText;
            private set
            {
                statusText = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChangedMembers

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


        public MainWindow()
        {
            InitializeComponent();
        }
        
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadGame();
            }

            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + "Shutting down launcher.");
            }

            App.Current.Shutdown();
        }

        private async Task LoadGame()
        {
            WriteLog("\nStarting launcher...");
            StatusTest = "Starting launch";

            /* Get the latest release from GitHub */
            WriteLog("Fetching latest release from GitHub...");
            StatusTest = "Looking for updates...";
            var latestRelease = await GitHub.GetLatestRelease();

            /* Throw error if latest release is null */
            if (latestRelease == null)
            {
                WriteLog("...Couldn't fetch the latest release from GitHub");
                throw new ApplicationException("Unable to fetch latest release from server.");
            }

            /* Get the config of the latest release */
            WriteLog("Getting game config from latest release...");
            var latestConfig = GetGameConfigFromRelease(latestRelease);

            /* Throw error if latest release is corrupted */
            if (latestConfig == null || latestConfig.Version == null)
            {
                WriteLog("...Latest release configuration is corrupted");
                throw new ApplicationException("The latest release from server is corrupt.");
            }

            /* Get exe installer for game */
            var installerAsset = latestRelease.Assets.FirstOrDefault((a) => a.ContentType == "application/x-msdownload");

            /* Throw error if installer is not found */
            if (installerAsset == null)
            {
                WriteLog("...Latest release has no installer attached");
                throw new ApplicationException("The latest release from server is corrupt.");
            }

            /* Get the current conifg */
            WriteLog("Getting current installation config...");
            var currentConfig = GetCurrentGameConfig();

            /* Throw error if current installation is corrupted */
            if (currentConfig != null && currentConfig.Version == null)
            {
                WriteLog("...Config file at '" + Paths.UltraManConfigFile + "' exists, but it is corrupted");
                throw new ApplicationException("The current installation is corrupt.");
            }

            /* If an update is required */
            if (currentConfig == null || latestConfig.Version != currentConfig.Version)
            {
                StatusTest = "Updating game";
                WriteLog("Game update required to v" + latestConfig.Version + "...");

                WriteLog("Downloading installer v" + latestConfig.Version + "...");
                var fileBytes = await httpClient.GetByteArrayAsync(installerAsset.BrowserDownloadUrl);
                await File.WriteAllBytesAsync(Paths.UltraManTempDownloadFile, fileBytes);

                WriteLog("Running installer v" + latestConfig.Version + "...");
                await InstallerManager.RunInstaller(Paths.UltraManTempDownloadFile, "/S");

                WriteLog("Marking v" + latestConfig.Version + " as installed...");
                SetCurrentGameConfig(latestConfig);

            }

            StatusTest = "Starting game";
            WriteLog("Running game v" + latestConfig.Version + "...");
            Process.Start(new ProcessStartInfo() { FileName = Paths.UltraManExeFile });
        }

        /// <summary> Writes log. </summary>
        /// <param name="message"> Log to write. </param>
        private void WriteLog(string message)
        {
            File.AppendAllText(Paths.UltraManLogFile, message + '\n');
        }

        private GameConfig? GetCurrentGameConfig()
        {
            if (!File.Exists(Paths.UltraManConfigFile))
                return null;

            return JsonConvert.DeserializeObject<GameConfig>(File.ReadAllText(Paths.UltraManConfigFile));
        }

        private void SetCurrentGameConfig(GameConfig gameVersion)
        {
            if (File.Exists(Paths.UltraManConfigFile))
                File.Delete(Paths.UltraManConfigFile);

            File.WriteAllText(Paths.UltraManConfigFile, JsonConvert.SerializeObject(gameVersion, Formatting.Indented));
        }

        private GameConfig? GetGameConfigFromRelease(Release release)
        {
            if (release == null || !Version.TryParse(release.TagName, out Version? version))
                return null;

            return new GameConfig()
            {
                Version = version
            };
        }
    }
}
