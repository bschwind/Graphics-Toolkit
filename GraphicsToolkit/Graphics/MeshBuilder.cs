﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraphicsToolkit.Graphics
{
    /// <summary>
    /// Used to draw simple shapes on the graphics card. Intended for graphics debugging.
    /// </summary>
    public class MeshBuilder
    {
        GraphicsDevice device;
        List<VertexPositionNormalTexture> softVerts, hardVerts;
        List<TriangleData> softTriangles, hardTriangles;
        bool hasBegun = false;

        public GraphicsDevice Device
        {
            get
            {
                return device;
            }
        }

        public MeshBuilder(GraphicsDevice gd)
        {
            device = gd;
            softVerts = new List<VertexPositionNormalTexture>();
            hardVerts = new List<VertexPositionNormalTexture>();
            softTriangles = new List<TriangleData>();
            hardTriangles = new List<TriangleData>();
        }

        /// <summary>
        /// Begins the batch process for drawing 3D primitives
        /// </summary>
        /// <param name="primType">Only line list and triangle list are supported</param>
        /// <param name="cam">The camera to render with</param>
        public void Begin()
        {
            if (hasBegun)
            {
                throw new Exception("Can't call Begin until current batch has ended");
            }

            softVerts.Clear();
            hardVerts.Clear();
            softTriangles.Clear();
            hardTriangles.Clear();

            hasBegun = true;
        }

        public void AddTriangle(Vector3 a, Vector3 b, Vector3 c, bool softEdge)
        {
            if (softEdge)
            {
                int aIndex = -1, bIndex = -1, cIndex = -1;

                for (int i = 0; i < softVerts.Count; i++)
                {
                    if (VerticesAreClose(a, softVerts[i].Position))
                    {
                        aIndex = i;
                    }
                    if (VerticesAreClose(b, softVerts[i].Position))
                    {
                        bIndex = i;
                    }
                    if (VerticesAreClose(c, softVerts[i].Position))
                    {
                        cIndex = i;
                    }
                }

                if (aIndex == -1)
                {
                    softVerts.Add(new VertexPositionNormalTexture(a, Vector3.Zero, Vector2.Zero));
                    aIndex = softVerts.Count - 1;
                }

                if (bIndex == -1)
                {
                    softVerts.Add(new VertexPositionNormalTexture(b, Vector3.Zero, Vector2.Zero));
                    bIndex = softVerts.Count - 1;
                }

                if (cIndex == -1)
                {
                    softVerts.Add(new VertexPositionNormalTexture(c, Vector3.Zero, Vector2.Zero));
                    cIndex = softVerts.Count - 1;
                }

                softTriangles.Add(new TriangleData(aIndex, bIndex, cIndex));
            }
            else
            {
                int aIndex, bIndex, cIndex;

                Vector3 normal = Vector3.Normalize(Vector3.Cross(c - a, b - a));
                hardVerts.Add(new VertexPositionNormalTexture(a, normal, Vector2.Zero));
                aIndex = hardVerts.Count - 1;
                hardVerts.Add(new VertexPositionNormalTexture(b, normal, Vector2.Zero));
                bIndex = hardVerts.Count - 1;
                hardVerts.Add(new VertexPositionNormalTexture(c, normal, Vector2.Zero));
                cIndex = hardVerts.Count - 1;

                hardTriangles.Add(new TriangleData(aIndex, bIndex, cIndex));
            }
        }

        public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, bool softEdge)
        {
            AddTriangle(v1, v2, v4, softEdge);
            AddTriangle(v2, v3, v4, softEdge);
        }

        private bool VerticesAreClose(Vector3 v1, Vector3 v2)
        {
            float threshold = 0.001f;
            if ((v1 - v2).Length() <= threshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Mesh End()
        {
            if (!hasBegun)
            {
                throw new Exception("Can't end a batch without beginning it!");
            }

            hasBegun = false;

            short[] finalIndices = new short[(softTriangles.Count + hardTriangles.Count) * 3];

            for (int i = 0; i < softTriangles.Count; i++)
            {
                TriangleData tri = softTriangles[i];

                finalIndices[(i*3)] = (short)tri.A;
                finalIndices[(i * 3) + 1] = (short)tri.B;
                finalIndices[(i * 3) + 2] = (short)tri.C;

                Vector3 a = softVerts[tri.A].Position;
                Vector3 b = softVerts[tri.B].Position;
                Vector3 c = softVerts[tri.C].Position;

                Vector3 normal = Vector3.Cross(c - a, b - a);

                VertexPositionNormalTexture newVert = new VertexPositionNormalTexture(softVerts[tri.A].Position, softVerts[tri.A].Normal + normal, softVerts[tri.A].TextureCoordinate);
                softVerts[tri.A] = newVert;
                newVert = new VertexPositionNormalTexture(softVerts[tri.B].Position, softVerts[tri.B].Normal + normal, softVerts[tri.B].TextureCoordinate);
                softVerts[tri.B] = newVert;
                newVert = new VertexPositionNormalTexture(softVerts[tri.C].Position, softVerts[tri.C].Normal + normal, softVerts[tri.C].TextureCoordinate);
                softVerts[tri.C] = newVert;
            }

            for (int i = 0; i < hardTriangles.Count; i++)
            {
                finalIndices[(i * 3) + (softTriangles.Count * 3)] = (short)(hardTriangles[i].A + softVerts.Count);
                finalIndices[(i * 3) + 1 + (softTriangles.Count * 3)] = (short)(hardTriangles[i].B + softVerts.Count);
                finalIndices[(i * 3) + 2 + (softTriangles.Count * 3)] = (short)(hardTriangles[i].C + softVerts.Count);
            }

            VertexPositionNormalTexture[] finalVerts = new VertexPositionNormalTexture[softVerts.Count + hardVerts.Count];
            for (int i = 0; i < softVerts.Count; i++)
            {
                VertexPositionNormalTexture vert = softVerts[i];
                vert.Normal = Vector3.Normalize(vert.Normal);
                finalVerts[i] = vert;
            }

            for (int i = 0; i < hardVerts.Count; i++)
            {
                finalVerts[i + softVerts.Count] = hardVerts[i];
            }

            VertexBuffer vBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), softVerts.Count + hardVerts.Count, BufferUsage.None);
            IndexBuffer iBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, finalIndices.Length, BufferUsage.None);
            vBuffer.SetData<VertexPositionNormalTexture>(finalVerts);
            iBuffer.SetData<short>(finalIndices);

            Mesh m = new Mesh();
            m.Indices = iBuffer;
            m.Vertices = vBuffer;
            
            return m;
        }

        private static int numVertsPerPrimitive(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.LineList:
                    return 2;
                case PrimitiveType.TriangleList:
                    return 3;
                default:
                    throw new Exception("PrimitiveDrawer doesn't support " + type.ToString());
            }
        }
    }
}