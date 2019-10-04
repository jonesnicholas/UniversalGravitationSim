using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class Body
    {
        public string name;

        public Vector p = new Vector();
        public Vector v = new Vector();
        public Vector a = new Vector();

        public Vector pNext = new Vector();
        public Vector vNext = new Vector();

        public double m = 0;
        public double rho = 1; //avg density
        public double r = 0;
        public bool pinned = false;
        public bool deletionFlag = false;

        #region constructors

        public Body()
        {
            Initialize();
        }

        public Body(double m0, double rho0 = 1, bool pin = false, string lbl = "<>")
        {
            p = new Vector(0, 0, 0);
            v = new Vector(0, 0, 0);
            rho = rho0;
            m = m0;
            pinned = pin;
            name = lbl;
            Initialize();
        }

        public Body(double px0, double py0, double vx0, double vy0, double m0, double rho0 = 1, bool pin = false, string lbl = "<>")
        {
            p = new Vector(px0, py0, 0);
            v = new Vector(vx0, vy0, 0);
            rho = rho0;
            m = m0;
            pinned = pin;
            name = lbl;
            Initialize();
        }

        public Body(Vector p0, Vector v0, double m0, double rho0 = 1, bool pin = false, string lbl = "<>")
        {
            p = p0;
            v = v0;
            rho = rho0;
            m = m0;
            pinned = pin;
            name = lbl;
            Initialize();
        }
        public Body(double px0, double py0, Body center, double m0, double rho0 = 1, string lbl = "<>")
        {
            p = new Vector(px0, py0, 0);
            double vMag = Math.Sqrt(center.m / p.Mag());
            Vector pN = p.Normal();
            v = new Vector(-pN.y * vMag, pN.x * vMag, 0);
            v += center.v;
            p += center.p;
            rho = rho0;
            m = m0;
            name = lbl;
            Initialize();
        }

        public Body(Vector p0, Body center, double m0, double rho0 = 1, string lbl = "<>")
        {
            p = p0;
            double vMag = Math.Sqrt(center.m / p0.Mag());
            Vector pN = p.Normal();
            v = new Vector(-pN.y * vMag, pN.x * vMag,0);
            v += center.v;
            p += center.p;
            rho = rho0;
            m = m0;
            name = lbl;
            Initialize();
        }
        #endregion  

        /// <summary>
        /// Updates the position and velocity to match the 'next' position and velocity as calculated by the physics engine.
        /// Does nothing if the body is 'pinned'
        /// </summary>
        public void UpdateBodyPV()
        {
            if (pinned)
                return;
            p = pNext;
            v = vNext;
        }

        /// <summary>
        /// Runs any code needed to initialize the body
        /// </summary>
        public virtual void Initialize()
        {
            EstR();
        }

        /// <summary>
        /// Estimates the radius of the body based on mass and density
        /// </summary>
        public void EstR()
        {
            r = Math.Pow(m / rho * 3.0 / 4.0 / Math.PI, 1.0 / 3.0);
        }

        public override string ToString()
        {
            return $"{name}: {p.ToString()} {v.ToString()}";
        }
    }
}
