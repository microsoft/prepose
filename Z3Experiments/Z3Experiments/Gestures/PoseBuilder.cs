using Microsoft.Z3;
using System.Collections.Generic;

namespace PreposeGestures
{
	public static class BodyRestrictionBuilder
	{
		// Therapy restrictions
		public static IBodyRestriction StraightPostureRestriction(int angleThreshold = 15)
		{
			Z3Point3D up = new Z3Point3D(0, 1, 0);

			double distanceThreshold = TrigonometryHelper.GetSine(angleThreshold);

			var result = new SimpleBodyRestriction(body =>
			{
				BoolExpr expr = body.Joints[JointType.SpineMid].IsNearerThan(up, distanceThreshold);
				expr = Z3.Context.MkAnd(expr, body.Joints[JointType.SpineShoulder].IsNearerThan(up, distanceThreshold));
				expr = Z3.Context.MkAnd(expr, body.Joints[JointType.Neck].IsNearerThan(up, distanceThreshold));
				expr = Z3.Context.MkAnd(expr, body.Joints[JointType.Head].IsNearerThan(up, distanceThreshold));
				return expr;
			});

			return result;
		}

		public static IBodyRestriction DistributeWeightRestriction(int angleThreshold = 15)
		{
			var result = new SimpleBodyRestriction(body =>
			{
				Z3Point3D up = new Z3Point3D(0, 1, 0);
				Z3Point3D legLeft = body.Joints[JointType.KneeLeft] + body.Joints[JointType.AnkleLeft];
				Z3Point3D legRight = body.Joints[JointType.KneeRight] + body.Joints[JointType.AnkleRight];
				Z3Point3D legsAverage = (legLeft + legRight) / 2;

				BoolExpr expr = legsAverage.IsAngleBetweenLessThan(up, angleThreshold);

				return expr;
			});

			return result;
		}

