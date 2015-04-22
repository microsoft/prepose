using Microsoft.Z3;
using System;
using System.Collections.Generic;

namespace PreposeGestures
{
	public class JointTypeHelper
	{
		public static JointType GetSidedJointType(SidedJointName name, JointSide side)
		{
			switch (name)
			{
				case (SidedJointName.Ankle):
					return (side == JointSide.Left) ? JointType.AnkleLeft : JointType.AnkleRight;
				case (SidedJointName.Elbow):
					return (side == JointSide.Left) ? JointType.ElbowLeft : JointType.ElbowRight;
				case (SidedJointName.Foot):
					return (side == JointSide.Left) ? JointType.FootLeft : JointType.FootRight;
				case (SidedJointName.Hand):
					return (side == JointSide.Left) ? JointType.HandLeft : JointType.HandRight;
				case (SidedJointName.HandTip):
					return (side == JointSide.Left) ? JointType.HandTipLeft : JointType.HandTipRight;
				case (SidedJointName.Hip):
					return (side == JointSide.Left) ? JointType.HipLeft : JointType.HipRight;
				case (SidedJointName.Knee):
					return (side == JointSide.Left) ? JointType.KneeLeft : JointType.KneeRight;
				case (SidedJointName.Shoulder):
					return (side == JointSide.Left) ? JointType.ShoulderLeft : JointType.ShoulderRight;
				case (SidedJointName.Thumb):
					return (side == JointSide.Left) ? JointType.ThumbLeft : JointType.ThumbRight;
				case (SidedJointName.Wrist):
					return (side == JointSide.Left) ? JointType.WristLeft : JointType.WristRight;

				default:
					throw new ArgumentException();
			}
		}

		// SpineBase is the root of the coordinate system
		public static JointType GetFather(JointType type)
		{
			JointType result = JointType.SpineMid;

			if (type == JointType.SpineMid) result = JointType.SpineBase;
			else if (type == JointType.SpineShoulder) result = JointType.SpineMid;
			else if (type == JointType.ShoulderLeft) result = JointType.SpineShoulder;
			else if (type == JointType.ElbowLeft) result = JointType.ShoulderLeft;
			else if (type == JointType.WristLeft) result = JointType.ElbowLeft;
			else if (type == JointType.HandLeft) result = JointType.WristLeft;
			else if (type == JointType.HandTipLeft) result = JointType.HandLeft;
			else if (type == JointType.ThumbLeft) result = JointType.WristLeft;
			else if (type == JointType.Neck) result = JointType.SpineShoulder;
			else if (type == JointType.Head) result = JointType.Neck;
			else if (type == JointType.ShoulderRight) result = JointType.SpineShoulder;
			else if (type == JointType.ElbowRight) result = JointType.ShoulderRight;
			else if (type == JointType.WristRight) result = JointType.ElbowRight;
			else if (type == JointType.HandRight) result = JointType.WristRight;
			else if (type == JointType.HandTipRight) result = JointType.HandRight;
			else if (type == JointType.ThumbRight) result = JointType.WristRight;
			else if (type == JointType.SpineBase) result = JointType.SpineBase;
			else if (type == JointType.HipLeft) result = JointType.SpineBase;
			else if (type == JointType.KneeLeft) result = JointType.HipLeft;
			else if (type == JointType.AnkleLeft) result = JointType.KneeLeft;
			else if (type == JointType.FootLeft) result = JointType.AnkleLeft;
			else if (type == JointType.HipRight) result = JointType.SpineBase;
			else if (type == JointType.KneeRight) result = JointType.HipRight;
			else if (type == JointType.AnkleRight) result = JointType.KneeRight;
			else if (type == JointType.FootRight) result = JointType.AnkleRight;

			return result;
		}

		// Arm considers only the elbow and the wrist,
		// This decision is taken based on common sense that if you ask
		// the user to put his arm in a specific direction,
		// he may do it with his hand pointing another direction and
		// it would still be correct.
		public static IEnumerable<JointType> GetArm(JointSide side)
		{
			yield return GetSidedJointType(SidedJointName.Wrist, side);
			yield return GetSidedJointType(SidedJointName.Elbow, side);
		}

