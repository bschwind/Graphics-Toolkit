using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GraphicsToolkit.Input;

namespace GraphicsToolkit.Graphics
{
    //Implements a first person camera that can look up and down, left and right,
    //and move in the direction it is facing
    public class FirstPersonCamera : Camera
    {
        private float xRot, yRot;
        private int lastMouseX, lastMouseY;
        private Matrix rotation;
        private Vector3 lookAtTarget;
        private float turnSpeed;
        private float moveSpeed;
        private float fieldOfView = 45f;
        private Vector3 pos, dir, up, left;
        protected static Vector3 startUp = Vector3.Up;
        protected static Vector3 startDir = new Vector3(0, 0, -1);

        public Vector3 Pos
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
            }
        }

        /// <summary>
        /// Creates a new first person camera, centered at the origin, looking down the negative Z axis.
        /// </summary>
        /// <param name="turnSpeed">How fast the user can rotate the camera. Value must be between 0.1 and 1.0</param>
        /// <param name="moveSpeed">The translational velocity of the camera> Value must be greater than 0</param>
        public FirstPersonCamera(float turnSpeed, float moveSpeed)
            : base()
        {
            if (turnSpeed < 0.1f || turnSpeed > 1.0f)
            {
                throw new ArgumentOutOfRangeException("turnSpeed", "Turn speed must be between 0.1 and 1.0");
            }

            if (moveSpeed <= 0f)
            {
                throw new ArgumentOutOfRangeException("moveSpeed", "Move speed must be greater than 0");
            }

            this.turnSpeed = turnSpeed;
            this.moveSpeed = moveSpeed;

            float aspectRatio = (float)Config.ScreenWidth / Config.ScreenHeight;

            pos = Vector3.Zero;
            dir = startDir;
            up = startUp;
            view = Matrix.CreateLookAt(pos, pos + startDir, startUp);
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

            //Update orienation according to mouse movement
            int currentX = mState.X;
            int currentY = mState.Y;
            int dx = currentX - lastMouseX;
            int dy = currentY - lastMouseY;

            xRot -= dy * turnSpeed * dt;
            xRot = MathHelper.Clamp(xRot, -MathHelper.PiOver2, MathHelper.PiOver2);
            yRot -= dx * turnSpeed * dt;
            yRot = Math.Sign(yRot) * Math.Abs(yRot % MathHelper.TwoPi);
            rotation = Matrix.CreateRotationX(xRot) * Matrix.CreateRotationY(yRot);
            Vector3.TransformNormal(ref startDir, ref rotation, out dir);
            Vector3.TransformNormal(ref startUp, ref rotation, out up);
            Vector3.Cross(ref up, ref dir, out left);

            //Update position according to WASD input
            float forwardMovement = 0f;
            float sideMovement = 0f;

            if (keyState.IsKeyDown(Keys.W))
            {
                forwardMovement = dt;
            }
            if (keyState.IsKeyDown(Keys.S))
            {
                forwardMovement = -dt;
            }
            if (keyState.IsKeyDown(Keys.A))
            {
                sideMovement = dt;
            }
            if (keyState.IsKeyDown(Keys.D))
            {
                sideMovement = -dt;
            }

            pos += dir * forwardMovement * moveSpeed + left * sideMovement * moveSpeed;
            lookAtTarget = pos + dir;

            //Update view with our new info
            Matrix.CreateLookAt(ref pos, ref lookAtTarget, ref up, out view);
        }
    }
}
