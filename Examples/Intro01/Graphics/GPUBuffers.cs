using OpenGL;
using System;
using System.Runtime.InteropServices;
using System.Numerics;

namespace Intro01
{
    public struct vertex
    {
        public vertex(Vector3 _position, Vector4 _color, Vector3 _normal)
        {
            position = _position;
            color = _color;
            normal = _normal;
        }
        public Vector3 position;
        public Vector4 color;
        public Vector3 normal;
    }

    public class GPU_CubeVertexBuffer
    {
        public void SetData(vertex []_vertices, int _count)
        {
            m_count = _count;

            GL.GenBuffers(1, out m_vbo);

            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbo);

            var elementSizeInByte = Marshal.SizeOf(typeof(vertex));

            int bufferSize = _count * elementSizeInByte;

            var dataHandle = GCHandle.Alloc(_vertices, GCHandleType.Pinned);
            var dataPtr = (IntPtr)(dataHandle.AddrOfPinnedObject().ToInt64() );

            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)bufferSize, dataPtr, BufferUsageHint.StaticDraw);

            dataHandle.Free();

        }

        public void Use()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbo);
        }

        public int m_count;
        uint m_vbo;

    }

    public class GPU_CubeIndexBuffer
    {
        public void SetData(UInt16 []_indices, int _count)
        {
            m_count = _count;

            GL.GenBuffers(1, out m_ibo);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_ibo);

            var elementSizeInByte = sizeof(UInt16);
            int bufferSize = _count * elementSizeInByte;

            var dataHandle = GCHandle.Alloc(_indices, GCHandleType.Pinned);
            var dataPtr = (IntPtr)(dataHandle.AddrOfPinnedObject().ToInt64());

            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)bufferSize, dataPtr, BufferUsageHint.StaticDraw);

            dataHandle.Free();

        }

        public void Use()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_ibo);
        }

        public int m_count;
        uint m_ibo;

    }


}