		public static IEnumerable<JointType> GetArms()
		{
			yield return JointType.ElbowLeft;
			yield return JointType.WristLeft;
			yield return JointType.ElbowRight;
			yield return JointType.WristRight;
		}

		public static IEnumerable<JointType> GetLeg(JointSide side)
		{
			yield return GetSidedJointType(SidedJointName.Knee, side);
			yield return GetSidedJointType(SidedJointName.Ankle, side);
		}

		public static IEnumerable<JointType> GetLegs()
		{
			yield return JointType.KneeLeft;
			yield return JointType.AnkleLeft;
			yield return JointType.KneeRight;
			yield return JointType.AnkleRight;
		}

		public static IEnumerable<JointType> GetShoulders()
		{
			yield return JointType.ShoulderLeft;
			yield return JointType.ShoulderRight;
		}

		public static IEnumerable<JointType> GetElbows()
		{
			yield return JointType.ElbowLeft;
			yield return JointType.ElbowRight;
		}

		public static IEnumerable<JointType> GetWrists()
		{
			yield return JointType.WristLeft;
			yield return JointType.WristRight;
		}

		public static IEnumerable<JointType> GetHands()
		{
			yield return JointType.HandLeft;
			yield return JointType.HandRight;
		}

		public static IEnumerable<JointType> GetHandsTips()
		{
			yield return JointType.HandTipLeft;
			yield return JointType.HandTipRight;
		}

		public static IEnumerable<JointType> GetThumbs()
		{
			yield return JointType.ThumbLeft;
			yield return JointType.ThumbRight;
		}

		public static IEnumerable<JointType> GetHips()
		{
			yield return JointType.HipLeft;
			yield return JointType.HipRight;
		}

		public static IEnumerable<JointType> GetKnees()
		{
			yield return JointType.KneeLeft;
			yield return JointType.KneeRight;
		}

		public static IEnumerable<JointType> GetAnkles()
		{
			yield return JointType.AnkleLeft;
			yield return JointType.AnkleRight;
		}

		public static IEnumerable<JointType> GetFeet()
		{
			yield return JointType.FootLeft;
			yield return JointType.FootRight;
		}

		public static IEnumerable<JointType> GetBack()
		{
			yield return JointType.SpineMid;
			yield return JointType.SpineShoulder;
			yield return JointType.ShoulderLeft;
			yield return JointType.ShoulderRight;
			yield return JointType.HipLeft;
			yield return JointType.HipRight;
		}

		public static IEnumerable<JointType> GetYou()
		{
			yield return JointType.SpineMid;
			yield return JointType.SpineShoulder;
			yield return JointType.ShoulderLeft;
			yield return JointType.ShoulderRight;
			yield return JointType.HipLeft;
			yield return JointType.HipRight;
			yield return JointType.Neck;
			yield return JointType.Head;
		}

		public static List<JointType> MergeJointTypeLists(List<JointType> list1, List<JointType> list2)
		{
			List<JointType> result = new List<JointType>();

			foreach(var joint in list1)
			{
				result.Add(joint);
			}

			foreach (var joint in list2)
			{
				if (!result.Contains(joint))
					result.Add(joint);
			}

			return result;
		}

		public static List<JointType> MergeJointTypeLists(
			List<JointType> list1, 
			List<JointType> list2,
			List<JointType> list3)
		{
			List<JointType> result = MergeJointTypeLists(list1, list2);
			result = MergeJointTypeLists(result, list3);

			return result;
		}

