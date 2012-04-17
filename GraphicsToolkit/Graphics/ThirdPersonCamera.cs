using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GraphicsToolkit.Input;
using System.Diagnostics;

namespace GraphicsToolkit.Graphics
{
    public class ThirdPersonCamera : Camera
    {
        private float xRot, yRot;
        private int lastMouseX, lastMouseY;
        private Vector3 lookAtTarget;
        private float fieldOfView = 45f;
        private float rotSpeed;
        Matrix rotation;
        private Vector3 dir, up, left;
        protected static Vector3 startDir;
        protected static Vector3 startUp = Vector3.Up;
        private float chaseDistance;
        private const float chaseSpeed = 20f;

        private Vector2 mouseDelta;

        public Vector2 MouseDelta
        {
            get
            {
                return mouseDelta;
            }
        }

        public Vector3 Forward
        {
            get
            {
                return dir;
            }
        }

        public Vector3 Right
        {
            get
            {
                return -left;
            }
        }

        public Vector3 Up
        {
            get
            {
                return up;
            }
        }

        public Vector3 TargetPos
        {
            get
            {
                return lookAtTarget;
            }
            set
            {
                lookAtTarget = value;
            }
        }

        public ThirdPersonCamera(Vector3 initialTargetPosition, float chaseDistance, float rotSpeed) : base()
        {
            startDir = new Vector3(0, 0, -1);
            lookAtTarget = initialTargetPosition;
            this.chaseDistance = chaseDistance;
            this.rotSpeed = rotSpeed;

            float aspectRatio = (float)Config.ScreenWidth / Config.ScreenHeight;

            dir = startDir;
            up = Vector3.Up;
          
            Matrix.CreateLookAt(ref pos, ref dir, ref up, out view);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fieldOfView), aspectRatio, nearPlane, farPlane);

            //Set the last mouse pos equal to the center of the screen
            //and center our current mouse position
            lastMouseX = Config.ScreenWidth / 2;
            lastMouseY = Config.ScreenHeight / 2;

            Mouse.SetPosition(lastMouseX, lastMouseY);
        }

        public override void Resize()
        {
            base.Resize();

            float aspectRatio = (float)Config.ScreenWidth / Config.ScreenHeight;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fieldOfView), aspectRatio, nearPlane, farPlane);
        }

        public override void Update(GameTime g)
        {
            base.Update(g);

            float dt = (float)g.ElapsedGameTime.TotalSeconds;
            MouseState mState = Mouse.GetState();
            KeyboardState keyState = InputHandler.KeyState;
            Mouse.SetPosition(Config.ScreenWidth / 2, Config.ScreenHeight / 2);

            //Update orientation according to mouse movement
            int currentX = mState.X;
            int currentY = mState.Y;
            int dx = currentX - lastMouseX;
            int dy = currentY - lastMouseY;
            mouseDelta = new Vector2(dx, dy);

            xRot -= dy *  rotSpeed * dt;
            xRot = MathHelper.Clamp(xRot, -MathHelper.PiOver2 + 0.1f, MathHelper.PiOver2 - 0.1f); //Limit how far above and below the camera can go
            yRot -= dx * rotSpeed * dt;
            yRot = Math.Sign(yRot) * Math.Abs(yRot % MathHelper.TwoPi);
            rotation = Matrix.CreateRotationX(xRot) * Matrix.CreateRotationY(yRot);
            Vector3.TransformNormal(ref startDir, ref rotation, out dir);
            Vector3.TransformNormal(ref startUp, ref rotation, out up);
            Vector3.Cross(ref up, ref dir, out left);

            //pos = dir + -1 * forward * chaseDistance;
            Vector3 desiredPos = lookAtTarget - (dir * chaseDistance);
            Vector3 vel = desiredPos - pos;
            pos += chaseSpeed * dt * vel;

            //Update view with our new info
            Matrix.CreateLookAt(ref pos, ref lookAtTarget, ref up, out view);
        }



    }
}
