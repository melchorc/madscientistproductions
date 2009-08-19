using System;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using System.IO;

namespace MadScience.Render
{
    public partial class Render : System.Windows.Forms.Form
    {
        public Render()
        {
            logMessageToFile("Initalising components...");
            InitializeComponent();
        }

        private void logMessageToFile(string message)
        {
            System.IO.StreamWriter sw = System.IO.File.AppendText(Path.Combine(Application.StartupPath, "renderWindow.log"));
            try
            {
                string logLine = System.String.Format(
                    "{0:G}: {1}.", System.DateTime.Now, message);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }

        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem renderModeToolStripMenuItem;
        private ToolStripMenuItem solidToolStripMenuItem;
        private ToolStripMenuItem wireframeToolStripMenuItem;
        private ToolStripMenuItem resetViewToolStripMenuItem;

        static void Main()
        {
            using (Render frm = new Render())
            {
                Application.Run(frm);
            }
        }


        private void solidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderWindow1.CurrentFillMode = 1;
            solidToolStripMenuItem.Checked = true;
            wireframeToolStripMenuItem.Checked = false;
            solidWireframeToolStripMenuItem.Checked = false;
        }

        private void wireframeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderWindow1.CurrentFillMode = 0;
            solidToolStripMenuItem.Checked = false;
            wireframeToolStripMenuItem.Checked = true;
            solidWireframeToolStripMenuItem.Checked = false;
        }

        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void resetViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderWindow1.ResetView();
        }

        private void solidWireframeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderWindow1.CurrentFillMode = 2;
            solidToolStripMenuItem.Checked = false;
            wireframeToolStripMenuItem.Checked = false;
            solidWireframeToolStripMenuItem.Checked = true;
        }

        private void Render_Load(object sender, EventArgs e)
        {
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                //MessageBox.Show(Environment.GetCommandLineArgs()[0] + " " + Environment.GetCommandLineArgs()[1]);
                Stream meshStream = File.OpenRead(Environment.GetCommandLineArgs()[1].ToString());
                MadScience.Render.modelInfo newModel = MadScience.Render.Helpers.geomToModel(meshStream);
                meshStream.Close();

                renderWindow1.statusLabel.Text = "Loaded " + Environment.GetCommandLineArgs()[1].ToString();
                renderWindow1.setModel(newModel);
                renderWindow1.resetDevice();
                renderWindow1.RenderEnabled = true;
            }
            else
            {
                logMessageToFile("RenderEnabled: true");
                renderWindow1.RenderEnabled = true;
                logMessageToFile("loadDefaultTextures");
                //renderWindow1.loadDefaultTextures();
                renderWindow1.resetDevice();
            }
        }
    }

}
