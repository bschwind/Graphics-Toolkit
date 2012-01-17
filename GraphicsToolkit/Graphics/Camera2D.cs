using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using GraphicsToolkit.GUI;
using GraphicsToolkit.Input;

namespace GraphicsToolkit.Graphics
{
    /// <summary>
    /// An orthographic camera for viewing 2D objects on the XY plane
    /// </summary>
    public class Camera2D : Camera
    {
        private Vector3 pos, vel; //These are stored internally as 3D vectors so we can use the Z component for zoom
        private float maxVel, dampingFactor, accFactor, zoomFactor, zoomVel, zoomAmount;
        private float normalizedWidth, normalizedHeight;

        private float maxZoom;
        private Panel panel;

        public Camera2D(Vector2 pos, Panel p)
        {
            this.panel = p;
            Resize();
            view = Matrix.CreateLookAt(new Vector3(pos.X, pos.Y, 1), new Vector3(pos.X, pos.Y, 0), Vector3.Up);

            this.pos = new Vector3(0, 0, 1);
            this.vel = Vector3.Zero;

            maxVel = 100f;
            maxZoom = 1000f;
            dampingFactor = 0.85f;
            accFactor = 10f;
            zoomFactor = 15f;
            zoomAmount = 1f;
        }

        public override void Resize()
        {
            base.Resize();

            float width = Config.ScreenWidth;
            float height = Config.ScreenHeight;

            if (width > height)
            {
                normalizedWidth = 1.0f;
                normalizedHeight = height / width;
            }
            else
            {
                normalizedWidth = width / height;
                normalizedHeight = 1.0f;
            }

            refresh();
        }

        public Vector2 GetWorldPos(Vector2 screenPos)
        {
            Vector3 result = panel.ViewPort.Unproject(new Vector3(screenPos.X, screenPos.Y, 0), projection, view, Matrix.Identity);
            return new Vector2(result.X, result.Y);
        }

        public Vector2 GetWorldMousePos()
        {
            MouseState mouse = InputHandler.MouseState;
            Vector3 result = panel.ViewPort.Unproject(new Vector3(mouse.X, mouse.Y, 0), projection, view, Matrix.Identity);
            return new Vector2(result.X, result.Y);
        }

        private Vector2 GetScreenPos(Vector3 worldPos)
        {
            Vector3 val = panel.ViewPort.Project(worldPos, projection, view, Matrix.Identity);
            return new Vector2(val.X, val.Y);
        }

        public Vector2 GetScreenPos(Vector2 worldPos)
        {
            Vector3 val = panel.ViewPort.Project(new Vector3(worldPos, 0), projection, view, Matrix.Identity);
            return new Vector2(val.X, val.Y);
        }

        public Rectangle WorldRectToScreen(Rectangle worldRect)
        {
            Vector3 upperLeft = new Vector3(worldRect.X, worldRect.Y, 0);
            Vector3 bottomRight = new Vector3(worldRect.X + worldRect.Width, worldRect.Y - worldRect.Height, 0);
            Vector2 newUpperLeft = GetScreenPos(upperLeft);
            Vector2 newBottomRight = GetScreenPos(bottomRight);

            return new Rectangle((int)newUpperLeft.X, (int)newUpperLeft.Y, (int)(newBottomRight.X - newUpperLeft.X), (int)(newBottomRight.Y - newUpperLeft.Y));
        }

        private void refresh()
        {
            projection = Matrix.CreateOrthographic(zoomAmount * normalizedWidth, zoomAmount * normalizedHeight, nearPlane, farPlane);
        }

        public override void Update(GameTime g)
        {
            base.Update(g);

            float dt = (float)g.ElapsedGameTime.TotalSeconds;

            float zoomAcc = 0f;
            if (InputHandler.IsKeyPressed(Keys.OemPlus))
            {
                zoomAcc = -1f;
            }
            if (InputHandler.IsKeyPressed(Keys.OemMinus))
            {
                zoomAcc = 1f;
            }

            zoomAcc *= zoomFactor;
            Vector3 acc = new Vector3(0, 0, 0);
            if (InputHandler.IsKeyPressed(Keys.A))
            {
                acc.X = -1f;
            }
            if (InputHandler.IsKeyPressed(Keys.D))
            {
                acc.X = 1f;
            }
            if (InputHandler.IsKeyPressed(Keys.W))
            {
                acc.Y = 1f;
            }
            if (InputHandler.IsKeyPressed(Keys.S))
            {
                acc.Y = -1f;
            }

            acc.X *= zoomAmount;
            acc.Y *= zoomAmount;

            vel += (acc * accFactor * dt);
            zoomVel += (zoomAcc * zoomFactor * dt);
            vel.X = MathHelper.Clamp(vel.X, -maxVel, maxVel);
            vel.Y = MathHelper.Clamp(vel.Y, -maxVel, maxVel);
            vel.Z = MathHelper.Clamp(vel.Z, -maxVel, maxVel);

            vel *= dampingFactor;
            zoomVel *= dampingFactor;

            pos += vel * dt;
            zoomAmount += zoomVel * dt;
            zoomAmount = MathHelper.Clamp(zoomAmount, 1f, maxZoom);

            view = Matrix.CreateLookAt(pos, pos + new Vector3(0, 0, -1), Vector3.Up);

            refresh();
        }
    }
}