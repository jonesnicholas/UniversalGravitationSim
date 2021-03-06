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
        public double scale = 1.0;//1.164e-10;//4.0;
        public Body focus = null;
        internal Vector baseWindowOffset;
        DateTime lastFrame = DateTime.Now;
        internal short fpsSmooth = 0;
        const short smoothFactor = 30;
        internal double fps = 0;
        internal double ups = 0;
        internal float staty = 10.0F;
        internal const float statyDY = 12;
        public volatile int updateCount = 0;

        public bool renderPerfStats = true;
        public bool renderReferenceLines = false;
        public bool useMinBodyRadius = false;
        public bool hideSmallBodies = false;
        public double minBodyRadius = 2.0;
        public double radiusScale = 1.0;
        
        public RenderEngine()
        {
        }

        public void BaseScaleForUniverse(Form form, Universe universe, double correction = 1.0)
        {
            if (form == null)
            {
                return;
            }
            double maxD = 0;
            if (universe.useRelative)
            {
                foreach(RelativeBody body in universe.GetBodies())
                {
                    double bodyD = body.GetAbsP().Mag();
                    maxD = bodyD > maxD ? bodyD : maxD;
                }
            }
            else
            {
                foreach (RelativeBody body in universe.GetBodies())
                {
                    double bodyD = body.p.Mag();
                    maxD = bodyD > maxD ? bodyD : maxD;
                }
            }
            double smallerSide = Math.Min(form.ClientRectangle.Width / 2.0, form.ClientRectangle.Height / 2.0);
            scale = smallerSide / maxD / correction;
        }

        public void runRenderEngine(Simulation simulation, Form form, PaintEventArgs e, Body newFocus = null)
        {
            DateTime start = DateTime.Now;
            
            focus = newFocus ?? focus;
            baseWindowOffset = new Vector(form.ClientRectangle.Width / 2.0, form.ClientRectangle.Height / 2.0,0);
            Graphics dc = e.Graphics;
            dc.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen RedPen = new Pen(Color.Red, 1);
            dc.DrawRectangle(RedPen, 10, 10, form.ClientRectangle.Width - 20, form.ClientRectangle.Size.Height - 20);

            renderBodies(dc, simulation.universe);

            staty = 10.0F;
            if (renderPerfStats)
            {
                RenderPerformanceStats(simulation, dc);
            }
            
            //Debug.WriteLine(((DateTime.Now)-start).TotalMilliseconds);
        }

        #region Rendering Methods

        internal void renderBodies(Graphics dc, Universe universe)
        {
            if (universe == null)
            {
                return;
            }
            Pen BodyPen = new Pen(Color.Black, 1);
            if (universe.useRelative)
            {
                RenderUniverseFromFocus(dc, BodyPen, universe, focus as RelativeBody);
            }
            else
            {
                foreach (Body body in universe.GetBodies())
                {
                    renderBody(dc, BodyPen, body);
                }
            }
        }

        internal void RenderPerformanceStats(Simulation simulation, Graphics dc)
        {
            fpsSmooth++;
            if (fpsSmooth == smoothFactor)
            {
                fpsSmooth = 0;
                double seconds = (DateTime.Now - lastFrame).TotalSeconds;
                lastFrame = DateTime.Now;
                fps = Math.Round(smoothFactor / seconds, 1);
                simulation.interval = 1.0 / fps;
                ups = Math.Round(updateCount / seconds, 1);
                updateCount = 0;
            }

            renderStatString("FPS", fps, 2, dc);
            renderStatString("UPS", ups, 2, dc);
            renderStatString("Interval", fps, 2, dc);
            renderStatString("Dilation", simulation.desiredTimeDilation, 0, dc);
            renderStatString("SimDegree", simulation.simDegree, 0, dc);
            renderStatString("dtUpdate", simulation.interval * simulation.desiredTimeDilation / simulation.simDegree, 2, dc);
            renderStatString("dtFrame", simulation.interval * simulation.desiredTimeDilation, 2, dc);
            renderStatString("dtSecond", simulation.interval * simulation.desiredTimeDilation * fps * (simulation.play ? 1 : 0), 2, dc);
        }

        internal void renderStatString(String str, double stat, int round, Graphics dc)
        {
            Font drawFont = new Font("Arial", 10);
            SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.Black);

            StringFormat drawFormat = new StringFormat();
            dc.DrawString($"{str}: {Math.Round(stat, round)}", drawFont, drawBrush, 10.0F, staty, drawFormat);
            staty += statyDY;
        }

        internal void renderBody(Graphics dc, Pen pen, Body body, Vector pOffset = null)
        {
            pOffset = pOffset ?? (focus != null ? body.p - focus.p : body.p);
            Vector windowPosition = pOffset * scale + baseWindowOffset;
            if (windowPosition.Mag() > baseWindowOffset.Mag() * 3)
            {
                //do not render things well outside of window
                return;
            }
            //renderCircle(dc, pen, windowPosition, Math.Max(Math.Pow(body.m, 1.0 / 3.0), 0.25) * scale);
            double radius = body.r * scale * radiusScale;
            if (useMinBodyRadius && !hideSmallBodies)
            {
                radius = radius < minBodyRadius ? minBodyRadius : radius;
            }
            if (!useMinBodyRadius || !hideSmallBodies || radius >= minBodyRadius)
            {
                renderCircle(dc, pen, windowPosition, radius);
            }
        }

        internal Vector WindowCoord(Vector position)
        {
            // position should already be realtive to the focus
            return position * scale + baseWindowOffset;
        }

        internal void renderArrow(Graphics dc, Pen pen, Vector startPos, Vector endPos)
        {
            renderWindowArrow(dc, pen, WindowCoord(startPos), WindowCoord(endPos));
        }

        internal void renderWindowArrow(Graphics dc, Pen pen, Vector startPos, Vector endPos)
        {
            if (startPos.Mag() > baseWindowOffset.Mag() * 3 || 
                endPos.Mag() > baseWindowOffset.Mag() * 3)
            {
                //do not render things well outside of window
                //TODO: if line passes through window, try to detect and render that part
                return;
            }

            Vector body = endPos - startPos;
            double theta = Math.Atan2(body.y, body.x);
            double thetaH1 = 0.75 * Math.PI + theta;
            double thetaH2 = 1.25 * Math.PI + theta;
            Vector H1 = body.Mag() / 10.0 * new Vector(Math.Cos(thetaH1), Math.Sin(thetaH1), 0);
            Vector H2 = body.Mag() / 10.0 * new Vector(Math.Cos(thetaH2), Math.Sin(thetaH2), 0);

            renderLine(dc, pen, startPos, endPos);
            renderLine(dc, pen, endPos, endPos + H1);
            renderLine(dc, pen, endPos, endPos + H2);
        }

        internal void renderLine(Graphics dc, Pen pen, Vector startPos, Vector endPos)
        {
            dc.DrawLine(pen, (float)startPos.x, (float)startPos.y, (float)endPos.x, (float)endPos.y);
        }

        internal void RenderUniverseFromFocus(Graphics dc, Pen pen, Universe universe, RelativeBody focus)
        {
            RelativeBody center = (RelativeBody)universe.GetBodies().First(); // TODO: better handle binary-type cases
            Pen childPen = new Pen(Color.Blue, 1);

            if (focus == null && center != null)
            {
                RecursiveRenderChildren(dc, childPen, center, center.p);
            }
            else
            {
                RecursiveRenderChildren(dc, childPen, focus, Vector.zeroVect);
                RecursiveRenderParents(dc, pen, focus, Vector.zeroVect);
            }
        }

        internal void RecursiveRenderParents(Graphics dc, Pen pen, RelativeBody body, Vector pOffset)
        {
            RelativeBody parent = body.parent;
            if (parent != null)
            {
                if (renderReferenceLines)
                {
                    renderArrow(dc, pen, pOffset - body.p, pOffset);
                }
                RecursiveRenderChildren(dc, pen, parent, pOffset - body.p, body);
                RecursiveRenderParents(dc, pen, parent, pOffset - body.p);
            }
        }

        internal void RecursiveRenderChildren(Graphics dc, Pen pen, RelativeBody body, Vector pOffset, RelativeBody skip = null)
        {
            renderBody(dc, pen, body, pOffset);
            foreach (RelativeBody child in body.children)
            {
                if (child != skip)
                {
                    if (renderReferenceLines)
                    {
                        renderArrow(dc, pen, pOffset, pOffset + child.p);
                    }
                    RecursiveRenderChildren(dc, pen, child, pOffset + child.p);
                }
            }
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

        public void focusNext(Universe universe)
        {
            if (focus == null)
            {
                focus = universe.GetBodies().First();
            }
            int ind = universe.GetBodies().IndexOf(focus);
            ind++;
            focus = ind >= universe.GetBodies().Count ? null : universe.GetBodies()[ind];
            Debug.WriteLine($"New Focus: {(focus == null ? "Barycenter" : focus.ToString())}");
        }

        public void focusNull()
        {
            focus = null;
        }

        public void scroll(double delta)
        {
            scale *= Math.Pow(2, delta / 120);
            //Debug.WriteLine(scale);
        }

        #endregion
    }
}

