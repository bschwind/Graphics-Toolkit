﻿using System;
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

        private Effect DirectionalLightEffect;

        private GraphicsDevice device;
        private RenderTarget2D colorTarget;
        private RenderTarget2D normalTarget;
        private RenderTarget2D depthTarget;
        private RenderTarget2D lightTarget;

        private BlendState LightMapBS;

        private VertexPositionTexture[] quad;
        private short[] quadIndices;

        public DeferredRenderer(GraphicsDevice device, ContentManager content)
        {
            this.device = device;

            InitializeGBuffer();
            SetUpQuad();
            SetUpLightMap();

            DeferredEffect = content.Load<Effect>("Effects/Deferred");
            DirectionalLightEffect = content.Load<Effect>("Effects/DeferredDirectionalLight");

            SetUpEffectParameters();
        }

        private void SetUpEffectParameters()
        {
            float halfPixWidth = (1f / Config.ScreenWidth) / 2f;
            float halfPixHeight = (1f / Config.ScreenHeight) / 2f;

            DeferredEffect.Parameters["HalfPixelWidth"].SetValue(halfPixWidth);
            DeferredEffect.Parameters["HalfPixelHeight"].SetValue(halfPixHeight);

            DirectionalLightEffect.Parameters["HalfPixelWidth"].SetValue(halfPixWidth);
            DirectionalLightEffect.Parameters["HalfPixelHeight"].SetValue(halfPixHeight);
        }

        private void SetUpLightMap()
        {  
            //Initialize LightMapBS
            LightMapBS = new BlendState();
            LightMapBS.ColorSourceBlend = Blend.One;
            LightMapBS.ColorDestinationBlend = Blend.One;
            LightMapBS.ColorBlendFunction = BlendFunction.Add;
            LightMapBS.AlphaSourceBlend = Blend.One;
            LightMapBS.AlphaDestinationBlend = Blend.One;
            LightMapBS.AlphaBlendFunction = BlendFunction.Add;

            //Create LightMap RT
            lightTarget = new RenderTarget2D(device, Config.ScreenWidth, Config.ScreenHeight, false, SurfaceFormat.Color, DepthFormat.None);
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
            //HalfVector4 because RGBA64 is unavailable
            //Extra slot for spec power
            colorTarget = new RenderTarget2D(device, Config.ScreenWidth, Config.ScreenHeight, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            //HalfVector4 is used for signed normals
            //Extra slot for spec intensity
            normalTarget = new RenderTarget2D(device, Config.ScreenWidth, Config.ScreenHeight, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            //Vector2 is used
            //One slot for depth, the other for ...?
            depthTarget = new RenderTarget2D(device, Config.ScreenWidth, Config.ScreenHeight, false, SurfaceFormat.Vector2, DepthFormat.None);
        }

        public void Render(List<Mesh> meshes, Camera cam)
        {
            ClearGBuffer();
            RenderGBuffer(meshes, cam);
            RenderLights(cam);

            
            device.BlendState = BlendState.Opaque;
            device.RasterizerState = RasterizerState.CullCounterClockwise;
            device.DepthStencilState = DepthStencilState.Default;

            //Test drawing
            device.SetRenderTargets(null);
            RenderTexturedQuad(colorTarget);
        }

        private void RenderGBuffer(List<Mesh> meshes, Camera cam)
        {
            device.DepthStencilState = DepthStencilState.Default;

            DeferredEffect.CurrentTechnique = DeferredEffect.Techniques["GenerateBuffer"];
            DeferredEffect.CurrentTechnique.Passes[0].Apply();
            DeferredEffect.Parameters["View"].SetValue(cam.View);
            DeferredEffect.Parameters["Projection"].SetValue(cam.Projection);

            device.SetRenderTargets(colorTarget, normalTarget, depthTarget);

            foreach (Mesh m in meshes)
            {
                DeferredEffect.Parameters["World"].SetValue(Matrix.Identity);
                device.Textures[0] = m.Texture;
                device.SetVertexBuffer(m.Vertices);
                device.Indices = m.Indices;

                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m.Vertices.VertexCount, 0, m.Indices.IndexCount / 3);
            }
        }

        private void ClearGBuffer()
        {
            //Set to ReadOnly depth
            device.DepthStencilState = DepthStencilState.DepthRead;
            DeferredEffect.CurrentTechnique = DeferredEffect.Techniques["ClearBuffer"];
            DeferredEffect.CurrentTechnique.Passes[0].Apply();
            device.SetRenderTargets(colorTarget, normalTarget, depthTarget);
            RenderQuad();
        }

        private void RenderLights(Camera cam)
        {
            device.SetRenderTarget(lightTarget);
            device.Clear(Color.Black);
            device.BlendState = LightMapBS;
            device.DepthStencilState = DepthStencilState.DepthRead;

            //Color Sampler
            device.Textures[0] = colorTarget;
            device.SamplerStates[0] = SamplerState.PointClamp;
            //Normal Sampler
            device.Textures[1] = normalTarget;
            device.SamplerStates[1] = SamplerState.PointClamp;
            //Depth
            device.Textures[2] = depthTarget;
            device.SamplerStates[2] = SamplerState.PointClamp;

            Matrix inverseView = Matrix.Invert(cam.View);
            Matrix inverseViewProj = Matrix.Invert(cam.View * cam.Projection);

            DirectionalLightEffect.Parameters["InverseViewProj"].SetValue(inverseViewProj);
            DirectionalLightEffect.Parameters["InverseView"].SetValue(inverseView);
            DirectionalLightEffect.Parameters["CamPos"].SetValue(cam.Pos);

            DirectionalLightEffect.Parameters["LightDir"].SetValue(new Vector3(0, -1, 0));
            DirectionalLightEffect.Parameters["LightColor"].SetValue(Color.White.ToVector4());
            DirectionalLightEffect.Parameters["LightIntensity"].SetValue(1);

            DirectionalLightEffect.CurrentTechnique.Passes[0].Apply();
            RenderQuad();
        }

        private void RenderTexturedQuad(Texture2D texture)
        {
            DeferredEffect.CurrentTechnique = DeferredEffect.Techniques["RenderQuad"];
            DeferredEffect.CurrentTechnique.Passes[0].Apply();
            device.Textures[0] = texture;
            device.SamplerStates[0] = SamplerState.PointClamp;
            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, quad, 0, 4, quadIndices, 0, 2);
        }

        private void RenderQuad()
        {
            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, quad, 0, 4, quadIndices, 0, 2);
        }
    }
}
