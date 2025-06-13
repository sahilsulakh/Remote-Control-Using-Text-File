// Copy of Form1.cs from .NET Latest. Update for .NET Framework compatibility as needed.

using System;
using System.Reflection;
using System.Windows.Forms;

namespace YourWorkspaceName               // <<<--- REPLACE WITH YOUR WORKSPACE NAME
{
    public partial class Form1 : Form
    {
        private UpdateHandler _updateHandler;
        private MasterClass _master;

        public Form1()
        {
            InitializeComponent();
            btnUpdateNow.Click += BtnUpdateNow_Click;
            btnUpdateCancel.Click += BtnUpdateCancel_Click;
            lblCurrentVersion.Text = $"Current Version: {VersionManager.GetCurrentVersion()}";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pnlUpdater.Visible = false;
            _updateHandler = new UpdateHandler(this);
            _updateHandler.InitializeWithUpdateCheck();
            _pollTimer.Start();
            _master = new MasterClass(this);
            _master.Start();
        }

        private void TimerMasterCheck_Tick(object sender, EventArgs e)
        {
            if (_updateHandler == null) return;
            _updateHandler.CheckForUpdates();
        }

        // Public helpers
        public void UpdateVersionInfo(string latestVersion)
        {
            lblLatestVersion.Text = $"Latest Version: {latestVersion}";
            lblCurrentVersion.Text = $"Current Version: {VersionManager.GetCurrentVersion()}";
        }

        public void BeginPatchUpdate(string url)
        {
            if (_updateHandler == null) return;
            btnUpdateNow.Visible = false;
            pnlUpdater.Visible = true;
            pnlUpdater.BringToFront();
            lblCFU.Text = "AutoUpdating...";
            _updateHandler.DownloadAndUpdate(url, prgUpdate, lblPercentage);
        }

        public void PromptMajorUpdate(string url, string latestVersion)
        {
            var result = MessageBox.Show(
                $"A major update (v{latestVersion}) is available.\nCurrent version: {VersionManager.GetCurrentVersion()}\n\nUpdate now?",
                "Major Update Available",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                BeginPatchUpdate(url);
            }
            else
            {
                lblCFU.Text = "Update postponed by user.";
            }
        }

        // UI Event handlers
        private void BtnUpdateNow_Click(object sender, EventArgs e)
        {
            if (btnUpdateNow.Tag is string)
            {
                BeginPatchUpdate(btnUpdateNow.Tag as string);
            }
        }

        private void BtnUpdateCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to cancel the update?", "Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                pnlUpdater.Visible = false;
                Application.Exit();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_updateHandler != null && _updateHandler.IsUpdating)
            {
                MessageBox.Show("Please wait until update completes", "Update in Progress",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }
    }

    // New Version Manager class to handle version properly
    public static class VersionManager
    {
        private const string HARDCODED_VERSION = "1.0.0.0";
        public static string GetCurrentVersion()
        {
            return HARDCODED_VERSION;
        }
        private static string GetAssemblyVersion()
        {
            try
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return version != null ? version.ToString() : "1.0.0.0";
            }
            catch
            {
                return "1.0.0.0";
            }
        }
        public class Version : IComparable<Version>
        {
            public int Major { get; private set; }
            public int Minor { get; private set; }
            public int Build { get; private set; }
            public int Revision { get; private set; }
            public Version(string version)
            {
                if (string.IsNullOrWhiteSpace(version))
                    throw new ArgumentException("Version string cannot be empty");
                var parts = version.Split('.');
                Major = parts.Length > 0 ? ParsePart(parts[0]) : 0;
                Minor = parts.Length > 1 ? ParsePart(parts[1]) : 0;
                Build = parts.Length > 2 ? ParsePart(parts[2]) : 0;
                Revision = parts.Length > 3 ? ParsePart(parts[3]) : 0;
            }
            private static int ParsePart(string part)
            {
                int result;
                if (int.TryParse(part, out result))
                {
                    return result;
                }
                return 0;
            }
            public int CompareTo(Version other)
            {
                if (other == null) return 1;
                if (Major != other.Major) return Major.CompareTo(other.Major);
                if (Minor != other.Minor) return Minor.CompareTo(other.Minor);
                if (Build != other.Build) return Build.CompareTo(other.Build);
                return Revision.CompareTo(other.Revision);
            }
            public static bool operator <(Version v1, Version v2) { return v1.CompareTo(v2) < 0; }
            public static bool operator >(Version v1, Version v2) { return v1.CompareTo(v2) > 0; }
            public static bool operator <=(Version v1, Version v2) { return v1.CompareTo(v2) <= 0; }
            public static bool operator >=(Version v1, Version v2) { return v1.CompareTo(v2) >= 0; }
            public override string ToString() { return string.Format("{0}.{1}.{2}.{3}", Major, Minor, Build, Revision); }
        }
    }
}
