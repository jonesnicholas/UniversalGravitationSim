using System;
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
        bool useRelative;

        public Physics(bool useRel)
        {
            useRelative = useRel;
        }

        public void update(ref List<Body> universe, double dt)
        {
            calcBodyTrajectory(universe, dt);
            //checkCollision(universe);
            if (useRelative)
            {
                correctForMovingReferenceFrames(universe.Select(body => (RelativeBody)body).ToList(), dt);
            }
            updateBodies(universe);
            removeFlaggedObjects(ref universe);
            //fixBarycenter(universe);
        }

        internal void calcBodyTrajectory(List<Body> universe, double dt)
        {
            if (parallel)
            {
                Parallel.ForEach(universe, body =>
                {
                    updateEuler(universe, body, dt);
                });
            }
            else
            {
                foreach (Body body in universe)
                {
                    updateEuler(universe, body, dt);
                }
            }
        }

        internal void updateBodies(List<Body> universe)
        {
            foreach (Body body in universe)
            {
                body.update();
            }
        }

        internal void removeFlaggedObjects(ref List<Body> universe)
        {
            universe = universe.Where(body => !body.deletionFlag).ToList();
        }

        internal void checkCollision(List<Body> universe)
        {
            for (int i=0; i<universe.Count(); i++)
            {
                Body a = universe[i];
                if (a.deletionFlag)
                {
                    continue;
                }
                for (int j=i+1; j<universe.Count(); j++)
                {
                    Body b = universe[j];
                    if (b.deletionFlag)
                    {
                        continue;
                    }
                    Vector pRel = a.pNext - b.pNext;
                    double r = a.r + b.r;
                    if (pRel.mag() < r)
                    {
                        b.deletionFlag = true;
                        a.vNext = (a.m * a.vNext + b.m * b.vNext) / (a.m + b.m);
                        a.rho = (a.m + b.m) / ((a.m / a.rho) + (b.m / b.rho));
                        a.m += b.m;
                        a.estR();
                    }
                }
            }
        }

        internal void updateEuler(List<Body> universe, Body body, double dt)
        {
            Vector a = acceleration(universe, body, body.p);
            body.a = a;
            body.vNext = body.v + a * dt;
            body.pNext = body.p + body.v * dt;
        }

        internal Vector acceleration(List<Body> universe, Body body, Vector position)
        {
            Vector a = new Vector(0, 0, 0);
            foreach (Body other in universe)
            {
                if (other == body)
                {
                    continue;
                }
                Vector dist = distance(body,other);
                //Debug.WriteLine(dist.ToString());
                a += other.m * dist.normal() / (Math.Pow(dist.mag(), 2));
            }
            //Debug.WriteLine(body.name + a.ToString());
            return a;
        }

        internal Vector distance(Body a, Body b)
        {
            Vector dist = new Vector();
            if (a == b)
            {
                return dist;
            }
            if (useRelative)
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

        internal void fixBarycenter(List<Body> universe)
        {
            //TODO: correct eqns for relative mode
            foreach (Body body in universe)
            {
                if (body.pinned)
                {
                    return;
                }
            }
            if (useRelative)
            {
                Vector weightedP = new Vector();
                double mass = 0;
                Body center = universe[0]; //TODO: Fix for cases where multiple bodies have "null" as parent e.g binary stars
                Vector barycenter = (center as RelativeBody).GetFamilyBarycenter();

                Debug.WriteLine($"bary: {barycenter.ToString()}");
                center.p -= barycenter;
            }
            else
            {
                Vector weightedP = new Vector();
                double mass = 0;
                foreach (Body body in universe)
                {
                    weightedP += body.m * body.p;
                    mass += body.m;
                }
                weightedP /= mass;
                Debug.WriteLine($"bary: {weightedP.ToString()}");
                foreach (Body body in universe)
                {
                    body.p -= weightedP;
                }
            }
        }

        internal void correctForMovingReferenceFrames(List<RelativeBody> universe, double dt)
        {
            // need to adjust the position/velocity vectors to account for the fact that reference frames are moving and accelerating.
            RelativeBody center = universe[0]; //TODO: rewrite to handle binary-style cases
            center.correctForMovingReferenceFrames(dt);
        }
    }
}
