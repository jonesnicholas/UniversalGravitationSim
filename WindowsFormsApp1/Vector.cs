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
        internal double m = 0;
        bool mUp = true;

        public Vector(double x0, double y0, double z0)
        {
            x = x0; y = y0; z = z0;
            m = mag();
        }

        public Vector()
        {
            mUp = false;
        }

        public static Vector randVect()
        {
            //2D only for now
            return new Vector(random.NextDouble(), random.NextDouble(), 0.0);
        }

        public double mag()
        {
            m = Math.Sqrt(x * x + y * y + z * z);
            return m;
        }

        public void normalize()
        {
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
            if (m != 0)
            {
                output /= m;
            }
            return output;
        }

        #region Operator Overloads
        public static Vector operator +(Vector a, Vector b)
        {
            Vector output = new Vector(a.x + b.x, a.y + b.y, a.z + b.z);
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
            return a * (1 / b);
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
        #endregion
    }
}
