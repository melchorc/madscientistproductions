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

        public event EventHandler RequireNewTextures;

        public RenderWindow()
        {
            this.ClientSize = new Size(640, 480);
            //this.Text = "Direct3D (DX9/C#) - Indexed Geometry";
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
            logMessageToFile("Initialising components");
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

        private Device d3dDevice = null;

        private int mousing = 0;
        private Point ptLastMousePosit;
        private Point ptCurrentMousePosit;
        private bool bMouseDragged = false;

        private int spinX = 180;
        private int spinY;
        private float transX;
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
        public Label statusLabel;
        public Label lblGeneratingTexture;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem frontToolStripMenuItem;
        private ToolStripMenuItem backToolStripMenuItem;
        private ToolStripMenuItem leftSideToolStripMenuItem;
        private ToolStripMenuItem rightToolStripMenuItem;
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

        public int shaderMode = 0;

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

        public void setModel(modelInfo newModel)
        {
            logMessageToFile("Set model");
            this.model = newModel;
            if (d3dDevice == null) Init();

            ResetView();

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

            //backgroundColour = Color.SlateBlue;
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
            d3dDevice.DeviceLost += new System.EventHandler(this.OnLostDevice);
            d3dDevice.DeviceResizing += new System.ComponentModel.CancelEventHandler(this.CancelResize);
            //OnResetDevice(d3dDevice, null);
        }

        private void CancelResize(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Width < 1 || Height < 1)
                e.Cancel = true;
        }

        public void OnLostDevice(object sender, EventArgs e)
        {
            Console.WriteLine("RenderWindow: Device Lost");
        }

        public void DeInit()
        {
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
            if (d3dDevice != null)
            {
                d3dDevice.Dispose();
                d3dDevice = null;
            }
            renderEnabled = false;
            statusLabel.Text = "3d view currently not initialised.";
            Invalidate();
        }

        private void setupShaders()
        {
            logMessageToFile("Setup shaders");
            // Load and initialize shader effect
            if (d3dDevice == null) return;

            try
            {
                string error = "";
                if (shaderMode == 0)
                {
                    shader = Effect.FromFile(d3dDevice, Path.Combine(Application.StartupPath, "BodyShader.fx"), null, ShaderFlags.None, null, out error);
                }
                if (shaderMode == 1)
                {
                    shader = Effect.FromFile(d3dDevice, Path.Combine(Application.StartupPath, "FaceShader.fx"), null, ShaderFlags.None, null, out error);
                }
                if (shader == null)
                {
                    MessageBox.Show(error);
                }
                else
                {
                    shader.SetValue(EffectHandle.FromString("gSkinTexture"), skinTexture);
                    shader.SetValue(EffectHandle.FromString("gSkinSpecular"), skinSpecular);
                    if (shaderMode == 0) shader.SetValue(EffectHandle.FromString("gMultiplyTexture"), model.textures.baseTexture);
                    shader.SetValue(EffectHandle.FromString("gStencilTexture"), model.textures.curStencil);
                    if (model.textures.curStencil != null) shader.SetValue(EffectHandle.FromString("gUseStencil"), true);
                    if (shaderMode == 0) shader.SetValue(EffectHandle.FromString("gSpecularTexture"), model.textures.specularTexture);
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
            catch (Exception ex)
            {
                logMessageToFile(ex.Message + Environment.NewLine + ex.StackTrace);
            }
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
            
            device.RenderState.ZBufferEnable = true;

            setupShaders();

            //
            // Create a vertex buffer...
            //

            if (model != null && model.numVertices > 0)
            {

                Console.WriteLine("num vertices: " + model.numVertices.ToString());

                this.statusLabel.Text = "Model loaded.  (" + String.Format("{0:0} polygons, ", model.numPolygons) + String.Format("{0:0} vertices)", model.numVertices);


                vertexDeclaration = new VertexDeclaration(device,  MadScience.Render.vertex.Elements);
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
            try
            {
                d3dDevice.TestCooperativeLevel();

                // Display the frame rate in the title bar of the form - not anymore since this is no longer the main form
                //this.Text = string.Format("Mesh Previewer ({0} fps)", FrameRate.CalculateFrameRate());

                d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, backgroundColour, 1.0f, 0);

                d3dDevice.BeginScene();

                // Load the values for the transformation matrices
                // View moves the virtual camera around the 3d model (Lighting moves with the model)
                // World moves the 3d model - (Lighting stays in place as the model rotates, illuminating different parts)
                Matrix viewx = Matrix.Translation(transX, 0f, 2f + transZ);
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
            }
            catch (DeviceLostException)
            {
                System.Threading.Thread.Sleep(500); // Sleep for a while
            }
            catch (DeviceNotResetException)
            {
                if (d3dDevice != null)
                {
                    d3dDevice.Dispose();
                    d3dDevice = null;
                }
                Init();
                resetDevice();
                //for some really strange reason, we lose our textures
                RequireNewTextures(this,new EventArgs());
            }


            //Application.DoEvents();
        }

        public void loadTexture(Stream textureInput, string outputTexture)
        {
            if (textureInput != null && textureInput.Length > 0 && textureInput != Stream.Null && d3dDevice != null)
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
                    case "skinTexture":
                        this.skinTexture = TextureLoader.FromStream(d3dDevice, textureInput);
                        break;
                    case "skinSpecular":
                        this.skinSpecular = TextureLoader.FromStream(d3dDevice, textureInput);
                        break;
                    case "normalMap":
                        this.normalMapTexture = TextureLoader.FromStream(d3dDevice, textureInput);
                        break;

                }
            }
            else
            {
                switch (outputTexture)
                {
                    case "ambientTexture":
                        this.model.textures.ambientTexture = null;
                        break;
                    case "baseTexture":
                        this.model.textures.baseTexture = null;
                        break;
                    case "specularTexture":
                        this.model.textures.specularTexture = null;
                        break;
                    case "stencilA":
                        this.model.textures.curStencil = null;
                        break;
                    case "skinTexture":
                        this.skinTexture = null;
                        break;
                    case "skinSpecular":
                        this.skinSpecular = null;
                        break;
                    case "normalMap":
                        this.normalMapTexture = null;
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
                        this.model.textures.ambientTexture = Texture.FromBitmap(d3dDevice, textureInput, Usage.None, Pool.Managed);
                        break;
                    case "baseTexture":
                        this.model.textures.baseTexture = Texture.FromBitmap(d3dDevice, textureInput, Usage.None, Pool.Managed);
                        break;
                    case "specularTexture":
                        this.model.textures.specularTexture = Texture.FromBitmap(d3dDevice, textureInput, Usage.None, Pool.Managed);
                        break;
                    case "stencilA":
                        this.model.textures.curStencil = Texture.FromBitmap(d3dDevice, textureInput, Usage.None, Pool.Managed);
                        break;
                    case "skinTexture":
                        this.skinTexture = Texture.FromBitmap(d3dDevice, textureInput, Usage.None, Pool.Managed);
                        break;
                    case "skinSpecular":
                        this.skinSpecular = Texture.FromBitmap(d3dDevice, textureInput, Usage.None, Pool.Managed);
                        break;
                    case "normalMap":
                        this.normalMapTexture = Texture.FromBitmap(d3dDevice, textureInput, Usage.None, Pool.Managed);
                        break;
                }

            }
        }

        public void loadDefaultTextures()
        {

            if (d3dDevice == null) Init();
            //else
            //    OnResetDevice(d3dDevice, null);

            if (File.Exists(Path.Combine(Application.StartupPath, "body_m.dds")))
            {
                logMessageToFile("Load default skintone");

                Stream skinTextureFile = File.OpenRead(Path.Combine(Application.StartupPath, "body_m.dds"));
                skinTexture = TextureLoader.FromStream(d3dDevice, skinTextureFile);
                skinTextureFile.Close();
            }

            if (File.Exists(Path.Combine(Application.StartupPath, "body_s.dds")))
            {
                logMessageToFile("Load default skintone specular");

                Stream skinSpecularFile = File.OpenRead(Path.Combine(Application.StartupPath, "body_s.dds"));
                skinSpecular = TextureLoader.FromStream(d3dDevice, skinSpecularFile);
                skinSpecularFile.Close();
            }

            if (File.Exists(Path.Combine(Application.StartupPath, "body_n.dds")))
            {
                logMessageToFile("Load default skintone normal map");

                Stream normalMapTextureFile = File.OpenRead(Path.Combine(Application.StartupPath, "body_n.dds"));
                normalMapTexture = TextureLoader.FromStream(d3dDevice, normalMapTextureFile);
                normalMapTextureFile.Close();
            }
            //OnResetDevice(d3dDevice, null);
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            if (d3dDevice == null)
            {
                Init();
                //loadDefaultTextures();
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

                if (mousing == 1)
                {
                    transZ += ((Single)(ptCurrentMousePosit.Y - ptLastMousePosit.Y) / 40);
                    spinX -= (ptCurrentMousePosit.X - ptLastMousePosit.X);
                }
                if (mousing == 2)
                {
                    spinY -= (ptCurrentMousePosit.Y - ptLastMousePosit.Y);
                    spinX -= (ptCurrentMousePosit.X - ptLastMousePosit.X);
                }
                if (mousing == 3)
                {
                    height -= ((Single)(ptCurrentMousePosit.Y - ptLastMousePosit.Y) / 40);
                    transX -= ((Single)(ptCurrentMousePosit.X - ptLastMousePosit.X) / 40);
                }

                if (spinX < 0) spinX += 360;
                if (spinY < 0) spinY += 360;
                if (spinX > 359) spinX -= 360;
                if (spinY > 359) spinY -= 360;

                Console.WriteLine(spinX.ToString() + " : " + spinY.ToString() + " : " + transX.ToString() + " : " + transZ.ToString() + " : " + height.ToString());
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

            if (renderEnabled && e.Button == MouseButtons.Right && !bMouseDragged)
            {
                this.contextMenuStrip1.Show(PointToScreen(new Point(e.X, e.Y)));
            }
        }

        public void ResetView()
        {
            ResetView(180);
        }

        private void ResetView(int spX)
        {
            Console.WriteLine("camera height: " + height.ToString());
            if (model != null)
            {
                Console.WriteLine("Min: " + model.bounds.min.X.ToString() + " : " + model.bounds.min.Y.ToString() + " : " + model.bounds.min.Z.ToString());
                Console.WriteLine("Mid: " + model.bounds.mid.X.ToString() + " : " + model.bounds.mid.Y.ToString() + " : " + model.bounds.mid.Z.ToString());
                Console.WriteLine("Max: " + model.bounds.max.X.ToString() + " : " + model.bounds.max.Y.ToString() + " : " + model.bounds.max.Z.ToString());

                height = model.bounds.mid.Y + ((model.bounds.max.Y - model.bounds.mid.Y) / 2f);
                if (spX == 180 || spX == 0) transZ = model.bounds.max.X * 0.2f;
                else transZ = model.bounds.max.Z * 0.2f;
            }
            else
            {
                height = 0;
                transX = 0;
                transZ = 0;
            }

            spinX = spX;
            spinY = 0;
            transX = 0;

            Console.WriteLine(spinX.ToString() + " : " + spinY.ToString() + " : " + transZ.ToString());


            Invalidate();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renderModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wireframeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solidToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solidWireframeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.frontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leftSideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusLabel = new System.Windows.Forms.Label();
            this.lblGeneratingTexture = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renderModeToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStrip1.Size = new System.Drawing.Size(139, 48);
            // 
            // renderModeToolStripMenuItem
            // 
            this.renderModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wireframeToolStripMenuItem,
            this.solidToolStripMenuItem,
            this.solidWireframeToolStripMenuItem});
            this.renderModeToolStripMenuItem.Name = "renderModeToolStripMenuItem";
            this.renderModeToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.renderModeToolStripMenuItem.Text = "Render Mode";
            // 
            // wireframeToolStripMenuItem
            // 
            this.wireframeToolStripMenuItem.Name = "wireframeToolStripMenuItem";
            this.wireframeToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.wireframeToolStripMenuItem.Text = "Wireframe";
            this.wireframeToolStripMenuItem.Click += new System.EventHandler(this.wireframeToolStripMenuItem_Click);
            // 
            // solidToolStripMenuItem
            // 
            this.solidToolStripMenuItem.Name = "solidToolStripMenuItem";
            this.solidToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
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
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.frontToolStripMenuItem,
            this.backToolStripMenuItem,
            this.leftSideToolStripMenuItem,
            this.rightToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // frontToolStripMenuItem
            // 
            this.frontToolStripMenuItem.Name = "frontToolStripMenuItem";
            this.frontToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.frontToolStripMenuItem.Text = "Front";
            this.frontToolStripMenuItem.Click += new System.EventHandler(this.frontToolStripMenuItem_Click);
            // 
            // backToolStripMenuItem
            // 
            this.backToolStripMenuItem.Name = "backToolStripMenuItem";
            this.backToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.backToolStripMenuItem.Text = "Back";
            this.backToolStripMenuItem.Click += new System.EventHandler(this.backToolStripMenuItem_Click);
            // 
            // leftSideToolStripMenuItem
            // 
            this.leftSideToolStripMenuItem.Name = "leftSideToolStripMenuItem";
            this.leftSideToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.leftSideToolStripMenuItem.Text = "Left";
            this.leftSideToolStripMenuItem.Click += new System.EventHandler(this.leftSideToolStripMenuItem_Click);
            // 
            // rightToolStripMenuItem
            // 
            this.rightToolStripMenuItem.Name = "rightToolStripMenuItem";
            this.rightToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.rightToolStripMenuItem.Text = "Right";
            this.rightToolStripMenuItem.Click += new System.EventHandler(this.rightToolStripMenuItem_Click);
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
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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

        private void RenderWindow_Load(object sender, EventArgs e)
        {
            if (d3dDevice == null) Init();
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
