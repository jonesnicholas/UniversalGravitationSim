using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            Simulation simRel = new Simulation();
            simRel.initialize(relative: true);
            Simulation simNotRel = new Simulation();
            simNotRel.initialize(relative: false);
            Assert.AreEqual(simRel.universe.GetBodies().Count, simNotRel.universe.GetBodies().Count);
            List<RelativeBody> relBodies = simRel.universe.GetBodies().Select(body => (RelativeBody) body).ToList();
            List<Body> nonrelBodies = simNotRel.universe.GetBodies();
            for (int i = 0; i < simRel.universe.GetBodies().Count; i++)
            {
                Assert.IsTrue(relBodies[i].GetAbsP() == nonrelBodies[i].p);
                Assert.IsTrue(relBodies[i].GetAbsV() == nonrelBodies[i].v);
                Assert.IsTrue(relBodies[i].a == nonrelBodies[i].a);
            }
        }

        [TestMethod]
        public void Scenario_Verify_RelativeToNonRelativeSimulation_OneStep()
        {
            Simulation simRel = new Simulation();
            simRel.initialize(relative: true);
            simRel.simDegree = 1;
            simRel.step();
            Simulation simNotRel = new Simulation();
            simNotRel.initialize(relative: false);
            simNotRel.simDegree = 1;
            simNotRel.step();

            List<RelativeBody> relBodies = simRel.universe.GetBodies().Select(body => (RelativeBody)body).ToList();
            List<Body> nonrelBodies = simNotRel.universe.GetBodies();

            Assert.AreEqual(relBodies.Count, nonrelBodies.Count);

            for (int i = 0; i < relBodies.Count; i++)
            {
                if (relBodies[i].a != nonrelBodies[i].a)
                {
                    Assert.Fail($"Non-matching accelerations {relBodies[i].a} {nonrelBodies[i].a}");
                }
                if (relBodies[i].GetAbsP() != nonrelBodies[i].p)
                {
                    Assert.Fail($"Non-matching positions {relBodies[i].GetAbsP()} {nonrelBodies[i].p}");
                }
                if (relBodies[i].GetAbsV() != nonrelBodies[i].v)
                {
                    simNotRel.universe.PrintUniverse();
                    simRel.universe.PrintUniverse();
                    Assert.Fail($"Non-matching velocities {relBodies[i].GetAbsV()} {nonrelBodies[i].v}");
                }
            }
        }
    }
}
