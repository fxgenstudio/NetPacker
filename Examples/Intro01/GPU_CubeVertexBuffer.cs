using OpenGL;
using System;
using System.Runtime.InteropServices;

namespace ConsoleSDL
{
	public class GPU_CubeVertexBuffer
	{
		public int m_count;

		private uint m_vbo;

		public void SetData(vertex[] _vertices, int _count)
		{
			this.m_count = _count;
			GL.GenBuffers(1, out this.m_vbo);
			GL.BindBuffer(BufferTarget.ArrayBuffer, this.m_vbo);
			int num = Marshal.SizeOf(typeof(vertex));
			int value = _count * num;
			GCHandle gCHandle = GCHandle.Alloc(_vertices, GCHandleType.Pinned);
			IntPtr n = (IntPtr)gCHandle.AddrOfPinnedObject().ToInt64();
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)value, n, BufferUsageHint.StaticDraw);
			gCHandle.Free();
		}

		public void Use()
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, this.m_vbo);
		}
	}
}
