using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class UniverseGenerator
    {
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
                    if (host.parentDepth() > nestFactor - 1)
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
                        mag = host.p.Mag() * Math.Pow(host.m / (3 * host.parent.m), 0.333) / 4.0;
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
                        name = $"{host.name.Substring(0, host.name.Length - 1)}{host.children.Count() + 1}";
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
            uni.HeirarchicalSort();
            return uni;
        }

        public static Universe GenerateTwoBody(bool useRel = false)
        {
            Universe universe = new Universe(useRel);
            if (useRel)
            {
                RelativeBody center = new RelativeBody(100, "Center");
                RelativeBody orbit = new RelativeBody(100, 0, center, 10, lbl: "Orbiter");
                universe.AddBody(center);
                universe.AddBody(orbit);
            }
            else
            {
                Body center = new Body(100, lbl:"Center");
                Body orbit = new Body(100, 0, center, 10, lbl: "Orbiter");
                universe.AddBody(center);
                universe.AddBody(orbit);
            }
            return universe;
        }

        public static Universe GenerateTestUniverse(bool useRel = false)
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

        public static Universe PseudoRealSolarSystem(bool useRel = false)
        {
            //TODO: Try to scrape this from ephemeris data
            Universe universe = new Universe(useRel, 6.67e-11);
            if (useRel)
            {
                RelativeBody sol = new RelativeBody(
                    new Vector(-3.91242e8, 1.13826e9, -1.36642e6),
                    null,
                    1988500e24,
                    new Vector(-1.46227e1, -7.226575e-1, 3.855625e-1),
                    1.408,
                    "Sol");
                universe.AddBody(sol);

                RelativeBody mer = new RelativeBody(
                    new Vector(3.7265248e10,2.99191857e10,-9.73742642e8),
                    sol,
                    3.302e23,
                    new Vector(-4.0081238e4,4.00785892165e4,6.95187127e3),
                    5.427,
                    "Mercury");
                universe.AddBody(mer);

                RelativeBody ven = new RelativeBody(
                    new Vector(-8.285427e10,6.823787e10,5.7176105e9),
                    sol,
                    48.685e23,
                    new Vector(-2.239593e4,-2.72111445e4,9.19038772e2),
                    5.204,
                    "Venus");
                universe.AddBody(ven);

                RelativeBody ear = new RelativeBody(
                    new Vector(1.1724955e11,-9.6037042e10,4.240154172e6),
                    sol,
                    5.97219e24,
                    new Vector(1.838051573e4,2.292561645e4,-4.80362870085e-2),
                    5.51,
                    "Earth");
                universe.AddBody(ear);

                RelativeBody mar = new RelativeBody(
                    new Vector(-2.176527e11,1.2096152e11,7.8749526e9),
                    sol,
                    6.4171e23,
                    new Vector(-1.0862549e4,-1.910899192e4,-1.338880148e2),
                    3.933,
                    "Mars");
                universe.AddBody(mar);

                RelativeBody jup = new RelativeBody(
                    new Vector(-7.7324016329e10,-7.85692429e11,4.9935554e9),
                    sol,
                    1898.13e24,
                    new Vector(1.28574958e4,-6.647244023e2,-2.84976624e2),
                    1.326,
                    "Jupiter");
                universe.AddBody(jup);

                RelativeBody sat = new RelativeBody(
                    new Vector(4.646603288e11,-1.429337911e12,6.35272334e9),
                    sol,
                    5.6834e26,
                    new Vector(8.6658972e3,2.95812489e3,-3.9639882e2),
                    0.687,
                    "Saturn");
                universe.AddBody(sat);

                RelativeBody ura = new RelativeBody(
                    new Vector(2.474143379e12,1.6380446e12,-2.595844e10),
                    sol,
                    86.813e24,
                    new Vector(-3.797021e3,5.360144026e3,6.888360866e1),
                    1.271,
                    "Uranus");
                universe.AddBody(ura);

                RelativeBody nep = new RelativeBody(
                    new Vector(4.3604752e12,-1.017071714e12,-7.955997546e10),
                    sol,
                    102.413e24,
                    new Vector(1.2111618e3,5.3261598e3,-1.3733646447e2),
                    1.638,
                    "Neptune");
                universe.AddBody(nep);

                RelativeBody mon = new RelativeBody(
                    new Vector(2.245235455e8,-3.335982581e8,-1.0562881438814e7),
                    ear,
                    7.349e22,
                    new Vector(8.23519443721e2,5.26278905202e2,-8.574012751e1),
                    3.3437,
                    "Moon");
                universe.AddBody(mon);

            }
            else
            {

            }
            return universe;
        }
    }
}
