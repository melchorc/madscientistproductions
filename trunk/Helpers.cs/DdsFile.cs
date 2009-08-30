//------------------------------------------------------------------------------
/*
	@brief		DDS File Type Plugin for Paint.NET

	@note		Copyright (c) 2006 Dean Ashton         http://www.dmashton.co.uk

	Permission is hereby granted, free of charge, to any person obtaining
	a copy of this software and associated documentation files (the 
	"Software"), to	deal in the Software without restriction, including
	without limitation the rights to use, copy, modify, merge, publish,
	distribute, sublicense, and/or sell copies of the Software, and to 
	permit persons to whom the Software is furnished to do so, subject to 
	the following conditions:

	The above copyright notice and this permission notice shall be included
	in all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
	OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
	MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
	IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
	CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
	TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
	SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
**/
//------------------------------------------------------------------------------

// If we want to do the alignment as per the (broken) DDS documentation, then we
// uncomment this define.. 
//#define	APPLY_PITCH_ALIGNMENT

using System;
using System.IO;
//using PaintDotNet;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DdsFileTypePlugin
{
	public enum DdsFileFormat
	{
        DDS_FORMAT_DXT1 = 0x31545844,
        DDS_FORMAT_DXT3 = 0x33545844,
        DDS_FORMAT_DXT5 = 0x35545844,
		DDS_FORMAT_A8R8G8B8,
		DDS_FORMAT_X8R8G8B8,
		DDS_FORMAT_A8B8G8R8,
		DDS_FORMAT_X8B8G8R8,
		DDS_FORMAT_A1R5G5B5,
		DDS_FORMAT_A4R4G4B4,
		DDS_FORMAT_R8G8B8,
		DDS_FORMAT_R5G6B5,
        DDS_FORMAT_A8L8,
        DDS_FORMAT_ATI1 = 0x31495441,
        DDS_FORMAT_ATI2 = 0x32495441,

		DDS_FORMAT_INVALID,
	};

	public class DdsPixelFormat
	{
		public enum PixelFormatFlags
		{
            DDS_ALPHAPIXELS =   0x00000001,
			DDS_FOURCC	    =	0x00000004,
			DDS_RGB		    =	0x00000040,
            DDS_ALPHA       =   0x00000002,
			//DDS_RGBA	    =	0x00000041,
            DDS_COMPRESSED  =   0x00000080,
            DDS_LUMINANCE   =   0x00020000,
		}

	    public uint	m_size;
	    public uint	m_flags;
	    public uint	m_fourCC;
	    public uint	m_rgbBitCount;
	    public uint	m_rBitMask;
	    public uint	m_gBitMask;
	    public uint	m_bBitMask;
	    public uint	m_aBitMask;

		public uint	Size()
		{
			return 8 * 4;
		}

		public void Initialise( DdsFileFormat fileFormat )
		{
			m_size = Size();
			switch( fileFormat )
			{
				case	DdsFileFormat.DDS_FORMAT_DXT1:
				case	DdsFileFormat.DDS_FORMAT_DXT3:
				case	DdsFileFormat.DDS_FORMAT_DXT5:
				{
					// DXT1/DXT3/DXT5
					m_flags			= ( int )PixelFormatFlags.DDS_FOURCC;
					m_rgbBitCount	=	0;
					m_rBitMask		=	0;
					m_gBitMask		=	0;
					m_bBitMask		=	0;
					m_aBitMask		=	0;
                    if (fileFormat == DdsFileFormat.DDS_FORMAT_DXT1) { m_fourCC = 0x31545844; }//"DXT1"
                    if (fileFormat == DdsFileFormat.DDS_FORMAT_DXT3) { m_fourCC = 0x33545844; } //"DXT1"
                    if (fileFormat == DdsFileFormat.DDS_FORMAT_DXT5) { m_fourCC = 0x35545844; } //"DXT1"
					break;
				}
	
				case	DdsFileFormat.DDS_FORMAT_A8R8G8B8:
				{	
					m_flags			= ( int )PixelFormatFlags.DDS_RGB + (int)PixelFormatFlags.DDS_ALPHAPIXELS;
					m_rgbBitCount	= 32;
					m_fourCC		= 0;
					m_rBitMask		= 0x00ff0000;
					m_gBitMask		= 0x0000ff00;
					m_bBitMask		= 0x000000ff;
					m_aBitMask		= 0xff000000;
					break;
				}

				case	DdsFileFormat.DDS_FORMAT_X8R8G8B8:
				{	
					m_flags			= ( int )PixelFormatFlags.DDS_RGB;
					m_rgbBitCount	= 32;
					m_fourCC		= 0;
					m_rBitMask		= 0x00ff0000;
					m_gBitMask		= 0x0000ff00;
					m_bBitMask		= 0x000000ff;
					m_aBitMask		= 0x00000000;
					break;
				}

				case	DdsFileFormat.DDS_FORMAT_A8B8G8R8:
				{	
					m_flags			= ( int )PixelFormatFlags.DDS_RGB + (int)PixelFormatFlags.DDS_ALPHAPIXELS;
					m_rgbBitCount	= 32;
					m_fourCC		= 0;
					m_rBitMask		= 0x000000ff;
					m_gBitMask		= 0x0000ff00;
					m_bBitMask		= 0x00ff0000;
					m_aBitMask		= 0xff000000;
					break;
				}

				case	DdsFileFormat.DDS_FORMAT_X8B8G8R8:
				{	
					m_flags			= ( int )PixelFormatFlags.DDS_RGB;
					m_rgbBitCount	= 32;
					m_fourCC		= 0;
					m_rBitMask		= 0x000000ff;
					m_gBitMask		= 0x0000ff00;
					m_bBitMask		= 0x00ff0000;
					m_aBitMask		= 0x00000000;
					break;
				}

				case	DdsFileFormat.DDS_FORMAT_A1R5G5B5:
				{	
					m_flags			= ( int )PixelFormatFlags.DDS_RGB + (int)PixelFormatFlags.DDS_ALPHAPIXELS;
					m_rgbBitCount	= 16;
					m_fourCC		= 0;
					m_rBitMask		= 0x00007c00;
					m_gBitMask		= 0x000003e0;
					m_bBitMask		= 0x0000001f;
					m_aBitMask		= 0x00008000;
					break;
				}

				case	DdsFileFormat.DDS_FORMAT_A4R4G4B4:
				{	
					m_flags			= ( int )PixelFormatFlags.DDS_RGB + (int)PixelFormatFlags.DDS_ALPHAPIXELS;
					m_rgbBitCount	= 16;
					m_fourCC		= 0;
					m_rBitMask		= 0x00000f00;
					m_gBitMask		= 0x000000f0;
					m_bBitMask		= 0x0000000f;
					m_aBitMask		= 0x0000f000;
					break;
				}

				case	DdsFileFormat.DDS_FORMAT_R8G8B8:
				{	
					m_flags			= ( int )PixelFormatFlags.DDS_RGB;
					m_fourCC		= 0;
					m_rgbBitCount	= 24;
					m_rBitMask		= 0x00ff0000;
					m_gBitMask		= 0x0000ff00;
					m_bBitMask		= 0x000000ff;
					m_aBitMask		= 0x00000000;
					break;
				}

				case	DdsFileFormat.DDS_FORMAT_R5G6B5:
				{	
					m_flags			= ( int )PixelFormatFlags.DDS_RGB;
					m_fourCC		= 0;
					m_rgbBitCount	= 16;
					m_rBitMask		= 0x0000f800;
					m_gBitMask		= 0x000007e0;
					m_bBitMask		= 0x0000001f;
					m_aBitMask		= 0x00000000;
					break;
				}
                case DdsFileFormat.DDS_FORMAT_A8L8:
                {
                    m_flags = (int)PixelFormatFlags.DDS_RGB + (int)PixelFormatFlags.DDS_ALPHAPIXELS;
                    m_fourCC = 0;
                    m_rgbBitCount = 16;
                    m_rBitMask = 0x000000ff;
                    m_gBitMask = 0x00000000;
                    m_bBitMask = 0x00000000;
                    m_aBitMask = 0x0000ff00;
                    break;

                }
				default:
					break;
			}
		}

		public void Read( BinaryReader input )
		{
			this.m_size			= input.ReadUInt32();
	    	this.m_flags		= input.ReadUInt32();
	    	this.m_fourCC		= input.ReadUInt32();
            this.m_rgbBitCount = input.ReadUInt32();
            this.m_rBitMask = input.ReadUInt32();
            this.m_gBitMask = input.ReadUInt32();
            this.m_bBitMask = input.ReadUInt32();
            this.m_aBitMask = input.ReadUInt32();
		}

	}

	public class DdsHeader
	{
		public enum HeaderFlags
		{
			DDS_HEADER_FLAGS_TEXTURE	=	0x00001007,	// DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT 
			DDS_HEADER_FLAGS_MIPMAP		=	0x00020000,	// DDSD_MIPMAPCOUNT
			DDS_HEADER_FLAGS_VOLUME		=	0x00800000,	// DDSD_DEPTH
			DDS_HEADER_FLAGS_PITCH		=	0x00000008,	// DDSD_PITCH
			DDS_HEADER_FLAGS_LINEARSIZE	=	0x00080000,	// DDSD_LINEARSIZE
		}

		public enum SurfaceFlags
		{
			DDS_SURFACE_FLAGS_TEXTURE	=	0x00001000,	// DDSCAPS_TEXTURE
			DDS_SURFACE_FLAGS_MIPMAP	=	0x00400008,	// DDSCAPS_COMPLEX | DDSCAPS_MIPMAP
			DDS_SURFACE_FLAGS_CUBEMAP	=	0x00000008,	// DDSCAPS_COMPLEX
		}

		public enum CubemapFlags
		{
			DDS_CUBEMAP_POSITIVEX		=	0x00000600, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEX
			DDS_CUBEMAP_NEGATIVEX		=	0x00000a00, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEX
			DDS_CUBEMAP_POSITIVEY		=	0x00001200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEY
			DDS_CUBEMAP_NEGATIVEY		=	0x00002200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEY
			DDS_CUBEMAP_POSITIVEZ		=	0x00004200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEZ
			DDS_CUBEMAP_NEGATIVEZ		=	0x00008200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEZ
		
			DDS_CUBEMAP_ALLFACES		=	(	DDS_CUBEMAP_POSITIVEX | DDS_CUBEMAP_NEGATIVEX |
												DDS_CUBEMAP_POSITIVEY | DDS_CUBEMAP_NEGATIVEY |
												DDS_CUBEMAP_POSITIVEZ | DDS_CUBEMAP_NEGATIVEZ )
		}

		public enum VolumeFlags
		{
			DDS_FLAGS_VOLUME			=	0x00200000,	// DDSCAPS2_VOLUME
		}

		public DdsHeader()
		{
			m_pixelFormat	= new DdsPixelFormat();
		}

		public uint	Size()
		{
			return ( 18 * 4 ) + m_pixelFormat.Size() + ( 5 * 4 );
		}

		public uint				m_size;
		public uint				m_headerFlags;
		public uint				m_height;
		public uint				m_width;
		public uint				m_pitchOrLinearSize;
		public uint				m_depth;
		public uint				m_mipMapCount;
		public uint				m_reserved1_0;
		public uint				m_reserved1_1;
		public uint				m_reserved1_2;
		public uint				m_reserved1_3;
		public uint				m_reserved1_4;
		public uint				m_reserved1_5;
		public uint				m_reserved1_6;
		public uint				m_reserved1_7;
		public uint				m_reserved1_8;
		public uint				m_reserved1_9;
		public uint				m_reserved1_10;
		public DdsPixelFormat	m_pixelFormat;
		public uint				m_surfaceFlags;
		public uint				m_cubemapFlags;
		public uint				m_reserved2_0;
		public uint				m_reserved2_1;
		public uint				m_reserved2_2;
        public string fileFormat;

		public void Read( System.IO.Stream input )
		{
            BinaryReader Utility = new BinaryReader(input);

			this.m_size					= Utility.ReadUInt32();
	    	this.m_headerFlags			= Utility.ReadUInt32();
	    	this.m_height				= Utility.ReadUInt32();
	    	this.m_width				= Utility.ReadUInt32();
	    	this.m_pitchOrLinearSize	= Utility.ReadUInt32();
	    	this.m_depth				= Utility.ReadUInt32();
	    	this.m_mipMapCount			= Utility.ReadUInt32();
	    	this.m_reserved1_0			= Utility.ReadUInt32();
	    	this.m_reserved1_1			= Utility.ReadUInt32();
	    	this.m_reserved1_2			= Utility.ReadUInt32();
	    	this.m_reserved1_3			= Utility.ReadUInt32();
	    	this.m_reserved1_4			= Utility.ReadUInt32();
	    	this.m_reserved1_5			= Utility.ReadUInt32();
	    	this.m_reserved1_6			= Utility.ReadUInt32();
	    	this.m_reserved1_7			= Utility.ReadUInt32();
	    	this.m_reserved1_8			= Utility.ReadUInt32();
	    	this.m_reserved1_9			= Utility.ReadUInt32();
	    	this.m_reserved1_10			= Utility.ReadUInt32();
            this.m_pixelFormat.Read(Utility);
			this.m_surfaceFlags			= Utility.ReadUInt32( );
			this.m_cubemapFlags			= Utility.ReadUInt32( );
			this.m_reserved2_0			= Utility.ReadUInt32( );
			this.m_reserved2_1			= Utility.ReadUInt32( );
			this.m_reserved2_2			= Utility.ReadUInt32( );

		}


	}	

	public partial class DdsFile
	{
		public	DdsFile()
		{
			m_header = new DdsHeader();
		}

        public Image Image()
        {
            return this.Image(true, true, true, false, true);
        }
        public Image Image(bool red)
        {
            return this.Image(true, false, false, false, true);
        }
        public Image Image(bool red, bool green)
        {
            return this.Image(true, true, false, false, true);
        }
        public Image Image(bool red, bool green, bool blue)
        {
            return this.Image(true, true, true, false, true);
        }

        private MemoryStream _rawDDS = new MemoryStream();
        public MemoryStream DDS()
        {
            _rawDDS.Seek(0, SeekOrigin.Begin);
            return this._rawDDS;
        }

        //public Image Image(Color background, Color HSVShift)
        //{
            //return Image(background, HSVShift, HSVShift, HSVShift, HSVShift);
        //}


        public Image Image(Color background, Color c1, Color c2, Color c3, Color c4)
        {

            int height = this.GetHeight();
            int width = this.GetWidth();

            if (height == 0 && width == 0)
            {
                height = 1;
                width = 1;
            }
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            //MemoryStream ms = new MemoryStream();
            //bitmap.Save(ms, ImageFormat.Bmp);
            //ms.Seek(54, SeekOrigin.Begin);

            //byte[] readPixelData = this.GetPixelData();
            byte[] readPixelData = this.m_pixelData;

            if (readPixelData == null) return bitmap;

            BitmapData newData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int pixelSize = 4;


            //MemoryStream ms = new MemoryStream();
            //bitmap.Save(ms, ImageFormat.Bmp);
            //ms.Seek(54, SeekOrigin.Begin);

            uint maxValue = 0;

            unsafe
            {
                for (int y = 0; y < height; y++)
                {
                    byte* nRow = (byte*)newData.Scan0 + (y * newData.Stride);

                    for (int x = 0; x < width; x++)
                    {

                        int readPixelOffset = (y * width * 4) + (x * 4);

                        int cred = 0;
                        int cgreen = 0;
                        int cblue = 0;
                        int calpha = 0;

                        if (c1 != Color.Empty) { cred = readPixelData[readPixelOffset + 0]; }
                        if (c2 != Color.Empty) { cgreen = readPixelData[readPixelOffset + 1]; }
                        if (c3 != Color.Empty) { cblue = readPixelData[readPixelOffset + 2]; }
                        if (c4 != Color.Empty)
                        {
                            calpha = readPixelData[readPixelOffset + 3];
                            // Inverse the alpha
                            //calpha = (255 - calpha);
                        }

                        Color color = Color.Empty;
                        if (c4 != Color.Empty)
                        {
                            color = Color.FromArgb(calpha, cred, cgreen, cblue);
                        }
                        else
                        {
                            color = Color.FromArgb(cred, cgreen, cblue);
                        }

                        Color color2 = Color.Black;
                        float num3 = 0f;

                        if (c1 != Color.Empty)
                        {
                            maxValue = ((uint)((((((uint)Convert.ToSingle(1.00)) * 0xff) << 0x18) + ((((uint)Convert.ToSingle(0.00) * 0xff) << 0x10)) + ((((uint)Convert.ToSingle(0.00)) * 0xff) << 8))) + (((uint)Convert.ToSingle(0.00)) * 0xff));
                            if (maxValue == uint.MaxValue)
                            {
                                num3 = 1f;
                            }
                            else if ((maxValue & 0xff000000) == 0xff000000)
                            {
                                num3 = 0.003921569f * color.R;
                            }
                            else if ((maxValue & 0xff0000) == 0xff0000)
                            {
                                num3 = 0.003921569f * color.G;
                            }
                            else if ((maxValue & 0xff00) == 0xff00)
                            {
                                num3 = 0.003921569f * color.B;
                            }
                            else if ((maxValue & 0xff) == 0xff)
                            {
                                num3 = 0.003921569f * color.A;
                            }
                            color2 = Color.FromArgb(0xff, Math.Max(0, Math.Min(0xff, (int)((color2.R + (color2.R * -num3)) + (((int)(c1.R * (0.003921569f * c1.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.G + (color2.G * -num3)) + (((int)(c1.G * (0.003921569f * c1.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.B + (color2.B * -num3)) + (((int)(c1.B * (0.003921569f * c1.A))) * num3)))));

                        }
                        if (c2 != Color.Empty)
                        {
                            maxValue = ((0 * 0xff) << 0x18) + ((1 * 0xff) << 0x10) + ((0 * 0xff) << 8) + ((0 * 0xff));
                            if (maxValue == uint.MaxValue)
                            {
                                num3 = 1f;
                            }
                            else if ((maxValue & 0xff000000) == 0xff000000)
                            {
                                num3 = 0.003921569f * color.R;
                            }
                            else if ((maxValue & 0xff0000) == 0xff0000)
                            {
                                num3 = 0.003921569f * color.G;
                            }
                            else if ((maxValue & 0xff00) == 0xff00)
                            {
                                num3 = 0.003921569f * color.B;
                            }
                            else if ((maxValue & 0xff) == 0xff)
                            {
                                num3 = 0.003921569f * color.A;
                            }
                            color2 = Color.FromArgb(0xff, Math.Max(0, Math.Min(0xff, (int)((color2.R + (color2.R * -num3)) + (((int)(c2.R * (0.003921569f * c2.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.G + (color2.G * -num3)) + (((int)(c2.G * (0.003921569f * c2.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.B + (color2.B * -num3)) + (((int)(c2.B * (0.003921569f * c2.A))) * num3)))));
                        }
                        if (c3 != Color.Empty)
                        {
                            maxValue = ((0 * 0xff) << 0x18) + ((0 * 0xff) << 0x10) + ((1 * 0xff) << 8) + ((0 * 0xff));
                            if (maxValue == uint.MaxValue)
                            {
                                num3 = 1f;
                            }
                            else if ((maxValue & 0xff000000) == 0xff000000)
                            {
                                num3 = 0.003921569f * color.R;
                            }
                            else if ((maxValue & 0xff0000) == 0xff0000)
                            {
                                num3 = 0.003921569f * color.G;
                            }
                            else if ((maxValue & 0xff00) == 0xff00)
                            {
                                num3 = 0.003921569f * color.B;
                            }
                            else if ((maxValue & 0xff) == 0xff)
                            {
                                num3 = 0.003921569f * color.A;
                            }
                            color2 = Color.FromArgb(0xff, Math.Max(0, Math.Min(0xff, (int)((color2.R + (color2.R * -num3)) + (((int)(c3.R * (0.003921569f * c3.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.G + (color2.G * -num3)) + (((int)(c3.G * (0.003921569f * c3.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.B + (color2.B * -num3)) + (((int)(c3.B * (0.003921569f * c3.A))) * num3)))));

                        }
                        if (c4 != Color.Empty)
                        {
                            maxValue = ((0 * 0xff) << 0x18) + ((0 * 0xff) << 0x10) + ((0 * 0xff) << 8) + ((1 * 0xff));
                            if (maxValue == uint.MaxValue)
                            {
                                num3 = 1f;
                            }
                            else if ((maxValue & 0xff000000) == 0xff000000)
                            {
                                num3 = 0.003921569f * color.R;
                            }
                            else if ((maxValue & 0xff0000) == 0xff0000)
                            {
                                num3 = 0.003921569f * color.G;
                            }
                            else if ((maxValue & 0xff00) == 0xff00)
                            {
                                num3 = 0.003921569f * color.B;
                            }
                            else if ((maxValue & 0xff) == 0xff)
                            {
                                num3 = 0.003921569f * color.A;
                            }
                            color2 = Color.FromArgb(0xff, Math.Max(0, Math.Min(0xff, (int)((color2.R + (color2.R * -num3)) + (((int)(c4.R * (0.003921569f * c4.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.G + (color2.G * -num3)) + (((int)(c4.G * (0.003921569f * c4.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.B + (color2.B * -num3)) + (((int)(c4.B * (0.003921569f * c4.A))) * num3)))));
                        }

                        //Color color = pixel.GetPixel(i, j);
                        //Color color2 = pixel2.GetPixel(i, j);

                        nRow[x * pixelSize] = color2.B;
                        nRow[x * pixelSize + 1] = color2.G;
                        nRow[x * pixelSize + 2] = color2.R;
                        nRow[x * pixelSize + 3] = color2.A;

                        /*
                        if (alpha)
                        {
                            if (!red && !green && !blue)
                            {

                            }
                            //ms.WriteByte((byte)calpha);
                            nRow[x * pixelSize + 3] = color2.A;

                            //bitmap.SetPixel(x, y, Color.FromArgb(calpha, cred, cgreen, cblue ));
                        }
                        else
                        {
                            //ms.WriteByte((byte)0);
                            nRow[x * pixelSize + 3] = 255;
                            //bitmap.SetPixel(x, y, Color.FromArgb(cred, cgreen, cblue));
                        }
                        //bitmap.SetPixel(x, y, color2);
                        */
                    }
                }
            }
            bitmap.UnlockBits(newData);

            return bitmap;
        }

        public Image Image(bool red, bool green, bool blue, bool alpha)
        {
            return Image(red, green, blue, alpha, false);
        }

        public Image Image(bool red, bool green, bool blue, bool alpha, bool invAlpha)
        {

            //unsafe
            //{
            int height = this.GetHeight();
            int width = this.GetWidth();

            //byte[] readPixelData = this.GetPixelData();
            //byte[] myBuffer = (byte[])readPixelData.Clone();
            //byte[] myBuffer = (byte[])this.GetPixelData().Clone();
            byte[] myBuffer = (byte[])this.m_pixelData.Clone();

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            if (myBuffer == null) return bitmap;

            //BitmapData newData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int pixelSize = 4;
            int sizeInBytes = (width * height * pixelSize);

            for (int i = 0; i < sizeInBytes; i += pixelSize)
            {
                //for (int y = (height - 1); y >= 0; y--)
                //for (int y = 0; y < height; y++)
                //{
                //get the data from the new image
                //byte* nRow = (byte*)newData.Scan0 + (y * newData.Stride);

                //for (int x = 0; x < width; x++)
                //{
                //int readPixelOffset = (y * width * 4) + (x * 4);

                uint cred = 0;
                uint cgreen = 0;
                uint cblue = 0;
                uint calpha = 0;

                //if (red) { cred = readPixelData[readPixelOffset + 0]; }
                //if (green) { cgreen = readPixelData[readPixelOffset + 1]; }
                //if (blue) { cblue = readPixelData[readPixelOffset + 2]; }

                if (alpha)
                {
                    //calpha = readPixelData[readPixelOffset + 3];
                    calpha = myBuffer[i + 3];
                    // Inverse the alpha
                    if (invAlpha) calpha = 255 - calpha;
                }
                else
                {
                    calpha = 255;
                }

                if (!red && !green && !blue)
                {
                    // Don't invert the alpha is viewing on it's own
                    // but instead do the entire image as greyscale
                    cred = calpha;
                    cblue = calpha;
                    cgreen = calpha;
                    calpha = 255;
                }
                else
                {
                    if (red) cred = myBuffer[i + 0];
                    if (green) cgreen = myBuffer[i + 1];
                    if (blue) cblue = myBuffer[i + 2];
                }

                        myBuffer[i + 2] = (byte)cred;
                        myBuffer[i + 1] = (byte)cgreen;
                        myBuffer[i + 0] = (byte)cblue;
                        myBuffer[i + 3] = (byte)calpha;


                //ms.WriteByte((byte)cblue);
                //ms.WriteByte((byte)cgreen);
                //ms.WriteByte((byte)cred);

                //if (x == 0 && y == 0) Console.WriteLine("A: " + calpha.ToString() + " R: " + cred.ToString() + " G: " + cgreen.ToString() + " B: " + cblue.ToString());
                /*
                if (!red && !green && !blue)
                {
                    // Don't invert the alpha is viewing on it's own
                    // but instead do the entire image as greyscale
                            
                    cred = calpha;
                    cblue = calpha;
                    cgreen = calpha;
                    calpha = 255;
                }
                */
                //nRow[x * pixelSize] = (byte)cblue;
                //nRow[x * pixelSize + 1] = (byte)cgreen;
                //nRow[x * pixelSize + 2] = (byte)cred;

                //if (alpha)
                //{
                //    if (!red && !green && !blue)
                //    {

                //    }
                //    //ms.WriteByte((byte)calpha);
                //    nRow[x * pixelSize + 3] = (byte)calpha;

                //bitmap.SetPixel(x, y, Color.FromArgb(calpha, cred, cgreen, cblue ));
                //}
                //else
                //{
                //    //ms.WriteByte((byte)0);
                //    nRow[x * pixelSize + 3] = 255;
                //    //bitmap.SetPixel(x, y, Color.FromArgb(cred, cgreen, cblue));
                //}
                //}
            }

            //bitmap = System.Drawing.Bitmap.FromStream(ms, NNop);
            //bitmap = new Bitmap(ms, );

            //bitmap.UnlockBits(newData);
            //sourceBitmap.UnlockBits(originalData);
            //Console.WriteLine(bitmap.GetPixel(0, 0).ToString());

            Rectangle area = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            BitmapData data = bitmap.LockBits(area, ImageLockMode.WriteOnly, bitmap.PixelFormat);

            Marshal.Copy(myBuffer, 0, data.Scan0, sizeInBytes);

            bitmap.UnlockBits(data);
            return bitmap;

            //return bitmap;
            //}
        }

        private void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
        }

        #region Decode Functions
        private static byte[] DecodeTextureData(Stream file, DdsFileFormat fourcc, int width, int height, int mipmaps, int mipmapno, bool mipmapreversed)
        {
            UInt32 start = GetMipMapOffset(fourcc, width, height, mipmaps, mipmapno, mipmapreversed);
            UInt32 size = GetMipMapSize(fourcc, width, height, mipmaps, mipmapno);

            //if (input.Length == 0) return null;
            if (size == 0 || size + start + file.Position > file.Length)
                return null;

            width = (width + (1 << mipmapno) - 1) >> mipmapno;
            height = (height + (1 << mipmapno) - 1) >> mipmapno;

            file.Seek(start, System.IO.SeekOrigin.Current);
            byte[] data = new byte[size];
            file.Read(data, 0, (int)size);

            byte[] pixeldata = new byte[width * height * 4];
            //pixeldata[0] = 0x42;
            //pixeldata[1] = 0x4d;
            //BitConverter.GetBytes(pixeldata.Length).CopyTo(pixeldata, 2);
            //pixeldata[10] = 0x36;
            //pixeldata[14] = 0x28;
            //BitConverter.GetBytes(width).CopyTo(pixeldata, 18);
            //BitConverter.GetBytes(height).CopyTo(pixeldata, 22);
            //pixeldata[26] = 1;
            //pixeldata[28] = 32;
            //BitConverter.GetBytes(width * height * 4).CopyTo(pixeldata, 0);

            switch (fourcc)
            {
                case DdsFileFormat.DDS_FORMAT_ATI1:
                    DecodeATI1Texture(pixeldata, 0, width, height, data);
                    break;
                case DdsFileFormat.DDS_FORMAT_ATI2:
                    DecodeATI2Texture(pixeldata, 0, width, height, data);
                    break;
                case DdsFileFormat.DDS_FORMAT_DXT1:
                    DecodeDXT1Texture(pixeldata, 0, width, height, data);
                    break;
                case DdsFileFormat.DDS_FORMAT_DXT5:
                    DecodeDXT5Texture(pixeldata, 0, width, height, data);
                    break;
            }

            //System.IO.MemoryStream ms = new System.IO.MemoryStream(pixeldata);
            //using (ms)
            //{
            //    return new Bitmap(Image.FromStream(ms));
            //}
            return pixeldata;
        }

        private static void DecodeDXT5AlphaBlock(int[] pixels, int poffset, byte[] data, int doffset, int[] alphavals)
        {
            UInt64 curdata = BitConverter.ToUInt64(data, doffset);
            alphavals[0] = (int)(curdata & 0xff);
            curdata >>= 8;
            alphavals[1] = (int)(curdata & 0xff);
            curdata >>= 8;
            if (alphavals[0] > alphavals[1])
            {
                for (int loopi = 1; loopi < 7; loopi++)
                {
                    alphavals[loopi + 1] = ((7 - loopi) * alphavals[0] + loopi * alphavals[1]) / 7;
                }
            }
            else
            {
                for (int loopi = 1; loopi < 5; loopi++)
                {
                    alphavals[loopi + 1] = ((5 - loopi) * alphavals[0] + loopi * alphavals[1]) / 5;
                }
                alphavals[6] = 0;
                alphavals[7] = 255;
            }

            for (int loopsuby = 0; loopsuby < 4; loopsuby++)
            {
                for (int loopsubx = 0; loopsubx < 4; loopsubx++)
                {
                    pixels[loopsuby * 4 + loopsubx + poffset] = alphavals[curdata & 7];
                    curdata >>= 3;
                }
            }
        }

        private static void DecodeATI1Texture(byte[] pixels, int poffset, int width, int height, byte[] data)
        {
            int[] colors = new int[8];
            for (int loopy = 0; loopy < height; loopy += 4)
            {
                for (int loopx = 0; loopx < width; loopx += 4)
                {
                    int loc = (((loopx + 3) >> 2) + ((loopy + 3) >> 2) * ((width + 3) >> 2)) << 3;
                    UInt64 curdata = BitConverter.ToUInt64(data, loc);
                    colors[0] = (int)(curdata & 0xff);
                    curdata >>= 8;
                    colors[1] = (int)(curdata & 0xff);
                    curdata >>= 8;
                    if (colors[0] > colors[1])
                    {
                        for (int loopi = 1; loopi < 7; loopi++)
                        {
                            colors[loopi + 1] = ((7 - loopi) * colors[0] + loopi * colors[1]) / 7;
                        }
                    }
                    else
                    {
                        for (int loopi = 1; loopi < 5; loopi++)
                        {
                            colors[loopi + 1] = ((5 - loopi) * colors[0] + loopi * colors[1]) / 5;
                        }
                        colors[6] = 0;
                        colors[7] = 255;
                    }

                    for (int loopsuby = 0; loopsuby < 4; loopsuby++)
                    {
                        if (loopy + loopsuby < height)
                        {
                            //int pixloc = poffset + (loopx << 2) + (height - (loopsuby + loopy + 1)) * (width << 2);
                            int pixloc = poffset + (loopx << 2) + ((loopsuby + loopy )) * (width << 2);
                            for (int loopsubx = 0; loopsubx < 4; loopsubx++, pixloc += 4)
                            {
                                if (loopx + loopsubx < width)
                                {
                                    uint c = (uint)colors[curdata & 7];
                                    curdata >>= 3;
                                    pixels[pixloc] =
                                        pixels[pixloc + 1] =
                                        pixels[pixloc + 2] = (byte)c;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void DecodeATI2Texture(byte[] pixels, int poffset, int width, int height, byte[] data)
        {
            int[] colorsg = new int[8];
            int[] colorsr = new int[8];
            for (int loopy = 0; loopy < height; loopy += 4)
            {
                for (int loopx = 0; loopx < width; loopx += 4)
                {
                    for (int loopc = 0; loopc < 2; loopc++)
                    {
                        int loc = (((loopx + 3) >> 2) + ((loopy + 3) >> 2) * ((width + 3) >> 2)) << 4;
                        UInt64 curdatag = BitConverter.ToUInt64(data, loc);
                        UInt64 curdatar = BitConverter.ToUInt64(data, loc + 8);
                        colorsg[0] = (int)(curdatag & 0xff);
                        curdatag >>= 8;
                        colorsg[1] = (int)(curdatag & 0xff);
                        curdatag >>= 8;
                        if (colorsg[0] > colorsg[1])
                        {
                            for (int loopi = 1; loopi < 7; loopi++)
                            {
                                colorsg[loopi + 1] = ((7 - loopi) * colorsg[0] + loopi * colorsg[1]) / 7;
                            }
                        }
                        else
                        {
                            for (int loopi = 1; loopi < 5; loopi++)
                            {
                                colorsg[loopi + 1] = ((5 - loopi) * colorsg[0] + loopi * colorsg[1]) / 5;
                            }
                            colorsg[6] = 0;
                            colorsg[7] = 255;
                        }
                        colorsr[0] = (int)(curdatar & 0xff);
                        curdatar >>= 8;
                        colorsr[1] = (int)(curdatar & 0xff);
                        curdatar >>= 8;
                        if (colorsr[0] > colorsr[1])
                        {
                            for (int loopi = 1; loopi < 7; loopi++)
                            {
                                colorsr[loopi + 1] = ((7 - loopi) * colorsr[0] + loopi * colorsr[1]) / 7;
                            }
                        }
                        else
                        {
                            for (int loopi = 1; loopi < 5; loopi++)
                            {
                                colorsr[loopi + 1] = ((5 - loopi) * colorsr[0] + loopi * colorsr[1]) / 5;
                            }
                            colorsr[6] = 0;
                            colorsr[7] = 255;
                        }

                        for (int loopsuby = 0; loopsuby < 4; loopsuby++)
                        {
                            if (loopy + loopsuby < height)
                            {
                                //int pixloc = poffset + (loopx << 2) + (height - (loopsuby + loopy + 1)) * (width << 2);
                                int pixloc = poffset + (loopx << 2) + ((loopsuby + loopy)) * (width << 2);
                                for (int loopsubx = 0; loopsubx < 4; loopsubx++, pixloc += 4)
                                {
                                    if (loopx + loopsubx < width)
                                    {
                                        int g = colorsg[curdatag & 7];
                                        curdatag >>= 3;
                                        int r = colorsr[curdatar & 7];
                                        curdatar >>= 3;
                                        double b = Math.Round(Math.Sqrt(128 * 128 - (r - 127) * (r - 127) - (g - 127) * (g - 127)) + 127);
                                        if (b > 255)
                                            b = 255;
                                        else if (b < 128 || double.IsNaN(b))
                                            b = 128;
                                        pixels[pixloc + 2] = (byte)(uint)r;
                                        pixels[pixloc + 1] = (byte)(uint)g;
                                        pixels[pixloc] = (byte)b;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void DecodeDXT1Texture(byte[] pixels, int poffset, int width, int height, byte[] data)
        {
            UInt32[] colors = new UInt32[4];
            for (int loopy = 0; loopy < height; loopy += 4)
            {
                for (int loopx = 0; loopx < width; loopx += 4)
                {
                    int loc = (((loopx + 3) >> 2) + ((loopy + 3) >> 2) * ((width + 3) >> 2)) << 3;
                    UInt64 curdataC = BitConverter.ToUInt64(data, loc);
                    UInt32 c1 = (UInt32)(curdataC & ((1 << 16) - 1));
                    curdataC >>= 16;
                    UInt32 c2 = (UInt32)(curdataC & ((1 << 16) - 1));
                    curdataC >>= 16;
                    UInt32 b1 = (UInt32)(c1 & ((1 << 5) - 1)) << 3;
                    b1 |= b1 >> 5;
                    UInt32 g1 = (UInt32)((c1 >> 5) & ((1 << 6) - 1)) << 2;
                    g1 |= g1 >> 6;
                    UInt32 r1 = (UInt32)(c1 >> 11) << 3;
                    r1 |= r1 >> 5;
                    UInt32 b2 = (UInt32)(c2 & ((1 << 5) - 1)) << 3;
                    b2 |= b2 >> 5;
                    UInt32 g2 = (UInt32)((c2 >> 5) & ((1 << 6) - 1)) << 2;
                    g2 |= g2 >> 6;
                    UInt32 r2 = (UInt32)(c2 >> 11) << 3;
                    r2 |= r2 >> 5;

                    /*
                    colors[0] = (r1 << 16) | (g1 << 8) | b1;
                    colors[1] = (r2 << 16) | (g2 << 8) | b2;
                    if (c1 > c2)
                    {
                        colors[2] =
                            (((2 * r1 + r2) / 3) << 16) |
                            (((2 * g1 + g2) / 3) << 8) |
                            ((2 * b1 + b2) / 3);
                        colors[3] =
                            (((r1 + 2 * r2) / 3) << 16) |
                            (((g1 + 2 * g2) / 3) << 8) |
                            ((b1 + 2 * b2) / 3);
                    }
                    else
                    {
                        colors[2] =
                            (((r1 + r2) >> 1) << 16) |
                            (((g1 + g2) >> 1) << 8) |
                            ((b1 + b2) >> 1);
                        colors[3] = 255 << 1;
                    }
                     */
                    colors[0] = (b1 << 16) | (g1 << 8) | r1;
                    colors[1] = (b2 << 16) | (g2 << 8) | r2;
                    if (c1 > c2)
                    {
                        colors[2] =
                            (((2 * b1 + b2) / 3) << 16) |
                            (((2 * g1 + g2) / 3) << 8) |
                            ((2 * r1 + r2) / 3);
                        colors[3] =
                            (((b1 + 2 * b2) / 3) << 16) |
                            (((g1 + 2 * g2) / 3) << 8) |
                            ((r1 + 2 * r2) / 3);
                    }
                    else
                    {
                        colors[2] =
                            (((b1 + b2) >> 1) << 16) |
                            (((g1 + g2) >> 1) << 8) |
                            ((r1 + r2) >> 1);
                        colors[3] = 255 << 1;
                    }


                    for (int loopsuby = 0; loopsuby < 4; loopsuby++)
                    {
                        //int pixloc = poffset + (loopx << 2) + (height - (loopsuby + loopy + 1)) * (width << 2);
                        int pixloc = poffset + (loopx << 2) + ((loopsuby + loopy)) * (width << 2);
                        for (int loopsubx = 0; loopsubx < 4; loopsubx++, pixloc += 4)
                        {
                            if (loopx + loopsubx < width)
                            {
                                BitConverter.GetBytes(colors[curdataC & 3]).CopyTo(pixels, pixloc);
                            }
                            curdataC >>= 2;
                        }
                    }
                }
            }
        }

        private static void DecodeDXT5Texture(byte[] pixels, int poffset, int width, int height, byte[] data)
        {
            UInt32[] alphas = new UInt32[8];
            UInt32[] colors = new UInt32[4];
            for (int loopy = 0; loopy < height; loopy += 4)
            {
                for (int loopx = 0; loopx < width; loopx += 4)
                {
                    int loc = (((loopx + 3) >> 2) + ((loopy + 3) >> 2) * ((width + 3) >> 2)) << 4;
                    UInt64 curdataA = BitConverter.ToUInt64(data, loc);
                    alphas[0] = (uint)(curdataA & 0xff);
                    curdataA >>= 8;
                    alphas[1] = (uint)(curdataA & 0xff);
                    curdataA >>= 8;
                    if (alphas[0] > alphas[1])
                    {
                        for (int loopi = 1; loopi < 7; loopi++)
                        {
                            alphas[loopi + 1] = ((UInt32)((7 - loopi) * alphas[0] + loopi * alphas[1]) / 7) << 24;
                        }
                    }
                    else
                    {
                        for (int loopi = 1; loopi < 5; loopi++)
                        {
                            alphas[loopi + 1] = ((UInt32)((5 - loopi) * alphas[0] + loopi * alphas[1]) / 5) << 24;
                        }
                        alphas[6] = 0;
                        alphas[7] = (UInt32)255 << 24;
                    }
                    alphas[0] <<= 24;
                    alphas[1] <<= 24;

                    UInt64 curdataC = BitConverter.ToUInt64(data, loc + 8);
                    UInt32 c1 = (UInt32)(curdataC & ((1 << 16) - 1));
                    curdataC >>= 16;
                    UInt32 c2 = (UInt32)(curdataC & ((1 << 16) - 1));
                    curdataC >>= 16;
                    UInt32 b1 = (UInt32)(c1 & ((1 << 5) - 1)) << 3;
                    b1 |= b1 >> 5;
                    UInt32 g1 = (UInt32)((c1 >> 5) & ((1 << 6) - 1)) << 2;
                    g1 |= g1 >> 6;
                    UInt32 r1 = (UInt32)(c1 >> 11) << 3;
                    r1 |= r1 >> 5;
                    UInt32 b2 = (UInt32)(c2 & ((1 << 5) - 1)) << 3;
                    b2 |= b2 >> 5;
                    UInt32 g2 = (UInt32)((c2 >> 5) & ((1 << 6) - 1)) << 2;
                    g2 |= g2 >> 6;
                    UInt32 r2 = (UInt32)(c2 >> 11) << 3;
                    r2 |= r2 >> 5;
                    //colors[0] = (r1 << 16) | (g1 << 8) | b1;
                    //colors[1] = (r2 << 16) | (g2 << 8) | b2;
                    //colors[2] =
                    //    (((2 * r1 + r2) / 3) << 16) |
                    //    (((2 * g1 + g2) / 3) << 8) |
                    //    ((2 * b1 + b2) / 3);
                    //colors[3] =
                    //    (((r1 + 2 * r2) / 3) << 16) |
                    //    (((g1 + 2 * g2) / 3) << 8) |
                    //    ((b1 + 2 * b2) / 3);
                    colors[0] = (b1 << 16) | (g1 << 8) | r1;
                    colors[1] = (b2 << 16) | (g2 << 8) | r2;
                    colors[2] =
                        (((2 * b1 + b2) / 3) << 16) |
                        (((2 * g1 + g2) / 3) << 8) |
                        ((2 * r1 + r2) / 3);
                    colors[3] =
                        (((b1 + 2 * b2) / 3) << 16) |
                        (((g1 + 2 * g2) / 3) << 8) |
                        ((r1 + 2 * r2) / 3);



                    for (int loopsuby = 0; loopsuby < 4; loopsuby++)
                    {
                        //int pixloc = poffset + (loopx << 2) + (height - (loopsuby + loopy + 1)) * (width << 2);
                        int pixloc = poffset + (loopx << 2) + ((loopsuby + loopy)) * (width << 2);
                        for (int loopsubx = 0; loopsubx < 4; loopsubx++, pixloc += 4)
                        {
                            if (loopx + loopsubx < width)
                            {
                                UInt32 c = alphas[curdataA & 7] | colors[curdataC & 3];
                                BitConverter.GetBytes(c).CopyTo(pixels, pixloc);
                            }
                            curdataA >>= 3;
                            curdataC >>= 2;
                        }
                    }
                }
            }
        }

        static private UInt32 GetMipMapOffset(DdsFileFormat fourcc, int width, int height, int mipmaps, int mipmapno, bool mipmapreversed)
        {
            UInt32 offset = 0;
            if (mipmapreversed)
            {
                for (int loop = mipmaps - 1; loop > mipmapno; loop--)
                {
                    offset += GetMipMapSize(fourcc, width, height, mipmaps, loop);
                }
            }
            else
            {
                for (int loop = mipmapno; loop > 0; loop--)
                {
                    offset += GetMipMapSize(fourcc, width, height, mipmaps, loop - 1);
                }
            }
            return offset;
        }

        static private UInt32 GetMipMapSize(DdsFileFormat fourcc, int width, int height, int mipmaps, int mipmapno)
        {
            width = (width + (1 << mipmapno) - 1) >> mipmapno;
            height = (height + (1 << mipmapno) - 1) >> mipmapno;
            switch (fourcc)
            {
                case DdsFileFormat.DDS_FORMAT_DXT1:
                    return (UInt32)(((width + 3) / 4) * ((height + 3) / 4)) * 8;
                case DdsFileFormat.DDS_FORMAT_DXT5:
                    return (UInt32)(((width + 3) / 4) * ((height + 3) / 4)) * 16;
                case DdsFileFormat.DDS_FORMAT_ATI1:
                    return (UInt32)(((width + 3) / 4) * ((height + 3) / 4)) * 8;
                case DdsFileFormat.DDS_FORMAT_ATI2:
                    return (UInt32)(((width + 3) / 4) * ((height + 3) / 4)) * 16;
                default:
                    return 0;
            }
        }
        #endregion

		public	void	Load( System.IO.Stream input )
		{
            if (input.Length == 0) return;

            ReadWriteStream(input, this._rawDDS);
            input.Seek(0, SeekOrigin.Begin);

            BinaryReader Utility = new BinaryReader(input);

			// Read the DDS tag. If it's not right, then bail.. 
			uint	ddsTag = Utility.ReadUInt32( );
			if ( ddsTag != 0x20534444 )
				throw new FormatException( "File does not appear to be a DDS image" );

			// Read everything in.. for now assume it worked like a charm..
			m_header.Read( input );

			if ( ( m_header.m_pixelFormat.m_flags & ( int )DdsPixelFormat.PixelFormatFlags.DDS_FOURCC ) != 0 )
			{
				//int	squishFlags = 0;
                DdsFileFormat ddsFormat = new DdsFileFormat();

				switch ( m_header.m_pixelFormat.m_fourCC )
				{
					case	0x31545844:
						//squishFlags = ( int )DdsSquish.SquishFlags.kDxt1;
                        ddsFormat = DdsFileFormat.DDS_FORMAT_DXT1;
                        m_header.fileFormat = "DXT1";
						break;

					case	0x33545844:
						//squishFlags = ( int )DdsSquish.SquishFlags.kDxt3;
                        ddsFormat = DdsFileFormat.DDS_FORMAT_DXT3;
                        m_header.fileFormat = "DXT3";
						break;

					case	0x35545844:
						//squishFlags = ( int )DdsSquish.SquishFlags.kDxt5;
                        ddsFormat = DdsFileFormat.DDS_FORMAT_DXT5;
                        m_header.fileFormat = "DXT5";
						break;
                    case    0x31495441:
                        ddsFormat = DdsFileFormat.DDS_FORMAT_ATI1;
                        m_header.fileFormat = "ATI1";
                        break;
                    case    0x32495441:
                        ddsFormat = DdsFileFormat.DDS_FORMAT_ATI2;
                        m_header.fileFormat = "ATI2";
                        break;

					default:
						throw new FormatException( "File is not a supported DDS format" );
				}

				// Compute size of compressed block area
				//int blockCount = ( ( GetWidth() + 3 )/4 ) * ( ( GetHeight() + 3 )/4 );
                //int blockSize = 0;
                //((squishFlags & (int)DdsSquish.SquishFlags.kDxt1) != 0) ? 8 : 16;
                //if ((squishFlags & (int)DdsFileFormat.DDS_FORMAT_DXT1) != 0)
                //{
                    //blockSize = 8;
                //}
                //else
                //{
                    //blockSize = 16;
                //}
				
				// Allocate room for compressed blocks, and read data into it.
				//byte[] compressedBlocks = new byte[ blockCount * blockSize ];
				//input.Read( compressedBlocks, 0, compressedBlocks.GetLength( 0 ) );

				// Now decompress..
				//m_pixelData = DdsSquish.DecompressImage( input, GetWidth(), GetHeight(), squishFlags );
                this.m_pixelData = DecodeTextureData(input, ddsFormat, GetWidth(), GetHeight(), 0, 0, false);
			}
			else
			{
				// We can only deal with the non-DXT formats we know about..  this is a bit of a mess..
				// Sorry..
				DdsFileFormat	fileFormat = DdsFileFormat.DDS_FORMAT_INVALID;

				if (	( m_header.m_pixelFormat.m_flags == ( int )DdsPixelFormat.PixelFormatFlags.DDS_RGB + (int)DdsPixelFormat.PixelFormatFlags.DDS_ALPHAPIXELS) && 
						( m_header.m_pixelFormat.m_rgbBitCount == 32 ) && 
						( m_header.m_pixelFormat.m_rBitMask == 0x00ff0000 ) && ( m_header.m_pixelFormat.m_gBitMask == 0x0000ff00 ) &&
						( m_header.m_pixelFormat.m_bBitMask == 0x000000ff ) && ( m_header.m_pixelFormat.m_aBitMask == 0xff000000 ) )
                { fileFormat = DdsFileFormat.DDS_FORMAT_A8R8G8B8; m_header.fileFormat = "ARGB8"; }
				else
				if (	( m_header.m_pixelFormat.m_flags == ( int )DdsPixelFormat.PixelFormatFlags.DDS_RGB ) && 
						( m_header.m_pixelFormat.m_rgbBitCount == 32 ) && 
						( m_header.m_pixelFormat.m_rBitMask == 0x00ff0000 ) && ( m_header.m_pixelFormat.m_gBitMask == 0x0000ff00 ) &&
						( m_header.m_pixelFormat.m_bBitMask == 0x000000ff ) && ( m_header.m_pixelFormat.m_aBitMask == 0x00000000 ) )
                { fileFormat = DdsFileFormat.DDS_FORMAT_X8R8G8B8; m_header.fileFormat = "X8RGB8"; }
				else
                    if ((m_header.m_pixelFormat.m_flags == (int)DdsPixelFormat.PixelFormatFlags.DDS_RGB + (int)DdsPixelFormat.PixelFormatFlags.DDS_ALPHAPIXELS) &&
                            (m_header.m_pixelFormat.m_rgbBitCount == 32) &&
                            (m_header.m_pixelFormat.m_rBitMask == 0x000000ff) && (m_header.m_pixelFormat.m_gBitMask == 0x0000ff00) &&
                            (m_header.m_pixelFormat.m_bBitMask == 0x00ff0000) && (m_header.m_pixelFormat.m_aBitMask == 0xff000000))
                    { fileFormat = DdsFileFormat.DDS_FORMAT_A8B8G8R8; m_header.fileFormat = "ABGR8"; }
                    else
                        if ((m_header.m_pixelFormat.m_flags == (int)DdsPixelFormat.PixelFormatFlags.DDS_RGB) &&
                                (m_header.m_pixelFormat.m_rgbBitCount == 32) &&
                                (m_header.m_pixelFormat.m_rBitMask == 0x000000ff) && (m_header.m_pixelFormat.m_gBitMask == 0x0000ff00) &&
                                (m_header.m_pixelFormat.m_bBitMask == 0x00ff0000) && (m_header.m_pixelFormat.m_aBitMask == 0x00000000))
                        { fileFormat = DdsFileFormat.DDS_FORMAT_X8B8G8R8; m_header.fileFormat = "XBGR8"; }
                        else
                            if ((m_header.m_pixelFormat.m_flags == (int)DdsPixelFormat.PixelFormatFlags.DDS_RGB + (int)DdsPixelFormat.PixelFormatFlags.DDS_ALPHAPIXELS) &&
                                    (m_header.m_pixelFormat.m_rgbBitCount == 16) &&
                                    (m_header.m_pixelFormat.m_rBitMask == 0x00007c00) && (m_header.m_pixelFormat.m_gBitMask == 0x000003e0) &&
                                    (m_header.m_pixelFormat.m_bBitMask == 0x0000001f) && (m_header.m_pixelFormat.m_aBitMask == 0x00008000))
                            { fileFormat = DdsFileFormat.DDS_FORMAT_A1R5G5B5; m_header.fileFormat = "ARGB"; }
                            else
                                if ((m_header.m_pixelFormat.m_flags == (int)DdsPixelFormat.PixelFormatFlags.DDS_RGB + (int)DdsPixelFormat.PixelFormatFlags.DDS_ALPHAPIXELS) &&
                                        (m_header.m_pixelFormat.m_rgbBitCount == 16) &&
                                        (m_header.m_pixelFormat.m_rBitMask == 0x00000f00) && (m_header.m_pixelFormat.m_gBitMask == 0x000000f0) &&
                                        (m_header.m_pixelFormat.m_bBitMask == 0x0000000f) && (m_header.m_pixelFormat.m_aBitMask == 0x0000f000))
                                {
                                    fileFormat = DdsFileFormat.DDS_FORMAT_A4R4G4B4;
                                    m_header.fileFormat = "ARGB4";
                                }
                                else
                                    if ((m_header.m_pixelFormat.m_flags == (int)DdsPixelFormat.PixelFormatFlags.DDS_RGB) &&
                                            (m_header.m_pixelFormat.m_rgbBitCount == 24) &&
                                            (m_header.m_pixelFormat.m_rBitMask == 0x00ff0000) && (m_header.m_pixelFormat.m_gBitMask == 0x0000ff00) &&
                                            (m_header.m_pixelFormat.m_bBitMask == 0x000000ff) && (m_header.m_pixelFormat.m_aBitMask == 0x00000000))
                                    { fileFormat = DdsFileFormat.DDS_FORMAT_R8G8B8; m_header.fileFormat = "RGB8"; }
                                    else
                                        if ((m_header.m_pixelFormat.m_flags == (int)DdsPixelFormat.PixelFormatFlags.DDS_RGB) &&
                                                (m_header.m_pixelFormat.m_rgbBitCount == 16) &&
                                                (m_header.m_pixelFormat.m_rBitMask == 0x0000f800) && (m_header.m_pixelFormat.m_gBitMask == 0x000007e0) &&
                                                (m_header.m_pixelFormat.m_bBitMask == 0x0000001f) && (m_header.m_pixelFormat.m_aBitMask == 0x00000000))
                                        { fileFormat = DdsFileFormat.DDS_FORMAT_R5G6B5; m_header.fileFormat = "RGB5"; }
                                        else
                                            if ((m_header.m_pixelFormat.m_flags == (int)DdsPixelFormat.PixelFormatFlags.DDS_LUMINANCE + (int)DdsPixelFormat.PixelFormatFlags.DDS_ALPHAPIXELS) &&
                                                (m_header.m_pixelFormat.m_rgbBitCount == 16) &&
                                                (m_header.m_pixelFormat.m_rBitMask == 0x000000ff) && (m_header.m_pixelFormat.m_gBitMask == 0x00000000) &&
                                                (m_header.m_pixelFormat.m_bBitMask == 0x00000000) && (m_header.m_pixelFormat.m_aBitMask == 0x0000ff00))
                                            { fileFormat = DdsFileFormat.DDS_FORMAT_A8L8; m_header.fileFormat = "A8L8"; }


				// If fileFormat is still invalid, then it's an unsupported format.
                if (fileFormat == DdsFileFormat.DDS_FORMAT_INVALID)
                {
                    System.Windows.Forms.MessageBox.Show("File is not a supported DDS format");
                    return;

                }

				// Size of a source pixel, in bytes
				int srcPixelSize = ( ( int )m_header.m_pixelFormat.m_rgbBitCount / 8 );

				// We need the pitch for a row, so we can allocate enough memory for the load.
				int rowPitch = 0;

				if ( ( m_header.m_headerFlags & ( int )DdsHeader.HeaderFlags.DDS_HEADER_FLAGS_PITCH ) != 0 )	
				{
					// Pitch specified.. so we can use directly
					rowPitch = ( int )m_header.m_pitchOrLinearSize;
				}
				else
				if ( ( m_header.m_headerFlags & ( int )DdsHeader.HeaderFlags.DDS_HEADER_FLAGS_LINEARSIZE ) != 0 )
				{
					// Linear size specified.. compute row pitch. Of course, this should never happen
					// as linear size is *supposed* to be for compressed textures. But Microsoft don't 
					// always play by the rules when it comes to DDS output. 
					rowPitch = ( int )m_header.m_pitchOrLinearSize / ( int )m_header.m_height;
				}
				else
				{
					// Another case of Microsoft not obeying their standard is the 'Convert to..' shell extension
					// that ships in the DirectX SDK. Seems to always leave flags empty..so no indication of pitch
					// or linear size. And - to cap it all off - they leave pitchOrLinearSize as *zero*. Zero??? If
					// we get this bizarre set of inputs, we just go 'screw it' and compute row pitch ourselves, 
					// making sure we DWORD align it (if that code path is enabled).
					rowPitch = ( ( int )m_header.m_width * srcPixelSize );

#if	APPLY_PITCH_ALIGNMENT
					rowPitch = ( ( ( int )rowPitch + 3 ) & ( ~3 ) );
#endif	// APPLY_PITCH_ALIGNMENT
				}

//				System.Diagnostics.Debug.WriteLine( "Image width : " + m_header.m_width + ", rowPitch = " + rowPitch );

				// Ok.. now, we need to allocate room for the bytes to read in from.. it's rowPitch bytes * height
				byte[] readPixelData = new byte[ rowPitch * m_header.m_height ];
				input.Read( readPixelData, 0, readPixelData.GetLength( 0 ) );

				// We now need space for the real pixel data.. that's width * height * 4..
				m_pixelData = new byte[ m_header.m_width * m_header.m_height * 4 ];

				// And now we have the arduous task of filling that up with stuff..
				for ( int destY = 0; destY < ( int )m_header.m_height; destY++ )	
				{
					for ( int destX = 0; destX < ( int )m_header.m_width; destX++ )	
					{
						// Compute source pixel offset
						int	srcPixelOffset = ( destY * rowPitch ) + ( destX * srcPixelSize );

						// Read our pixel
						uint	pixelColour = 0;
						uint	pixelRed	= 0;
						uint	pixelGreen	= 0;	
						uint	pixelBlue	= 0;
						uint	pixelAlpha	= 0;

						// Build our pixel colour as a DWORD	
						for ( int loop = 0; loop < srcPixelSize; loop++ )
						{
							pixelColour |= ( uint )( readPixelData[ srcPixelOffset + loop ] << ( 8 * loop ) );
						}

						if ( fileFormat == DdsFileFormat.DDS_FORMAT_A8R8G8B8 )
						{
							pixelAlpha	= ( pixelColour >> 24 ) & 0xff;
							pixelRed	= ( pixelColour >> 16 ) & 0xff;
							pixelGreen	= ( pixelColour >> 8  ) & 0xff;
							pixelBlue	= ( pixelColour >> 0  ) & 0xff;
						}
						else
						if ( fileFormat == DdsFileFormat.DDS_FORMAT_X8R8G8B8 )
						{
							pixelAlpha	= 0xff;
							pixelRed	= ( pixelColour >> 16 ) & 0xff;
							pixelGreen	= ( pixelColour >> 8  ) & 0xff;
							pixelBlue	= ( pixelColour >> 0  ) & 0xff;
						}
						else
						if ( fileFormat == DdsFileFormat.DDS_FORMAT_A8B8G8R8 )
						{
							pixelAlpha	= ( pixelColour >> 24 ) & 0xff;
							pixelRed	= ( pixelColour >> 0  ) & 0xff;
							pixelGreen	= ( pixelColour >> 8  ) & 0xff;
							pixelBlue	= ( pixelColour >> 16 ) & 0xff;
						}
						else
						if ( fileFormat == DdsFileFormat.DDS_FORMAT_X8B8G8R8 )
						{
							pixelAlpha	= 0xff;
							pixelRed	= ( pixelColour >> 0  ) & 0xff;
							pixelGreen	= ( pixelColour >> 8  ) & 0xff;
							pixelBlue	= ( pixelColour >> 16 ) & 0xff;
						}
						else
						if ( fileFormat == DdsFileFormat.DDS_FORMAT_A1R5G5B5 )
						{
							pixelAlpha	= ( pixelColour >> 15 ) * 0xff;
							pixelRed	= ( pixelColour >> 10 ) & 0x1f;
							pixelGreen	= ( pixelColour >> 5  ) & 0x1f;
							pixelBlue	= ( pixelColour >> 0  ) & 0x1f;

							pixelRed	= ( pixelRed   << 3 ) | ( pixelRed   >> 2 );
							pixelGreen	= ( pixelGreen << 3 ) | ( pixelGreen >> 2 );
							pixelBlue	= ( pixelBlue  << 3 ) | ( pixelBlue  >> 2 );
						}
						else
						if ( fileFormat == DdsFileFormat.DDS_FORMAT_A4R4G4B4 )
						{
							pixelAlpha	= ( pixelColour >> 12 ) & 0xff;
							pixelRed	= ( pixelColour >> 8  ) & 0x0f;
							pixelGreen	= ( pixelColour >> 4  ) & 0x0f;
							pixelBlue	= ( pixelColour >> 0  ) & 0x0f;

							pixelAlpha	= ( pixelAlpha << 4 ) | ( pixelAlpha >> 0 );
							pixelRed	= ( pixelRed   << 4 ) | ( pixelRed   >> 0 );
							pixelGreen	= ( pixelGreen << 4 ) | ( pixelGreen >> 0 );
							pixelBlue	= ( pixelBlue  << 4 ) | ( pixelBlue  >> 0 );
						}
						else
						if ( fileFormat == DdsFileFormat.DDS_FORMAT_R8G8B8 )
						{
							pixelAlpha	= 0xff;
							pixelRed	= ( pixelColour >> 16 ) & 0xff;
							pixelGreen	= ( pixelColour >> 8  ) & 0xff;
							pixelBlue	= ( pixelColour >> 0  ) & 0xff;
						}
						else
						if ( fileFormat == DdsFileFormat.DDS_FORMAT_R5G6B5 )
						{
							pixelAlpha	= 0xff;
							pixelRed	= ( pixelColour >> 11 ) & 0x1f;
							pixelGreen	= ( pixelColour >> 5  ) & 0x3f;
							pixelBlue	= ( pixelColour >> 0  ) & 0x1f;

							pixelRed	= ( pixelRed   << 3 ) | ( pixelRed   >> 2 );
							pixelGreen	= ( pixelGreen << 2 ) | ( pixelGreen >> 4 );
							pixelBlue	= ( pixelBlue  << 3 ) | ( pixelBlue  >> 2 );
						}
															
						// Write the colours away..
						int	destPixelOffset	= ( destY * ( int )m_header.m_width * 4 ) + ( destX * 4 );
						m_pixelData[ destPixelOffset + 0 ]	= ( byte )pixelRed;
						m_pixelData[ destPixelOffset + 1 ]	= ( byte )pixelGreen;
						m_pixelData[ destPixelOffset + 2 ]	= ( byte )pixelBlue;
						m_pixelData[ destPixelOffset + 3 ]	= ( byte )pixelAlpha;
					}
				}	
			}
		}
	
		public	int		GetWidth()
		{
			return ( int )m_header.m_width;
		}

		public	int		GetHeight()
		{
			return ( int )m_header.m_height;
		}

		public	byte[]	GetPixelData()
		{
			return m_pixelData;
		}

		// Loaded DDS header (also uses storage for save)
		public	DdsHeader	m_header;
	
		// Pixel data
		byte[]				m_pixelData;
		
	}
}