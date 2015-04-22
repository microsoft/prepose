using Microsoft.Z3;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;

namespace PreposeGestures
{
    public interface IBodyRestriction
    {
        bool IsBodyAccepted(Z3Body body);

        BoolExpr Evaluate(Z3Body body);

        // Keep track of which joints are restricted
        List<JointType> GetJointTypes();

        int GetRestrictionCount();
    }

    public class CompositeBodyRestriction : IBodyRestriction
    {
        // Create an empty body restriction
        public CompositeBodyRestriction()
        {
            this.Restrictions = new List<SimpleBodyRestriction>();
        }

        public int DistinctRestrictedJointsCount
        {
            get
            {
                int retval = 0;
                List<JointType> allJointsRestricted = new List<JointType>();
                foreach (var r in this.Restrictions)
                {
                    foreach (var j in r.GetJointTypes())
                    {
                        allJointsRestricted.Add(j);
                    }
                }
                var distinctAllJointsRestricted = allJointsRestricted.Distinct();
                retval = distinctAllJointsRestricted.Count(); 

                return retval; 
            }
        }

        public CompositeBodyRestriction(SimpleBodyRestriction soleBodyRestriction)
        {
            this.Restrictions = new List<SimpleBodyRestriction>();
            this.Restrictions.Add(soleBodyRestriction);
        }

        public int NumNegatedRestrictions
        {
            get
            {
                int retval = 0;
                foreach (var r in this.Restrictions)
                {
                    if (r.isNegated)
                    {
                        retval++; 
                    }
                }
                return retval; 
            }
        }

        public void And(SimpleBodyRestriction that)
        {
            this.Restrictions.Add(that);
        }

        public CompositeBodyRestriction And(IBodyRestriction that)
        {
            CompositeBodyRestriction result = new CompositeBodyRestriction();

            result.Restrictions.AddRange(this.Restrictions);
            if (that is CompositeBodyRestriction)
            {
                result.Restrictions.AddRange(((CompositeBodyRestriction)that).Restrictions);
            }
            else
            {
                result.Restrictions.Add(((SimpleBodyRestriction)that));
            }

            return result;
        }

        public BoolExpr Evaluate(Z3Body body)
        {
            Func<Z3Body, BoolExpr> composedExpr = bodyInput =>
            {
                BoolExpr result = Z3.Context.MkTrue();

                foreach (var restriction in this.Restrictions)
                {
                    result = Z3.Context.MkAnd(result, restriction.Evaluate(bodyInput));
                }

                return result;
            };

            return composedExpr(body);
        }

        public bool IsBodyAccepted(Z3Body body)
        {
            return IsBodyAccepted(body, Z3.Context);
        }
        public bool IsBodyAccepted(Z3Body body, Context localContext)
        {
            BoolExpr resultExpr = Evaluate(body);
            bool result = Z3.EvaluateBoolExpr(resultExpr, localContext);
            
            return result;
        }

        protected List<SimpleBodyRestriction> Restrictions;

        
        public List<JointType> GetJointTypes()
        {
            List<JointType> result = new List<JointType>();

            foreach(var restriction in Restrictions)
            {
                List<JointType> jointTypes = restriction.GetJointTypes();
                foreach (var jointType in jointTypes)
                {
                    if (!result.Contains(jointType))
                        result.Add(jointType);
                }
            }

            return result;
        }

        public override string ToString()
        {
            if (Restrictions.Count > 0)
            {
                return string.Format("[{0}: \n\t\t\t{1}]", this.Restrictions.Count, string.Join(", \n\t\t\t", this.Restrictions));
            }
            else
            {
                return "none";
            }
        }


        public int GetRestrictionCount()
        {
            int result = 0;
            foreach (var restriction in this.Restrictions) {
                result += restriction.GetRestrictionCount();
            }

            return result;
        }
    }

    public class SimpleBodyRestriction : IBodyRestriction
    {
        public bool isNegated; 
        // restrictedJoints is the list of the the joints that must be activated by the restriction
        internal SimpleBodyRestriction(Func<Z3Body, BoolExpr> restriction, List<JointType> restrictedJoints)
        {
            this.RestrictionFunc = restriction;
            this.RestrictedJoints = restrictedJoints;
        }

        internal SimpleBodyRestriction(Func<Z3Body, BoolExpr> restriction, JointType restrictedJoint)
        {
            this.RestrictionFunc = restriction;
            this.RestrictedJoints = new List<JointType>();
            this.RestrictedJoints.Add(restrictedJoint);
        }

