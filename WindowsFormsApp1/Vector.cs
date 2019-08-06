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

        public static Vector randVect()
        {
            //2D only for now
            return new Vector(random.NextDouble(), random.NextDouble(), 0.0);
        }

        public double mag()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        public void normalize()
        {
            double m = mag();
            if (m == 0)
                return;
            x /= m;
            y /= m;
            z /= m;
            m = 1;
        }

        public Vector normal()
        {
            Vector output = new Vector(x, y, z);
            double m = mag();
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
            if (ReferenceEquals(a,null) || ReferenceEquals(b,null))
            {
                return false;
            }
            return checkRoughlyEqual(a, b);
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

        internal static bool checkRoughlyEqual(Vector expected, Vector actual)
        {
            double scale = Math.Max(expected.mag(), actual.mag());
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