		public static Dictionary<JointType, Z3Point3D> CreateSyntheticJoints()
		{
			var result = new Dictionary<JointType, Z3Point3D>();

			Z3Point3D spineBase = Z3Point3D.DirectionPoint(Direction.Front);
			Z3Point3D spineMid = Z3Point3D.DirectionPoint(Direction.Up);
			Z3Point3D spineShoulder = Z3Point3D.DirectionPoint(Direction.Up);
			Z3Point3D neck = Z3Point3D.DirectionPoint(Direction.Up);
			Z3Point3D head = Z3Point3D.DirectionPoint(Direction.Up);

			Z3Point3D shoulderLeft = new Z3Point3D();
			shoulderLeft.X = Z3Math.Real(-0.7);
			shoulderLeft.Y = Z3Math.Real(-0.7);
			shoulderLeft.Z = Z3Math.Zero;
			Z3Point3D elbowLeft = Z3Point3D.DirectionPoint(Direction.Down);
			Z3Point3D wristLeft = Z3Point3D.DirectionPoint(Direction.Down);
			Z3Point3D handLeft = Z3Point3D.DirectionPoint(Direction.Down);
			Z3Point3D handTipLeft = Z3Point3D.DirectionPoint(Direction.Down);
			Z3Point3D thumbLeft = Z3Point3D.DirectionPoint(Direction.Down);
			Z3Point3D hipLeft = new Z3Point3D();
			shoulderLeft.X = Z3Math.Real(-0.7);
			shoulderLeft.Y = Z3Math.Real(-0.7);
			shoulderLeft.Z = Z3Math.Zero;
			Z3Point3D kneeLeft = Z3Point3D.DirectionPoint(Direction.Down);
			Z3Point3D ankleLeft = Z3Point3D.DirectionPoint(Direction.Down);
			Z3Point3D footLeft = Z3Point3D.DirectionPoint(Direction.Down);

			Z3Point3D shoulderRight = new Z3Point3D();
			shoulderRight.X = Z3Math.Real(0.7);
			shoulderRight.Y = Z3Math.Real(-0.7);
			shoulderRight.Z = Z3Math.Zero;
			Z3Point3D elbowRight = Z3Point3D.DirectionPoint(Direction.Down);
			Z3Point3D wristRight = Z3Point3D.DirectionPoint(Direction.Down);
			Z3Point3D handRight = Z3Point3D.DirectionPoint(Direction.Down);
			Z3Point3D handTipRight = Z3Point3D.DirectionPoint(Direction.Down);
			Z3Point3D thumbRight = Z3Point3D.DirectionPoint(Direction.Down);
			Z3Point3D hipRight = new Z3Point3D();
			shoulderRight.X = Z3Math.Real(0.7);
			shoulderRight.Y = Z3Math.Real(-0.7);
			shoulderRight.Z = Z3Math.Zero;
			Z3Point3D kneeRight = Z3Point3D.DirectionPoint(Direction.Down);
			Z3Point3D ankleRight = Z3Point3D.DirectionPoint(Direction.Down);
			Z3Point3D footRight = Z3Point3D.DirectionPoint(Direction.Down);

			result.Add(JointType.SpineBase, spineBase);
			result.Add(JointType.SpineMid, spineMid);
			result.Add(JointType.SpineShoulder, spineShoulder);
			result.Add(JointType.Neck, neck);
			result.Add(JointType.Head, head);
			result.Add(JointType.ShoulderLeft, shoulderLeft);
			result.Add(JointType.ElbowLeft, elbowLeft);
			result.Add(JointType.WristLeft, wristLeft);
			result.Add(JointType.HandLeft, handLeft);
			result.Add(JointType.HandTipLeft, handTipLeft);
			result.Add(JointType.ThumbLeft, thumbLeft);
			result.Add(JointType.HipLeft, hipLeft);
			result.Add(JointType.KneeLeft, kneeLeft);
			result.Add(JointType.AnkleLeft, ankleLeft);
			result.Add(JointType.FootLeft, footLeft);
			result.Add(JointType.ShoulderRight, shoulderRight);
			result.Add(JointType.ElbowRight, elbowRight);
			result.Add(JointType.WristRight, wristRight);
			result.Add(JointType.HandRight, handRight);
			result.Add(JointType.HandTipRight, handTipRight);
			result.Add(JointType.ThumbRight, thumbRight);
			result.Add(JointType.HipRight, hipRight);
			result.Add(JointType.KneeRight, kneeRight);
			result.Add(JointType.AnkleRight, ankleRight);
			result.Add(JointType.FootRight, footRight);

			return result;
		}

