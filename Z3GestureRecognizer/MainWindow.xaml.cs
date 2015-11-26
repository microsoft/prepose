//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace PreposeGestureRecognizer
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
    using System.Linq;
    using Microsoft.Kinect;
    using PreposeGestures;
    using PreposeGestures.Parser;
    using System.Windows.Media.Media3D;
    using PreposeGestureRecognizer;
    using System.Text;
    using System.Windows.Controls;
    using ICSharpCode.AvalonEdit.Highlighting;
    using System.Xml;
    using ICSharpCode.AvalonEdit.CodeCompletion;
    using System.Reflection;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Global Variables
        /// <summary>
        /// Radius of drawn hand circles
        /// </summary>
        private const double HandSize = 30;

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

        /// <summary>
        /// Helper window to autocompletion while coding
        /// </summary>
        private CompletionWindow completionWindow;
        #endregion

        #region MainWindow Management
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            // Load our custom highlighting definition
            IHighlightingDefinition customHighlighting;
            using (Stream s = typeof(MainWindow).Assembly.GetManifestResourceStream("PreposeGestureRecognizer.CustomHighlighting.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", new string[] { ".cool" }, customHighlighting);


            // create a stopwatch for FPS calculation
            this.stopwatch = new Stopwatch();

            // Intialize the GestureStatistics lists 
            GestureStatistics.synthTimes = new List<StatisticsEntrySynthesize>();
            GestureStatistics.matchTimes = new List<StatisticsEntryMatch>();

            // one sensor is currently supported
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
                this.displayHeight = frameDescription.Height;

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
        #endregion

        #region Main Loop Function
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

                            foreach (var body in this.bodies)
                            {
                                if (useSyntheticData || body.IsTracked)
                                {
                                    this.DrawClippedEdges(body, dc);

                                    IReadOnlyDictionary<Microsoft.Kinect.JointType, Joint> joints = body.Joints;
                                    if (useSyntheticData)
                                        joints = Z3KinectConverter.CreateSyntheticJoints();

                                    // convert the joint points to depth (display) space
                                    var jointPoints = new Dictionary<Microsoft.Kinect.JointType, Point>();
                                    foreach (Microsoft.Kinect.JointType jointType in joints.Keys)
                                    {
                                        DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(joints[jointType].Position);
                                        jointPoints.Add(jointType, new Point(depthSpacePoint.X, depthSpacePoint.Y));
                                    }

                                    this.DrawBody(joints, jointPoints, dc);

                                    // TODO: We are considering only one body
                                    if (playingGesture)
                                    {
                                        this.ManageGestureApp(joints, dc);
                                    }
                                }
                            }

                            // prevent drawing outside of our render area
                            this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred: '{0}'", ex);
                // ignore if the frame is no longer available
            }
        }
        #endregion

        #region Kinect Drawing Functions
        /// <summary>
        /// Draws a body
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBody(IReadOnlyDictionary<Microsoft.Kinect.JointType, Joint> joints, IDictionary<Microsoft.Kinect.JointType, Point> jointPoints, DrawingContext drawingContext)
        {
            // Draw the bones

            // Torso
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.Head, Microsoft.Kinect.JointType.Neck, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.Neck, Microsoft.Kinect.JointType.SpineShoulder, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.SpineShoulder, Microsoft.Kinect.JointType.SpineMid, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.SpineMid, Microsoft.Kinect.JointType.SpineBase, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.SpineShoulder, Microsoft.Kinect.JointType.ShoulderRight, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.SpineShoulder, Microsoft.Kinect.JointType.ShoulderLeft, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.SpineBase, Microsoft.Kinect.JointType.HipRight, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.SpineBase, Microsoft.Kinect.JointType.HipLeft, drawingContext);

            // Right Arm    
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.ShoulderRight, Microsoft.Kinect.JointType.ElbowRight, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.ElbowRight, Microsoft.Kinect.JointType.WristRight, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.WristRight, Microsoft.Kinect.JointType.HandRight, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.HandRight, Microsoft.Kinect.JointType.HandTipRight, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.WristRight, Microsoft.Kinect.JointType.ThumbRight, drawingContext);

            // Left Arm
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.ShoulderLeft, Microsoft.Kinect.JointType.ElbowLeft, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.ElbowLeft, Microsoft.Kinect.JointType.WristLeft, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.WristLeft, Microsoft.Kinect.JointType.HandLeft, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.HandLeft, Microsoft.Kinect.JointType.HandTipLeft, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.WristLeft, Microsoft.Kinect.JointType.ThumbLeft, drawingContext);

            // Right Leg
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.HipRight, Microsoft.Kinect.JointType.KneeRight, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.KneeRight, Microsoft.Kinect.JointType.AnkleRight, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.AnkleRight, Microsoft.Kinect.JointType.FootRight, drawingContext);

            // Left Leg
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.HipLeft, Microsoft.Kinect.JointType.KneeLeft, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.KneeLeft, Microsoft.Kinect.JointType.AnkleLeft, drawingContext);
            this.DrawBone(joints, jointPoints, Microsoft.Kinect.JointType.AnkleLeft, Microsoft.Kinect.JointType.FootLeft, drawingContext);

            // Draw the joints
            foreach (var jointType in joints.Keys)
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

        /// <summary>
        /// Draws one bone of a body (joint to joint)
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="jointType0">first joint of bone to draw</param>
        /// <param name="jointType1">second joint of bone to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBone(
            IReadOnlyDictionary<Microsoft.Kinect.JointType, Joint> joints,
            IDictionary<Microsoft.Kinect.JointType, Point> jointPoints,
            Microsoft.Kinect.JointType jointType0,
            Microsoft.Kinect.JointType jointType1, DrawingContext
            drawingContext)
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

        #endregion

        #region Z3 Gestures Management
        // Gestures logic
        private static bool playingGesture = false;
        private static string parsedText = "";
        //private static KinectApp gestureApp;
        private PreposeGestures.BodyMatcher matcher;
        private static List<ProgressBar> mainProgressBars = null;
        private static List<TextBlock> posesTextBlocks = null;

        private void ManageGestureApp(
            IReadOnlyDictionary<Microsoft.Kinect.JointType, Microsoft.Kinect.Joint> kinectJoints,
            DrawingContext dc)
        {
            //var time = this.stopwatch.ElapsedMilliseconds;
            //Console.WriteLine("Z3 milliseconds " + time);
            //this.stopwatch.Reset();
            //this.stopwatch.Start();

            var status =
                BodyMatching.TestBody(this.matcher, kinectJoints, jumpToNextPose);

            jumpToNextPose = false;

            RenderFeedback(status);
            this.DrawTarget(kinectJoints, dc);
        }

        private void PrecisionSlider_Loaded(object sender, RoutedEventArgs e)
        {
            this.PrecisionSlider.Value = 30;
        }

        private void PrecisionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.matcher != null)
            {
                this.matcher.Precision = (int)this.PrecisionSlider.Value;
            }

            if (this.PrecisionTextBlock != null)
                this.PrecisionTextBlock.Text = ((int)(this.PrecisionSlider.Value)).ToString() + "°";
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (jumpToNextPose == false)
                jumpToNextPose = true;
        }

        public bool jumpToNextPose { get; set; }

        public bool useSyntheticData { get; set; }

        private void SyntheticCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.useSyntheticData = true;
        }

        private void SyntheticCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.useSyntheticData = false;
        }

        #endregion

        #region Rendering Addtional UI Feedback
        private void StartMainProgressBars(List<ExecutionStep> steps)
        {
            if (mainProgressBars == null)
                mainProgressBars = new List<ProgressBar>();

            if (posesTextBlocks == null)
                posesTextBlocks = new List<TextBlock>();

            var barWidth = ((this.Width - 110) / steps.Count) - 5;
            var xPos = 10;
            foreach (var step in steps)
            {
                var bar = new ProgressBar();
                bar.Minimum = 0;
                bar.Maximum = 100;
                bar.Margin = new Thickness(xPos, 0, 0, -60);
                bar.Height = 30;
                bar.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                bar.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

                bar.Width = barWidth;
                Grid.SetRow(bar, 2);
                Grid.SetColumnSpan(bar, 2);
                this.MainGrid.Children.Add(bar);
                mainProgressBars.Add(bar);

                var textBlock = new TextBlock();
                textBlock.Text = step.Pose.Name;
                textBlock.Margin = new Thickness(xPos - 1, 0, 0, -28);
                textBlock.TextAlignment = TextAlignment.Left;
                textBlock.FontSize = 19;
                textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                textBlock.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                Grid.SetRow(textBlock, 2);
                Grid.SetColumnSpan(textBlock, 2);
                this.MainGrid.Children.Add(textBlock);
                posesTextBlocks.Add(textBlock);

                xPos += (int)barWidth + 5;
            }
        }

        private void StopMainProgressBars()
        {
            foreach (var bar in mainProgressBars)
            {
                this.MainGrid.Children.Remove(bar);
            }

            foreach (var textBlock in posesTextBlocks)
            {
                this.MainGrid.Children.Remove(textBlock);
            }

            mainProgressBars.Clear();
            posesTextBlocks.Clear();
        }

        private void RenderFeedback(List<GestureStatus> feedback)
        {
            //var gesturesIDs = gestureApp.GetMostAdvancedGesturesIDs();

            //var bars = new List<ProgressBar>();
            //var gesture = feedback[gesturesIDs[0]];
            var gesture = feedback[feedback.Count - 1];

            //this.MainGestureAndPoseNameTextBlock.Text = gesture.GestureName;// +": " + gesture.StepNames[gesture.CurrentStep];
            var mainPosePercentage =
                (2.0 - gesture.Distance) / 2;
            var mainGesturePercentage =
                (double)gesture.CurrentStep /
                gesture.NumSteps +
                mainPosePercentage /
                gesture.NumSteps;

            for (int i = 0; i < gesture.NumSteps; ++i)
            {
                if (i < gesture.CurrentStep)
                    mainProgressBars[i].Value = 100;

                else if (i == gesture.CurrentStep)
                    mainProgressBars[i].Value = 100 * mainPosePercentage;

                else
                    mainProgressBars[i].Value = 0;
            }

            CompletedTimesTextBlock.Text = feedback[feedback.Count - 1].CompletedCount.ToString();
            if (feedback[feedback.Count - 1].CompletedCount == 1)
                TimesTextBlock.Text = "time";
            else
                TimesTextBlock.Text = "times";
        }
        #endregion

        #region Rendering Target
        private void DrawTarget(
            IReadOnlyDictionary<Microsoft.Kinect.JointType, Microsoft.Kinect.Joint> baseJoints,
            DrawingContext dc)
        {
            //var target = gestureApp.Gestures[gestureApp.Gestures.Count - 1].GetTarget();
            //var status = gestureApp.Gestures[gestureApp.Gestures.Count - 1].GetStatus();
            var target = matcher.GetLastGestureTarget();
            var status = matcher.GetLastGestureStatus();

            var shadowJoints = Z3KinectConverter.CreateKinectJoints(baseJoints, target);

            // convert the joint points to depth (display) space
            var shadowPoints = new Dictionary<Microsoft.Kinect.JointType, Point>();
            foreach (Microsoft.Kinect.JointType jointType in shadowJoints.Keys)
            {
                DepthSpacePoint depthSpacePoint =
                    this.coordinateMapper.MapCameraPointToDepthSpace(shadowJoints[jointType].Position);

                shadowPoints.Add(jointType, new Point(depthSpacePoint.X, depthSpacePoint.Y));
            }

            var basePoints = new Dictionary<Microsoft.Kinect.JointType, Point>();
            foreach (Microsoft.Kinect.JointType jointType in baseJoints.Keys)
            {
                DepthSpacePoint depthSpacePoint =
                    this.coordinateMapper.MapCameraPointToDepthSpace(baseJoints[jointType].Position);

                basePoints.Add(jointType, new Point(depthSpacePoint.X, depthSpacePoint.Y));
            }

            var brush = new SolidColorBrush(Colors.White);
            this.DrawShadow(shadowPoints, basePoints, target.TransformedJoints, status.DistanceVectors, brush, dc);
        }

        private void DrawShadow(IDictionary<Microsoft.Kinect.JointType, Point> shadowPoints,
                                IDictionary<Microsoft.Kinect.JointType, Point> realPoints,
                                List<PreposeGestures.JointType> calculatedJoints,
                                Dictionary<PreposeGestures.JointType, PreposeGestures.Point3D> distanceVectors,
                                Brush brush,
                                DrawingContext drawingContext)
        {
            // Draw the bones
            foreach (var point in shadowPoints)
            {
                this.DrawShadowBone(
                    shadowPoints,
                    realPoints,
                    (Microsoft.Kinect.JointType)JointTypeHelper.GetFather((PreposeGestures.JointType)point.Key),
                    point.Key,
                    calculatedJoints,
                    distanceVectors,
                    brush,
                    drawingContext);
            }

            // Draw the joints
            foreach (Microsoft.Kinect.JointType jointType in shadowPoints.Keys)
            {
                Brush drawBrush = this.trackedJointBrush;
                drawingContext.DrawEllipse(drawBrush, null, shadowPoints[jointType], 2, 2);
            }
        }

        private void DrawShadowBone(IDictionary<Microsoft.Kinect.JointType, Point> shadowPoints,
                                    IDictionary<Microsoft.Kinect.JointType, Point> realPoints,
                                    Microsoft.Kinect.JointType jointType0,
                                    Microsoft.Kinect.JointType jointType1,
                                    List<PreposeGestures.JointType> calculatedJoints,
                                    Dictionary<PreposeGestures.JointType, PreposeGestures.Point3D> distanceVectors,
                                    Brush brush,
                                    DrawingContext drawingContext)
        {

            double width = 3;
            //double alpha = 1;

            if (calculatedJoints.Contains((PreposeGestures.JointType)jointType1))
            {
                double depth = Math.Abs(distanceVectors[(PreposeGestures.JointType)jointType1].Z);
                width = Math.Min(2 + depth * 20, 20);
                //alpha = Math.Max(0, Math.Min(0.9, 1 - (distanceVectors[(Z3Gestures.JointType)jointType1].Norm() * 0.5)));
            }

            Pen currentPen = new Pen(Brushes.Green, width);
            currentPen.StartLineCap = PenLineCap.Triangle;
            currentPen.EndLineCap = PenLineCap.Triangle;

            drawingContext.DrawLine(currentPen, realPoints[jointType0], realPoints[jointType1]);

            brush.Opacity = 0.95;
            Pen shadowPen = new Pen(brush, 3);
            shadowPen.StartLineCap = PenLineCap.Triangle;
            shadowPen.EndLineCap = PenLineCap.Triangle;

            if (calculatedJoints.Contains((PreposeGestures.JointType)jointType0) &&
                !calculatedJoints.Contains((PreposeGestures.JointType)jointType1))
                drawingContext.DrawLine(shadowPen, realPoints[jointType0], realPoints[jointType1]);
            else
                drawingContext.DrawLine(shadowPen, shadowPoints[jointType0], shadowPoints[jointType1]);
        }

        #endregion

        #region Script Compiling
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!playingGesture)
            {
                parsedText = this.ScriptTextBox.Text;

                try
                {
                    var app = PreposeGestures.App.ReadAppText(parsedText);
                    matcher = new BodyMatcher(app, (int)PrecisionSlider.Value);
                    Debug.WriteLine(app);

                    // get the last gesture
                    this.MainGestureAndPoseNameTextBlock.Text = app.Gestures[app.Gestures.Count - 1].Name;
                    this.StartMainProgressBars(app.Gestures[app.Gestures.Count - 1].Steps);

                    this.CompletedGrid.Visibility = System.Windows.Visibility.Visible;
                    this.MainGestureAndPoseNameTextBlock.Visibility = System.Windows.Visibility.Visible;

                    this.ScriptTextBox.IsReadOnly = true;
                    this.ScriptTextBox.Background = Brushes.WhiteSmoke;
                    this.ScriptTextBox.Foreground = Brushes.DarkGray;
                    this.StartButton.Content = "ll";
                    this.compileStatus.Text = "OK";
                    playingGesture = true;
                }
                catch
                {
                    this.compileStatus.Text = "Script compilation error";
                }
            }
            else
            {
                this.StopMainProgressBars();

                this.CompletedGrid.Visibility = System.Windows.Visibility.Hidden;
                this.MainGestureAndPoseNameTextBlock.Text = "";
                this.MainGestureAndPoseNameTextBlock.Visibility = System.Windows.Visibility.Hidden;
                this.ScriptTextBox.Visibility = System.Windows.Visibility.Visible;
                this.ScriptTextBox.IsReadOnly = false;
                this.ScriptTextBox.Background = Brushes.White;
                this.ScriptTextBox.Foreground = Brushes.Black;
                this.ScriptTextBox.Text = parsedText;
                this.StartButton.Content = " ► ";
                this.compileStatus.Text = "";
                playingGesture = false;
            }
        }

        private void ScriptTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            this.ScriptTextBox.TextArea.TextEntering += ScriptTextBox_TextEntering;
            this.ScriptTextBox.TextArea.TextEntered += ScriptTextBox_TextEntered;

            int index = AppDomain.CurrentDomain.BaseDirectory.IndexOf("prepose") + "prepose".Length;
            string testPath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Substring(0, index), "Z3Experiments\\Z3Experiments"), "Tests");
            testPath = Path.Combine(testPath, "soccer.app");
            string text = System.IO.File.ReadAllText(testPath);
            this.ScriptTextBox.Text = text;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.InitialDirectory = (System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".app";
            dlg.Filter = "Gesture APP Files (*.app)|*.app|Text Files (*.txt)|*.txt";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                string content = File.ReadAllText(filename);
                this.ScriptTextBox.Text = content;
                //textBox1.Text = filename;
            }
        }
        #endregion

        #region Script Code Completion
        private void ScriptTextBox_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var line = GetLine(this.ScriptTextBox.Text, this.ScriptTextBox.TextArea.Caret.Line);
            var lastFewWords = new string[4];
            lastFewWords[0] = ""; lastFewWords[1] = ""; lastFewWords[2] = ""; lastFewWords[3] = "";

            var end = this.ScriptTextBox.TextArea.Caret.Column - 1;

            lastFewWords[0] = GetWordBehindCaret(line, end);

            if (lastFewWords[0].Length >= 2) lastFewWords[1] = GetWordBehindCaret(line, end - lastFewWords[0].Length - 1);
            if (lastFewWords[1].Length >= 2) lastFewWords[2] = GetWordBehindCaret(line, end - lastFewWords[0].Length - lastFewWords[1].Length - 2);
            if (lastFewWords[2].Length >= 2) lastFewWords[3] = GetWordBehindCaret(line, end - lastFewWords[0].Length - lastFewWords[1].Length - lastFewWords[2].Length - 3);

            IList<ICompletionData> data = null;
            var comparer = CultureInfo.InvariantCulture.CompareInfo;

            // Add completions using the first of the last words
            if (lastFewWords[0].Length >= 1)
            {
                var word = lastFewWords[0];
                foreach (var option in CompletionHelper.Actions) data = CheckAndAddCompletion(comparer, word, data, option);
                foreach (var option in CompletionHelper.TypeDeclarations) data = CheckAndAddCompletion(comparer, word, data, option);
                foreach (var option in CompletionHelper.Directions) data = CheckAndAddCompletion(comparer, word, data, option);
                foreach (var option in CompletionHelper.RelativeDirections) data = CheckAndAddCompletion(comparer, word, data, option);
                foreach (var option in CompletionHelper.BodyParts) data = CheckAndAddCompletion(comparer, word, data, option);
            }

            // Add completions using the second of the last words
            if (lastFewWords[1].Length >= 1)
            {
                var word = lastFewWords[1] + " " + lastFewWords[0];
                foreach (var option in CompletionHelper.Directions) data = CheckAndAddCompletion(comparer, word, data, option);
                foreach (var option in CompletionHelper.RelativeDirections) data = CheckAndAddCompletion(comparer, word, data, option);
                foreach (var option in CompletionHelper.BodyParts) data = CheckAndAddCompletion(comparer, word, data, option);
            }

            // Add completions using the third of the last words
            if (lastFewWords[2].Length >= 1)
            {
                var word = lastFewWords[2] + " " + lastFewWords[1] + " " + lastFewWords[0];
                foreach (var option in CompletionHelper.Directions) data = CheckAndAddCompletion(comparer, word, data, option);
                foreach (var option in CompletionHelper.RelativeDirections) data = CheckAndAddCompletion(comparer, word, data, option);
                foreach (var option in CompletionHelper.BodyParts) data = CheckAndAddCompletion(comparer, word, data, option);
            }

            // Add completions using the fourth of the last words
            if (lastFewWords[3].Length >= 1)
            {
                var word = lastFewWords[3] + " " + lastFewWords[2] + " " + lastFewWords[1] + " " + lastFewWords[0];
                foreach (var option in CompletionHelper.RelativeDirections) data = CheckAndAddCompletion(comparer, word, data, option);
                foreach (var option in CompletionHelper.BodyParts) data = CheckAndAddCompletion(comparer, word, data, option);
            }

            // Show completion window if data is not null
            if (data != null)
            {
                // If we only have one completion, select it
                if (data.Count == 1)
                    completionWindow.CompletionList.SelectItem(data[0].Text);

                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
            }
            else
            {
                if (completionWindow != null)
                    completionWindow.Close();
            }
        }

        private IList<ICompletionData> CheckAndAddCompletion(CompareInfo comparer, string word, IList<ICompletionData> data, string option)
        {
            if (comparer.IsPrefix(option, word))
            {
                if (data == null)
                {
                    completionWindow = new CompletionWindow(this.ScriptTextBox.TextArea);
                    completionWindow.CompletionList.InsertionRequested += CompletionList_InsertionRequested;
                    data = completionWindow.CompletionList.CompletionData;
                }

                data.Add(new PreposeSyntaxCompletionData(option));
            }

            return data;
        }

        private string GetWordBehindCaret(string line, int end)
        {
            var start = end - 1;
            var comparer = CultureInfo.InvariantCulture.CompareInfo;
            var word = "";
            while (start >= 0)
            {
                // Check for a white space or a tab
                var check = line.Substring(start, 1);
                if (check.Equals(" ") || check.Equals("\t"))
                    break;

                word = line.Substring(start, end - start);
                start--;
            }
            return word;
        }

        private void CompletionList_InsertionRequested(object sender, EventArgs e)
        {
            var selected = ((ICSharpCode.AvalonEdit.CodeCompletion.CompletionList)sender).SelectedItem;
            if (selected != null)
            {
                var inserted = selected.Content as string;
                var row = this.ScriptTextBox.TextArea.Caret.Line;
                var offset = this.ScriptTextBox.TextArea.Caret.Offset;

                var line = GetLine(this.ScriptTextBox.Text, row);

                // Search for prefix
                bool found = false;
                var end = this.ScriptTextBox.TextArea.Caret.Column - inserted.Length - 1;
                var start = end - 1;
                var comparer = CultureInfo.InvariantCulture.CompareInfo;
                while (!found && start >= 0)
                {
                    var substring = line.Substring(start, end - start);
                    found = comparer.IsPrefix(inserted, substring);
                    start--;
                }

                // If we found the spare prefix, cut it from the current line
                if (found)
                {
                    var lengthPart1 = offset - (inserted.Length + (end - start)) + 1;
                    var startPart2 = offset - (inserted.Length);
                    var lengthPart2 = ScriptTextBox.Text.Length - startPart2;

                    var part1 = this.ScriptTextBox.Text.Substring(0, lengthPart1);
                    var part2 = this.ScriptTextBox.Text.Substring(startPart2, lengthPart2);
                    this.ScriptTextBox.Text = part1 + part2;

                    this.ScriptTextBox.CaretOffset = offset - (end - start) + 1;
                }
            }
        }

        string GetLine(string text, int lineNo)
        {
            string[] lines = text.Replace("\r", "").Split('\n');
            return lines.Length >= lineNo ? lines[lineNo - 1] : null;
        }

        private void ScriptTextBox_TextEntering(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GestureStatistics.DumpStatisticsToFile("matchstats.csv");
        }
    }

    public static class CompletionHelper
    {
        public static readonly string[] TypeDeclarations = { "APP",
                                                             "GESTURE",
                                                             "POSE",
                                                             "EXECUTION:" };

        public static readonly string[] Actions = { "point",
                                                    "rotate",
                                                    "put",
                                                    "touch",
                                                    "align" };

        public static readonly string[] Directions = {  "up",
                                                        "down",
                                                        "to your front",
                                                        "to your back",
                                                        "to your left",
                                                        "to your righ",
                                                        "in front of" };

        public static readonly string[] RelativeDirections = { "behind",
                                                               "above",
                                                               "on top of",
                                                               "below",
                                                               "to the left of",
                                                               "to the right of"};


        public static readonly string[] BodyParts = { "your neck",
                                                      "your head",
                                                      "your spine mid",
                                                      "your spine base",
                                                      "your spine shoulder",
                                                      "your left shoulder",
                                                      "your left elbow",
                                                      "your left wrist",
                                                      "your left hand",
                                                      "your left hand tip",
                                                      "your left thumb",
                                                      "your left hip",
                                                      "your left knee",
                                                      "your left ankle",
                                                      "your left foot",
                                                      "your right shoulder",
                                                      "your right elbow",
                                                      "your right wrist",
                                                      "your right hand",
                                                      "your right hand tip",
                                                      "your right thumb",
                                                      "your right hip",
                                                      "your right knee",
                                                      "your right ankle",
                                                      "your right foot",
                                                      "your left arm",
                                                      "your left leg",
                                                      "your right arm",
                                                      "your right leg",
                                                      "your spine",
                                                      "your back",
                                                      "your arms",
                                                      "your legs",
                                                      "your shoulders",
                                                      "your wrists",
                                                      "your elbows",
                                                      "your hands",
                                                      "your hands tips",
                                                      "your thumbs",
                                                      "your hips",
                                                      "your knees",
                                                      "your ankles",
                                                      "your feet",
                                                      "you"};
    }
}