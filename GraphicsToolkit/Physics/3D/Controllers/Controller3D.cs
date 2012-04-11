using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._3D.Bodies;

namespace GraphicsToolkit.Physics._3D.Controllers
{
    public abstract class Controller3D
    {
        private RigidBody3D body;

        public Controller3D(RigidBody3D body)
        {
            this.body = body;
        }

        public void AddForce(Vector3 f)
        {
            body.AddForce(f);
        }

        /// <summary>
        /// Adds a force f onto a point p in the local space of the body
        /// </summary>
        /// <param name="f">The force to apply</param>
        /// <param name="p">The point on the body to which the force is applied</param>
        public void AddForce(Vector3 f, Vector3 p)
        {
            body.AddForce(f, p);
        }

        public void SetVelocity(Vector3 v)
        {
            body.Vel = v;
        }

        public abstract void Update(float dt);
    }
}
