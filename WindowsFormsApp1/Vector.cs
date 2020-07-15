using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class Vector
    {
        static Random random = new Random();

        public double x = 0;
        public double y = 0;
        public double z = 0;

        public static Vector zeroVect = new Vector();

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }

        public Vector(double x0, double y0, double z0)
        {
            x = x0; y = y0; z = z0;
        }

        public Vector()
        {
            // default is zero vector
        }

        /// <summary>
        /// Generates a random 2D vector (x, y, 0.0)
        /// </summary>
        /// <returns>a 2D vector with random x and y components</returns>
        public static Vector RandVect()
        {
            //2D only for now
            return new Vector(random.NextDouble(), random.NextDouble(), 0.0);
        }

        /// <summary>
        /// Gets the magnitude of the vector
        /// </summary>
        /// <returns>the magnitude of the vector</returns>
        public double Mag()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        /// <summary>
        /// Normalizes the vector, keeping the direction the same, but changing the magnitude to 1. 
        /// If the vector is the zero vector, keep as is
        /// </summary>
        public void Normalize()
        {
            double m = Mag();
            if (m == 0)
                return;
            x /= m;
            y /= m;
            z /= m;
            m = 1;
        }

        /// <summary>
        /// Generates the unit vector associated with the current vector. If the vector is the zero vector, return a zero vector
        /// </summary>
        /// <returns>a new vector with same direction but magnitude 1</returns>
        public Vector Normal()
        {
            Vector output = new Vector(x, y, z);
            double m = Mag();
            if (m != 0)
            {
                output /= m;
            }
            return output;
        }

        #region Operator Overloads
        public static bool operator ==(Vector a, Vector b)
        {
            if (ReferenceEquals(a,b))
            {
                return true;
            }
            if (a is null || b is null)
            {
                return false;
            }
            return CheckRoughlyEqual(a, b);
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return !(a == b);
        }

        public static Vector operator +(Vector a, Vector b)
        {
            Vector output = new Vector(a.x + b.x, a.y + b.y, a.z + b.z);
            return output;
        }

        public static Vector operator -(Vector a)
        {
            Vector output = new Vector(-a.x, -a.y, -a.z);
            return output;
        }

        public static Vector operator -(Vector a, Vector b)
        {
            Vector output = a + (-b);
            return output;
        }

        public static Vector operator *(Vector a, double b)
        {
            Vector output = new Vector(a.x * b, a.y * b, a.z * b);
            return output;
        }

        public static Vector operator *(double a, Vector b)
        {
            return b * a;
        }

        public static Vector operator /(Vector a, double b)
        {
            return a * (1.0 / b);
        }

        public static double operator *(Vector a, Vector b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vector operator /(Vector a, Vector b)
        {
            Vector output = new Vector(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
            return output;
        }
        #endregion

        /// <summary>
        /// Determines if two vectors are close enough to be considered 'equal'
        /// Should only be used for unit tests
        /// </summary>
        /// <param name="expected">Expected value of Vector</param>
        /// <param name="actual">Actual value of Vector</param>
        /// <returns>if the vectors are roughly equal</returns>
        internal static bool CheckRoughlyEqual(Vector expected, Vector actual)
        {
            double scale = Math.Max(expected.Mag(), actual.Mag());
            double epsilon = scale * 1E-15;
            if (Math.Abs(expected.x - actual.x) > epsilon)
            {
                return false;
            }
            if (Math.Abs(expected.y - actual.y) > epsilon)
            {
                return false;
            }
            if (Math.Abs(expected.z - actual.z) > epsilon)
            {
                return false;
            }
            return true;
        }
    }
}
