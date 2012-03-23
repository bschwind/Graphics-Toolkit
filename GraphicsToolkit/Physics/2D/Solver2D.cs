using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._2D.Bodies;

namespace GraphicsToolkit.Physics._2D
{
    public class Solver2D
    {
        public static void Solve(List<Contact2D> contacts, int iterations, float dt)
        {
            bool SpecSequential = false;
            float dtInv = 1f / dt;
            for (int j = 0; j < iterations; j++)
            {
                for (int i = 0; i < contacts.Count; i++)
                {
                    Contact2D con = contacts[i];
                    Vector2 n = con.Normal;

                    float relNv = Vector2.Dot(con.B.Vel - con.A.Vel, n);

                    //Do either speculative or speculative sequential
                    if (!SpecSequential)
                    {
                        float remove = relNv + con.Dist * dtInv;

                        if (remove < 0)
                        {
                            float mag = remove / (con.A.InvMass + con.B.InvMass);
                            Vector2 imp = con.Normal * mag;
                            con.ApplyImpulses(imp);
                            //Vector2 aVel = con.A.GetVelocityOfPoint(con.pointA);
                            //Vector2 bVel = con.B.GetVelocityOfPoint(con.pointB);
                            //Vector2 relVel = bVel - aVel;
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
