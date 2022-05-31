using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace SimplePaint.Objects
{
    public class DrawLine : Shape
    {

        public DrawLine()
        {
            name = "Line";
        }

        public DrawLine(Color color)
        {
            name = "Line";
            this.color = color;
        }

        public override object Clone()
        {
            return new DrawLine
            {
                pointHead = pointHead,
                pointTail = pointTail,
                contourWidth = contourWidth,
                isSelected = isSelected,
                color = color,
                name = name
            };
        }

        public override void drawShape(Graphics g)
        {
            using (GraphicsPath path = graphicsPath)
            {
                using (Pen pen = new Pen(color, contourWidth))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        protected override GraphicsPath graphicsPath
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                path.AddLine(pointHead, pointTail);
                return path;
            }
        }

        public override bool isHit(Point p)
        {
            bool inside = false;
            using (GraphicsPath path = graphicsPath)
            {
                using (Pen pen = new Pen(color, contourWidth + 3))
                {
                    inside = path.IsOutlineVisible(p, pen);
                }
            }
            return inside;
        }

        public override int isHitControlsPoint(System.Drawing.Point p)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new Rectangle(pointHead.X - 4, pointHead.Y - 4, 8, 8));
            if (path.IsVisible(p)) return 0;
            path.AddRectangle(new Rectangle(pointTail.X - 4, pointTail.Y - 4, 8, 8));
            if (path.IsVisible(p)) return 7;
            return -1;
        }

    }
}
