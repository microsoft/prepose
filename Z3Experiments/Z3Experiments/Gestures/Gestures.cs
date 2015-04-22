using Microsoft.Z3;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace PreposeGestures
{
	public class Gesture
	{
		public Gesture(string name)
		{
			this.Name = name;
			this.Steps = new List<ExecutionStep>();
			this.DeclaredPoses = new List<Pose>();
		}

		public void AddPose(Pose pose)
		{
			this.DeclaredPoses.Add(pose);
		}

		public void AddStep(ExecutionStep newStep)
		{
			this.Steps.Add(newStep);
		}

        public void FinalResult(Z3Body input, out Z3Body transformed, out BoolExpr evaluation)
        {
            transformed = input;
            evaluation = Z3Math.True;

            foreach (var step in this.Steps)
            {
                var pose = step.Pose;
                transformed = pose.Transform.Transform(transformed);
                evaluation = Z3.Context.MkAnd(evaluation, pose.Restriction.Evaluate(transformed));
            }
        }

		public override string ToString()
		{
			return string.Format("GESTURE {0} = \n\t\t{1}\n\t\tEXECUTION = \n\t\t\t{2}", this.Name,
				string.Join("\n\t\t", this.DeclaredPoses),
				string.Join("\n\t\t\t", this.Steps));
		}
       

		public List<ExecutionStep> Steps { get; private set; }

        public int NegatedRestrictionCount
        {
            get { return this.DeclaredPoses.Sum(p => p.NegatedRestrictionCount); }
        }
        public int RestrictionCount
        {
            get { return this.DeclaredPoses.Sum(p => p.RestrictionCount); }
        }

        public int TransformCount
        {
            get { return this.DeclaredPoses.Sum(p => p.TransformCount); }
        }

        public int DistinctTransformedJointsCount
        {
            get
            {
                int retval = 0;

                List<JointType> allJoints = new List<JointType>(); 
                foreach (var p in this.DeclaredPoses)
                {
                    foreach (var j in p.GetTransformJointTypes())
                    {
                        allJoints.Add(j);
                    }
                }
                var distinctJoints = allJoints.Distinct();
                retval = distinctJoints.Count(); 
                return retval; 
            }
        }

		public List<Pose> DeclaredPoses { get; private set; }


		public string Name { get; set; }

    }	
}