using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YourWorkspaceName               // <<<--- REPLACE WITH YOUR ACTUAL WORKSPACE NAME
{
    public partial class Form1 : Form
    {

        private MasterClass _master;

        public Form1()
        {
            InitializeComponent();
            _master = new MasterClass(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _master.Start();
        }
    }
}

