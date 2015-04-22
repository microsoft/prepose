using PreposeGestures;
using System.Collections.Generic;

namespace PreposeGestureRecognizer
{
    internal class BodyMatching
    {


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
    }
}
