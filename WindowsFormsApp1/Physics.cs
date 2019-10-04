﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class Physics
    {
        public bool parallel = false;

        public Physics()
        {
            
        }

        public void update(ref Universe universe, double dt)
        {
            calcBodyTrajectory(universe, dt);
            //checkCollision(universe);
            updateBodies(universe);
            UpdateReferenceFramesIfNecessary(universe);
            //removeFlaggedObjects(ref universe);
            fixBarycenter(universe);
        }

        internal void calcBodyTrajectory(Universe universe, double dt)
        {
            if (parallel)
            {
                Parallel.ForEach(universe.GetBodies(), body =>
                {
                    updateEuler(universe, body, dt);
                });
            }
            else
            {
                if (universe.useRelative)
                {
                    foreach (Body body in universe.GetBodies())
                    {
                        updateEulerV(universe, body, dt);
                    }
                    correctForMovingReferenceFrames(universe, dt);
                    foreach (Body body in universe.GetBodies())
                    {
                        updateEulerP(universe, body, dt);
                    }
                }
                else
                {
                    foreach (Body body in universe.GetBodies())
                    {
                        updateEuler(universe, body, dt);
                    }
                }
            }
        }

        internal void updateBodies(Universe universe)
        {
            foreach (Body body in universe.GetBodies())
            {
                body.UpdateBodyPV();
            }
        }

        internal void removeFlaggedObjects(ref Universe universe)
        {
            //TODO: Do actual removal intelligently in the universe class
            //universe = universe.Where(body => !body.deletionFlag).ToList();
        }

        internal void checkCollision(Universe universe)
        {
            //TODO: rework, verify, and find a way to intelligently spatial partition
            List<Body> allBodies = universe.GetBodies();
            for (int i=0; i< allBodies.Count(); i++)
            {
                Body a = allBodies[i];
                if (a.deletionFlag)
                {
                    continue;
                }
                for (int j=i+1; j< allBodies.Count(); j++)
                {
                    Body b = allBodies[j];
                    if (b.deletionFlag)
                    {
                        continue;
                    }
                    Vector pRel = a.pNext - b.pNext;
                    double r = a.r + b.r;
                    if (pRel.Mag() < r)
                    {
                        b.deletionFlag = true;
                        a.vNext = (a.m * a.vNext + b.m * b.vNext) / (a.m + b.m);
                        a.rho = (a.m + b.m) / ((a.m / a.rho) + (b.m / b.rho));
                        a.m += b.m;
                        a.EstR();
                    }
                }
            }
        }

        internal void updateEulerV(Universe universe, Body body, double dt)
        {
            Vector a = acceleration(universe, body, body.p);
            body.a = a;
            body.vNext = body.v + a * dt;
        }

        internal void updateEulerP(Universe universe, Body body, double dt)
        {
            body.pNext = body.p + body.vNext * dt;
        }

        internal void updateEuler(Universe universe, Body body, double dt)
        {
            Vector a = acceleration(universe, body, body.p);
            body.a = a;
            body.vNext = body.v + a * dt;
            body.pNext = body.p + body.vNext * dt;
        }

        internal Vector acceleration(Universe universe, Body body, Vector position)
        {
            Vector a = new Vector(0, 0, 0);
            foreach (Body other in universe.GetBodies())
            {
                if (other == body)
                {
                    continue;
                }
                Vector dist = distance(body, other, universe.useRelative);
                //Debug.WriteLine(dist.ToString());
                a += universe.G * other.m * dist.Normal() / (Math.Pow(dist.Mag(), 2));
            }
            //Debug.WriteLine(body.name + a.ToString());
            return a;
        }

        internal Vector distance(Body a, Body b, bool relativeMeasurements)
        {
            Vector dist = new Vector();
            if (a == b)
            {
                return dist;
            }
            if (relativeMeasurements)
            {
                RelativeBody ra = (RelativeBody)a;
                RelativeBody rb = (RelativeBody)b;

                RelativeBody mutualParent = ra.getMutualParent(rb);
                dist = rb.distanceFromParent(mutualParent) - ra.distanceFromParent(mutualParent);
            }
            else
            {
                dist = b.p - a.p;
            }
            return dist;
        }

        internal void fixBarycenter(Universe universe)
        {
            //TODO: correct eqns for relative mode
            foreach (Body body in universe.GetBodies())
            {
                if (body.pinned)
                {
                    return;
                }
            }
            if (universe.useRelative)
            {
                Vector weightedP = new Vector();
                Body center = universe.GetBodies().First(); //TODO: Fix for cases where multiple bodies have "null" as parent e.g binary stars
                Vector barycenter = (center as RelativeBody).GetFamilyBarycenter();

                //Debug.WriteLine($"bary: {barycenter.ToString()}");
                center.p = -barycenter;
            }
            else
            {
                Vector weightedP = new Vector();
                double mass = 0;
                foreach (Body body in universe.GetBodies())
                {
                    weightedP += body.m * body.p;
                    mass += body.m;
                }
                weightedP /= mass;
                //Debug.WriteLine($"bary: {weightedP.ToString()}");
                foreach (Body body in universe.GetBodies())
                {
                    body.p -= weightedP;
                }
            }
        }

        internal void correctForMovingReferenceFrames(Universe universe, double dt)
        {
            // need to adjust the position/velocity vectors to account for the fact that reference frames are moving and accelerating.
            RelativeBody center = (RelativeBody) universe.GetBodies().First(); //TODO: rewrite to handle binary-style cases
            center.correctForMovingReferenceFrames(dt);
        }

        internal void UpdateReferenceFramesIfNecessary(Universe universe)
        {
            if (!universe.useRelative)
            {
                return; //non-relative mode doesn't use reference frames
            }

            foreach (RelativeBody body in universe.GetBodies())
            {
                // If body leaves Hill sphere of parent, set reference frame to grandparent.
                // TODO: Look into precomputing hill sphere radius
                if (body.parent != null && body.parent.parent != null)
                {
                    double hillRad = body.parent.p.Mag() * Math.Pow(body.parent.m / 3.0 / body.parent.parent.m, 1.0 / 3.0);
                    if (body.p.Mag() > hillRad)
                    {
                        body.p += body.parent.p;
                        body.v += body.parent.v;
                        (body.parent.parent as RelativeBody).AdoptChild(body);
                    }
                }
                // TODO: check to see if body has entered hill sphere of any more-massive siblings.

            }
        }
    }
}
