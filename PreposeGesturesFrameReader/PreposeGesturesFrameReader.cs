using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
using Microsoft.Kinect;

/* This implements a 'PreposeGesturesFrameReader' interface to Prepose. The interface is modeled on the VisualGestureBuilderFrameReader */
/* found in Public Preview build 1407 of the Kinect for Windows SDK v2. */ 

namespace PreposeGestures
{
    public class PreposeGesturesDatabase
    {
        private App parsedPreposeCode;
        public PreposeGesturesDatabase(string preposeFile)
        {
            parsedPreposeCode = PreposeGestures.App.ReadApp(preposeFile);
            AvailableGestures = (IReadOnlyList<Gesture>)parsedPreposeCode.Gestures; 
        }

        public PreposeGesturesDatabase(Func<PreposeGestures.App> preposeGestureAppFunc)
        {
            parsedPreposeCode = preposeGestureAppFunc();
            AvailableGestures = (IReadOnlyList<Gesture>)parsedPreposeCode.Gestures;
        }

        public static PreposeGesturesDatabase FromText(string text)
        {
            return new PreposeGesturesDatabase(() => PreposeGestures.App.ReadAppText(text));
        }

        public IReadOnlyList<Gesture> AvailableGestures { get; private set; }
        public uint AvailableGesturesCount { get; private set; }
    }

    public sealed class DiscreteGestureResult : IDisposable
    {
        public DiscreteGestureResult(bool detected, bool firstFrameDetected, float confidence)
        {
            throw new NotImplementedException();
        }

        public float Confidence { get; set; }
        public bool Detected { get; set; }
        public bool FirstFrameDetected { get; set; }

        public void Dispose()
        {
            return;
        }

        [HandleProcessCorruptedStateExceptions]
        protected void Dispose(bool A_0)
        {
            return;
        }
    }


    public class PreposeGesturesFrame : IDisposable
    {
        public TimeSpan RelativeTime { get; set; }
        public ulong TrackingId { get; set;  }

        public List<GestureStatus> results; 
        public PreposeGesturesFrameSource PreposeGesturesFrameSource { get; set; }

        public DiscreteGestureResult GetDiscreteGestureResult(Gesture gesture)
        {
            bool detected = false;
            bool firstFrame = false;
            float confidence = 0.0f;

            foreach (GestureStatus gs in results)
            {
                if (gesture.Name.Equals(gs.GestureName))
                {
                    detected = gs.succeededDetection;
                    firstFrame = gs.succeededDetectionFirstFrame;
                    confidence = (float)gs.confidence;

                }
            }

            return new DiscreteGestureResult(detected, firstFrame, confidence);
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        [HandleProcessCorruptedStateExceptions]
        protected void Dispose(bool A_0)
        {
            throw new NotImplementedException();
        }
    }
    public class PreposeGesturesFrameSource : IDisposable, INotifyPropertyChanged
    {
        public BodyMatcher myMatcher;
        private PreposeGesturesFrameReader myReader; 
        public PreposeGesturesFrameSource(KinectSensor sensor, ulong initialTrackingId)
        {
            KinectSensor = sensor;
            TrackingId = initialTrackingId;
            Gestures = new List<Gesture>();
            myMatcher = new BodyMatcher(Gestures);
            myReader = new PreposeGesturesFrameReader(this);
        }


        public uint GestureCount;
        public List<Gesture> Gestures { get; set; }
        public bool HorizontalMirror { get; set; }
        public bool IsActive { get; set; }
         

        public bool IsTrackingIdValid { get; set; }
        public KinectSensor KinectSensor { get; set; }
        
        public ulong TrackingId { get; set; }
        
        public virtual event PropertyChangedEventHandler PropertyChanged;

        public void AddGesture(Gesture gesture)
        {
            Gestures.Add(gesture);
            myMatcher.AddGesture(gesture);
            GestureCount++; 
        }
        public void AddGestures(List<Gesture> gestures)
        {
            foreach (Gesture gesture in gestures)
            {
                Gestures.Add(gesture);
                myMatcher.AddGesture(gesture);
                GestureCount++;
            }
        }
        
        public  void Dispose()
        {
            return; 
        }


        [HandleProcessCorruptedStateExceptions]
        protected void Dispose(bool A_0)
        {
            return; 
        }
        
