using Guna.UI2.WinForms;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YourWorkspaceName               // <<<--- REPLACE WITH YOUR ACTUAL WORKSPACE NAME
{
    public class UpdateHandler
    {
        private readonly Form1 mainForm;
        private static readonly HttpClient httpClient;
        private readonly string updateUrl = "https://keymaster-agni.vercel.app/api/vault/ellRHrNSMrcAvtnKsvHVqyOvgbT2/update.txt";     // <<<--- GET IT FROM KEYMASTER VAULT PAGE

        public bool IsUpdating { get; private set; }

        static UpdateHandler()
        {
            var handler = new HttpClientHandler();
            
            // Set SSL protocols compatible with .NET Framework 4.7.2
            try
            {
                handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
            }
            catch
            {
                // Fallback for older systems that might not support TLS 1.3
                handler.SslProtocols = SslProtocols.Tls12;
            }
            
            handler.ServerCertificateCustomValidationCallback = delegate { return true; };

            httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromSeconds(45);
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TestApplication/1.0");
        }

        public UpdateHandler(Form1 form)
        {
            mainForm = form;
        }

        public async Task InitializeWithUpdateCheckAsync()
        {
            try
            {
                await CheckForUpdatesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Initial check failed: {0}", ex.Message));
            }
        }

        public async Task CheckForUpdatesAsync()
        {
            try
            {
                // Use the VersionManager to get the correct current version
                string currentVersionStr = VersionManager.GetCurrentVersion().Trim();
                var currentVersion = new VersionManager.Version(currentVersionStr);

                string updateInfo = await httpClient.GetStringAsync(updateUrl);
                string[] lines = updateInfo.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length < 3)
                {
                    UpdateStatus("Invalid update info format");
                    return;
                }

                string downloadUrl = lines[0].Trim();
                string latestVersionStr = lines[1].Trim();
                string updateType = lines[2].Trim().ToLower();

                try
                {
                    var latestVersion = new VersionManager.Version(latestVersionStr);

                    // Update UI with version info - use Invoke with Action for .NET Framework compatibility
                    mainForm.Invoke(new Action(() => mainForm.UpdateVersionInfo(latestVersionStr)));

                    bool updateNeeded = currentVersion < latestVersion;

                    if (!updateNeeded)
                    {
                        UpdateStatus("Your application is up to date");
                        return;
                    }

                    mainForm.Invoke(new Action(() =>
                    {
                        if (updateType == "major")
                        {
                            mainForm.PromptMajorUpdate(downloadUrl, latestVersionStr);
                        }
                        else
                        {
                            mainForm.BeginPatchUpdate(downloadUrl);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    UpdateStatus(string.Format("Version error: {0}", ex.Message));
                }
            }
            catch (Exception ex)
            {
                UpdateStatus(string.Format("Update check failed: {0}", ex.Message));
            }
        }

        public async Task DownloadAndUpdateAsync(
            string url,
            Guna2ProgressBar progressBar,
            Label percentageLabel)
        {
            if (IsUpdating) return;
            IsUpdating = true;

            try
            {
                UpdateStatus("Downloading update...");
                string tempFile = Path.Combine(Path.GetTempPath(), string.Format("{0}.exe", Guid.NewGuid()));

                await DownloadFileWithProgressAsync(url, tempFile, progressBar, percentageLabel);
                if (!File.Exists(tempFile)) 
                    throw new FileNotFoundException("Downloaded file not found");

                UpdateStatus("Preparing installer...");
                CreateUpdateScript(Application.ExecutablePath, tempFile);

                UpdateStatus("Restarting application...");
                await Task.Delay(500);
                Application.Exit();
            }
            catch (Exception ex)
            {
                UpdateStatus(string.Format("Error: {0}", ex.Message));
                IsUpdating = false;
            }
        }

        private static async Task DownloadFileWithProgressAsync(
            string url,
            string outputPath,
            Guna2ProgressBar progressBar,
            Label percentageLabel)
        {
            using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(outputPath, FileMode.Create))
                {
                    var buffer = new byte[8192];
                    long totalRead = 0;
                    long totalBytes = response.Content.Headers.ContentLength.HasValue ? 
                        response.Content.Headers.ContentLength.Value : 0;
                    int bytesRead;

                    while (true)
                    {
                        bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead <= 0) break;

                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        totalRead += bytesRead;

                        if (totalBytes <= 0) continue;

                        int progress = (int)(totalRead * 100 / totalBytes);
                        
                        // Use MethodInvoker for .NET Framework compatibility
                        progressBar.Invoke(new MethodInvoker(() =>
                        {
                            progressBar.Value = progress;
                            percentageLabel.Text = string.Format("{0}%", progress);
                        }));
                    }
                }
            }
        }

        private static void CreateUpdateScript(string currentExe, string updateFile)
        {
            string batchPath = Path.Combine(Path.GetTempPath(), "update.bat");
            string script = string.Format(@"
@echo off
chcp 65001 >nul
:loop
tasklist /fi ""IMAGENAME eq {0}"" | find /i ""{0}"" >nul
if %errorlevel%==0 (
    timeout /t 1 /nobreak >nul
    goto loop
)
del ""{1}.bak"" 2>nul
move ""{1}"" ""{1}.bak"" 2>nul
move ""{2}"" ""{1}""
start """" ""{1}""
del ""{3}""
exit", 
                Path.GetFileName(currentExe),
                currentExe,
                updateFile,
                batchPath);

            File.WriteAllText(batchPath, script);

            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = string.Format("/c start \"\" \"{0}\"", batchPath),
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };

            Process.Start(startInfo);
        }

        private void UpdateStatus(string message)
        {
            if (!mainForm.IsDisposed)
            {
                mainForm.Invoke(new Action(() => mainForm.lblCFU.Text = message));
            }
        }
    }
}