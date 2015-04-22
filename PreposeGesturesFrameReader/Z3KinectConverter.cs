using Microsoft.Kinect;
using Microsoft.Z3;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using PreposeGestures;

namespace PreposeGestures
{
    /// <summary>
    /// This converts Kinect bodies to Z3 bodies and back.
    /// </summary>
	class Z3KinectConverter
	{
        /// <summary>
        /// This function converts Kinect joints to a Z3 body.
        /// It also changes the basis of body coordinates to make it
        /// invariant to body position in relation to the sensor.
        /// 
        /// The origin of the new coordinate system is user hips.
        /// </summary>
        /// <param name="kinectJoints"></param>
        /// <returns></returns>
		public static Z3Body CreateZ3Body(
            IReadOnlyDictionary<Microsoft.Kinect.JointType, Microsoft.Kinect.Joint> 
            kinectJoints)
		{            
			var jointVectors = new Dictionary<Microsoft.Kinect.JointType, Vector3D>();
			jointVectors.Add(Microsoft.Kinect.JointType.SpineMid, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.SpineMid], kinectJoints[Microsoft.Kinect.JointType.SpineBase]));
			jointVectors.Add(Microsoft.Kinect.JointType.SpineShoulder, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.SpineShoulder], kinectJoints[Microsoft.Kinect.JointType.SpineMid]));
			jointVectors.Add(Microsoft.Kinect.JointType.ShoulderLeft, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.ShoulderLeft], kinectJoints[Microsoft.Kinect.JointType.SpineShoulder]));
			jointVectors.Add(Microsoft.Kinect.JointType.ElbowLeft, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.ElbowLeft], kinectJoints[Microsoft.Kinect.JointType.ShoulderLeft]));
			jointVectors.Add(Microsoft.Kinect.JointType.WristLeft, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.WristLeft], kinectJoints[Microsoft.Kinect.JointType.ElbowLeft]));
			jointVectors.Add(Microsoft.Kinect.JointType.HandLeft, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.HandLeft], kinectJoints[Microsoft.Kinect.JointType.WristLeft]));
			jointVectors.Add(Microsoft.Kinect.JointType.HandTipLeft, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.HandTipLeft], kinectJoints[Microsoft.Kinect.JointType.HandLeft]));
			jointVectors.Add(Microsoft.Kinect.JointType.ThumbLeft, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.ThumbLeft], kinectJoints[Microsoft.Kinect.JointType.HandLeft]));
			jointVectors.Add(Microsoft.Kinect.JointType.Neck, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.Neck], kinectJoints[Microsoft.Kinect.JointType.SpineShoulder]));
			jointVectors.Add(Microsoft.Kinect.JointType.Head, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.Head], kinectJoints[Microsoft.Kinect.JointType.Neck]));
			jointVectors.Add(Microsoft.Kinect.JointType.ShoulderRight, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.ShoulderRight], kinectJoints[Microsoft.Kinect.JointType.SpineShoulder]));
			jointVectors.Add(Microsoft.Kinect.JointType.ElbowRight, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.ElbowRight], kinectJoints[Microsoft.Kinect.JointType.ShoulderRight]));
			jointVectors.Add(Microsoft.Kinect.JointType.WristRight, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.WristRight], kinectJoints[Microsoft.Kinect.JointType.ElbowRight]));
			jointVectors.Add(Microsoft.Kinect.JointType.HandRight, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.HandRight], kinectJoints[Microsoft.Kinect.JointType.WristRight]));
			jointVectors.Add(Microsoft.Kinect.JointType.HandTipRight, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.HandTipRight], kinectJoints[Microsoft.Kinect.JointType.HandRight]));
			jointVectors.Add(Microsoft.Kinect.JointType.ThumbRight, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.ThumbRight], kinectJoints[Microsoft.Kinect.JointType.HandRight]));

			// Spine base is the root of the system, as it has no direction to store, it stores its own position
			jointVectors.Add(Microsoft.Kinect.JointType.SpineBase, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.SpineBase], new Joint()));
			jointVectors.Add(Microsoft.Kinect.JointType.HipLeft, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.HipLeft], kinectJoints[Microsoft.Kinect.JointType.SpineBase]));
			jointVectors.Add(Microsoft.Kinect.JointType.KneeLeft, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.KneeLeft], kinectJoints[Microsoft.Kinect.JointType.HipLeft]));
			jointVectors.Add(Microsoft.Kinect.JointType.AnkleLeft, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.AnkleLeft], kinectJoints[Microsoft.Kinect.JointType.KneeLeft]));
			jointVectors.Add(Microsoft.Kinect.JointType.FootLeft, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.FootLeft], kinectJoints[Microsoft.Kinect.JointType.AnkleLeft]));
			jointVectors.Add(Microsoft.Kinect.JointType.HipRight, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.HipRight], kinectJoints[Microsoft.Kinect.JointType.SpineBase]));
			jointVectors.Add(Microsoft.Kinect.JointType.KneeRight, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.KneeRight], kinectJoints[Microsoft.Kinect.JointType.HipRight]));
			jointVectors.Add(Microsoft.Kinect.JointType.AnkleRight, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.AnkleRight], kinectJoints[Microsoft.Kinect.JointType.KneeRight]));
			jointVectors.Add(Microsoft.Kinect.JointType.FootRight, SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.FootRight], kinectJoints[Microsoft.Kinect.JointType.AnkleRight]));

			var rotationMatrix = new Matrix3D();

			InitMatrix(out rotationMatrix, kinectJoints);

			rotationMatrix.Invert();

			var joints = new Dictionary<PreposeGestures.JointType, Z3Point3D>();
			var norms = new Dictionary<PreposeGestures.JointType, ArithExpr>();

            norms.Add(PreposeGestures.JointType.SpineBase, Z3Math.Real(jointVectors[Microsoft.Kinect.JointType.SpineBase].Length));
            joints.Add(PreposeGestures.JointType.SpineBase,
                       new Z3Point3D(
                        jointVectors[Microsoft.Kinect.JointType.SpineBase].X,
                        jointVectors[Microsoft.Kinect.JointType.SpineBase].Y,
                        jointVectors[Microsoft.Kinect.JointType.SpineBase].Z));

			var jointTypes = EnumUtil.GetValues<Microsoft.Kinect.JointType>();
			foreach (var jointType in jointTypes)
			{
                if (jointType != Microsoft.Kinect.JointType.SpineBase)
				{
					jointVectors[jointType] = jointVectors[jointType] * rotationMatrix;

					var z3Joint = Convert(jointType);

					norms.Add(z3Joint, Z3Math.Real(jointVectors[jointType].Length));

                    var temp = jointVectors[jointType];
                    temp.Normalize();

					joints.Add(
						z3Joint,
						new Z3Point3D(
						temp.X,
						temp.Y,
						-temp.Z));
				}
			}

			return new Z3Body(joints, norms);
		}

        /// <summary>
        /// This maps a Z3 body to Kinect joints. 
        /// Inverse of the method above. 
        /// Does coordinate mapping as well.
        /// </summary>
        /// <param name="baseJoints"></param>
        /// <param name="target"></param>
        /// <returns></returns>
		public static Dictionary<Microsoft.Kinect.JointType, Microsoft.Kinect.Joint> 
            CreateKinectJoints(
			IReadOnlyDictionary<Microsoft.Kinect.JointType, Microsoft.Kinect.Joint> 
            baseJoints,
			Z3Target target)
		{
			// Calc base rotation matrix
			var rotationMatrix = new Matrix3D();
			InitMatrix(out rotationMatrix, baseJoints);

			// Acquire all vectors from Z3 target
			var jointVectors = new Dictionary<Microsoft.Kinect.JointType, Vector3D>();
			
			// Spine base is the root of the system, as it has no direction to store, it stores its own position
			jointVectors.Add(
				Microsoft.Kinect.JointType.SpineBase, 
				new Vector3D(
					baseJoints[Microsoft.Kinect.JointType.SpineBase].Position.X,
					baseJoints[Microsoft.Kinect.JointType.SpineBase].Position.Y,
					baseJoints[Microsoft.Kinect.JointType.SpineBase].Position.Z));

			var z3JointTypes = EnumUtil.GetValues<PreposeGestures.JointType>();
            var targetJoints = target.TransformedJoints;
			foreach (var jointType in z3JointTypes)
			{
				if (jointType != PreposeGestures.JointType.SpineBase)
				{
					// If target has calculated the joint add target jointType
                    if (targetJoints.Contains(jointType))
					{
                        AddZ3JointToVectors3D(target.Body, baseJoints, jointVectors, jointType);
					}
					// Or else use the joint from current Kinect data (baseJoints)
					else
					{
						jointVectors.Add(
							(Microsoft.Kinect.JointType)jointType,
							new Vector3D(
								baseJoints[(Microsoft.Kinect.JointType)jointType].Position.X,
								baseJoints[(Microsoft.Kinect.JointType)jointType].Position.Y,
								baseJoints[(Microsoft.Kinect.JointType)jointType].Position.Z));
					}
				}
			}

			// Rotate all vectors to the current base body coordinate system
			var kinectJointTypes = EnumUtil.GetValues<Microsoft.Kinect.JointType>();
			foreach (var jointType in kinectJointTypes)
			{
				if (jointType != Microsoft.Kinect.JointType.SpineBase &&
                    targetJoints.Contains((PreposeGestures.JointType)jointType))
				{
					jointVectors[jointType] *= rotationMatrix;				
				}
			}

			// Add base body position (spineBase position) to translate result
			// The operations order matter in this case			
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.SpineMid);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.SpineShoulder);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.Neck);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.Head);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.ShoulderLeft);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.ElbowLeft);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.WristLeft);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.HandLeft);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.HandTipLeft);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.ThumbLeft);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.ShoulderRight);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.ElbowRight);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.WristRight);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.HandRight);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.HandTipRight);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.ThumbRight);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.HipLeft);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.KneeLeft);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.AnkleLeft);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.FootLeft);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.HipRight);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.KneeRight);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.AnkleRight);
            SumWithFatherPosition(jointVectors, targetJoints, Microsoft.Kinect.JointType.FootRight);

			// Filling the results
			var result = new Dictionary<Microsoft.Kinect.JointType, Microsoft.Kinect.Joint>();

			foreach (var jointType in kinectJointTypes)
			{
				var joint = new Joint();
                var position = new CameraSpacePoint();
                position.X = (float)jointVectors[jointType].X;
                position.Y = (float)jointVectors[jointType].Y;
                position.Z = (float)jointVectors[jointType].Z;

				joint.Position = position;
				joint.TrackingState = TrackingState.Tracked;

				result.Add(jointType, joint);
			}

			return result;
		}


        /// <summary>
        /// This returns a fake body for testing.
        /// </summary>
        /// <returns></returns>
        internal static
            Dictionary<Microsoft.Kinect.JointType, Microsoft.Kinect.Joint>
            CreateSyntheticJoints()
        {
            var result = new Dictionary<Microsoft.Kinect.JointType, Microsoft.Kinect.Joint>();

            var position = new CameraSpacePoint();
            var SpineBase = new Joint();
            position.X = 0.180275574f;
            position.Y = -0.31635946f;
            position.Z = 2.11578083f;
            SpineBase.Position = position;
            SpineBase.TrackingState = TrackingState.Tracked;

            var SpineMid = new Joint();
            position.X = 0.171743378f;
            position.Y = -0.01149734f;
            position.Z = 2.110403f;
            SpineMid.Position = position;
            SpineMid.TrackingState = TrackingState.Tracked;

            var Neck = new Joint();
            position.X = 0.16232343f;
            position.Y = 0.284290969f;
            position.Z = 2.09287357f;
            Neck.Position = position;
            Neck.TrackingState = TrackingState.Tracked;

            var Head = new Joint();
            position.X = 0.167406484f;
            position.Y = 0.414793521f;
            position.Z = 2.06464386f;
            Head.Position = position;
            Head.TrackingState = TrackingState.Tracked;

            var ShoulderLeft = new Joint();
            position.X = 0.006158624f;
            position.Y = 0.16339241f;
            position.Z = 2.09529877f;
            ShoulderLeft.Position = position;
            ShoulderLeft.TrackingState = TrackingState.Tracked;

            var ElbowLeft = new Joint();
            position.X = -0.0233890526f;
            position.Y = -0.08364678f;
            position.Z = 2.09722638f;
            ElbowLeft.Position = position;
            ElbowLeft.TrackingState = TrackingState.Tracked;

            var WristLeft = new Joint();
            position.X = -0.0796562f;
            position.Y = -0.2848963f;
            position.Z = 2.03556085f;
            WristLeft.Position = position;
            WristLeft.TrackingState = TrackingState.Tracked;

            var HandLeft = new Joint();
            position.X = -0.0844078958f;
            position.Y = -0.343340725f;
            position.Z = 2.03300261f;
            HandLeft.Position = position;
            HandLeft.TrackingState = TrackingState.Tracked;

            var ShoulderRight = new Joint();
            position.X = 0.313944131f;
            position.Y = 0.169815212f;
            position.Z = 2.070334f;
            ShoulderRight.Position = position;
            ShoulderRight.TrackingState = TrackingState.Tracked;

            var ElbowRight = new Joint();
            position.X = 0.360907167f;
            position.Y = -0.08759691f;
            position.Z = 2.086809f;
            ElbowRight.Position = position;
            ElbowRight.TrackingState = TrackingState.Tracked;

            var WristRight = new Joint();
            position.X = 0.425080419f;
            position.Y = -0.286533624f;
            position.Z = 2.02806973f;
            WristRight.Position = position;
            WristRight.TrackingState = TrackingState.Tracked;

            var HandRight = new Joint();
            position.X = 0.424313754f;
            position.Y = -0.3093492f;
            position.Z = 2.03541732f;
            HandRight.Position = position;
            HandRight.TrackingState = TrackingState.Tracked;

            var HipLeft = new Joint();
            position.X = 0.110814437f;
            position.Y = -0.306975663f;
            position.Z = 2.08125377f;
            HipLeft.Position = position;
            HipLeft.TrackingState = TrackingState.Tracked;

            var KneeLeft = new Joint();
            position.X = 0.0678925142f;
            position.Y = -0.5989468f;
            position.Z = 2.12125182f;
            KneeLeft.Position = position;
            KneeLeft.TrackingState = TrackingState.Tracked;

            var AnkleLeft = new Joint();
            position.X = 0.0534116626f;
            position.Y = -0.849217057f;
            position.Z = 2.15557933f;
            AnkleLeft.Position = position;
            AnkleLeft.TrackingState = TrackingState.Tracked;

            var FootLeft = new Joint();
            position.X = 0.0447675139f;
            position.Y = -0.877429366f;
            position.Z = 2.037694f;
            FootLeft.Position = position;
            FootLeft.TrackingState = TrackingState.Tracked;

            var HipRight = new Joint();
            position.X = 0.243845716f;
            position.Y = -0.315492958f;
            position.Z = 2.08171487f;
            HipRight.Position = position;
            HipRight.TrackingState = TrackingState.Tracked;

            var KneeRight = new Joint();
            position.X = 0.333192945f;
            position.Y = -0.59623307f;
            position.Z = 2.14912224f;
            KneeRight.Position = position;
            KneeRight.TrackingState = TrackingState.Tracked;

            var AnkleRight = new Joint();
            position.X = 0.3287078f;
            position.Y = -0.8853898f;
            position.Z = 2.142943f;
            AnkleRight.Position = position;
            AnkleRight.TrackingState = TrackingState.Tracked;

            var FootRight = new Joint();
            position.X = 0.3904703f;
            position.Y = -0.9469924f;
            position.Z = 2.158338f;
            FootRight.Position = position;
            FootRight.TrackingState = TrackingState.Tracked;

            var SpineShoulder = new Joint();
            position.X = 0.164821967f;
            position.Y = 0.211534128f;
            position.Z = 2.09942484f;
            SpineShoulder.Position = position;
            SpineShoulder.TrackingState = TrackingState.Tracked;

            var HandTipLeft = new Joint();
            position.X = -0.07759059f;
            position.Y = -0.390459776f;
            position.Z = 2.0392375f;
            HandTipLeft.Position = position;
            HandTipLeft.TrackingState = TrackingState.Tracked;

            var ThumbLeft = new Joint();
            position.X = -0.06412995f;
            position.Y = -0.368352175f;
            position.Z = 2.049333f;
            ThumbLeft.Position = position;
            ThumbLeft.TrackingState = TrackingState.Tracked;

            var HandTipRight = new Joint();
            position.X = 0.435322165f;
            position.Y = -0.3547445f;
            position.Z = 2.03026271f;
            HandTipRight.Position = position;
            HandTipRight.TrackingState = TrackingState.Tracked;

            var ThumbRight = new Joint();
            position.X = 0.428867549f;
            position.Y = -0.327544183f;
            position.Z = 2.03225f;
            ThumbRight.Position = position;
            ThumbRight.TrackingState = TrackingState.Tracked;


            result.Add(Microsoft.Kinect.JointType.SpineBase, SpineBase);
            result.Add(Microsoft.Kinect.JointType.SpineMid, SpineMid);
            result.Add(Microsoft.Kinect.JointType.SpineShoulder, SpineShoulder);
            result.Add(Microsoft.Kinect.JointType.Neck, Neck);
            result.Add(Microsoft.Kinect.JointType.Head, Head);
            result.Add(Microsoft.Kinect.JointType.ShoulderLeft, ShoulderLeft);
            result.Add(Microsoft.Kinect.JointType.ElbowLeft, ElbowLeft);
            result.Add(Microsoft.Kinect.JointType.WristLeft, WristLeft);
            result.Add(Microsoft.Kinect.JointType.HandLeft, HandLeft);
            result.Add(Microsoft.Kinect.JointType.HandTipLeft, HandTipLeft);
            result.Add(Microsoft.Kinect.JointType.ThumbLeft, ThumbLeft);
            result.Add(Microsoft.Kinect.JointType.HipLeft, HipLeft);
            result.Add(Microsoft.Kinect.JointType.KneeLeft, KneeLeft);
            result.Add(Microsoft.Kinect.JointType.AnkleLeft, AnkleLeft);
            result.Add(Microsoft.Kinect.JointType.FootLeft, FootLeft);
            result.Add(Microsoft.Kinect.JointType.ShoulderRight, ShoulderRight);
            result.Add(Microsoft.Kinect.JointType.ElbowRight, ElbowRight);
            result.Add(Microsoft.Kinect.JointType.WristRight, WristRight);
            result.Add(Microsoft.Kinect.JointType.HandRight, HandRight);
            result.Add(Microsoft.Kinect.JointType.HandTipRight, HandTipRight);
            result.Add(Microsoft.Kinect.JointType.ThumbRight, ThumbRight);
            result.Add(Microsoft.Kinect.JointType.HipRight, HipRight);
            result.Add(Microsoft.Kinect.JointType.KneeRight, KneeRight);
            result.Add(Microsoft.Kinect.JointType.AnkleRight, AnkleRight);
            result.Add(Microsoft.Kinect.JointType.FootRight, FootRight);

            return result;
        }

		private static void SumWithFatherPosition(
			Dictionary<Microsoft.Kinect.JointType, Vector3D> jointVectors,
			List<PreposeGestures.JointType> targetCalculatedJoints,
			Microsoft.Kinect.JointType jointType)
		{
			if (targetCalculatedJoints.Contains((PreposeGestures.JointType)jointType))
				jointVectors[jointType] +=
					jointVectors[(Microsoft.Kinect.JointType)JointTypeHelper.GetFather((PreposeGestures.JointType)jointType)];
		}

		private static void AddZ3JointToVectors3D(
			Z3Body targetBody, 
            IReadOnlyDictionary<Microsoft.Kinect.JointType, Microsoft.Kinect.Joint> baseJoints,
			Dictionary<Microsoft.Kinect.JointType, Vector3D> jointVectors,
			PreposeGestures.JointType jointType)
		{
			var vector3D = new Vector3D(
							 targetBody.Joints[jointType].GetXValue(),
							 targetBody.Joints[jointType].GetYValue(),
							 -targetBody.Joints[jointType].GetZValue());

            //var norm = Z3Math.GetRealValue(targetBody.Norms[jointType]);
            var norm = 
                new Vector3D(
                baseJoints[(Microsoft.Kinect.JointType)jointType].Position.X - baseJoints[(Microsoft.Kinect.JointType)JointTypeHelper.GetFather(jointType)].Position.X,
                baseJoints[(Microsoft.Kinect.JointType)jointType].Position.Y - baseJoints[(Microsoft.Kinect.JointType)JointTypeHelper.GetFather(jointType)].Position.Y,
                baseJoints[(Microsoft.Kinect.JointType)jointType].Position.Z - baseJoints[(Microsoft.Kinect.JointType)JointTypeHelper.GetFather(jointType)].Position.Z).Length;

            vector3D = vector3D * norm;

			jointVectors.Add((Microsoft.Kinect.JointType)jointType, vector3D);
		}

		private static PreposeGestures.JointType Convert(Microsoft.Kinect.JointType jointType)
		{
			return (PreposeGestures.JointType)jointType;
		}

		private static void InitMatrix(out Matrix3D rotationMatrix, IReadOnlyDictionary<Microsoft.Kinect.JointType, Microsoft.Kinect.Joint> kinectJoints)
		{
			var j = new Vector3D(0.0, 1.0, 0.0);

			var i = SubtractJoints(kinectJoints[Microsoft.Kinect.JointType.HipRight], kinectJoints[Microsoft.Kinect.JointType.HipLeft]);
			i.Normalize();

			Vector3D k = Vector3D.CrossProduct(i, j);
			k.Normalize();

			rotationMatrix = new Matrix3D();

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


        private static Vector3D SubtractJoints(Joint j1, Joint j2)
        {
            return new Vector3D(j1.Position.X - j2.Position.X,
                                j1.Position.Y - j2.Position.Y,
                                j1.Position.Z - j2.Position.Z);
        }
    }
}
