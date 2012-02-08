using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace RHITMobileUtil
{
    public class Trilaterator : Form
    {
        private const int _cornerRadius = 4;
        private List<Corner> _corners = new List<Corner>();
        private List<Point> _points = new List<Point>();
        private Bitmap _bitmap;

        public Trilaterator()
        {
            _bitmap = new Bitmap(Program.trifile);
            Size = new Size(_bitmap.Width + 16, _bitmap.Height + 40);
            MaximumSize = Size;
            MinimumSize = Size;
            Paint += new PaintEventHandler(MainForm_Paint);
            MouseClick += new MouseEventHandler(MainForm_Click);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            g.DrawImage(_bitmap, new Rectangle(0, 0, _bitmap.Width, _bitmap.Height));

            foreach (var corner in _corners)
            {
                g.FillEllipse(Brushes.Red, corner.X - _cornerRadius, corner.Y - _cornerRadius, _cornerRadius * 2, _cornerRadius * 2);
                g.DrawEllipse(new Pen(Color.Black), corner.X - _cornerRadius, corner.Y - _cornerRadius, _cornerRadius * 2, _cornerRadius * 2);
            }

            foreach (var point in _points)
            {
                g.FillEllipse(Brushes.LimeGreen, point.X - _cornerRadius, point.Y - _cornerRadius, _cornerRadius * 2, _cornerRadius * 2);
                g.DrawEllipse(new Pen(Color.Black), point.X - _cornerRadius, point.Y - _cornerRadius, _cornerRadius * 2, _cornerRadius * 2);
            }
        }

        private void MainForm_Click(object sender, MouseEventArgs e)
        {
            Console.Write("Type 'c' for corner, 'p' for point, 'd' to delete, 'a' to delete all, or any other key to cancel: ");
            var key = Console.ReadKey();
            Console.WriteLine();
            switch (key.KeyChar)
            {
                case 'c':
                    string data = Clipboard.GetText();
                    int commapos = data.IndexOf(',');
                    if (commapos > 0)
                    {
                        double lat;
                        if (Double.TryParse(data.Substring(0, commapos), out lat))
                        {
                            double lon;
                            if (Double.TryParse(data.Substring(commapos + 1), out lon))
                            {
                                _corners.Add(new Corner(lat, lon, e.X, e.Y));
                                Console.WriteLine("Corner added.");
                                Refresh();
                            }
                            else
                            {
                                Console.WriteLine("Bad longitude in clipboard.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Bad latitude in clipboard.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No comma in clipboard.");
                    }
                    break;
                case 'p':
                    if (_corners.Count() >= 3)
                    {
                        _corners.Sort((corner1, corner2) => DistSqr(corner1.X, corner1.Y, e.X, e.Y) - DistSqr(corner2.X, corner2.Y, e.X, e.Y));
                        double lat = 0;
                        double lon = 0;
                        if (e.X == _corners[0].X && e.Y == _corners[0].Y)
                        {
                            lat = _corners[0].Lat;
                            lon = _corners[0].Lon;
                        }
                        else
                        {
                            // Line 1: e, _corners[0]
                            // Line 2: _corners[1], _corners[2]
                            double q = (e.Y - _corners[1].Y) * (_corners[2].X - _corners[1].X) - (e.X - _corners[1].X) * (_corners[2].Y - _corners[1].Y);
                            double d = (_corners[0].X - e.X) * (_corners[2].Y - _corners[1].Y) - (_corners[0].Y - e.Y) * (_corners[2].X - _corners[1].X);
                            if (d == 0)
                            {
                                Console.WriteLine("Lines are parallel.");
                                break;
                            }
                            double r = q / d;
                            q = (e.Y - _corners[1].Y) * (_corners[0].X - e.X) - (e.X - _corners[1].X) * (_corners[0].Y - e.Y);
                            double s = q / d;

                            //double iX = e.X + r * (_corners[0].X - e.X);
                            //double iY = e.Y + r * (_corners[0].Y - e.Y);

                            lat = (s * _corners[1].Lat + r * _corners[0].Lat - s * _corners[2].Lat - _corners[1].Lat) / (r - 1);
                            lon = (s * _corners[1].Lon + r * _corners[0].Lon - s * _corners[2].Lon - _corners[1].Lon) / (r - 1);
                        }
                        Clipboard.SetText(String.Format("{0},{1}", lat, lon));
                        _points.Add(new Point(e.X, e.Y));
                        Console.WriteLine("Point added.");
                        Refresh();
                    }
                    else
                    {
                        Console.WriteLine("Must have at least three corners.");
                    }
                    break;
                case 'd':
                    int numCorners = _corners.Count();
                    int numPoints = _points.Count();
                    _corners = _corners.Where(corner => DistSqr(corner.X, corner.Y, e.X, e.Y) > _cornerRadius * _cornerRadius).ToList();
                    _points = _points.Where(point => DistSqr(point.X, point.Y, e.X, e.Y) > _cornerRadius * _cornerRadius).ToList();
                    Console.WriteLine("{0} corner(s) and {1} point(s) deleted.", numCorners - _corners.Count(), numPoints - _points.Count());
                    Refresh();
                    break;
                case 'a':
                    numCorners = _corners.Count();
                    numPoints = _points.Count();
                    _corners.Clear();
                    _points.Clear();
                    Console.WriteLine("{0} corner(s) and {1} point(s) deleted.", numCorners - _corners.Count(), numPoints - _points.Count());
                    Refresh();
                    break;
                default:
                    Console.WriteLine("Click canceled.");
                    break;
            }
            Console.WriteLine();
        }

        private static int DistSqr(int x1, int y1, int x2, int y2)
        {
            return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
        }
    }

    public class Corner
    {
        public Corner(double lat, double lon, int x, int y)
        {
            Lat = lat;
            Lon = lon;
            X = x;
            Y = y;
        }

        public double Lat { get; set; }
        public double Lon { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
