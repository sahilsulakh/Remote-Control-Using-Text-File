using System;
using System.Reflection;
using System.Windows.Forms;

namespace YourWorkspaceName               // <<<--- REPLACE WITH YOUR ACTUAL WORKSPACE NAME
{
    public partial class Form1 : Form
    {
     
        private MasterClass? _master;

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            _master = new MasterClass(this);
            _master.Start();
        }
   
     }
}