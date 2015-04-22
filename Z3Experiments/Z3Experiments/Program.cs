using Microsoft.Z3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace PreposeGestures
{
	class Program
	{
        static void Main(string[] args)
        {
            var index = AppDomain.CurrentDomain.BaseDirectory.IndexOf("Z3Experiments") + "Z3Experiments".Length;
            var testPath = Path.Combine(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory.Substring(0, index),
                    "Z3Experiments"), "Tests");
            Contract.Assert(Directory.Exists(testPath), "Can't find " + testPath);
            var files = Directory.GetFiles(testPath, "*.app");
            var runner = new Runner(files.Select(f => Path.Combine(testPath, f)));

            runner.Run(); 

            return;
        }

    }

    internal class Runner
    {
        //public FileStream benchmarkOutputSafety = File.OpenWrite();
        //public FileStream benchmarkOutputConflict = new TextWriter(File.OpenWrite());
        private TextWriter swSafety;
        private TextWriter swConflict;
        private TextWriter swValidity;

        const bool DoSafetyChecking = false;
        const bool DoAmbiguityChecking = false;
        const bool DoValidityChecking = true; 
        internal IEnumerable<string> Files { get; private set; }

        internal Runner(IEnumerable<string> files)
        {
            swConflict = File.CreateText("benchmarkConflict.csv");
            swSafety = File.CreateText("benchmarkSafety.csv");
            swValidity = File.CreateText("benchmarkValidity.csv"); ;
            this.Files = files;

            swSafety.WriteLine("Name,NumTransforms,NumRestrictions,NumNegatedRestrictions,NumSteps, Time");
            swSafety.Flush();
        }

        internal void Run()
        {
            foreach (var file in this.Files)
            {
                ProcessFile(file);
            }

            swSafety.Flush();
            swConflict.Flush();
        }

        private List<Gesture> CreateInterpolatedGestures(Gesture ges)
        {
            List<Gesture> retList = new List<Gesture>();

            int maxInterpolateCount = ges.Steps.Count;
            string nameStem = ges.Name + "-interpolated-";
            for (int i = 0; i < maxInterpolateCount; i++ )
            {
                // Interpolated gestures are named for the number of execution steps they have
                string interpolatedGestureName = nameStem + i.ToString();

                Gesture interpolatedGesture = new Gesture(interpolatedGestureName); 

                // An interpolated gesture has all the poses of the original gesture
                // because we don't know in advance which poses will be used by the ExecutionSteps
                // CONSIDER: we could introspect on the execution steps and only copy over the needed poses
                //            -- need to check if that actually makes any difference for the formula we create 
                foreach (var pose in ges.DeclaredPoses)
                {
                    interpolatedGesture.DeclaredPoses.Add(pose);
                }

                // An interpolated gesture has a subset of the ExecutionSteps of the original gesture. 
                // If the original gesture has N steps, we create N interpolated gestures, each with 
                // a prefix of the steps of the original gesture. 
                for (int j = 0; j < i; j++)
                {
                    interpolatedGesture.Steps.Add(ges.Steps[i]); 
                }
                retList.Add(interpolatedGesture);
            }
                return retList; 
        }
 
        private App CreateNewAppWithInterpolatedGestures(App inputApp)
        {
            App retApp = new App(inputApp.Name);

            foreach (var ges in inputApp.Gestures)
            {
                var interpolatedGestures = CreateInterpolatedGestures(ges);
                foreach (var intGes in interpolatedGestures)
                {
                    retApp.Gestures.Add(intGes);
                }
                retApp.Gestures.Add(ges);
            }
            return retApp; 
        }

        private void ProcessFile(string file)
        {
            Console.WriteLine("Reading {0}", file);
            var rawApp = App.ReadApp(file);
            //        var app = CreateNewAppWithInterpolatedGestures(rawApp);
            var app = rawApp;

            if (DoValidityChecking)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                List<PoseSafetyException> validityExceptions = null;
                List<long> validityTimes = null;
                var isValid = Validity.IsInternallyValid(app, out validityExceptions, out validityTimes);

                stopwatch.Stop();

                Console.WriteLine("validity\t{0}", stopwatch.ElapsedMilliseconds);
                Console.WriteLine("restrictions:\t{0}", app.Gestures.Sum(r => r.RestrictionCount));

                int i = 0;

                foreach (var gesture in app.Gestures)
                {
                    Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
                        gesture.Name, gesture.TransformCount, gesture.RestrictionCount, gesture.NegatedRestrictionCount, gesture.Steps.Count, validityTimes[i]);

                    swValidity.WriteLine("{0},{1},{2},{3},{4},{5}",
                        gesture.Name,
                        gesture.TransformCount,
                        gesture.RestrictionCount,
                        gesture.NegatedRestrictionCount,
                        gesture.Steps.Count,
                        validityTimes[i]);
                    i++;
                    swValidity.Flush(); 
                }

            }

            if (DoSafetyChecking)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                // Test if app gestures are safe
                List<PoseSafetyException> safetyExceptions = null;
                List<long> safetyTimes = null;
                var isSafe = Safety.IsWithinDefaultSafetyRestrictions(app, out safetyExceptions, out safetyTimes);
                if (!isSafe)
                {
                    Console.WriteLine("Safety exceptions:");
                    foreach (var ex in safetyExceptions)
                    {
                        Console.WriteLine("\t{0}", ex);
                    }
                    Console.WriteLine();
                }
                stopwatch.Stop();
                Console.WriteLine("safety\t{0}", stopwatch.ElapsedMilliseconds);
                Console.WriteLine("restrictions:\t{0}", app.Gestures.Sum(r => r.RestrictionCount));
                int i = 0;
                foreach (var gesture in app.Gestures)
                {
                    Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
                        gesture.Name, gesture.TransformCount, gesture.RestrictionCount, gesture.NegatedRestrictionCount, gesture.Steps.Count, safetyTimes[i]);

                    swSafety.WriteLine("{0},{1},{2},{3},{4},{5}",
                        gesture.Name,
                        gesture.TransformCount,
                        gesture.RestrictionCount,
                        gesture.NegatedRestrictionCount,
                        gesture.Steps.Count,
                        safetyTimes[i]);

                    i++;
                }
            }

            if (DoAmbiguityChecking)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                uint numConflicts = 0;
                // Test if app gestures present conflicts
                List<PairwiseConflictException> conflictExceptions = null;
                List<PreposeGestures.Ambiguity.AmbiguityTime> ambiguityTimes;
                var hasConflicts = Ambiguity.HasPairwiseConflicts(app,
                    out conflictExceptions,
                    out ambiguityTimes);
                if (hasConflicts)
                {
                    foreach (var ex in conflictExceptions)
                    {
                        numConflicts++;
                    }
                }
                stopwatch.Stop();
                long elapsedTime = stopwatch.ElapsedMilliseconds;
                var numGestures = app.Gestures.Count();
                var numPoses = 0;
                var numTotalRestrictions = app.Gestures.Sum(r => r.RestrictionCount);
                var numTotalNegatedRestrictions = app.Gestures.Sum(r => r.NegatedRestrictionCount);
                var numTotalTransforms = app.Gestures.Sum(r => r.TransformCount);
                var numTotalTransformedJoints = app.Gestures.Sum(r => r.DistinctTransformedJointsCount);
                var numTotalSteps = app.Gestures.Sum(r => r.Steps.Count());
                foreach (var ges in app.Gestures)
                {
                    numPoses += ges.DeclaredPoses.Count();
                }


                swConflict.WriteLine();
                swConflict.WriteLine("Name,ElapsedTime,NumGestures,NumPoses,NumTotalRestrictions,NumTotalNegatedRestrictions,NumTotalTransforms, NumTotalTransformedJoints,NumSteps,NumConflicts");
                swConflict.Flush();


                swConflict.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                    app.Name,
                    numGestures,
                    numPoses,
                    numTotalRestrictions,
                    numTotalNegatedRestrictions,
                    numTotalTransforms,
                    numTotalTransformedJoints,
                    numTotalSteps,
                    numConflicts,

                    elapsedTime
                    );
                swConflict.Flush();

                swConflict.WriteLine();
                swConflict.WriteLine("NameG1*NameG2,NumPosesG1,NumRestrictionsG1,NumNegatedRestrictionsG1,NumTransformsG1,NumStepsG1,NumDistinctJointsG1,NumPosesG2,NumRestrictionsG2,NumNegatedRestrictionsG2,NumTransformsG2,NumStepsG2,NumDistinctJointsG2,Conflict,NumPivots,ElapsedTime");
                swConflict.Flush();

                foreach (var timing in ambiguityTimes)
                {
                    Console.WriteLine("Logging results for {0}X{1}", timing.Gesture1.Name, timing.Gesture2.Name);

                    uint numPivots = 0;
                    long time = timing.Time;

                    if (timing.CheckResult.bTimedOut)
                    {
                        time = -1; // Timed out goes to -1
                    }

                    if (timing.CheckResult.stats != null)
                    {
                        if (timing.CheckResult.stats["pivots"] != null)
                        {
                            numPivots = timing.CheckResult.stats["pivots"].UIntValue;
                        }
                    }

                    var times = new object[]{
                        string.Format("{0}*{1}", timing.Gesture1.Name, timing.Gesture2.Name),
                        timing.Gesture1.DeclaredPoses.Count, 
                        timing.Gesture1.RestrictionCount,
                        timing.Gesture1.NegatedRestrictionCount,
                        timing.Gesture1.TransformCount,
                        timing.Gesture1.Steps.Count,
                        timing.Gesture1.DistinctTransformedJointsCount,
                        timing.Gesture2.DeclaredPoses.Count, 
                        timing.Gesture2.RestrictionCount,
                        timing.Gesture2.NegatedRestrictionCount,
                        timing.Gesture2.TransformCount,
                        timing.Gesture2.Steps.Count,
                        timing.Gesture2.DistinctTransformedJointsCount,
                        timing.Conflict,
                        numPivots,
                        time,
                    };

                    Statistics stats = timing.CheckResult.stats;

                    string output = string.Join(",", times);
                    swConflict.WriteLine("{0}", output);
                    swConflict.Flush();
                }
            }
        }

        // Simple Runtime Test Codes
        public static class Test
        {
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

            static ArithExpr CalcApproximateCoordFromManhattanToEuclidianSystem(
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
                ArithExpr sqrt1div3 = Z3Math.Real(0.57735026918962576450914878050196);

                // The remaining common length will be weighted by this
                // This way for example a (1, 1, 0) vector will become
                // A (0.7, 0.7, 0.7) with norm near to 1
                ArithExpr sin45 = Z3Math.Real(0.70710678118654752440084436210485);

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

            public static void Run()
            {
                Z3Point3D constPoint = Z3Point3D.MkZ3Const("const"); // ("const X", "const Y", "const Z")

                Z3Point3D normalized = new Z3Point3D();

                ArithExpr higherCoord =
                Z3Math.Max(
                Z3Math.Max(
                Z3Math.Abs(constPoint.X),
                Z3Math.Abs(constPoint.Y)),
                Z3Math.Abs(constPoint.Z));

                normalized.X = Z3.Context.MkDiv(constPoint.X, constPoint.Y);
                normalized.Y = Z3Math.One;//Z3.Context.MkDiv(constPoint.Y, higherCoord);
                normalized.Z = Z3.Context.MkDiv(constPoint.Z, constPoint.Y);

                normalized.X = CalcApproximateCoordFromManhattanToEuclidianSystem(normalized.X, normalized.Y, normalized.Z);
                normalized.Y = CalcApproximateCoordFromManhattanToEuclidianSystem(normalized.Y, normalized.X, normalized.Z);
                normalized.Z = CalcApproximateCoordFromManhattanToEuclidianSystem(normalized.Z, normalized.Y, normalized.X);

                Z3Point3D up = Z3Point3D.DirectionPoint(Direction.Up); // (0, 1, 0)
                Z3Point3D distVec = normalized - up;

                ArithExpr distance =
                    Max(
                    Abs(CalcApproximateCoordFromManhattanToEuclidianSystem(distVec.X, distVec.Y, distVec.Z)),
                    Abs(CalcApproximateCoordFromManhattanToEuclidianSystem(distVec.Y, distVec.X, distVec.Z)),
                    Abs(CalcApproximateCoordFromManhattanToEuclidianSystem(distVec.Z, distVec.Y, distVec.X)));

                BoolExpr expr = Z3.Context.MkLt(distance, Z3.Context.MkReal(1, 2));


                Solver solver = Z3.Context.MkSolver();
                solver.Assert(expr);
                Status status = solver.Check();
                Statistics stats = solver.Statistics;

                switch (status)
                {
                    case Status.UNKNOWN:
                        Console.WriteLine("Solver check for witness returned Status.UNKNOWN because: " + solver.ReasonUnknown);
                        throw new ArgumentException("Test Failed Expception");

                    case Status.UNSATISFIABLE:
                        Console.WriteLine("There is no valid witness for " + expr);
                        throw new ArgumentException("Test Failed Expception");

                    case Status.SATISFIABLE:
                        Console.WriteLine("OK, model: " + solver.Model);
                        break;
                }
            }
        }

        public static void ProveExample2(Context ctx)
        {
            Console.WriteLine("ProveExample2");

            /* declare function g */
            Sort I = ctx.IntSort;

            FuncDecl g = ctx.MkFuncDecl("g", I, I);

            /* create x, y, and z */
            IntExpr x = ctx.MkIntConst("x");
            IntExpr y = ctx.MkIntConst("y");
            IntExpr z = ctx.MkIntConst("z");

            /* create gx, gy, gz */
            Expr gx = ctx.MkApp(g, x);
            Expr gy = ctx.MkApp(g, y);
            Expr gz = ctx.MkApp(g, z);

            /* create zero */
            IntExpr zero = ctx.MkInt(0);

            /* assert not(g(g(x) - g(y)) = g(z)) */
            ArithExpr gx_gy = ctx.MkSub((IntExpr)gx, (IntExpr)gy);
            Expr ggx_gy = ctx.MkApp(g, gx_gy);
            BoolExpr eq = ctx.MkEq(ggx_gy, gz);
            BoolExpr c1 = ctx.MkNot(eq);

            /* assert x + z <= y */
            ArithExpr x_plus_z = ctx.MkAdd(x, z);
            BoolExpr c2 = ctx.MkLe(x_plus_z, y);

            /* assert y <= x */
            BoolExpr c3 = ctx.MkLe(y, x);

            /* prove z < 0 */
            BoolExpr f = ctx.MkLt(z, zero);
            Console.WriteLine("prove: not(g(g(x) - g(y)) = g(z)), x + z <= y <= x implies z < 0");
            Prove(ctx, f, c1, c2, c3);

            /* disprove z < -1 */
            IntExpr minus_one = ctx.MkInt(-1);
            f = ctx.MkLt(z, minus_one);
            Console.WriteLine("disprove: not(g(g(x) - g(y)) = g(z)), x + z <= y <= x implies z < -1");
            Disprove(ctx, f, c1, c2, c3);
        }

        public static void TupleExample(Context ctx)
        {
            Console.WriteLine("TupleExample");

            Sort int_type = ctx.IntSort;
            TupleSort tuple = ctx.MkTupleSort(
             ctx.MkSymbol("mk_tuple"),         // name of tuple constructor
             new Symbol[] { ctx.MkSymbol("first"), ctx.MkSymbol("second") },  // names of projection operators
             new Sort[] { int_type, int_type } // types of projection operators         
                );
            FuncDecl first = tuple.FieldDecls[0];  // declarations are for projections
            FuncDecl second = tuple.FieldDecls[1];
            Expr x = ctx.MkConst("x", int_type);
            Expr y = ctx.MkConst("y", int_type);
            Expr n1 = tuple.MkDecl[x, y];
            Expr n2 = first[n1];
            BoolExpr n3 = ctx.MkEq(x, n2);
            Console.WriteLine("Tuple example: {0}", n3);
            Prove(ctx, n3);
        }

        static void Prove(Context ctx, BoolExpr f, params BoolExpr[] assumptions)
        {
            Console.WriteLine("Proving: " + f);
            Solver s = ctx.MkSolver();
            foreach (BoolExpr a in assumptions)
                s.Assert(a);
            s.Assert(ctx.MkNot(f));
            Status q = s.Check();

            switch (q)
            {
                case Status.UNKNOWN:
                    Console.WriteLine("Unknown because: " + s.ReasonUnknown);
                    break;
                case Status.SATISFIABLE:
                    throw new ArgumentException("Test Failed Expception");
                case Status.UNSATISFIABLE:
                    Console.WriteLine("OK, proof: " + s.Proof);
                    break;
            }
        }

        static void Disprove(Context ctx, BoolExpr f, params BoolExpr[] assumptions)
        {
            Console.WriteLine("Disproving: " + f);
            Solver s = Z3.Context.MkSolver();
            foreach (BoolExpr a in assumptions)
                s.Assert(a);
            s.Assert(ctx.MkNot(f));
            Status q = s.Check();

            switch (q)
            {
                case Status.UNKNOWN:
                    Console.WriteLine("Unknown because: " + s.ReasonUnknown);
                    break;
                case Status.SATISFIABLE:
                    Console.WriteLine("OK, model: " + s.Model);
                    break;
                case Status.UNSATISFIABLE:
                    throw new ArgumentException("Test Failed Expception");
            }
        }

        
    }
}