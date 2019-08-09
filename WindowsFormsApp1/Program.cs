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
            sim.formWindow = mainWindow;
            Universe test = Universe.GenPseudoRandomUniverse(seed: 2);
            sim.initialize(test);
            //sim.initialize(relative: true);
            mainWindow.Size = new Size(400, 300);
            Application.Run(mainWindow);
        }
    }
}
