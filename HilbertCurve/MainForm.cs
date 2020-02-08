using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ColorMine;

namespace HilbertCurve
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        Bitmap _bitmap;
        Graphics _GFX;
        int _order, _N, _total;
        PointF[] _path;
        private void MainForm_Load(object sender, EventArgs e)
        {
            _bitmap = new Bitmap(pbCanvas.Width, pbCanvas.Height);
            _GFX = Graphics.FromImage(_bitmap);
            _GFX.Clear(Color.Black);

            pbCanvas.Image = _bitmap;

            _order = 6;
            _N = (int)Math.Pow(2, _order);
            _total = _N * _N;

            _path = new PointF[_total];

            for (int i = 0; i < _total; i++)
            {
                _path[i] = Hilbert(i);
                float len = pbCanvas.Width / _N;
                _path[i].X *= len;
                _path[i].Y *= len;
                _path[i].X += len / 2;
                _path[i].Y += len / 2;
            }

            timerLoop.Enabled = true;
        }

        int counter;
        private void Loop(object sender, EventArgs e)
        {
            _GFX.Clear(Color.Black);
            for (int i = 1; i < counter; i++)
            {
                float h = i * 360 / _path.Length;
                var hsb = new ColorMine.ColorSpaces.Hsb { H = h, S = 1, B = 1 };
                Color col = Color.FromArgb((int)hsb.ToRgb().R, (int)hsb.ToRgb().G, (int)hsb.ToRgb().B);
                _GFX.DrawLine(new Pen(col), _path[i], _path[i - 1]);
            }

            counter += 1;
            if (counter >= _path.Length)
            {
                counter = 0;
            }
            pbCanvas.Image = _bitmap;
        }

        private PointF Hilbert(int i)
        {
            PointF[] points =
            {
                new PointF(0, 0),
                new PointF(0, 1),
                new PointF(1, 1),
                new PointF(1, 0)
            };

            int index = i & 3;
            PointF v = points[index];

            for (int j = 1; j < _order; j++)
            {
                i = i >> 2;
                index = i & 3;
                float len = (float)Math.Pow(2, j);
                if (index == 0)
                {
                    float temp = v.X;
                    v.X = v.Y;
                    v.Y = temp;
                } else if (index == 1)
                {
                    v.Y += len;
                } else if (index == 2)
                {
                    v.X += len;
                    v.Y += len;
                } else if (index == 3)
                {
                    float temp = len - 1 - v.X;
                    v.X = len - 1 - v.Y;
                    v.Y = temp;
                    v.X += len;
                }
            }
            return v;
        }
    }
}
