using System;
using System.Collections.Generic;
using System.Text;

namespace MadScience.Render
{
    partial class Render
    {
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renderModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solidToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wireframeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solidWireframeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renderWindow1 = new MadScience.Render.RenderWindow();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Menu;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(753, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renderModeToolStripMenuItem,
            this.resetViewToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.toolsToolStripMenuItem.Text = "View";
            this.toolsToolStripMenuItem.Click += new System.EventHandler(this.toolsToolStripMenuItem_Click);
            // 
            // renderModeToolStripMenuItem
            // 
            this.renderModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.solidToolStripMenuItem,
            this.wireframeToolStripMenuItem,
            this.solidWireframeToolStripMenuItem});
            this.renderModeToolStripMenuItem.Name = "renderModeToolStripMenuItem";
            this.renderModeToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.renderModeToolStripMenuItem.Text = "Render Mode";
            // 
            // solidToolStripMenuItem
            // 
            this.solidToolStripMenuItem.Checked = true;
            this.solidToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.solidToolStripMenuItem.Name = "solidToolStripMenuItem";
            this.solidToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.solidToolStripMenuItem.Text = "Solid";
            this.solidToolStripMenuItem.Click += new System.EventHandler(this.solidToolStripMenuItem_Click);
            // 
            // wireframeToolStripMenuItem
            // 
            this.wireframeToolStripMenuItem.Name = "wireframeToolStripMenuItem";
            this.wireframeToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.wireframeToolStripMenuItem.Text = "Wireframe";
            this.wireframeToolStripMenuItem.Click += new System.EventHandler(this.wireframeToolStripMenuItem_Click);
            // 
            // solidWireframeToolStripMenuItem
            // 
            this.solidWireframeToolStripMenuItem.Name = "solidWireframeToolStripMenuItem";
            this.solidWireframeToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.solidWireframeToolStripMenuItem.Text = "Solid+Wireframe";
            this.solidWireframeToolStripMenuItem.Click += new System.EventHandler(this.solidWireframeToolStripMenuItem_Click);
            // 
            // resetViewToolStripMenuItem
            // 
            this.resetViewToolStripMenuItem.Name = "resetViewToolStripMenuItem";
            this.resetViewToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.resetViewToolStripMenuItem.Text = "Reset View";
            this.resetViewToolStripMenuItem.Click += new System.EventHandler(this.resetViewToolStripMenuItem_Click);
            // 
            // renderWindow1
            // 
            //this.renderWindow1.BackgroundColour = System.Drawing.Color.Empty;
            this.renderWindow1.CurrentFillMode = 1;
            this.renderWindow1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderWindow1.Location = new System.Drawing.Point(0, 24);
            this.renderWindow1.Name = "renderWindow1";
            this.renderWindow1.RenderEnabled = false;
            this.renderWindow1.Size = new System.Drawing.Size(753, 533);
            this.renderWindow1.TabIndex = 1;
            this.renderWindow1.WireframeColour = System.Drawing.Color.Empty;
            // 
            // Render
            // 
            this.ClientSize = new System.Drawing.Size(753, 557);
            this.Controls.Add(this.renderWindow1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Render";
            this.Load += new System.EventHandler(this.Render_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private RenderWindow renderWindow1;
        private System.Windows.Forms.ToolStripMenuItem solidWireframeToolStripMenuItem;

    }
}
