﻿using System;
using System.Collections.Generic;
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

        public void update(List<Body> universe, double dt)
        {
            calcBodyTrajectory(universe, dt);
            //checkCollision(universe);
            updateBodies(universe);
            removeFlaggedObjects(universe);
            fixBarycenter(universe);
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

        internal void removeFlaggedObjects(List<Body> universe)
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
            body.vNext = body.v + a * dt;
            body.pNext = body.p + body.vNext * dt;
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
                a += other.m * dist.normal() / (Math.Pow(dist.m, 2));
            }
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

                RelativeBody mutualParent = getMutualParent(ra, rb);
                dist = distanceFromParent(rb, mutualParent) - distanceFromParent(ra, mutualParent);
            }
            else
            {
                dist = b.p - a.p;
            }
            return dist;
        }

        internal RelativeBody getMutualParent(RelativeBody a, RelativeBody b)
        {
            int depthA = a.parentDepth();
            int depthB = b.parentDepth();
            RelativeBody aParent = a.parent;
            RelativeBody bParent = b.parent;
            while (depthA > depthB)
            {
                depthA--;
            }
            while (depthB > depthA)
            {
                depthB--;
            }
            while (aParent != bParent)
            {
                aParent = aParent.parent;
                bParent = bParent.parent;
            }
            return aParent;
        }

        internal Vector distanceFromParent(RelativeBody body, RelativeBody parent)
        {
            RelativeBody par = body.parent;
            Vector relDis = body.p;
            while (par != parent)
            {
                relDis += par.p;
                par = par.parent;
            }
            return relDis;
        }

        internal void fixBarycenter(List<Body> universe)
        {
            foreach (Body body in universe)
            {
                if (body.pinned)
                {
                    return;
                }
            }
            Vector weightedP = new Vector();
            double mass = 0;
            foreach (Body body in universe)
            {
                weightedP += body.m * body.p;
                mass += body.m;
            }
            weightedP /= mass;
            foreach (Body body in universe)
            {
                body.p -= weightedP;
            }
        }
    }
}