// MIT License - Copyright (C) FxGen 
// This file is subject to the terms and conditions defined in 
// file 'LICENSE.txt', which is part of this source code package. 

using System;
using System.Numerics;
using Intro01;

namespace Voxel
{

    public struct SBall
    {
        public System.Numerics.Vector3 p;
        public System.Numerics.Vector3 v;
        public System.Numerics.Vector3 a;
        public float t;
        public float m; //Mass
    }


    public class CubeMetaBall
    {
        Random m_rnd = new Random();

        SBall[] m_listBalls = new SBall[BALLS_COUNT];

        protected int m_nGridSize;
        protected float m_fLevel;
        protected float m_fVoxelSize;

        protected float[] m_pfGridEnergy;

        protected CubeBrush m_pbrush;

        const int BALLS_COUNT = 4;

        public CubeMetaBall()
        {
            m_fLevel = 100.0f;
            m_pfGridEnergy = null;
            //m_pnGridPointStatus = nullptr;
            //m_pnGridVoxelStatus = nullptr;
            m_pbrush = null;


            //Create balls

            for (int i = 0; i < BALLS_COUNT; i++)
            {
                SBall ball = new SBall();

                ball.a.X = ((float)m_rnd.NextDouble() * 50 - 1) / 2;
                ball.a.Y = ((float)m_rnd.NextDouble() * 4 - 1) / 2;
                ball.a.Z = ((float)m_rnd.NextDouble() * 3 - 1) / 2;
                ball.t = (float)m_rnd.NextDouble();
                ball.m = 2;

                m_listBalls[i] = ball;
            }

            //


        }
        public void Dispose()
        {
            if (m_pfGridEnergy != null)
            {
                m_pfGridEnergy = null;
            }

            //if (m_pnGridPointStatus != nullptr)
            //	delete[]m_pnGridPointStatus;

            //if (m_pnGridVoxelStatus != nullptr)
            //	delete[]m_pnGridVoxelStatus;


        }

        public void SetGridSize(int nSize)
        {
            m_fVoxelSize = 2.0f / (float)nSize;
            m_nGridSize = nSize;

            m_pfGridEnergy = new float[(nSize + 1) * (nSize + 1) * (nSize + 1)];
            //m_pnGridPointStatus = new char[(nSize + 1)*(nSize + 1)*(nSize + 1)];
            //m_pnGridVoxelStatus = new char[nSize*nSize*nSize];

            m_pbrush = new CubeBrush(m_nGridSize, m_nGridSize, m_nGridSize, false);
        }

        //int CubeMetaBall::ComputeGridVoxel(int x, int y, int z)
        //{
        //	float b[8];

        //	b[0] = ComputeGridPointEnergy(x, y, z);
        //	b[1] = ComputeGridPointEnergy(x + 1, y, z);
        //	b[2] = ComputeGridPointEnergy(x + 1, y, z + 1);
        //	b[3] = ComputeGridPointEnergy(x, y, z + 1);
        //	b[4] = ComputeGridPointEnergy(x, y + 1, z);
        //	b[5] = ComputeGridPointEnergy(x + 1, y + 1, z);
        //	b[6] = ComputeGridPointEnergy(x + 1, y + 1, z + 1);
        //	b[7] = ComputeGridPointEnergy(x, y + 1, z + 1);

        //	float fx = ConvertGridPointToWorldCoordinate(x) + m_fVoxelSize / 2;
        //	float fy = ConvertGridPointToWorldCoordinate(y) + m_fVoxelSize / 2;
        //	float fz = ConvertGridPointToWorldCoordinate(z) + m_fVoxelSize / 2;

        //	int c = 0;
        //	c |= b[0] > m_fLevel ? (1 << 0) : 0;
        //	c |= b[1] > m_fLevel ? (1 << 1) : 0;
        //	c |= b[2] > m_fLevel ? (1 << 2) : 0;
        //	c |= b[3] > m_fLevel ? (1 << 3) : 0;
        //	c |= b[4] > m_fLevel ? (1 << 4) : 0;
        //	c |= b[5] > m_fLevel ? (1 << 5) : 0;
        //	c |= b[6] > m_fLevel ? (1 << 6) : 0;
        //	c |= b[7] > m_fLevel ? (1 << 7) : 0;

