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
        DateTime lastFrame = DateTime.Now;
        internal short fpsSmooth = 0;
        const short smoothFactor = 20;
        double fps = 0;
        
        public void renderUniverse(List<Body> universe, Form form, PaintEventArgs e, Body newFocus = null)
        {
            DateTime start = DateTime.Now;
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
            fpsSmooth++;
            if (fpsSmooth == smoothFactor)
            {
                fpsSmooth = 0;
                double seconds = (DateTime.Now - lastFrame).TotalSeconds;
                lastFrame = DateTime.Now;
                fps = Math.Round(smoothFactor / seconds, 1);
            }
            Font drawFont = new Font("Arial", 16);
            SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.Black);
            float x = 150.0F;
            float y = 50.0F;
            StringFormat drawFormat = new StringFormat();
            dc.DrawString(fps.ToString(), drawFont, drawBrush, 150, 50, drawFormat);
            Debug.WriteLine(((DateTime.Now)-start).TotalMilliseconds);
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

