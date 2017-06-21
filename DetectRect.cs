using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace WindowsFormsApplication2
{
    class DetectRect
    {
        public static Bitmap GetScreenShot(Point TopLeft, Point DownRight)
        {
            //(x,y,width,height)
            Rectangle rect = new Rectangle(TopLeft.X, TopLeft.Y, DownRight.X-TopLeft.X, DownRight.Y-TopLeft.Y);
            Bitmap result = new Bitmap(rect.Width,rect.Height, PixelFormat.Format32bppArgb);
            {
                using (Graphics gfx = Graphics.FromImage(result))
                {
                    gfx.CopyFromScreen(rect.Left, rect.Top, 0, 0, result.Size, CopyPixelOperation.SourceCopy);
                }
            }
            //result.Save("navid.bmp", ImageFormat.Bmp);
            return result;
        }
        public static Point[] FindColorRectCoord(Color color,Point TopLeft, Point DownRight)
        {
            int searchValue = color.ToArgb();
            List<Point> result = new List<Point>();
            using (Bitmap bmp = GetScreenShot(TopLeft,DownRight))
            {
                //initialization of two points
                Point Screen_topleft = new Point(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                Point Screen_downright = new Point(0, 0);
                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        if (searchValue.Equals(bmp.GetPixel(x, y).ToArgb()))
                        {
                            Point tmp = new Point(x, y);
                            //Finding two points
                            if ((tmp.X + TopLeft.X) >= Screen_downright.X && (tmp.Y + TopLeft.Y) >= Screen_downright.Y)
                            {
                                //Screen_downright = tmp;
                                Screen_downright.X = tmp.X + TopLeft.X;
                                Screen_downright.Y = tmp.Y + TopLeft.Y;
                            }
                            if ((tmp.X + TopLeft.X) <= Screen_topleft.X && (tmp.Y + TopLeft.Y) <= Screen_topleft.Y)
                            {
                                //Screen_topleft = tmp;
                                Screen_topleft.X = tmp.X + TopLeft.X;
                                Screen_topleft.Y = tmp.Y + TopLeft.Y;

                            }
                            //result.Add(tmp);
                        }
                    }
                }
                result.Add(Screen_topleft);
                result.Add(Screen_downright);
            }
            return result.ToArray();
        }
    }

}
