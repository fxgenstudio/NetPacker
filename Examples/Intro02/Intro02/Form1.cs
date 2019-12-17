using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Intro02
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


            CreateOpenGLContext();
        }

        IntPtr _hDC, _hRC;
        protected virtual void CreateOpenGLContext()
        {
            try
            {
                PIXELFORMATDESCRIPTOR pfd = new PIXELFORMATDESCRIPTOR();
                pfd.Initialize();


                _hDC = GetDC(Handle);
                if (_hDC == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                int iPixelformat = ChoosePixelFormat(_hDC, ref pfd);
                if (iPixelformat == 0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                if (SetPixelFormat(_hDC, iPixelformat, ref pfd) == 0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                _hRC = wglCreateContext(_hDC);
                if (_hRC == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                if (wglMakeCurrent(_hDC, _hRC) == 0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            }
            catch (Exception)
            {
                if (_hRC != IntPtr.Zero)
                    wglDeleteContext(_hRC);

                if (_hDC != IntPtr.Zero)
                    ReleaseDC(Handle, _hDC);

                throw;
            }
        }

        #region OpenGL32.dll
        [DllImport("opengl32.dll", SetLastError = true)]
        public static extern System.IntPtr wglGetCurrentContext();
        [DllImport("opengl32.dll", SetLastError = true)]
        public static extern System.IntPtr wglGetCurrentDC();
        [DllImport("opengl32.dll", SetLastError = true)]
        public static extern System.IntPtr wglCreateContext(IntPtr hdc);
        [DllImport("opengl32.dll", SetLastError = true)]
        public static extern System.Int32 wglMakeCurrent(IntPtr hdc, IntPtr hglrc);
        [DllImport("opengl32.dll", SetLastError = true)]
        public static extern System.Int32 wglDeleteContext(IntPtr hglrc);
        [DllImport("opengl32.dll", SetLastError = true)]
        public static extern bool wglShareLists(IntPtr hglrcDest, IntPtr hglrsSrc);
        #endregion

        #region PIXELFORMATDESCRIPTOR 
        public const byte
            PFD_TYPE_RGBA = 0,
            PFD_TYPE_COLORINDEX = 1;

        public const sbyte
            PFD_MAIN_PLANE = 0,
            PFD_OVERLAY_PLANE = 1,
            PFD_UNDERLAY_PLANE = -1;

        public const System.UInt32
            PFD_DOUBLEBUFFER = 0x00000001,
            PFD_STEREO = 0x00000002,
            PFD_DRAW_TO_WINDOW = 0x00000004,
            PFD_DRAW_TO_BITMAP = 0x00000008,
            PFD_SUPPORT_GDI = 0x00000010,
            PFD_SUPPORT_OPENGL = 0x00000020,
            PFD_GENERIC_FORMAT = 0x00000040,
            PFD_NEED_PALETTE = 0x00000080,
            PFD_NEED_SYSTEM_PALETTE = 0x00000100,
            PFD_SWAP_EXCHANGE = 0x00000200,
            PFD_SWAP_COPY = 0x00000400,
            PFD_SWAP_LAYER_BUFFERS = 0x00000800,
            PFD_GENERIC_ACCELERATED = 0x00001000,
            PFD_SUPPORT_DIRECTDRAW = 0x00002000;

        public const System.UInt32
            PFD_DEPTH_DONTCARE = 0x20000000,
            PFD_DOUBLEBUFFER_DONTCARE = 0x40000000,
            PFD_STEREO_DONTCARE = 0x80000000;

        [StructLayout(LayoutKind.Sequential)]
        public struct PIXELFORMATDESCRIPTOR
        {
            public ushort nSize, nVersion;
            public uint dwFlags;
            public byte
                iPixelType, cColorBits, cRedBits, cRedShift,
                cGreenBits, cGreenShift, cBlueBits, cBlueShift,
                cAlphaBits, cAlphaShift, cAccumBits, cAccumRedBits,
                cAccumGreenBits, cAccumBlueBits, cAccumAlphaBits,
                cDepthBits, cStencilBits, cAuxBuffers, iLayerType, bReserved;
            public uint dwLayerMask, dwVisibleMask, dwDamageMask;

            public void Initialize()
            {
                nSize = (ushort)Marshal.SizeOf(this);
                nVersion = 1;
                dwFlags = PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER | PFD_GENERIC_ACCELERATED;
                iPixelType = PFD_TYPE_RGBA;
                cColorBits = 32;
                cAccumBits = 32;
                cDepthBits = 24;
                cStencilBits = 8;
                cAuxBuffers = 0;
                iLayerType = 0; //PFD_MAIN_PLANE
                bReserved = 0;
                dwLayerMask = dwVisibleMask = dwDamageMask = 0;
            }
        }
        #endregion

        #region Functions from gdi32.dll
        [DllImport("gdi32.dll", SetLastError = true)]
        public unsafe static extern System.Int32 ChoosePixelFormat(IntPtr hdc, ref PIXELFORMATDESCRIPTOR ppfd);
        [DllImport("gdi32.dll", SetLastError = true)]
        public unsafe static extern System.Int32 SetPixelFormat(IntPtr hdc, System.Int32 iPixelFormat, ref PIXELFORMATDESCRIPTOR ppfd);
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern System.Int32 SwapBuffers(IntPtr hdc);
        #endregion

        #region Functions from user32.dll
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static unsafe extern System.Int32 ReleaseDC(IntPtr hWnd, IntPtr hDC);
        #endregion

        [DllImport("kernel32.dll")]
        public static extern UInt32 GetLastError();
    }
 
}
