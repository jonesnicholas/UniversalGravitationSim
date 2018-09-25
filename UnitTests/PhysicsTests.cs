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
            Physics physics = new Physics(true);
            Assert.IsNotNull(physics);
            physics = new Physics(false);
            Assert.IsNotNull(physics);
        }

        [TestMethod]
        public void Physics_Verify_UpdateBodies()
        {
            List<bool> tfl = new List<bool>() { true, false };
            foreach (bool tf in tfl)
            {
                Physics physics = new Physics(tf);
                List<Body> universe = new List<Body>();
                List<Vector> newVects = new List<Vector>();
                int numBodies = 4;
                for (int i=0; i<numBodies; i++)
                {
                    Body b = tf ? new Body() : new RelativeBody();
                    b.p = randVect();
                    Vector rVecNext = randVect();
                    newVects.Add(rVecNext);
                    b.pNext = rVecNext;
                    universe.Add(b);
                }
                physics.updateBodies(universe);
                for (int i=0; i<numBodies; i++)
                {
                    Assert.AreEqual(newVects[i], universe[i].p);
                }
            }
        }

        [TestMethod]
        public void Physics_Verify_RemoveFlaggedObjects()
        {
            List<bool> tfl = new List<bool>() { true, false };
            foreach (bool tf in tfl)
            {
                Physics physics = new Physics(tf);
                List<Body> universe = new List<Body>();
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
                    universe.Add(b);
                }
                physics.removeFlaggedObjects(ref universe);
                Assert.AreEqual(numBodies - countFlagged, universe.Count);
            }
        }

        [TestMethod]
        public void Physics_Verify_Absolute_Distance()
        {
            Body bodyA = new Body();
            Vector vecA = randVect();
            bodyA.p = vecA;

            Body bodyB = new Body();
            Vector vecB = randVect();
            bodyB.p = vecB;

            Vector manDist = vecB - vecA;

            Physics physics = new Physics(false);
            Vector dist = physics.distance(bodyA, bodyB);
            Assert.AreEqual(manDist.x, dist.x);
            Assert.AreEqual(manDist.y, dist.y);
            Assert.AreEqual(manDist.z, dist.z);
        }

        [TestMethod]
        public void Physics_Verify_Relative_Distance()
        {
            RelativeBody s = new RelativeBody();
            Vector p1p = randVect();
            RelativeBody p1 = new RelativeBody(p1p,parentBody:s);
            Vector p1m1p = randVect();
            RelativeBody p1m1 = new RelativeBody(p1m1p, parentBody:p1);
            Vector p2p = randVect();
            RelativeBody p2 = new RelativeBody(p2p, parentBody:s);
            Vector p2m1p = randVect();
            RelativeBody p2m1 = new RelativeBody(p2m1p, parentBody:p2);

            Physics physics = new Physics(true);

            Vector p1Dp2 = physics.distance(p1, p2);
            Vector p1Dp2EST = p2p - p1p;
            checkRoughlyEqual(p1Dp2EST, p1Dp2);

            Vector p1m1Dp2m1 = physics.distance(p1m1, p2m1);
            Vector p1m1Dp2m2EST = p2m1p + p2p - p1m1p - p1p;
            checkRoughlyEqual(p1m1Dp2m1, p1m1Dp2m2EST);

            Vector p1Dp2m1 = physics.distance(p1, p2m1);
            Vector p1Dp2m1EST = p2m1p + p2p - p1p;
            checkRoughlyEqual(p1Dp2m1, p1Dp2m1EST);

            Vector p2m1Dp1 = physics.distance(p2m1, p1);
            Vector p2m1Dp1EST = p1p - p2m1p - p2p;
            checkRoughlyEqual(p2m1Dp1, p2m1Dp1EST);
        }

        internal void checkRoughlyEqual(Vector expected, Vector actual)
        {
            double scale = Math.Max(expected.mag(), actual.mag());
            double epsilon = scale*1E-15;
            if (Math.Abs(expected.x - actual.x) > epsilon)
            {
                Assert.AreEqual(expected.x, actual.x);
            }
            if (Math.Abs(expected.y - actual.y) > epsilon)
            {
                Assert.AreEqual(expected.y, actual.y);
            }
            if (Math.Abs(expected.z - actual.z) > epsilon)
            {
                Assert.AreEqual(expected.z, actual.z);
            }
        }

        internal Vector randVect()
        {
            return new Vector(random.NextDouble(), random.NextDouble(), random.NextDouble());
        }
    }
}
