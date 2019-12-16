using Voxel;
using OpenGL;
using System;
using System.Numerics;

namespace Intro01
{

    class Program
    {

        static int _isExiting;
        static private IntPtr _winhandle;
        static private IntPtr _context;
        //static CubeBrush brush;

        static CubeMetaBall metaball;

        static Shader shader;
        private static float rotationAngle;

        static void Main(string[] args)
        {

            //Get SDL
            Sdl.Version sversion;
            Sdl.GetVersion(out sversion);

    

            var version = 100 * sversion.Major + 10 * sversion.Minor + sversion.Patch;

            if (version < 205)
                Console.WriteLine("Please use SDL 2.0.5 or higher.");

            Console.WriteLine("SDL version {0}.{1}.{2}.\n", sversion.Major, sversion.Minor, sversion.Patch);

            // Needed so VS can debug the project on Windows
            //if (version >= 205 && CurrentPlatform.OS == OS.Windows && Debugger.IsAttached)
            Sdl.SetHint("SDL_WINDOWS_DISABLE_THREAD_NAMING", "1");


            //Init SDL
            Sdl.Init((int)(Sdl.InitFlags.Video |
                Sdl.InitFlags.Joystick |
                Sdl.InitFlags.GameController |
                Sdl.InitFlags.Haptic));

            Sdl.DisableScreenSaver();


            Sdl.GL.SetAttribute(Sdl.GL.Attribute.RedSize, 8);
            Sdl.GL.SetAttribute(Sdl.GL.Attribute.GreenSize, 8);
            Sdl.GL.SetAttribute(Sdl.GL.Attribute.BlueSize, 8);
            Sdl.GL.SetAttribute(Sdl.GL.Attribute.AlphaSize, 8);

            Sdl.GL.SetAttribute(Sdl.GL.Attribute.DoubleBuffer, 1);

            //GLES
            //Sdl.GL.SetAttribute(Sdl.GL.Attribute.ContextProfileMAsl, 0x0004);//SDL_GL_CONTEXT_PROFILE_ES
            //Sdl.GL.SetAttribute(Sdl.GL.Attribute.ContextMajorVersion, 2);
            //Sdl.GL.SetAttribute(Sdl.GL.Attribute.ContextMinorVersion, 0);


            //Create Window
            var initflags =
                           Sdl.Window.State.OpenGL |
                           Sdl.Window.State.InputFocus |
                           Sdl.Window.State.MouseFocus;

            _winhandle = Sdl.Window.Create("Intro01", 128, 128, 800, 600, initflags);

            Sdl.SetHint("SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS", "0");
            Sdl.SetHint("SDL_JOYSTICK_ALLOW_BACKGROUND_EVENTS", "1");


            //Create OpenGLES window
            _context = Sdl.GL.CreateContext(_winhandle);
            Sdl.GL.MakeCurrent(_winhandle, _context);


            if (Sdl.GL.SetSwapInterval(1) < 0)
            {
                Console.WriteLine("Warning: Unable to set VSync! SDL Error: {0}\n", Sdl.GetError());
            }


            //Load OpenGL fonctions
            GL.LoadEntryPoints();


            //Trace Infos

            Console.WriteLine("Vendor: {0}", GL.GetString(StringName.Vendor));
            Console.WriteLine("Renderer: {0}", GL.GetString(StringName.Renderer));
            Console.WriteLine("GL version: {0}", GL.GetString(StringName.Version));
            //Console.WriteLine("Extensions: {0}", GL.GetString(StringName.Extensions));
            int bits = IntPtr.Size * 8;
            Console.WriteLine("Process: {0}-bit", bits);

            //Vector
            //Console.WriteLine("Hardware Vector: {0}", System.Numerics.Vector.IsHardwareAccelerated);

            //Load Shader
            shader = new Shader();
            shader.Load();


            //Load Brush

            //MagicaVoxelLoader loader = new MagicaVoxelLoader();

            //var fs = File.OpenRead(@"monu1.vox");
            //BinaryReader br = new BinaryReader(fs);
            //if (loader.ReadFile(br))
            //{
            //    brush = new CubeBrush(loader.Array, loader.Palette);

            //    brush.GenerateMesh();
            //}

            //Create metaball
            metaball = new CubeMetaBall();
            metaball.SetGridSize(64);

            //Loop

            GC.Collect();

            RunLoop();

            //Exit
            Sdl.GL.DeleteContext(_context);

            Sdl.Quit();
        }


