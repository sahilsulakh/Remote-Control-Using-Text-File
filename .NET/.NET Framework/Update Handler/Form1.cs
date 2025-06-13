using System;
using System.Reflection;
using System.Windows.Forms;

namespace YourWorkspaceName               // <<<--- REPLACE WITH YOUR ACTUAL NAMESPACE NAME
{
    public partial class Form1 : Form
    {
        private UpdateHandler _updateHandler;

        public Form1()
        {
            InitializeComponent();
            btnUpdateNow.Click += BtnUpdateNow_Click;
            btnUpdateCancel.Click += BtnUpdateCancel_Click;

            // Initialize version display with the correct version
            lblCurrentVersion.Text = string.Format("Current Version: {0}", VersionManager.GetCurrentVersion());
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            pnlUpdater.Visible = false;
            _updateHandler = new UpdateHandler(this);
            await _updateHandler.InitializeWithUpdateCheckAsync();
            timerMasterCheck.Start();
        }

        private async void TimerMasterCheck_Tick(object sender, EventArgs e)
        {
            if (_updateHandler == null) return;
            await _updateHandler.CheckForUpdatesAsync();
        }

        // Public helpers
        public void UpdateVersionInfo(string latestVersion)
        {
            lblLatestVersion.Text = string.Format("Latest Version: {0}", latestVersion);
            lblCurrentVersion.Text = string.Format("Current Version: {0}", VersionManager.GetCurrentVersion());
        }

        public void BeginPatchUpdate(string url)
        {
            if (_updateHandler == null) return;

            btnUpdateNow.Visible = false;
            pnlUpdater.Visible = true;
            pnlUpdater.BringToFront();
            lblCFU.Text = "Auto‑Updating...";
            
            // Fire and forget pattern for .NET Framework compatibility
            var task = _updateHandler.DownloadAndUpdateAsync(url, prgUpdate, lblPercentage);
        }

        public void PromptMajorUpdate(string url, string latestVersion)
        {
            var result = MessageBox.Show(
                string.Format("A major update (v{0}) is available.\nCurrent version: {1}\n\nUpdate now?", 
                    latestVersion, VersionManager.GetCurrentVersion()),
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
            var url = btnUpdateNow.Tag as string;
            if (!string.IsNullOrEmpty(url))
            {
                BeginPatchUpdate(url);
            }
        }

        private void BtnUpdateCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Cancel update?", "Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                pnlUpdater.Visible = false;
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

    // Version Manager class compatible with .NET Framework 4.7.2
    public static class VersionManager
    {
        // Option 1: Hardcoded version (easiest to change)
        private const string HARDCODED_VERSION = "1.0.0.0";

        // Option 2: Use this if you want to get version from assembly
        public static string GetCurrentVersion()
        {
            // You can choose which method to use by uncommenting the desired line:

            // Method 1: Use hardcoded version (recommended for easy updates)
            return HARDCODED_VERSION;

            // Method 2: Get from Assembly version
            //return GetAssemblyVersion();
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

        // Custom Version class for .NET Framework 4.7.2 compatibility
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

            public static bool operator <(Version v1, Version v2)
            {
                if (ReferenceEquals(v1, null)) return !ReferenceEquals(v2, null);
                return v1.CompareTo(v2) < 0;
            }

            public static bool operator >(Version v1, Version v2)
            {
                if (ReferenceEquals(v1, null)) return false;
                return v1.CompareTo(v2) > 0;
            }

            public static bool operator <=(Version v1, Version v2)
            {
                if (ReferenceEquals(v1, null)) return true;
                return v1.CompareTo(v2) <= 0;
            }

            public static bool operator >=(Version v1, Version v2)
            {
                if (ReferenceEquals(v1, null)) return ReferenceEquals(v2, null);
                return v1.CompareTo(v2) >= 0;
            }

            public override string ToString()
            {
                return string.Format("{0}.{1}.{2}.{3}", Major, Minor, Build, Revision);
            }

            public override bool Equals(object obj)
            {
                var other = obj as Version;
                if (other == null) return false;
                return CompareTo(other) == 0;
            }

            public override int GetHashCode()
            {
                return Major.GetHashCode() ^ Minor.GetHashCode() ^ Build.GetHashCode() ^ Revision.GetHashCode();
            }
        }
    }
}