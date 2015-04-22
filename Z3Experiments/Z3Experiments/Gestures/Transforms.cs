using Microsoft.Z3;
using System;
using System.Collections.Generic;
using System.Text;

namespace PreposeGestures
{
    public enum BodyPlaneType
    {
        Frontal = 0,
        Sagittal = 1,
        Horizontal = 2,
    }

    public enum RelativeDirection
    {
        InFrontOfYour,
        BehindYour,
        OnTopOfYour,
        BelowYour,
        ToTheLeftOfYour,
        ToTheRightOfYour,
    }

    public enum Direction
    {
        Up,
        Down,
        Front,
        Back,
        Left,
        Right,
    }

    public enum RotationDirection
    {
        Clockwise = 0,
        CounterClockwise = 1,
    }

    public class JointTransform
    {
        internal JointTransform(Func<Z3Point3D, Z3Point3D> transform)
        {
            this.TransformFunc = transform;
        }

        public override bool Equals(object obj)
        {
            var that = obj as JointTransform;
            Solver solver = Z3.Context.MkSolver();

            solver.Push();

            RealExpr x = Z3.Context.MkRealConst("x");
            RealExpr y = Z3.Context.MkRealConst("y");
            RealExpr z = Z3.Context.MkRealConst("z");

            Z3Point3D joint = new Z3Point3D(x, y, z);
            Z3Point3D thisJoint = this.TransformFunc(joint);
            Z3Point3D thatJoint = that.TransformFunc(joint);

            BoolExpr xEq = Z3.Context.MkEq(thisJoint.X, thatJoint.X);
            BoolExpr yEq = Z3.Context.MkEq(thisJoint.Y, thatJoint.Y);
            BoolExpr zEq = Z3.Context.MkEq(thisJoint.Z, thatJoint.Z);

            BoolExpr equalsExpr = Z3.Context.MkAnd(xEq, yEq, zEq);

            solver.Assert(Z3.Context.MkNot(equalsExpr));
            Status status = solver.Check();
            Statistics stats = solver.Statistics; 

            //Console.WriteLine("EqualsExpr: " + equalsExpr);
            //Console.WriteLine("Proving: " + equalsExpr);
            //switch (status)
            //{
            //    case Status.UNKNOWN:
            //        Console.WriteLine("Unknown because:\n" + solver.ReasonUnknown);
            //        break;
            //    case Status.SATISFIABLE:
            //        throw new ArgumentException("Test Failed Expception");
            //    case Status.UNSATISFIABLE:
            //        Console.WriteLine("OK, proof:\n" + solver.Proof);
            //        break;
            //}

            bool result = false;
            if (status == Status.UNSATISFIABLE)
                result = true;

            solver.Pop();

            return result;
        }

        public JointTransform Compose(JointTransform that)
        {
            Func<Z3Point3D, Z3Point3D> composed =
                joint => that.TransformFunc(this.TransformFunc(joint));

            return new JointTransform(composed);
        }

        public Z3Point3D Transform(Z3Point3D joint)
        {
            Z3Point3D result = TransformFunc(joint);
            return result;
        }

        public override string ToString()
        {
            return this.TransformFunc.Target.GetType().DeclaringType.ToString();
        }

        private Func<Z3Point3D, Z3Point3D> TransformFunc { get; set; }
    }

    public class RemainStillJointTransform : JointTransform
    {
        public RemainStillJointTransform()
            : base(joint => joint)
        { }
    }

    public class SetJointDirectionTransform : JointTransform
    {
        public SetJointDirectionTransform(double x, double y, double z)
            : base(joint => new Z3Point3D(Z3Math.Real(x), Z3Math.Real(y), Z3Math.Real(z)))
        { }

        public SetJointDirectionTransform(Z3Point3D jointToSet)
            : base(joint => jointToSet)
        { }
    }

