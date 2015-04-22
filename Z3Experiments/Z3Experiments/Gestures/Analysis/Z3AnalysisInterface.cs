using Microsoft.Z3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading; 
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;

namespace PreposeGestures
{
    /// <summary>
    /// Check various safety conditions using the Z3 solver.
    /// </summary>
	public static class Z3AnalysisInterface
	{
		/// <summary>
		/// Generates a witness body tha matches the restrictions that 
		/// are passed in.
		/// </summary>
		/// <param name="bodyRestriction"></param>
		/// <returns></returns>
		public static Z3Body GenerateWitness(IBodyRestriction bodyRestriction)
		{
			var body = Z3Body.MkZ3Const();

			var expr = bodyRestriction.Evaluate(body);

			var checkResult = CheckStatus(expr);
			var witness = CreateBodyWitness(
				body,
				checkResult.Model,
				bodyRestriction.GetJointTypes(),
				JointTypeHelper.CreateDefaultZ3Body());

			return witness;
		}

		/// <summary>
		/// Returns a witness body that matches given output restrictions 
		/// for a particular bodyTransform.
		/// </summary>
		/// <param name="bodyTransform"></param>
		/// <param name="outputBodyRestriction"></param>
		/// <returns></returns>
		public static Z3Body GenerateWitness(BodyTransform bodyTransform, CompositeBodyRestriction outputBodyRestriction)
		{
			var body = Z3Body.MkZ3Const();
			var transformedBody = bodyTransform.Transform(body);

			var expr = outputBodyRestriction.Evaluate(transformedBody);

			var evaluatedJoints =
				JointTypeHelper.MergeJointTypeLists(
				bodyTransform.GetJointTypes(),
				outputBodyRestriction.GetJointTypes());

			var checkResult = CheckStatus(expr);
			if (checkResult.Status == Status.SATISFIABLE)
			{
				var witness = CreateBodyWitness(
					body,
					checkResult.Model,
					evaluatedJoints,
					JointTypeHelper.CreateDefaultZ3Body());
				return witness;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Produces a witness body given input restrictions, output
		/// restrictions and a transform.
		/// </summary>
		/// <param name="inputBodyRestriction"></param>
		/// <param name="bodyTransform"></param>
		/// <param name="outputBodyRestriction"></param>
		/// <returns></returns>
		public static Z3Body GenerateWitness(
			CompositeBodyRestriction inputBodyRestriction,
			BodyTransform bodyTransform,
			CompositeBodyRestriction outputBodyRestriction)
		{
			var body = Z3Body.MkZ3Const();
			var transformedBody = bodyTransform.Transform(body);

			var expr1 = inputBodyRestriction.Evaluate(body);
			var expr2 = outputBodyRestriction.Evaluate(transformedBody);
			var expr = Z3.Context.MkAnd(expr1, expr2);

			var evaluatedJoints =
				JointTypeHelper.MergeJointTypeLists(
				inputBodyRestriction.GetJointTypes(),
				bodyTransform.GetJointTypes(),
				outputBodyRestriction.GetJointTypes());

			var checkResult = CheckStatus(expr);
			if (checkResult.Status == Status.SATISFIABLE)
			{
				var witness = CreateBodyWitness(
					body,
					checkResult.Model,
					evaluatedJoints,
					JointTypeHelper.CreateDefaultZ3Body());
				return witness;
			}
			else
			{
				return null;
			}
		}

		// Generates a body witness which satisfies two conditions
		// 1. It is within a range (angle threshold) of a transform from a start body
		// 2. It is within the considered restrictions
		public static Z3Target GenerateTarget(
			BodyTransform transform, 
			CompositeBodyRestriction restriction,
			Z3Body startBody,
			int angleThreshold)
		{
			var z3ConstBody = Z3Body.MkZ3Const();
			z3ConstBody.Norms = startBody.Norms;
			var transformedBody = transform.Transform(startBody);

			var joints = transform.GetJointTypes().Union(restriction.GetJointTypes()).ToList();
			var isNearExpr = z3ConstBody.IsAngleBetweenLessThan(transformedBody, joints, angleThreshold);
			var evaluateExpr = restriction.Evaluate(z3ConstBody);
			//var normsExpr = BodyRestrictionBuilder.EvaluateNorms(startBody, z3ConstBody);

			//var expr = Z3.Context.MkAnd(isNearExpr, evaluateExpr, normsExpr);
			//var expr = Z3.Context.MkAnd(evaluateExpr, normsExpr);
			var expr = Z3.Context.MkAnd(evaluateExpr, isNearExpr);

			var checkResult = CheckStatus(expr);
			if (checkResult.Status == Status.SATISFIABLE)
			{
				var witness = CreateBodyWitness(
					z3ConstBody, 
					checkResult.Model,
					restriction.GetJointTypes(), 
					startBody);

				var target = new Z3Target();
				target.Body = witness;
				target.RestrictedJoints = restriction.GetJointTypes();
				target.TransformedJoints = transform.GetJointTypes();

				foreach (var jointType in transform.GetJointTypes())
					target.Body.Joints[jointType] = transformedBody.Joints[jointType];

				return target;
			}
			else 
			{
				return null;    
			}
		}


        public static void WriteExprToDisk(BoolExpr expr, string exprName, string path)
        {
            // Convert the expr to a SMT-LIB formatted string 
            var exprArray = new BoolExpr[0];
            var output = Z3.Context.BenchmarkToSMTString(exprName, "QF_AUFLIRA", "unknown", "", exprArray, expr);

            if (File.Exists(path))
            {
            //    // Note that no lock is put on the 
            //    // file and the possibility exists 
            //    // that another process could do 
            //    // something with it between 
            //    // the calls to Exists and Delete.
                File.Delete(path);
            }

            //// Create the file. 
            using (FileStream fs = File.Create(path))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(output);
                // Write the SMT string to the file
                fs.Write(info, 0, info.Length);
            }
        }


		/// <summary>
		/// Checks expression status and returns expression status 
		/// plus a model if applicable.
		/// </summary>
		/// <param name="expr"></param>
		/// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
		public static SolverCheckResult CheckStatus(BoolExpr expr)
		{
			var result = new SolverCheckResult();
            var exprArray = new BoolExpr[0];
            Solver solver;
            bool bTimedOut = false;

            solver = Z3.Context.MkSolver("AUFLIRA");
            solver.Assert(expr);

            var status = solver.Check(); 
            Statistics stats = solver.Statistics; 

			result.Status = status;
            result.stats = stats;
            result.bTimedOut = bTimedOut; 

			if (status == Status.SATISFIABLE)
			{
				result.Model = solver.Model;
			}

			if (status == Status.UNKNOWN)
			{
				result.ReasonUnknown = solver.ReasonUnknown;
			}

            if (bTimedOut)
            {
                result.Status = Status.UNKNOWN; 
            }


			return result;
		}

		public static Z3Body CreateBodyWitness(
			Z3Body z3ConstCheckedBody,
			Model model,
			List<JointType> evaluatedJoints,
			Z3Body defaultBody)
		{
			var witness = new Z3Body();
			var jointTypes = EnumUtil.GetValues<JointType>();
			foreach (var jointType in jointTypes)
			{
				if (evaluatedJoints.Contains(jointType))
				{
					var joint = new Z3Point3D(
						model.Evaluate(z3ConstCheckedBody.Joints[jointType].X, true) as ArithExpr,
						model.Evaluate(z3ConstCheckedBody.Joints[jointType].Y, true) as ArithExpr,
						model.Evaluate(z3ConstCheckedBody.Joints[jointType].Z, true) as ArithExpr);
					witness.Joints.Add(jointType, joint);

					var norm = model.Evaluate(z3ConstCheckedBody.Norms[jointType]) as ArithExpr;

					// Check if norm is still an app (meaning it can be anything), then set it to be the default norm
					if (norm.ASTKind == Z3_ast_kind.Z3_APP_AST)
						witness.Norms.Add(jointType, defaultBody.Norms[jointType]);
					else
						witness.Norms.Add(jointType, norm);
				}
				else
				{
					witness.Joints.Add(jointType, defaultBody.Joints[jointType]);
					witness.Norms.Add(jointType, defaultBody.Norms[jointType]);
				}
			}

			return witness;
		}
	}

	/// <summary>
	/// Wraps the result of solver computation.
	/// </summary>
	public class SolverCheckResult
	{
		public Model Model { get; internal set; }

		public Status Status { get; internal set; }

        public Statistics stats { get; internal set; }

		public string ReasonUnknown { get; internal set; }

        public bool bTimedOut { get; internal set; }
	}

	public class SafetyException : Exception
	{
		public SafetyException(string message) : base(message) { }
	}

	public class PoseSafetyException : SafetyException
	{
		public Z3Body Body { get; private set; }
		public Pose Pose { get; private set; }
		public PoseSafetyException(string message) : base(message) { }
		public PoseSafetyException(string message, Pose pose, Z3Body body)
			: base(message)
		{
			this.Body = body;
			this.Pose = pose;
		}

		public override string ToString()
		{
			return string.Format("{0}\n\t{1}", this.Pose, this.Body);
		}
	}	
}
