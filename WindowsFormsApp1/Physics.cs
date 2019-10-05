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

        public Physics()
        {
            
        }

        /// <summary>
        /// Update all elements in universe over a timestep of dt
        /// </summary>
        /// <param name="universe">A reference to the universe to be updated</param>
        /// <param name="dt">the timestep of the update</param>
        public void Update(ref Universe universe, double dt)
        {
            CalcBodyTrajectory(universe, dt);
            //checkCollision(universe);
            UpdateBodies(universe);
            UpdateReferenceFramesIfNecessary(universe);
            //removeFlaggedObjects(ref universe);
            FixBarycenter(universe);
        }

        /// <summary>
        /// Determines the new P and V for each body in the universe
        /// </summary>
        /// <param name="universe">The Universe to update</param>
        /// <param name="dt">the timestep of the update</param>
        internal void CalcBodyTrajectory(Universe universe, double dt)
        {
            if (parallel)
            {
                Parallel.ForEach(universe.GetBodies(), body =>
                {
                    UpdateEuler(universe, body, dt);
                });
            }
            else
            {
                if (universe.useRelative)
                {
                    foreach (Body body in universe.GetBodies())
                    {
                        UpdateEulerV(universe, body, dt);
                    }
                    CorrectForMovingReferenceFrames(universe, dt);
                    foreach (Body body in universe.GetBodies())
                    {
                        UpdateEulerP(body, dt);
                    }
                }
                else
                {
                    foreach (Body body in universe.GetBodies())
                    {
                        UpdateEuler(universe, body, dt);
                    }
                }
            }
        }

        /// <summary>
        /// For each body in universe, set the current P and V values to be the calculated 'next' values
        /// </summary>
        /// <param name="universe">Universe to update</param>
        internal void UpdateBodies(Universe universe)
        {
            foreach (Body body in universe.GetBodies())
            {
                body.UpdateBodyPV();
            }
        }

        internal void RemoveFlaggedObjects(ref Universe universe)
        {
            //TODO: Do actual removal intelligently in the universe class
            //universe = universe.Where(body => !body.deletionFlag).ToList();
        }

        internal void CheckCollision(Universe universe)
        {
            //TODO: rework, verify, and find a way to intelligently use spatial partitioning
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

        /// <summary>
        /// calculate the 'next' velocity of the body using the acceleration, and an Euler numerical approximation
        /// </summary>
        /// <param name="universe">the universe the body is in, used to determine gravitational acceleration</param>
        /// <param name="body">the body to update</param>
        /// <param name="dt">the timestep of the update</param>
        internal void UpdateEulerV(Universe universe, Body body, double dt)
        {
            Vector a = Acceleration(universe, body);
            body.a = a;
            body.vNext = body.v + a * dt;
        }

        /// <summary>
        /// calculate the 'next' position of the body using the computed velocity, and an Euler numerical approximation
        /// </summary>
        /// <param name="body">the body to update</param>
        /// <param name="dt">the timestep of the update</param>
        internal void UpdateEulerP(Body body, double dt)
        {
            body.pNext = body.p + body.vNext * dt;
        }

        /// <summary>
        /// calculate the 'next' position and velocity of the body using the acceleration and computed velocity, 
        /// and an Euler numerical approximation
        /// </summary>
        /// <param name="universe">the universe the body is in, used to calculate acceleration</param>
        /// <param name="body">the body to update</param>
        /// <param name="dt">the timestep of the update</param>
        internal void UpdateEuler(Universe universe, Body body, double dt)
        {
            Vector a = Acceleration(universe, body);
            body.a = a;
            body.vNext = body.v + a * dt;
            body.pNext = body.p + body.vNext * dt;
        }

        /// <summary>
        /// Calculate the acceleration vector of a given body due to the other bodies in a given universe
        /// </summary>
        /// <param name="universe">the universe containing the bodies imparting gravitational force</param>
        /// <param name="body">the body to calculate acceleration for</param>
        /// <returns>the calculated acceleration vector</returns>
        internal Vector Acceleration(Universe universe, Body body)
        {
            Vector a = new Vector(0, 0, 0);
            foreach (Body other in universe.GetBodies())
            {
                if (other == body)
                {
                    continue;
                }
                Vector dist = Distance(body, other, universe.useRelative);
                a += universe.G * other.m * dist.Normal() / (Math.Pow(dist.Mag(), 2));
            }
            return a;
        }
        /// <summary>
        /// Gets the 'distance' vector from a to b
        /// </summary>
        /// <param name="a">origin body</param>
        /// <param name="b">destination body</param>
        /// <param name="relativeMeasurements">bool determining if these bodies are using 'relative measurements'</param>
        /// <returns>the vector from A's position to B's position</returns>
        internal Vector Distance(Body a, Body b, bool relativeMeasurements)
        {
            // TODO: Look into making this a method within the Body classes, doesn't really need to be here
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

        /// <summary>
        /// Adjusts the position of all bodies in universe such that the barycenter is kept as close to the origin as possible
        /// </summary>
        /// <param name="universe"></param>
        internal void FixBarycenter(Universe universe)
        {
            //TODO: consider setting 'barycenter velocity' to zero here as well
            foreach (Body body in universe.GetBodies())
            {
                if (body.pinned)
                {
                    return; //if there are any pinned bodies, then we shouldn't need to do this (?)
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

        /// <summary>
        /// When using reference frame modes, the fact that reference frames can accelerate needs to be corrected for
        /// </summary>
        /// <param name="universe">universe to correct</param>
        /// <param name="dt">timestep used</param>
        internal void CorrectForMovingReferenceFrames(Universe universe, double dt)
        {
            // need to adjust the position/velocity vectors to account for the fact that reference frames are moving and accelerating.
            RelativeBody center = (RelativeBody) universe.GetBodies().First(); //TODO: rewrite to handle binary-style cases
            center.correctForMovingReferenceFrames(dt);
        }

        /// <summary>
        /// Determine if any Bodies have moved far enough away from their parent, or close enough to a 'better' parent to justify updating their reference frame
        /// </summary>
        /// <param name="universe">The universe to consider for required reference frame updates</param>
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
