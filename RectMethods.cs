using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    class RectMethods
    {
        public static Point FindLeftCenter(Point[] points)
        {
            Point UpLeft = points.First();
            Point DownRight = points.Last();
            Point tmp = new Point(UpLeft.X, (UpLeft.Y+DownRight.Y)/2);
            return tmp;           
        }

        public static Point FindRightCenter(Point[] points) 
        {
            Point UpLeft = points.First();
            Point DownRight = points.Last();
            Point tmp = new Point(DownRight.X, (UpLeft.Y + DownRight.Y) / 2);
            return tmp;           
            
        }

        //Now this code is in DetectRect
        public static Point[] FindImportantPoints(Point[] points) 
        {
            List<Point> result = new List<Point>();
            Point UpLeft = new Point(0,0);
            Point DownRight = new Point(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            //Find Two Points of the Yellow Rectangle
            foreach(Point p in points)
            {
                if (p.X >= DownRight.X && p.Y >= DownRight.Y)
                {
                    DownRight = p;
                }
                if (p.X <= UpLeft.X && p.Y <= UpLeft.Y)
                {
                    UpLeft = p;
                }
            }
            result.Add(UpLeft);
            result.Add(DownRight);
            return result.ToArray();
        }

    }
}
