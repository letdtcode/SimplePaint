using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using SimplePaint.Objects;


namespace SimplePaint.Addition
{
    class DataHandlingImp : DataHandling
    {
        SetUpView setUpView;

        Manage dataManager;

        public DataHandlingImp(SetUpView setUpView)
        {
            this.setUpView = setUpView;
            dataManager = Manage.getInstance();
        }

        public void onClickMouseDown(Point p)
        {
            dataManager.isSave = false;
            dataManager.isNotNone = true;
            if (dataManager.currentShape.Equals(CurrentShape.Void))
            {
                if (!(Control.ModifierKeys == Keys.Control))
                    dataManager.offAllShapeSelected();
                setUpView.refreshDrawing();
                handleClickToSelect(p);
            }
            else
            {
                handleClickToDraw(p);
            }
        }

        public void handleClickToSelect(Point p)
        {
            for (int i = 0; i < dataManager.shapeList.Count; ++i)
            {
                if (!(dataManager.shapeList[i] is MyPen))
                    dataManager.pointToResize = dataManager.shapeList[i].isHitControlsPoint(p);
                if (dataManager.pointToResize != -1)
                {
                    dataManager.shapeList[i].changePoint(dataManager.pointToResize);
                    dataManager.shapeToMove = dataManager.shapeList[i];
                    break;
                }
                else if (dataManager.shapeList[i].isHit(p))
                {
                    dataManager.shapeToMove = dataManager.shapeList[i];
                    dataManager.shapeList[i].isSelected = true;
                    if (dataManager.shapeList[i] is MyPen)
                    {
                        if (((MyPen)dataManager.shapeList[i]).isEraser)
                        {
                            dataManager.shapeList[i].isSelected = false;
                            dataManager.shapeToMove = null;
                        }
                    }
                    break;
                }
            }

            if (dataManager.pointToResize != -1)
            {
                dataManager.cursorCurrent = p;
            }
            else if (dataManager.shapeToMove != null)
            {
                dataManager.isMovingShape = true;
                dataManager.cursorCurrent = p;
            }
            else
            {
                dataManager.isMovingMouse = true;
                dataManager.rectangleRegion = new Rectangle(p, new Size(0, 0));
            }
        }

        private void handleClickToDraw(Point p)
        {
            dataManager.isMouseDown = true;
            dataManager.offAllShapeSelected();
            if (dataManager.currentShape.Equals(CurrentShape.Line))
            {
                dataManager.addEntity(new DrawLine
                {
                    pointHead = p,
                    pointTail = p,
                    contourWidth = dataManager.lineSize,
                    color = dataManager.colorCurrent,
                    isFill = dataManager.isFill
                });
            }
            else if (dataManager.currentShape.Equals(CurrentShape.Rectangle))
            {
                dataManager.addEntity(new DrawRectangle
                {
                    pointHead = p,
                    pointTail = p,
                    contourWidth = dataManager.lineSize,
                    color = dataManager.colorCurrent,
                    isFill = dataManager.isFill
                });
            }
            else if (dataManager.currentShape.Equals(CurrentShape.Ellipse))
            {
                dataManager.addEntity(new DrawEllipse
                {
                    pointHead = p,
                    pointTail = p,
                    contourWidth = dataManager.lineSize,
                    color = dataManager.colorCurrent,
                    isFill = dataManager.isFill
                });
            }

            else if (dataManager.currentShape.Equals(CurrentShape.Curve))
            {
                if (!dataManager.isDrawingCurve)
                {
                    dataManager.isDrawingCurve = true;
                    DrawCurve bezier = new DrawCurve
                    {
                        color = dataManager.colorCurrent,
                        contourWidth = dataManager.lineSize,
                        isFill = dataManager.isFill
                    };
                    bezier.points.Add(p);
                    bezier.points.Add(p);
                    dataManager.shapeList.Add(bezier);
                }
                else
                {
                    DrawCurve bezier = dataManager.shapeList[dataManager.shapeList.Count - 1] as DrawCurve;
                    bezier.points[bezier.points.Count - 1] = p;
                    bezier.points.Add(p);
                }
                dataManager.isMouseDown = false;
            }
            else if (dataManager.currentShape.Equals(CurrentShape.Polygon))
            {
                if (!dataManager.isDrawingPolygon)
                {
                    dataManager.isDrawingPolygon = true;
                    DrawPolygon polygon = new DrawPolygon
                    {
                        color = dataManager.colorCurrent,
                        contourWidth = dataManager.lineSize,
                        isFill = dataManager.isFill

                    };
                    polygon.points.Add(p);
                    polygon.points.Add(p);
                    dataManager.shapeList.Add(polygon);
                }
                else
                {
                    DrawPolygon polygon = dataManager.shapeList[dataManager.shapeList.Count - 1] as DrawPolygon;
                    polygon.points[polygon.points.Count - 1] = p;
                    polygon.points.Add(p);
                }
                dataManager.isMouseDown = false;
            }
            else if (dataManager.currentShape.Equals(CurrentShape.Pen))
            {
                dataManager.isDrawingPen = true;
                MyPen pen = new MyPen
                {
                    color = dataManager.colorCurrent,
                    contourWidth = dataManager.lineSize,
                    isFill = dataManager.isFill
                };
                pen.points.Add(p);
                pen.points.Add(p);
                dataManager.shapeList.Add(pen);
            }
            else if (dataManager.currentShape.Equals(CurrentShape.Eraser))
            {
                dataManager.isDrawingEraser = true;
                MyPen pen = new MyPen
                {
                    color = Color.White,
                    contourWidth = dataManager.lineSize
                };
                pen.isEraser = true;
                pen.points.Add(p);
                pen.points.Add(p);
                dataManager.shapeList.Add(pen);
            }
        }

