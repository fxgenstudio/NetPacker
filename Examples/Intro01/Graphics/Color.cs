// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Text;
using System.Diagnostics;
using System.Numerics;

namespace Graphics
{

   


    /// <summary>
    /// Describes a 32-bit packed color.
    /// </summary>
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct Color : IEquatable<Color>
    {

	// ARGB
        private uint _packedValue;


        /// <summary>
        /// Constructs an RGBA color from the XYZW unit length components of a vector.
        /// </summary>
        /// <param name="color">A <see cref="Vector4"/> representing color.</param>
        //public Color(Vector4 color)
        //{
        //    _packedValue = 0;

            

        //    R = (byte)MathHelper.Clamp(color.X * 255, Byte.MinValue, Byte.MaxValue);
        //    G = (byte)MathHelper.Clamp(color.Y * 255, Byte.MinValue, Byte.MaxValue);
        //    B = (byte)MathHelper.Clamp(color.Z * 255, Byte.MinValue, Byte.MaxValue);
        //    A = (byte)MathHelper.Clamp(color.W * 255, Byte.MinValue, Byte.MaxValue);
        //}

        ///// <summary>
        ///// Constructs an RGBA color from the XYZ unit length components of a vector. Alpha value will be opaque.
        ///// </summary>
        ///// <param name="color">A <see cref="Vector3"/> representing color.</param>
        //public Color(Vector3 color)
        //{
        //    _packedValue = 0;

        //    R = (byte)MathHelper.Clamp(color.X * 255, Byte.MinValue, Byte.MaxValue);
        //    G = (byte)MathHelper.Clamp(color.Y * 255, Byte.MinValue, Byte.MaxValue);
        //    B = (byte)MathHelper.Clamp(color.Z * 255, Byte.MinValue, Byte.MaxValue);
        //    A = 255;
        //}

        ///// <summary>
        ///// Constructs an RGBA color from a <see cref="Color"/> and an alpha value.
        ///// </summary>
        ///// <param name="color">A <see cref="Color"/> for RGB values of new <see cref="Color"/> instance.</param>
        ///// <param name="alpha">The alpha component value from 0 to 255.</param>
        //public Color(Color color, int alpha)
        //{
        //    _packedValue = 0;

        //    R = color.R;
        //    G = color.G;
        //    B = color.B;
        //    A = (byte)MathHelper.Clamp(alpha, Byte.MinValue, Byte.MaxValue);
        //}

        ///// <summary>
        ///// Constructs an RGBA color from color and alpha value.
        ///// </summary>
        ///// <param name="color">A <see cref="Color"/> for RGB values of new <see cref="Color"/> instance.</param>
        ///// <param name="alpha">Alpha component value from 0.0f to 1.0f.</param>
        //public Color(Color color, float alpha)
        //{
        //    _packedValue = 0;

        //    R = color.R;
        //    G = color.G;
        //    B = color.B;
        //    A = (byte)MathHelper.Clamp(alpha * 255, Byte.MinValue, Byte.MaxValue);
        //}

        /// <summary>
        /// Constructs an RGBA color from scalars which representing red, green and blue values. Alpha value will be opaque.
        /// </summary>
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
        //public Color(float r, float g, float b)
        //{
        //    _packedValue = 0;
			
        //    R = (byte)MathHelper.Clamp(r * 255, Byte.MinValue, Byte.MaxValue);
        //    G = (byte)MathHelper.Clamp(g * 255, Byte.MinValue, Byte.MaxValue);
        //    B = (byte)MathHelper.Clamp(b * 255, Byte.MinValue, Byte.MaxValue);
        //    A = 255;
        //}

        /// <summary>
        /// Constructs an RGBA color from scalars which representing red, green and blue values. Alpha value will be opaque.
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        public Color(int r, int g, int b)
        {
            _packedValue = 0;
            R = (byte)MathHelper.Clamp(r, Byte.MinValue, Byte.MaxValue);
            G = (byte)MathHelper.Clamp(g, Byte.MinValue, Byte.MaxValue);
            B = (byte)MathHelper.Clamp(b, Byte.MinValue, Byte.MaxValue);
            A = (byte)255;
        }

        /// <summary>
        /// Constructs an RGBA color from scalars which representing red, green, blue and alpha values.
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        /// <param name="alpha">Alpha component value from 0 to 255.</param>
        public Color(int r, int g, int b, int alpha)
        {
            _packedValue = 0;
            R = (byte)MathHelper.Clamp(r, Byte.MinValue, Byte.MaxValue);
            G = (byte)MathHelper.Clamp(g, Byte.MinValue, Byte.MaxValue);
            B = (byte)MathHelper.Clamp(b, Byte.MinValue, Byte.MaxValue);
            A = (byte)MathHelper.Clamp(alpha, Byte.MinValue, Byte.MaxValue);
        }

        /// <summary>
        /// Constructs an RGBA color from scalars which representing red, green, blue and alpha values.
        /// </summary>
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
        /// <param name="alpha">Alpha component value from 0.0f to 1.0f.</param>
        //public Color(float r, float g, float b, float alpha)
        //{
        //    _packedValue = 0;
			
        //    R = (byte)MathHelper.Clamp(r * 255, Byte.MinValue, Byte.MaxValue);
        //    G = (byte)MathHelper.Clamp(g * 255, Byte.MinValue, Byte.MaxValue);
        //    B = (byte)MathHelper.Clamp(b * 255, Byte.MinValue, Byte.MaxValue);
        //    A = (byte)MathHelper.Clamp(alpha * 255, Byte.MinValue, Byte.MaxValue);
        //}

        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
         
        public byte B
        {
            get
            {
                unchecked
                {
                    return (byte) (this._packedValue >> 16);
                }
            }
            set
            {
                this._packedValue = (this._packedValue & 0xff00ffff) | ((uint)value << 16);
            }
        }

        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
         
        public byte G
        {
            get
            {
                unchecked
                {
                    return (byte)(this._packedValue >> 8);
                }
            }
            set
            {
                this._packedValue = (this._packedValue & 0xffff00ff) | ((uint)value << 8);
            }
        }

        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
         
        public byte R
        {
            get
            {
                unchecked
                {
                    return (byte) this._packedValue;
                }
            }
            set
            {
                this._packedValue = (this._packedValue & 0xffffff00) | value;
            }
        }

        /// <summary>
        /// Gets or sets the alpha component.
        /// </summary>
         
        public byte A
        {
            get
            {
                unchecked
                {
                    return (byte)(this._packedValue >> 24);
                }
            }
            set
            {
                this._packedValue = (this._packedValue & 0x00ffffff) | ((uint)value << 24);
            }
        }
		



        /// <summary>
        /// Gets the hash code of this <see cref="Color"/>.
        /// </summary>
        /// <returns>Hash code of this <see cref="Color"/>.</returns>
        public override int GetHashCode()
        {
            return this._packedValue.GetHashCode();
        }
	
        /// <summary>
        /// Compares whether current instance is equal to specified object.
        /// </summary>
        /// <param name="obj">The <see cref="Color"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public override bool Equals(object obj)
        {
            return ((obj is Color) && this.Equals((Color)obj));
        }

   

        /// <summary>
        /// Performs linear interpolation of <see cref="Color"/>.
        /// </summary>
        /// <param name="value1">Source <see cref="Color"/>.</param>
        /// <param name="value2">Destination <see cref="Color"/>.</param>
        /// <param name="amount">Interpolation factor.</param>
        /// <returns>Interpolated <see cref="Color"/>.</returns>
        public static Color Lerp(Color value1, Color value2, Single amount)
        {
			amount = MathHelper.Clamp(amount, 0, 1);
            return new Color(   
                (int)MathHelper.Lerp(value1.R, value2.R, amount),
                (int)MathHelper.Lerp(value1.G, value2.G, amount),
                (int)MathHelper.Lerp(value1.B, value2.B, amount),
                (int)MathHelper.Lerp(value1.A, value2.A, amount) );
        }
        /// <summary>
        /// Gets a <see cref="Vector4"/> representation for this object.
        /// </summary>
        /// <returns>A <see cref="Vector4"/> representation for this object.</returns>
        public Vector4 ToVector4()
        {
            return new Vector4(R / 255.0f, G / 255.0f, B / 255.0f, A / 255.0f);
        }
	
	/// <summary>
        /// Gets or sets packed value of this <see cref="Color"/>.
        /// </summary>
        public UInt32 PackedValue
        {
            get { return _packedValue; }
            set { _packedValue = value; }
        }



        /// <summary>
        /// Returns a <see cref="String"/> representation of this <see cref="Color"/> in the format:
        /// {R:[red] G:[green] B:[blue] A:[alpha]}
        /// </summary>
        /// <returns><see cref="String"/> representation of this <see cref="Color"/>.</returns>
	//public override string ToString ()
	//{
 //       StringBuilder sb = new StringBuilder(25);
 //       sb.Append("{R:");
 //       sb.Append(R);
 //       sb.Append(" G:");
 //       sb.Append(G);
 //       sb.Append(" B:");
 //       sb.Append(B);
 //       sb.Append(" A:");
 //       sb.Append(A);
 //       sb.Append("}");
 //       return sb.ToString();
	//}
	
	
	/// <summary>
        /// Translate a non-premultipled alpha <see cref="Color"/> to a <see cref="Color"/> that contains premultiplied alpha.
        /// </summary>
        /// <param name="r">Red component value.</param>
        /// <param name="g">Green component value.</param>
        /// <param name="b">Blue component value.</param>
        /// <param name="a">Alpha component value.</param>
        /// <returns>A <see cref="Color"/> which contains premultiplied alpha data.</returns>
        public static Color FromNonPremultiplied(int r, int g, int b, int a)
        {
            return new Color((byte)(r * a / 255),(byte)(g * a / 255), (byte)(b * a / 255), a);
        }

        #region IEquatable<Color> Members
	
	/// <summary>
        /// Compares whether current instance is equal to specified <see cref="Color"/>.
        /// </summary>
        /// <param name="other">The <see cref="Color"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public bool Equals(Color other)
        {
	    return this.PackedValue == other.PackedValue;
        }

        #endregion
    }
}
