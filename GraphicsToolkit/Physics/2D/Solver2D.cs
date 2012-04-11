//Special thanks to Paul Firth:
//http://www.wildbunny.co.uk/blog/2011/03/25/speculative-contacts-an-continuous-collision-engine-approach-part-1/

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
            float muD = 0.7f;

            bool SpecSequential = true;
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
                        //double remove = relNv + con.m_dist / Constants.kTimeStep(1/30);
                        if (remove < 0)
                        {
                            Vector2 aVel = con.A.GetVelocityOfWorldPoint(con.pointA);
                            Vector2 bVel = con.B.GetVelocityOfWorldPoint(con.pointB);
                            Vector2 relVel = bVel - aVel;

                            float mag = remove * con.InvDenom;
                            
                            Vector2 imp = con.Normal * mag;

                            Vector2 t = relVel-(Vector2.Dot(relVel,n))*n;
                            if (!(t.Length() <= float.Epsilon))
                            {
                                t = Vector2.Normalize(t);
                            }

                            Vector2 frictionJ = -(muD * Math.Abs(mag)) *t;

                            con.ApplyImpulses(imp-frictionJ);
                        }
                    }
                    else
                    {
                        Vector2 aVel = con.A.GetVelocityOfWorldPoint(con.pointA);
                        Vector2 bVel = con.B.GetVelocityOfWorldPoint(con.pointB);
                        Vector2 relVel = bVel - aVel;

                        float remove = relNv + con.Dist * dtInv;

                        float mag = remove * con.InvDenom;
                        float newImpulse = Math.Min(mag + con.ImpulseN, 0);
                        float change = newImpulse - con.ImpulseN;

                        Vector2 imp = con.Normal * change;

                        // apply impulse
                        con.ApplyImpulses(imp);
                        con.ImpulseN = newImpulse;

                        float absMag = Math.Abs(con.ImpulseN) * muD;

                        // friction
                        mag = Vector2.Dot(relVel, GameMath.Perp2D(n)) * con.InvDenom;
                        newImpulse = MathHelper.Clamp(mag + con.ImpulseT, -absMag, absMag);
                        change = newImpulse - con.ImpulseT;
                        imp = GameMath.Perp2D(n) * change;

                        // apply impulse
                        con.ApplyImpulses(imp);
                        con.ImpulseT = newImpulse;
                    }
                }
            }
        }
    }
}
