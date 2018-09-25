using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class RelativeBody : Body
    {
        public RelativeBody parent;

        #region constructors

        public RelativeBody()
        {
            parent = null;
        }

        public RelativeBody(Vector inP, 
            double m0 = 0, 
            Vector inV = null,
            RelativeBody parentBody = null, 
            double rho0 = 1, 
            string lbl = "<>")
        {
            parent = parentBody;
            pinned = parent == null;
            p = inP;
            inV = parent == null ? new Vector() : inV;
            if (inV == null)
            {
                Vector pN = p.normal();
                double mag = Math.Sqrt(parent.m / pN.mag());
                v = mag * (new Vector(-pN.y, pN.x, 0));
            }
            rho = rho0;
            m = m0;
            name = lbl;
            initialize();
        }
        #endregion  

        public int parentDepth()
        {
            RelativeBody p = (RelativeBody)parent;
            int output = 1;
            while (p != null)
            {
                p = (RelativeBody)p.parent;
                output++;
            }
            return output;
        }

        internal RelativeBody getMutualParent(RelativeBody other)
        {
            int depthA = this.parentDepth();
            int depthB = other.parentDepth();
            RelativeBody aParent = this.parent;
            RelativeBody bParent = other.parent;
            while (depthA > depthB)
            {
                depthA--;
                aParent = aParent.parent;
            }
            while (depthB > depthA)
            {
                depthB--;
                bParent = bParent.parent;
            }
            while (aParent != bParent)
            {
                aParent = aParent.parent;
                bParent = bParent.parent;
            }
            return aParent;
        }

        internal Vector distanceFromParent(RelativeBody parent)
        {
            RelativeBody par = this.parent;
            Vector relDis = p;
            while (par != parent)
            {
                relDis += par.p;
                par = par.parent;
            }
            return relDis;
        }
    }
}
