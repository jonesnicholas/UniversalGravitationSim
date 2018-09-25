﻿using System;
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
        const short smoothFactor = 30;
        internal double fps = 0;
        internal double ups = 0;
        public volatile int updateCount = 0;
        bool useRelative;
        
        public RenderEngine(bool useRel)
        {
            useRelative = useRel;
        }

        public void runRenderEngine(List<Body> universe, Form form, PaintEventArgs e, Body newFocus = null)
        {
            DateTime start = DateTime.Now;
            
            focus = newFocus == null ? focus : newFocus;
            baseWindowOffset = new Vector(form.ClientRectangle.Width / 2.0, form.ClientRectangle.Height / 2.0,0);
            Graphics dc = e.Graphics;
            dc.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen RedPen = new Pen(Color.Red, 1);
            dc.DrawRectangle(RedPen, 10, 10, form.ClientRectangle.Width - 20, form.ClientRectangle.Size.Height - 20);

            renderBodies(dc, universe);

            renderPerformanceStats(dc);
            
            //Debug.WriteLine(((DateTime.Now)-start).TotalMilliseconds);
        }

        #region Rendering Methods

        internal void renderBodies(Graphics dc, List<Body> universe)
        {
            if (universe == null)
            {
                return;
            }
            Pen BodyPen = new Pen(Color.Black, 1);
            foreach (Body body in universe)
            {
                renderBody(dc, BodyPen, body);
            }
        }

        internal void renderPerformanceStats(Graphics dc)
        {
            fpsSmooth++;
            if (fpsSmooth == smoothFactor)
            {
                fpsSmooth = 0;
                double seconds = (DateTime.Now - lastFrame).TotalSeconds;
                lastFrame = DateTime.Now;
                fps = Math.Round(smoothFactor / seconds, 1);
                ups = Math.Round(updateCount / seconds, 1);
                updateCount = 0;
            }
            Font drawFont = new Font("Arial", 10);
            SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.Black);
            float x = 10.0F;
            float y = 10.0F;
            StringFormat drawFormat = new StringFormat();
            dc.DrawString(fps.ToString(), drawFont, drawBrush, x, y, drawFormat);
            dc.DrawString(ups.ToString(), drawFont, drawBrush, x, y + 12, drawFormat);
        }

        internal void renderBody(Graphics dc, Pen pen, Body body)
        {
            Vector pOffset = (focus != null ? body.p - focus.p : body.p);
            Vector windowPosition = pOffset * scale + baseWindowOffset;
            //renderCircle(dc, pen, windowPosition, Math.Max(Math.Pow(body.m, 1.0 / 3.0), 0.25) * scale);
            renderCircle(dc, pen, windowPosition, body.r * scale);
            //renderBodyInfoBox(dc, body, windowPosition);
        }

        internal void renderBodyInfoBox(Graphics dc, Body body, Vector Position)
        {
            Pen InfoPen = new Pen(Color.Blue, 1);
            float lineLength = 20;
            PointF Center = new PointF((float)Position.x, (float)Position.y);
            PointF TopLeft = new PointF(Center.X + lineLength, Center.Y + lineLength);
            SizeF BoxSize = new SizeF(60.0F, 14.0F);
            RectangleF InfoRect = new RectangleF(TopLeft, BoxSize);
            dc.DrawLine(InfoPen, Center, TopLeft);
            dc.DrawRectangle(InfoPen, InfoRect.X, InfoRect.Y, InfoRect.Width, InfoRect.Height);
            dc.DrawString(body.name, new Font("Arial",10), new SolidBrush(System.Drawing.Color.Blue), TopLeft);
        }

        internal void renderCircle(Graphics dc, Pen pen, Vector position, double radius)
        {
            dc.DrawEllipse(pen, (float)(position.x - radius), (float)(position.y - radius), (float)(2 * radius), (float)(2 * radius));
        }

        #endregion

        #region control methods

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

        public void scroll(double delta)
        {
            scale *= Math.Pow(2, delta / 120);
        }

        #endregion
    }
}
