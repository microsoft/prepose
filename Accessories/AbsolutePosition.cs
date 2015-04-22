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

    public class AbsolutePosition : Position
    {


        public double x { get; set; }
        public double y { get; set; }

        public AbsolutePosition(double x0, double y0)
        {
            x = x0;
            y = y0;
        }

        public double Distance()
        {
            return Math.Sqrt(Math.Pow(x,2)+Math.Pow(y,2));
        }

        public AbsolutePosition increment(RelativePosition p)
        {
            if(p!=null)
               return new AbsolutePosition(x + p.x, y + p.y);
            return null;
        }

        public Point getPoint()
        {
            return new Point(x, y);
        }
    }
}