        public void onClickMouseMove(Point p)
        {
            if (dataManager.isMouseDown)
            {
                dataManager.updatePointTail(p);
                setUpView.refreshDrawing();
            }
            else if (dataManager.pointToResize != -1)
            {
                if (!(dataManager.shapeToMove is GroupShape) && !(dataManager.shapeToMove is MyPen))
                {
                    setUpView.movingControlPoint(dataManager.shapeToMove,
                        p, dataManager.cursorCurrent,
                        dataManager.pointToResize);
                    dataManager.cursorCurrent = p;
                }

            }
            else if (dataManager.isMovingShape)
            {
                setUpView.movingShape(dataManager.shapeToMove, dataManager.distanceXY(dataManager.cursorCurrent, p));
                dataManager.cursorCurrent = p;
            }
            else if (dataManager.currentShape.Equals(CurrentShape.Void))
            {
                if (dataManager.isMovingMouse)
                {
                    dataManager.updateRectangleRegion(p);
                    setUpView.refreshDrawing();
                }
                else
                {

                    //TODO: Kiếm tra xem trong danh sách tồn tại hình nào chứa điểm p không
                    if (dataManager.shapeList.Exists(shape => isInside(shape, p)))
                    {
                        setUpView.setCursor(Cursors.SizeAll);
                    }
                    else
                    {
                        setUpView.setCursor(Cursors.Default);
                    }

                }
            }

            if (dataManager.isDrawingCurve)
            {
                DrawCurve bezier = dataManager.shapeList[dataManager.shapeList.Count - 1] as DrawCurve;
                bezier.points[bezier.points.Count - 1] = p;
                setUpView.refreshDrawing();
            }
            else if (dataManager.isDrawingPolygon)
            {
                DrawPolygon polygon = dataManager.shapeList[dataManager.shapeList.Count - 1] as DrawPolygon;
                polygon.points[polygon.points.Count - 1] = p;
                setUpView.refreshDrawing();
            }
            else if (dataManager.isDrawingPen)
            {
                MyPen pen = dataManager.shapeList[dataManager.shapeList.Count - 1] as MyPen;
                pen.points.Add(p);
                RegionShape.setPointHeadTail(pen);
                setUpView.refreshDrawing();
            }
            else if (dataManager.isDrawingEraser)
            {
                MyPen pen = dataManager.shapeList[dataManager.shapeList.Count - 1] as MyPen;
                pen.points.Add(p);
                RegionShape.setPointHeadTail(pen);
                setUpView.refreshDrawing();
            }
        }

        private bool isInside(Shape shape, Point p)
        {
            if (shape is MyPen)
            {
                MyPen pen = shape as MyPen;
                if (pen.isEraser) return false;
                return true;
            }
            return shape.isHit(p);
        }

        public void onClickMouseUp()
        {
            dataManager.isMouseDown = false;
            if (dataManager.pointToResize != -1)
            {
                dataManager.pointToResize = -1;
                dataManager.shapeToMove = null;
            }
            else if (dataManager.isMovingShape)
            {
                dataManager.isMovingShape = false;
                dataManager.shapeToMove = null;
            }
            else if (dataManager.isMovingMouse)
            {
                dataManager.isMovingMouse = false;
                dataManager.offAllShapeSelected();

                //TODO: kiểm tra khi kéo chuột chọn một vùng thì có hình nào tồn tại bên
                //trong hay là không, nếu có thì hình đó được chọn
                for (int i = 0; i < dataManager.shapeList.Count; ++i)
                {
                    if (dataManager.shapeList[i].isInRegion(dataManager.rectangleRegion))
                    {
                        dataManager.shapeList[i].isSelected = true;
                    }
                    if (dataManager.shapeList[i] is MyPen)
                    {
                        MyPen pen = dataManager.shapeList[i] as MyPen;
                        if (pen.isEraser)
                            dataManager.shapeList[i].isSelected = false;
                    }
                }
                setUpView.refreshDrawing();
            }
            if (dataManager.isDrawingPen)
            {
                dataManager.isDrawingPen = false;
            }
            else if (dataManager.isDrawingEraser)
            {
                dataManager.isDrawingEraser = false;
            }
        }