        //	// Compute vertices from marching pyramid case
        //	/*fx = ConvertGridPointToWorldCoordinate(x);
        //	fy = ConvertGridPointToWorldCoordinate(y);
        //	fz = ConvertGridPointToWorldCoordinate(z);

        //	int i = 0;*/
        //	//unsigned short EdgeIndices[12];
        //	//memset(EdgeIndices, 0xFF, 12 * sizeof(unsigned short));
        //	//while (1)
        //	//{
        //	//	int nEdge = CMarchingCubes::m_CubeTriangles[c][i];
        //	//	if (nEdge == -1)
        //	//		break;

        //	//	if (EdgeIndices[nEdge] == 0xFFFF)
        //	//	{
        //	//		EdgeIndices[nEdge] = m_nNumVertices;

        //	//		// Optimization: It's possible that the non-interior edges
        //	//		// have been computed already in neighbouring voxels

        //	//		// Compute the vertex by interpolating between the two points
        //	//		int nIndex0 = CMarchingCubes::m_CubeEdges[nEdge][0];
        //	//		int nIndex1 = CMarchingCubes::m_CubeEdges[nEdge][1];

        //	//		float t = (m_fLevel - b[nIndex0]) / (b[nIndex1] - b[nIndex0]);

        //	//		m_pVertices[m_nNumVertices].v[0] = CMarchingCubes::m_CubeVertices[nIndex0][0] * (1 - t) + CMarchingCubes::m_CubeVertices[nIndex1][0] * t;
        //	//		m_pVertices[m_nNumVertices].v[1] = CMarchingCubes::m_CubeVertices[nIndex0][1] * (1 - t) + CMarchingCubes::m_CubeVertices[nIndex1][1] * t;
        //	//		m_pVertices[m_nNumVertices].v[2] = CMarchingCubes::m_CubeVertices[nIndex0][2] * (1 - t) + CMarchingCubes::m_CubeVertices[nIndex1][2] * t;

        //	//		m_pVertices[m_nNumVertices].v[0] = fx + m_pVertices[m_nNumVertices].v[0] * m_fVoxelSize;
        //	//		m_pVertices[m_nNumVertices].v[1] = fy + m_pVertices[m_nNumVertices].v[1] * m_fVoxelSize;
        //	//		m_pVertices[m_nNumVertices].v[2] = fz + m_pVertices[m_nNumVertices].v[2] * m_fVoxelSize;

        //	//		// Compute the normal at the vertex
        //	//		ComputeNormal(&m_pVertices[m_nNumVertices]);

        //	//		m_nNumVertices++;
        //	//	}

        //	//	// Add the edge's vertex index to the index list
        //	//	m_pIndices[m_nNumIndices] = EdgeIndices[nEdge];

        //	//	m_nNumIndices++;

        //	//	i++;
        //	//}

        //	SetGridVoxelComputed(x, y, z);

        //	//if (m_nNumIndices >= MAX_INDICES - 30)
        //	//{
        //	//	// Render the computed triangles
        //	//	glDrawElements(GL_TRIANGLES, m_nNumIndices, GL_UNSIGNED_SHORT, m_pIndices);

        //	//	m_nNumVertices = 0;
        //	//	m_nNumIndices = 0;
        //	//}

        //	return c;
        //}

        //void CubeMetaBall::SetGridVoxelComputed(int x, int y, int z)
        //{
        //	m_pnGridVoxelStatus[x +
        //		y*m_nGridSize +
        //		z*m_nGridSize*m_nGridSize] = 1;
        //}

