using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace MadScience.Render
{
    public partial class RenderWindow : UserControl
    {
        public RenderWindow()
        {
            this.ClientSize = new Size(640, 480);
            //this.Text = "Direct3D (DX9/C#) - Indexed Geometry";
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
            logMessageToFile("Initialising components");
            InitializeComponent();
        }

        private Color backgroundColour = new Color();
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

        private Device d3dDevice = null;

        private int mousing = 0;
        private Point ptLastMousePosit;
        private Point ptCurrentMousePosit;
        private bool bMouseDragged = false;

        private int spinX = 180;
        private int spinY;
        private float transZ;
        private float height;

        MadScience.Render.modelInfo model = new modelInfo();

        private VertexDeclaration vertexDeclaration = null;
        private VertexBuffer vertexBuffer = null;
        private IndexBuffer indexBuffer = null;

        private Texture skinTexture;
        private Texture skinSpecular;
        private Texture normalMapTexture;
        private ContextMenuStrip contextMenuStrip1;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem renderModeToolStripMenuItem;
        private ToolStripMenuItem wireframeToolStripMenuItem;
        private ToolStripMenuItem solidToolStripMenuItem;
        private ToolStripMenuItem solidWireframeToolStripMenuItem;
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
                if (d3dDevice != null)
                {
                    if (fillMode == 0) { d3dDevice.RenderState.FillMode = FillMode.WireFrame; }
                    if (fillMode == 1) { d3dDevice.RenderState.FillMode = FillMode.Solid; }
                    if (fillMode == 2) { d3dDevice.RenderState.FillMode = FillMode.Solid; }
                }
                Invalidate();
            }
        }
        private FillMode getFillMode()
        {
            if (fillMode == 0) return FillMode.WireFrame;
            if (fillMode == 1) return FillMode.Solid;
            if (fillMode == 2) return FillMode.Solid;
            return FillMode.Solid;
        }

        private Effect shader;
        private Effect wireframe;

        private void logMessageToFile(string message)
        {
            System.IO.StreamWriter sw = System.IO.File.AppendText(Application.StartupPath + "\\renderWindow.log");
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

        public void setModel(modelInfo newModel)
        {
            logMessageToFile("Set model");
            this.model = newModel;
            if (d3dDevice == null) Init();

            height = model.bounds.mid.Y / 1.35f;

            Console.WriteLine("mid Y: " + model.bounds.mid.Y.ToString());
            Console.WriteLine("camera height: " + height.ToString());

            //else
            //    OnResetDevice(d3dDevice, null);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            // Force render enabled to be set explicitly so it doesn't try to render into the designer window
            if (renderEnabled)
            {
                this.Render();
                if (mousing > 0)
                    this.Invalidate();
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(this.BackColor), e.ClipRectangle);
            }
        }

        protected override void Dispose(bool disposing)
        {
            logMessageToFile("Disposing components");

            base.Dispose(disposing);

            // Clean up directx resources

            if (vertexDeclaration != null)
                vertexDeclaration.Dispose();
            vertexDeclaration = null;
            if (vertexBuffer != null)
                vertexBuffer.Dispose();
            vertexBuffer = null;
            if (indexBuffer != null)
                indexBuffer.Dispose();
            indexBuffer = null;
            if (shader != null)
                shader.Dispose();
            shader = null;
            if (skinTexture != null)
                skinTexture.Dispose();
            skinTexture = null;
            if (skinSpecular != null)
                skinSpecular.Dispose();
            skinSpecular = null;
            if (normalMapTexture != null)
                normalMapTexture.Dispose();
            normalMapTexture = null;
            if (model != null)
            {
                if (model.textures.ambientTexture != null)
                    model.textures.ambientTexture.Dispose();
                model.textures.ambientTexture = null;
                if (model.textures.baseTexture != null)
                    model.textures.baseTexture.Dispose();
                model.textures.baseTexture = null;
                if (model.textures.curStencil != null)
                    model.textures.curStencil.Dispose();
                model.textures.curStencil = null;
                if (model.textures.specularTexture != null)
                    model.textures.specularTexture.Dispose();
                model.textures.specularTexture = null;
                model = null;
            }
        }

        /// <summary>
        /// This method basically creates and initialize the Direct3D device and
        /// anything else that doens't need to be recreated after a device 
        /// reset.
        /// </summary>
        /// 

        private void Init()
        {

            logMessageToFile("Initialise 3d");

            solidToolStripMenuItem_Click(null, null);

            backgroundColour = Color.SlateBlue;
            wireframeColour = Color.FromArgb(127, 127, 127);

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
            
            d3dDevice = new Device(0, DeviceType.Hardware, this, flags, d3dpp);

            // Register an event-handler for DeviceReset and call it to continue
            // our setup.
            d3dDevice.DeviceReset += new System.EventHandler(this.OnResetDevice);

            //OnResetDevice(d3dDevice, null);
        }

        private void setupShaders()
        {
            logMessageToFile("Setup shaders");
            // Load and initialize shader effect
            string error;
            shader = Effect.FromFile(d3dDevice, Path.Combine(Application.StartupPath, "BodyShader.fx"), null, ShaderFlags.None, null, out error);
            if (shader == null)
            {
                MessageBox.Show(error);
            }
            else
            {
                shader.SetValue(EffectHandle.FromString("gSkinTexture"), skinTexture);
                shader.SetValue(EffectHandle.FromString("gSkinSpecular"), skinSpecular);
                shader.SetValue(EffectHandle.FromString("gMultiplyTexture"), model.textures.baseTexture);
                shader.SetValue(EffectHandle.FromString("gStencilTexture"), model.textures.curStencil);
                if (model.textures.curStencil != null) shader.SetValue(EffectHandle.FromString("gUseStencil"), true);
                shader.SetValue(EffectHandle.FromString("gSpecularTexture"), model.textures.specularTexture);
                shader.SetValue(EffectHandle.FromString("gReliefTexture"), normalMapTexture);
                shader.SetValue(EffectHandle.FromString("gTileCount"), 1.0f);
                shader.SetValue(EffectHandle.FromString("gAmbiColor"), new ColorValue(0.6f, 0.6f, 0.6f));
                shader.SetValue(EffectHandle.FromString("gLamp0Pos"), new Vector4(-10f, 10f, -10f, 1.0f));
                shader.SetValue(EffectHandle.FromString("gPhongExp"), 10.0f);
                shader.SetValue(EffectHandle.FromString("gPhongExp"), 10.0f);
                shader.SetValue(EffectHandle.FromString("gSpecColor"), new ColorValue(0.2f, 0.2f, 0.2f));
                shader.SetValue(EffectHandle.FromString("gSurfaceColor"), new ColorValue(0.5f, 0.5f, 0.5f));
                shader.Technique = shader.GetTechnique("normal_mapping");
            }
            wireframe = Effect.FromFile(d3dDevice, Path.Combine(Application.StartupPath, "Wireframe.fx"), null, ShaderFlags.None, null, out error);
            wireframe.SetValue(EffectHandle.FromString("gColor"), new ColorValue((int)wireframeColour.R, (int)wireframeColour.G, (int)wireframeColour.B));
            wireframe.Technique = wireframe.GetTechnique("wireframe");
        }

        public void resetDevice()
        {
            OnResetDevice(d3dDevice, null);
        }

        /// <summary>
        /// This event-handler is a good place to create and initialize any 
        /// Direct3D related objects, which may become invalid during a 
        /// device reset.
        /// </summary>
        public void OnResetDevice(object sender, EventArgs e)
        {
            logMessageToFile("Reset device");

            Device device = (Device)sender;

            if (device == null) return;

            this.Invalidate();

            device.RenderState.FillMode = getFillMode();

            setupShaders();

            //
            // Create a vertex buffer...
            //

            if (model.numVertices > 0)
            {

                Console.WriteLine(model.bounds.mid.X.ToString() + " : " + model.bounds.mid.Y.ToString() + " : " + model.bounds.mid.Z.ToString());
                Console.WriteLine(spinX.ToString() + " : " + spinY.ToString() + " : " + transZ.ToString() + " : " + height.ToString());
                Console.WriteLine("num vertices: " + model.numVertices.ToString());


                vertexDeclaration = new VertexDeclaration(device,
                    MadScience.Render.vertex.Elements);
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
            }
        }

        /// <summary>
        /// This method is dedicated completely to rendering our 3D scene and is
        /// is called by the OnPaint() event-handler.
        /// </summary>
        private void Render()
        {
            if (d3dDevice == null || shader == null)
                return;

            // Display the frame rate in the title bar of the form - not anymore since this is no longer the main form
            //this.Text = string.Format("Mesh Previewer ({0} fps)", FrameRate.CalculateFrameRate());

            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, backgroundColour, 1.0f, 0);

            d3dDevice.BeginScene();

            // Load the values for the transformation matrices
            // View moves the virtual camera around the 3d model (Lighting moves with the model)
            // World moves the 3d model - (Lighting stays in place as the model rotates, illuminating different parts)
            Matrix viewx = Matrix.Translation(0f, 0, 2f + transZ);
            Matrix worldx = Matrix.Translation(0f, -height, 0f) * Matrix.RotationYawPitchRoll(Geometry.DegreeToRadian(spinX),
                    Geometry.DegreeToRadian(-spinY), 0.0f);
            // * 

            // Projection matrix - typically can be left alone unless the viewport needs to be adjusted
            Matrix projx = Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f),
                (float)Width / Height,
                0.1f, 100.0f);

            // World transform
            shader.SetValue("gWorldXf", worldx);
            // Inverse transpose world transform
            shader.SetValueTranspose("gWorldITXf", Matrix.Invert(worldx));
            // World/view/projection transform
            shader.SetValue("gWvpXf", worldx * viewx * projx);
            if (fillMode == 2)
            {
                wireframe.SetValue("gWvpXf", worldx * viewx * projx);
            }
            // View inverse transform
            shader.SetValue("gViewIXf", Matrix.Invert(viewx));
            // View transform
            shader.SetValue("gViewXf", viewx);
            // World/view transform
            shader.SetValue("gWorldViewXf", worldx * viewx);

            if (model.numVertices > 0 && indexBuffer != null && vertexBuffer != null)
            {

                int passes = shader.Begin(FX.DoNotSaveState);
                for (int loop = 0; loop < passes; loop++)
                {
                    shader.BeginPass(loop);
                    d3dDevice.SetStreamSource(0, vertexBuffer, 0);
                    d3dDevice.Indices = indexBuffer;
                    d3dDevice.VertexDeclaration = vertexDeclaration;

                    d3dDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, (int)model.numVertices, 0, (int)model.numPolygons);
                    shader.EndPass();
                }
                shader.End();
                if (fillMode == 2)
                {
                    passes = wireframe.Begin(FX.None);
                    for (int loop = 0; loop < passes; loop++)
                    {
                        wireframe.BeginPass(loop);
                        d3dDevice.SetStreamSource(0, vertexBuffer, 0);
                        d3dDevice.Indices = indexBuffer;
                        d3dDevice.VertexDeclaration = vertexDeclaration;

                        d3dDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, (int)model.numVertices, 0, (int)model.numPolygons);
                        wireframe.EndPass();
                    }
                    wireframe.End();
                }

            }
            d3dDevice.EndScene();

            d3dDevice.Present();

            //Application.DoEvents();
        }

        public void loadTexture(Stream textureInput, string outputTexture)
        {
            if (textureInput != null && d3dDevice != null)
            {
                logMessageToFile("Load texture");
                switch (outputTexture)
                {
                    case "ambientTexture":
                        this.model.textures.ambientTexture = TextureLoader.FromStream(d3dDevice, textureInput);
                        break;
                    case "baseTexture":
                        this.model.textures.baseTexture = TextureLoader.FromStream(d3dDevice, textureInput);
                        break;
                    case "specularTexture":
                        this.model.textures.specularTexture = TextureLoader.FromStream(d3dDevice, textureInput);
                        break;
                    case "stencilA":
                        this.model.textures.curStencil = TextureLoader.FromStream(d3dDevice, textureInput);
                        break;
                }

            }
        }

        public void loadTextureFromBitmap(Bitmap textureInput, string outputTexture)
        {
            if (textureInput != null && d3dDevice != null)
            {
                logMessageToFile("Load texture");
                switch (outputTexture)
                {
                    case "ambientTexture":
                        this.model.textures.ambientTexture = Texture.FromBitmap(d3dDevice, textureInput, Usage.AutoGenerateMipMap, Pool.Managed);
                        break;
                    case "baseTexture":
                        this.model.textures.baseTexture = Texture.FromBitmap(d3dDevice, textureInput , Usage.AutoGenerateMipMap, Pool.Managed);
                        break;
                    case "specularTexture":
                        this.model.textures.specularTexture = Texture.FromBitmap(d3dDevice, textureInput, Usage.AutoGenerateMipMap, Pool.Managed);
                        break;
                    case "stencilA":
                        this.model.textures.curStencil = Texture.FromBitmap(d3dDevice, textureInput, Usage.AutoGenerateMipMap, Pool.Managed);
                        break;
                }

            }
        }

        public void loadDefaultTextures()
        {

            if (d3dDevice == null) Init();
            //else
            //    OnResetDevice(d3dDevice, null);

            logMessageToFile("Load default skintone");

            Stream skinTextureFile = File.OpenRead(Path.Combine(Application.StartupPath, "afBody_m_0xb4cdc208d8d51bf0_0x00B2D882-0x00000000-0xB4CDC208D8D51BF0.dds"));
            skinTexture = TextureLoader.FromStream(d3dDevice, skinTextureFile);
            skinTextureFile.Close();

            logMessageToFile("Load default skintone specular");

            Stream skinSpecularFile = File.OpenRead(Path.Combine(Application.StartupPath, "S3_00B2D882_00000000_B4CDC208D8D51BEE_afBody_s_0xb4cdc208d8d51bee%%+_IMG.dds"));
            skinSpecular = TextureLoader.FromStream(d3dDevice, skinSpecularFile);
            skinSpecularFile.Close();

            logMessageToFile("Load default skintone normal map");

            Stream normalMapTextureFile = File.OpenRead(Path.Combine(Application.StartupPath, "S3_00B2D882_00000000_B4CDC208D8D51BF3_afBody_n_0xb4cdc208d8d51bf3%%+_IMG.dds"));
            normalMapTexture = TextureLoader.FromStream(d3dDevice, normalMapTextureFile);
            normalMapTextureFile.Close();

            //OnResetDevice(d3dDevice, null);
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            if (d3dDevice == null)
            {
                Init();
                loadDefaultTextures();
                Refresh();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            ptCurrentMousePosit = PointToScreen(new Point(e.X, e.Y));

            if (mousing > 0)
            {
                bMouseDragged = true;

                spinX -= (ptCurrentMousePosit.X - ptLastMousePosit.X);
                if (mousing == 1)
                {
                    transZ += ((Single)(ptCurrentMousePosit.Y - ptLastMousePosit.Y) / 40);
                }
                if (mousing == 2)
                {
                    spinY -= (ptCurrentMousePosit.Y - ptLastMousePosit.Y);
                }
                if (mousing == 3)
                {
                    height -= ((Single)(ptCurrentMousePosit.Y - ptLastMousePosit.Y) / 40);
                }

                if (spinX < 0) spinX += 360;
                if (spinY < 0) spinY += 360;
                if (spinX > 359) spinX -= 360;
                if (spinY > 359) spinY -= 360;

                //Console.WriteLine(spinX.ToString() + " : " + spinY.ToString() + " : " + spinZ.ToString() + " : " + height.ToString());
            }

            ptLastMousePosit = ptCurrentMousePosit;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Delta > 0)
            {
                // Towards object
                transZ += -0.25f;
            }
            else
            {
                // Away from object
                transZ += 0.25f;
            }

            Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
             base.OnKeyDown(e);
             //Console.WriteLine(e.Alt.ToString());
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

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
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            bMouseDragged = false;

            mousing = 0;
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Button == MouseButtons.Right && !bMouseDragged)
            {
                this.contextMenuStrip1.Show(PointToScreen(new Point(e.X, e.Y)));
            }
        }

        public void ResetView()
        {
            height = model.bounds.mid.Y / 1.35f;
            spinX = 180;
            spinY = 0;
            transZ = 0;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renderModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wireframeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solidToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solidWireframeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renderModeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStrip1.Size = new System.Drawing.Size(139, 26);
            // 
            // renderModeToolStripMenuItem
            // 
            this.renderModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wireframeToolStripMenuItem,
            this.solidToolStripMenuItem,
            this.solidWireframeToolStripMenuItem});
            this.renderModeToolStripMenuItem.Name = "renderModeToolStripMenuItem";
            this.renderModeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.renderModeToolStripMenuItem.Text = "Render Mode";
            // 
            // wireframeToolStripMenuItem
            // 
            this.wireframeToolStripMenuItem.Name = "wireframeToolStripMenuItem";
            this.wireframeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.wireframeToolStripMenuItem.Text = "Wireframe";
            this.wireframeToolStripMenuItem.Click += new System.EventHandler(this.wireframeToolStripMenuItem_Click);
            // 
            // solidToolStripMenuItem
            // 
            this.solidToolStripMenuItem.Name = "solidToolStripMenuItem";
            this.solidToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.solidToolStripMenuItem.Text = "Solid";
            this.solidToolStripMenuItem.Click += new System.EventHandler(this.solidToolStripMenuItem_Click);
            // 
            // solidWireframeToolStripMenuItem
            // 
            this.solidWireframeToolStripMenuItem.Name = "solidWireframeToolStripMenuItem";
            this.solidWireframeToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.solidWireframeToolStripMenuItem.Text = "Solid+Wireframe";
            this.solidWireframeToolStripMenuItem.Click += new System.EventHandler(this.solidWireframeToolStripMenuItem_Click);
            // 
            // RenderWindow
            // 
            //this.ContextMenuStrip = this.contextMenuStrip1;
            this.Name = "RenderWindow";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void solidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CurrentFillMode = 1;
            solidToolStripMenuItem.Checked = true;
            wireframeToolStripMenuItem.Checked = false;
            solidWireframeToolStripMenuItem.Checked = false;
        }

        private void wireframeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CurrentFillMode = 0;
            solidToolStripMenuItem.Checked = false;
            wireframeToolStripMenuItem.Checked = true;
            solidWireframeToolStripMenuItem.Checked = false;
        }

        private void solidWireframeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CurrentFillMode = 2;
            solidToolStripMenuItem.Checked = false;
            wireframeToolStripMenuItem.Checked = false;
            solidWireframeToolStripMenuItem.Checked = true;
        }

    }
}
