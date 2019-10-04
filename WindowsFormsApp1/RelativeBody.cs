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
        public List<RelativeBody> children;

        #region constructors

        public RelativeBody(double m0 = 0, string lbl = "")
        {
            parent = null;
            name = lbl;
            m = m0;
            Initialize();
        }

        public RelativeBody(RelativeBody parentBody)
        {
            parent = parentBody;
            Initialize();
        }

        public RelativeBody(
            double x0,
            double y0,
            RelativeBody parentBody = null,
            double m0 = 0,
            Vector inV = null,
            double rho0 = 1,
            string lbl = "<>")
        {
            Vector inP = new Vector(x0, y0, 0.0);
            parent = parentBody;
            p = inP;
            v = parent == null ? new Vector() : inV;
            if (v == null)
            {
                Vector pN = p.Normal();
                double mag = Math.Sqrt(parent.m / p.Mag());
                v = mag * (new Vector(-pN.y, pN.x, pN.z));
            }
            rho = rho0;
            m = m0;
            name = lbl;
            Initialize();
        }

        public RelativeBody(
            Vector inP,
            RelativeBody parentBody = null,
            double m0 = 0, 
            Vector inV = null,
            double rho0 = 1, 
            string lbl = "<>")
        {
            parent = parentBody;
            p = inP;
            v = parent == null ? new Vector() : inV;
            if (v == null)
            {
                Vector pN = p.Normal();
                double mag = Math.Sqrt(parent.m / p.Mag());
                v = mag * (new Vector(-pN.y, pN.x, pN.z));
            }
            rho = rho0;
            m = m0;
            name = lbl;
            Initialize();
        }
        #endregion  

        public override void Initialize()
        {
            base.Initialize();
            //pinned = parent == null; //TODO: Work-around this, need soln for binary objects
            children = new List<RelativeBody>();
            if (this.parent != null)
            {
                parent.AdoptChild(this);
            }
        }

        /// <summary>
        /// Adds the child to this body's list of children, removes from old parent's children, and sets child's parent to this
        /// </summary>
        /// <param name="child">The body being 'adopted'</param>
        public void AdoptChild(RelativeBody child)
        {
            if (!children.Contains(child))
            {
                if (child.parent != null && child.parent != this)
                {
                    child.parent.AbandonChild(child);
                }
                child.parent = this;
                children.Add(child);
            }
        }

        private void AbandonChild(RelativeBody child)
        {
            if (children.Contains(child))
            {
                children.Remove(child);
                child.parent = null;
            }
        }

        public int parentDepth()
        {
            RelativeBody p = (RelativeBody)parent;
            int output = 0;
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
            RelativeBody aParent = this;
            RelativeBody bParent = other;
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
            RelativeBody par = this;
            Vector relDis = new Vector();
            while (par != null && par != parent)
            {
                relDis += par.p;
                par = par.parent;
            }
            return relDis;
        }

        public double GetFamilyMass()
        {
            double mass = this.m;
            foreach(RelativeBody child in children)
            {
                mass += child.GetFamilyMass();
            }
            return mass;
        }

        public Vector GetFamilyBarycenter()
        {
            double mass = this.m;
            Vector Bary = new Vector();
            foreach(RelativeBody child in children)
            {
                Vector subBary = child.GetFamilyBarycenter() + child.p;
                double subm = child.GetFamilyMass();

                Bary += subm * subBary;
                mass += subm;
            }
            Bary /= mass;
            return Bary;
        }

        public Vector GetAbsP()
        {
            if (this.parent == null)
            {
                return this.p;
            }
            return this.p + parent.GetAbsP();
        }

        public Vector GetAbsV()
        {
            if (this.parent == null)
            {
                return this.v;
            }
            return this.v + parent.GetAbsV();
        }

        public void correctForMovingReferenceFrames(double dt)
        {
            if (parent != null)
            {
                vNext -= dt * parent.a;
            }
            foreach (RelativeBody child in children)
            {
                child.correctForMovingReferenceFrames(dt);
            }
        }
    }
}
