// Copy of UpdateHandler.cs from .NET Latest. Update for .NET Framework compatibility as needed.

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YourWorkspaceName               // <<<--- REPLACE WITH YOUR WORKSPACE NAME
{
    public class UpdateHandler
    {
        private readonly Form1 mainForm;
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly string updateUrl = "https://keymaster-agni.vercel.app/api/vault/ellRHrNSMrcAvtnKsvHVqyOvgbT2/update.txt";     // <<<--- GET IT FROM KEYMASTER VAULT PAGE

        public bool IsUpdating { get; private set; }

        public UpdateHandler(Form1 form) { mainForm = form; }

        public void InitializeWithUpdateCheck()
        {
            try
            {
                Task.Run(() => CheckForUpdatesAsync());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Initial check failed: " + ex.Message);
            }
        }

        public async Task CheckForUpdatesAsync()
        {
            try
            {
                string currentVersionStr = VersionManager.GetCurrentVersion().Trim();
                VersionManager.Version currentVersion = new VersionManager.Version(currentVersionStr);
                string updateInfo = await httpClient.GetStringAsync(updateUrl);
                string[] lines = updateInfo.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
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
                    VersionManager.Version latestVersion = new VersionManager.Version(latestVersionStr);
                    mainForm.Invoke((MethodInvoker)delegate { mainForm.UpdateVersionInfo(latestVersionStr); });
                    bool updateNeeded = currentVersion < latestVersion;
                    if (!updateNeeded)
                    {
                        UpdateStatus("Your application is up to date");
                        return;
                    }
                    mainForm.Invoke((MethodInvoker)delegate
                    {
                        if (updateType == "major")
                        {
                            mainForm.PromptMajorUpdate(downloadUrl, latestVersionStr);
                        }
                        else
                        {
                            mainForm.BeginPatchUpdate(downloadUrl);
                        }
                    });
                }
                catch (Exception ex)
                {
                    UpdateStatus("Version error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus("Update check failed: " + ex.Message);
            }
        }

        public void DownloadAndUpdate(string url, ProgressBar progressBar, Label percentageLabel)
        {
            if (IsUpdating) return;
            IsUpdating = true;
            Task.Run(async () =>
            {
                try
                {
                    UpdateStatus("Downloading update...");
                    string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".exe");
                    await DownloadFileWithProgressAsync(url, tempFile, progressBar, percentageLabel);
                    if (!File.Exists(tempFile))
                    {
                        UpdateStatus("Downloaded file not found");
                        IsUpdating = false;
                        return;
                    }
                    UpdateStatus("Preparing installer...");
                    CreateUpdateScript(Application.ExecutablePath, tempFile);
                    UpdateStatus("Restarting application...");
                    await Task.Delay(500);
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    UpdateStatus("Error: " + ex.Message);
                    IsUpdating = false;
                }
            });
        }

        private static async Task DownloadFileWithProgressAsync(string url, string outputPath, ProgressBar progressBar, Label percentageLabel)
        {
            using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(outputPath, FileMode.Create))
                {
                    var buffer = new byte[8192];
                    long totalRead = 0;
                    long totalBytes = response.Content.Headers.ContentLength ?? 0;
                    int bytesRead;
                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        totalRead += bytesRead;
                        if (totalBytes > 0)
                        {
                            int progress = (int)(totalRead * 100 / totalBytes);
                            progressBar.Invoke((MethodInvoker)delegate
                            {
                                progressBar.Value = progress;
                                percentageLabel.Text = progress + "%";
                            });
                        }
                    }
                }
            }
        }

        private static void CreateUpdateScript(string currentExe, string updateFile)
        {
            string batchPath = Path.Combine(Path.GetTempPath(), "update.bat");
            string script = string.Format("\r\n@echo off\r\nchcp 65001 >nul\r\n:loop\r\ntasklist /fi \"IMAGENAME eq {0}\" | find /i \"{0}\" >nul\r\nif %errorlevel%==0 (\r\n    timeout /t 1 /nobreak >nul\r\n    goto loop\r\n)\r\ndel \"{0}.bak\" 2>nul\r\nmove \"{0}\" \"{0}.bak\" 2>nul\r\nmove \"{1}\" \"{0}\"\r\nstart \"\" \"{0}\"\r\ndel \"{2}\"\r\nexit", Path.GetFileName(currentExe), updateFile, batchPath);
            File.WriteAllText(batchPath, script);
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c start \"\" \"" + batchPath + "\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            });
        }

        private void UpdateStatus(string message)
        {
            if (!mainForm.IsDisposed)
            {
                mainForm.Invoke((MethodInvoker)delegate { mainForm.lblCFU.Text = message; });
            }
        }
    }
}
