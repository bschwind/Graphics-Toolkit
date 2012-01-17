﻿﻿using System;
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
    public class PrimitiveBatch
    {
        GraphicsDevice device;
        BasicEffect effect;

        //XNA is multiplatform. Some platforms can only draw so many vertices at once,
        //depending on the profile. These numbers come from the XNA documentation for
        //The "Reach" and "HiDef" profiles.
        //HiDef devices: PC, Xbox 360
        //Reach devices: Windows Phone 7, Zune
        //HiDef devices can run both HiDef and Reach profiles
        const int maxVertsPerDrawReach = 65535 * 2;
        const int maxVertsPerDrawHiDef = 1048575 * 2;
        int maxVertsPerDraw;

        int vertCounter = 0;
        int vertsPerPrimitive;
        VertexPositionColor[] verts;
        Camera currentCam;
        bool hasBegun = false;
        PrimitiveType currentType;

        public GraphicsDevice Device
        {
            get
            {
                return device;
            }
        }

        public PrimitiveBatch(GraphicsDevice gd)
        {
            device = gd;
            effect = new BasicEffect(device);
            effect.VertexColorEnabled = true;

            //Limit our buffer to the max allowed batch size for reach
            //or hidef, depending on the profile
            if (device.GraphicsProfile == GraphicsProfile.Reach)
            {
                maxVertsPerDraw = maxVertsPerDrawReach;
            }
            else
            {
                maxVertsPerDraw = maxVertsPerDrawHiDef;
            }

            verts = new VertexPositionColor[maxVertsPerDraw];
        }

        /// <summary>
        /// Begins the batch process for drawing 3D primitives
        /// </summary>
        /// <param name="primType">Only line list and triangle list are supported</param>
        /// <param name="cam">The camera to render with</param>
        public void Begin(PrimitiveType primType, Camera cam)
        {
            if (hasBegun)
            {
                throw new Exception("Can't call Begin until current batch has ended");
            }

            vertCounter = 0;
            vertsPerPrimitive = numVertsPerPrimitive(primType);
            currentCam = cam;
            currentType = primType;

            hasBegun = true;
        }

        /// <summary>
        /// The simplest call for PrimitiveBatch. Adds a single vertex to be rendered to the batch.
        /// If called twice with different vertices while using a line list, a line would be rendered.
        /// </summary>
        /// <param name="vpc"></param>
        public void AddVertex(VertexPositionColor vpc)
        {
            if (!hasBegun)
            {
                throw new Exception("You must begin a batch before you can add vertices");
            }

            if (vertCounter >= verts.Length)
            {
                Flush();
            }

            verts[vertCounter] = vpc;
            vertCounter++;
        }

        /// <summary>
        /// Draws a line with a given color
        /// </summary>
        /// <param name="v1">The position of the first vertex</param>
        /// <param name="v2">The position of the second vertex</param>
        /// <param name="color">The color of the line</param>
        public void DrawLine(Vector3 v1, Vector3 v2, Color color)
        {
            AddVertex(new VertexPositionColor(v1, color));
            AddVertex(new VertexPositionColor(v2, color));
        }

        /// <summary>
        /// Draws a 2D line with a given color
        /// </summary>
        /// <param name="v1">The position of the first vertex</param>
        /// <param name="v2">The position of the second vertex</param>
        /// <param name="color">The color of the line</param>
        public void DrawLine(Vector2 v1, Vector2 v2, Color color)
        {
            AddVertex(new VertexPositionColor(new Vector3(v1, 0), color));
            AddVertex(new VertexPositionColor(new Vector3(v2, 0), color));
        }

        /// <summary>
        /// Draws a triangle at the given points, with the given color
        /// </summary>
        /// <param name="v1">The triangle's first vertex</param>
        /// <param name="v2">The triangle's second vertex</param>
        /// <param name="v3">The triangle's third vertex</param>
        /// <param name="color">The color of the triangle</param>
        public void DrawTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color color)
        {
            AddVertex(new VertexPositionColor(v1, color));
            AddVertex(new VertexPositionColor(v2, color));
            AddVertex(new VertexPositionColor(v2, color));
            AddVertex(new VertexPositionColor(v3, color));
            AddVertex(new VertexPositionColor(v3, color));
            AddVertex(new VertexPositionColor(v1, color));
        }

        public void DrawTriangle(Vector2 v1, Vector2 v2, Vector2 v3, Color color)
        {
            AddVertex(new VertexPositionColor(new Vector3(v1, 0), color));
            AddVertex(new VertexPositionColor(new Vector3(v2, 0), color));
            AddVertex(new VertexPositionColor(new Vector3(v2, 0), color));
            AddVertex(new VertexPositionColor(new Vector3(v3, 0), color));
            AddVertex(new VertexPositionColor(new Vector3(v3, 0), color));
            AddVertex(new VertexPositionColor(new Vector3(v1, 0), color));
        }

        public void DrawCircle(Vector2 center, float radius, int segments, Color color)
        {
            int iterations = segments;

            Vector3 prevPos = new Vector3(center, 0) + new Vector3(radius, 0, 0);

            for (int i = 0; i < iterations; i++)
            {
                float angle = ((float)i / iterations) * MathHelper.TwoPi;
                Vector3 pos = new Vector3(center, 0) + new Vector3((float)Math.Cos(angle) * radius, (float)Math.Sin(angle) * radius, 0);
                AddVertex(new VertexPositionColor(prevPos, color));
                AddVertex(new VertexPositionColor(pos, color));
                prevPos = pos;
            }

            AddVertex(new VertexPositionColor(prevPos, color));
            AddVertex(new VertexPositionColor(new Vector3(center, 0) + new Vector3(radius, 0, 0), color));
        }

        public void DrawPie(Vector2 center, float radius, float startAngle, float endAngle, int segments, Color color)
        {
            AddVertex(new VertexPositionColor(new Vector3(center, 0), color));
            AddVertex(new VertexPositionColor(new Vector3(center, 0) + new Vector3((float)Math.Cos(startAngle), (float)Math.Sin(startAngle), 0) * radius, color));

            float delta = Math.Abs(endAngle - startAngle) / segments;

            for (int i = 0; i < segments; i++)
            {
                float start = startAngle + (i * delta);
                float end = startAngle + ((i + 1) * delta);
                AddVertex(new VertexPositionColor(new Vector3(center, 0) + new Vector3((float)Math.Cos(start), (float)Math.Sin(start), 0) * radius, color));
                AddVertex(new VertexPositionColor(new Vector3(center, 0) + new Vector3((float)Math.Cos(end), (float)Math.Sin(end), 0) * radius, color));
            }

            AddVertex(new VertexPositionColor(new Vector3(center, 0) + new Vector3((float)Math.Cos(endAngle), (float)Math.Sin(endAngle), 0) * radius, color));
            AddVertex(new VertexPositionColor(new Vector3(center, 0), color));
        }

        /// <summary>
        /// Draws a grid with specified width and depth, in the positive XZ plane
        /// </summary>
        /// <param name="width">The number of cells the grid should have along the X axis</param>
        /// <param name="depth">The number of cells the grid should have along the Z axis</param>
        /// <param name="color">The color of the grid</param>
        public void DrawXZGrid(int width, int depth, Color color)
        {
            for (int x = 0; x <= width; x++)
            {
                DrawLine(new Vector3(x, 0, 0), new Vector3(x, 0, depth), color);
            }

            for (int z = 0; z <= depth; z++)
            {
                DrawLine(new Vector3(0, 0, z), new Vector3(width, 0, z), color);
            }
        }

        /// <summary>
        /// Draws a grid with specified width and height, in the positive XY plane
        /// </summary>
        /// <param name="width">The number of cells the grid should have along the X axis</param>
        /// <param name="depth">The number of cells the grid should have along the Y axis</param>
        /// <param name="color">The color of the grid</param>
        public void DrawXYGrid(int width, int height, Color color)
        {
            for (int x = 0; x <= width; x++)
            {
                DrawLine(new Vector3(x, 0, 0), new Vector3(x, height, 0), color);
            }

            for (int y = 0; y <= height; y++)
            {
                DrawLine(new Vector3(0, y, 0), new Vector3(width, y, 0), color);
            }
        }

        /// <summary>
        /// Draws a grid with specified width and height, in the positive YZ plane
        /// </summary>
        /// <param name="width">The number of cells the grid should have along the X axis</param>
        /// <param name="depth">The number of cells the grid should have along the Y axis</param>
        /// <param name="color">The color of the grid</param>
        public void DrawYZGrid(int height, int depth, Color color)
        {
            for (int y = 0; y <= height; y++)
            {
                DrawLine(new Vector3(0, y, 0), new Vector3(0, y, depth), color);
            }

            for (int z = 0; z <= depth; z++)
            {
                DrawLine(new Vector3(0, 0, z), new Vector3(0, height, z), color);
            }
        }

        public void Draw2DGrid(int width, int height, Color color)
        {
            for (int x = 0; x <= width; x++)
            {
                DrawLine(new Vector2(x, 0), new Vector2(x, height), color);
            }

            for (int y = 0; y <= height; y++)
            {
                DrawLine(new Vector2(0, y), new Vector2(width, y), color);
            }
        }

        /// <summary>
        /// Fills a triangle with a specified color. Must be called with triangle list as the primitive type.
        /// </summary>
        /// <param name="v1">The triangle's first vertex</param>
        /// <param name="v2">The triangle's second vertex</param>
        /// <param name="v3">The triangle's third vertex</param>
        /// <param name="color">The color of the triangle</param>
        public void FillTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color color)
        {
            Vector3 normal = Vector3.Normalize(Vector3.Cross(v3 - v1, v2 - v1));

            AddVertex(new VertexPositionColor(v1, color));
            AddVertex(new VertexPositionColor(v2, color));
            AddVertex(new VertexPositionColor(v3, color));
        }

        public void FillTriangle(Vector2 v1, Vector2 v2, Vector2 v3, Color color)
        {
            AddVertex(new VertexPositionColor(new Vector3(v1, 0), color));
            AddVertex(new VertexPositionColor(new Vector3(v2, 0), color));
            AddVertex(new VertexPositionColor(new Vector3(v3, 0), color));
        }

        public void FillQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color color)
        {
            FillTriangle(v1, v2, v4, color);
            FillTriangle(v2, v3, v4, color);
        }

        public void FillQuad(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4, Color color)
        {
            FillTriangle(v1, v2, v4, color);
            FillTriangle(v2, v3, v4, color);
        }

        public void FillCircle(Vector2 center, float radius, int segments, Color color)
        {
            float startAngle = 0.0f;
            float endAngle = MathHelper.TwoPi;
            float delta = Math.Abs(endAngle - startAngle) / segments;

            for (int i = 0; i < segments; i++)
            {
                float start = startAngle + (i * delta);
                float end = startAngle + ((i + 1) * delta);

                FillTriangle(center, center + new Vector2((float)Math.Cos(end) * radius, (float)Math.Sin(end) * radius), center + new Vector2((float)Math.Cos(start) * radius, (float)Math.Sin(start) * radius), color);
            }
        }

        public void FillPie(Vector2 center, float radius, float startAngle, float endAngle, int segments, Color color)
        {
            float delta = Math.Abs(endAngle - startAngle) / segments;

            for (int i = 0; i < segments; i++)
            {
                float start = startAngle + (i * delta);
                float end = startAngle + ((i + 1) * delta);

                FillTriangle(center, center + new Vector2((float)Math.Cos(end) * radius, (float)Math.Sin(end) * radius), center + new Vector2((float)Math.Cos(start) * radius, (float)Math.Sin(start) * radius), color);
            }
        }

        /// <summary>
        /// Submits the current buffer to the graphics card for rendering
        /// </summary>
        private void Flush()
        {
            if (vertCounter <= 0)
            {
                return;
            }
            int primitiveCount = vertCounter / vertsPerPrimitive;

            effect.View = currentCam.View;
            effect.Projection = currentCam.Projection;

            effect.CurrentTechnique.Passes[0].Apply();
            device.DrawUserPrimitives<VertexPositionColor>(currentType, verts, 0, primitiveCount);

            vertCounter = 0;
        }

        /// <summary>
        /// Given a vertex buffer and a primitive type, draws the contents of buffer.
        /// </summary>
        /// <param name="buffer">The vertex buffer to render</param>
        /// <param name="primType">The primtitive type to use</param>
        /// <param name="cam"></param>
        public void RenderVertexBuffer(VertexBuffer buffer, PrimitiveType primType, Camera cam)
        {
            device.SetVertexBuffer(buffer);
            effect.View = currentCam.View;
            effect.Projection = currentCam.Projection;

            effect.CurrentTechnique.Passes[0].Apply();
            int passes = buffer.VertexCount / maxVertsPerDraw;
            int remainder = buffer.VertexCount % maxVertsPerDraw;
            int offset = 0;
            for (int i = 0; i < passes; i++)
            {
                device.DrawPrimitives(primType, offset, maxVertsPerDraw / numVertsPerPrimitive(primType));
                offset += maxVertsPerDraw;
            }

            device.DrawPrimitives(primType, offset, remainder / numVertsPerPrimitive(primType));
        }

        /// <summary>
        /// Finalizes the batch call
        /// </summary>
        public void End()
        {
            if (!hasBegun)
            {
                throw new Exception("Can't end a batch without beginning it!");
            }

            Flush();
            hasBegun = false;
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