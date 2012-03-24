//Special thanks to Paul Firth:
//http://www.wildbunny.co.uk/blog/2011/03/25/speculative-contacts-an-continuous-collision-engine-approach-part-1/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._3D.Bodies;

namespace GraphicsToolkit.Physics._3D
{
    public class Solver3D
    {
        public static void Solve(List<Contact3D> contacts, int iterations, float dt)
        {
            bool SpecSequential = false;
            float dtInv = 1f / dt;
            for (int j = 0; j < iterations; j++)
            {
                for (int i = 0; i < contacts.Count; i++)
                {
                    Contact3D con = contacts[i];
                    Vector3 n = con.Normal;

                    float relNv = Vector3.Dot(con.B.Vel - con.A.Vel, n);

                    //Do either speculative or speculative sequential
                    if (!SpecSequential)
                    {
                        float remove = relNv + con.Dist * dtInv;

                        if (remove < 0)
                        {
                            float mag = remove / (con.A.InvMass + con.B.InvMass);
                            Vector3 imp = con.Normal * mag;
                            con.ApplyImpulses(imp);
                            //Vector3 aVel = con.A.GetVelocityOfPoint(con.pointA);
                            //Vector3 bVel = con.B.GetVelocityOfPoint(con.pointB);
                            //Vector3 relVel = bVel - aVel;
                            //con.B.AddForce(relVel * (con.A.InvMass + con.B.InvMass));
                            //con.A.AddForce(relVel * (con.A.InvMass + con.B.InvMass));
                            //con.ApplyImpulses(relVel / (con.A.InvMass + con.B.InvMass));
                        }
                    }
                    else
                    {
                        //Do sequential...later
                    }
                }
            }
        }
    }
}
