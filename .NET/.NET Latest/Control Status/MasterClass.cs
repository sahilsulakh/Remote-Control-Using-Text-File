using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

// Give the WinForms timer an alias so “Timer” is no longer ambiguous
using WinFormsTimer = System.Windows.Forms.Timer;

namespace YourWorspaceName               // <<<--- REPLACE WITH YOUR ACTUAL WORKSPACE NAME
{

    public sealed class MasterClass : IDisposable
    {
        private const string ControlFileUrl =
            "https://keymaster-agni.vercel.app/api/vault/ellRHrNSMrcAvtnKsvHVqyOvgbT2/control.txt";     // <<<--- GET IT FROM KEYMASTER VAULT PAGE

        private const int PollIntervalMs = 2_000;   // 2 s
        private static readonly HttpClient Http = new();   // Shared, static

        private readonly Form _mainForm;
        private readonly WinFormsTimer _pollTimer;
        private string? _lastStatus;                
        private bool _maintenanceMessageShown;

        public MasterClass(Form form)
        {
            _mainForm = form ?? throw new ArgumentNullException(nameof(form));

            _pollTimer = new WinFormsTimer { Interval = PollIntervalMs };
            _pollTimer.Tick += async (_, __) => await CheckControlFileAsync();
        }

        public void Start() => _pollTimer.Start();
        public void Stop() => _pollTimer.Stop();

        public void Dispose()
        {
            _pollTimer.Dispose();
            Http.Dispose();
            GC.SuppressFinalize(this);
        }

        // ──────────────────────────────────────────────────────────
        //  INTERNAL LOGIC
        // ──────────────────────────────────────────────────────────
        private async Task CheckControlFileAsync()
        {
            try
            {
                string raw = await Http.GetStringAsync(ControlFileUrl);
                string status = raw.Trim().ToUpperInvariant();

                if (!string.Equals(status, _lastStatus, StringComparison.Ordinal))
                {
                    _lastStatus = status;
                    ApplyStatus(status);
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(
                    "masterclass_log.txt",
                    $"{DateTime.Now:u}  {ex.GetType().Name}: {ex.Message}{Environment.NewLine}");
            }
        }

        private void ApplyStatus(string status)
        {
            // Always marshal back to the UI thread
            _mainForm.BeginInvoke((MethodInvoker)(() =>
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
            }));
        }

        private void SetControlsEnabled(bool enabled) => ToggleRecursive(_mainForm, enabled);

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
