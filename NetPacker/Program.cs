using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Resources;
using System.Text;

namespace NetPacker
{

    class Program
    {
        private static string appToBeCompressed;
        private static string outputFile;
        private static List<string> dllInstructions = new List<string>();
        private static string possibleMessage = "", possibleAssembly = "";

        private static ResourceManager manager = new ResourceManager("NetPacker.Properties.Resources", Assembly.GetExecutingAssembly());


        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            int i = 0;

            Console.WriteLine("NetPacker v0.1");

            //Test ILOptimizer only
            //ILOptimizer op = new ILOptimizer();
            //op.Pack(@"D:\temp\my\CSharp\ConsoleSDL\bin\Release\ConsoleSDL.exe");
            //return;

            //flag variables.
            string iconSelected = "";

            bool displayCode = false; //display the code?
            bool ripIconMode = false; //rip the icon from the source executable?
            bool export = false; //export an empty assembly file? (or the sevensharpzip file).

            if (args.Length < 2)
            {
                //information.
                Console.WriteLine("[Input Exe | -e] [Output File] (-w) (-d) (-rip) (dlls) ... ");
                Console.WriteLine("-e (Exports a file with all the default compilation tags for you to modify. You could also use this option to export the SevenSharpZip.dll file)");
                Console.WriteLine("-w (Specifies to not launch a console at the start of the application. Windows Mode)");
                Console.WriteLine("-d (Displays the code generated.)");
                Console.WriteLine("-rip (attempts to rip the icon from the exe it is compressing.");

                Environment.Exit(0);
            }

            //compiler parameters.
            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();


            foreach (String word in args)
            {
                if (i == 0)
                {
                    if (word.Trim() == "-e") //export mode flag
                    {
                        export = true;
                    }
                    else
                        appToBeCompressed = word; //sets the input executable.
                }
                else if (i == 1)
                {
                    outputFile = word;  //sets the output executable.

                    //if the extension is a dll, then export the SevenZipSharp file.
                    //if (export && word.EndsWith(".dll"))
                    //{
                    //    File.WriteAllBytes(word, (byte[])manager.GetObject(EXTERNAL_COMPRESSOR));
                    //    return;
                    //}
                    //otherwise export the blank assembly file.
                }
                else
                {

                    //goes through all the flag data.
                    //read about the flags above.
                    if (export)
                    {
                        //nothing.
                    }
                    else
                    if (word.Trim() == "-rip") // command flag to rip icon from previous exe (to the best of its ability).
                    {
                        ripIconMode = true;
                    }
                    else
                    if (word.Trim() == "-w") //command flag to compile as a window executable, and not as a console executable.
                    {
                        parameters.CompilerOptions = "/target:winexe";
                    }
                    else
                    if (word.Trim() == "-d") // command flag to display the generated code. 
                    {
                        displayCode = true;
                    }
                    else // otherwise, just assume it is a dll file.
                    {
                        dllInstructions.Add("" + word.Trim() + "");
                    }

                }

                i++;
            }

            //if the export flag is toggled on, then print out the blank C# tag assembly file. 
            if (export)
            {
                File.WriteAllText(outputFile, manager.GetString("Assembly"));
                return;
            }


            //Provide a compiler version (this might be refractored out later on).
            var providerOptions = new Dictionary<string, string>();
            providerOptions.Add("CompilerVersion", "v4.0");


