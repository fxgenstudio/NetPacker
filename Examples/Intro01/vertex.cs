using System;
using System.Numerics;

namespace ConsoleSDL
{
	public struct vertex
	{
		public Vector3 position;

		public Vector4 color;

		public Vector3 normal;

		public vertex(Vector3 _position, Vector4 _color, Vector3 _normal)
		{
			this.position = _position;
			this.color = _color;
			this.normal = _normal;
		}
	}
}
