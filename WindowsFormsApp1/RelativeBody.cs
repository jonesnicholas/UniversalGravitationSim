﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class RelativeBody : Body
    {
        Body parent;

        #region constructors

        public RelativeBody(Vector inP, 
            double m0, 
            Vector inV = null,
            Body parentBody = null, 
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
    }
}