        public void Update(float _dt)
        {
            int i;

            // Update metaballs positions
            for (i = 0; i < BALLS_COUNT; i++)
            {
                ref SBall ball = ref m_listBalls[i];


                ball.p += _dt * ball.v;
                //ball.p.Y += _dt * ball.v.Y;
                //ball.p.Z += _dt * ball.v.Z;

                ball.t -= _dt;
                if (ball.t < 0)
                {
                    // When is the next time to act?
                    ball.t = (float)m_rnd.NextDouble();

                    // Use a new attraction point
                    ball.a.X = ((float)m_rnd.NextDouble() * 2 - 1) / 2;
                    ball.a.Y = ((float)m_rnd.NextDouble() * 2 - 1) / 2;
                    ball.a.Z = ((float)m_rnd.NextDouble() * 2 - 1) / 2;
                }

                // Accelerate towards the attraction point
                //  float x = ball.a.X - ball.p.X;
                //  float y = ball.a.Y - ball.p.Y;
                //  float z = ball.a.Z - ball.p.Z;

                // float fDist = 1.0f / (float)System.Math.Sqrt(x * x + y * y + z * z);


                // x *= fDist;
                // y *= fDist;
                // z *= fDist;

                // ball.v.X += 0.1f * x * _dt;
                // ball.v.Y += 0.1f * y * _dt;
                // ball.v.Z += 0.1f * z * _dt;


                var dd = ball.a - ball.p;
                var l = 1.0f / dd.Length();
                dd *= l;
                ball.v += 0.1f * dd * _dt;


                //float fDist =  ball.v.X * ball.v.X + ball.v.Y * ball.v.Y + ball.v.Z * ball.v.Z;
                var fDist = ball.v.LengthSquared();


                if (fDist > 0.040f)
                {
                    fDist = 1.0f / (float)System.Math.Sqrt(fDist);
                    // ball.v.X = 0.20f * ball.v.X * fDist;
                    // ball.v.X = 0.20f * ball.v.Y * fDist;
                    // ball.v.Z = 0.20f * ball.v.Z * fDist;

                    ball.v = 0.20f * ball.v * fDist;
                }

                if (ball.p.X < -1 + m_fVoxelSize)
                {
                    ball.p.X = -1 + m_fVoxelSize;
                    ball.v.X = 0F;
                }
                if (ball.p.X > 1 - m_fVoxelSize)
                {
                    ball.p.X = 1 - m_fVoxelSize;
                    ball.v.X = 0F;
                }
                if (ball.p.Y < -1 + m_fVoxelSize)
                {
                    ball.p.Y = -1 + m_fVoxelSize;
                    ball.v.Y = 0F;
                }
                if (ball.p.Y > 1 - m_fVoxelSize)
                {
                    ball.p.Y = 1 - m_fVoxelSize;
                    ball.v.Y = 0F;
                }
                if (ball.p.Z < -1 + m_fVoxelSize)
                {
                    ball.p.Z = -1 + m_fVoxelSize;
                    ball.v.Z = 0F;
                }
                if (ball.p.Z > 1 - m_fVoxelSize)
                {
                    ball.p.Z = 1 - m_fVoxelSize;
                    ball.v.Z = 0F;
                }
            }

            //////////////////////////////////////////
            //Compute grid energy

            //memset(m_pnGridPointStatus, 0, (m_nGridSize + 1)*(m_nGridSize + 1)*(m_nGridSize + 1));


            //memset(m_pnGridVoxelStatus, 0, m_nGridSize*m_nGridSize*m_nGridSize);


            {
                for (int z = 0; z < m_nGridSize; z++)
                {
                    for (int y = 0; y < m_nGridSize; y++)
                    {
                        int nIndex = y * (m_nGridSize + 1) + z * (m_nGridSize + 1) * (m_nGridSize + 1);
                        for (int x = 0; x < m_nGridSize; x++)
                        {
                            ComputeGridPointEnergy(x, y, z, nIndex);
                            nIndex++;
                        }
                    }
                }
            }

            /////////////////////////////////////////
            // Rendu

            //int nCase, x, y, z;
            //bool bComputed;


            //for (i = 0; i < m_listBalls.Count(); i++)
            //{
            //	SBall& ball = m_listBalls[i];


            //	x = ConvertWorldCoordinateToGridPoint(ball.p[0]);
            //	y = ConvertWorldCoordinateToGridPoint(ball.p[1]);
            //	z = ConvertWorldCoordinateToGridPoint(ball.p[2]);

            //	// Work our way out from the center of the ball until the surface is
            //	// reached. If the voxel at the surface is already computed then this
            //	// ball share surface with a previous ball.
            //	bComputed = false;
            //	while (1)
            //	{
            //		if (IsGridVoxelComputed(x, y, z))
            //		{
            //			bComputed = true;
            //			break;
            //		}

            //		nCase = ComputeGridVoxel(x, y, z);
            //		if (nCase < 255)
            //			break;

            //		z--;
            //	}

            //	if (bComputed)
            //		continue;

            //	// Compute all voxels on the surface by computing neighbouring voxels
            //	// if the surface goes into them.
            //	AddNeighborsToList(nCase, x, y, z);

            //	while (m_nNumOpenVoxels)
            //	{
            //		m_nNumOpenVoxels--;
            //		x = m_pOpenVoxels[m_nNumOpenVoxels * 3];
            //		y = m_pOpenVoxels[m_nNumOpenVoxels * 3 + 1];
            //		z = m_pOpenVoxels[m_nNumOpenVoxels * 3 + 2];

            //		nCase = ComputeGridVoxel(x, y, z);

            //		AddNeighborsToList(nCase, x, y, z);
            //	}
            //}


        }
        public void Draw(Shader _shader, ref Matrix4x4 _view, ref Matrix4x4 _proj, ref Matrix4x4 _world)
        {
            int x;
            int y;
            int z;
            int cubeCount = 0;


            m_pbrush.m_array.Clear();

            //Parse Grid Energy
            SmallCube c = new SmallCube();
            c.byMatL0 = 128;

            int nIndex = 0;
            for (z = 0; z < m_nGridSize; z++)
            {
                for (y = 0; y < m_nGridSize; y++)
                {
                    nIndex = y * (m_nGridSize + 1) + z * (m_nGridSize + 1) * (m_nGridSize + 1);

                    for (x = 0; x < m_nGridSize; x++)
                    {
                        //int nIndex = x +
                        //	y*(m_nGridSize + 1) +
                        //	z*(m_nGridSize + 1)*(m_nGridSize + 1);

                        float e = m_pfGridEnergy[nIndex];
                        if (e > m_fLevel)
                        {
                            c.byMatL0 = (byte)(128 + z); // ((e - m_fLevel) / 64);

                            cubeCount++;
                            m_pbrush.SetCube(x, y, z, c);
                        }

                        nIndex++;
                    }

                }

            }

            m_pbrush.GenerateMesh();

            m_pbrush.Draw(_shader, ref _view, ref _proj, ref _world);

            //printf("cubeCount :%d\n", cubeCount);

        }


