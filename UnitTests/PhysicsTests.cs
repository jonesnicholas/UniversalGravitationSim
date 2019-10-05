using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsFormsApp1;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class PhysicsTests
    {
        Random random = new Random();

        [TestMethod]
        public void Physics_Verify_Constructor()
        {
            Physics physics = new Physics();
            Assert.IsNotNull(physics);
        }

        [TestMethod]
        public void Physics_Verify_UpdateBodies()
        {
            List<bool> tfl = new List<bool>() { true, false };
            foreach (bool tf in tfl)
            {
                Physics physics = new Physics();
                Universe universe = new Universe(tf);
                List<Vector> newVects = new List<Vector>();
                int numBodies = 4;
                for (int i=0; i<numBodies; i++)
                {
                    Body b = tf ? new Body() : new RelativeBody();
                    b.p = Vector.RandVect();
                    Vector rVecNext = Vector.RandVect();
                    newVects.Add(rVecNext);
                    b.pNext = rVecNext;
                    universe.AddBody(b);
                }
                physics.UpdateBodies(universe);
                for (int i=0; i<numBodies; i++)
                {
                    Assert.IsTrue(newVects[i] == universe.GetBodies()[i].p);
                }
            }
        }

        [TestMethod]
        [Ignore] //ignoring this while I refactor
        public void Physics_Verify_RemoveFlaggedObjects()
        {
            List<bool> tfl = new List<bool>() { true, false };
            foreach (bool tf in tfl)
            {
                Physics physics = new Physics();
                Universe universe = new Universe(tf);
                int numBodies = 10;
                int countFlagged = 0;
                for (int i = 0; i < numBodies; i++)
                {
                    Body b = tf ? new Body() : new RelativeBody();
                    if (random.NextDouble() < 0.5)
                    {
                        b.deletionFlag = true;
                        countFlagged++;
                    }
                    universe.AddBody(b);
                }
                physics.RemoveFlaggedObjects(ref universe);
                Assert.AreEqual(numBodies - countFlagged, universe.GetBodies().Count);
            }
        }

        [TestMethod]
        public void Physics_Verify_Absolute_Distance()
        {
            Body bodyA = new Body();
            Vector vecA = Vector.RandVect();
            bodyA.p = vecA;

            Body bodyB = new Body();
            Vector vecB = Vector.RandVect();
            bodyB.p = vecB;

            Vector manDist = vecB - vecA;

            Physics physics = new Physics();
            Vector dist = physics.Distance(bodyA, bodyB, false);
            Assert.AreEqual(manDist.x, dist.x);
            Assert.AreEqual(manDist.y, dist.y);
            Assert.AreEqual(manDist.z, dist.z);
        }

        [TestMethod]
        public void Physics_Verify_Relative_Distance()
        {
            RelativeBody s = new RelativeBody();
            Vector p1p = Vector.RandVect();
            RelativeBody p1 = new RelativeBody(p1p,parentBody:s);
            Vector p1m1p = Vector.RandVect();
            RelativeBody p1m1 = new RelativeBody(p1m1p, parentBody:p1);
            Vector p2p = Vector.RandVect();
            RelativeBody p2 = new RelativeBody(p2p, parentBody:s);
            Vector p2m1p = Vector.RandVect();
            RelativeBody p2m1 = new RelativeBody(p2m1p, parentBody:p2);

            Physics physics = new Physics();

            Vector p1Dp2 = physics.Distance(p1, p2, true);
            Vector p1Dp2EST = p2p - p1p;
            Assert.IsTrue(p1Dp2EST == p1Dp2);

            Vector p1m1Dp2m1 = physics.Distance(p1m1, p2m1, true);
            Vector p1m1Dp2m2EST = p2m1p + p2p - p1m1p - p1p;
            Assert.IsTrue(p1m1Dp2m1 == p1m1Dp2m2EST);

            Vector p1Dp2m1 = physics.Distance(p1, p2m1, true);
            Vector p1Dp2m1EST = p2m1p + p2p - p1p;
            Assert.IsTrue(p1Dp2m1 == p1Dp2m1EST);

            Vector p2m1Dp1 = physics.Distance(p2m1, p1, true);
            Vector p2m1Dp1EST = p1p - p2m1p - p2p;
            Assert.IsTrue(p2m1Dp1 == p2m1Dp1EST);
        }
    }
}
