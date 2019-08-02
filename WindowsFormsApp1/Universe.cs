using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class Universe
    {
        public bool useRelative;
        private List<Body> bodies;

        public Universe(bool relative = false)
        {
            useRelative = relative;
            bodies = new List<Body>();
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

        public static Universe GenerateSampleUniverse(bool useRel = false)
        {
            Universe universe = new Universe(useRel);
            if (universe.useRelative)
            {
                RelativeBody sr = new RelativeBody(100, "Sol");
                RelativeBody jr = new RelativeBody(200, 100, sr, 1, lbl: "Jool");
                RelativeBody tr = new RelativeBody(20, 2, jr, 0.01, lbl: "Tylo");
                RelativeBody ast = new RelativeBody(3, -1, tr, 0.05, lbl: "Ast");

                universe.AddBody(sr);
                universe.AddBody(jr);
                universe.AddBody(tr);
                universe.AddBody(ast);
            }
            else
            {
                Body s = new Body(100, lbl: "Sol");
                Body j = new Body(200, 100, s, 1, lbl: "Jool");
                Body t = new Body(20, 2, j, 0.01, lbl: "Tylo");
                Body a = new Body(3, -1, t, 0.05, lbl: "Ast");

                universe.AddBody(s);
                universe.AddBody(j);
                universe.AddBody(t);
                universe.AddBody(a);
            }

            return universe;
        }
    }
}