		public static IBodyRestriction TestBodyRestriction()
		{
			var result = new SimpleBodyRestriction(body =>
			{
				double distanceThreshold = 0.4;

				ArithExpr distanceThresholdSquared = Z3Math.Real(distanceThreshold * distanceThreshold);

				ArithExpr x1 = body.Joints[JointType.SpineMid].X;
				ArithExpr y1 = body.Joints[JointType.SpineMid].Y;
				ArithExpr z1 = body.Joints[JointType.SpineMid].Z;

				ArithExpr x2 = Z3Math.Zero;
				ArithExpr y2 = Z3Math.One;
				ArithExpr z2 = Z3Math.Zero;

				//ArithExpr x2 = body.Joints[JointType.SpineBase].X;
				//ArithExpr y2 = body.Joints[JointType.SpineBase].Y;
				//ArithExpr z2 = body.Joints[JointType.SpineBase].Z;

				var xExpr = Z3Math.Mul(Z3Math.Sub(x1, x2), Z3Math.Sub(x1, x2));
				var yExpr = Z3Math.Mul(Z3Math.Sub(y1, y2), Z3Math.Sub(y1, y2));
				var zExpr = Z3Math.Mul(Z3Math.Sub(z1, z2), Z3Math.Sub(z1, z2));

				var distanceSquared = Z3Math.Add(xExpr, yExpr, zExpr);

				// This is runs fine
				//var addExpr = Z3Math.Add(xExpr, Z3Math.Mul(Z3Math.Sub(y1, y2), y2));

				// This runs fine
				//var addExpr = Z3Math.Add(xExpr, Z3Math.Mul(Z3Math.Sub(y1, y2), Z3Math.Sub(y1, y1)));

				// This runs fine
				//var addExpr = Z3Math.Add(xExpr, Z3Math.Mul(Z3Math.Sub(y1, y2), Z3Math.Sub(y2, y2)));

				// This runs fine
				//var addExpr = Z3Math.Add(xExpr, Z3Math.Mul(Z3Math.Sub(y1, y2), Z3Math.Add(y1, y1)));

				// This runs fine
				//var addExpr = Z3Math.Add(xExpr, Z3Math.Mul(Z3Math.Sub(y1, y2), Z3Math.Add(y1, y2)));

				// This runs fine
				//var addExpr = Z3Math.Add(xExpr, Z3Math.Mul(Z3Math.Sub(y1, y2), Z3Math.Mul(y1, y2)));

				// This runs fine
				//var addExpr = Z3Math.Add(
				//    Z3Math.Mul(Z3Math.Sub(x1, x2), Z3Math.Sub(x1, x2)),
				//    Z3Math.Mul(Z3Math.Sub(y1, y2), Z3Math.Add(y1, y2)));

				// But this does not
				//var addExpr = Z3Math.Add(
				//    Z3Math.Mul(Z3Math.Sub(x1, x2), Z3Math.Sub(x1, x2)),
				//    Z3Math.Mul(Z3Math.Add(y1, y2), Z3Math.Add(y1, y2)));

				// This fine
				//var addExpr = Z3Math.Add(
				//    Z3Math.Mul(Z3Math.Sub(x1, x2), Z3Math.Sub(x1, x2)),
				//    Z3Math.Mul(Z3Math.Sub(y1, y2), Z3Math.Sub(y1, Z3.Context.MkReal(0))));

				// This does not
				//var addExpr = Z3Math.Add(
				//    Z3Math.Mul(Z3Math.Sub(x1, x2), Z3Math.Sub(x1, x2)),
				//    Z3Math.Mul(Z3Math.Sub(y1, y2), Z3Math.Sub(y1, Z3.Context.MkReal(1))));

				// This runs fine
				//var addExpr = Z3Math.Add(
				//    Z3Math.Mul(Z3Math.Sub(x1, x2), Z3Math.Sub(x1, x2)),
				//    Z3Math.Mul(Z3Math.Sub(y1, y2), Z3Math.Sub(y1, Z3.Context.MkReal(1, 7))));

				// This runs fine
				//var addExpr = Z3Math.Add(
				//    Z3Math.Mul(Z3Math.Sub(x1, x2), Z3Math.Sub(x1, x2)),
				//    Z3Math.Mul(Z3Math.Sub(y1, Z3.Context.MkReal(1, 6)), Z3Math.Sub(y1, Z3.Context.MkReal(1, 6))));

				// This runs fine
				//var addExpr = Z3Math.Add(
				//    Z3Math.Mul(Z3Math.Sub(x1, x2), Z3Math.Sub(x1, x2)),
				//    Z3Math.Mul(Z3Math.Sub(y1, Z3.Context.MkReal(1, 3)), Z3Math.Sub(y1, Z3.Context.MkReal(1, 3))));

				// This does not
				// asserting: (x1 - 0)² + (y1 - 1/2)² <= 0.4²
				// which is x1² + y1² - y/2 - y/2 + 1/4 <= 0.16
				// which is x1² + y1² - y + 0.25 <= 0.16
				// which is x1² + y1² - y <= -0.09
				// if x1 = 0 and y1 = 0.1 we would have a solution 
				// 0 + 0.01 - 0.1 <= 0.09
				// 0.09 <= 0.09
				// but z3 is taking too long to solve this.
				var addExpr = Z3Math.Add(
					Z3Math.Mul(Z3Math.Sub(x1, x2), Z3Math.Sub(x1, x2)),
					Z3Math.Mul(Z3Math.Sub(y1, Z3.Context.MkReal(1, 2)), Z3Math.Sub(y1, Z3.Context.MkReal(1, 2))));

				// But this is not (Z3 just stops while solving it)
				//var addExpr = Z3Math.Add(xExpr, Z3Math.Mul(Z3Math.Sub(y1, y2), Z3Math.Sub(y1, y2)));
				//var addExpr = Z3Math.Add(xExpr, Z3Math.Mul(Z3Math.Sub(y1, y2), Z3Math.Sub(y1, y1)));

				//BoolExpr expr = Z3.Context.MkLt(distanceSquared, distanceThresholdSquared);
				BoolExpr expr = Z3.Context.MkLe(addExpr, distanceThresholdSquared);

				// Why it complains about this Greater-Than case?
				//BoolExpr expr = Z3.Context.MkGt(xExpr, distanceThresholdSquared);

				return expr;
			});

			return result;
		}        

