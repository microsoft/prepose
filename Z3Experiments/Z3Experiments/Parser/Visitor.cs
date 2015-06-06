using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PreposeGestures;

namespace PreposeGestures.Parser
{
	internal class AppConverter : PreposeGesturesBaseVisitor<Wrapper>
	{
		public override Wrapper VisitApp(PreposeGesturesParser.AppContext context)
		{
			IList<Gesture> gestures = new List<Gesture>();
			foreach (var gesture in context.gesture()) {
				var converted = (Gesture)this.Visit(gesture);
				gestures.Add(converted);
			}
			return new Wrapper(
				new App(
					context.ID().GetText(),
					gestures
			));
		}

		public override Wrapper VisitGesture(PreposeGesturesParser.GestureContext context)
		{
			var g = new Gesture(context.ID().GetText());
			foreach (var d in context.pose()) {
				var converted = (Pose)this.Visit(d);
				g.AddPose(converted);
			}

			foreach (var d in context.execution().execution_step())
			{
				var converted = (ExecutionStep)this.Visit(d);
				g.AddStep(converted);
			}
			return new Wrapper(g);
		}

		public override Wrapper VisitBody_part(PreposeGesturesParser.Body_partContext context)
		{
			return base.VisitBody_part(context);
		}

		public override Wrapper VisitSpine(PreposeGesturesParser.SpineContext context)
		{
			var joints = new List<JointType>();
			joints.Add(JointType.SpineMid);
			joints.Add(JointType.SpineShoulder);

			return new Wrapper(new JointGroup(joints));
		}

		public override Wrapper VisitBack(PreposeGesturesParser.BackContext context)
		{
			var joints = JointTypeHelper.GetBack();
			return new Wrapper(new JointGroup(joints));
		}

		public override Wrapper VisitArm(PreposeGesturesParser.ArmContext context)
		{
			var side = (JointSide)this.Visit(context.side());
			var joints = JointTypeHelper.GetArm(side);

			return new Wrapper(new JointGroup(joints));
		}

		public override Wrapper VisitArms(PreposeGesturesParser.ArmsContext context)
		{
			var joints = JointTypeHelper.GetArms();
			return new Wrapper(new JointGroup(joints));
		}

		public override Wrapper VisitShoulders(PreposeGesturesParser.ShouldersContext context)
		{
			var joints = JointTypeHelper.GetShoulders();
			return new Wrapper(new JointGroup(joints));
		}

		public override Wrapper VisitWrists(PreposeGesturesParser.WristsContext context)
		{
			var joints = JointTypeHelper.GetWrists();
			return new Wrapper(new JointGroup(joints));
		}

		public override Wrapper VisitElbows(PreposeGesturesParser.ElbowsContext context)
		{
			var joints = JointTypeHelper.GetElbows();
			return new Wrapper(new JointGroup(joints));
		}

		public override Wrapper VisitHands(PreposeGesturesParser.HandsContext context)
		{
			var joints = JointTypeHelper.GetHands();
			return new Wrapper(new JointGroup(joints));
		}

		public override Wrapper VisitHands_tips(PreposeGesturesParser.Hands_tipsContext context)
		{
			var joints = JointTypeHelper.GetHandsTips();
			return new Wrapper(new JointGroup(joints));
		}

		public override Wrapper VisitThumbs(PreposeGesturesParser.ThumbsContext context)
		{
			var joints = JointTypeHelper.GetThumbs();
			return new Wrapper(new JointGroup(joints));
		}

		public override Wrapper VisitLeg(PreposeGesturesParser.LegContext context)
		{
			var side = (JointSide)this.Visit(context.side());
			var joints = JointTypeHelper.GetLeg(side);
			return new Wrapper(new JointGroup(joints));
		}

		public override Wrapper VisitLegs(PreposeGesturesParser.LegsContext context)
		{
			var joints = JointTypeHelper.GetLegs();
			return new Wrapper(new JointGroup(joints));
		}

		public override Wrapper VisitHips(PreposeGesturesParser.HipsContext context)
		{
			var joints = JointTypeHelper.GetHips();
			return new Wrapper(new JointGroup(joints));
		}

