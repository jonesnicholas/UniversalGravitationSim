using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class Simulation
    {
        public Universe universe;
        public Physics physics;
        internal RenderEngine renderEngine;
        public bool play = false;
        public double simDegree = 100.0;
        public double interval = 0.01;
        public double desiredTimeDilation = 200.0;
        public FormWindow formWindow;

        public Simulation(RenderEngine rEng = null)
        {
            renderEngine = rEng ?? new RenderEngine();
        }

        public void initialize(Universe uni = null, bool relative = false)
        {
            universe = uni == null ? Universe.GenerateSampleUniverse(relative) : uni;
            physics = new Physics();
        }

        public void update()
        {
            DateTime start = DateTime.Now;
            double dt = interval*desiredTimeDilation / simDegree;
            if (!play)
            {
                return;
            }
            for (int i=0; i<simDegree; i++)
            {
                physics.update(ref universe, dt);
                renderEngine.updateCount++;
            }
            double timeInterval = (start - DateTime.Now).TotalSeconds;
            //Debug.WriteLine(timeInterval);
        }

        public void step()
        {
            printUniverse();
            bool hold = play;
            play = true;
            update();
            play = hold;
            printUniverse();
        }

        public void render(PaintEventArgs e)
        {
            renderEngine.runRenderEngine(universe,formWindow,e);
        }

        public void printUniverse()
        {
            Debug.WriteLine("Universe");
            foreach (Body body in universe.GetBodies())
            {
                if (universe.useRelative)
                {
                    RelativeBody relBod = body as RelativeBody;
                    //Debug.WriteLine($"{relBod.name}: P:({relBod.GetAbsP().x},{relBod.GetAbsP().y}) V:({relBod.GetAbsV().x},{relBod.GetAbsV().y} A:{relBod.a})");
                    Debug.WriteLine($"{relBod.name}: P:({relBod.GetAbsP().x},{relBod.GetAbsP().y}) V:({relBod.GetAbsV().x},{relBod.GetAbsV().y})");
                }
                else
                {
                    Debug.WriteLine($"{body.name}: P:({body.p.x},{body.p.y}) V:({body.v.x},{body.v.y})");
                }
            }
            Debug.WriteLine("========");
        }
    }
}