    public class RotateJointTransform : JointTransform
    {
        public RotateJointTransform(int angle, BodyPlaneType plane, RotationDirection rotationDirection = RotationDirection.Clockwise)
            : base(
            joint =>
            {
                double cosInput = TrigonometryHelper.GetCosine(angle);
                double sinInput = TrigonometryHelper.GetSine(angle);

                if (rotationDirection == RotationDirection.CounterClockwise)
                    sinInput = -sinInput;

                var cos = Z3Math.Real(cosInput);
                var sin = Z3Math.Real(sinInput);
                var sinNeg = Z3Math.Real(-sinInput);

                Z3Point3D result = new Z3Point3D(joint.X, joint.Y, joint.Z);

                switch (plane)
                {
                    case BodyPlaneType.Frontal:
                        result.Y = Z3Math.Add(Z3Math.Mul(cos, joint.Y), Z3Math.Mul(sin, joint.Z));
                        result.Z = Z3Math.Add(Z3Math.Mul(sinNeg, joint.Y), Z3Math.Mul(cos, joint.Z));
                        break;

                    case BodyPlaneType.Sagittal:
                        result.X = Z3Math.Add(Z3Math.Mul(cos, joint.X), Z3Math.Mul(sin, joint.Y));
                        result.Y = Z3Math.Add(Z3Math.Mul(sinNeg, joint.X), Z3Math.Mul(cos, joint.Y));
                        break;

                    case BodyPlaneType.Horizontal:
                        result.X = Z3Math.Add(Z3Math.Mul(cos, joint.X), Z3Math.Mul(sin, joint.Z));
                        result.Z = Z3Math.Add(Z3Math.Mul(sinNeg, joint.X), Z3Math.Mul(cos, joint.Z));
                        break;

                    default:
                        break;
                }

                return result;
            })
        { }

