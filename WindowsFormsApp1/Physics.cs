using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class Physics
    {
        public void update(List<Body> universe, double dt)
        {
            foreach (Body body in universe)
            {
                updateEuler(universe, body, dt);
            }
            //checkCollision(universe);
            foreach (Body body in universe)
            {
                body.update();
            }
            universe = universe.Where(body => !body.deletionFlag).ToList();
            fixBarycenter(universe);
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
                Vector dist = other.p - body.p;
                a += other.m * dist.normal() / (Math.Pow(dist.m, 2));
            }
            return a;
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