        public void getDrawing(Graphics g)
        {
            dataManager.shapeList.ForEach(shape =>
            {
                setUpView.setDrawing(shape, g);
                if (shape.isSelected)
                {
                    drawRegionForShape(shape, g);
                }

            });
            if (dataManager.isMovingMouse)
            {
                using (Pen pen = new Pen(Color.DarkBlue, 1)
                {
                    DashPattern = new float[] { 3, 3, 3, 3 },
                    DashStyle = DashStyle.Custom
                })
                {
                    setUpView.setDrawingRegionRectangle(pen, dataManager.rectangleRegion, g);
                }

            }
            if (dataManager.pointToResize != -1)
            {
                if (dataManager.shapeToMove is GroupShape) return;
                drawRegionForShape(dataManager.shapeToMove, g);
            }
        }

        private void drawRegionForShape(Shape shape, Graphics g)
        {
            if (shape is DrawLine)
            {
                setUpView.setDrawingLineSelected(shape, new SolidBrush(Color.DarkBlue), g);

            }
            else if (shape is MyPen)
            {
                if (!((MyPen)shape).isEraser)
                {
                    using (Pen pen = new Pen(Color.DarkBlue, 1)
                    {
                        DashPattern = new float[] { 3, 3, 3, 3 },
                        DashStyle = DashStyle.Custom
                    })
                    {
                        setUpView.setDrawingRegionRectangle(pen, shape.getRectangle(), g);
                    }
                }
            }
            else if (shape is DrawCurve)
            {
                DrawCurve curve = (DrawCurve)shape;
                for (int i = 0; i < curve.points.Count; i++)
                {
                    setUpView.setDrawingCurveSelected(curve.points, new SolidBrush(Color.DarkBlue), g);
                }
            }
            else if (shape is DrawPolygon)
            {
                DrawPolygon polygon = (DrawPolygon)shape;
                for (int i = 0; i < polygon.points.Count; i++)
                {
                    setUpView.setDrawingCurveSelected(polygon.points, new SolidBrush(Color.DarkBlue), g);
                }
            }
            else
            {
                using (Pen pen = new Pen(Color.DarkBlue, 1)
                {
                    DashPattern = new float[] { 3, 3, 3, 3 },
                    DashStyle = DashStyle.Custom
                })
                {
                    setUpView.setDrawingRegionRectangle(pen, shape.getRectangle(shape.pointHead, shape.pointTail), g);
                    setUpView.setDrawingCurveSelected(RegionShape.getControlPoints(shape),
                        new SolidBrush(Color.DarkBlue), g);
                }
            }
        }

        public void onClickDrawLine()
        {
            setDefaultToDraw();
            dataManager.currentShape = CurrentShape.Line;
        }

        public void onClickDrawRectangle()
        {
            setDefaultToDraw();
            dataManager.currentShape = CurrentShape.Rectangle;
        }

        public void onClickDrawEllipse()
        {
            setDefaultToDraw();
            dataManager.currentShape = CurrentShape.Ellipse;
        }

        public void onClickDrawBezier()
        {
            setDefaultToDraw();
            dataManager.currentShape = CurrentShape.Curve;
        }

        public void onClickDrawPolygon()
        {
            setDefaultToDraw();
            dataManager.currentShape = CurrentShape.Polygon;
        }

        public void onClickDrawPen()
        {
            setDefaultToDraw();
            dataManager.currentShape = CurrentShape.Pen;
        }

        public void onClickDrawEraser()
        {
            setDefaultToDraw();
            dataManager.currentShape = CurrentShape.Eraser;
        }

        public void onClickStopDrawing(MouseButtons mouse)
        {
            if (mouse == MouseButtons.Right)
            {
                if (dataManager.currentShape.Equals(CurrentShape.Polygon))
                {
                    DrawPolygon polygon = dataManager.shapeList[dataManager.shapeList.Count - 1] as DrawPolygon;
                    polygon.points.Remove(polygon.points[polygon.points.Count - 1]);
                    dataManager.isDrawingPolygon = false;
                    RegionShape.setPointHeadTail(polygon);
                }
                else if (dataManager.currentShape.Equals(CurrentShape.Curve))
                {
                    DrawCurve curve = dataManager.shapeList[dataManager.shapeList.Count - 1] as DrawCurve;
                    curve.points.Remove(curve.points[curve.points.Count - 1]);
                    dataManager.isDrawingCurve = false;
                    RegionShape.setPointHeadTail(curve);
                }
            }
        }

        private void setDefaultToDraw()
        {
            dataManager.offAllShapeSelected();
            setUpView.refreshDrawing();
            setUpView.setCursor(Cursors.Default);
        }

    }
}