        internal SimpleBodyRestriction(Func<Z3Body, BoolExpr> restriction, params JointType[] restrictedJoints)
        {
            Contract.Ensures(restrictedJoints.Length > 0);

            this.RestrictionFunc = restriction;
            this.RestrictedJoints = new List<JointType>();

            foreach(var restrictedJoint in restrictedJoints)
                this.RestrictedJoints.Add(restrictedJoint);
        }

        public override bool Equals(object obj)
        {
            var that = obj as SimpleBodyRestriction;
            Solver solver = Z3.Context.MkSolver();
            
            Z3Body body = Z3Body.MkZ3Const();
            var jointTypes = EnumUtil.GetValues<JointType>();
            BoolExpr thisResult = this.Evaluate(body);
            BoolExpr thatResult = that.Evaluate(body);

            BoolExpr equalsExpr = Z3.Context.MkEq(thisResult, thatResult);

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
            
            return result;
        }

        public bool IsBodyAccepted(Z3Body body)
        {
            BoolExpr resultExpr = RestrictionFunc(body);

            Solver solver = Z3.Context.MkSolver();

            solver.Push();
            solver.Assert(Z3.Context.MkNot(resultExpr));
            Status status = solver.Check();
            Statistics stats = solver.Statistics; 

            bool result = (status == Status.UNSATISFIABLE);

            solver.Pop();

            return result;
        }

        public BoolExpr Evaluate(Z3Body body)
        {
            return this.RestrictionFunc(body);
        }

        public List<JointType> GetJointTypes()
        {
            return this.RestrictedJoints;
        }

        static object ToStringHelper(System.Linq.Expressions.Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Constant:
                    return ((ConstantExpression)exp).Value;
                case ExpressionType.MemberAccess:
                    var me = (MemberExpression)exp;
                    switch (me.Member.MemberType)
                    {
                        case System.Reflection.MemberTypes.Field:
                            return ((FieldInfo)me.Member).GetValue(ToStringHelper(me.Expression));
                        case MemberTypes.Property:
                            return ((PropertyInfo)me.Member).GetValue(ToStringHelper(me.Expression), null);
                        default:
                            throw new NotSupportedException(me.Member.MemberType.ToString());
                    }
                default:
                    throw new NotSupportedException(exp.NodeType.ToString());
            }
        }
        
        private Func<Z3Body, BoolExpr> RestrictionFunc { get; set; }

        // Keep track of which joints are restricted
        private List<JointType> RestrictedJoints;


