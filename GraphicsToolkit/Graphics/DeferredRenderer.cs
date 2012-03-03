using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GraphicsToolkit.Graphics
{
    public class DeferredRenderer
    {
        private Effect DeferredEffect;

        private GraphicsDevice device;
        private RenderTarget2D colorTarget;
        private RenderTarget2D normalTarget;
        private RenderTarget2D depthTarget;

        private VertexPositionTexture[] quad;
        private short[] quadIndices;

        public DeferredRenderer(GraphicsDevice device, ContentManager content)
        {
            this.device = device;
            InitializeGBuffer();
            SetUpQuad();
            DeferredEffect = content.Load<Effect>("Effects/Deferred");

            DeferredEffect.Parameters["HalfPixelWidth"].SetValue((1f / Config.ScreenWidth) / 2f);
            DeferredEffect.Parameters["HalfPixelHeight"].SetValue((1f / Config.ScreenHeight) / 2f);
        }

        private void SetUpQuad()
        {
            quad = new VertexPositionTexture[]
                        {
                            new VertexPositionTexture(
                                new Vector3(-1,1,0),
                                new Vector2(0,0)),
                            new VertexPositionTexture(
                                new Vector3(1,1,0),
                                new Vector2(1,0)),
                            new VertexPositionTexture(
                                new Vector3(1,-1,0),
                                new Vector2(1,1)),
                            new VertexPositionTexture(
                                new Vector3(-1,-1,0),
                                new Vector2(0,1))
                        };

            quadIndices = new short[] { 0, 1, 2, 2, 3, 0 };
        }

        private void InitializeGBuffer()
        {
            colorTarget = new RenderTarget2D(device, Config.ScreenWidth, Config.ScreenHeight, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            normalTarget = new RenderTarget2D(device, Config.ScreenWidth, Config.ScreenHeight, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            depthTarget = new RenderTarget2D(device, Config.ScreenWidth, Config.ScreenHeight, false, SurfaceFormat.Vector2, DepthFormat.None);
        }

        public void Render(List<Mesh> meshes, Camera cam)
        {
            DeferredEffect.CurrentTechnique = DeferredEffect.Techniques["GenerateBuffer"];
            DeferredEffect.Techniques["GenerateBuffer"].Passes[0].Apply();
            DeferredEffect.Parameters["View"].SetValue(cam.View);
            DeferredEffect.Parameters["Projection"].SetValue(cam.Projection);

            device.SetRenderTarget(colorTarget);

            foreach (Mesh m in meshes)
            {
                DeferredEffect.Parameters["World"].SetValue(Matrix.Identity);
                device.Textures[0] = m.Texture;
                device.SetVertexBuffer(m.Vertices);
                device.Indices = m.Indices;

                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m.Vertices.VertexCount, 0, m.Indices.IndexCount / 3);
            }
            device.SetRenderTarget(null);

            RenderQuad(colorTarget);
        }

        private void RenderQuad(Texture2D texture)
        {
            DeferredEffect.CurrentTechnique = DeferredEffect.Techniques["RenderQuad"];
            DeferredEffect.CurrentTechnique.Passes[0].Apply();
            device.Textures[0] = texture;
            device.SamplerStates[0] = SamplerState.PointClamp;
            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, quad, 0, 4, quadIndices, 0, 2);
        }
    }
}