		private static Dictionary<JointType, ArithExpr> CreateSyntheticNorms()
		{
			var result = new Dictionary<JointType, ArithExpr>();

			ArithExpr spineBase = Z3Math.Real(0.0);
			ArithExpr spineMid = Z3Math.Real(0.3);
			ArithExpr spineShoulder = Z3Math.Real(0.3);
			ArithExpr neck = Z3Math.Real(0.15);
			ArithExpr head = Z3Math.Real(0.15);
			ArithExpr shoulderLeft = Z3Math.Real(0.25);
			ArithExpr elbowLeft = Z3Math.Real(0.25);
			ArithExpr wristLeft = Z3Math.Real(0.25);
			ArithExpr handLeft = Z3Math.Real(0.05);
			ArithExpr handTipLeft = Z3Math.Real(0.05);
			ArithExpr thumbLeft = Z3Math.Real(0.05);
			ArithExpr hipLeft = Z3Math.Real(0.25);
			ArithExpr kneeLeft = Z3Math.Real(0.35);
			ArithExpr ankleLeft = Z3Math.Real(0.35);
			ArithExpr footLeft = Z3Math.Real(0.1);
			ArithExpr shoulderRight = Z3Math.Real(0.25);
			ArithExpr elbowRight = Z3Math.Real(0.25);
			ArithExpr wristRight = Z3Math.Real(0.25);
			ArithExpr handRight = Z3Math.Real(0.05);
			ArithExpr handTipRight = Z3Math.Real(0.05);
			ArithExpr thumbRight = Z3Math.Real(0.05);
			ArithExpr hipRight = Z3Math.Real(0.25);
			ArithExpr kneeRight = Z3Math.Real(0.35);
			ArithExpr ankleRight = Z3Math.Real(0.35);
			ArithExpr footRight = Z3Math.Real(0.1);

			result.Add(JointType.SpineBase, spineBase);
			result.Add(JointType.SpineMid, spineMid);
			result.Add(JointType.SpineShoulder, spineShoulder);
			result.Add(JointType.Neck, neck);
			result.Add(JointType.Head, head);
			result.Add(JointType.ShoulderLeft, shoulderLeft);
			result.Add(JointType.ElbowLeft, elbowLeft);
			result.Add(JointType.WristLeft, wristLeft);
			result.Add(JointType.HandLeft, handLeft);
			result.Add(JointType.HandTipLeft, handTipLeft);
			result.Add(JointType.ThumbLeft, thumbLeft);
			result.Add(JointType.HipLeft, hipLeft);
			result.Add(JointType.KneeLeft, kneeLeft);
			result.Add(JointType.AnkleLeft, ankleLeft);
			result.Add(JointType.FootLeft, footLeft);
			result.Add(JointType.ShoulderRight, shoulderRight);
			result.Add(JointType.ElbowRight, elbowRight);
			result.Add(JointType.WristRight, wristRight);
			result.Add(JointType.HandRight, handRight);
			result.Add(JointType.HandTipRight, handTipRight);
			result.Add(JointType.ThumbRight, thumbRight);
			result.Add(JointType.HipRight, hipRight);
			result.Add(JointType.KneeRight, kneeRight);
			result.Add(JointType.AnkleRight, ankleRight);
			result.Add(JointType.FootRight, footRight);

			return result;
		}

		internal static Z3Body CreateDefaultZ3Body()
		{
			return new Z3Body(JointTypeHelper.CreateSyntheticJoints(), JointTypeHelper.CreateSyntheticNorms());
		}

        internal static List<JointType> GetListFromLeafToRoot(JointType leaf)
        {
            var result = new List<JointType>();

            var father = GetFather(leaf);

            while(father != leaf)
            {
                result.Add(leaf);
                leaf = father;
                father = GetFather(father);
            }

            return result;
        }
    }
}
