using Microsoft.Z3;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace PreposeGestures
{
    /// <summary>
    /// Various safety checks.
    /// </summary>
    internal class Safety
    {
        public static bool IsWithinDefaultSafetyRestrictions(
            App app,
            out List<PoseSafetyException> allExceptions, 
            out List<long> elapsedTimes)
        {
            allExceptions = new List<PoseSafetyException>();
            elapsedTimes = new List<long>();
            var result = true;
            foreach (var gesture in app.Gestures)
            {
                List<PoseSafetyException> exceptions = null;
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                if (!IsWithinDefaultSafetyRestrictions(gesture, out exceptions))
                {
                    result = false;
                    Contract.Assert(exceptions != null);
                    allExceptions.AddRange(exceptions);
                }
                stopwatch.Stop();
                elapsedTimes.Add(stopwatch.ElapsedMilliseconds);
            }
            return result;
        }

        /// <summary>
        /// Checks if the pose is within default safety 
        /// restrictions when the transform and restrictions 
        /// are applied.
        /// </summary>
        /// <returns>True if it's safe</returns>
        public static bool IsWithinDefaultSafetyRestrictions(Gesture gesture, out List<PoseSafetyException> exceptions)
        {
            bool result = true;
            exceptions = new List<PoseSafetyException>();
            foreach (var step in gesture.Steps)
            {
                var pose = step.Pose;
                Z3Body witness = null;
                if (!Safety.IsWithinSafetyRestrictions(pose, out witness))
                {
                    var exception = new PoseSafetyException(
                        "Default safety violation", pose, witness
                        );
                    exceptions.Add(exception);
                    result = false;
                }
            }

            return result;
        }


        /// <summary>
        /// Checks if the pose is within default safety 
        /// restrictions when the transform and restrictions 
        /// are applied.
        /// </summary>
        /// <returns>True if it's safe</returns>
        public static bool IsWithinSafetyRestrictions(Pose pose, out Z3Body witness)
        {
            Z3Body input = Z3Body.MkZ3Const();
            Z3Body transformed = pose.Transform.Transform(input);

            IBodyRestriction safe = Safety.DefaultSafetyRestriction();

            BoolExpr inputSafe = safe.Evaluate(input);
            BoolExpr transformedRestricted = pose.Restriction.Evaluate(transformed);

            // Try to generate a unsafe witness using the transform
            BoolExpr outputUnsafe = Z3.Context.MkNot(safe.Evaluate(transformed));

            // Put together all expressions and search for unsat
            BoolExpr expr = Z3.Context.MkAnd(inputSafe, transformedRestricted, outputUnsafe);

            SolverCheckResult solverResult = Z3AnalysisInterface.CheckStatus(expr);

            if (solverResult.Status == Status.SATISFIABLE)
            {
                //Z3Body 
                witness =
                    Z3AnalysisInterface.CreateBodyWitness(
                    transformed,
                    solverResult.Model,
                    pose.GetAllJointTypes(),
                    JointTypeHelper.CreateDefaultZ3Body());

                return false;
            }
            else if (solverResult.Status == Status.UNKNOWN)
            {
                //Z3Body 
                witness = JointTypeHelper.CreateDefaultZ3Body();

                return false;
            }
            else
            {
                Contract.Assert(solverResult.Status == Status.UNSATISFIABLE);
                witness = null;
                return true;
            }
        }


        /// <summary>
        /// This is a basic safety check to make sure we don't break any bones.
        /// </summary>
        /// <returns>True if safe</returns>
        public static IBodyRestriction DefaultSafetyRestriction()
        {
            var result = new CompositeBodyRestriction();

            int inclinationThreshold = 45;

            // Head
            // Make sure neck is not inclinated beyond the threshold            
            var head = new SimpleBodyRestriction(body =>
            {
                Z3Point3D up = new Z3Point3D(0, 1, 0);
                BoolExpr expr1 = body.Joints[JointType.Head].IsAngleBetweenLessThan(up, inclinationThreshold);
                BoolExpr expr2 = body.Joints[JointType.Neck].IsAngleBetweenLessThan(up, inclinationThreshold);
                BoolExpr expr = Z3.Context.MkAnd(expr1, expr2);

                return expr;
            });
            result.And(head);

            // Spine
            // Make sure spine is not inclinated beyond the threshold
            var spine = new SimpleBodyRestriction(body =>
            {
                Z3Point3D up = new Z3Point3D(0, 1, 0);
                BoolExpr expr1 = body.Joints[JointType.SpineMid].IsAngleBetweenLessThan(up, inclinationThreshold);
                BoolExpr expr2 = body.Joints[JointType.SpineShoulder].IsAngleBetweenLessThan(up, inclinationThreshold);
                BoolExpr expr3 =
                    body.Joints[JointType.SpineMid].IsAngleBetweenLessThan(
                    body.Joints[JointType.SpineShoulder], inclinationThreshold);
                BoolExpr expr = Z3.Context.MkAnd(expr1, expr2, expr3);

                return expr;
            });
            result.And(spine);

            // Shoulders
            // Make sure shoulders are not bent            
            var shoulders = new SimpleBodyRestriction(body =>
            {
                BoolExpr expr =
                    body.Joints[JointType.SpineMid].IsAngleBetweenGreaterThan(
                    body.Joints[JointType.SpineShoulder], 120);

                return expr;
            });
            result.And(shoulders);

            // Elbows
            // Make sure elbows are not behind the back
            // And also that they are not on the top/back sub-space
            var elbows = new SimpleBodyRestriction(body =>
            {
                BoolExpr exprRight1 =
                    Z3.Context.MkNot(
                    Z3.Context.MkAnd(
                    Z3.Context.MkLt(body.Joints[JointType.ElbowRight].Z, Z3Math.Zero),
                    Z3.Context.MkLt(body.Joints[JointType.ElbowRight].X, Z3Math.Zero)));

                BoolExpr exprRight2 =
                    Z3.Context.MkNot(
                    Z3.Context.MkAnd(
                    Z3.Context.MkLt(body.Joints[JointType.ElbowRight].Z, Z3Math.Zero),
                    Z3.Context.MkGt(body.Joints[JointType.ElbowRight].Y, Z3Math.Zero)));

                BoolExpr exprLeft1 =
                    Z3.Context.MkNot(
                    Z3.Context.MkAnd(
                    Z3.Context.MkLt(body.Joints[JointType.ElbowLeft].Z, Z3Math.Zero),
                    Z3.Context.MkGt(body.Joints[JointType.ElbowLeft].X, Z3Math.Zero)));

                BoolExpr exprLeft2 =
                    Z3.Context.MkNot(
                    Z3.Context.MkAnd(
                    Z3.Context.MkLt(body.Joints[JointType.ElbowLeft].Z, Z3Math.Zero),
                    Z3.Context.MkGt(body.Joints[JointType.ElbowLeft].Y, Z3Math.Zero)));

                BoolExpr expr = Z3.Context.MkAnd(exprLeft1, exprLeft2, exprRight1, exprRight2);

                return expr;
            });
            result.And(elbows);

            // Wrists
            // Make sure the inclination of wrists towards the back is not higher than the inclinatin of the elbows
            // unless elbows are up or wrists are directed to torso
            // TODO

            // Hips
            // Make sure hips are aligned with the shoulders or at lest within the range
            var hips = new SimpleBodyRestriction(body =>
            {
                Z3Point3D shouldersSum =
                    body.Joints[JointType.ShoulderLeft].GetInverted() +
                    body.Joints[JointType.ShoulderRight];

                Z3Point3D hipsSum =
                    body.Joints[JointType.HipLeft].GetInverted() +
                    body.Joints[JointType.HipRight];

                BoolExpr expr = shouldersSum.IsAngleBetweenLessThan(hipsSum, 45);
                return expr;
            });
            result.And(hips);

            // Legs
            // Make sure legs are not higher than threshold
            var legs = new SimpleBodyRestriction(body =>
            {
                BoolExpr expr1 = Z3.Context.MkLt(body.Joints[JointType.KneeLeft].Y, Z3Math.Real(0));
                BoolExpr expr2 = Z3.Context.MkLt(body.Joints[JointType.KneeRight].Y, Z3Math.Real(0));
                BoolExpr expr = Z3.Context.MkAnd(expr1, expr2);

                return expr;
            });
            result.And(legs);

            // Ankles
            // Make sure ankles are not inclinated up more than knees
            // unless ankles are pointing back
            // TODO

            return result;
        }
    }
}
