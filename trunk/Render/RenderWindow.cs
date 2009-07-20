using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace MadScience.Render
{

    public class DX9Form : System.Windows.Forms.Form
    {
        private Device d3dDevice = null;

        private int  mousing = 0;
        private Point ptLastMousePosit;
        private Point ptCurrentMousePosit;

        private int spinX;
        private int spinY;
        private float spinZ;
        private float height;

        private MatrixStack matrixStack;

        MadScience.Render.modelInfo model = new modelInfo();

        private VertexBuffer vertexBuffer = null;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem renderModeToolStripMenuItem;
        private ToolStripMenuItem solidToolStripMenuItem;
        private ToolStripMenuItem wireframeToolStripMenuItem;
        private PictureBox pictureBox1;
        private ToolStripMenuItem resetViewToolStripMenuItem;
        private IndexBuffer indexBuffer = null;


        public DX9Form()
        {
            this.ClientSize = new Size(640, 480);
            this.Text = "Direct3D (DX9/C#) - Indexed Geometry";
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
            InitializeComponent();
        }

        public void loadModel(modelInfo newModel)
        {
            this.model = newModel;
            if (d3dDevice == null) Init();
            OnResetDevice(d3dDevice, null);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            this.Render();
            this.Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        static void Main()
        {
            using (DX9Form frm = new DX9Form())
            {
                frm.Init();
                Application.Run(frm);

            }
        }

        private Material material;
        private void SetupLights()
        {

            material = new Material();

            material.Diffuse = Color.White;
            material.Specular = Color.White;
            material.SpecularSharpness = 15.0F;
            material.Ambient = Color.White;

            d3dDevice.Material = material;

            //d3dDevice.RenderState.Ambient = System.Drawing.Color.White;

        }

        /// <summary>
        /// This method basically creates and initialize the Direct3D device and
        /// anything else that doens't need to be recreated after a device 
        /// reset.
        /// </summary>
        private void Init()
        {

            //InitializeComponent();

            this.Invalidate();

            //
            // Do we support hardware vertex processing? If so, use it. 
            // If not, downgrade to software.
            //

            Caps caps = Manager.GetDeviceCaps(Manager.Adapters.Default.Adapter,
                                               DeviceType.Hardware);
            CreateFlags flags;

            if (caps.DeviceCaps.SupportsHardwareTransformAndLight)
                flags = CreateFlags.HardwareVertexProcessing;
            else
                flags = CreateFlags.SoftwareVertexProcessing;

            Console.WriteLine(caps.MaxTextureBlendStages.ToString());

            //
            // Everything checks out - create a simple, windowed device.
            //

            PresentParameters d3dpp = new PresentParameters();

            d3dpp.BackBufferFormat = Format.Unknown;
            d3dpp.SwapEffect = SwapEffect.Discard;
            d3dpp.Windowed = true;
            d3dpp.EnableAutoDepthStencil = true;
            d3dpp.AutoDepthStencilFormat = DepthFormat.D16;
            d3dpp.PresentationInterval = PresentInterval.Default;

            d3dDevice = new Device(0, DeviceType.Hardware, pictureBox1, flags, d3dpp);

            // Register an event-handler for DeviceReset and call it to continue
            // our setup.
            d3dDevice.DeviceReset += new System.EventHandler(this.OnResetDevice);
            OnResetDevice(d3dDevice, null);
        }

        /// <summary>
        /// This event-handler is a good place to create and initialize any 
        /// Direct3D related objects, which may become invalid during a 
        /// device reset.
        /// </summary>
        public void OnResetDevice(object sender, EventArgs e)
        {
            Device device = (Device)sender;

            if (device == null) return;

            this.Invalidate();

            if (wireframeToolStripMenuItem.Checked)
            {
                device.RenderState.FillMode = FillMode.WireFrame;
            }
            else
            {
                device.RenderState.FillMode = FillMode.Solid;
            }

            device.Transform.Projection =
                Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f),
                (float)this.pictureBox1.Width / this.pictureBox1.Height,
                0.1f, 100.0f);

            device.RenderState.ZBufferEnable = true;
            device.RenderState.Lighting = false;
            device.RenderState.CullMode = Cull.None;

            //Enable alpha blending in the device

            //device.RenderState.SourceBlend = Blend.SourceAlpha;
            //device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
            //device.RenderState.AlphaBlendEnable = true;

            //
            // Create a vertex buffer...
            //

            if (model.numVertices > 0)
            {

                height = model.bounds.mid.Y / 1.5f;
                spinX = 180;

                Console.WriteLine(model.bounds.mid.X.ToString() + " : " + model.bounds.mid.Y.ToString() + " : " + model.bounds.mid.Z.ToString());
                Console.WriteLine(spinX.ToString() + " : " + spinY.ToString() + " : " + spinZ.ToString() + " : " + height.ToString());
                Console.WriteLine("num vertices: " + model.numVertices.ToString());


                vertexBuffer = new VertexBuffer(typeof(MadScience.Render.vertex),
                                                 (int)model.numVertices, device,
                                                 Usage.Dynamic | Usage.WriteOnly,
                                                 MadScience.Render.vertex.FVF_Flags,
                                                 Pool.Default);

                GraphicsStream gStream = vertexBuffer.Lock(0, 0, LockFlags.None);

                // Now, copy the vertex data into the vertex buffer
                gStream.Write(model.vertexData.ToArray());

                vertexBuffer.Unlock();

                //
                // Create an index buffer to use with our indexed vertex buffer...
                //

                indexBuffer = new IndexBuffer(typeof(int), (int)(model.faceData.Count * 2), device,
                                               Usage.WriteOnly, Pool.Default);

                gStream = indexBuffer.Lock(0, 0, LockFlags.None);

                // Now, copy the indices data into the index buffer
                gStream.Write(model.faceData.ToArray());

                indexBuffer.Unlock();

                SetupLights();

                matrixStack = new MatrixStack();
            }

            this.Invalidate();
            this.Refresh();


        }

        private double deltaTime;

        /// <summary>
        /// This method is dedicated completely to rendering our 3D scene and is
        /// is called by the OnPaint() event-handler.
        /// </summary>
        private void Render()
        {

            if (d3dDevice == null) return;

            deltaTime = HiResTimer.GetElapsedTime();

            // Display the frame rate in the title bar of the form
            this.Text = string.Format("Mesh Previewer ({0} fps)", FrameRate.CalculateFrameRate());
            HiResTimer.Start();

            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.SteelBlue, 1.0f, 0);

            d3dDevice.BeginScene();

            d3dDevice.RenderState.Ambient = System.Drawing.Color.White;



            if (model.numVertices > 0 && indexBuffer != null && vertexBuffer != null)
            {

                d3dDevice.RenderState.AlphaBlendEnable = true;
                d3dDevice.RenderState.AlphaSourceBlend = Blend.SourceAlpha;
                d3dDevice.RenderState.AlphaDestinationBlend = Blend.InvSourceAlpha;
                d3dDevice.VertexFormat = MadScience.Render.vertex.FVF_Flags;

                d3dDevice.SetTexture(0, model.textures.baseTexture);
                d3dDevice.SetTexture(1, model.textures.curStencil);

                d3dDevice.SetSamplerState(0, SamplerStageStates.MinFilter, 1);
                d3dDevice.SetSamplerState(0, SamplerStageStates.MagFilter, 1);

                d3dDevice.SetTextureStageState(0, TextureStageStates.TextureCoordinateIndex, 0);
                d3dDevice.SetTextureStageState(0, TextureStageStates.ColorOperation, (int)TextureOperation.BlendTextureAlpha);
                //d3dDevice.SetTextureStageState(0, TextureStageStates.ColorArgument1, (int)TextureArgument.TextureColor);
                //d3dDevice.SetTextureStageState(0, TextureStageStates.ColorArgument2, (int)TextureArgument.TextureColor );

                //d3dDevice.SetTextureStageState(0, TextureStageStates.AlphaOperation, (int)TextureOperation.BlendTextureAlpha );
                //d3dDevice.SetTextureStageState(0, TextureStageStates.AlphaArgument1, (int)TextureArgument.TextureColor );
                


                //d3dDevice.SetTextureStageState(0, TextureStageStates.TextureCoordinateIndex, 0);

                //d3dDevice.SetTextureStageState(0, TextureStageStates.ColorArgument2, 0);

                d3dDevice.SetSamplerState(1, SamplerStageStates.MinFilter, 1);
                d3dDevice.SetSamplerState(1, SamplerStageStates.MagFilter, 1);
                d3dDevice.SetTextureStageState(1, TextureStageStates.TextureCoordinateIndex, 1);
                d3dDevice.SetTextureStageState(1, TextureStageStates.ColorOperation, (int)TextureOperation.BlendTextureAlpha);
                //d3dDevice.SetTextureStageState(1, TextureStageStates.AlphaOperation, (int)TextureOperation.MultiplyAdd);
                //d3dDevice.SetTextureStageState(1, TextureStageStates.ColorArgument1, (int)TextureArgument.TextureColor );
                //d3dDevice.SetTextureStageState(1, TextureStageStates.ColorArgument2, (int)TextureArgument.Diffuse );

                //d3dDevice.Material = material;

                d3dDevice.SetStreamSource(0, vertexBuffer, 0);
                d3dDevice.Indices = indexBuffer;

                d3dDevice.Transform.World =
                    Matrix.RotationYawPitchRoll(Geometry.DegreeToRadian(spinX),
                    Geometry.DegreeToRadian(0f), 0.0f) *
                    Matrix.Translation(0f, -height, spinZ + 2f);

                d3dDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, (int)model.numVertices, 0, (int)model.numPolygons);
            }
            d3dDevice.EndScene();

            d3dDevice.Present();

            HiResTimer.Reset();

            //Application.DoEvents();
        }

        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renderModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solidToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wireframeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.resetViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            this.wireframeToolStripMenuItem});
            this.renderModeToolStripMenuItem.Name = "renderModeToolStripMenuItem";
            this.renderModeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.renderModeToolStripMenuItem.Text = "Render Mode";
            // 
            // solidToolStripMenuItem
            // 
            this.solidToolStripMenuItem.Checked = true;
            this.solidToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.solidToolStripMenuItem.Name = "solidToolStripMenuItem";
            this.solidToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.solidToolStripMenuItem.Text = "Solid";
            this.solidToolStripMenuItem.Click += new System.EventHandler(this.solidToolStripMenuItem_Click);
            // 
            // wireframeToolStripMenuItem
            // 
            this.wireframeToolStripMenuItem.Name = "wireframeToolStripMenuItem";
            this.wireframeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.wireframeToolStripMenuItem.Text = "Wireframe";
            this.wireframeToolStripMenuItem.Click += new System.EventHandler(this.wireframeToolStripMenuItem_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 24);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(753, 533);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // resetViewToolStripMenuItem
            // 
            this.resetViewToolStripMenuItem.Name = "resetViewToolStripMenuItem";
            this.resetViewToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.resetViewToolStripMenuItem.Text = "Reset View";
            this.resetViewToolStripMenuItem.Click += new System.EventHandler(this.resetViewToolStripMenuItem_Click);
            // 
            // DX9Form
            // 
            this.ClientSize = new System.Drawing.Size(753, 557);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DX9Form";
            this.Load += new System.EventHandler(this.DX9Form_Load);
            this.Shown += new System.EventHandler(this.DX9Form_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void loadModel()
        {
            //Stream input = File.OpenRead(@"C:\Programming\Projects\Sims 3\MadScience\Render\bin\Debug\#00000000F9818729.simgeom");
            Stream input = File.OpenRead(@"P:\Stuart\Desktop\afTopShirtTee_crew_lod1_0x0000000031AFB84E.simgeom");

            this.model = MadScience.Render.Helpers.geomToModel(input);
            this.model.name = "Meep";

            input.Close();

            //Stream baseTexture = File.OpenRead(@"P:\Stuart\Desktop\flatWhite_0x90de6db90e35feb7.dds");
            Stream baseTexture = File.OpenRead(@"P:\Stuart\Desktop\afTopShirtTee_crew__0x94e9f715e099bcbd.dds");
            model.textures.baseTexture = TextureLoader.FromStream(d3dDevice, baseTexture);
            baseTexture.Close();

            Stream stencilTexture = File.OpenRead(@"P:\Stuart\Desktop\afTopShirtTee_crew__0xb4ce3fdcc42907f6.dds");
            model.textures.curStencil = TextureLoader.FromStream(d3dDevice, stencilTexture);
            stencilTexture.Close();

            //Stream baseTexture = File.OpenRead(@"P:\Stuart\Desktop\afTopBlouseOffShoul_0xb5683b98ca67ecfe.dds");
            //this.model.textures.curStencil = Texture.FromStream(d3dDevice, baseTexture, Usage.None, Pool.Managed);
            //baseTexture.Close();
            //d3dDevice.SetSamplerState(0, SamplerStageStates.MinFilter, true);

            OnResetDevice(d3dDevice, null);
        }

        private void DX9Form_Load(object sender, EventArgs e)
        {

        }

        private void DX9Form_Shown(object sender, EventArgs e)
        {
            loadModel();
            Refresh();
        }

        private void solidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            d3dDevice.RenderState.FillMode = FillMode.Solid;
            solidToolStripMenuItem.Checked = true;
            wireframeToolStripMenuItem.Checked = false;
        }

        private void wireframeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            d3dDevice.RenderState.FillMode = FillMode.WireFrame;
            solidToolStripMenuItem.Checked = false;
            wireframeToolStripMenuItem.Checked = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            ptCurrentMousePosit = PointToScreen(new Point(e.X, e.Y));

            if (mousing > 0)
            {
                spinX -= (ptCurrentMousePosit.X - ptLastMousePosit.X);
                if (mousing == 1)
                {
                    spinY -= (ptCurrentMousePosit.Y - ptLastMousePosit.Y);
                }
                if (mousing == 2)
                {
                    spinZ += ((Single)(ptCurrentMousePosit.Y - ptLastMousePosit.Y) / 5);
                }
                if (mousing == 3)
                {
                    height -= ((Single)(ptCurrentMousePosit.Y - ptLastMousePosit.Y) / 10);
                }

                if (spinX < 0) spinX = 359;
                if (spinY < 0) spinY = 359;
                if (spinX > 359) spinX = 0;
                if (spinY > 359) spinY = 0;

                Console.WriteLine(spinX.ToString() + " : " + spinY.ToString() + " : " + spinZ.ToString() + " : " + height.ToString() );
            }

            ptLastMousePosit = ptCurrentMousePosit;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            ptLastMousePosit = ptCurrentMousePosit = PointToScreen(new Point(e.X, e.Y));
            if (e.Button == MouseButtons.Middle)
            {
                mousing = 3;
            }
            if (e.Button == MouseButtons.Left)
            {
                mousing = 1;
            }
            if (e.Button == MouseButtons.Right)
            {
                mousing = 2;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mousing = 0;
        }

        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void resetViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            height = model.bounds.mid.Y / 2;
            spinX = 0;
            spinY = 0;
            spinZ = 0;
        }
    }

}
