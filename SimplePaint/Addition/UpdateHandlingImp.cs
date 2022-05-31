    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

using SimplePaint.Objects;


namespace SimplePaint.Addition
{
    class UpdateHandlingImp : UpdateHandling
    {
        SetUpView setUpView;

        Manage dataManager;

        public UpdateHandlingImp(SetUpView setUpView)
        {
            this.setUpView = setUpView;
            dataManager = Manage.getInstance();
        }

        public void onClickSelectMode()
        {
            dataManager.offAllShapeSelected();
            setUpView.refreshDrawing();
            dataManager.currentShape = CurrentShape.Void;
            setUpView.setCursor(Cursors.Default);
        }

        public void onClickSelectColor(System.Drawing.Color color, Graphics g)
        {
            dataManager.colorCurrent = color;
            setUpView.setColor(color);
            foreach (Shape item in dataManager.shapeList)
            {
                if (item.isSelected)
                {
                    item.color = color;
                    setUpView.setDrawing(item, g);
                }
            }
        }

        public void onClickSelectSize(int size)
        {
            dataManager.lineSize = size;
        }

        public void onClickSelectFill(Button btn, Graphics g)
        {
            dataManager.isFill = !dataManager.isFill;
            if (btn.BackColor.Equals(Color.LightCyan))
                setUpView.setColor(btn, SystemColors.Control);
            else
                setUpView.setColor(btn, Color.LightCyan);
        }

    }
}
