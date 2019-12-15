// MIT License - Copyright (C) FxGen 
// This file is subject to the terms and conditions defined in 
// file 'LICENSE.txt', which is part of this source code package. 

using System;

namespace Voxel
{
    public struct Vector3Int
    {

        public int X;
        public int Y;
        public int Z;


        public Vector3Int(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }



        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: { return X; }
                    case 1: { return Y; }
                    case 2: { return Z; }
                    default: throw new ArgumentException("index");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: { X = value; break; }
                    case 1: { Y = value; break; }
                    case 2: { Z = value; break; }
                    default: throw new ArgumentException("index");
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector3Int) return this.Equals((Vector3Int)obj);
            else return false;
        }

        public bool Equals(Vector3Int other)
        {
            return ((this.X == other.X) && (this.Y == other.Y) && (this.Z == other.Z));
        }


    }



}
