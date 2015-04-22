using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.Kinect.GestureRecognizer
{
	using System.IO;
	using System.Windows;
	using System.Windows.Media;
	using Microsoft.Kinect;
	using System.Windows.Media.Media3D;

	public partial class TreeBodyPart
	{
		private void HelperJointToXYZ(Dictionary<JointType, Vector3D> vectors3D, JointType jointType, ref double x, ref double y, ref double z)
		{
			x = vectors3D[jointType].X;
			y = vectors3D[jointType].Y;
			z = vectors3D[jointType].Z;
		}

		public TreeBodyPart(Dictionary<JointType, Vector3D> vectors3D, Side s, BodyPart bodyPart, Brush b)
		{
			children = new TreeBodyPart[BodyPartUtil.getRank(bodyPart)];
			symbol = bodyPart;
			this.s = s;
			switch (bodyPart)
			{
				case BodyPart.SpineMid:
					{
						HelperJointToXYZ(vectors3D, JointType.SpineMid, ref x, ref y, ref z);
						s = Side.C;
						children[0] = new TreeBodyPart(vectors3D, s, BodyPart.SpineShoulder, b);
						children[1] = new TreeBodyPart(vectors3D, s, BodyPart.SpineBase, b);
						break;
					}
				case BodyPart.SpineShoulder:
					{
						switch (s)
						{
							case Side.C:
								{
									HelperJointToXYZ(vectors3D, JointType.SpineShoulder, ref x, ref y, ref z);
									s = Side.C;
									children[0] = new TreeBodyPart(vectors3D, Side.L, BodyPart.Shoulder, b);
									children[1] = new TreeBodyPart(vectors3D, Side.C, BodyPart.Neck, b);
									children[2] = new TreeBodyPart(vectors3D, Side.R, BodyPart.Shoulder, b);
									break;
								}

						} break;
					}
				case BodyPart.Shoulder:
					{
						switch (s)
						{
							case Side.R:
								{
									HelperJointToXYZ(vectors3D, JointType.ShoulderRight, ref x, ref y, ref z);
									children[0] = new TreeBodyPart(vectors3D, s, BodyPart.Elbow, b);
									break;
								}
							case Side.L:
								{
									HelperJointToXYZ(vectors3D, JointType.ShoulderLeft, ref x, ref y, ref z);
									children[0] = new TreeBodyPart(vectors3D, s, BodyPart.Elbow, b); break;
								}
						}
						break;
					}
				case BodyPart.Elbow:
					{
						switch (s)
						{
							case Side.R:
								{
									HelperJointToXYZ(vectors3D, JointType.ElbowRight, ref x, ref y, ref z);
									children[0] = new TreeBodyPart(vectors3D, s, BodyPart.Wrist, b);
									break;
								}
							case Side.L:
								{
									HelperJointToXYZ(vectors3D, JointType.ElbowLeft, ref x, ref y, ref z);
									children[0] = new TreeBodyPart(vectors3D, s, BodyPart.Wrist, b);
									break;
								}
							default: throw new Exception();
						}
						break;
					}
				case BodyPart.Wrist:
					{
						switch (s)
						{
							case Side.R:
								{
									HelperJointToXYZ(vectors3D, JointType.WristRight, ref x, ref y, ref z);
									children[0] = new TreeBodyPart(vectors3D, s, BodyPart.Hand, b);
									break;
								}
							case Side.L:
								{
									HelperJointToXYZ(vectors3D, JointType.WristLeft, ref x, ref y, ref z);
									children[0] = new TreeBodyPart(vectors3D, s, BodyPart.Hand, b);
									break;
								}
							default: throw new Exception();
						}
						break;
					}
				case BodyPart.Hand:
					{
						switch (s)
						{
							case Side.R:
								{
									HelperJointToXYZ(vectors3D, JointType.HandRight, ref x, ref y, ref z);
									children[0] = new TreeBodyPart(vectors3D, s, BodyPart.HandTip, b);
									children[1] = new TreeBodyPart(vectors3D, s, BodyPart.Thumb, b);
									break;
								}
							case Side.L:
								{
									HelperJointToXYZ(vectors3D, JointType.HandLeft, ref x, ref y, ref z);
									children[0] = new TreeBodyPart(vectors3D, s, BodyPart.HandTip, b);
									children[1] = new TreeBodyPart(vectors3D, s, BodyPart.Thumb, b);
									break;
								}
							default: throw new Exception();
						}
						break;
					}
				case BodyPart.HandTip:
					{
						switch (s)
						{
							case Side.R:
								{
									HelperJointToXYZ(vectors3D, JointType.HandTipRight, ref x, ref y, ref z);
									break;
								}
							case Side.L:
								{
									HelperJointToXYZ(vectors3D, JointType.HandTipLeft, ref x, ref y, ref z);
									break;
								}
							default: throw new Exception();
						}
						break;
					}
				case BodyPart.Thumb:
					{
						switch (s)
						{
							case Side.R:
								{
									HelperJointToXYZ(vectors3D, JointType.ThumbRight, ref x, ref y, ref z);
									break;
								}
							case Side.L:
								{
									HelperJointToXYZ(vectors3D, JointType.ThumbLeft, ref x, ref y, ref z);
									break;
								}
							default: throw new Exception();
						}
						break;
					}
				case BodyPart.Neck:
					{
						switch (s)
						{
							case Side.C:
								{
									HelperJointToXYZ(vectors3D, JointType.Neck, ref x, ref y, ref z);
									children[0] = new TreeBodyPart(vectors3D, s, BodyPart.Head, b);
									break;
								}
							default: throw new Exception();
						}
						break;
					}
				case BodyPart.Head:
					{
						switch (s)
						{
							case Side.C:
								{
									HelperJointToXYZ(vectors3D, JointType.Head, ref x, ref y, ref z);
									break;
								}
							default: throw new Exception();
						}
						break;
					}
				case BodyPart.SpineBase:
					{
						switch (s)
						{
							case Side.C:
								{
									HelperJointToXYZ(vectors3D, JointType.SpineBase, ref x, ref y, ref z);
									children[0] = new TreeBodyPart(vectors3D, Side.L, BodyPart.Hip, b);
									children[1] = new TreeBodyPart(vectors3D, Side.R, BodyPart.Hip, b);
									break;
								}
						}
						break;
					}
				case BodyPart.Hip:
					{
						switch (s)
						{
							case Side.R:
								{
									HelperJointToXYZ(vectors3D, JointType.HipRight, ref x, ref y, ref z);
									children[0] = new TreeBodyPart(vectors3D, s, BodyPart.Knee, b);
									break;
								}
							case Side.L:
								{
									HelperJointToXYZ(vectors3D, JointType.HipLeft, ref x, ref y, ref z);
									children[0] = new TreeBodyPart(vectors3D, s, BodyPart.Knee, b);
									break;
								}
						}
						break;
					}
				case BodyPart.Knee:
					{
						switch (s)
						{
							case Side.R:
								{
									HelperJointToXYZ(vectors3D, JointType.KneeRight, ref x, ref y, ref z);
									children[0] = new TreeBodyPart(vectors3D, s, BodyPart.Ankle, b);
									break;
								}
							case Side.L:
								{
									HelperJointToXYZ(vectors3D, JointType.KneeLeft, ref x, ref y, ref z);
									children[0] = new TreeBodyPart(vectors3D, s, BodyPart.Ankle, b);
									break;
								}
							default: throw new Exception();
						}
						break;
					}
				case BodyPart.Ankle:
					{
						switch (s)
						{
							case Side.R:
								{
									HelperJointToXYZ(vectors3D, JointType.AnkleRight, ref x, ref y, ref z);
									children[0] = new TreeBodyPart(vectors3D, s, BodyPart.Foot, b);
									break;
								}
							case Side.L:
								{
									HelperJointToXYZ(vectors3D, JointType.AnkleLeft, ref x, ref y, ref z);
									children[0] = new TreeBodyPart(vectors3D, s, BodyPart.Foot, b);
									break;
								}
							default: throw new Exception();
						}
						break;
					}
				case BodyPart.Foot:
					{
						switch (s)
						{
							case Side.R:
								{
									HelperJointToXYZ(vectors3D, JointType.FootRight, ref x, ref y, ref z);
									break;
								}
							case Side.L:
								{
									HelperJointToXYZ(vectors3D, JointType.FootLeft, ref x, ref y, ref z);
									break;
								}
							default: throw new Exception();
						}
						break;
					}
				default: throw new Exception();
			}

			this.langsLabel = new HashSet<string>();
			ComputeLabel();
		}

		public JointType GetJointType()
		{
			JointType result = JointType.SpineMid;
			if (symbol == BodyPart.SpineMid) result = JointType.SpineMid;
			if (symbol == BodyPart.SpineShoulder) result = JointType.SpineShoulder;
			if (symbol == BodyPart.Shoulder && s == Side.L) result = JointType.ShoulderLeft;
			if (symbol == BodyPart.Shoulder && s == Side.R) result = JointType.ShoulderRight;
			if (symbol == BodyPart.Elbow && s == Side.L) result = JointType.ElbowLeft;
			if (symbol == BodyPart.Elbow && s == Side.R) result = JointType.ElbowRight;
			if (symbol == BodyPart.Wrist && s == Side.L) result = JointType.WristLeft;
			if (symbol == BodyPart.Wrist && s == Side.R) result = JointType.WristRight;
			if (symbol == BodyPart.Hand && s == Side.L) result = JointType.HandLeft;
			if (symbol == BodyPart.Hand && s == Side.R) result = JointType.HandRight;
			if (symbol == BodyPart.HandTip && s == Side.L) result = JointType.HandTipLeft;
			if (symbol == BodyPart.HandTip && s == Side.R) result = JointType.HandTipRight;
			if (symbol == BodyPart.Thumb && s == Side.L) result = JointType.ThumbLeft;
			if (symbol == BodyPart.Thumb && s == Side.R) result = JointType.ThumbRight;
			if (symbol == BodyPart.Neck) result = JointType.Neck;
			if (symbol == BodyPart.Head) result = JointType.Head;
			if (symbol == BodyPart.SpineBase) result = JointType.SpineBase;
			if (symbol == BodyPart.Hip && s == Side.L) result = JointType.HipLeft;
			if (symbol == BodyPart.Hip && s == Side.R) result = JointType.HipRight;
			if (symbol == BodyPart.Knee && s == Side.L) result = JointType.KneeLeft;
			if (symbol == BodyPart.Knee && s == Side.R) result = JointType.KneeRight;
			if (symbol == BodyPart.Ankle && s == Side.L) result = JointType.AnkleLeft;
			if (symbol == BodyPart.Ankle && s == Side.R) result = JointType.AnkleRight;
			if (symbol == BodyPart.Foot && s == Side.L) result = JointType.FootLeft;
			if (symbol == BodyPart.Foot && s == Side.R) result = JointType.FootRight;

			return result;
		}

		public void ReComputeLabel()
		{
			this.langsLabel = new HashSet<string>();
			//this.ComputeLabel();
			// TODO: this should recompute the labels for every node on the tree
		}

		public void GetVectors(ref Dictionary<JointType, Vector3D> vectors3D, ref Dictionary<JointType, bool> actList)
		{
			vectors3D.Add(GetJointType(), new Vector3D(x, y, z));
			actList.Add(GetJointType(), at);

			foreach (TreeBodyPart bpt in children)
			{
				bpt.GetVectors(ref vectors3D, ref actList);
			}
		}

		public bool IsActivated()
		{
			bool result = at;

			foreach (TreeBodyPart bpt in children)
			{
				result = result || bpt.IsActivated();
			}

			return result;
		}

		//public void DrawBonesAndJoints(DrawingContext drawingContext, double x0, double y0)
		//{
		//  BrushConverter conv = new BrushConverter();
		//  if (!double.IsNaN(x0) && !double.IsNaN(x))
		//  {
		//    if (symbol != BodyPart.SpineMid)
		//    {
		//      if (at)
		//        drawingContext.DrawLine(new Pen(Brushes.Green, 5), new Point(x0, y0), new Point(x0 + x, y0 + y));
		//      else
		//        drawingContext.DrawLine(new Pen(Brushes.Green, 1), new Point(x0, y0), new Point(x0 + x, y0 + y));
		//    }
		//    foreach (TreeBodyPart bpt in children)
		//    {
		//      bpt.DrawBonesAndJoints(drawingContext, x0 + x, y0 + y);
		//    }
		//  }
		//}

		//public void DrawBonesAndJoints(DrawingContext drawingContext, IReadOnlyDictionary<JointType, Joint> body)
		//{
		//  BrushConverter conv = new BrushConverter();
		//  if (!double.IsNaN(x0) && !double.IsNaN(x))
		//  {
		//    if (symbol != BodyPart.SpineMid)
		//    {
		//      if (at)
		//        drawingContext.DrawLine(new Pen(Brushes.Green, 5), new Point(x0, y0), new Point(x0 + x, y0 + y));
		//      else
		//        drawingContext.DrawLine(new Pen(Brushes.Green, 1), new Point(x0, y0), new Point(x0 + x, y0 + y));
		//    }
		//    foreach (TreeBodyPart bpt in children)
		//    {
		//      bpt.DrawBonesAndJoints(drawingContext, x0 + x, y0 + y);
		//    }
		//  }
		//}

		//Body pose matching here
		private const double PRECISION = 0.5;

		public bool SimilarTo(TreeBodyPart bpt2)
		{

			if (double.IsNaN(x) || double.IsNaN(bpt2.x))
			{
				return false;
			}

			bool result = true;
			if (bpt2.at)
			{
				double distance = Math.Sqrt(Math.Pow(x - bpt2.x, 2) + Math.Pow(y - bpt2.y, 2) + Math.Pow(z - bpt2.z, 2));
				result = distance < PRECISION;
			}

			int i = 0;
			foreach (TreeBodyPart bpt in children)
			{
				result = result && bpt.SimilarTo(bpt2.children[i]);
				i = i + 1;
			}
			return result;
		}

		public bool allTracked()
		{
			bool tracked = (!double.IsNaN(x));
			foreach (TreeBodyPart bpt in children)
			{
				tracked = tracked && bpt.allTracked();
			}
			return tracked;
		}

		public TreeBodyPart MakeMove(Transduction move)
		{
			IEnumerable<TreeBodyPart> res;
			switch (move)
			{

				case Transduction.ToLeft: { res = toleft(); break; }
				case Transduction.ToRight: { res = toright(); break; }
				case Transduction.ToUp: { res = toup(); break; }
				case Transduction.ToDown: { res = todown(); break; }
				case Transduction.ToFront: { res = tofront(); break; }
				case Transduction.RotFroClock: { res = rotFroClock(); break; }
				case Transduction.RotFroCClock: { res = rotFroCClock(); break; }
				case Transduction.RotSagClock: { res = rotSagClock(); break; }
				case Transduction.RotSagCClock: { res = rotSagCClock(); break; }
				case Transduction.RotHorClock: { res = rotHorClock(); break; }
				case Transduction.RotHorCClock: { res = rotHorCClock(); break; }
				case Transduction.LeftST: { res = leftst(); break; }
				case Transduction.RightST: { res = rightst(); break; }
				case Transduction.ResetJT: { res = resetjt(); break; }
				case Transduction.ResetST: { res = resetst(); break; }
				case Transduction.ResetAT: { res = resetat(); break; }
				case Transduction.SpineShoulder: { res = spineshoulder(); break; }
				case Transduction.SpineBase: { res = spinebase(); break; }
				case Transduction.Hand: { res = hand(); break; }
				case Transduction.Neck: { res = neck(); break; }
				case Transduction.Shoulder: { res = shoulder(); break; }
				case Transduction.Elbow: { res = elbow(); break; }
				case Transduction.Wrist: { res = wrist(); break; }
				case Transduction.Hip: { res = hip(); break; }
				case Transduction.Knee: { res = knee(); break; }
				case Transduction.Head: { res = head(); break; }
				case Transduction.HandTip: { res = handtip(); break; }
				case Transduction.Thumb: { res = thumb(); break; }
				case Transduction.Foot: { res = foot(); break; }
				default: throw new Exception("This move doesn't exist");
			}
			foreach (var b in res) return b;
			return null;
		}

	}
}