        protected float ComputeEnergy(ref System.Numerics.Vector3 _pos)
        {
            float fEnergy = 0F;
            float fSqDist;

            for (int i = 0; i < BALLS_COUNT; i++)
            {
                ref SBall ball = ref m_listBalls[i];

                // The formula for the energy is
                //
                //   e += mass/distance^2

                //fSqDist = (ball.p.X - x) * (ball.p.X - x) + (ball.p.Y - y) * (ball.p.Y - y) + (ball.p.Z - z) * (ball.p.Z - z);
                fSqDist = (ball.p - _pos).LengthSquared();

                if (fSqDist < 0.0001f)
                {
                    fSqDist = 0.0001f;
                }

                fEnergy += ball.m / fSqDist;
            }

            return fEnergy;
        }

        protected float ComputeGridPointEnergy(int x, int y, int z, int _index)
        {
            //int nIndex = x + y * (m_nGridSize + 1) + z * (m_nGridSize + 1) * (m_nGridSize + 1);
            int nIndex = _index;

            //IsGridPointComputed
            //if (m_pnGridPointStatus[nIndex] == 1)
            //	return m_pfGridEnergy[nIndex];

            // The energy on the edges are always zero to make sure the isosurface is
            // always closed.
            if (x == 0 || y == 0 || z == 0 || x == m_nGridSize || y == m_nGridSize || z == m_nGridSize)
            {
                m_pfGridEnergy[nIndex] = 0.0f;

                //m_pnGridPointStatus[nIndex] = 1;
                return 0.0f;
            }
            System.Numerics.Vector3 pos = new System.Numerics.Vector3(x, y, z);
            pos *= m_fVoxelSize;   // ConvertGridPointToWorldCoordinate
            pos -= System.Numerics.Vector3.One;
            //float fx = ConvertGridPointToWorldCoordinate(x);
            //float fy = ConvertGridPointToWorldCoordinate(y);
            //float fz = ConvertGridPointToWorldCoordinate(z);

            var e = ComputeEnergy(ref pos);
            m_pfGridEnergy[nIndex] = e;

            //m_pnGridPointStatus[nIndex] = 1;

            return e;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //protected float ConvertGridPointToWorldCoordinate(int x)
        //{
        //	return (float)x * m_fVoxelSize - 1.0f;
        //}

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // protected int ConvertWorldCoordinateToGridPoint(float x)
        // {
        // 	return (int)((x + 1.0f) / m_fVoxelSize + 0.5f);
        // }


    }

}

