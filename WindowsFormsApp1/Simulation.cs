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
        public double desiredTimeDilation = 20.0;
        public FormWindow formWindow;

        public Simulation(RenderEngine rEng = null)
        {
            renderEngine = rEng ?? new RenderEngine();
        }

        public void initialize(Universe uni = null, bool relative = false)
        {
            universe = uni == null ? UniverseGenerator.GenerateTestUniverse(relative) : uni;
            physics = new Physics();
            renderEngine.BaseScaleForUniverse(formWindow, universe);
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
                physics.Update(ref universe, dt);
                renderEngine.updateCount++;
            }
            double timeInterval = (start - DateTime.Now).TotalSeconds;
            //Debug.WriteLine(timeInterval);
        }

        public void step()
        {
            //universe.PrintUniverse();
            bool hold = play;
            play = true;
            update();
            play = hold;
            //universe.PrintUniverse();
        }

        public void render(PaintEventArgs e)
        {
            renderEngine.runRenderEngine(this, formWindow,e);
        }
    }
}
