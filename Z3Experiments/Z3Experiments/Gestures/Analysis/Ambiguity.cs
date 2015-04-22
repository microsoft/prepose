using Microsoft.Z3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PreposeGestures
{
	public class Ambiguity
	{
	//	public IList<BodyTransform> Transforms { get; private set; }

	//	public Ambiguity(App app)
	//	{
	//		this.Transforms = GetTransforms(app);
	//	}

	//	private Ambiguity(IList<BodyTransform> transforms, IList<string> names = null)
	//	{
	//		Contract.Requires(names == null || names.Count == transforms.Count);
	//		this.Transforms = new List<BodyTransform>(transforms);
	//	}

	//	private static IList<BodyTransform> GetTransforms(App app)
	//	{
	//		throw new NotImplementedException();
	//	}

	//	/// <summary>
	//	/// Checks if the set has duplicate transforms.
	//	/// </summary>
	//	/// <returns></returns>
	//	public bool HasDuplicates(IList<string> names = null)
	//	{
	//		for (int i = 0; i < this.Transforms.Count; ++i)
	//		{
	//			// these are ordered pairs of transforms (i,j) where i<j
	//			for (int j = i + 1; j < Transforms.Count; ++j)
	//			{
	//				if (Transforms[i].Equals(Transforms[j]))
	//				{
	//					Console.WriteLine("Transforms {0} and {1} are equals.", 
	//						i, j);
	//					Console.WriteLine("Transform {0}",
	//						names != null ? names[i] : Transforms[i].ToString());
	//					Console.WriteLine("Transform {0}",
	//						names != null ? names[j] : Transforms[j].ToString());

	//					return true;
	//				}
	//			}
	//		}

	//		return false;
	//	}

	//	private IEnumerable<KeyValuePair<string, BodyTransform>> GetPairs() {
	//		for (int i = 0; i < this.Transforms.Count; ++i)
	//		{
	//			// these are ordered pairs of transforms (i,j) where i<j
	//			for (int j = i + 1; j < Transforms.Count; ++j)
	//			{
	//				var result = new KeyValuePair<string,BodyTransform>
	//					(
	//						string.Format("({0},{1})",i,j), 
	//						Transforms[i].Compose(Transforms[j])
	//					);
	//				yield return result;
	//			}
	//		}
	//	}

	//	public bool HasConflictingPairs()
	//	{
	//		var pairs = GetPairs();
	//		IList<BodyTransform> transforms = pairs.Select(p => p.Value).ToList();
	//		IList<string> names = pairs.Select(p => p.Key).ToList();
	//		var pairAmbiguity = new Ambiguity(transforms, names);
	//		return pairAmbiguity.HasDuplicates();
	//	}


        public class AmbiguityTime
        {
            public Gesture Gesture1 { get; internal set; }
            public Gesture Gesture2 { get; internal set; }

            public long Time { get; internal set; }

            public bool Conflict { get; internal set; }

            public SolverCheckResult CheckResult { get; internal set; }
        }

        public static bool DumpZ3Constraints = true; 

        /// <summary>
        /// Check for pairwise ambiguity
        /// </summary>
        /// <returns></returns>
        public static bool HasPairwiseConflicts(App app, out List<PairwiseConflictException> allExceptions, out List<AmbiguityTime> ambiguityTimes, int precision = 15)
        {
            List<Gesture> conflictGestures = (List<Gesture>)app.Gestures;

            bool result = false;
            allExceptions = new List<PairwiseConflictException>();
            ambiguityTimes = new List<AmbiguityTime>();

            for (int i = 0; i < conflictGestures.Count - 1; i++)
            {
                for (int j = i + 1; j < conflictGestures.Count; j++)
                {
                    var gesture1 = conflictGestures[i];
                    var gesture2 = conflictGestures[j];

                    // Create const input body
                    var input = Z3Body.MkZ3Const();

                    var allJoints = EnumUtil.GetValues<JointType>().ToList();

                    Z3Body transformed1 = null;
                    Z3Body transformed2 = null;

                    BoolExpr evaluation1 = Z3Math.True;
                    BoolExpr evaluation2 = Z3Math.True;

                    // Pass input through both gestures
                    gesture1.FinalResult(input, out transformed1, out evaluation1);
                    gesture2.FinalResult(input, out transformed2, out evaluation2);

                    // Check if it is possible that both outputs are equals
                    // This is performed by checking if is possible that all expressions are true

                    var isNearExpr = transformed1.IsNearerThan(
                    transformed2, precision);
                    var expr = Z3.Context.MkAnd(isNearExpr, evaluation1, evaluation2);

                    // If we are dumping Z3 constraints, then convert the expression to a SMTLIB formatted string
                    // and dump it to disk. Note this is not included in the timing for the individual pair of gestures,
                    // but it _is_ included in the timing for the app overall. 
                    if (DumpZ3Constraints)
                    {
                        string exprName = String.Join("X", gesture1.Name, gesture2.Name);
                        string exprPath = exprName + ".smt2";
                        Z3AnalysisInterface.WriteExprToDisk(expr, exprName, exprPath);
                    }

                    // Check if we have an ambiguity conflict. Record the time it takes. 
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var checkResult = Z3AnalysisInterface.CheckStatus(expr);
                    stopwatch.Stop();


                    if (checkResult.Status == Status.SATISFIABLE)
                    {
                        var witness = Z3AnalysisInterface.CreateBodyWitness(
                            input,
                            checkResult.Model,
                            allJoints,
                            JointTypeHelper.CreateDefaultZ3Body());

                        var exception = new PairwiseConflictException(
                            "Conflict detected between pair of gestures",
                            gesture1,
                            gesture2,
                            witness);

                        allExceptions.Add(exception);

                        result = true;
                    }
                    // TODO the witness here should exist, this case shouldn't be needed
                    else if (checkResult.Status == Status.UNKNOWN)
                    {
                        var witness = JointTypeHelper.CreateDefaultZ3Body();

                        var exception = new PairwiseConflictException(
                            "Conflict detected between pair of gestures, the reason is unknown",
                            gesture1,
                            gesture2,
                            witness);

                        allExceptions.Add(exception);

                        result = true;
                    }

                    ambiguityTimes.Add(new AmbiguityTime
                    {
                        Gesture1 = gesture1,
                        Gesture2 = gesture2,
                        Time = stopwatch.ElapsedMilliseconds,
                        Conflict = result,
                        CheckResult = checkResult
                    });


                }
            }

            return result;
        }
	}
    public class PairwiseConflictException : Exception
    {
        public Z3Body Witness { get; private set; }
        public Gesture Gesture1 { get; private set; }
        public Gesture Gesture2 { get; private set; }
        public PairwiseConflictException(string message) : base(message) { }
        public PairwiseConflictException(string message, Gesture gesture1, Gesture gesture2, Z3Body body)
            : base(message)
        {
            this.Witness = body;
            this.Gesture1 = gesture1;
            this.Gesture2 = gesture2;
        }

        public override string ToString()
        {
            return string.Format("Gesture 1: {0}, gesture 2: {1}, witness: {2}",
                this.Gesture1.Name, this.Gesture2.Name, this.Witness);
        }
    }
}
