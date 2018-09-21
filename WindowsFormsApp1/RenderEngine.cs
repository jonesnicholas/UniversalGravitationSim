using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    using System.Windows.Forms;
    using System.Drawing;
    using System.Linq.Expressions;
    using System.Diagnostics;

    public class RenderEngine
    {
        public double scale = 4.0;
        public Body focus = null;
        internal Vector baseWindowOffset;
        internal DateTime lastFrame = DateTime.Now;
        
        public void renderUniverse(List<Body> universe, Form form, PaintEventArgs e, Body newFocus = null)
        {
            if (universe == null)
            {
                return;
            }
            focus = newFocus == null ? focus : newFocus;
            baseWindowOffset = new Vector(form.ClientRectangle.Width / 2.0, form.ClientRectangle.Height / 2.0,0);
            Graphics dc = e.Graphics;
            Pen RedPen = new Pen(Color.Red, 1);
            dc.DrawRectangle(RedPen, 10, 10, form.ClientRectangle.Width - 20, form.ClientRectangle.Size.Height - 20);
            foreach(Body body in universe)
            {
                renderBody(dc,RedPen,body);
            }
            DateTime thisFrame = DateTime.Now;
            TimeSpan interval = thisFrame - lastFrame;
            double ms = interval.TotalMilliseconds;
            double fps = 1000.0 / ms;
            Debug.WriteLine("FPS: " + (int)Math.Round(fps, 0));
            lastFrame = thisFrame;
        }

        public void renderBody(Graphics dc, Pen pen, Body body)
        {
            Vector pOffset = (focus != null ? body.p - focus.p : body.p);
            Vector windowPosition = pOffset * scale + baseWindowOffset;
            //renderCircle(dc, pen, windowPosition, Math.Max(Math.Pow(body.m, 1.0 / 3.0), 0.25) * scale);
            renderCircle(dc, pen, windowPosition, body.r * scale);
        }

        public void focusNext(List<Body> universe)
        {
            if (focus == null)
            {
                focus = universe.First();
            }
            int ind = universe.IndexOf(focus);
            ind++;
            focus = ind >= universe.Count ? null : universe[ind];
        }

        public void focusNull()
        {
            focus = null;
        }

        internal void renderCircle(Graphics dc, Pen pen, Vector position, double radius)
        {
            dc.DrawEllipse(pen, (float)(position.x - radius), (float)(position.y - radius), (float)(2 * radius), (float)(2 * radius));
        }

        public void scroll(double delta)
        {
            scale *= Math.Pow(2, delta / 120);
        }
    }
}

