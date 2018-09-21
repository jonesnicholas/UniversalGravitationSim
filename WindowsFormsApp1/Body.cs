using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class Body
    {
        public Vector p;
        public Vector v;

        public Vector pNext;
        public Vector vNext;

        public double m;
        public double rho; //avg density
        public double r;
        public bool pinned;
        public bool deletionFlag = false;

        #region constructors
        public Body(double px0, double py0, double vx0, double vy0, double m0, double rho0 = 1, bool pin = false)
        {
            p = new Vector(px0, py0, 0);
            v = new Vector(vx0, vy0, 0);
            rho = rho0;
            m = m0;
            pinned = pin;
            initialize();
        }

        public Body(Vector p0, Vector v0, double m0, double rho0 = 1, bool pin = false)
        {
            p = p0;
            v = v0;
            rho = rho0;
            m = m0;
            pinned = pin;
            initialize();
        }
        public Body(double px0, double py0, Body center, double m0, double rho0 = 1)
        {
            p = new Vector(px0, py0, 0);
            double vMag = Math.Sqrt(center.m / p.mag());
            Vector pN = p.normal();
            v = new Vector(-pN.y * vMag, pN.x * vMag, 0);
            v += center.v;
            p += center.p;
            rho = rho0;
            m = m0;
            initialize();
        }

        public Body(Vector p0, Body center, double m0, double rho0 = 1)
        {
            p = p0;
            double vMag = Math.Sqrt(center.m / p0.mag());
            Vector pN = p.normal();
            v = new Vector(-pN.y * vMag, pN.x * vMag,0);
            v += center.v;
            p += center.p;
            rho = rho0;
            m = m0;
            initialize();
        }
        #endregion  

        public void update()
        {
            if (pinned)
                return;
            p = pNext;
            v = vNext;
        }

        public void initialize()
        {
            estR();
        }

        public void estR()
        {
            r = Math.Pow(m / rho * 3.0 / 4.0 / Math.PI, 1.0 / 3.0);
        }
    }
}
