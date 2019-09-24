using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Simulation sim = new Simulation();
            FormWindow mainWindow = new FormWindow(sim);
            mainWindow.Size = new Size(400, 300);
            sim.formWindow = mainWindow;
            //Universe test = UniverseGenerator.GenPseudoRandomUniverse(seed: 1);
            Universe rss = UniverseGenerator.PseudoRealSolarSystem(true);
            sim.initialize(rss);
            //sim.initialize(relative: true);
            //Universe small = UniverseGenerator.GenerateTwoBody(true);
            //sim.initialize(small);
            Application.Run(mainWindow);
        }
    }
}