        public RotateJointTransform(int angle, Direction direction)
            : base(
            joint =>
            {
                double cosInput = TrigonometryHelper.GetCosine(angle);
                double sinInput = TrigonometryHelper.GetSine(angle);

                var cos = Z3Math.Real(cosInput);
                var sin = Z3Math.Real(sinInput);
                var sinNeg = Z3Math.Real(-sinInput);

                Z3Point3D result = new Z3Point3D(joint.X, joint.Y, joint.Z);

                // The performed rotation depends on current values of X, Y and Z
                // The rotation plane and direction changes depending on the relation between the coordinates
                switch (direction)
                {
                    case Direction.Back:
                        result.X =
                            // if Abs(X) >= Abs(Y)
                            // if X > 0
                            // rotate counter clockwise
                            // else rotate clockwise
                            // else return X
                            Z3.Context.MkITE(Z3.Context.MkGe(Z3Math.Abs(result.X), Z3Math.Abs(result.Y)),
                            Z3.Context.MkITE(Z3.Context.MkGe(result.X, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(cos, joint.X), Z3Math.Mul(sin, joint.Z)),
                            Z3Math.Add(Z3Math.Mul(cos, joint.X), Z3Math.Mul(sinNeg, joint.Z))),
                            joint.X) as ArithExpr;

                        result.Y =
                            // if Abs(X) >= Abs(Y) 
                            // then return Y
                            // else rotate Y
                            // if Y > 0
                            // rotate counter clockwise
                            // else rotate clockwise
                            Z3.Context.MkITE(Z3.Context.MkGe(Z3Math.Abs(joint.X), Z3Math.Abs(joint.Y)),
                            joint.Y,
                            Z3.Context.MkITE(Z3.Context.MkGe(joint.Y, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(cos, joint.Y), Z3Math.Mul(sin, joint.Z)),
                            Z3Math.Add(Z3Math.Mul(cos, joint.Y), Z3Math.Mul(sinNeg, joint.Z)))) as ArithExpr;

                        result.Z =
                            // if Abs(X) >= Abs(Y)
                            // if X > 0
                            // rotate counter clockwise
                            // else rotate clockwise
                            // else if Y > 0
                            // rotate counter clockwise
                            // else rotate clockwise
                            Z3.Context.MkITE(Z3.Context.MkGe(Z3Math.Abs(joint.X), Z3Math.Abs(joint.Y)),
                            Z3.Context.MkITE(Z3.Context.MkGe(joint.X, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(sinNeg, joint.X), Z3Math.Mul(cos, joint.Z)),
                            Z3Math.Add(Z3Math.Mul(sin, joint.X), Z3Math.Mul(cos, joint.Z))),
                            Z3.Context.MkITE(Z3.Context.MkGe(joint.Y, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(sinNeg, joint.Y), Z3Math.Mul(cos, joint.Z)),
                            Z3Math.Add(Z3Math.Mul(sin, joint.Y), Z3Math.Mul(cos, joint.Z)))) as ArithExpr;
                        break;

                    case Direction.Front:
                        result.X =
                            // if Abs(X) >= Abs(Y)
                            // if X > 0
                            // rotate clockwise
                            // else rotate counter clockwise
                            // else return X
                            Z3.Context.MkITE(Z3.Context.MkGe(Z3Math.Abs(result.X), Z3Math.Abs(result.Y)),
                            Z3.Context.MkITE(Z3.Context.MkGe(result.X, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(cos, joint.X), Z3Math.Mul(sinNeg, joint.Z)),
                            Z3Math.Add(Z3Math.Mul(cos, joint.X), Z3Math.Mul(sin, joint.Z))),
                            joint.X) as ArithExpr;

                        result.Y =
                            // if Abs(X) >= Abs(Y) 
                            // then return Y
                            // else rotate Y
                            // if Y > 0
                            // rotate clockwise
                            // else rotate counter clockwise
                            Z3.Context.MkITE(Z3.Context.MkGe(Z3Math.Abs(joint.X), Z3Math.Abs(joint.Y)),
                            joint.Y,
                            Z3.Context.MkITE(Z3.Context.MkGe(joint.Y, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(cos, joint.Y), Z3Math.Mul(sinNeg, joint.Z)),
                            Z3Math.Add(Z3Math.Mul(cos, joint.Y), Z3Math.Mul(sin, joint.Z)))) as ArithExpr;

                        result.Z =
                            // if Abs(X) >= Abs(Y)
                            // if X > 0
                            // rotate clockwise
                            // else rotate counter clockwise
                            // else if Y > 0
                            // rotate clockwise
                            // else rotate counter clockwise
                            Z3.Context.MkITE(Z3.Context.MkGe(Z3Math.Abs(joint.X), Z3Math.Abs(joint.Y)),
                            Z3.Context.MkITE(Z3.Context.MkGe(joint.X, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(sin, joint.X), Z3Math.Mul(cos, joint.Z)),
                            Z3Math.Add(Z3Math.Mul(sinNeg, joint.X), Z3Math.Mul(cos, joint.Z))),
                            Z3.Context.MkITE(Z3.Context.MkGe(joint.Y, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(sin, joint.Y), Z3Math.Mul(cos, joint.Z)),
                            Z3Math.Add(Z3Math.Mul(sinNeg, joint.Y), Z3Math.Mul(cos, joint.Z)))) as ArithExpr;
                        break;

                    case Direction.Down:
                        result.X =
                            // if Abs(X) >= Abs(Z)
                            // if X > 0
                            // rotate counter clockwise
                            // else rotate clockwise
                            // else return X
                            Z3.Context.MkITE(Z3.Context.MkGe(Z3Math.Abs(result.X), Z3Math.Abs(result.Z)),
                            Z3.Context.MkITE(Z3.Context.MkGe(result.X, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(cos, joint.X), Z3Math.Mul(sin, joint.Y)),
                            Z3Math.Add(Z3Math.Mul(cos, joint.X), Z3Math.Mul(sinNeg, joint.Y))),
                            joint.X) as ArithExpr;

                        result.Y =
                            // if Abs(X) >= Abs(Z)
                            // if X > 0
                            // rotate counter clockwise
                            // else rotate clockwise
                            // else if Z > 0
                            // rotate counter clockwise
                            // else rotate clockwise
                            Z3.Context.MkITE(Z3.Context.MkGe(Z3Math.Abs(joint.X), Z3Math.Abs(joint.Z)),
                            Z3.Context.MkITE(Z3.Context.MkGe(joint.X, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(sinNeg, joint.X), Z3Math.Mul(cos, joint.Y)),
                            Z3Math.Add(Z3Math.Mul(sin, joint.X), Z3Math.Mul(cos, joint.Y))),
                            Z3.Context.MkITE(Z3.Context.MkGe(joint.Z, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(sinNeg, joint.Z), Z3Math.Mul(cos, joint.Y)),
                            Z3Math.Add(Z3Math.Mul(sin, joint.Z), Z3Math.Mul(cos, joint.Y)))) as ArithExpr;

                        result.Z =
                            // if Abs(X) >= Abs(Z) 
                            // then return Z
                            // else rotate Z
                            // if Z > 0
                            // rotate counter clockwise
                            // else rotate clockwise
                            Z3.Context.MkITE(Z3.Context.MkGe(Z3Math.Abs(joint.X), Z3Math.Abs(joint.Z)),
                            joint.Z,
                            Z3.Context.MkITE(Z3.Context.MkGe(joint.Z, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(cos, joint.Z), Z3Math.Mul(sin, joint.Y)),
                            Z3Math.Add(Z3Math.Mul(cos, joint.Z), Z3Math.Mul(sinNeg, joint.Y)))) as ArithExpr;
                        break;

                    case Direction.Up:
                        result.X =
                            // if Abs(X) >= Abs(Z)
                            // if X > 0
                            // rotate clockwise
                            // else rotate counter clockwise
                            // else return X
                            Z3.Context.MkITE(Z3.Context.MkGe(Z3Math.Abs(result.X), Z3Math.Abs(result.Z)),
                            Z3.Context.MkITE(Z3.Context.MkGe(result.X, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(cos, joint.X), Z3Math.Mul(sinNeg, joint.Y)),
                            Z3Math.Add(Z3Math.Mul(cos, joint.X), Z3Math.Mul(sin, joint.Y))),
                            joint.X) as ArithExpr;

                        result.Y =
                            // if Abs(X) >= Abs(Z)
                            // if X > 0
                            // rotate clockwise
                            // else rotate counter clockwise
                            // else if Z > 0
                            // rotate clockwise
                            // else rotate counter clockwise
                            Z3.Context.MkITE(Z3.Context.MkGe(Z3Math.Abs(joint.X), Z3Math.Abs(joint.Z)),
                            Z3.Context.MkITE(Z3.Context.MkGe(joint.X, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(sin, joint.X), Z3Math.Mul(cos, joint.Y)),
                            Z3Math.Add(Z3Math.Mul(sinNeg, joint.X), Z3Math.Mul(cos, joint.Y))),
                            Z3.Context.MkITE(Z3.Context.MkGe(joint.Z, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(sin, joint.Z), Z3Math.Mul(cos, joint.Y)),
                            Z3Math.Add(Z3Math.Mul(sinNeg, joint.Z), Z3Math.Mul(cos, joint.Y)))) as ArithExpr;

                        result.Z =
                            // if Abs(X) >= Abs(Z) 
                            // then return Z
                            // else rotate Z
                            // if Z > 0
                            // rotate clockwise
                            // else rotate counter clockwise
                            Z3.Context.MkITE(Z3.Context.MkGe(Z3Math.Abs(joint.X), Z3Math.Abs(joint.Z)),
                            joint.Z,
                            Z3.Context.MkITE(Z3.Context.MkGe(joint.Z, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(cos, joint.Z), Z3Math.Mul(sinNeg, joint.Y)),
                            Z3Math.Add(Z3Math.Mul(cos, joint.Z), Z3Math.Mul(sin, joint.Y)))) as ArithExpr;
                        break;

                    case Direction.Right:
                        result.X =
                            // if Abs(Y) >= Abs(Z)
                            // if Y > 0
                            // rotate counter clockwise
                            // else rotate clockwise
                            // else if Z > 0
                            // rotate counter clockwise
                            // else rotate clockwise
                            Z3.Context.MkITE(Z3.Context.MkGe(Z3Math.Abs(joint.Y), Z3Math.Abs(joint.Z)),
                            Z3.Context.MkITE(Z3.Context.MkGe(joint.Y, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(sin, joint.Y), Z3Math.Mul(cos, joint.X)),
                            Z3Math.Add(Z3Math.Mul(sinNeg, joint.Y), Z3Math.Mul(cos, joint.X))),
                            Z3.Context.MkITE(Z3.Context.MkGe(joint.Z, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(sin, joint.Z), Z3Math.Mul(cos, joint.X)),
                            Z3Math.Add(Z3Math.Mul(sinNeg, joint.Z), Z3Math.Mul(cos, joint.X)))) as ArithExpr;

                        result.Y =
                            // if Abs(Y) >= Abs(Z)
                            // if Y > 0
                            // rotate counter clockwise
                            // else rotate clockwise
                            // else return Y
                            Z3.Context.MkITE(Z3.Context.MkGe(Z3Math.Abs(result.Y), Z3Math.Abs(result.Z)),
                            Z3.Context.MkITE(Z3.Context.MkGe(result.Y, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(cos, joint.Y), Z3Math.Mul(sinNeg, joint.X)),
                            Z3Math.Add(Z3Math.Mul(cos, joint.Y), Z3Math.Mul(sin, joint.X))),
                            joint.Y) as ArithExpr;

                        result.Z =
                            // if Abs(X) >= Abs(Z) 
                            // then return Z
                            // else rotate Z
                            // if Z > 0
                            // rotate counter clockwise
                            // else rotate clockwise
                            Z3.Context.MkITE(Z3.Context.MkGe(Z3Math.Abs(joint.Y), Z3Math.Abs(joint.Z)),
                            joint.Z,
                            Z3.Context.MkITE(Z3.Context.MkGe(joint.Z, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(cos, joint.Z), Z3Math.Mul(sinNeg, joint.X)),
                            Z3Math.Add(Z3Math.Mul(cos, joint.Z), Z3Math.Mul(sin, joint.X)))) as ArithExpr;
                        break;                        

                    case Direction.Left:
                        result.X =
                            // if Abs(Y) >= Abs(Z)
                            // if Y > 0
                            // rotate counter clockwise
                            // else rotate clockwise
                            // else if Z > 0
                            // rotate counter clockwise
                            // else rotate clockwise
                            Z3.Context.MkITE(Z3.Context.MkGe(Z3Math.Abs(joint.Y), Z3Math.Abs(joint.Z)),
                            Z3.Context.MkITE(Z3.Context.MkGe(joint.Y, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(sinNeg, joint.Y), Z3Math.Mul(cos, joint.X)),
                            Z3Math.Add(Z3Math.Mul(sin, joint.Y), Z3Math.Mul(cos, joint.X))),
                            Z3.Context.MkITE(Z3.Context.MkGe(joint.Z, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(sinNeg, joint.Z), Z3Math.Mul(cos, joint.X)),
                            Z3Math.Add(Z3Math.Mul(sin, joint.Z), Z3Math.Mul(cos, joint.X)))) as ArithExpr;

                        result.Y =
                            // if Abs(Y) >= Abs(Z)
                            // if Y > 0
                            // rotate counter clockwise
                            // else rotate clockwise
                            // else return Y
                            Z3.Context.MkITE(Z3.Context.MkGe(Z3Math.Abs(result.Y), Z3Math.Abs(result.Z)),
                            Z3.Context.MkITE(Z3.Context.MkGe(result.Y, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(cos, joint.Y), Z3Math.Mul(sin, joint.X)),
                            Z3Math.Add(Z3Math.Mul(cos, joint.Y), Z3Math.Mul(sinNeg, joint.X))),
                            joint.Y) as ArithExpr;

                        result.Z =
                            // if Abs(X) >= Abs(Z) 
                            // then return Z
                            // else rotate Z
                            // if Z > 0
                            // rotate counter clockwise
                            // else rotate clockwise
                            Z3.Context.MkITE(Z3.Context.MkGe(Z3Math.Abs(joint.Y), Z3Math.Abs(joint.Z)),
                            joint.Z,
                            Z3.Context.MkITE(Z3.Context.MkGe(joint.Z, Z3Math.Zero),
                            Z3Math.Add(Z3Math.Mul(cos, joint.Z), Z3Math.Mul(sin, joint.X)),
                            Z3Math.Add(Z3Math.Mul(cos, joint.Z), Z3Math.Mul(sinNeg, joint.X)))) as ArithExpr;
                        break;

                    default:
                        break;
                }

                return result;
            })
        { }
    }

