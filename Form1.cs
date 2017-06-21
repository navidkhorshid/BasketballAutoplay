using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using Utilities;
using System.IO;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        //HOOK from Utilities
        globalKeyboardHook gkh = new globalKeyboardHook();

        
        private const int leftDown = 0x02;
        private const int leftUp = 0x04;

        private static Point Ball = new Point();
        private static Point Target = new Point();
        private bool flag_ball = false;
        private static bool dragging = false;

        //Top and Down
        private static Point TopLeft = new Point();
        private static Point DownRight = new Point();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Watch for keys B and T
            gkh.HookedKeys.Add(Keys.B);
            gkh.HookedKeys.Add(Keys.T);
            gkh.HookedKeys.Add(Keys.A);
            gkh.HookedKeys.Add(Keys.M);
            //Top and Down
            gkh.HookedKeys.Add(Keys.U);
            gkh.HookedKeys.Add(Keys.I);

            gkh.KeyDown += new KeyEventHandler(gkh_KeyDown);
        }


        void gkh_KeyDown(object sender, KeyEventArgs e)
        {
            //lstLog.Items.Add("Down\t" + e.KeyCode.ToString());
            if (e.KeyCode.ToString().Equals("B"))
            {
                Ball = Cursor.Position;
                //To make sure the Ball position is set
                flag_ball = true;
                //set textboxes
                textBox1.Text = Ball.X.ToString();
                textBox2.Text = Ball.Y.ToString();
            }
            if (e.KeyCode.ToString().Equals("T"))
            {
                trigger_target(Cursor.Position);
            }
            if (e.KeyCode.ToString().Equals("A"))
            {
                //#D3C05B - DARK Yellow
                //#F7E16A - BRIGHT Yellow
                //if upper left and down right points are defined by user then
                if (!textBox7.Text.Equals("") && !textBox8.Text.Equals("") && !textBox9.Text.Equals("") && !textBox10.Text.Equals("")) 
                {
                    Color _color = System.Drawing.ColorTranslator.FromHtml("#D3C05B");
                    Point[] points = DetectRect.FindColorRectCoord(_color, TopLeft, DownRight);
                    trigger_target(RectMethods.FindLeftCenter(points));
                }
                else { MessageBox.Show("Enter UpperLeft and DownRight points."); }
            }
            if (e.KeyCode.ToString().Equals("M"))
            {
                if (!textBox7.Text.Equals("") && !textBox8.Text.Equals("") && !textBox9.Text.Equals("") && !textBox10.Text.Equals(""))
                {
                    start_timer();  
                }
                else { MessageBox.Show("Enter UpperLeft and DownRight points."); }
            }
            if (e.KeyCode.ToString().Equals("U"))
            {
                TopLeft = Cursor.Position;
                //set textboxes
                textBox8.Text = TopLeft.X.ToString();
                textBox7.Text = TopLeft.Y.ToString();
            }
            if (e.KeyCode.ToString().Equals("I"))
            {
                DownRight = Cursor.Position;
                //set textboxes
                textBox10.Text = DownRight.X.ToString();
                textBox9.Text = DownRight.Y.ToString();
            }
            e.Handled = true;
        }

        public void start_timer()
        {
            //timer2.Interval = int.Parse(numericUpDown2.Value.ToString()) * 20;
            timer2.Enabled = true;
            
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            //timer1.Interval = int.Parse(numericUpDown2.Value.ToString()) * 20;
            List<Point> points = new List<Point>();

            Color _color = System.Drawing.ColorTranslator.FromHtml("#D3C05B");
            points = DetectRect.FindColorRectCoord(_color, TopLeft, DownRight).ToList();
            //if downright is in left from middle of X in screen
            int diff = Ball.X - points.First().X;// check if the basket is on the left
            if (diff >= 70)
            {
                timer1.Enabled = true;
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            Color _color = System.Drawing.ColorTranslator.FromHtml("#D3C05B");
            Point[] points = DetectRect.FindColorRectCoord(_color, TopLeft, DownRight);
            Point tmp = RectMethods.FindRightCenter(points);
            //agar y payeentar bood bayad zoodtar raha beshe: yani be constant hatman Y ezafe beshe
            int constant = 45;
            switch ((tmp.Y-TopLeft.Y)/35) //Difference of Y is nearly 380: so in order to have 5 division we need to divide by 78 or sth 
            {
                case 1:
                    constant += 7;
                    break;
                case 2:
                    constant += 8;
                    break;
                case 3:
                    constant += 8;
                    break;
                case 4:
                    constant += 9;
                    break;
                case 5:
                    constant += 9;
                    break;
                case 6:
                    constant += 10;
                    break;
                case 7:
                    constant += 11;
                    break;
                case 8:
                    constant += 12;
                    break;
                case 9:
                    constant += 13;
                    break;
                case 10:
                    constant += 14;
                    break;

            }
            if ((Ball.X - tmp.X) >= 1 && (Ball.X - tmp.X) <= constant)
            {
                Point middle = new Point(Ball.X, tmp.Y);
                trigger_target(middle);
                timer1.Enabled = false;
            }  
        }


        

        public void trigger_target(Point target) 
        {
            Target = target;
            //set textboxes
            textBox6.Text = Target.X.ToString();
            textBox5.Text = Target.Y.ToString();
            //TRIGGER DRAGGING if we know where the ball center is
            if (flag_ball)
            {
                //Divide Value from NumericUpDown
                int DB = int.Parse(numericUpDown1.Value.ToString());
                //Find a point which is 1/n from Ball's Point on the line BALL-TARGET
                //Geometry behind this idea
                //https://math.stackexchange.com/questions/563566/how-do-i-find-the-middle1-2-1-3-1-4-etc-of-a-line
                int x_new = (((DB - 1) * Ball.X) + (1 * Target.X)) / DB;
                int y_new = (((DB - 1) * Ball.Y) + (1 * Target.Y)) / DB;

                //set textboxes
                textBox3.Text = x_new.ToString();
                textBox4.Text = y_new.ToString();

                //Start Drag and Dropping
                Grab(Ball.X, Ball.Y);
                Release(x_new, y_new);
            }
        }

        public static void Grab(int xPos, int yPos)
        {
            dragging = true;

            Cursor.Position = new Point(xPos, yPos);
            mouse_event(leftDown, (uint)xPos, (uint)yPos, 0, 0);
            
            var t = new Thread(CheckMouseStatus);
            t.Start();
        }
        public static void Release(int xPos, int yPos)
        {
            dragging = false;
            Cursor.Position = new Point(xPos, yPos);
            mouse_event(leftUp, (uint)xPos, (uint)yPos, 0, 0);
            
        }

        private static void CheckMouseStatus()
        {
            do
            {
                Cursor.Position = new Point();
            }
            while (dragging);
            //Return to the place clicked in the first place
            Cursor.Position = Target;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer2.Enabled = false;
        }


        
        

    }
}
