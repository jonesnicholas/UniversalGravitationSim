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
        public double interval = 1.0;
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

            physics = new Physics();
        }

        public void update()
        {
            double dt = interval / simDegree;
            for (int i=0; i<simDegree; i++)
            {
                physics.update(universe, dt);
            }
        }

        public void render(PaintEventArgs e)
        {
            renderEngine.renderUniverse(universe,formWindow,e);
        }
    }
}
