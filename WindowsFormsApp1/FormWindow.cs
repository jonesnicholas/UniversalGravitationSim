using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FormWindow : Form
    {
        readonly Timer frameTimer = new Timer();
        internal Simulation simulation;

        public FormWindow(Simulation sim)
        {
            this.simulation = sim;
            this.InitializeComponent();
            this.frameTimer.Tick += new EventHandler(this.frame_Timer_Tick);
            double targetFPS = 120;
            this.frameTimer.Interval = (int)Math.Ceiling(1000.0/targetFPS);
            this.frameTimer.Enabled = true;
            this.frameTimer.Start();
            DoubleBuffered = true;
            this.MouseWheel += formWindow_MouseWheel;
            this.KeyPress += formWindow_KeyPressed;
            this.Focus();
        }

        void frame_Timer_Tick(object sender, EventArgs e)
        {
            if (!simulation.play)
            {
                this.Invalidate();
            }
        }

        private void formWindow_MouseWheel(object sender, MouseEventArgs e)
        {
            simulation.renderEngine.scroll(e.Delta);
        }

        private void formWindow_KeyPressed(object sender, KeyPressEventArgs e)
        {
            Debug.WriteLine(e.KeyChar);
            if (e.KeyChar == '0')
            {
                simulation.renderEngine.focusNext(simulation.universe);
                Debug.WriteLine("Attempting switch");
            }
            if (e.KeyChar == ' ')
            {
                simulation.play = !simulation.play;
            }
            if (e.KeyChar == '-')
            {
                simulation.simDegree = simulation.simDegree == 1 ? 1 : simulation.simDegree - 1;
                Debug.WriteLine("simDegree: " + simulation.simDegree);
            }
            if (e.KeyChar == '+')
            {
                simulation.simDegree++;
                Debug.WriteLine("simDegree: " + simulation.simDegree);
            }
        }
    }
}