            //create a code compiler.
            using (CSharpCodeProvider codeProvider = new CSharpCodeProvider(providerOptions))
            {
                //compiler
                ICodeCompiler icc = codeProvider.CreateCompiler();

                //tell it to make an exe.
                parameters.GenerateExecutable = true;
                parameters.CompilerOptions = "-platform:x86";

                //if it supports resource files on this platform, go through with the generation.
                if (codeProvider.Supports(GeneratorSupport.Resources))
                {
                    parameters.EmbeddedResources.Add("resource.resources");
                    string source = GenerateSource(); //generate the code and embed dlls.

                    //if the displayCode flag is on, print out the source code generated.
                    if (displayCode)
                    {
                        Console.WriteLine(source);
                    }

                    //add the system dll to referenced assemblies.
                    parameters.ReferencedAssemblies.Add("System.dll");

                    //set it to export the assembly.
                    parameters.OutputAssembly = outputFile;



                    //check icon flags.
                    if (iconSelected != "" && File.Exists(iconSelected))
                    {
                        parameters.CompilerOptions += " /win32icon:" + iconSelected;
                    }
                    else
                    if (ripIconMode)
                    {
                        //weird hack to get the icon to extract from original exe.
                        Icon iconFromExe = Icon.ExtractAssociatedIcon(appToBeCompressed);
                        if (iconFromExe != null)
                        {
                            //convert to bitmap.
                            Bitmap iconBitmapFromExe = iconFromExe.ToBitmap();

                            //use a custom method to save as icon. C#'s built in method is not very good.
                            SaveAsIcon(new Bitmap(iconBitmapFromExe, new Size((int)(iconBitmapFromExe.Size.Width * 1.0f), (int)(iconBitmapFromExe.Size.Height * 1.0f))), appToBeCompressed + "icon.ico");

                            //set it to use that icon.
                            parameters.CompilerOptions += " /win32icon:" + Directory.GetCurrentDirectory() + "\\" + appToBeCompressed + "icon.ico";
                        }
                    }

                    //try compiling, print out errors if it can't.
                    CompilerResults results = icc.CompileAssemblyFromSource(parameters, source);

                    //if it compiled, move it to be a temp file, it might need more repackaging. 
                    //if (File.Exists(outputFile))
                    //{
                    //    if (File.Exists(outputFile + "_temp")) File.Delete(outputFile + "_temp");
                    //    File.Move(outputFile, outputFile + "_temp");
                    //}


                    //Delete some of the temporary compilation files.
                    File.Delete(outputFile + "temp_c.dat");
                    File.Delete("resource.resources");
                    File.Delete(outputFile + "_temp");

                    //it only needs to delete this if the icon was ripped in the first place.
                    if (ripIconMode)
                        File.Delete(appToBeCompressed + "icon.ico");


                    //print out any errors in the compilation process.
                    if (results.Errors.Count > 0)
                    {
                        //loops thorugh all the issues.
                        foreach (CompilerError CompErr in results.Errors)
                        {

                            Console.WriteLine("Line number " + CompErr.Line +
                            ", Error Number: " + CompErr.ErrorNumber +
                            ", '" + CompErr.ErrorText + ";" +
                            Environment.NewLine + Environment.NewLine);
                        }
                    }


                }
                else
                {
                    //stop here if it doesn't support resources.
                }

            }

        }



        /// <summary>
        /// Compresses the Application and writes it into the Resource file.
        /// Also generates some code related to the Application launching.
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        private static string CompressApplication(ResourceWriter writer)
        {
            //this is what the application is called in the resource file.
            const string APPLICATION_NAME = "app";

            //ILOptimiser
            ILOptimizer ilopt = new ILOptimizer();
            ilopt.Pack(outputFile + "_temp", outputFile + "temp_ilopt.dat");


            //generates a temporary file to compress into.
            FileStream stream = File.Open(outputFile + "temp_c.dat", FileMode.Create);

            //opens the input file (which has been merged with some other dlls).
            FileStream stream2 = File.OpenRead(outputFile + "temp_ilopt.dat");

            //sets the mode to follow when it generates the code. It swaps between G-Zip and LZMA.
            string mode;
            long appSize = stream2.Length;
            Stream gStream = null;

            //Console.WriteLine($"NetPacker: Original ExeToPack Size: {appSize}");

            //gzip
            gStream = new GZipStream(stream, CompressionLevel.Optimal);
            mode = "GZipStream(memStr, CompressionMode.Decompress)";

            //compresses, not the most efficient way to compress, I should buffer it, but it doesn't matter.
            while (stream2.Position < stream2.Length)
            {
                gStream.WriteByte((byte)stream2.ReadByte());
            }


            //closes each of the streams.
            gStream.Close();
            stream.Close(); //closing this one manually because some compression libraries don't have the wrapper close the stream passed into it.
            stream2.Close();

            //Display
            long lengthOriginal = new System.IO.FileInfo(outputFile + "_temp").Length;
            long lengthCompressed = new System.IO.FileInfo(outputFile + "temp_c.dat").Length;
            Console.WriteLine($"NetPacker: GZip Compressed Size: {lengthCompressed}  Percent: {100-(lengthCompressed*100/ lengthOriginal)}");


            //add the resource to the file.
            writer.AddResource(APPLICATION_NAME, File.ReadAllBytes(outputFile + "temp_c.dat"));

            //add the code.
            string code = manager.GetString("AppMethod").Replace("%appname%", APPLICATION_NAME).Replace("%appsize%", "" + appSize).Replace("%mode%", mode);

            return code;
        }

        /// <summary>
        /// Merges dlls into one file prior to compression.
        /// </summary>
        private static void GetDLLs()
        {
            //adds the output file to the dll instructions.
            dllInstructions.Insert(0, "/out:" + outputFile + "_temp");

            //adds the input file to the dll instructions.
            dllInstructions.Insert(1, appToBeCompressed);


            if (dllInstructions.Count != 2) //if there have been instructions added prior, that means dlls have been passed in, repack it.
            {
                new ILRepacking.ILRepack(new ILRepacking.RepackOptions(new ILRepacking.CommandLine(dllInstructions))).Repack();
            }
            else //repacking isn't necessary if there are no dlls. just rename the file.
            {
                if (File.Exists(outputFile + "_temp")) File.Delete(outputFile + "_temp");
                File.Copy(appToBeCompressed, outputFile + "_temp");
            }


        }

