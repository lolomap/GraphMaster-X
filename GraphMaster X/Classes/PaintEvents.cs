using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace GraphMaster_X.Classes
{
    public class PaintEvents
    {
        public delegate void PaintPointEventHandler(Point point);
        public static event PaintPointEventHandler PainPointEvent;
        public static void CallPaintPointEvent(Point point) { PainPointEvent(point); }

        public delegate Line PaintLineEventHandler(Point p1, Point p2);
        public static event PaintLineEventHandler PaintLineEvent;
        public static void CallPaintLineEvent(Point p1, Point p2) { PaintLineEvent(p1, p2); }
    }
}