        static void RunLoop()
        {
            int screenWidth = 800;
            int screenHeight = 600;

            // create the matrices
            Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(60.0f * 0.0174533f, screenWidth / (float)screenHeight, 0.5f, 1000.0f);
            Matrix4x4 view = Matrix4x4.CreateLookAt(new Vector3(0, 0, 150.0f), new Vector3(0, 0, 0), new Vector3(0, 1.0f, 0));

            int totalTrace = 0;
            int needTrace = 60;
            while (true)
            {
                SdlRunLoop();

                GL.DepthFunc(DepthFunction.Less);
                GL.Enable(EnableCap.DepthTest);
                GL.DepthMask(true);

                GL.CullFace(CullFaceMode.Front);
                GL.Enable(EnableCap.CullFace);

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.ClearColor(0.05f, 0.05f, 0.1f, 1.0f);


                Matrix4x4 model = Matrix4x4.CreateRotationY(rotationAngle);
                rotationAngle += 0.01f;


                //if (brush!=null)
                //    brush.Draw(shader, ref view, ref projection, ref model);

                if (metaball != null)
                {
                    int t0 = Environment.TickCount;

                    metaball.Update(0.1f);
                    int t1 = Environment.TickCount;

                    totalTrace += t1 - t0;
                }

                if (needTrace-- < 0)
                {
                    Console.WriteLine("speed {0}", totalTrace / 60);
                    needTrace = 60;
                    totalTrace = 0;
                }

                if (metaball != null)
                    metaball.Draw(shader, ref view, ref projection, ref model);

                Sdl.GL.SwapWindow(_winhandle);

                if (_isExiting > 0)
                    break;
            }


        }

        static void SdlRunLoop()
        {
            Sdl.Event ev;

            while (Sdl.PollEvent(out ev) == 1)
            {
                if (ev.Type == Sdl.EventType.Quit)
                    _isExiting++;
                //else if (ev.Type == Sdl.EventType.JoyDeviceAdded)
                //    Joystick.AddDevice(ev.JoystickDevice.Which);
                //else if (ev.Type == Sdl.EventType.ControllerDeviceRemoved)
                //    GamePad.RemoveDevice(ev.ControllerDevice.Which);
                //else if (ev.Type == Sdl.EventType.JoyDeviceRemoved)
                //    Joystick.RemoveDevice(ev.JoystickDevice.Which);
                //else if (ev.Type == Sdl.EventType.MouseWheel)
                //    Mouse.ScrollY += ev.Wheel.Y * 120;
                //else if (ev.Type == Sdl.EventType.KeyDown)
                //{
                //    var key = KeyboardUtil.ToXna(ev.Key.Keysym.Sym);
                //    if (!_keys.Contains(key))
                //        _keys.Add(key);
                //    char character = (char)ev.Key.Keysym.Sym;
                //    if (char.IsControl(character))
                //        _view.CallTextInput(character, key);
                //}
                //else if (ev.Type == Sdl.EventType.KeyUp)
                //{
                //    var key = KeyboardUtil.ToXna(ev.Key.Keysym.Sym);
                //    _keys.Remove(key);
                //}
                //else if (ev.Type == Sdl.EventType.TextInput)
                //{
                //    int len = 0;
                //    string text = String.Empty;
                //    unsafe
                //    {
                //        while (Marshal.ReadByte((IntPtr)ev.Text.Text, len) != 0)
                //        {
                //            len++;
                //        }
                //        var buffer = new byte[len];
                //        Marshal.Copy((IntPtr)ev.Text.Text, buffer, 0, len);
                //        text = System.Text.Encoding.UTF8.GetString(buffer);
                //    }
                //    if (text.Length == 0)
                //        continue;
                //    foreach (var c in text)
                //    {
                //        var key = KeyboardUtil.ToXna((int)c);
                //        _view.CallTextInput(c, key);
                //    }
                //}
                //else if (ev.Type == Sdl.EventType.WindowEvent)
                //{
                //    if (ev.Window.EventID == Sdl.Window.EventId.Resized || ev.Window.EventID == Sdl.Window.EventId.SizeChanged)
                //        _view.ClientResize(ev.Window.Data1, ev.Window.Data2);
                //    else if (ev.Window.EventID == Sdl.Window.EventId.FocusGained)
                //        IsActive = true;
                //    else if (ev.Window.EventID == Sdl.Window.EventId.FocusLost)
                //        IsActive = false;
                //}
            }
        }


    }
}