        /// <summary>
        /// Gets the ending stub of the source code generated.
        /// </summary>
        /// <returns></returns>
        private static string GetEnd()
        {
            return manager.GetString("AppMethodEnd");
        }

        /// <summary>
        /// Gets any messages that may have been passed in.
        /// </summary>
        /// <returns></returns>
        private static string GetMessage()
        {
            string result = "";
            //if there is a message, and it exists, add it into the code.
            if (possibleMessage != "")
            {
                if (File.Exists(possibleMessage))
                {
                    //I am lazy, I didn't want to do a bunch of work to get text onto a line, so I base-64 encoded it.
                    string text = File.ReadAllText(possibleMessage);
                    text = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
                    result += "String msg = \"" + text + "\";\n";
                    result += "msg = Encoding.UTF8.GetString(Convert.FromBase64String(msg));\n";
                    result += "Console.WriteLine(msg);\n";
                }
            }

            return result;
        }


        /// <summary>
        /// Gets the code in the main method.
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        private static string GetMainMethodCode(ResourceWriter writer)
        {
            string result = "";


            GetDLLs();
            result += GetMessage() + "\n";
            result += CompressApplication(writer) + "\n";
            result += GetEnd() + "\n";



            return result;
        }



        /// <summary>
        /// Goes through all the auxillary methods to generate the source code to load the application after it is compressed.
        /// </summary>
        /// <returns></returns>
        private static string GenerateSource()
        {
            string result = "";

            //usings.
            result += "using System;\n";
            result += "using System.Reflection;\n";
            result += "using System.Resources;\n";
            result += "using System.Text;\n";
            result += "using System.IO;\n";
            result += "using System.IO.Compression;\n";

            result += GetAssemblyInfo() + "\n";

            //create a resource writer, so we can embed the compressed materials.
            ResourceWriter writer = new ResourceWriter(File.Open("resource.resources", FileMode.Create));

            //add the basic structure.
            result += "namespace CompressedApp\n{\nclass Program\n{\n[STAThread]\nstatic void Main(string[] args)\n{\n" + GetMainMethodCode(writer) + "\n}\n}\n}\n";


            //close the resource writer.
            writer.Close();


            return result;
        }

        /// <summary>
        /// Get the assembly information.
        /// This is all the stuff like "Trademark year"
        /// "Product Name"
        /// "Company"
        /// </summary>
        /// <returns></returns>
        private static string GetAssemblyInfo()
        {
            string result = "";

            if (possibleAssembly != "" && File.Exists(possibleAssembly))
                result += File.ReadAllText(possibleAssembly);

            return result;
        }


        /// <summary>
        /// Used to save an extracted icon from a file.
        /// This isn't absolutely necessary for this application, but I think it is useful.
        /// </summary>
        /// <param name="SourceBitmap"></param>
        /// <param name="FilePath"></param>
        private static void SaveAsIcon(Bitmap SourceBitmap, string FilePath)
        {
            FileStream FS = new FileStream(FilePath, FileMode.Create);
            // ICO header
            FS.WriteByte(0); FS.WriteByte(0);
            FS.WriteByte(1); FS.WriteByte(0);
            FS.WriteByte(1); FS.WriteByte(0);

            // Image size
            FS.WriteByte((byte)SourceBitmap.Width);
            FS.WriteByte((byte)SourceBitmap.Height);
            // Palette
            FS.WriteByte(0);
            // Reserved
            FS.WriteByte(0);
            // Number of color planes
            FS.WriteByte(0); FS.WriteByte(0);
            // Bits per pixel
            FS.WriteByte(32); FS.WriteByte(0);

            // Data size, will be written after the data
            FS.WriteByte(0);
            FS.WriteByte(0);
            FS.WriteByte(0);
            FS.WriteByte(0);

            // Offset to image data, fixed at 22
            FS.WriteByte(22);
            FS.WriteByte(0);
            FS.WriteByte(0);
            FS.WriteByte(0);

            // Writing actual data
            SourceBitmap.Save(FS, ImageFormat.Png);

            // Getting data length (file length minus header)
            long Len = FS.Length - 22;

            // Write it in the correct place
            FS.Seek(14, SeekOrigin.Begin);
            FS.WriteByte((byte)Len);
            FS.WriteByte((byte)(Len >> 8));

            FS.Close();
        }




     
    }
}
