using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class Simulation
    {
        public List<Body> universe;
        public Physics physics;
        internal RenderEngine renderEngine;
        public bool play = false;
        public int simDegree = 1;
        public FormWindow formWindow;

        public Simulation(RenderEngine rEng)
        {
            renderEngine = rEng ?? new RenderEngine();
        }

        public void initialize()
        {
            Body center = new Body(0, 0, 0, 0, 100,1, true);
            Body a = new Body(200,0,center,1,10);
            //Body b = new Body(105, 0, 0, 1 + Math.Sqrt(0.2), 0.1);

            Body c1 = new Body(10, 0, 0, 0, 100);
            Body c2 = new Body(-10, 0, 0, 0, 10);

            Body s = new Body(0, 0, 0, 0, 100);
            Body j = new Body(200, 0, s, 1);
            Body t = new Body(5, 0, j, 0.01);
            Body l = new Body(10, 0, j, 0.05);
            

            //universe = new List<Body>() { center, a, b };
            //universe = new List<Body>() { center, a };
            //universe = new List<Body>() { c1,c2 };
            universe = new List<Body>() { s, j, t, l};

            /*int numAs = 100;
            Random random = new Random();
            for (int i=0; i<numAs; i++)
            {
                double theta = random.NextDouble() * 4 * Math.Acos(0);
                double r = random.NextDouble() * 10 + 110;
                Vector p0 = new Vector(r * Math.Cos(theta), r * Math.Sin(theta), 0);
                double vMag = Math.Sqrt(center.m / r) * (1 + 0.1 * (1.0 - 2.0 * random.NextDouble()));
                Vector v0 = new Vector(-p0.y, p0.x, 0);
                v0 = v0.normal() * vMag;

                v0 *= 0;

                Body atr = new Body(p0.x, p0.y, v0.x, v0.y, 1,0.1);
                universe.Add(atr);
            }*/

            physics = new Physics();
        }

        public void render(PaintEventArgs e)
        {
            renderEngine.renderUniverse(universe,formWindow,e);
        }
    }
}
