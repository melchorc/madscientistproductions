using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MadScience.Render
{
    public partial class RenderWindow : UserControl
    {

        public event EventHandler RequireNewTextures;

        public RenderWindow()
        {
            this.ClientSize = new Size(640, 480);
            //this.Text = "Direct3D (DX9/C#) - Indexed Geometry";
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
            InitializeComponent();
        }

        private Color backgroundColour = Color.SlateBlue;
        [System.ComponentModel.Browsable(false)]
        public Color BackgroundColour
        {
            get
            {
                return backgroundColour;
            }
            set
            {
                backgroundColour = value;
            }
        }
        private Color wireframeColour = new Color();
        [System.ComponentModel.Browsable(false)]
        public Color WireframeColour
        {
            get
            {
                return wireframeColour;
            }
            set
            {
                wireframeColour = value;
            }
        }


        private bool renderEnabled;
        [System.ComponentModel.Browsable(false)]
        public bool RenderEnabled
        {
            get
            {
                return renderEnabled;
            }
            set
            {
                renderEnabled = value;
                if (renderEnabled)
                    Invalidate();
            }
        }

        private int mousing = 0;
        public Label statusLabel;
        public Label lblGeneratingTexture;
        //private System.ComponentModel.IContainer components;

        private int fillMode = 1;
        [System.ComponentModel.Browsable(false)]
        public int CurrentFillMode
        {
            get
            {
                return fillMode;
            }
            set
            {
                fillMode = value;
                Invalidate();
            }
        }

        public int shaderMode = 0;

        public void setModel(modelInfo newModel)
        {
        }

        public void setModel(modelInfo newModel, int modelNumber)
        {
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void Init()
        {
        }

        private void CancelResize(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Width < 1 || Height < 1)
                e.Cancel = true;
        }

        public void OnLostDevice(object sender, EventArgs e)
        {
        }

        public void DeInit()
        {
            renderEnabled = false;
            statusLabel.Text = "3d view currently not initialised.";
        }

        private void setupShaders()
        {
        }

        public void resetDevice()
        {
        }

        /// <summary>
        /// This event-handler is a good place to create and initialize any 
        /// Direct3D related objects, which may become invalid during a 
        /// device reset.
        /// </summary>
        public void OnResetDevice(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// This method is dedicated completely to rendering our 3D scene and is
        /// is called by the OnPaint() event-handler.
        /// </summary>
        private void Render()
        {
        }

        public void loadTexture(Stream textureInput, string outputTexture)
        {
        }

        public void loadTextureFromBitmap(Bitmap textureInput, string outputTexture)
        {
        }

        public void loadDefaultTextures()
        {
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
             base.OnKeyDown(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }

        public void ResetView()
        {
            ResetView(180);
        }

        private void ResetView(int spX)
        {
        }

        private void InitializeComponent()
        {
            this.statusLabel = new System.Windows.Forms.Label();
            this.lblGeneratingTexture = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.BackColor = System.Drawing.SystemColors.Menu;
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.statusLabel.Location = new System.Drawing.Point(318, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(153, 13);
            this.statusLabel.TabIndex = 1;
            this.statusLabel.Text = "3d view currently not initialised.";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // lblGeneratingTexture
            // 
            this.lblGeneratingTexture.AutoSize = true;
            this.lblGeneratingTexture.BackColor = System.Drawing.SystemColors.Menu;
            this.lblGeneratingTexture.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblGeneratingTexture.Location = new System.Drawing.Point(0, 233);
            this.lblGeneratingTexture.Name = "lblGeneratingTexture";
            this.lblGeneratingTexture.Size = new System.Drawing.Size(103, 13);
            this.lblGeneratingTexture.TabIndex = 2;
            this.lblGeneratingTexture.Text = "Generating texture...";
            this.lblGeneratingTexture.Visible = false;
            // 
            // RenderWindow
            // 
            this.Controls.Add(this.lblGeneratingTexture);
            this.Controls.Add(this.statusLabel);
            this.Name = "RenderWindow";
            this.Size = new System.Drawing.Size(471, 246);
            this.Load += new System.EventHandler(this.RenderWindow_Load);
            this.VisibleChanged += new System.EventHandler(this.RenderWindow_VisibleChanged);
            this.Resize += new System.EventHandler(this.RenderWindow_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void RenderWindow_Load(object sender, EventArgs e)
        {
        }

        private void frontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetView(180);
        }

        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetView(0);
        }

        private void leftSideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetView(90);
        }

        private void rightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetView(270);
        }

        private void RenderWindow_Resize(object sender, EventArgs e)
        {

        }

        private void RenderWindow_VisibleChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Test2");
        }

    }
}