		public override Wrapper VisitKnees(PreposeGesturesParser.KneesContext context)
		{
			var joints = JointTypeHelper.GetKnees();
			return new Wrapper(new JointGroup(joints));
		}

		public override Wrapper VisitAnkles(PreposeGesturesParser.AnklesContext context)
		{
			var joints = JointTypeHelper.GetAnkles();
			return new Wrapper(new JointGroup(joints));
		}

		public override Wrapper VisitFeet(PreposeGesturesParser.FeetContext context)
		{
			var joints = JointTypeHelper.GetFeet();
			return new Wrapper(new JointGroup(joints));
		}

		public override Wrapper VisitPose(PreposeGesturesParser.PoseContext context)
		{
			var bt = new BodyTransform();
			var br = new CompositeBodyRestriction();
			foreach (var s in context.statement())
			{
				Contract.Assert(s != null);
				var w = this.Visit(s);
                if (w == null)
                {
                    throw new PreposeParserException("Failed to parse statement", s);
                }
				var statement = w.GetValue();
				if (statement != null)
				{
					if (statement is BodyTransform)
					{
						bt = bt.Compose((BodyTransform)statement);
						continue;
					}

					if (statement is CompositeBodyRestriction)
					{
						br = br.And((CompositeBodyRestriction)statement);
						continue;
					}

                    throw new PreposeParserException("Invalid return type", s);
				}
			}

			var pose = new Pose(context.ID().GetText(), bt, br);
			if (this.Poses.ContainsKey(pose.Name))
			{
                throw new PreposeParserException("Pose " + pose.Name + " has been previously seen.", context);
			}

			this.Poses.Add(pose.Name, pose);
			
			return new Wrapper(pose);
		}

		//        rotate_transform :  
		//                'rotate' 'your' body_part angle angular_direction 'on' 'the' ? reference_plane;
		public override Wrapper VisitRotate_plane_transform(PreposeGesturesParser.Rotate_plane_transformContext context)
		{
			BodyTransform transform = new BodyTransform();
			var direction = (RotationDirection)this.Visit(context.angular_direction());
			var plane = (BodyPlaneType)this.Visit(context.reference_plane());
			var angleText = context.NUMBER().GetText();
			var angle = Convert.ToInt32(angleText);
			foreach (var b in context.body_part())
			{
				var converted = (JointGroup)this.Visit(b);
				transform = transform.Compose(
					converted.Aggregate(j =>
						BodyTransformBuilder.RotateTransform(j, angle, plane, direction)));
			}

			return new Wrapper(transform);
		}

		public override Wrapper VisitRotate_direction_transform(PreposeGesturesParser.Rotate_direction_transformContext context)
		{
			BodyTransform transform = new BodyTransform();
			var direction = (Direction)this.Visit(context.direction());
			var angleText = context.NUMBER().GetText();
			var angle = Convert.ToInt32(angleText);            

			foreach (var b in context.body_part())
			{
				var converted = (JointGroup)this.Visit(b);
				transform = transform.Compose(
					converted.Aggregate(j =>
						BodyTransformBuilder.RotateTransform(j, angle, direction)));
			}

			return new Wrapper(transform);
		}

		//point_to_transform :    
		//                                'point' 'your' body_part direction;
		public override Wrapper VisitPoint_to_transform(PreposeGesturesParser.Point_to_transformContext context)
		{
			var direction = (Direction)this.Visit(context.direction());
			BodyTransform transform = new BodyTransform();

			foreach (var b in context.body_part())
			{
				var converted = (JointGroup)this.Visit(b);
				transform = transform.Compose(converted.Aggregate(j => BodyTransformBuilder.PointToTransform(j, direction)));
			}

			return new Wrapper(transform);
		}

        static bool dont = false;
        public override Wrapper VisitRestriction(PreposeGesturesParser.RestrictionContext context)
        {
            var negate = context.dont();
            if (negate != null) dont = true;
            else dont = false;
            return base.VisitRestriction(context);
        }

