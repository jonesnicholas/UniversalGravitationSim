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
                RelativeBody tr = new RelativeBody(10, 2, jr, 0.1, lbl: "Tylo");
                RelativeBody ast = new RelativeBody(1, -1, tr, 0.05, lbl: "Ast");

                universe.AddBody(sr);
                universe.AddBody(jr);
                universe.AddBody(tr);
                universe.AddBody(ast);
            }
            else
            {
                Body s = new Body(100, lbl: "Sol");
                Body j = new Body(200, 100, s, 1, lbl: "Jool");
                Body t = new Body(10, 2, j, 0.1, lbl: "Tylo");
                Body a = new Body(1, -1, t, 0.05, lbl: "Ast");

                universe.AddBody(s);
                universe.AddBody(j);
                universe.AddBody(t);
                universe.AddBody(a);
            }

            return universe;
        }

        public static Universe GenPseudoRandomUniverse(
            bool relative = true,
            int seed = 0,
            int count = 20,
            int nestFactor = 2,
            double mScale = 100.0,
            double mRatio = 1000.0,
            double dScale = 500.0)
        {
            Random random = seed == 0 ? new Random() : new Random(seed);
            Universe uni = new Universe(relative);
            double avgSplitFactor = Math.Log(count, nestFactor);
            int total = 1;
            if (relative)
            {
                RelativeBody center = new RelativeBody(mScale, "0_*");
                uni.AddBody(center);
                int continues = 0;
                while (total < count)
                {
                    RelativeBody host = (RelativeBody)uni.GetBodies()[random.Next(0, uni.GetBodies().Count())];
                    if (host.parentDepth() > nestFactor - 1 )
                    {
                        continues++;
                        if (continues < 4)
                        {
                            continue;
                        }
                        host = center;
                    }
                    continues = 0;
                    double mass = (host.m / mRatio) * Math.Pow(2, -host.children.Count());
                    //double mag = random.NextDouble() * dScale * host.m / mScale;
                    double mag;
                    if (host == center)
                    {
                        mag = dScale;
                    }
                    else
                    {
                        mag = host.p.mag() * Math.Pow(host.m / (3 * host.parent.m), 0.333) / 4.0;
                    }
                    mag *= random.NextDouble();
                    double theta = random.NextDouble() * Math.PI * 2;
                    Vector distance = new Vector(mag * Math.Cos(theta), mag * Math.Sin(theta), 0);

                    string name;
                    if (host == center)
                    {
                        name = $"{center.children.Count() + 1}_*";
                    }
                    else if (host.parentDepth() == 1)
                    {
                        name = $"{host.name.Substring(0, host.name.Length-1)}{host.children.Count() + 1}";
                    }
                    else
                    {
                        name = $"{host.name}_{host.children.Count() + 1}";
                    }

                    RelativeBody addition = new RelativeBody(distance, host, mass, lbl: $"{name}");
                    uni.AddBody(addition);
                    total++;
                }
            }

            return uni;
        }
    }
}
