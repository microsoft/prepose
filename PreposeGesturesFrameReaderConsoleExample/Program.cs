using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using PreposeGestures;

/* This is a small console program that shows the FrameReader interface to Prepose. */
/* The FrameReader interface is intended to be as close to the existing Visual Gesture Builder interface as possible. */ 
/* This example is adapted from Ben Lower's VGBConsoleSample program. */ 

namespace PreposeGesturesFrameReaderConsoleExample
{
    class Program
    {
        PreposeGesturesDatabase pgd;
        PreposeGesturesFrameSource pgfs;
        PreposeGesturesFrameReader pgr;
        Gesture gesture;
        KinectSensor sensor;
        BodyFrameReader bfr;

        static void Main(string[] args)
        {
            Program prog = new Program();
            prog.Initialize();
            Console.Read();
        }

        private void Initialize()
        {
            sensor = KinectSensor.GetDefault();
            bfr = sensor.BodyFrameSource.OpenReader();
            bfr.FrameArrived += bfr_FrameArrived;
            pgd = new PreposeGesturesDatabase("soccer.app");
            pgfs = new PreposeGesturesFrameSource(KinectSensor.GetDefault(), 0);

            foreach (var g in pgd.AvailableGestures)
            {
                if (g.Name.Equals("ola"))
                {
                    gesture = g;
                    pgfs.AddGesture(gesture);
                }
            }
            pgr = pgfs.OpenReader();
            pgfs.GetIsEnabled(gesture);
            pgr.FrameArrived += pgr_FrameArrived;
            sensor.Open();

        }

        void bfr_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            //Check to see if VGB has a valid tracking id, if not find a new body to track
            if (!pgfs.IsTrackingIdValid)
            {

                using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
                {
                    if (bodyFrame != null)
                    {
                        Body[] bodies = new Body[6];
                        bodyFrame.GetAndRefreshBodyData(bodies);
                        Body closestBody = null;
                        //iterate through the bodies and pick the one closest to the camera
                        foreach (Body b in bodies)
                        {
                            if (b.IsTracked)
                            {
                                if (closestBody == null)
                                {
                                    closestBody = b;
                                }
                                else
                                {
                                    Joint newHeadJoint = b.Joints[Microsoft.Kinect.JointType.Head];
                                    Joint oldHeadJoint = closestBody.Joints[Microsoft.Kinect.JointType.Head];
                                    if (newHeadJoint.TrackingState == TrackingState.Tracked && newHeadJoint.Position.Z < oldHeadJoint.Position.Z)
                                    {
                                        closestBody = b;
                                    }
                                }
                            }
                        }

                        //if we found a tracked body, update the trackingid for vgb
                        if (closestBody != null)
                        {
                            pgfs.TrackingId = closestBody.TrackingId;
                        }
                    }
                }
            }
        }


        void pgr_FrameArrived(object sender, PreposeGesturesFrameArrivedEventArgs e)
        {
            
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    //This check is almost certainly not needed for this sample, left in for debugging help
                    if (pgfs.IsTrackingIdValid)
                    {
                          DiscreteGestureResult result = (DiscreteGestureResult)frame.GetDiscreteGestureResult(gesture);

                        //If it is detected, and it this this gesture was not detected on the last frame, then call the gesture as hit
                        //If you didn't require "FirstFrameDetected" every frame for the gesture would count as a unique instance
                        //This case just uses "Detected" as a bool, we could tune our detection threshold by using result.Confidence and set our own min value
                        if (result.Detected == true && result.FirstFrameDetected)
                         {
                             Console.WriteLine("Gesture detected!");
                         }
                    }
                }
            }
             
        }

    }
}