		//touch_restriction       :  'touch' 'your'? body_part 'with' 'your'? side 'hand';
		public override Wrapper VisitTouch_restriction(PreposeGesturesParser.Touch_restrictionContext context)
		{
			var joint = (JointGroup)this.Visit(context.body_part());
			var side = (JointSide)this.Visit(context.side());

			CompositeBodyRestriction restriction = joint.Aggregate(j => new TouchBodyRestriction(j, side, 0.2, dont));

			return new Wrapper(restriction);
		}

		// TODO test this logic to aggergate restrictions
		//put_restriction :  'put' 'your'? body_part relative_direction body_part;
		public override Wrapper VisitPut_restriction(PreposeGesturesParser.Put_restrictionContext context)
		{
			var target = (JointGroup)this.Visit(context.body_part()[context.body_part().Count - 1]);            

			var direction = (RelativeDirection)this.Visit(context.relative_direction());

			CompositeBodyRestriction restriction = new CompositeBodyRestriction();

			for (int i = 0; i < context.body_part().Count - 1; ++i)
			{
				var joint = (JointGroup)this.Visit(context.body_part()[i]);                

				restriction = restriction.And(
				target.Aggregate(j2 =>
					joint.Aggregate(j1 => new PutBodyRestriction(j1, j2, direction, dont))));
			}

			return new Wrapper(restriction);
		}

		//align your body_part and your body_part;
		public override Wrapper VisitAlign_restriction(PreposeGesturesParser.Align_restrictionContext context)
		{
			var target = (JointGroup)this.Visit(context.body_part()[context.body_part().Count - 1]);

			CompositeBodyRestriction restriction = new CompositeBodyRestriction();

			for (int i = 0; i < context.body_part().Count - 1; ++i)
			{
				var joint = (JointGroup)this.Visit(context.body_part()[i]);

				restriction = restriction.And(
				target.Aggregate(j2 =>
					joint.Aggregate(j1 => new AlignBodyRestriction(j1, j2, 20, dont))));
			}

			return new Wrapper(restriction);
		}

		////keep_restriction :   'keep' 'your'? body_part relative_constraint ;
		//public override Wrapper VisitKeep_restriction(PreposeGesturesParser.Keep_restrictionContext context)
		//{
		//    var joint = (JointGroup)this.Visit(context.body_part());
		//    var direction = (Direction)this.Visit(context.relative_constraint().direction());
		//    CompositeBodyRestriction restriction = joint.Aggregate(j => new KeepRestriction(j, direction));

		//    return new Wrapper(restriction);
		//}

		public override Wrapper VisitJoint(PreposeGesturesParser.JointContext context)
		{
			if (context.side() != null)
			{
				var side = (JointSide)this.Visit(context.side());
				switch (context.sided_joint().GetText())
				{
					case "shoulder": return side == JointSide.Left ? new Wrapper(JointType.ShoulderLeft) : new Wrapper(JointType.ShoulderRight);
					case "elbow": return side == JointSide.Left ? new Wrapper(JointType.ElbowLeft) : new Wrapper(JointType.ElbowRight);
					case "hand": return side == JointSide.Left ? new Wrapper(JointType.HandLeft) : new Wrapper(JointType.HandRight);
					case "hip": return side == JointSide.Left ? new Wrapper(JointType.HipLeft) : new Wrapper(JointType.HipRight);
					case "wrist": return side == JointSide.Left ? new Wrapper(JointType.WristLeft) : new Wrapper(JointType.WristRight);
					case "handtip": return side == JointSide.Left ? new Wrapper(JointType.HandTipLeft) : new Wrapper(JointType.HandTipRight);
					case "thumb": return side == JointSide.Left ? new Wrapper(JointType.ThumbLeft) : new Wrapper(JointType.ThumbRight);
					case "knee": return side == JointSide.Left ? new Wrapper(JointType.KneeLeft) : new Wrapper(JointType.KneeRight);
					case "ankle": return side == JointSide.Left ? new Wrapper(JointType.AnkleLeft) : new Wrapper(JointType.AnkleRight);
					case "foot": return side == JointSide.Left ? new Wrapper(JointType.FootLeft) : new Wrapper(JointType.FootRight);
					
					default: throw new ArgumentException(context.center_joint().GetText());
				}
			}
			else
			{
				switch (context.center_joint().GetText())
				{
					case "neck": return new Wrapper(JointType.Neck);
					case "head": return new Wrapper(JointType.Head);
					case "hips": return new Wrapper(new JointGroup(new JointType[] { JointTypeHelper.GetSidedJointType(SidedJointName.Hip, JointSide.Left), JointTypeHelper.GetSidedJointType(SidedJointName.Hip, JointSide.Right) }));
					case "back": return new Wrapper(new JointGroup(JointTypeHelper.GetBack()));
					case "you": return new Wrapper(new JointGroup(JointTypeHelper.GetYou()));

					default: throw new ArgumentException();
				}
			}
		}

