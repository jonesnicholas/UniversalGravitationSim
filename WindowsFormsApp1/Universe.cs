using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class Universe
    {
        public bool useRelative;
        private List<Body> bodies;
        public double G;

        public Universe(bool relative = false, double inG = 1.0)
        {
            useRelative = relative;
            bodies = new List<Body>();
            G = inG;
        }

        public void AddBody(Body body)
        {
            //TODO: Properly handle cases where relative state of body doesn't match relative state of universe
            if (!bodies.Contains(body))
            {
                bodies.Add(body);
            }
        }

        public List<Body> GetBodies()
        {
            return bodies;
        }

        public void toggleRelative()
        {
            //TODO: This is gonna be tricky...
        }

        

        public void HeirarchicalSort()
        {
            if (!useRelative)
            {
                return;
            }
            RelativeBody center = (RelativeBody) bodies.First();
            List<Body> sorted = new List<Body>();
            RecursiveAdd(center, sorted);
            bodies = sorted;
        }

        public void RecursiveAdd(RelativeBody body, List<Body> list)
        {
            list.Add(body);
            body.children = body.children.OrderBy(child => child.p.mag()).ToList();
            foreach(RelativeBody child in body.children)
            {
                RecursiveAdd(child, list);
            }
        }

        public void PrintUniverse()
        {
            Debug.WriteLine("Universe");
            if (useRelative)
            {
                foreach(RelativeBody body in bodies)
                {
                    Debug.WriteLine($"{body.name}: {body.a} {body.GetAbsV()} {body.GetAbsP()}");
                }
            }
            else
            {
                foreach (Body body in bodies)
                {
                    Debug.WriteLine($"{body.name}: {body.a} {body.v} {body.p}");
                }
            }
            Debug.WriteLine("============");
        }
    }
}
