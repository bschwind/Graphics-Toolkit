﻿//Special thanks to Paul Firth:
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
                            con.A.InContact = true;
                            con.B.InContact = true;

                            con.A.Normal = con.Normal;
                            con.B.Normal = con.Normal;
                        }
                    }
                    else
                    {
                        Vector3 aVel = con.A.GetVelocityOfWorldPoint(con.pointA);
                        Vector3 bVel = con.B.GetVelocityOfWorldPoint(con.pointB);
                        Vector3 relVel = bVel - aVel;

                        float remove = relNv + con.Dist * dtInv;

                        //float mag = remove * con.InvDenom;
                        float mag = remove * (1/(con.A.InvMass + con.B.InvMass));
                        float newImpulse = Math.Min(mag + con.ImpulseN, 0);
                        float change = newImpulse - con.ImpulseN;

                        if (change < 0.0f)
                        {
                            con.A.InContact = true;
                            con.B.InContact = true;

                            con.A.Normal = con.Normal;
                            con.B.Normal = con.Normal;
                        }

                        Vector3 imp = con.Normal * change;

                        // apply impulse
                        con.ApplyImpulses(imp);
                        con.ImpulseN = newImpulse;
                    }
                }
            }
        }
    }
}
