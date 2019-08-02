using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsFormsApp1;

namespace UnitTests
{
    [TestClass]
    public class ScenarioTests
    {
        [TestMethod]
        public void Scenario_Verify_RelativeToNonRelativeSimulation_Start()
        {
            Simulation simRel = new Simulation(useRel: true);
            simRel.initialize();
            Simulation simNotRel = new Simulation(useRel: false);
            simNotRel.initialize();
            Assert.AreEqual(simRel.universe.Count, simNotRel.universe.Count);
            for (int i = 0; i < simRel.universe.Count; i++)
            {
                Assert.IsTrue(((RelativeBody)simRel.universe[i]).GetAbsP() == simNotRel.universe[i].p);
                Assert.IsTrue(((RelativeBody)simRel.universe[i]).GetAbsV() == simNotRel.universe[i].v);
                Assert.IsTrue(((RelativeBody)simRel.universe[i]).a == simNotRel.universe[i].a);
            }
        }

        [TestMethod]
        public void Scenario_Verify_RelativeToNonRelativeSimulation_OneStep()
        {
            Simulation simRel = new Simulation(useRel: true);
            simRel.initialize();
            simRel.simDegree = 1;
            simRel.step();
            Simulation simNotRel = new Simulation(useRel: false);
            simNotRel.initialize();
            simNotRel.simDegree = 1;
            simNotRel.step();
            Assert.AreEqual(simRel.universe.Count, simNotRel.universe.Count);

            for (int i = 0; i < simRel.universe.Count; i++)
            {
                if (((RelativeBody)simRel.universe[i]).GetAbsP() != simNotRel.universe[i].p)
                {
                    Assert.Fail($"Non-matching positions {((RelativeBody)simRel.universe[i]).GetAbsP()} {simNotRel.universe[i].p}");
                }
                if (((RelativeBody)simRel.universe[i]).GetAbsV() != simNotRel.universe[i].v)
                {
                    Assert.Fail($"Non-matching velocities {((RelativeBody)simRel.universe[i]).GetAbsV()} {simNotRel.universe[i].v}");
                }
                if (((RelativeBody)simRel.universe[i]).a != simNotRel.universe[i].a)
                {
                    Assert.Fail($"Non-matching accelerations {((RelativeBody)simRel.universe[i]).a} {simNotRel.universe[i].a}");
                }
            }
        }
    }
}
