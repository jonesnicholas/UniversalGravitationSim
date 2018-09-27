using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1;

namespace UnitTests
{
    [TestClass]
    public class VectorTests
    {
        Random random = new Random();

        [TestMethod]
        public void Vector_Verify_Constructors()
        {
            Vector empty = new Vector();
            Assert.AreEqual(0, empty.x);
            Assert.AreEqual(0, empty.y);
            Assert.AreEqual(0, empty.z);

            Vector custom = new Vector(1.0, -2.0, 2.0);
            Assert.AreEqual(1.0, custom.x);
            Assert.AreEqual(-2.0, custom.y);
            Assert.AreEqual(2.0, custom.z);

            double rx = random.NextDouble();
            double ry = random.NextDouble();
            double rz = random.NextDouble();

            Vector rVec = new Vector(rx, ry, rz);
            Assert.AreEqual(rx, rVec.x);
            Assert.AreEqual(ry, rVec.y);
            Assert.AreEqual(rz, rVec.z);
        }

        [TestMethod]
        public void Vector_Verify_RandVect()
        {
            Vector randA = Vector.randVect();
            Vector randB = Vector.randVect();
            Assert.IsNotNull(randA);
            Assert.AreNotEqual(0, randA.mag());
            Assert.AreNotEqual(randA.x, randB.x);
            Assert.AreNotEqual(randA.y, randB.y);

            Assert.AreEqual(randA.z, randB.z);
            Assert.AreEqual(randA.z, 0);
        }

        [TestMethod]
        public void Vector_Verify_Mag()
        {
            double rx = random.NextDouble();
            double ry = random.NextDouble();
            double rz = random.NextDouble();

            double square = rx * rx + ry * ry + rz * rz;
            Vector vector = new Vector(rx, ry, rz);
            checkRoughlyEqual(square, vector.mag() * vector.mag());

            checkRoughlyEqual(rx, vector.x);
            checkRoughlyEqual(ry, vector.y);
            checkRoughlyEqual(rz, vector.z);

        }

        [TestMethod]
        public void Vector_Verify_Normalization()
        {
            Vector vector = Vector.randVect();
            double magnitude = vector.mag();
            Vector normal = vector.normal();

            Assert.AreEqual(magnitude, vector.mag());
            checkRoughlyEqual(1, normal.mag());
            Assert.IsTrue(normal * magnitude == vector);

            vector.normalize();

            checkRoughlyEqual(1, vector.mag());
            Assert.IsTrue(vector == normal);
        }

        [TestMethod]
        public void Vector_Verify_Operator_Equals()
        {
            Vector randA = Vector.randVect();
            Vector randB = Vector.randVect();
            Vector randAEqual = new Vector(randA.x, randA.y, randA.z);
            Vector nullVec = null;

            Assert.IsTrue(randA == randAEqual);
            Assert.IsTrue(randA == randA);
            Assert.IsTrue(nullVec == nullVec);
            Assert.IsFalse(randA == nullVec);
            Assert.IsFalse(randA == randB);
        }

        [TestMethod]
        public void Vector_Verify_Operator_NotEquals()
        {
            Vector randA = Vector.randVect();
            Vector randB = Vector.randVect();
            Vector randAEqual = new Vector(randA.x, randA.y, randA.z);
            Vector nullVec = null;

            Assert.IsFalse(randA != randAEqual);
            Assert.IsFalse(randA != randA);
            Assert.IsFalse(nullVec != nullVec);
            Assert.IsTrue(randA != nullVec);
            Assert.IsTrue(randA != randB);
        }

        [TestMethod]
        public void Vector_Verify_Operator_Plus()
        {
            double ax = random.NextDouble();
            double ay = random.NextDouble();
            double az = random.NextDouble();
            double bx = random.NextDouble();
            double by = random.NextDouble();
            double bz = random.NextDouble();

            Vector A = new Vector(ax, ay, az);
            Vector B = new Vector(bx, by, bz);
            Vector C = A + B;

            Assert.AreEqual(ax + bx, C.x);
            Assert.AreEqual(ay + by, C.y);
            Assert.AreEqual(az + bz, C.z);

            A += B;

            Assert.IsTrue(A == C);
        }

        internal void checkRoughlyEqual(double expected, double actual, double zeroScale = 90)
        {
            double scale = Math.Max(Math.Abs(expected), Math.Abs(actual));
            if (expected == 0)
            {
                scale = zeroScale;
            }
            double epsilon = Math.Max(scale * 1E-15, Double.Epsilon);
            if (Math.Abs(expected - actual) > epsilon)
            {
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
