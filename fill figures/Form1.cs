using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace fill_figures
{
    public partial class Form1 : Form
    {
        const int sizeX = 400;
        const int sizeY = 400;
        Bitmap _bmp = new Bitmap(sizeX,sizeY);
        bool isDraw;
        Pen _pen = new  Pen(Color.Black, 20);
        Brush _brush = new SolidBrush(Color.Black);
        Color old_color;

        //int num_of_points;
        List<Point> points = new List<Point>();
        public Form1()
        {
          InitializeComponent();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isDraw = true;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isDraw = false;
        }
        void Drawing (Bitmap bmp, Brush brush, Point point)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.FillEllipse(Brushes.Black, point.X, point.Y, 20, 20);
            
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDraw)
            {
                Drawing(_bmp, _brush, new Point(e.X, e.Y));
                points.Add(new Point(e.X, e.Y));
                pictureBox1.Image = _bmp;
            }
        }
        void Brezenhem(int x1, int y1, int x2, int y2)
        {
            int deltaX = Math.Abs(x2 - x1);
            int deltaY = Math.Abs(y2 - y1);
            int signX = x1 < x2 ? 1 : -1;
            int signY = y1 < y2 ? 1 : -1;
            
            int error = deltaX - deltaY;
            Drawing(_bmp, _brush, new Point(x2, y2));
            points.Add(new Point(x2, y2));
            while (x1 != x2 || y1 != y2)
            {
                Drawing(_bmp, _brush, new Point(x1, y1));
                points.Add(new Point(x1, y1));
                int error2 = error * 2;

                if (error2 > -deltaY)
                {
                    error -= deltaY;
                    x1 += signX;
                }
                if (error2 < deltaX)
                {
                    error += deltaX;
                    y1 += signY;
                }
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            _bmp = new Bitmap(sizeX, sizeY);
            pictureBox1.Image = _bmp;
           
            Brezenhem(100, 100, 200, 100);
            Brezenhem(200, 100, 200, 150);
            Brezenhem(200, 150, 100, 300);
            Brezenhem(100, 300, 100, 100);
            pictureBox1.Image = _bmp;
            
        }

        void Recursive(Bitmap bmp, int x, int y, Color oldColor, Color newColor)
        {
            if (0 <= x && x < sizeX && 0 <= y && y < sizeY && bmp.GetPixel(x, y) == oldColor)
            {
                bmp.SetPixel(x, y, newColor);
                Recursive(bmp, x - 1, y, oldColor, newColor);
                Recursive(bmp, x, y - 1, oldColor, newColor);
                Recursive(bmp, x, y + 1, oldColor, newColor);
                Recursive(bmp, x + 1, y, oldColor, newColor);
            }

        }
        void Loop(Bitmap bmp, Point startPoint, Color oldColor, Color newColor)
        {
            List<int> x = new List<int>();
            List<int> y = new List<int>();
            x.Add(startPoint.X);
            y.Add(startPoint.Y);

            while (x.Count != 0)
            {
                if (0 <= x[0] && x[0] < sizeX && 0 <= y[0] && y[0] < sizeY && bmp.GetPixel(x[0], y[0]) == oldColor)
                {
                    bmp.SetPixel(x[0], y[0], newColor);

                    x.Add(x[0] - 1);
                    y.Add(y[0]);

                    x.Add(x[0]);
                    y.Add(y[0] - 1);

                    x.Add(x[0]);
                    y.Add(y[0] + 1);

                    x.Add(x[0] + 1);
                    y.Add(y[0]);
                }
                x.RemoveAt(0);
                y.RemoveAt(0);
            }
        }
        void Algorithm(List<Point> points)
        {
            for (int i = 0; i < points.Count(); i++) // i - номер текущего шага
            {
                int pos = i;
                int tmp = points[i].Y;
                Point tmp1 = points[i];
                for (int j = i + 1; j < points.Count(); j++) 
                {
                    if (points[j].Y < tmp)
                    {
                        pos = j;
                        tmp = points[j].Y;
                        tmp1 = points[j];
                    }
                }
                points[pos] = points[i];
                points[i] = tmp1; 
            }
            for (int inx = 0; inx < points.Count; inx++)
            {

                Point buf = new Point(points[inx].X, points[inx].Y);
                Point final_point;

                for (int i = inx; i < points.Count; i++)
                {
                    final_point = new Point(points[i].X + 2, points[i].Y);
                    int tmp = points[i].Y - buf.Y;
                    if (points[i].X == buf.X + 1) break;
                    else
                    {
                        if ((points[i].Y == buf.Y) || tmp < 2)
                        {
                            using (Graphics g = Graphics.FromImage(_bmp))
                            {
                                g.DrawLine(_pen, final_point, buf);
                            }
                        }
                    }
                }
            }

            points.Clear();
        }
        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            if (radioButton1.Checked)
            {
                old_color = _bmp.GetPixel(e.X, e.Y);
                Recursive(_bmp, e.X, e.Y, old_color, Color.Black);
                pictureBox1.Image = _bmp;
                _bmp.Save("1.bmp");
            }
            else if (radioButton2.Checked)
            {
                old_color = _bmp.GetPixel(e.X, e.Y);
                Loop(_bmp, new Point(e.X,e.Y), old_color, Color.Black);
                pictureBox1.Image = _bmp;
                _bmp.Save("1.bmp");
            }
            else if (radioButton3.Checked)
            {
                Algorithm(points);
                pictureBox1.Image = _bmp;
                _bmp.Save("1.bmp");
            }
            else
            {
                MessageBox.Show("Please, choose any algorithm!");
            }
        }

    }
}