		public override Wrapper VisitReference_plane(PreposeGesturesParser.Reference_planeContext context)
		{
			if (context.GetText().StartsWith("frontal")) return new Wrapper(BodyPlaneType.Frontal);
			if (context.GetText().StartsWith("sagittal")) return new Wrapper(BodyPlaneType.Sagittal);
			if (context.GetText().StartsWith("horizontal")) return new Wrapper(BodyPlaneType.Horizontal);

			throw new ArgumentException("Invalid " + context.GetText());
		}

		public override Wrapper VisitRelative_direction(PreposeGesturesParser.Relative_directionContext context)
		{
			var text = context.GetText();
			switch (text)
			{
				case "infrontofyour":
				case "infrontof": return new Wrapper(PreposeGestures.RelativeDirection.InFrontOfYour);
				case "behindyour": return new Wrapper(PreposeGestures.RelativeDirection.BehindYour);
				case "ontopofyour": return new Wrapper(PreposeGestures.RelativeDirection.OnTopOfYour);
				case "aboveyour": return new Wrapper(PreposeGestures.RelativeDirection.OnTopOfYour);
				case "belowyour": return new Wrapper(PreposeGestures.RelativeDirection.BelowYour);
				case "totheleftofyour": return new Wrapper(PreposeGestures.RelativeDirection.ToTheLeftOfYour);
				case "totherightofyour": return new Wrapper(PreposeGestures.RelativeDirection.ToTheRightOfYour);

				default:
					throw new ArgumentException(context.GetText());
			}
		}

		public override Wrapper VisitAngular_direction(PreposeGesturesParser.Angular_directionContext context)
		{
			if (context.GetText().StartsWith("counter"))
			{
				return new Wrapper(RotationDirection.CounterClockwise);
			}
			else
			{
				return new Wrapper(RotationDirection.Clockwise);
			}
		}

		public override Wrapper VisitDirection(PreposeGesturesParser.DirectionContext context)
		{
			var text = context.GetText();
			switch (text)
			{
				case "up": return new Wrapper( PreposeGestures.Direction.Up);
				case "down": return new Wrapper(PreposeGestures.Direction.Down);
				case "front": return new Wrapper(PreposeGestures.Direction.Front);
				case "back": return new Wrapper(PreposeGestures.Direction.Back);
				case "left": return new Wrapper(PreposeGestures.Direction.Left);
				case "right": return new Wrapper(PreposeGestures.Direction.Right);

				default: throw new ArgumentException(context.GetText());
			}
		}

		public override Wrapper VisitSide(PreposeGesturesParser.SideContext context)
		{
			if (context.GetText().Equals("left"))
			{
				return new Wrapper(JointSide.Left);
			}
			if (context.GetText().Equals("right"))
			{
				return new Wrapper(JointSide.Right);
			}

			throw new ArgumentException();
		}

		private Dictionary<string, Pose> Poses = new Dictionary<string, Pose>();

		public override Wrapper VisitExecution_step(PreposeGesturesParser.Execution_stepContext context)
		{
			var pose = GetPose(context.ID().GetText());
			if (context.motion_constraint() != null && context.hold_constraint() != null) { 
				var restriction = context.motion_constraint().GetText();                
				var motionConstraint = Wrapper.Convert(restriction);
				var duration = int.Parse(context.hold_constraint().NUMBER().GetText());
				return 
					new Wrapper(
						new ExecutionStep(
							motionConstraint, 
							pose, 
							duration));
			}
			if (context.motion_constraint() != null) {
				var restriction = context.motion_constraint().GetText();
				var motionConstraint = Wrapper.Convert(restriction);
				return
					new Wrapper(
						new ExecutionStep(
							motionConstraint,
							pose));
			}

			if (context.hold_constraint() != null)
			{
				var duration = int.Parse(context.hold_constraint().NUMBER().GetText());
				return
					new Wrapper(
						new ExecutionStep(
							pose, duration));
			}

			return new Wrapper(new ExecutionStep(pose));
		}