        public int GetRestrictionCount()
        {
            return 1;
        }
    }

    public class NoBodyRestriction : SimpleBodyRestriction
    {
        public NoBodyRestriction() : base(body => Z3.Context.MkTrue(), new List<JointType>()) { }

        public override string ToString()
        {
            return "none";
        }
    }

    public class LowerThanBodyRestriction : SimpleBodyRestriction
    {
        public LowerThanBodyRestriction(JointType jointType, int angleThreshold)
            : base(
            body =>
            {
                double maxY = TrigonometryHelper.GetSine(angleThreshold);
                return Z3.Context.MkLt(body.Joints[jointType].Y, Z3Math.Real(maxY));
            },
            jointType)
        {
            this.JointType = jointType;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", this.GetType().Name, this.JointType);
        }

        public JointType JointType { get; private set; }
    }

    public class KeepAngleRestriction : SimpleBodyRestriction
    {
        public KeepAngleRestriction(JointType jointType, Direction direction, int angleThreshold = 15)
            : base(
            body =>
            {
                return body.Joints[jointType].IsAngleBetweenLessThan(Z3Point3D.DirectionPoint(direction), angleThreshold);
            },
            jointType)
        {
            this.JointType = jointType;
            this.Direction = direction;
        }

        public override string ToString()
        {
            return string.Format("keep your {0} {1}", this.JointType, this.Direction);
        }

        public JointType JointType { get; private set; }
        public Direction Direction { get; set; }
    }

    public class TouchBodyRestriction : SimpleBodyRestriction
    {
        public TouchBodyRestriction(JointType jointType, JointSide handSide, double distanceThreshold = 0.2, bool dont = false)
            : base(body =>
            {
                JointType sidedHandType = JointTypeHelper.GetSidedJointType(SidedJointName.Hand, handSide);

                Z3Point3D joint1Position = body.GetJointPosition(jointType);
                Z3Point3D joint2Position = body.GetJointPosition(sidedHandType);

                BoolExpr expr = joint1Position.IsNearerThan(joint2Position, distanceThreshold);
                if (dont)
                {
                    expr = Z3.Context.MkNot(expr);
                }

                return expr;
            },
            jointType,
            JointTypeHelper.GetSidedJointType(SidedJointName.Hand, handSide))
        {
            this.JointType = jointType;
            this.HandSide = handSide;
            if (dont)
            {
                isNegated = true; 
            }
            else
            {
                isNegated = false;
            }
        }

        public override string ToString()
        {
            return string.Format("touch your {0} with your {1} hand", this.JointType, this.HandSide);
        }

        public JointSide HandSide { get; private set; }

        public JointType JointType { get; private set; }
    }

    public class PutBodyRestriction : SimpleBodyRestriction
    {
        public PutBodyRestriction(JointType jointType1, JointType jointType2, RelativeDirection direction, bool dont = false)
            : base(body =>
            {
                ArithExpr distanceThreshold = Z3Math.Real(0.1);

                Z3Point3D joint1Position = body.GetJointPosition(jointType1);
                Z3Point3D joint2Position = body.GetJointPosition(jointType2);

                BoolExpr expr = Z3.Context.MkTrue();

                switch (direction)
                {
                    case RelativeDirection.InFrontOfYour:
                        expr = Z3.Context.MkGt(joint1Position.Z, Z3Math.Add(joint2Position.Z, distanceThreshold));
                        break;

                    case RelativeDirection.BehindYour:
                        expr = Z3.Context.MkLt(joint1Position.Z, Z3Math.Sub(joint2Position.Z, distanceThreshold));
                        break;

                    case RelativeDirection.ToTheRightOfYour:
                        expr = Z3.Context.MkGt(joint1Position.X, Z3Math.Add(joint2Position.X, distanceThreshold));
                        break;

                    case RelativeDirection.ToTheLeftOfYour:
                        expr = Z3.Context.MkLt(joint1Position.X, Z3Math.Sub(joint2Position.X, distanceThreshold));
                        break;

                    case RelativeDirection.OnTopOfYour:
                        expr = Z3.Context.MkGt(joint1Position.Y, Z3Math.Add(joint2Position.Y, distanceThreshold));
                        break;

                    case RelativeDirection.BelowYour:
                        expr = Z3.Context.MkLt(joint1Position.Y, Z3Math.Sub(joint2Position.Y, distanceThreshold));
                        break;
                }

                if (dont) expr = Z3.Context.MkNot(expr);

                return expr;
            },
            //JointTypeHelper.GetListFromLeafToRoot(jointType1).Union(JointTypeHelper.GetListFromLeafToRoot(jointType2)).ToList())
            jointType1,
            jointType2)
        {
            this.JointType1 = jointType1;
            this.JointType2 = jointType2;
            
            this.Direction = direction;       
            if (dont)
            {
                this.isNegated = true; 
            }
            else
            {
                this.isNegated = false; 
            }
        }

        public override string ToString()
        {
            return string.Format("put your {0} {1} {2}", this.JointType1, this.Direction, this.JointType2);
        }

        public JointType JointType1 { get; set; }

        public JointType JointType2 { get; set; }

        public RelativeDirection Direction { get; set; }
    }

    public class AlignBodyRestriction : SimpleBodyRestriction
    {
        public AlignBodyRestriction(JointType jointType1, JointType jointType2, int angleThreshold = 20, bool dont = false) :
            base((body =>
             {
                 var joint1 = body.Joints[jointType1];
                 var joint2 = body.Joints[jointType2];

                 BoolExpr expr = joint1.IsAngleBetweenLessThan(joint2, angleThreshold);
                 if (dont) expr = Z3.Context.MkNot(expr);
                 return expr;
             }), jointType1, jointType2)
        {
            this.JointType1 = jointType1;
            this.JointType2 = jointType2;
            if (dont)
            {
                this.isNegated = true; 
            }
            else
            {
                this.isNegated = false; 
            }
        }

        public override string ToString()
        {
            return string.Format("align your {0} and your {1}", this.JointType1, this.JointType2);
        }

        public JointType JointType1 { get; set; }

        public JointType JointType2 { get; set; }
    }

    // TODO Create restriction to filter invalid bodies (e.g.: with (0, 0, 0) points)
}
