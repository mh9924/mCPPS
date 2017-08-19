using System;
using System.Threading;
using System.Windows.Forms;

namespace mattsCPPS
{
    public partial class mApp : MetroFramework.Forms.MetroForm
    {
        public static bool started = false;

        public mApp()
        {
            InitializeComponent();
        }

        private void metroTile1_Click(object sender, EventArgs e)
        {
            if (!started)
            {
                mApp.started = true;
                metroTile1.Enabled = false;
                mattsCPPS.StartServer(this);
                metroTile1.Text = "stop server";
                metroTile1.Visible = true;
                metroTile1.Enabled = true;
                return;
            }
            mApp.started = false;
            metroTile1.Enabled = false;
            mattsCPPS.StopServer();
            metroTile1.Text = "start server";
            metroTile1.Enabled = true;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.SetOut(new TextBoxConsole(cpps_output));
        }

        private void metroLabel1_Click(object sender, EventArgs e)
        {

        }

        private void cpps_output_TextChanged(object sender, EventArgs e)
        {
            cpps_output.SelectionStart = cpps_output.Text.Length;
            cpps_output.ScrollToCaret();
        }

        public void serverStopEvent()
        {
            mApp.started = false;
            metroTile1.Text = "start server";
        }
    }
}
