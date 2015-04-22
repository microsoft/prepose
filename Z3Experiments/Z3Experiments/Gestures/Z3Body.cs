using Microsoft.Z3;
using Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace PreposeGestures
{
	public enum JointSide
	{
		Left = 0,
		Right = 1,
		Center = 3,
	}

	public enum SidedJointName
	{
		Shoulder = 0,
		Elbow = 1,
		Wrist = 2,
		Hand = 3,
		Hip = 4,
		Knee = 5,
		Ankle = 6,
		Foot = 7,
		HandTip = 8,
		Thumb = 9,
	}

	/// <summary>
	/// This enum is parallel to the Kinect enum found in 
	/// Microsoft.Kinect.dll.
	/// </summary>
	public enum JointType
	{
		SpineBase = 0,
		SpineMid = 1,
		Neck = 2,
		Head = 3,
		ShoulderLeft = 4,
		ElbowLeft = 5,
		WristLeft = 6,
		HandLeft = 7,
		ShoulderRight = 8,
		ElbowRight = 9,
		WristRight = 10,
		HandRight = 11,
		HipLeft = 12,
		KneeLeft = 13,
		AnkleLeft = 14,
		FootLeft = 15,
		HipRight = 16,
		KneeRight = 17,
		AnkleRight = 18,
		FootRight = 19,
		SpineShoulder = 20,
		HandTipLeft = 21,
		ThumbLeft = 22,
		HandTipRight = 23,
		ThumbRight = 24,
	}

	public static class Z3
	{
		public static Context Context = new Context(new Dictionary<string, string>() { { "MODEL", "true" }, /*{ "VERBOSE", "100" },*/ { "PROOF_MODE", "2" } });

        public static bool EvaluateBoolExpr(BoolExpr expr)
        {
            return EvaluateBoolExpr(expr, Z3.Context);
        }
		public static bool EvaluateBoolExpr(BoolExpr expr, Context localContext)
		{
			Solver solver = localContext.MkSolver();

            solver.Assert(localContext.MkNot(expr));
			Status status = solver.Check();
            Statistics stats = solver.Statistics; 

			bool result = false;
			if (status == Status.UNSATISFIABLE)
				result = true;

			return result;
		}
	}


	public static class Z3Math
	{
		// Arithmetic helpers
		public static ArithExpr Abs(ArithExpr expr)
		{
			Expr result = Z3.Context.MkITE(
				Z3.Context.MkGe(expr, Z3Math.Zero),
				expr,
				Z3Math.Neg(expr));

			return result as ArithExpr;
		}

		public static ArithExpr Min(ArithExpr expr1, ArithExpr expr2)
		{
			Expr result = Z3.Context.MkITE(
				Z3.Context.MkLt(expr1, expr2),
				expr1,
				expr2);

			return result as ArithExpr;
		}

		public static ArithExpr Min(ArithExpr expr1, ArithExpr expr2, ArithExpr expr3)
		{
			return Z3Math.Min(Z3Math.Min(expr1, expr2), expr3);
		}

		public static ArithExpr Max(ArithExpr expr1, ArithExpr expr2)
		{
			Expr result = Z3.Context.MkITE(
				Z3.Context.MkGe(expr1, expr2),
				expr1,
				expr2);

			return result as ArithExpr;
		}

		public static ArithExpr Max(ArithExpr expr1, ArithExpr expr2, ArithExpr expr3)
		{
			return Z3Math.Max(Z3Math.Max(expr1, expr2), expr3);
		}

		public static ArithExpr Add(params ArithExpr[] exprs)
		{
			ArithExpr result = Z3.Context.MkAdd(exprs);
			return result as ArithExpr;
		}

        public static ArithExpr Sub(params ArithExpr[] exprs)
        {
            return Sub(Z3.Context, exprs);
        }
		public static ArithExpr Sub(Context localContext, params ArithExpr[] exprs)
		{
			ArithExpr result = Z3.Context.MkSub(exprs);
			return result as ArithExpr;
		}

		public static ArithExpr Mul(params ArithExpr[] exprs)
		{
			ArithExpr result = Z3.Context.MkMul(exprs);
			return result as ArithExpr;
		}

		public static ArithExpr Div(ArithExpr expr1, ArithExpr expr2)
		{
			ArithExpr result = Z3.Context.MkDiv(expr1, expr2);
			return result as ArithExpr;
		}

		// Negates a number: returns = -expr
		public static ArithExpr Neg(ArithExpr expr)
		{
			ArithExpr result = Z3Math.Sub(Zero, expr);
			return result as ArithExpr;
		}

        public static ArithExpr Real(double number)
        {
            return Real(number, Z3.Context);
        }
		public static ArithExpr Real(double number, Context localZ3Context)
		{
			ArithExpr result = Rational.MkRational(number, localZ3Context);
			return result;
		}

        public static double GetRealValue(ArithExpr expr)
        {
            return GetRealValue(expr, Z3.Context);
        }

		public static double GetRealValue(ArithExpr expr, Context localContext)
		{
			RatNum xRatNum = expr.Simplify() as RatNum;
			IntNum d = xRatNum.Denominator;
			IntNum n = xRatNum.Numerator;
			BigRational bigRational = new BigRational(n.BigInteger, d.BigInteger);
			return (double)bigRational;
		}

		public static ArithExpr Zero = Z3Math.Real(0);
		public static ArithExpr One = Z3Math.Real(1);
		public static ArithExpr OneNeg = Z3Math.Real(-1);

		// Boolean algebra helpers
		public static BoolExpr True = Z3.Context.MkTrue();
		public static BoolExpr False = Z3.Context.MkFalse();
	}
	
	public class JointGroup
	{
		public JointGroup(IEnumerable<JointType> joints)
		{
			this.Joints = joints;
		}

		public JointGroup(JointType joints)
		{
			this.Joints = new JointType[] { joints };
		}

		public IEnumerable<JointType> Joints { get; private set; }

		public CompositeBodyRestriction Aggregate(Func<JointType, IBodyRestriction> func)
		{
			var result = new CompositeBodyRestriction();
			foreach (var joint in this.Joints)
			{
				var that = func(joint);
				result = result.And(that);
			}

			return result;
		}

		public BodyTransform Aggregate(Func<JointType, BodyTransform> func)
		{
			var result = new BodyTransform();
			foreach (var joint in this.Joints)
			{
				var that = func(joint);
				result = result.Compose(that);
			}

			return result;
		}

		public JointType Only()
		{
			Contract.Requires(this.Joints.Count() == 1);
			return this.Joints.First();
		}
	}

	public static class EnumUtil
	{
		public static IEnumerable<T> GetValues<T>()
		{
			return Enum.GetValues(typeof(T)).Cast<T>();
		}
	}

	public static class Rational
	{
		// Constructor that takes double and make rational number of it
		// The first nine digits will be saved, the rest of the double value is lost
		public static RatNum MkRational(double value, Context localZ3Context)
		{
			if (value >= int.MaxValue || value < int.MinValue)
				throw new ArgumentException("value is greater than int.MaxValue or less than int.MinValue and cannot be parsed to a Rational");

			int magnitude = (int)Math.Ceiling(Math.Log10(Math.Abs(value)));
			int denPower = Math.Min(Math.Max(9 - magnitude, 0), 9);

			int denominator = (int)(Math.Pow(10, denPower));
			int numerator = (int)(value * denominator);

            return localZ3Context.MkReal(numerator, denominator);
		}
	}

    public class Point3D
    {
        public Point3D() : this (0, 0, 0)
		{
		}

        public Point3D(double x, double y, double z)
		{
            this.X = x;
            this.Y = y;
            this.Z = z;
		}

        public double Norm()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public override string ToString()
        {
            return string.Format("X: {0}, Y: {1}, Z: {2}",
                this.X,
                this.Y,
                this.Z);
        }
    }

	public class Z3Point3D
	{
		public Z3Point3D() : this (0, 0, 0)
		{
		}

		public Z3Point3D(ArithExpr x, ArithExpr y, ArithExpr z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public Z3Point3D(double x, double y, double z)
		{
			this.X = Z3Math.Real(x);
			this.Y = Z3Math.Real(y);
			this.Z = Z3Math.Real(z);
        }

        public Z3Point3D(double x, double y, double z, Context localZ3Context)
        {
            this.X = Z3Math.Real(x,localZ3Context);
            this.Y = Z3Math.Real(y,localZ3Context);
            this.Z = Z3Math.Real(z,localZ3Context);
        }



		public static Z3Point3D MkZ3Const(string prefix)
		{
			Z3Point3D result = new Z3Point3D();
			result.X = Z3.Context.MkRealConst(prefix + " X");
			result.Y = Z3.Context.MkRealConst(prefix + " Y");
			result.Z = Z3.Context.MkRealConst(prefix + " Z");

			return result;
		}

		public static Z3Point3D Up()
		{
			return new Z3Point3D(0, 1, 0);
		}

		public static Z3Point3D Down()
		{
			return new Z3Point3D(0, -1, 0);
		}

		public static Z3Point3D Left()
		{
			return new Z3Point3D(-1, 0, 0);
		}

		public static Z3Point3D Right()
		{
			return new Z3Point3D(1, 0, 0);
		}

		public static Z3Point3D Front()
		{
			return new Z3Point3D(0, 0, 1);
		}

		public static Z3Point3D Back()
		{
			return new Z3Point3D(0, 0, -1);
		}

		public static Z3Point3D DirectionPoint(Direction direction)
		{
			Z3Point3D point3D = new Z3Point3D();

			switch (direction)
			{
				case Direction.Up:
					point3D = Z3Point3D.Up();
					break;

				case Direction.Down:
					point3D = Z3Point3D.Down();
					break;

				case Direction.Left:
					point3D = Z3Point3D.Left();
					break;

				case Direction.Right:
					point3D = Z3Point3D.Right();
					break;

				case Direction.Front:
					point3D = Z3Point3D.Front();
					break;

				case Direction.Back:
					point3D = Z3Point3D.Back();
					break;
			}

			return point3D;
		}

		ArithExpr CalcApproximateCoordFromManhattanToEuclidianSystem(
			ArithExpr firstCoord,
			ArithExpr secondCoord,
			ArithExpr thirdCoord)
		{
			// Work only with values length
			// Values sign will be assigned again in the end
			ArithExpr firstCoordLength = Z3Math.Abs(firstCoord);
			ArithExpr secondCoordLength = Z3Math.Abs(secondCoord);
			ArithExpr thirdCoordLength = Z3Math.Abs(thirdCoord);

			// The all common length will be weighted by this
			// This way for example a (1, 1, 1) vector will become
			// A (0.57, 0.57, 0.57) with norm near to 1
			//ArithExpr sqrt1div3 = Z3Math.Real(0.57735026918962576450914878050196);
            ArithExpr sqrt1div3 = Z3Math.Real(0.577);

			// The remaining common length will be weighted by this
			// This way for example a (1, 1, 0) vector will become
			// A (0.7, 0.7, 0.7) with norm near to 1
			//ArithExpr sin45 = Z3Math.Real(0.70710678118654752440084436210485)
			ArithExpr sin45 = Z3Math.Real(0.707);

			// Calc common length between x, y, z
			ArithExpr allCommonLength =
				Z3Math.Min(
					firstCoordLength,
					secondCoordLength,
					thirdCoordLength);

			// Calc the common length between the target coord (firstCoord)
			// and the higher coord between the second and third coords
			ArithExpr lastTwoCommonLength =
				Z3Math.Max(
				Z3Math.Min(secondCoordLength, firstCoordLength),
				Z3Math.Min(thirdCoordLength, firstCoordLength));

			// Calc exclusevely common length with the remaining coordinate  
			ArithExpr lastTwoExclusiveCommonLength =
				Z3Math.Sub(
					lastTwoCommonLength,
					allCommonLength);

			// Calc remaining length
			ArithExpr especificLength =
				Z3Math.Sub(firstCoordLength,
				Z3Math.Add(lastTwoExclusiveCommonLength, allCommonLength));

			// Calc weighted lengths
			ArithExpr weigthedLength1 = Z3Math.Mul(lastTwoExclusiveCommonLength, sin45);
			ArithExpr weigthedLength2 = Z3Math.Mul(allCommonLength, sqrt1div3);

			// Calc weighted result length
			ArithExpr resultLength =
				Z3Math.Add(
				especificLength,
				weigthedLength1,
				weigthedLength2);

			// The transform doesn't change the sign of the coordinate
			// Recover it from original data
			Expr result =
				Z3.Context.MkITE(
				Z3.Context.MkGe(firstCoord, Z3Math.Zero),
				resultLength,
				Z3Math.Neg(resultLength));

			return result as ArithExpr;
		}

		public Z3Point3D GetManhattanNormalized()
		{
			Z3Point3D result = new Z3Point3D();

			ArithExpr higherCoord = 
				Z3Math.Max(
				Z3Math.Max(
				Z3Math.Abs(this.X), 
				Z3Math.Abs(this.Y)),
				Z3Math.Abs(this.Z));

			result.X = Z3Math.Div(this.X, higherCoord);
			result.Y = Z3Math.Div(this.Y, higherCoord);
			result.Z = Z3Math.Div(this.Z, higherCoord);

			return result;
		}

		//public Z3Point3D GetApproximateNormalized()
		//{
		//	Z3Point3D result = this.GetManhattanNormalized();

		//	result.X = CalcApproximateCoordFromManhattanToEuclidianSystem(result.X, result.Y, result.Z);
		//	result.Y = CalcApproximateCoordFromManhattanToEuclidianSystem(result.Y, result.X, result.Z);
		//	result.Z = CalcApproximateCoordFromManhattanToEuclidianSystem(result.Z, result.Y, result.X);

		//	return result;
		//}

		public ArithExpr Norm()
		{
			ArithExpr result =
				Z3Math.Max(
				Z3Math.Abs(CalcApproximateCoordFromManhattanToEuclidianSystem(this.X, this.Y, this.Z)),
				Z3Math.Abs(CalcApproximateCoordFromManhattanToEuclidianSystem(this.Y, this.X, this.Z)),
				Z3Math.Abs(CalcApproximateCoordFromManhattanToEuclidianSystem(this.Z, this.Y, this.X)));

			return result;
		}

		public ArithExpr CalcApproximateDistance(Z3Point3D that)
		{
			// Manhattan distance vector
            Z3Point3D distVec = this.GrabDistancePoint3D(that);

            //ArithExpr result =
            //    Z3Math.Add(
            //    Z3Math.Abs(CalcApproximateCoordFromManhattanToEuclidianSystem(distVec.X, distVec.Y, distVec.Z)),
            //    Z3Math.Abs(CalcApproximateCoordFromManhattanToEuclidianSystem(distVec.Y, distVec.X, distVec.Z)),
            //    Z3Math.Abs(CalcApproximateCoordFromManhattanToEuclidianSystem(distVec.Z, distVec.Y, distVec.X)));

            ArithExpr result =
                Z3Math.Max(
                Z3Math.Abs(distVec.X),
                Z3Math.Abs(distVec.Y),
                Z3Math.Abs(distVec.Z));

			return result;
		}

        public Z3Point3D GrabDistancePoint3D(Z3Point3D that)
        {
            // Manhattan distance vector
            Z3Point3D result = this - that;
            return result;
        }

		//public ArithExpr CalcApproximateDistanceBetweenNormalized(Z3Point3D that)
		//{
		//	Z3Point3D thisNormalized = this.GetApproximateNormalized();
		//	Z3Point3D thatNormalized = that.GetApproximateNormalized();

		//	ArithExpr result = thisNormalized.CalcApproximateDistance(thatNormalized);
		//	return result;
		//}
		
		// Assumes vectors are normalized
		public BoolExpr IsAngleBetweenLessThan(Z3Point3D that, int angleThreshold)
		{   
			double distanceThreshold = TrigonometryHelper.GetDistance(angleThreshold);
			ArithExpr distance = this.CalcApproximateDistance(that);

			BoolExpr result = Z3.Context.MkLt(distance, Z3Math.Real(distanceThreshold));

            // TODO remove this, test code
            SolverCheckResult checkResult = Z3AnalysisInterface.CheckStatus(result);
            if (checkResult.Status == Status.SATISFIABLE)
            {
                var joint = new Z3Point3D(
                        checkResult.Model.Evaluate(this.X, true) as ArithExpr,
                        checkResult.Model.Evaluate(this.Y, true) as ArithExpr,
                        checkResult.Model.Evaluate(this.Z, true) as ArithExpr);

                var distanceSolvedExpr = checkResult.Model.Evaluate(distance, true) as ArithExpr;
                var distanceValue = Z3Math.GetRealValue(distanceSolvedExpr);
            }
            // end of test code

			return result;
		}

		public BoolExpr IsAngleBetweenGreaterThan(Z3Point3D that, int angleThreshold)
		{
			BoolExpr result = Z3.Context.MkNot(this.IsAngleBetweenLessThan(that, angleThreshold));
			return result;
		}

		public BoolExpr IsNearerThan(Z3Point3D that, double distanceThreshold)
		{
			ArithExpr distance = this.CalcApproximateDistance(that);
			BoolExpr result = Z3.Context.MkLt(distance, Z3Math.Real(distanceThreshold));
			return result;
		}

		public BoolExpr IsFartherThan(Z3Point3D that, double distanceThreshold)
		{
			BoolExpr result = Z3.Context.MkNot(this.IsNearerThan(that, distanceThreshold));
			return result;
		}
		
		public static Z3Point3D operator +(Z3Point3D p3d, Z3Point3D that)
		{
			return new Z3Point3D(
					Z3Math.Add(p3d.X, that.X),
					Z3Math.Add(p3d.Y, that.Y),
					Z3Math.Add(p3d.Z, that.Z));
		}

		public static Z3Point3D operator -(Z3Point3D p3d, Z3Point3D that)
		{
			return new Z3Point3D(
					Z3Math.Sub(p3d.X, that.X),
					Z3Math.Sub(p3d.Y, that.Y),
					Z3Math.Sub(p3d.Z, that.Z));
		}

		public static Z3Point3D operator *(Z3Point3D p3d, double value)
		{
			return new Z3Point3D(
					Z3Math.Mul(p3d.X, Z3Math.Real(value)),
					Z3Math.Mul(p3d.Y, Z3Math.Real(value)),
					Z3Math.Mul(p3d.Z, Z3Math.Real(value)));
		}

		public static Z3Point3D operator *(Z3Point3D p3d, ArithExpr value)
		{
			return new Z3Point3D(
					Z3Math.Mul(p3d.X, value),
					Z3Math.Mul(p3d.Y, value),
					Z3Math.Mul(p3d.Z, value));
		}

		public static Z3Point3D operator /(Z3Point3D p3d, double value)
		{
			return new Z3Point3D(
					Z3Math.Div(p3d.X, Z3Math.Real(value)),
					Z3Math.Div(p3d.Y, Z3Math.Real(value)),
					Z3Math.Div(p3d.Z, Z3Math.Real(value)));
		}

		public double GetXValue()
		{
			return Z3Math.GetRealValue(X);
		}

		public double GetYValue()
		{
			return Z3Math.GetRealValue(Y);
		}

		public double GetZValue()
		{
			return Z3Math.GetRealValue(Z);
		}

		public Z3Point3D GetInverted()
		{
			ArithExpr invertedX = Z3Math.Neg(this.X);
			ArithExpr invertedY = Z3Math.Neg(this.Y);
			ArithExpr invertedZ = Z3Math.Neg(this.Z);

			return new Z3Point3D(invertedX, invertedY, invertedZ);
		}

		public ArithExpr X { get; set; }

		public ArithExpr Y { get; set; }

		public ArithExpr Z { get; set; }

		public override string ToString()
		{
			return string.Format("X: {0}, Y: {1}, Z: {2}",
				this.GetXValue(),
				this.GetYValue(),
				this.GetZValue());
		}

		internal BoolExpr IsEqualTo(Z3Point3D that)
		{
			var result = Z3.Context.MkAnd(
				Z3.Context.MkEq(this.X, that.X),
				Z3.Context.MkEq(this.Y, that.Y),
				Z3.Context.MkEq(this.Z, that.Z));

			return result;
		}
	}

	public class Z3Body
	{
		public Z3Body()
		{
			this.Joints = new Dictionary<JointType, Z3Point3D>();
			this.Norms = new Dictionary<JointType, ArithExpr>();
		}

		public Z3Body(Dictionary<JointType, Z3Point3D> joints, Dictionary<JointType, ArithExpr> norms)
		{
			this.Joints = joints;
			this.Norms = norms;
		}

        public Z3Body(Z3Body baseBody)
        {
            foreach (var jointType in baseBody.Joints.Keys)
            {
                this.Joints.Add(jointType,
                    new Z3Point3D(
                        Z3Math.GetRealValue(baseBody.Joints[jointType].X),
                        Z3Math.GetRealValue(baseBody.Joints[jointType].Y),
                        Z3Math.GetRealValue(baseBody.Joints[jointType].Z)
                    ));

                this.Norms.Add(jointType, Z3Math.Real(Z3Math.GetRealValue(baseBody.Norms[jointType])));
            }
        }

		public override string ToString()
		{
			var buf = new StringBuilder("Body:");

			foreach (var joint in Joints.OrderBy(k => k.Key))
			{
				buf.AppendFormat("\n\t{0}: {1}", joint.Key, joint.Value);
			}

			return buf.ToString();
		}

		public static Z3Body MkZ3Const()
		{
			Z3Body result = new Z3Body();

			var jointTypes = EnumUtil.GetValues<JointType>();
			foreach (var jointType in jointTypes)
			{
				result.Joints.Add(jointType, Z3Point3D.MkZ3Const(jointType.ToString()));
				result.Norms.Add(jointType, Z3.Context.MkRealConst(jointType.ToString() + " Norm"));
			}

			return result;
		}

		public BoolExpr IsNearerThan(Z3Body that, double distanceThreshold)
		{
			BoolExpr result = Z3.Context.MkTrue();

			var jointTypes = EnumUtil.GetValues<JointType>();
			foreach (var jointType in jointTypes)
			{
				result = Z3.Context.MkAnd(this.Joints[jointType].IsNearerThan(that.Joints[jointType], distanceThreshold));
			}

			return result;
		}

		// Calculates average joint distance to target body considering only the active joints
		public Dictionary<JointType, ArithExpr> CalcDistanceExprs(Z3Body that, List<JointType> joints)
		{
            var result = new Dictionary<JointType, ArithExpr>();// Z3Math.Zero;

			foreach (var jointType in joints)
			{                
				// Only add distance if the target (that) body has the joint active
				// This allows partial matching, evaluating only the needed joints for the pose
				//result = Z3Math.Add(result, this.Joints[jointType].CalcApproximateDistance(that.Joints[jointType]));
                //result = Z3Math.Max(result, this.Joints[jointType].CalcApproximateDistance(that.Joints[jointType]));
                result.Add(jointType, this.Joints[jointType].CalcApproximateDistance(that.Joints[jointType]));
			}
			//result = Z3Math.Div(result, Z3Math.Real(joints.Count));

			return result;
		}

        public Dictionary<JointType, Z3Point3D> GrabDistancePoint3Ds(Z3Body that, List<JointType> joints)
        {
            var result = new Dictionary<JointType, Z3Point3D>();// Z3Math.Zero;

            foreach (var jointType in joints)
            {
                result.Add(jointType, this.Joints[jointType].GrabDistancePoint3D(that.Joints[jointType]));
            }

            return result;
        }

        //public Dictionary<JointType, ArithExpr> CalcDistanceExprs(Z3Target target)
        //{
        //    return CalcDistanceExprs(target.Body, target.TransformedJoints);
        //}

		public BoolExpr IsAngleBetweenLessThan(Z3Body that, List<JointType> joints, int angleThreshold)
		{
			BoolExpr result = Z3Math.True;

			foreach (var jointType in joints)
			{
				// Only add distance if the target (that) body has the joint active
				// This allows partial matching, evaluating only the needed joints for the pose
				result = 
					Z3.Context.MkAnd(
					result, 
					this.Joints[jointType].IsAngleBetweenLessThan(that.Joints[jointType], angleThreshold));
			}

			return result;
		}

		public BoolExpr IsEqualsTo(Z3Body that)
		{
			BoolExpr result = Z3Math.True;
			var allJoints = EnumUtil.GetValues<JointType>();

			foreach (var jointType in allJoints)
			{
				// Only add distance if the target (that) body has the joint active
				// This allows partial matching, evaluating only the needed joints for the pose
				result =
					Z3.Context.MkAnd(
					result,
					this.Joints[jointType].IsEqualTo(that.Joints[jointType]));
			}

			return result;
		}

        //public Dictionary<JointType, double> GrabDistances(Z3Body that, List<JointType> joints)
        //{
        //    var distanceExprs = this.GrabDistancePoint3Ds(that, joints);
        //    var result = new Dictionary<JointType, double>();
        //    foreach (var jointType in joints)
        //        result.Add(jointType, Z3Math.GetRealValue(distanceExprs[jointType]));

        //    return result;
        //}

        //public List<JointType> GetList

		public Z3Point3D GetJointPosition(JointType jointType)
		{
			Z3Point3D result = new Z3Point3D(0, 0, 0);

			JointType currentJointType = jointType;

			// SpineBase is the root of the coordinate system
			while (currentJointType != JointType.SpineBase)
			{
				result = result + (this.Joints[currentJointType] * this.Norms[currentJointType]);
				currentJointType = JointTypeHelper.GetFather(currentJointType);
			}

            result = result + (this.Joints[jointType] * this.Norms[jointType]);

			return result;
		}
		
		public Dictionary<JointType, Z3Point3D> Joints { get; set; }
		public Dictionary<JointType, ArithExpr> Norms { get; set; }
	}
}