		public static IBodyRestriction DontTwistHipRestriction(int angleThreshold = 15)
		{
			Z3Point3D up = new Z3Point3D(0, 1, 0);

			var result = new SimpleBodyRestriction(body =>
			{
				// Shoulders, hips and feet must be aligned
				Z3Point3D leftToRightShoulderVec = body.Joints[JointType.ShoulderLeft].GetInverted() + body.Joints[JointType.ShoulderRight];
				Z3Point3D leftToRightHipVec = body.Joints[JointType.HipLeft].GetInverted() + body.Joints[JointType.HipRight];

				Z3Point3D leftAnklePosition =
					body.Joints[JointType.HipLeft] +
					body.Joints[JointType.KneeLeft] +
					body.Joints[JointType.AnkleLeft];

				Z3Point3D rightAnklePosition =
					body.Joints[JointType.HipRight] +
					body.Joints[JointType.KneeRight] +
					body.Joints[JointType.AnkleRight];

				Z3Point3D leftToRightAnkleVec = rightAnklePosition - leftAnklePosition;

				BoolExpr expr1 = leftToRightShoulderVec.IsAngleBetweenLessThan(leftToRightHipVec, angleThreshold);
				BoolExpr expr2 = leftToRightShoulderVec.IsAngleBetweenLessThan(leftToRightAnkleVec, angleThreshold);
				BoolExpr expr3 = leftToRightAnkleVec.IsAngleBetweenLessThan(leftToRightHipVec, angleThreshold);

				BoolExpr expr = Z3.Context.MkAnd(expr1, expr2, expr3);

				return expr;
			});

			return result;
		}

		public static IBodyRestriction ShouldersRelaxedRestriction(int angleThreshold = -5)
		{
			double maxY = TrigonometryHelper.GetSine(angleThreshold);

			var result = new SimpleBodyRestriction
				(body =>
				{
					// Check if both shoulders have a lower Y than maxY
					BoolExpr expr1 = Z3.Context.MkLt(body.Joints[JointType.ShoulderLeft].Y, Z3Math.Real(maxY));
					BoolExpr expr2 = Z3.Context.MkLt(body.Joints[JointType.ShoulderRight].Y, Z3Math.Real(maxY));

					BoolExpr expr = Z3.Context.MkAnd(expr1, expr2);

					return expr;
				});

			return result;
		}

		//internal static SimpleBodyRestriction TouchRestriction(JointType jointType, JointSide handSide)
		//{
		//    double maxY = TrigonometryHelper.GetSine(5);

		//    return new TouchBodyRestriction(jointType, handSide);
		//}

		internal static BoolExpr EvaluateNorms(Z3Body body1, Z3Body body2)
		{
			double normsThreshold = 0.1;

			BoolExpr result = Z3Math.True;
			var jointTypes = EnumUtil.GetValues<JointType>();
			foreach (var jointType in jointTypes)
			{
				// Calc the distance between the two norms
				ArithExpr distance = 
					Z3Math.Abs(
					Z3Math.Sub(
					body1.Norms[jointType], 
					body2.Norms[jointType]));

				// Create the boolean expression to evaluate the distance
				result = 
					Z3.Context.MkAnd(
					result, 
					Z3.Context.MkLt(
					distance, 
					Z3Math.Real(normsThreshold)));
			}

			return result;
		}
	}

	public static class BodyTransformBuilder {
		// Therapy transforms
		public static BodyTransform ArmsDownTransform()
		{
			return new BodyTransform()
				.Compose(JointType.ElbowLeft, new SetJointDirectionTransform(0, -1, 0))
				.Compose(JointType.WristLeft, new SetJointDirectionTransform(0, -1, 0))
				.Compose(JointType.ElbowRight, new SetJointDirectionTransform(0, -1, 0))
				.Compose(JointType.WristRight, new SetJointDirectionTransform(0, -1, 0));
		}

		public static BodyTransform CrossoverArmStretchTransform(JointSide sideOfStretchedArm)
		{
			JointSide oppositeSide;
			JointTransform stretchDirectionTransform;

			// First set the stretch direction and the opposite side (helper arm side)
			if (sideOfStretchedArm == JointSide.Left)
			{
				oppositeSide = JointSide.Right;
				stretchDirectionTransform = new SetJointDirectionTransform(1, 0, 0);
			}
			else
			{
				oppositeSide = JointSide.Left;
				stretchDirectionTransform = new SetJointDirectionTransform(-1, 0, 0);
			}

			// Get all joint types for both stretched and support arm
			JointType stretchedElbow = JointTypeHelper.GetSidedJointType(SidedJointName.Elbow, sideOfStretchedArm);
			JointType stretchedWrist = JointTypeHelper.GetSidedJointType(SidedJointName.Wrist, sideOfStretchedArm);

			JointType supportElbow = JointTypeHelper.GetSidedJointType(SidedJointName.Elbow, oppositeSide);
			JointType supportWrist = JointTypeHelper.GetSidedJointType(SidedJointName.Wrist, oppositeSide);


			// Apply the transforms in both arms
			return new BodyTransform()
				.Compose(stretchedElbow, stretchDirectionTransform)
				.Compose(stretchedWrist, stretchDirectionTransform)
				.Compose(supportElbow, new SetJointDirectionTransform(0, -1, 0))
				.Compose(supportWrist, new SetJointDirectionTransform(0, 1, 0));
		}

