using Microsoft.Z3;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PreposeGestures
{
	public class Pose
	{
		public Pose(string name)
		{
			this.Name = name;
			this.Transform = new BodyTransform();
			this.Restriction = new CompositeBodyRestriction();
		}

		public Pose(string name, BodyTransform transform)
		{
			this.Name = name;
			this.Transform = transform;
			this.Restriction = new CompositeBodyRestriction(new NoBodyRestriction());
		}

        public int DistinctRestrictedJointsCount
        {
            get
            {
                int retval = 0;
                retval = this.Restriction.DistinctRestrictedJointsCount; 
                return retval; 
            }
        }

        public int RestrictionCount
        {
            get
            {
                return this.Restriction.GetRestrictionCount();
            }
        }

        public int NegatedRestrictionCount
        {
            get
            {
                return this.Restriction.NumNegatedRestrictions; 
            }

        }

        public int TransformCount
        {
            get
            {
                return this.Transform.TransformCount;
            }
        }

		public Pose(string name, BodyTransform transform, IBodyRestriction restriction)
		{
			this.Name = name;
			this.Transform = transform;
			this.Restriction = (restriction is CompositeBodyRestriction) ?
				(CompositeBodyRestriction)restriction :
				new CompositeBodyRestriction((SimpleBodyRestriction)restriction);			

			// Check if restriction allows transform
			if (!this.IsTransformAcceptedByRestriction())
			{
				throw new ArgumentException("The restriction does not allow the transform.", "restriction");
			}
		}

		public bool IsTransformAcceptedByRestriction()
		{
			Z3Body body = Z3Body.MkZ3Const();
			Z3Body transformedBody = this.Transform.Transform(body);
			BoolExpr expr = this.Restriction.Evaluate(transformedBody);

			SolverCheckResult checkResult = Z3AnalysisInterface.CheckStatus(expr);

			return (checkResult.Status != Status.UNSATISFIABLE);
		}

		

		public void Compose(JointType jointType, JointTransform point3DTransform)
		{
			this.Transform.Compose(jointType, point3DTransform);
		}

		public void Compose(BodyTransform newTransform)
		{
			this.Transform.Compose(newTransform);
		}

		public void Compose(IBodyRestriction newRestriction)
		{
			this.Restriction.And(newRestriction);
		}

		public bool IsBodyAccepted(
			Z3Body input)
		{
			bool result = this.Restriction.IsBodyAccepted(input);
			return result;
		}

		public bool IsTransformedBodyAccepted(Z3Body body)
		{
			bool result = false;

			Z3Body transformedBody = this.Transform.Transform(body);
			result = this.IsBodyAccepted(transformedBody);

			return result;
		}

		public Z3Target CalcNearestTargetBody(Z3Body startBody)
		{
			// Create binary search to look for nearest body
			int numSteps = 6;
			int angleThreshold = 90;
			int angleIncrement = 90;
			Z3Target target = null;
			for (int i = 0; i < numSteps; ++i)
			{
				// Ask for a witness which is within the range
				target = Z3AnalysisInterface.GenerateTarget(
					this.Transform,
					this.Restriction,
					startBody,
					angleThreshold);

				// Update angle threshold
				angleIncrement /= 2;

				if (target != null)
				{
					angleThreshold -= angleIncrement;
				}
				else
				{
					angleThreshold += angleIncrement;
				}
			}

            // If target still null it probably means Z3 was unable to solve the restrictions
            // This way we generate a target using onlye the transforms
            if(target == null)
            {
                target = new Z3Target();
                target.Body = this.Transform.Transform(startBody);
                target.TransformedJoints = this.Transform.GetJointTypes();
            }

            // If target still null assing a new body as an error proof policy
            if(target == null)
            {
                target = new Z3Target();
                target.Body = startBody;
            }

			return target;
		}

		public List<JointType> GetAllJointTypes()
		{
            return Transform.GetJointTypes().Union(Restriction.GetJointTypes()).ToList();
		}

        public List<JointType> GetRestrictionJointTypes()
        {
            return this.Restriction.GetJointTypes();
        }

        public List<JointType> GetTransformJointTypes()
        {
            return this.Transform.GetJointTypes();
        }

		internal BodyTransform Transform { get; private set; }

		internal CompositeBodyRestriction Restriction { get; private set; }
		public string Name { get; set; }

		public override string ToString()
		{
			return string.Format("POSE {0} : {1}", this.Name, this.Restriction);
		}

    }
}