    public class BodyTransform
    {
        public BodyTransform()
        {
            this.JointTransforms = new Dictionary<JointType, JointTransform>();
        }

        public BodyTransform(Dictionary<JointType, JointTransform> jointsTransforms)
        {
            this.JointTransforms = jointsTransforms;
        }

        public int TransformCount
        {
            get { { return this.JointTransforms.Count; } }
        }

        public BodyTransform(JointType jointType, JointTransform point3DTransform)
            : this()
        {
            this.ComposeJointTransform(jointType, point3DTransform);
        }

        public BodyTransform ComposeJointTransform(JointType jointType, JointTransform point3DTransform)
        {
            BodyTransform composed = new BodyTransform(this.JointTransforms);

            if (!this.JointTransforms.ContainsKey(jointType))
                composed.JointTransforms.Add(jointType, point3DTransform);
            else
                composed.JointTransforms[jointType] = this.JointTransforms[jointType].Compose(point3DTransform);

            return composed;
        }

        public BodyTransform Compose(BodyTransform that)
        {
            BodyTransform composed = new BodyTransform();

            var jointTypes = EnumUtil.GetValues<JointType>();

            foreach (var jointType in jointTypes)
            {
                // Both BodyTransforms have a transform for this joint, so compose them
                if (this.JointTransforms.ContainsKey(jointType) &&
                    that.JointTransforms.ContainsKey(jointType))
                {
                    composed.JointTransforms.Add(jointType,
                        this.JointTransforms[jointType].Compose(
                        that.JointTransforms[jointType]));
                }

                // Only this have the transform, so add it
                else if (this.JointTransforms.ContainsKey(jointType) &&
                    !that.JointTransforms.ContainsKey(jointType))
                {
                    composed.JointTransforms.Add(jointType,
                        this.JointTransforms[jointType]);
                }

                // Only that have the transform
                else if (!this.JointTransforms.ContainsKey(jointType) &&
                    that.JointTransforms.ContainsKey(jointType))
                {
                    composed.JointTransforms.Add(jointType,
                        that.JointTransforms[jointType]);
                }

                // Do nothing in case neither this or that have the transform for the joint
            }

            return composed;
        }