		public static BodyTransform PointToTransform(JointType jointType, Direction direction)
		{
			Z3Point3D point3D = Z3Point3D.DirectionPoint(direction);            

			return new BodyTransform(jointType, new SetJointDirectionTransform(point3D));
		}

		public static BodyTransform RotateTransform(JointType jointType, int angle, BodyPlaneType plane, RotationDirection direction)
		{
			return new BodyTransform(jointType, new RotateJointTransform(angle, plane, direction));
		}

		public static BodyTransform RotateTransform(JointType jointType, int angle, Direction direction)
		{
			return new BodyTransform(jointType, new RotateJointTransform(angle, direction));

			//BodyPlaneType plane = new BodyPlaneType();
			//RotationDirection rotationDir = new RotationDirection();

			//// TODO still have to evaluate which rotationDir brings the vector nearer to the intended direction
			//switch (direction)
			//{
			//	case (Direction.Front):
			//		plane = BodyPlaneType.Sagittal;
			//		rotationDir = RotationDirection.Clockwise;
			//		break;
			//	case (Direction.Back):
			//		plane = BodyPlaneType.Sagittal;
			//		rotationDir = RotationDirection.CounterClockwise;
			//		break;
			//	case (Direction.Right):
			//		plane = BodyPlaneType.Frontal;
			//		rotationDir = RotationDirection.Clockwise;
			//		break;
			//	case (Direction.Left):
			//		plane = BodyPlaneType.Frontal;
			//		rotationDir = RotationDirection.CounterClockwise;
			//		break;
			//	case (Direction.Up):
			//		plane = BodyPlaneType.Horizontal;
			//		rotationDir = RotationDirection.Clockwise;
			//		break;
			//	case (Direction.Down):
			//		plane = BodyPlaneType.Horizontal;
			//		rotationDir = RotationDirection.CounterClockwise;
			//		break;
			//	default:
			//		break;
			//}

			//return new BodyTransform(jointType, new RotateJointTransform(angle, plane, rotationDir));
		}
	}

	public class PoseBuilder
	{
		// Gestures as a List of Poses
		public static IEnumerable<Pose> CrossoverArmStretch(int numberOfRepetitions = 4)
		{
			var startTransform = BodyTransformBuilder.ArmsDownTransform();

			yield return new Pose("ArmsDownTransform", startTransform, BodyRestrictionBuilder.ShouldersRelaxedRestriction());

			var leftStretch = BodyTransformBuilder.CrossoverArmStretchTransform(JointSide.Left);
			var rightStretch = BodyTransformBuilder.CrossoverArmStretchTransform(JointSide.Right);

			for (int i = 0; i < numberOfRepetitions; ++i)
			{
				yield return (new Pose("leftStretch", leftStretch));
				yield return (new Pose("rightStretch", rightStretch));
			}
		}

		public static IEnumerable<Pose> ElbowFlexion(int numberOfRepetitions = 8)
		{
			// First create the start pose and set both arms to point down
			var startTransform = BodyTransformBuilder.ArmsDownTransform();

			// The create a new pose for the rise of the left arm
			BodyTransform leftUpTransform = new BodyTransform();
			leftUpTransform = startTransform.Compose(JointType.WristLeft, new SetJointDirectionTransform(0, 1, 0));

			// And another for the rise of the right arm
			BodyTransform rightUpTransform = new BodyTransform();
			rightUpTransform = startTransform.Compose(JointType.WristRight, new SetJointDirectionTransform(0, 1, 0));

			// Add the needed 
			yield return new Pose("", startTransform);
			for (int i = 0; i < numberOfRepetitions; ++i)
			{
				yield return (new Pose("", leftUpTransform));
				yield return (new Pose("", rightUpTransform));
			}
		}

