using Graphics;
using OpenGL;
using System;
using System.Runtime.InteropServices;
using System.Numerics;

namespace Intro01
{
    public class Shader
    {


        const string vertexShaderSource = @"
        attribute vec3 LVertexPos;
		attribute vec4 LVertexColor;
		attribute vec3 LVertexNormal;

		uniform mat4 projectionMatrix;
		uniform mat4 viewMatrix;
		uniform mat4 modelMatrix;
		varying vec4 texShadeColor;

		void main()
		{
		  gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(LVertexPos, 1.0);
		  texShadeColor =  LVertexColor;
		}";


        const string fragmentShaderSource = @"
        precision mediump float;
		varying vec4 texShadeColor;

		void main()
		{
			gl_FragColor = texShadeColor;
		}";


        int m_shaderProgramID;
        int m_vertexShaderID;
        int m_fragmentShaderID;

        public int m_vertexPosAttr;
        public int m_vertexColorAttr;
        public int m_vertexNormalAttr;

        int m_projMtxParam;
        int m_viewMtxParam;
        int m_modelMtxParam;

        public bool Load()
        {
            m_shaderProgramID = GL.CreateProgram();

            //Create Vertex Shader
            m_vertexShaderID = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(m_vertexShaderID, vertexShaderSource);
            GL.CompileShader(m_vertexShaderID);

            // Check vertex shader for errors
            int vShaderCompiled = 0;
            GL.GetShader(m_vertexShaderID, ShaderParameter.CompileStatus, out vShaderCompiled);

            if (vShaderCompiled == 0)
            {
                if (GL.IsShader(m_vertexShaderID))
                {
                    var str = GL.GetShaderInfoLog(m_vertexShaderID);
                    Console.WriteLine(str);
                }
                return false;
            }

            // Attach vertex shader to program
            GL.AttachShader(m_shaderProgramID, m_vertexShaderID);


            //Create fragment shader


            // Create fragment shader
            m_fragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);

            // Set fragment source
            GL.ShaderSource(m_fragmentShaderID, fragmentShaderSource);
            // Compile fragment source
            GL.CompileShader(m_fragmentShaderID);

            // Check fragment shader for errors
            int fShaderCompiled = 0;
            GL.GetShader(m_fragmentShaderID, ShaderParameter.CompileStatus, out fShaderCompiled);

            if (fShaderCompiled == 0)
            {
                if (GL.IsShader(m_fragmentShaderID))
                {
                    var str = GL.GetShaderInfoLog(m_fragmentShaderID);
                    Console.WriteLine(str);
                }
                return false;
            }

            //Attach fragment shader to program
            GL.AttachShader(m_shaderProgramID, m_fragmentShaderID);

            // Assign indexes
            //GL.BindAttribLocation(m_shaderProgramID, 0, "LVertexPos");
            //GL.BindAttribLocation(m_shaderProgramID, 1, "LVertexColor");
            //GL.BindAttribLocation(m_shaderProgramID, 2, "LTexCoord");

            //Link program
            GL.LinkProgram(m_shaderProgramID);

            int programSuccess = 0;
            GL.GetProgram(m_shaderProgramID, GetProgramParameterName.LinkStatus, out programSuccess);

            if (programSuccess == 0)
            {
                Console.WriteLine("Error linking program {0}", m_shaderProgramID);

                if (GL.IsProgram(m_shaderProgramID))
                {
                    var str = GL.GetProgramInfoLog(m_shaderProgramID);
                    Console.WriteLine(str);
                }

                return false;
            }


            ////////

            /////////////////////////////////////
            // Set Vertex Attributs

            // Get attribute locations

            m_vertexPosAttr = GL.GetAttribLocation(m_shaderProgramID, "LVertexPos");
            m_vertexColorAttr = GL.GetAttribLocation(m_shaderProgramID, "LVertexColor");
            m_vertexNormalAttr = GL.GetAttribLocation(m_shaderProgramID, "LVertexNormal");

            // Check for errors
            if (m_vertexPosAttr == -1)
            {
                Console.WriteLine("LVertexPos is not a valid glsl program variable!\n");
            }
            if (m_vertexColorAttr == -1)
            {
                Console.WriteLine("LVertexColor is not a valid glsl program variable!\n");
            }

            if (m_vertexNormalAttr == -1)
            {
                Console.WriteLine("LVertexNormal is not a valid glsl program variable!\n");
            }

            /////////////////////////////////////
            // Get parameters ID
            m_projMtxParam = GL.GetUniformLocation(m_shaderProgramID, "projectionMatrix"); // Get location of the projection matrix in the shader
            m_viewMtxParam = GL.GetUniformLocation(m_shaderProgramID, "viewMatrix"); // Get location of the view matrix in the shader
            m_modelMtxParam = GL.GetUniformLocation(m_shaderProgramID, "modelMatrix"); // Get location of the model matrix in the shader



            return true;
        }

        public void Use()
        {
            GL.UseProgram(m_shaderProgramID);

            // Set attribute data pointers
            var elementSizeInByte = Marshal.SizeOf(typeof(vertex));

            GL.VertexAttribPointer(m_vertexPosAttr, 3, VertexAttribPointerType.Float, false, elementSizeInByte, (IntPtr)0);
            GL.VertexAttribPointer(m_vertexColorAttr, 4, VertexAttribPointerType.Float, false, elementSizeInByte, (IntPtr)12);
            GL.VertexAttribPointer(m_vertexNormalAttr, 3, VertexAttribPointerType.Float, false, elementSizeInByte, (IntPtr)28);


            GL.EnableVertexAttribArray(m_vertexPosAttr);
            GL.EnableVertexAttribArray(m_vertexColorAttr);
            GL.EnableVertexAttribArray(m_vertexNormalAttr);

        }

     

        public void UnUse()
        {
            GL.DisableVertexAttribArray(m_vertexPosAttr);
            GL.DisableVertexAttribArray(m_vertexColorAttr);
            GL.DisableVertexAttribArray(m_vertexNormalAttr);
            
        }


        public void SetMatrix(ref Matrix4x4  _proj, ref Matrix4x4  _view, ref Matrix4x4  _model)
        {

            //Set parameters
            UniformMatrix4(m_projMtxParam, false, ref _proj); // Send projection matrix to the shader
            UniformMatrix4(m_viewMtxParam, false, ref _view); // Send projection matrix to the shader
            UniformMatrix4(m_modelMtxParam, false, ref _model); // Send projection matrix to the shader

        }

        public void SetWorld(ref Matrix4x4 _model)
        {

            //Set parameters
            UniformMatrix4(m_modelMtxParam, false, ref _model); // Send projection matrix to the shader

        }

        public static void UniformMatrix4(int location, bool transpose, ref Matrix4x4 matrix)
        {
            unsafe
            {
                fixed (float* matrix_ptr = &matrix.M11)
                {
                    GL.UniformMatrix4fv(location, 1, transpose, matrix_ptr);
                }
            }
        }

    }

}