        public BodyTransform Compose(JointType jointType, JointTransform point3DTransform)
        {
            BodyTransform that = new BodyTransform(jointType, point3DTransform);
            return this.Compose(that);
        }

        public override bool Equals(object obj)
        {
            var that = obj as BodyTransform;
            if (that == null) return false;

            var jointTypes = EnumUtil.GetValues<JointType>();
            bool result = true;

            foreach (var jointType in jointTypes)
            {
                bool thisContainsJointTransform = this.JointTransforms.ContainsKey(jointType);
                bool thatContainsJointTransform = that.JointTransforms.ContainsKey(jointType);

                // Both BodyTransforms have a transform for this joint, so compare them
                if (thisContainsJointTransform && thatContainsJointTransform)
                {
                    result = result &&
                        this.JointTransforms[jointType].Equals(
                        that.JointTransforms[jointType]);
                }

                // If only one BodyTransform has the transform for the joint then they are not equals
                // TODO: LUCAS, what is the ^ below about?
                // it is a XOR, we know if only of them have the transform they are different for sure
                else if (thisContainsJointTransform ^ thatContainsJointTransform)
                {
                    result = false;
                }

                // result remains true in case neither this or that have the transform for the joint
            }

            return result;
        }

        /// <summary>
        /// Apply this transform to obtain an output body.
        /// </summary>
        /// <param name="inputBody"></param>
        /// <returns></returns>
        public Z3Body Transform(Z3Body inputBody)
        {
            var outputBody = new Z3Body();
            var jointTypes = EnumUtil.GetValues<JointType>();

            foreach (var jointType in jointTypes)
            {
                // Both this and inputBody have the jointType, so transform the joint
                if (this.JointTransforms.ContainsKey(jointType) &&
                    inputBody.Joints.ContainsKey(jointType))
                {
                    outputBody.Joints.Add(jointType,
                        this.JointTransforms[jointType].Transform(
                        inputBody.Joints[jointType]));

                    outputBody.Norms.Add(jointType,
                        inputBody.Norms[jointType]);
                }

                // The joint exists in body but there is no transform for it, so add it
                else if (inputBody.Joints.ContainsKey(jointType))
                {
                    outputBody.Joints.Add(jointType,
                        inputBody.Joints[jointType]);

                    outputBody.Norms.Add(jointType,
                        inputBody.Norms[jointType]);
                }

                // Do nothing otherwise
            }

            return outputBody;
        }

        public List<JointType> GetJointTypes()
        {
            var result = new List<JointType>();

            foreach (var transform in JointTransforms)
            {
                result.Add(transform.Key);
            }

            return result;
        }

        private Dictionary<JointType, JointTransform> JointTransforms { get; set; }

        public override string ToString()
        {
            var buf = new StringBuilder("BodyTransform:");

            var jointTypes = EnumUtil.GetValues<JointType>();

            foreach (var jointType in jointTypes)
            {
                if (this.JointTransforms.ContainsKey(jointType))
                    buf.AppendFormat("\n\t{0}: {1}", jointType, this.JointTransforms[jointType]);
            }

            return buf.ToString();
        }
    }
}