		private Pose GetPose(string id)
		{
			return this.Poses[id];
		}
	}

	internal class Wrapper
	{
		private object value;

		internal Wrapper(App app)
		{ 
			this.value = app;
		}

		internal Wrapper(Pose pose)
		{
			this.value = pose;
		}

		internal Wrapper(BodyPlaneType planeType) {
			this.value = planeType;
		}

		internal Wrapper(RotationDirection dir)
		{
			this.value = dir;
		}

		internal Wrapper(JointType joint)
		{
			this.value = joint;
		}

		internal Wrapper(JointSide side)
		{
			this.value = side;
		}

		internal Wrapper(ExecutionStep step)
		{
			this.value = step;
		}

		internal Wrapper(Gesture gesture)
		{
			this.value = gesture;
		}

		internal Wrapper(IBodyRestriction restriction)
		{
			this.value = restriction;
		}

		internal Wrapper(RelativeDirection direction) {
			this.value = direction;
		}

		internal Wrapper(Direction direction)
		{
			this.value = direction;
		}

		internal Wrapper(BodyTransform transform)
		{
			this.value = transform;
		}

		internal Wrapper(JointTransform transform)
		{
			this.value = transform;
		}

		internal Wrapper(JointGroup joints)
		{
			this.value = joints;
		}

		public static implicit operator Gesture(Wrapper w) {
			Contract.Requires(w != null);
			return (Gesture)w.value;
		}

		public static implicit operator ExecutionStep(Wrapper w)
		{
			Contract.Requires(w != null);
			return (ExecutionStep)w.value;
		}

		public static implicit operator RelativeDirection(Wrapper w)
		{
			Contract.Requires(w != null);
			return (RelativeDirection)w.value;
		}

		public static implicit operator String(Wrapper w)
		{
			Contract.Requires(w != null);
			return (String)w.value;
		}

		public static implicit operator RotationDirection(Wrapper w)
		{
			Contract.Requires(w != null);
			return (RotationDirection)w.value;
		}

		public static implicit operator BodyPlaneType(Wrapper w)
		{
			Contract.Requires(w != null);
			return (BodyPlaneType)w.value;
		}

		public static implicit operator Direction(Wrapper w)
		{
			Contract.Requires(w != null);
			return (Direction)w.value;
		}

		public static implicit operator CompositeBodyRestriction(Wrapper w)
		{
			Contract.Requires(w != null);
			return (CompositeBodyRestriction)w.value;
		}

		public static implicit operator JointType(Wrapper w)
		{
			Contract.Requires(w != null);
			return (JointType)w.value;
		}

		public static implicit operator JointGroup(Wrapper w)
		{
			Contract.Requires(w != null);
			if (w.value is JointType)
			{
				return new JointGroup((JointType)w.value);
			}
			else
			{
				return (JointGroup)w.value;
			}
		}

		public static implicit operator JointSide(Wrapper w)
		{
			Contract.Requires(w != null);
			return (JointSide)w.value;
		}

		public static implicit operator Pose(Wrapper w)
		{
			Contract.Requires(w != null);
			return (Pose)w.value;
		}

		public static implicit operator BodyTransform(Wrapper w)
		{
			Contract.Requires(w != null);
			return (BodyTransform)w.value;
		}

		public static implicit operator JointTransform(Wrapper w)
		{
			Contract.Requires(w != null);
			return (JointTransform)w.value;
		}

		public static implicit operator App(Wrapper w)
		{
			Contract.Requires(w != null);
			return (App)w.value;
		}

		public static MotionRestriction Convert(string restriction) {
			switch (restriction) {
				case "slowly": return MotionRestriction.Slowly;
				case "rapidly": return MotionRestriction.Rapidly;

				default: throw new ArgumentException(restriction);
			}
		}

		internal object GetValue()
		{
			return this.value;
		}
	}
}