		public static IEnumerable<Pose> PassiveExternalRotation(int numberOfRepetitions = 4)
		{
			// Create the left pose and set both arms to point down
			BodyTransform leftTransform = new BodyTransform();
			leftTransform = leftTransform.Compose(JointType.ElbowLeft, new SetJointDirectionTransform(0, -1, 0));
			leftTransform = leftTransform.Compose(JointType.WristLeft, new SetJointDirectionTransform(-0.7, 0, 0.7));
			leftTransform = leftTransform.Compose(JointType.ElbowRight, new SetJointDirectionTransform(0, -1, 0));
			leftTransform = leftTransform.Compose(JointType.WristRight, new SetJointDirectionTransform(0, 0, 1));
			Pose leftPose = new Pose("DontTwistHipRestriction-l", leftTransform, BodyRestrictionBuilder.DontTwistHipRestriction(5));

			// Create the left pose and set both arms to point down
			BodyTransform rightTransform = new BodyTransform();
			rightTransform = rightTransform.Compose(JointType.ElbowLeft, new SetJointDirectionTransform(0, -1, 0));
			rightTransform = rightTransform.Compose(JointType.WristLeft, new SetJointDirectionTransform(0, 0, 1));
			rightTransform = rightTransform.Compose(JointType.ElbowRight, new SetJointDirectionTransform(0, -1, 0));
			rightTransform = rightTransform.Compose(JointType.WristRight, new SetJointDirectionTransform(0.7, 0, 0.7));
			Pose rightPose = new Pose("DontTwistHipRestriction-r", rightTransform, BodyRestrictionBuilder.DontTwistHipRestriction(5));

			for (int i = 0; i < numberOfRepetitions; ++i)
			{
				yield return (leftPose);
				yield return (rightPose);
			}
		}

		public static IEnumerable<Pose> ShoulderFlexion(int numberOfRepetitions = 3)
		{
			BodyTransform leftUp = new BodyTransform();
			leftUp = leftUp.Compose(JointType.ElbowLeft, new SetJointDirectionTransform(0, 1, 0));
			leftUp = leftUp.Compose(JointType.WristLeft, new SetJointDirectionTransform(0, 1, 0));
			leftUp = leftUp.Compose(JointType.ElbowRight, new SetJointDirectionTransform(0, -1, 0));
			leftUp = leftUp.Compose(JointType.WristRight, new SetJointDirectionTransform(0, -1, 0));
			Pose leftPose = new Pose("leftUp", leftUp);

			BodyTransform rightUp = new BodyTransform();
			rightUp = rightUp.Compose(JointType.ElbowLeft, new SetJointDirectionTransform(0, -1, 0));
			rightUp = rightUp.Compose(JointType.WristLeft, new SetJointDirectionTransform(0, -1, 0));
			rightUp = rightUp.Compose(JointType.ElbowRight, new SetJointDirectionTransform(0, 1, 0));
			rightUp = rightUp.Compose(JointType.WristRight, new SetJointDirectionTransform(0, 1, 0));
			Pose rightPose = new Pose("rightUp", rightUp);

			for (int i = 0; i < numberOfRepetitions; ++i)
			{
				yield return (leftPose);
				yield return (rightPose);
			}
		}

		public static IEnumerable<Pose> ShoulderAbduction(int numberOfRepetitions = 3)
		{
			BodyTransform leftUp = new BodyTransform();
			leftUp = leftUp.Compose(JointType.ElbowLeft, new SetJointDirectionTransform(-1, 0, 0));
			leftUp = leftUp.Compose(JointType.WristLeft, new SetJointDirectionTransform(-1, 0, 0));
			leftUp = leftUp.Compose(JointType.ElbowRight, new SetJointDirectionTransform(0, -1, 0));
			leftUp = leftUp.Compose(JointType.WristRight, new SetJointDirectionTransform(0, -1, 0));
			Pose leftPose = new Pose("leftUp", leftUp);

			BodyTransform rightUp = new BodyTransform();
			rightUp = rightUp.Compose(JointType.ElbowLeft, new SetJointDirectionTransform(0, -1, 0));
			rightUp = rightUp.Compose(JointType.WristLeft, new SetJointDirectionTransform(0, -1, 0));
			rightUp = rightUp.Compose(JointType.ElbowRight, new SetJointDirectionTransform(1, 0, 0));
			rightUp = rightUp.Compose(JointType.WristRight, new SetJointDirectionTransform(1, 0, 0));
			Pose rightPose = new Pose("rightUp", rightUp);

			for (int i = 0; i < numberOfRepetitions; ++i)
			{
				yield return (leftPose);
				yield return (rightPose);
			}
		}
	}    
}
