// Copy of MasterClass.cs from .NET Latest. Update for .NET Framework compatibility as needed.

using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using WinFormsTimer = System.Windows.Forms.Timer;

namespace YourWorkspaceName               // <<<--- REPLACE WITH YOUR WORKSPACE NAME
{
    /// <summary>
    /// Polls a remote control.txt file and toggles the UI (ACTIVE / PAUSED / STOPPED).
    /// </summary>
    public sealed class MasterClass : IDisposable
    {
        private const string ControlFileUrl =
            "https://keymaster-agni.vercel.app/api/vault/ellRHrNSMrcAvtnKsvHVqyOvgbT2/control.txt";     // <<<--- GET IT FROM KEYMASTER VAULT PAGE

        private const int PollIntervalMs = 2000;   // 2s
        private static readonly WebClient Web = new WebClient();   // Use WebClient for .NET Framework

        private readonly Form _mainForm;
        private readonly WinFormsTimer _pollTimer;
        private string _lastStatus;
        private bool _maintenanceMessageShown;

        public MasterClass(Form form)
        {
            _mainForm = form ?? throw new ArgumentNullException("form");
            _pollTimer = new WinFormsTimer { Interval = PollIntervalMs };
            _pollTimer.Tick += (s, e) => CheckControlFile();
        }

        public void Start() { _pollTimer.Start(); }
        public void Stop() { _pollTimer.Stop(); }

        public void Dispose()
        {
            _pollTimer.Dispose();
            Web.Dispose();
            GC.SuppressFinalize(this);
        }

        // INTERNAL LOGIC
        private void CheckControlFile()
        {
            try
            {
                string raw = Web.DownloadString(ControlFileUrl);
                string status = raw.Trim().ToUpperInvariant();
                if (status != _lastStatus)
                {
                    _lastStatus = status;
                    ApplyStatus(status);
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(
                    "masterclass_log.txt",
                    string.Format("{0:u}  {1}: {2}{3}", DateTime.Now, ex.GetType().Name, ex.Message, Environment.NewLine));
            }
        }

        private void ApplyStatus(string status)
        {
            // Always marshal back to the UI thread
            _mainForm.BeginInvoke((MethodInvoker)delegate
            {
                switch (status)
                {
                    case "STOPPED":
                        MessageBox.Show(
                            "This application is currently out of service. Please try again later.",
                            "Application Stopped",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Stop);
                        Application.Exit();
                        break;
                    case "PAUSED":
                        SetControlsEnabled(false);
                        if (!_maintenanceMessageShown)
                        {
                            MessageBox.Show(
                                "The application is currently under maintenance. Please try again later.",
                                "Maintenance Mode",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            _maintenanceMessageShown = true;
                        }
                        break;
                    case "ACTIVE":
                        SetControlsEnabled(true);
                        _maintenanceMessageShown = false;
                        break;
                    default:
                        // Unknown value – ignore.
                        break;
                }
            });
        }

        private void SetControlsEnabled(bool enabled) { ToggleRecursive(_mainForm, enabled); }
        private static void ToggleRecursive(Control parent, bool enabled)
        {
            foreach (Control ctrl in parent.Controls)
            {
                ctrl.Enabled = enabled;
                if (ctrl.HasChildren)
                    ToggleRecursive(ctrl, enabled);
            }
        }
    }
}
