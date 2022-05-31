using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace SimplePaint.Objects
{
    /// <summary>
    /// Lớp dại diện cho đường vẽ bằng pencil
    /// </summary>
    public class MyPen : DrawCurve
    {
        //TODO: cho biết chọn chế độ xóa hay là không
        public bool isEraser { get; set; }
        public MyPen()
        {
            name = "Pen";
        }

        public MyPen(Color color)
        {
            name = "Pen";
            this.color = color;
        }

    }
}
