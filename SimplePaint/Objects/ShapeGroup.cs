using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Drawing;


namespace SimplePaint.Objects
{
    /// <summary>
    /// Lớp đại diện cho một hình gồm nhiều hình với nhau
    /// </summary>
    public class GroupShape : Shape, IEnumerable
    {
        private List<Shape> shapes;

        public GroupShape()
        {
            name = "Group";
            shapes = new List<Shape>();

        }

        public Shape this[int index]
        {
            get
            {
                return shapes[index];
            }
            set
            {
                shapes[index] = value;
            }
        }

        private GraphicsPath[] graphicsPaths
        {
            get
            {
                GraphicsPath[] paths = new GraphicsPath[shapes.Count];

                for (int i = 0; i < shapes.Count; i++)
                {
                    GraphicsPath path = new GraphicsPath();
                    if (shapes[i] is DrawLine)
                    {
                        DrawLine line = (DrawLine)shapes[i];
                        path.AddLine(line.pointHead, line.pointTail);
                    }
                    else if (shapes[i] is DrawCurve)
                    {
                        DrawCurve curve = (DrawCurve)shapes[i];
                        path.AddCurve(curve.points.ToArray());
                    }
                    else if (shapes[i] is DrawEllipse)
                    {
                        DrawEllipse ellipse = (DrawEllipse)shapes[i];
                        path.AddEllipse(new Rectangle(ellipse.pointHead.X,
                            ellipse.pointHead.Y,
                            ellipse.pointTail.X - ellipse.pointHead.X,
                            ellipse.pointTail.Y - ellipse.pointHead.Y));
                    }
                    else if (shapes[i] is DrawRectangle)
                    {
                        DrawRectangle rect = (DrawRectangle)shapes[i];

                        path.AddRectangle(new RectangleF(rect.pointHead.X,
                            rect.pointHead.Y,
                            rect.pointTail.X - rect.pointHead.X,
                            rect.pointTail.Y - rect.pointHead.Y));
                    }
                    else if (shapes[i] is DrawPolygon)
                    {
                        DrawPolygon polygon = (DrawPolygon)shapes[i];
                        path.AddPolygon(polygon.points.ToArray());
                    }
                    else if (shapes[i] is GroupShape)
                    {
                        GroupShape group = (GroupShape)shapes[i];
                        for (int j = 0; j < group.graphicsPaths.Length; j++)
                        {
                            path.AddPath(group.graphicsPaths[j], false);
                        }
                    }
                    paths[i] = path;
                }

                return paths;
            }
        }

        /// <summary>
        /// Phương thức thêm một hình vào danh sách
        /// </summary>
        /// <param name="shape">Hình cần thêm</param>
        public void addShape(Shape shape)
        {
            shapes.Add(shape);
        }

        public override object Clone()
        {
            GroupShape group = new GroupShape
            {
                pointHead = pointHead,
                pointTail = pointTail,
                isSelected = isSelected,
                name = name,
                color = color,
                contourWidth = contourWidth
            };
            for (int i = 0; i < shapes.Count; i++)
            {
                group.shapes.Add(shapes[i].Clone() as Shape);
            }
            return group;
        }

        public override void drawShape(System.Drawing.Graphics g)
        {
            GraphicsPath[] paths = graphicsPaths;
            for (int i = 0; i < paths.Length; i++)
            {
                using (GraphicsPath path = paths[i])
                {
                    if (shapes[i] is DrawRectangle || shapes[i] is DrawEllipse || shapes[i] is DrawPolygon)
                    {
                        if (shapes[i].isFill)
                        {
                            using (Brush brush = new SolidBrush(shapes[i].color))
                            {
                                g.FillPath(brush, path);
                            }
                        }
                        else
                        {
                            using (Pen pen = new Pen(shapes[i].color, shapes[i].contourWidth))
                            {
                                g.DrawPath(pen, path);
                            }
                        }
                    }
                    else if (shapes[i] is GroupShape)
                    {
                        GroupShape group = (GroupShape)shapes[i];
                        group.drawShape(g);
                    }
                    else
                    {
                        using (Pen pen = new Pen(shapes[i].color, shapes[i].contourWidth))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }
            }
        }

        public override bool isHit(System.Drawing.Point p)
        {
            GraphicsPath[] paths = graphicsPaths;
            for (int i = 0; i < paths.Length; i++)
            {
                using (GraphicsPath path = paths[i])
                {
                    if (shapes[i] is DrawRectangle || shapes[i] is DrawEllipse)
                    {
                        if (((DrawRectangle)shapes[i]).isFill)
                        {
                            using (Brush brush = new SolidBrush(Color.Black))
                            {
                                if (path.IsVisible(p))
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            using (Pen pen = new Pen(Color.Black, contourWidth + 3))
                            {
                                if (path.IsOutlineVisible(p, pen))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    else if (!(shapes[i] is GroupShape))
                    {
                        using (Pen pen = new Pen(Color.Black, contourWidth + 3))
                        {
                            if (path.IsOutlineVisible(p, pen))
                            {
                                return true;
                            }
                        }
                    }
                    else if (shapes[i] is GroupShape)
                    {
                        GroupShape group = (GroupShape)shapes[i];
                        return group.isHit(p);
                    }
                }
            }

            return false;
        }

        protected override System.Drawing.Drawing2D.GraphicsPath graphicsPath
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerator GetEnumerator()
        {
            return shapes.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return shapes.Count;
            }
        }

        public override void moveShape(Point distance)
        {
            for (int i = 0; i < shapes.Count; i++)
            {
                if (shapes[i] is GroupShape)
                {
                    GroupShape group = (GroupShape)shapes[i];
                    group.moveShape(distance);
                }
                else
                {
                    shapes[i].moveShape(distance);
                }
            }
            base.moveShape(distance);
        }

    }
}