        public bool GetIsEnabled(Gesture gesture)
        {
            // Gestures are always enabled right now -- keeping this for compatibility with VisualGestureBuilder API
            return true; 
        }
        

        public PreposeGesturesFrameReader OpenReader()
        {
            return myReader;
        }
        
        public void RemoveGesture(Gesture gesture)
        {
            Gestures.Remove(gesture);
        }
        public void SetIsEnabled(Gesture gesture, bool isEnabled)
        {
            throw new NotImplementedException(); 
        }
        

    }

    public sealed class PreposeGesturesFrameArrivedEventArgs : EventArgs
    {
        public PreposeGesturesFrameReference FrameReference { get; set; }
    }

    public sealed class PreposeGesturesFrameReference 
    {
        private PreposeGesturesFrame myFrame; 

        public PreposeGesturesFrameReference(PreposeGesturesFrame frame)
        {
            myFrame = frame;
        }

        public TimeSpan RelativeTime { get; set; }
        public PreposeGesturesFrame AcquireFrame()
        {
            return myFrame; 
        }
    }

    public class PreposeGesturesFrameReader
    {
        private PreposeGesturesFrameSource mySource;
        private BodyFrameReader myBodyReader; 
        public PreposeGesturesFrameReader(PreposeGesturesFrameSource source)
        {
            mySource = source;
            myBodyReader = mySource.KinectSensor.BodyFrameSource.OpenReader();
            myBodyReader.FrameArrived += myBodyReader_FrameArrived;
        }

        /// <summary>
        /// Array for the bodies
        /// </summary>
        private Body[] bodies = new Body[6]; 


        void myBodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            if (this.IsPaused)
                return;

            PreposeGesturesFrame retFrame = new PreposeGesturesFrame();
            PreposeGesturesFrameArrivedEventArgs upArgs = new PreposeGesturesFrameArrivedEventArgs(); 

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    // Perform the gesture matching on this frame
                    var z3body = new Z3Body();
                    bodyFrame.GetAndRefreshBodyData(this.bodies);

                    foreach (var body in this.bodies)
                    {
                        if (body.TrackingId == this.mySource.TrackingId)
                        {
                            // We are at the correct body - go ahead and feed it to the BodyMatcher
                            IReadOnlyDictionary<Microsoft.Kinect.JointType, Joint> joints = body.Joints;
                            z3body = Z3KinectConverter.CreateZ3Body(joints);
                            var result = this.PreposeGesturesFrameSource.myMatcher.TestBody(z3body);

                            // Fill in the gesture results for this frame
                            retFrame.results = result;
                            break;
                        }
                    }
                }
            }
            
            // TODO: revisit the way the PreposeGesturesFrameReference is implemented to avoid keeping around massive amounts of frames
            PreposeGesturesFrameReference retFrameReference = new PreposeGesturesFrameReference(retFrame);
            upArgs.FrameReference = retFrameReference;

            // Signal that we have a new PreposeGesturesFrame arrived
            FrameArrived(this, upArgs);
        }


        /// <summary>
        /// Matches a Kinect body to a set of gestures within the app.
        /// </summary>
        /// <param name="kinectJoints">Body representation</param>
        /// <param name="jumpToNextPose">synthesizes a new body in correct position if true</param>
        /// <returns></returns>
        public static List<GestureStatus> TestBody(BodyMatcher matcher,
            IReadOnlyDictionary<Microsoft.Kinect.JointType,
            Microsoft.Kinect.Joint> kinectJoints,
            bool jumpToNextPose = false)
        {
            // convert Kinect.Body to Z3Body
            var body = new Z3Body();
            if (!jumpToNextPose)
            {
                body = Z3KinectConverter.CreateZ3Body(kinectJoints);
            }
            else
            {
                //body = GetCopiedBodyValues(this.Gestures[this.GetMostAdvancedGesturesIDs()[0]].GetTarget().Body);
                var firstGestureBody = matcher.GetLastGestureTarget().Body;
                body = new Z3Body(firstGestureBody);
            }

            return matcher.TestBody(body);
        }


        public bool IsPaused { get; set; }

        public PreposeGesturesFrameSource PreposeGesturesFrameSource
        {
            get
            {
                return this.mySource;
            }
        }


        public event EventHandler<PreposeGesturesFrameArrivedEventArgs> FrameArrived; 

        public PreposeGesturesFrame CalculateAndAcquireLastFrame()
        {
            throw new NotImplementedException();
        }
    }
}
