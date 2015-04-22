using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.Kinect.GestureRecognizer
{
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Kinect;

    public class RelativePosition : Position
    {
        public double x { get; set; }
        public double y { get; set; }

        public RelativePosition (double x0, double y0){
            x = x0;
            y = y0;
        }

        public double Distance()
        {
            return Math.Sqrt(Math.Pow(x,2)+Math.Pow(y,2));
        }

        public double Difference(RelativePosition p1)
        {
            return Math.Pow(x - p1.x,2) + Math.Pow(y - p1.y,2);
        }

        public void Rotate(double degree)
        {
            x= (Math.Cos(degree) * x) - (Math.Sin(degree) * y);
            y= (Math.Sin(degree) * x) + (Math.Cos(degree) * y);
        }

        public void TranslateHorizontal(double dist)
        {
            x = x + dist;           
        }

        public void TranslateVertical(double dist)
        {
            y = y + dist;
        }

        public void print() {
            Console.Write("{0},{1}", x, y);
        }


    }
}
