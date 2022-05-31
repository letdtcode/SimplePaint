using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using SimplePaint.Addition;
using SimplePaint.Objects;

namespace SimplePaint
{
    public partial class Form1 : Form,SetUpView
    {
        private TaskHandling taskHandling;
        private DataHandling dataHandling;
        private UpdateHandling updateHandling;
        private Graphics gr;
        public Form1()
        {
            InitializeComponent();
            changeRegion();
            initComponents();
            gr = ptbDraw.CreateGraphics();

        }
        
        private void changeRegion()
        {
            using (GraphicsPath path = new GraphicsPath())
            {

                path.AddLine(new Point(0, 20), new Point(0, this.Height - 150));

                path.AddLine(new Point(0, this.Height - 150),
                    new Point((this.Width / 2) - 50, this.Height - 150));

                path.AddLine(new Point((this.Width / 2) - 50, this.Height - 150),
                    new Point((this.Width / 2) - 80, this.Height - 110));

                path.AddLine(new Point((this.Width / 2) - 80, this.Height - 110),
                    new Point((this.Width / 2) - 150, this.Height - 90));

                path.AddLine(new Point((this.Width / 2) - 150, this.Height - 90),
                    new Point((this.Width / 2) + 150, this.Height - 90));

                path.AddLine(new Point((this.Width / 2) + 150, this.Height - 90),
                    new Point((this.Width / 2) + 80, this.Height - 110));

                path.AddLine(new Point((this.Width / 2) + 80, this.Height - 110),
                    new Point((this.Width / 2) + 50, this.Height - 150));

                path.AddLine(new Point((this.Width / 2) + 50, this.Height - 150),
                    new Point(this.Width, this.Height - 150));

                path.AddLine(new Point(this.Width, this.Height - 150), new Point(this.Width, 20));

                path.AddLine(new Point(this.Width, 20), new Point(0, 20));

                this.Region = new Region(path);
            }
        }
        private void initComponents()
        {
            dataHandling = new DataHandlingImp(this);
            taskHandling = new TaskHandlingImp(this);
            updateHandling = new UpdateHandlingImp(this);
            updateHandling.onClickSelectColor(btnColor.BackColor, gr);
            updateHandling.onClickSelectSize(btnLineSize.Value + 1);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            taskHandling.onClickNewImage(ptbDraw);
        }

        private void btnPen_Click(object sender, EventArgs e)
        {
            dataHandling.onClickDrawPen();
        }

        private void btnEraser_Click(object sender, EventArgs e)
        {
            dataHandling.onClickDrawEraser();
        }

        private void btnFill_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            updateHandling.onClickSelectFill(btn, gr);
        }

        private void btnClick_Click(object sender, EventArgs e)
        {
            updateHandling.onClickSelectMode();
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            dataHandling.onClickDrawLine();
        }

        private void btnEllipse_Click(object sender, EventArgs e)
        {
            dataHandling.onClickDrawEllipse();
        }

        private void btnRectangle_Click(object sender, EventArgs e)
        {
            dataHandling.onClickDrawRectangle();
        }

        private void btnBezier_Click(object sender, EventArgs e)
        {
            dataHandling.onClickDrawBezier();
        }

        private void btnPolygon_Click(object sender, EventArgs e)
        {
            dataHandling.onClickDrawPolygon();
        }

        private void btnGroup_Click(object sender, EventArgs e)
        {
            taskHandling.onClickDrawGroup();
        }

        private void btnUnGroup_Click(object sender, EventArgs e)
        {
            taskHandling.onClickDrawUnGroup();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            taskHandling.onClickDeleteShape();
        }

        private void ptbDraw_Click(object sender, EventArgs e)
        {

        }

        private void ptbDraw_MouseClick(object sender, MouseEventArgs e)
        {
            dataHandling.onClickStopDrawing(e.Button);
        }

        private void SetMouseDown(object sender, MouseEventArgs e)
        {
            dataHandling.onClickMouseDown(e.Location);
        }

        private void SetMouseMove(object sender, MouseEventArgs e)
        {
            dataHandling.onClickMouseMove(e.Location);
        }

        private void SetMouseUp(object sender, MouseEventArgs e)
        {
            dataHandling.onClickMouseUp();
        }

        private void ptbDraw_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            dataHandling.getDrawing(e.Graphics);
        }

        public void refreshDrawing()
        {
            ptbDraw.Invalidate();
        }

        public void setCursor(Cursor cursor)
        {
            ptbDraw.Cursor = cursor;
        }

        public void setColor(Color color)
        {
            btnColor.BackColor = color;
        }

        public void setColor(Button btn, Color color)
        {
            btnColor.BackColor = color;
        }

        public void setDrawing(Shape shape, Graphics g)
        {
            shape.drawShape(g);
        }

        public void setDrawingLineSelected(Shape shape, Brush brush, Graphics g)
        {
            g.FillRectangle(brush, new System.Drawing.Rectangle(shape.pointHead.X - 4, shape.pointHead.Y - 4, 8, 8));
            g.FillRectangle(brush, new System.Drawing.Rectangle(shape.pointTail.X - 4, shape.pointTail.Y - 4, 8, 8));
        }

        public void setDrawingCurveSelected(List<Point> points, Brush brush, Graphics g)
        {
            for (int i = 0; i < points.Count; ++i)
            {
                g.FillRectangle(brush, new System.Drawing.Rectangle(points[i].X - 4, points[i].Y - 4, 8, 8));
            }
        }

        public void setDrawingRegionRectangle(Pen p, Rectangle rectangle, Graphics g)
        {
            g.DrawRectangle(p, rectangle);
        }

        public void movingShape(Shape shape, Point distance)
        {
            shape.moveShape(distance);
            refreshDrawing();
        }

        public void movingControlPoint(Shape shape, Point pointCurrent, Point previous, int indexPoint)
        {
            shape.moveControlPoint(pointCurrent, previous, indexPoint);
            refreshDrawing();
        }

        private void btnChangeColor(object sender, EventArgs e)
        {
            PictureBox ptb = sender as PictureBox;
            btnColor.BackColor = ptb.BackColor;
            updateHandling.onClickSelectColor(ptb.BackColor, gr);
        }

        private void btnLineSize_Scroll(object sender, EventArgs e)
        {
            updateHandling.onClickSelectSize(btnLineSize.Value + 1);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            taskHandling.onClickOpenImage(ptbDraw);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            taskHandling.onClickSaveImage(ptbDraw);
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            taskHandling.onClickClearAll(ptbDraw);
        }
    }
}
