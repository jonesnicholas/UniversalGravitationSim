using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsFormsApp1;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class RelativeBodyTests
    {
        Random random = new Random();

        [TestMethod]
        public void RelativeBody_Verify_Constructors_Basic()
        {
            RelativeBody empty = new RelativeBody();

            Vector p0 = Vector.RandVect();
            double mass = random.NextDouble();
            Vector v0 = Vector.RandVect();
            double rho = random.NextDouble();
            string name = "test";
            RelativeBody relBody = new RelativeBody(p0, empty, mass, v0, rho, name);
            Assert.IsTrue(p0 ==relBody.p);
            Assert.AreEqual(mass, relBody.m);
            Assert.IsTrue(v0 == relBody.v);
            Assert.AreEqual(empty, relBody.parent);
            Assert.AreEqual(rho, relBody.rho);
            Assert.AreEqual(name, relBody.name);
        }

        [TestMethod]
        public void RelativeBody_Verify_Constructor_EmptyVelocity()
        {
            double centerMass = 100;
            double separation = 100;
            RelativeBody center = new RelativeBody(centerMass);

            List<int> quads = new List<int>() { 0, 1, 2, 3 };
            foreach (int quad in quads)
            {
                bool xPos = quad == 1 || quad == 4;
                bool yPos = quad == 1 || quad == 2;

                Vector p0 = separation * Vector.RandVect().Normal();
                p0.x *= xPos ? 1 : -1;
                p0.y *= yPos ? 1 : -1;
                checkRoughlyEqual(separation, p0.Mag());

                RelativeBody relBody = new RelativeBody(p0,center, 1);
                checkRoughlyEqual(Math.Sqrt(centerMass / separation), relBody.v.Mag());
                checkRoughlyEqual(0, relBody.p * relBody.v);
            }
        }

        [TestMethod]
        public void RelativeBody_Verify_ParentDepth()
        {
            int layers = 5;
            RelativeBody center = new RelativeBody();
            RelativeBody reference = center;
            for (int i=0; i<layers; i++)
            {
                reference = new RelativeBody(Vector.RandVect(), parentBody: reference);
            }
            Assert.AreEqual(layers, reference.parentDepth());
        }

        [TestMethod]
        public void RelativeBody_Verify_GetMutualParent()
        {
            RelativeBody center = new RelativeBody();
            RelativeBody p1 = new RelativeBody(center);
            RelativeBody p1m1 = new RelativeBody(p1);
            RelativeBody p2 = new RelativeBody(center);
            RelativeBody p2m1 = new RelativeBody(center);

            Assert.AreEqual(center, p1.getMutualParent(p2));
            Assert.AreEqual(center, p1.getMutualParent(center));
            Assert.AreEqual(center, center.getMutualParent(p1));
            Assert.AreEqual(center, p1m1.getMutualParent(p2m1));
            Assert.AreEqual(center, p1m1.getMutualParent(p2));
            Assert.AreEqual(center, p1.getMutualParent(p2m1));
            Assert.AreEqual(p1, p1.getMutualParent(p1m1));
            Assert.AreEqual(p1, p1m1.getMutualParent(p1));
        }

        [TestMethod]
        public void RelativeBody_Verify_DistanceFromParent()
        {
            for (int layers=1; layers<=4; layers++)
            {
                RelativeBody center = new RelativeBody();
                RelativeBody outer = center;
                Vector totalSeparation = new Vector();
                for (int i=0; i<layers; i++)
                {
                    Vector p0 = Vector.RandVect();
                    totalSeparation += p0;
                    outer = new RelativeBody(p0, parentBody: outer);
                }
                Assert.IsTrue(totalSeparation == outer.distanceFromParent(center));
            }
        }

        internal void checkRoughlyEqual(double expected, double actual, double zeroScale = 90)
        {
            double scale = Math.Max(Math.Abs(expected), Math.Abs(actual));
            if (expected == 0)
            {
                scale = zeroScale;
            }
            double epsilon = Math.Max(scale * 1E-15,Double.Epsilon);
            if (Math.Abs(expected - actual) > epsilon)
            {
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
