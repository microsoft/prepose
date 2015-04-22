//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace BodyBasicsWPF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Controls;
    using Microsoft.Automata;
    using System.Threading;
    using Microsoft.Kinect;
    using Microsoft.Samples.Kinect.GestureRecognizer;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Radius of drawn hand circles
        /// </summary>
        private const double HandSize = 30;

        //test code
        private List<Vector4> lastOrientations;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as closed
        /// </summary>
        private readonly Brush handClosedBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as opened
        /// </summary>
        private readonly Brush handOpenBrush = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as in lasso (pointer) position
        /// </summary>
        private readonly Brush handLassoBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Drawing group for body rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor = null;

        /// <summary>
        /// Coordinate mapper to map one type of point to another
        /// </summary>
        private CoordinateMapper coordinateMapper = null;

        /// <summary>
        /// Reader for body frames
        /// </summary>
        private BodyFrameReader bodyFrameReader = null;

        /// <summary>
        /// Array for the bodies
        /// </summary>
        private Body[] bodies = null;

        /// <summary>
        /// Width of display (depth space)
        /// </summary>
        private int displayWidth;

        /// <summary>
        /// Height of display (depth space)
        /// </summary>
        private int displayHeight;

        /// <summary>
        /// The time of the first frame received
        /// </summary>
        private TimeSpan startTime = new TimeSpan(0);

        /// <summary>
        /// Current status text to display
        /// </summary>
        private string statusText = null;

        /// <summary>
        /// Next time to update FPS/frame time status
        /// </summary>
        private DateTime nextStatusUpdate = DateTime.MinValue;

        /// <summary>
        /// Number of frames since last FPS/frame time status
        /// </summary>
        private uint framesSinceUpdate = 0;

        /// <summary>
        /// Timer for FPS calculation
        /// </summary>
        private Stopwatch stopwatch = null;

        //My vars
        private TreeBodyPart bodyPartTree;
        private List<TreeBodyPart> shadows;
        private List<Transduction> moves;
        private TreeBodyPart shadow;
        private bool updated = false;
        private bool matched = false;
        private bool started = false;
        private bool first = false;
        MoveSequence moveSeq;
        private List<Pen> myPen;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            // create a stopwatch for FPS calculation
            this.stopwatch = new Stopwatch();

            //test cpde
            this.lastOrientations = new List<Vector4>();

            // for Alpha, one sensor is supported
            this.kinectSensor = KinectSensor.GetDefault();

            if (this.kinectSensor != null)
            {
                // get the coordinate mapper
                this.coordinateMapper = this.kinectSensor.CoordinateMapper;

                // open the sensor
                this.kinectSensor.Open();

                // get the depth (display) extents
                FrameDescription frameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;
                this.displayWidth = frameDescription.Width;
                this.displayHeight = frameDescription.Width;

                this.bodies = new Body[this.kinectSensor.BodyFrameSource.BodyCount];

                // open the reader for the body frames
                this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

                // set the status text
                this.StatusText = Properties.Resources.InitializingStatusTextFormat;
            }
            else
            {
                // on failure, set the status text
                this.StatusText = Properties.Resources.NoSensorStatusText;
            }

            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // use the window object as the view model in this simple example
            this.DataContext = this;

            // initialize the components (controls) of the window
            this.InitializeComponent();

            //
            List<Transduction> movs = TransductionUtil.GetAll();
            TextBlock tb;
            foreach (Transduction m in movs)
            {
                tb = new TextBlock();
                //i = Math.DivRem(count, movs.Count, out j);                
                tb.Background = Brushes.Black;
                tb.Foreground = TransductionUtil.ToColor(m).Brush;
                tb.Text = TransductionUtil.ToChar(m) + " : " + m;
                tb.Width = 210;
                CommandsTextGrid.Children.Add(tb);
            }
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                return this.imageSource;
            }
        }

        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;

                    // notify any bound elements that the text has changed
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }

        /// <summary>
        /// Execute start up tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.FrameArrived += this.Reader_FrameArrived;
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        private void ActivIn()
        {
            Regexp.Foreground = Brushes.Red;
            Regexp.Text = "Task Completed!! Enter another task!!";
            Regexp.IsReadOnly = false;
            Start.IsEnabled = true;
        }

        //public delegate void DrawTree(TreeBodyPart s);
        //private void DrawTreeIn(TreeBodyPart s)
        //{
        //    using (DrawingContext dc = this.drawingGroup.Append())
        //    {
        //        s.DrawBonesAndJoints(dc, 0.0, 0.0);
        //    }
        //}

        private Vector3D SubtractJoints(Joint j1, Joint j2)
        {
            return new Vector3D(j1.Position.X - j2.Position.X,
                                j1.Position.Y - j2.Position.Y,
                                j1.Position.Z - j2.Position.Z);
        }

        private Joint SumVectors(Vector3D v1, Vector3D v2)
        {
            Joint result = new Joint();
            var position = new CameraSpacePoint();

            position.X = (float)(v1.X + v2.X);
            position.Y = (float)(v1.Y + v2.Y);
            position.Z = (float)(v1.Z + v2.Z);
            result.Position = position;

            return result;
        }

        private Joint SumVectorJoint(Vector3D v2, Joint j1)
        {
            Joint result = new Joint();
            var position = new CameraSpacePoint();

            position.X = (float)(j1.Position.X + v2.X);
            position.Y = (float)(j1.Position.Y + v2.Y);
            position.Z = (float)(j1.Position.Z + v2.Z);
            result.Position = position;

            return result;
        }

        private Joint SumJoints(Joint j1, Joint j2)
        {
            Joint result = new Joint();
            var position = new CameraSpacePoint();

            position.X = j1.Position.X + j2.Position.X;
            position.Y = j1.Position.Y + j2.Position.Y;
            position.Z = j1.Position.Z + j2.Position.Z;
            result.Position = position;

            return result;
        }

        //private Dictionary<JointType, Joint> MapVectorsToJoints(Dictionary<JointType, Vector3D> vectors, IReadOnlyDictionary<JointType, Joint> joints)
        //{
        //  Dictionary<JointType, Joint> result = new Dictionary<JointType, Joint>();

        //  Matrix3D rotationMatrix = new Matrix3D();
        //  {
        //    Vector3D j = new Vector3D(0.0, 1.0, 0.0);

        //    Vector3D i = SubtractJoints(joints[JointType.HipRight], joints[JointType.HipLeft]);
        //    i.Normalize();

        //    Vector3D k = Vector3D.CrossProduct(i, j);
        //    k.Normalize();

        //    rotationMatrix.M11 = i.X;
        //    rotationMatrix.M12 = i.Y;
        //    rotationMatrix.M13 = i.Z;
        //    rotationMatrix.M21 = j.X;
        //    rotationMatrix.M22 = j.Y;
        //    rotationMatrix.M23 = j.Z;
        //    rotationMatrix.M31 = k.X;
        //    rotationMatrix.M32 = k.Y;
        //    rotationMatrix.M33 = k.Z;
        //  }

        //  for (int i = 0; i < vectors.Count; i++)
        //  {
        //    vectors[(JointType)i] = vectors[(JointType)i] * rotationMatrix;
        //    //Vector3D normalized = vectors[i];
        //    //normalized.Normalize();
        //    //vectors[i] = normalized;
        //  }

        //  result.Add(JointType.SpineMid,      SumVectorJoint(vectors[JointType.SpineMid     ],  joints[JointType.SpineBase]));
        //  result.Add(JointType.SpineShoulder, SumVectorJoint(vectors[JointType.SpineShoulder],  result[JointType.SpineMid]));
        //  result.Add(JointType.ShoulderLeft,  SumVectorJoint(vectors[JointType.ShoulderLeft ],  result[JointType.SpineShoulder]));
        //  result.Add(JointType.ElbowLeft,     SumVectorJoint(vectors[JointType.ElbowLeft    ],  result[JointType.ShoulderLeft]));
        //  result.Add(JointType.WristLeft,     SumVectorJoint(vectors[JointType.WristLeft    ],  result[JointType.ElbowLeft]));
        //  result.Add(JointType.HandLeft,      SumVectorJoint(vectors[JointType.HandLeft     ],  result[JointType.WristLeft]));
        //  result.Add(JointType.HandTipLeft,   SumVectorJoint(vectors[JointType.HandTipLeft  ],  result[JointType.HandLeft]));
        //  result.Add(JointType.ThumbLeft,     SumVectorJoint(vectors[JointType.ThumbLeft    ],  result[JointType.HandLeft]));
        //  result.Add(JointType.Neck,          SumVectorJoint(vectors[JointType.Neck         ],  result[JointType.SpineShoulder]));
        //  result.Add(JointType.Head,          SumVectorJoint(vectors[JointType.Head         ],  result[JointType.Neck]));
        //  result.Add(JointType.ShoulderRight, SumVectorJoint(vectors[JointType.ShoulderRight], result[JointType.SpineShoulder]));
        //  result.Add(JointType.ElbowRight,    SumVectorJoint(vectors[JointType.ElbowRight   ], result[JointType.ShoulderRight]));
        //  result.Add(JointType.WristRight,    SumVectorJoint(vectors[JointType.WristRight   ], result[JointType.ElbowRight]));
        //  result.Add(JointType.HandRight,     SumVectorJoint(vectors[JointType.HandRight    ], result[JointType.WristRight]));
        //  result.Add(JointType.HandTipRight,  SumVectorJoint(vectors[JointType.HandTipRight ], result[JointType.HandRight]));
        //  result.Add(JointType.ThumbRight,    SumVectorJoint(vectors[JointType.ThumbRight   ], result[JointType.HandRight]));
        //  result.Add(JointType.SpineBase,     joints[JointType.SpineBase]);
        //  result.Add(JointType.HipLeft,       SumVectorJoint(vectors[JointType.HipLeft      ], result[JointType.SpineBase]));
        //  result.Add(JointType.KneeLeft,      SumVectorJoint(vectors[JointType.KneeLeft     ], result[JointType.HipLeft]));
        //  result.Add(JointType.AnkleLeft,     SumVectorJoint(vectors[JointType.AnkleLeft    ], result[JointType.KneeLeft]));
        //  result.Add(JointType.FootLeft,      SumVectorJoint(vectors[JointType.FootLeft     ], result[JointType.AnkleLeft]));
        //  result.Add(JointType.HipRight,      SumVectorJoint(vectors[JointType.HipRight     ], result[JointType.SpineBase]));
        //  result.Add(JointType.KneeRight,     SumVectorJoint(vectors[JointType.KneeRight    ], result[JointType.HipRight]));
        //  result.Add(JointType.AnkleRight,    SumVectorJoint(vectors[JointType.AnkleRight   ], result[JointType.KneeRight]));
        //  result.Add(JointType.FootRight,     SumVectorJoint(vectors[JointType.FootRight    ], result[JointType.AnkleRight]));

        //  return result;
        //}
        
        private JointType GetFather(JointType type)
        {
            JointType result = JointType.SpineMid;

            if (type == JointType.SpineMid) result = JointType.SpineMid;
            if (type == JointType.SpineShoulder) result = JointType.SpineMid;
            if (type == JointType.ShoulderLeft) result = JointType.SpineShoulder;
            if (type == JointType.ElbowLeft) result = JointType.ShoulderLeft;
            if (type == JointType.WristLeft) result = JointType.ElbowLeft;
            if (type == JointType.HandLeft) result = JointType.WristLeft;
            if (type == JointType.HandTipLeft) result = JointType.HandLeft;
            if (type == JointType.ThumbLeft) result = JointType.HandLeft;
            if (type == JointType.Neck) result = JointType.SpineShoulder;
            if (type == JointType.Head) result = JointType.Neck;
            if (type == JointType.ShoulderRight) result = JointType.SpineShoulder;
            if (type == JointType.ElbowRight) result = JointType.ShoulderRight;
            if (type == JointType.WristRight) result = JointType.ElbowRight;
            if (type == JointType.HandRight) result = JointType.WristRight;
            if (type == JointType.HandTipRight) result = JointType.HandRight;
            if (type == JointType.ThumbRight) result = JointType.HandRight;
            if (type == JointType.SpineBase) result = JointType.SpineMid;
            if (type == JointType.HipLeft) result = JointType.SpineBase;
            if (type == JointType.KneeLeft) result = JointType.HipLeft;
            if (type == JointType.AnkleLeft) result = JointType.KneeLeft;
            if (type == JointType.FootLeft) result = JointType.AnkleLeft;
            if (type == JointType.HipRight) result = JointType.SpineBase;
            if (type == JointType.KneeRight) result = JointType.HipRight;
            if (type == JointType.AnkleRight) result = JointType.KneeRight;
            if (type == JointType.FootRight) result = JointType.AnkleRight;

            return result;
        }

        private double Norm(Vector3D v)
        {
            return Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
        }

        private void SetNorm(ref Vector3D v, double norm)
        {
            v.Normalize();
            v.X *= norm;
            v.Y *= norm;
            v.Z *= norm;
        }

        private Dictionary<JointType, Joint> MapShadowVectorsToJoints(Dictionary<JointType, Vector3D> vectors,
          Dictionary<JointType, bool> actList,
          IReadOnlyDictionary<JointType, Joint> joints)
        {
            Dictionary<JointType, Joint> result = new Dictionary<JointType, Joint>();

            Matrix3D rotationMatrix = new Matrix3D();
            {
                Vector3D j = new Vector3D(0.0, 1.0, 0.0);

                Vector3D i = SubtractJoints(joints[JointType.HipRight], joints[JointType.HipLeft]);
                i.Normalize();

                Vector3D k = Vector3D.CrossProduct(i, j);
                k.Normalize();

                rotationMatrix.M11 = i.X;
                rotationMatrix.M12 = i.Y;
                rotationMatrix.M13 = i.Z;
                rotationMatrix.M21 = j.X;
                rotationMatrix.M22 = j.Y;
                rotationMatrix.M23 = j.Z;
                rotationMatrix.M31 = k.X;
                rotationMatrix.M32 = k.Y;
                rotationMatrix.M33 = k.Z;
            }

            for (int i = 0; i < vectors.Count; i++)
            {
                vectors[(JointType)i] = vectors[(JointType)i] * rotationMatrix;
                Vector3D reshaped = vectors[(JointType)i];
                SetNorm(ref reshaped, Norm(SubtractJoints(joints[(JointType)i], joints[GetFather((JointType)i)])));
                vectors[(JointType)i] = reshaped;
            }

            //here all vectors are added to result
            // if vectors parents were active the vectors are summed with the result itself instead of the body
            // this behavior creates a better feedback for users to follow just the part of the tree which is needed
            // to propagate this behavior the current joint is also set to active

            foreach (var vector in vectors)
            {
                if (actList[vector.Key])
                    if (actList[GetFather(vector.Key)])
                        result.Add(vector.Key, SumVectorJoint(vector.Value, result[vector.Key]));
                    else
                        result.Add(vector.Key, SumVectorJoint(vector.Value, joints[vector.Key]));
                else
                    result.Add(vector.Key, joints[vector.Key]);
            }

            return result;
        }

        private Dictionary<JointType, Vector3D> MapJointsToVectors(IReadOnlyDictionary<JointType, Joint> joints)
        {
            //Vectors are added in the same order as the bodytree depthfirst search
            Dictionary<JointType, Vector3D> jointVectors = new Dictionary<JointType, Vector3D>();
            jointVectors.Add(JointType.SpineMid, SubtractJoints(joints[JointType.SpineMid], joints[JointType.SpineBase]));
            jointVectors.Add(JointType.SpineShoulder, SubtractJoints(joints[JointType.SpineShoulder], joints[JointType.SpineMid]));
            jointVectors.Add(JointType.ShoulderLeft, SubtractJoints(joints[JointType.ShoulderLeft], joints[JointType.SpineShoulder]));
            jointVectors.Add(JointType.ElbowLeft, SubtractJoints(joints[JointType.ElbowLeft], joints[JointType.ShoulderLeft]));
            jointVectors.Add(JointType.WristLeft, SubtractJoints(joints[JointType.WristLeft], joints[JointType.ElbowLeft]));
            jointVectors.Add(JointType.HandLeft, SubtractJoints(joints[JointType.HandLeft], joints[JointType.WristLeft]));
            jointVectors.Add(JointType.HandTipLeft, SubtractJoints(joints[JointType.HandTipLeft], joints[JointType.HandLeft]));
            jointVectors.Add(JointType.ThumbLeft, SubtractJoints(joints[JointType.ThumbLeft], joints[JointType.HandLeft]));
            jointVectors.Add(JointType.Neck, SubtractJoints(joints[JointType.Neck], joints[JointType.SpineShoulder]));
            jointVectors.Add(JointType.Head, SubtractJoints(joints[JointType.Head], joints[JointType.Neck]));
            jointVectors.Add(JointType.ShoulderRight, SubtractJoints(joints[JointType.ShoulderRight], joints[JointType.SpineShoulder]));
            jointVectors.Add(JointType.ElbowRight, SubtractJoints(joints[JointType.ElbowRight], joints[JointType.ShoulderRight]));
            jointVectors.Add(JointType.WristRight, SubtractJoints(joints[JointType.WristRight], joints[JointType.ElbowRight]));
            jointVectors.Add(JointType.HandRight, SubtractJoints(joints[JointType.HandRight], joints[JointType.WristRight]));
            jointVectors.Add(JointType.HandTipRight, SubtractJoints(joints[JointType.HandTipRight], joints[JointType.HandRight]));
            jointVectors.Add(JointType.ThumbRight, SubtractJoints(joints[JointType.ThumbRight], joints[JointType.HandRight]));
            jointVectors.Add(JointType.SpineBase, SubtractJoints(joints[JointType.SpineBase], new Joint()));
            jointVectors.Add(JointType.HipLeft, SubtractJoints(joints[JointType.HipLeft], joints[JointType.SpineBase]));
            jointVectors.Add(JointType.KneeLeft, SubtractJoints(joints[JointType.KneeLeft], joints[JointType.HipLeft]));
            jointVectors.Add(JointType.AnkleLeft, SubtractJoints(joints[JointType.AnkleLeft], joints[JointType.KneeLeft]));
            jointVectors.Add(JointType.FootLeft, SubtractJoints(joints[JointType.FootLeft], joints[JointType.AnkleLeft]));
            jointVectors.Add(JointType.HipRight, SubtractJoints(joints[JointType.HipRight], joints[JointType.SpineBase]));
            jointVectors.Add(JointType.KneeRight, SubtractJoints(joints[JointType.KneeRight], joints[JointType.HipRight]));
            jointVectors.Add(JointType.AnkleRight, SubtractJoints(joints[JointType.AnkleRight], joints[JointType.KneeRight]));
            jointVectors.Add(JointType.FootRight, SubtractJoints(joints[JointType.FootRight], joints[JointType.AnkleRight]));

            Matrix3D rotationMatrix = new Matrix3D();

            {
                Vector3D j = new Vector3D(0.0, 1.0, 0.0);

                Vector3D i = SubtractJoints(joints[JointType.HipRight], joints[JointType.HipLeft]);
                i.Normalize();

                Vector3D k = Vector3D.CrossProduct(i, j);
                k.Normalize();

                rotationMatrix.M11 = i.X;
                rotationMatrix.M12 = i.Y;
                rotationMatrix.M13 = i.Z;
                rotationMatrix.M21 = j.X;
                rotationMatrix.M22 = j.Y;
                rotationMatrix.M23 = j.Z;
                rotationMatrix.M31 = k.X;
                rotationMatrix.M32 = k.Y;
                rotationMatrix.M33 = k.Z;
            }

            rotationMatrix.Invert();


            for (int i = 0; i < jointVectors.Count; i++)
            {
                jointVectors[(JointType)i] = jointVectors[(JointType)i] * rotationMatrix;
                Vector3D normalized = jointVectors[(JointType)i];
                normalized.Normalize();
                jointVectors[(JointType)i] = normalized;
            }

            return jointVectors;
        }

        /// <summary>
        /// Handles the body frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            BodyFrameReference frameReference = e.FrameReference;

            if (this.startTime.Ticks == 0)
            {
                this.startTime = frameReference.RelativeTime;
            }

            try
            {
                BodyFrame frame = frameReference.AcquireFrame();

                if (frame != null)
                {
                    // BodyFrame is IDisposable
                    using (frame)
                    {
                        this.framesSinceUpdate++;

                        // update status unless last message is sticky for a while
                        if (DateTime.Now >= this.nextStatusUpdate)
                        {
                            // calcuate fps based on last frame received
                            double fps = 0.0;

                            if (this.stopwatch.IsRunning)
                            {
                                this.stopwatch.Stop();
                                fps = this.framesSinceUpdate / this.stopwatch.Elapsed.TotalSeconds;
                                this.stopwatch.Reset();
                            }

                            this.nextStatusUpdate = DateTime.Now + TimeSpan.FromSeconds(1);
                            this.StatusText = string.Format(Properties.Resources.StandardStatusTextFormat, fps, frameReference.RelativeTime - this.startTime);
                        }

                        if (!this.stopwatch.IsRunning)
                        {
                            this.framesSinceUpdate = 0;
                            this.stopwatch.Start();
                        }

                        using (DrawingContext dc = this.drawingGroup.Open())
                        {
                            // Draw a transparent background to set the render size
                            dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));

                            // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                            // As long as those body objects are not disposed and not set to null in the array,
                            // those body objects will be re-used.
                            frame.GetAndRefreshBodyData(this.bodies);

                            foreach (Body body in this.bodies)
                            {
                                if (body.IsTracked)
                                {
                                    this.DrawClippedEdges(body, dc);

                                    IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                                    // convert the joint points to depth (display) space
                                    Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();
                                    foreach (JointType jointType in joints.Keys)
                                    {
                                        DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(joints[jointType].Position);
                                        jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                                    }

                                    this.DrawBody(joints, jointPoints, dc);

                                    this.DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
                                    this.DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);

                                    Dictionary<JointType, Vector3D> bodyVectors = MapJointsToVectors(joints);

                                    bodyPartTree = new TreeBodyPart(bodyVectors, Side.C, BodyPart.SpineMid, Brushes.Firebrick);

                                    ////Language Test
                                    //IEnumerable<TreeBodyPart> res;
                                    //res = bodyPartTree.spineshoulder();
                                    //foreach (var b in res)
                                    //{
                                    //	bodyPartTree = b;
                                    //}
                                    ////res = bodyPartTree.elbow();
                                    ////foreach (var b in res)
                                    ////{
                                    ////	bodyPartTree = b;
                                    ////}
                                    //bodyPartTree.ReComputeLabel();
                                    //this.FeedbackText.Visibility = System.Windows.Visibility.Visible;
                                    //if (bodyPartTree.ispointingup())
                                    //	this.FeedbackText.Text = "straight";
                                    //else
                                    //	this.FeedbackText.Text = "take care";
                                    //res = bodyPartTree.resetjt();
                                    //foreach (var b in res)
                                    //{
                                    //	bodyPartTree = b;
                                    //}

                                    if (started && first)
                                    {
                                        first = false;
                                        updated = true;
                                    }

                                    if (updated)
                                    {
                                        //If a skeleton has matched we compute the new set of available moves
                                        if (matched)
                                        {
                                            moves = moveSeq.AvailableMoves();
                                            myPen = new List<Pen>();
                                            shadows = new List<TreeBodyPart>();

                                            shadow = new TreeBodyPart(bodyVectors, Side.C, BodyPart.SpineMid, Brushes.AliceBlue);
                                            //foreach (Transduction m in moves)
                                            //{
                                            shadow = shadow.MakeMove(moves[0]);
                                            while (!shadow.IsActivated() && !moveSeq.isFinal())
                                            {
                                                moveSeq.MakeOneStep(moves[0]);
                                                moves = moveSeq.AvailableMoves();
                                                shadow = shadow.MakeMove(moves[0]);
                                            }

                                            shadows.Add(shadow);
                                            shadow = new TreeBodyPart(bodyVectors, Side.C, BodyPart.SpineMid, Brushes.AliceBlue);
                                            //}
                                            matched = false;
                                        }

                                        for (int i = 0; i < shadows.Count; i++)
                                        {
                                            if (bodyPartTree.SimilarTo(shadows[i]))
                                            {
                                                moveSeq.MakeOneStep(moves[i]);
                                                if (moveSeq.isFinal())
                                                {
                                                    updated = false;
                                                    started = false;
                                                    ActivIn();
                                                }
                                                else
                                                {
                                                    matched = true;
                                                }
                                                break;
                                            }
                                        }
                                    }

                                    if (started && updated)
                                    {
                                        foreach (var s in shadows)
                                        {
                                            Dictionary<JointType, Vector3D> vectors = new Dictionary<JointType, Vector3D>();
                                            Dictionary<JointType, bool> actList = new Dictionary<JointType, bool>();
                                            s.GetVectors(ref vectors, ref actList);
                                            //Dictionary<JointType, Joint> shadowJoints = MapVectorsToJoints(vectors, joints);
                                            Dictionary<JointType, Joint> shadowJoints = MapShadowVectorsToJoints(vectors, actList, joints);

                                            Dictionary<JointType, Point> shadowJointPoints = new Dictionary<JointType, Point>();
                                            foreach (JointType shadowJointType in shadowJoints.Keys)
                                            {
                                                DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(shadowJoints[shadowJointType].Position);
                                                shadowJointPoints[shadowJointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                                            }

                                            this.DrawShadow(shadowJointPoints, jointPoints, actList, dc);
                                        }
                                    }

                                    // prevent drawing outside of our render area
                                    //this.Dispatcher.Invoke(new Clipp(this.ClippIn), new object[] { });
                                }
                            }

                            // prevent drawing outside of our render area
                            this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignore if the frame is no longer available
            }
        }

        /// <summary>
        /// Draws a body
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext)
        {
            // Draw the bones

            // Torso
            this.DrawBone(joints, jointPoints, JointType.Head, JointType.Neck, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.Neck, JointType.SpineShoulder, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.SpineShoulder, JointType.SpineMid, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.SpineMid, JointType.SpineBase, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.SpineShoulder, JointType.ShoulderRight, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.SpineShoulder, JointType.ShoulderLeft, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.SpineBase, JointType.HipRight, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.SpineBase, JointType.HipLeft, drawingContext);

            // Right Arm    
            this.DrawBone(joints, jointPoints, JointType.ShoulderRight, JointType.ElbowRight, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.ElbowRight, JointType.WristRight, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.WristRight, JointType.HandRight, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.HandRight, JointType.HandTipRight, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.WristRight, JointType.ThumbRight, drawingContext);

            // Left Arm
            this.DrawBone(joints, jointPoints, JointType.ShoulderLeft, JointType.ElbowLeft, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.ElbowLeft, JointType.WristLeft, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.WristLeft, JointType.HandLeft, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.HandLeft, JointType.HandTipLeft, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.WristLeft, JointType.ThumbLeft, drawingContext);

            // Right Leg
            this.DrawBone(joints, jointPoints, JointType.HipRight, JointType.KneeRight, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.KneeRight, JointType.AnkleRight, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.AnkleRight, JointType.FootRight, drawingContext);

            // Left Leg
            this.DrawBone(joints, jointPoints, JointType.HipLeft, JointType.KneeLeft, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.KneeLeft, JointType.AnkleLeft, drawingContext);
            this.DrawBone(joints, jointPoints, JointType.AnkleLeft, JointType.FootLeft, drawingContext);

            // Draw the joints
            foreach (JointType jointType in joints.Keys)
            {
                Brush drawBrush = null;

                TrackingState trackingState = joints[jointType].TrackingState;

                if (trackingState == TrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
                }
            }
        }

        private void DrawShadow(IDictionary<JointType, Point> shadowPoints,
                                IDictionary<JointType, Point> realPoints,
                                IDictionary<JointType, bool> actList,
                                DrawingContext drawingContext)
        {
            // Draw the bones
            foreach (var point in shadowPoints)
            {
                this.DrawShadowBone(shadowPoints, realPoints, actList, GetFather(point.Key), point.Key, drawingContext);
            }

            // Draw the joints
            foreach (JointType jointType in shadowPoints.Keys)
            {
                Brush drawBrush = null;

                if (actList[jointType])
                {
                    drawBrush = this.trackedJointBrush;
                }
                else
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, shadowPoints[jointType], 2, 2);
                }
            }
        }

        /// <summary>
        /// Draws one bone of a body (joint to joint)
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="jointType0">first joint of bone to draw</param>
        /// <param name="jointType1">second joint of bone to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == TrackingState.Inferred &&
                joint1.TrackingState == TrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
            {
                drawPen = this.trackedBonePen;
            }

            drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
        }

        private void DrawShadowBone(IDictionary<JointType, Point> shadowPoints,
                                    IDictionary<JointType, Point> realPoints,
                                    IDictionary<JointType, bool> actList,
                                    JointType jointType0,
                                    JointType jointType1,
                                    DrawingContext drawingContext)
        {
            Pen activePen = new Pen(Brushes.White, 5);
            Pen inactivePen = new Pen(Brushes.DarkGray, 1);

            Pen drawPen = activePen;
            if (!actList[jointType1]) drawPen = inactivePen;

            //if(!actList[jointType0])
            //  drawingContext.DrawLine(drawPen, realPoints[jointType0], shadowPoints[jointType1]);
            //else
            if (actList[jointType1])
                drawingContext.DrawLine(drawPen, shadowPoints[jointType0], shadowPoints[jointType1]);
        }

        /// <summary>
        /// Draws a hand symbol if the hand is tracked: red circle = closed, green circle = opened; blue circle = lasso
        /// </summary>
        /// <param name="handState">state of the hand</param>
        /// <param name="handPosition">position of the hand</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawHand(HandState handState, Point handPosition, DrawingContext drawingContext)
        {
            switch (handState)
            {
                case HandState.Closed:
                    drawingContext.DrawEllipse(this.handClosedBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Open:
                    drawingContext.DrawEllipse(this.handOpenBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Lasso:
                    drawingContext.DrawEllipse(this.handLassoBrush, null, handPosition, HandSize, HandSize);
                    break;
            }
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping body data
        /// </summary>
        /// <param name="body">body to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawClippedEdges(Body body, DrawingContext drawingContext)
        {
            FrameEdges clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, this.displayHeight - ClipBoundsThickness, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, this.displayHeight));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(this.displayWidth - ClipBoundsThickness, 0, ClipBoundsThickness, this.displayHeight));
            }
        }

        public void RegexpClicked(object sender, EventArgs e)
        {
            if (!Regexp.IsReadOnly)
            {
                Regexp.Foreground = Brushes.Black;
                Regexp.Text = "";
            }
        }

        public void StartClicked(object sender, EventArgs e)
        {
            if (!IsGood(Regexp.Text))
            {
                Regexp.Foreground = Brushes.Red;
                Regexp.Text = "Illegal Expression";
                return;
            }


            if (!started)
            {
                Start.IsEnabled = false;
                first = true;
                started = true;
                matched = true;
                moveSeq = new MoveSequence(Regexp.Text);
                Regexp.Foreground = Brushes.Blue;
                Regexp.IsReadOnly = true;
                //Regexp.Di
            }
        }

        private bool IsGood(string regex)
        {
            CharSetSolver solver = new CharSetSolver(BitWidth.BV7);
            Automaton<BvSet> aut = solver.Convert("^(" + regex + ")$").Determinize(solver).Minimize(solver);
            Automaton<BvSet> aut2 = solver.Convert("^([a-y]*)$").Determinize(solver).Minimize(solver);
            return aut.Minus(aut2, solver).IsEmpty;
        }

        //private bool MatchLanguage(TreeBodyPart body)
        //{
        //    if (body.leftArmIsDirectedLeftUp())
        //        return true;
        //    else
        //        return false;
        //}

        private void TestLang_Click(object sender, RoutedEventArgs e)
        {
            //if (MatchLanguage(bodyPartTree))
            //{
            //    TestLang.Content = "matched!";
            //}
            //else
            //{
            //    TestLang.Content = "not matched!";
            //}
        }

        //void QuatToMat(Matrix33 m, Vector4 q) {

        //    float x2 = q.X + q.X;
        //    float y2 = q.Y + q.Y;
        //    float z2 = q.Z + q.Z;

        //    float xx2 = q.X * x2;
        //    float yy2 = q.Y * y2;
        //    float zz2 = q.Z * z2;

        //    m.i.X = 1.0f - yy2 - zz2;
        //    m.j.Y = 1.0f - xx2 - zz2;
        //    m.k.Z = 1.0f - xx2 - yy2;

        //    float yz2 = q.Y * z2;
        //    float wx2 = q.W * x2;

        //    m.k.Y = yz2 - wx2;
        //    m.j.Z = yz2 + wx2;

        //    float xy2 = q.X * y2;
        //    float wz2 = q.W * z2;

        //    m.j.X = xy2 - wz2;
        //    m.i.Y = xy2 + wz2;

        //    float xz2 = q.X * z2;
        //    float wy2 = q.W * y2;

        //    m.i.Z = xz2 - wy2; 
        //    m.k.X = xz2 + wy2;
        //}
    }
}
