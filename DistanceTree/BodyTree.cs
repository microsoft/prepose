using System;
using System.Collections;
using System.Collections.Generic;
namespace Microsoft.Samples.Kinect.GestureRecognizer
{

	public enum Side { C, L, R }

	public enum BodyPart { SpineMid, SpineShoulder, Neck, Head, Shoulder, Elbow, Wrist, Hand, HandTip, Thumb, SpineBase, Hip, Knee, Ankle, Foot }

	class BodyPartUtil
	{
		public static int getRank(BodyPart el)
		{
			switch (el)
			{
				case (BodyPart.SpineMid):
					{
						return 2;
					}
				case (BodyPart.SpineShoulder):
					{
						return 3;
					}
				case (BodyPart.Neck):
					{
						return 1;
					}
				case (BodyPart.Head):
					{
						return 0;
					}
				case (BodyPart.Shoulder):
					{
						return 1;
					}
				case (BodyPart.Elbow):
					{
						return 1;
					}
				case (BodyPart.Wrist):
					{
						return 1;
					}
				case (BodyPart.Hand):
					{
						return 2;
					}
				case (BodyPart.HandTip):
					{
						return 0;
					}
				case (BodyPart.Thumb):
					{
						return 0;
					}
				case (BodyPart.SpineBase):
					{
						return 2;
					}
				case (BodyPart.Hip):
					{
						return 1;
					}
				case (BodyPart.Knee):
					{
						return 1;
					}
				case (BodyPart.Ankle):
					{
						return 1;
					}
				case (BodyPart.Foot):
					{
						return 0;
					}
			}
			return -1;
		}
	}

	partial class TreeBodyPart
	{
		double x;
		double y;
		double z;
		Side s;
		bool jt;
		bool st;
		bool at;

		BodyPart symbol;

		HashSet<string> langsLabel;
		TreeBodyPart[] children;

		public const double rangep = 0.9;

		public const double rangen = (0.0 - rangep);

		public const double sin = 0.25;

		public const double cos = 0.96;

		public bool fisleft(double x)
		{
			return (x < 0.0);
		}
		public bool fisright(double x)
		{
			return (x > 0.0);
		}
		public bool fisup(double y)
		{
			return (y > 0.0);
		}
		public bool fisdown(double y)
		{
			return (y < 0.0);
		}
		public bool fisfront(double z)
		{
			return (z > 0.0);
		}
		public bool fisback(double z)
		{
			return (z < 0.0);
		}
		public bool fispointingleft(double x)
		{
			return (x < rangen);
		}
		public bool fispointingright(double x)
		{
			return (x > rangep);
		}
		public bool fispointingup(double y)
		{
			return (y > rangep);
		}
		public bool fispointingdown(double y)
		{
			return (y < rangen);
		}
		public bool fispointingfront(double z)
		{
			return (z > rangep);
		}
		public bool fispointingback(double z)
		{
			return (z < rangen);
		}
		public double frotFroClockX(double x, double y, double z)
		{
			return ((x * cos) + (y * sin));
		}
		public double frotFroClockY(double x, double y, double z)
		{
			return ((x * (0.0 - sin)) + (y * cos));
		}
		public double frotFroCClockX(double x, double y, double z)
		{
			return ((x * cos) + (y * (0.0 - sin)));
		}
		public double frotFroCClockY(double x, double y, double z)
		{
			return ((x * sin) + (y * cos));
		}
		public double frotSagClockY(double x, double y, double z)
		{
			return ((y * cos) + (z * sin));
		}
		public double frotSagClockZ(double x, double y, double z)
		{
			return ((y * (0.0 - sin)) + (z * cos));
		}
		public double frotSagCClockY(double x, double y, double z)
		{
			return ((y * cos) + (z * (0.0 - sin)));
		}
		public double frotSagCClockZ(double x, double y, double z)
		{
			return ((y * sin) + (z * cos));
		}
		public double frotHorClockX(double x, double y, double z)
		{
			return ((x * cos) + (z * sin));
		}
		public double frotHorClockZ(double x, double y, double z)
		{
			return ((x * (0.0 - sin)) + (z * cos));
		}
		public double frotHorCClockX(double x, double y, double z)
		{
			return ((x * cos) + (z * (0.0 - sin)));
		}
		public double frotHorCClockZ(double x, double y, double z)
		{
			return ((x * sin) + (z * cos));
		}
		private TreeBodyPart(BodyPart symbol, double x, double y, double z, Side s, bool jt, bool st, bool at, TreeBodyPart[] children)
		{
			this.symbol = symbol;
			this.x = x;
			this.y = y;
			this.z = z;
			this.s = s;
			this.jt = jt;
			this.st = st;
			this.at = at;
			this.children = children;
			this.langsLabel = new HashSet<string>();
			ComputeLabel();
		}

		public TreeBodyPart this[int _i] { get { return children[_i]; } }

		public static TreeBodyPart MakeTree(BodyPart symbol, double x, double y, double z, Side s, bool jt, bool st, bool at, TreeBodyPart[] children)
		{
			if (children.Length == BodyPartUtil.getRank(symbol))
			{
				TreeBodyPart t = new TreeBodyPart(symbol, x, y, z, s, jt, st, at, children);
				return t;
			}
			return null;
		}


		private void ComputeLabel()
		{
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((s == Side.C))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lspineShoulder" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "lspineBase" }))
						{
							this.langsLabel.Add("lbody");
						}
					}
				}
			}

			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((s == Side.C))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lshoulder", "lleft" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "lneck" }))
						{
							if (this.children[2].langsLabel.IsSupersetOf(new string[] { "lshoulder", "lright" }))
							{
								this.langsLabel.Add("lspineShoulder");
							}
						}
					}
				}
			}

			if (this.symbol == BodyPart.SpineBase)
			{
				if ((s == Side.C))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lhip", "lleft" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "lhip", "lright" }))
						{
							this.langsLabel.Add("lspineBase");
						}
					}
				}
			}

			if (this.symbol == BodyPart.Hand)
			{
				if (true)
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lhandtip" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "lthumb" }))
						{
							this.langsLabel.Add("lhand");
						}
					}
				}
			}

			if (this.symbol == BodyPart.Neck)
			{
				if (true)
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lhead" }))
					{
						this.langsLabel.Add("lneck");
					}
				}
			}

			if (this.symbol == BodyPart.Shoulder)
			{
				if (true)
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lelbow" }))
					{
						this.langsLabel.Add("lshoulder");
					}
				}
			}

			if (this.symbol == BodyPart.Elbow)
			{
				if (true)
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lwrist" }))
					{
						this.langsLabel.Add("lelbow");
					}
				}
			}

			if (this.symbol == BodyPart.Wrist)
			{
				if (true)
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lhand" }))
					{
						this.langsLabel.Add("lwrist");
					}
				}
			}

			if (this.symbol == BodyPart.Hip)
			{
				if (true)
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lknee" }))
					{
						this.langsLabel.Add("lhip");
					}
				}
			}

			if (this.symbol == BodyPart.Knee)
			{
				if (true)
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lankle" }))
					{
						this.langsLabel.Add("lknee");
					}
				}
			}

			if (this.symbol == BodyPart.Ankle)
			{
				if (true)
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lfoot" }))
					{
						this.langsLabel.Add("lankle");
					}
				}
			}

			if (this.symbol == BodyPart.Head)
			{
				if (true)
				{
					this.langsLabel.Add("lhead");
				}
			}

			if (this.symbol == BodyPart.HandTip)
			{
				if (true)
				{
					this.langsLabel.Add("lhandtip");
				}
			}

			if (this.symbol == BodyPart.Thumb)
			{
				if (true)
				{
					this.langsLabel.Add("lthumb");
				}
			}

			if (this.symbol == BodyPart.Foot)
			{
				if (true)
				{
					this.langsLabel.Add("lfoot");
				}
			}

			if (this.symbol == BodyPart.Shoulder)
			{
				if ((s == Side.L))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lleft" }))
					{
						this.langsLabel.Add("lleft");
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((s == Side.L))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lleft" }))
					{
						this.langsLabel.Add("lleft");
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((s == Side.L))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lleft" }))
					{
						this.langsLabel.Add("lleft");
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((s == Side.L))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lleft" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "lleft" }))
						{
							this.langsLabel.Add("lleft");
						}
					}
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((s == Side.L))
				{
					this.langsLabel.Add("lleft");
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((s == Side.L))
				{
					this.langsLabel.Add("lleft");
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((s == Side.L))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lleft" }))
					{
						this.langsLabel.Add("lleft");
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((s == Side.L))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lleft" }))
					{
						this.langsLabel.Add("lleft");
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((s == Side.L))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lleft" }))
					{
						this.langsLabel.Add("lleft");
					}
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((s == Side.L))
				{
					this.langsLabel.Add("lleft");
				}
			}

			if (this.symbol == BodyPart.Shoulder)
			{
				if ((s == Side.R))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lright" }))
					{
						this.langsLabel.Add("lright");
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((s == Side.R))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lright" }))
					{
						this.langsLabel.Add("lright");
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((s == Side.R))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lright" }))
					{
						this.langsLabel.Add("lright");
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((s == Side.R))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lright" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "lright" }))
						{
							this.langsLabel.Add("lright");
						}
					}
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((s == Side.R))
				{
					this.langsLabel.Add("lright");
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((s == Side.R))
				{
					this.langsLabel.Add("lright");
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((s == Side.R))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lright" }))
					{
						this.langsLabel.Add("lright");
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((s == Side.R))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lright" }))
					{
						this.langsLabel.Add("lright");
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((s == Side.R))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "lright" }))
					{
						this.langsLabel.Add("lright");
					}
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((s == Side.R))
				{
					this.langsLabel.Add("lright");
				}
			}

			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (((fisleft(x)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isleft" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isleft" }))
						{
							if (this.children[2].langsLabel.IsSupersetOf(new string[] { "isleft" }))
							{
								this.langsLabel.Add("isleft");
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if (((fisleft(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isleft" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isleft" }))
						{
							this.langsLabel.Add("isleft");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if (((fisleft(x)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isleft" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isleft" }))
						{
							this.langsLabel.Add("isleft");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (((fisleft(x)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isleft" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isleft" }))
						{
							this.langsLabel.Add("isleft");
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if (((fisleft(x)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isleft" }))
					{
						this.langsLabel.Add("isleft");
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (((fisleft(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isleft" }))
					{
						this.langsLabel.Add("isleft");
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (((fisleft(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isleft" }))
					{
						this.langsLabel.Add("isleft");
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (((fisleft(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isleft" }))
					{
						this.langsLabel.Add("isleft");
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (((fisleft(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isleft" }))
					{
						this.langsLabel.Add("isleft");
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if (((fisleft(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isleft" }))
					{
						this.langsLabel.Add("isleft");
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if (((fisleft(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isleft" }))
					{
						this.langsLabel.Add("isleft");
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if (((fisleft(x)) && (jt && true)))
				{
					this.langsLabel.Add("isleft");
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if (((fisleft(x)) && (jt && st)))
				{
					this.langsLabel.Add("isleft");
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if (((fisleft(x)) && (jt && st)))
				{
					this.langsLabel.Add("isleft");
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if (((fisleft(x)) && (jt && st)))
				{
					this.langsLabel.Add("isleft");
				}
			}

			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (((fisright(x)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isright" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isright" }))
						{
							if (this.children[2].langsLabel.IsSupersetOf(new string[] { "isright" }))
							{
								this.langsLabel.Add("isright");
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if (((fisright(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isright" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isright" }))
						{
							this.langsLabel.Add("isright");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if (((fisright(x)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isright" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isright" }))
						{
							this.langsLabel.Add("isright");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (((fisright(x)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isright" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isright" }))
						{
							this.langsLabel.Add("isright");
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if (((fisright(x)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isright" }))
					{
						this.langsLabel.Add("isright");
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (((fisright(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isright" }))
					{
						this.langsLabel.Add("isright");
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (((fisright(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isright" }))
					{
						this.langsLabel.Add("isright");
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (((fisright(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isright" }))
					{
						this.langsLabel.Add("isright");
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (((fisright(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isright" }))
					{
						this.langsLabel.Add("isright");
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if (((fisright(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isright" }))
					{
						this.langsLabel.Add("isright");
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if (((fisright(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isright" }))
					{
						this.langsLabel.Add("isright");
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if (((fisright(x)) && (jt && true)))
				{
					this.langsLabel.Add("isright");
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if (((fisright(x)) && (jt && st)))
				{
					this.langsLabel.Add("isright");
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if (((fisright(x)) && (jt && st)))
				{
					this.langsLabel.Add("isright");
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if (((fisright(x)) && (jt && st)))
				{
					this.langsLabel.Add("isright");
				}
			}

			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (((fisup(y)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isup" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isup" }))
						{
							if (this.children[2].langsLabel.IsSupersetOf(new string[] { "isup" }))
							{
								this.langsLabel.Add("isup");
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if (((fisup(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isup" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isup" }))
						{
							this.langsLabel.Add("isup");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if (((fisup(y)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isup" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isup" }))
						{
							this.langsLabel.Add("isup");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (((fisup(y)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isup" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isup" }))
						{
							this.langsLabel.Add("isup");
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if (((fisup(y)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isup" }))
					{
						this.langsLabel.Add("isup");
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (((fisup(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isup" }))
					{
						this.langsLabel.Add("isup");
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (((fisup(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isup" }))
					{
						this.langsLabel.Add("isup");
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (((fisup(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isup" }))
					{
						this.langsLabel.Add("isup");
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (((fisup(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isup" }))
					{
						this.langsLabel.Add("isup");
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if (((fisup(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isup" }))
					{
						this.langsLabel.Add("isup");
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if (((fisup(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isup" }))
					{
						this.langsLabel.Add("isup");
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if (((fisup(y)) && (jt && true)))
				{
					this.langsLabel.Add("isup");
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if (((fisup(y)) && (jt && st)))
				{
					this.langsLabel.Add("isup");
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if (((fisup(y)) && (jt && st)))
				{
					this.langsLabel.Add("isup");
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if (((fisup(y)) && (jt && st)))
				{
					this.langsLabel.Add("isup");
				}
			}

			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (((fisdown(y)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isdown" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isdown" }))
						{
							if (this.children[2].langsLabel.IsSupersetOf(new string[] { "isdown" }))
							{
								this.langsLabel.Add("isdown");
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if (((fisdown(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isdown" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isdown" }))
						{
							this.langsLabel.Add("isdown");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if (((fisdown(y)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isdown" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isdown" }))
						{
							this.langsLabel.Add("isdown");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (((fisdown(y)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isdown" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isdown" }))
						{
							this.langsLabel.Add("isdown");
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if (((fisdown(y)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isdown" }))
					{
						this.langsLabel.Add("isdown");
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (((fisdown(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isdown" }))
					{
						this.langsLabel.Add("isdown");
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (((fisdown(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isdown" }))
					{
						this.langsLabel.Add("isdown");
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (((fisdown(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isdown" }))
					{
						this.langsLabel.Add("isdown");
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (((fisdown(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isdown" }))
					{
						this.langsLabel.Add("isdown");
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if (((fisdown(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isdown" }))
					{
						this.langsLabel.Add("isdown");
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if (((fisdown(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isdown" }))
					{
						this.langsLabel.Add("isdown");
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if (((fisdown(y)) && (jt && true)))
				{
					this.langsLabel.Add("isdown");
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if (((fisdown(y)) && (jt && st)))
				{
					this.langsLabel.Add("isdown");
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if (((fisdown(y)) && (jt && st)))
				{
					this.langsLabel.Add("isdown");
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if (((fisdown(y)) && (jt && st)))
				{
					this.langsLabel.Add("isdown");
				}
			}

			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (((fisfront(z)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isfront" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isfront" }))
						{
							if (this.children[2].langsLabel.IsSupersetOf(new string[] { "isfront" }))
							{
								this.langsLabel.Add("isfront");
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if (((fisfront(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isfront" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isfront" }))
						{
							this.langsLabel.Add("isfront");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if (((fisfront(z)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isfront" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isfront" }))
						{
							this.langsLabel.Add("isfront");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (((fisfront(z)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isfront" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isfront" }))
						{
							this.langsLabel.Add("isfront");
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if (((fisfront(z)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isfront" }))
					{
						this.langsLabel.Add("isfront");
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (((fisfront(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isfront" }))
					{
						this.langsLabel.Add("isfront");
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (((fisfront(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isfront" }))
					{
						this.langsLabel.Add("isfront");
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (((fisfront(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isfront" }))
					{
						this.langsLabel.Add("isfront");
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (((fisfront(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isfront" }))
					{
						this.langsLabel.Add("isfront");
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if (((fisfront(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isfront" }))
					{
						this.langsLabel.Add("isfront");
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if (((fisfront(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isfront" }))
					{
						this.langsLabel.Add("isfront");
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if (((fisfront(z)) && (jt && true)))
				{
					this.langsLabel.Add("isfront");
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if (((fisfront(z)) && (jt && st)))
				{
					this.langsLabel.Add("isfront");
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if (((fisfront(z)) && (jt && st)))
				{
					this.langsLabel.Add("isfront");
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if (((fisfront(z)) && (jt && st)))
				{
					this.langsLabel.Add("isfront");
				}
			}

			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (((fisback(z)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isback" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isback" }))
						{
							if (this.children[2].langsLabel.IsSupersetOf(new string[] { "isback" }))
							{
								this.langsLabel.Add("isback");
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if (((fisback(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isback" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isback" }))
						{
							this.langsLabel.Add("isback");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if (((fisback(z)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isback" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isback" }))
						{
							this.langsLabel.Add("isback");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (((fisback(z)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isback" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "isback" }))
						{
							this.langsLabel.Add("isback");
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if (((fisback(z)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isback" }))
					{
						this.langsLabel.Add("isback");
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (((fisback(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isback" }))
					{
						this.langsLabel.Add("isback");
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (((fisback(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isback" }))
					{
						this.langsLabel.Add("isback");
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (((fisback(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isback" }))
					{
						this.langsLabel.Add("isback");
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (((fisback(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isback" }))
					{
						this.langsLabel.Add("isback");
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if (((fisback(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isback" }))
					{
						this.langsLabel.Add("isback");
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if (((fisback(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "isback" }))
					{
						this.langsLabel.Add("isback");
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if (((fisback(z)) && (jt && true)))
				{
					this.langsLabel.Add("isback");
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if (((fisback(z)) && (jt && st)))
				{
					this.langsLabel.Add("isback");
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if (((fisback(z)) && (jt && st)))
				{
					this.langsLabel.Add("isback");
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if (((fisback(z)) && (jt && st)))
				{
					this.langsLabel.Add("isback");
				}
			}

			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (((fispointingleft(x)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingleft" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingleft" }))
						{
							if (this.children[2].langsLabel.IsSupersetOf(new string[] { "ispointingleft" }))
							{
								this.langsLabel.Add("ispointingleft");
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if (((fispointingleft(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingleft" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingleft" }))
						{
							this.langsLabel.Add("ispointingleft");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if (((fispointingleft(x)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingleft" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingleft" }))
						{
							this.langsLabel.Add("ispointingleft");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (((fispointingleft(x)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingleft" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingleft" }))
						{
							this.langsLabel.Add("ispointingleft");
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if (((fispointingleft(x)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingleft" }))
					{
						this.langsLabel.Add("ispointingleft");
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (((fispointingleft(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingleft" }))
					{
						this.langsLabel.Add("ispointingleft");
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (((fispointingleft(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingleft" }))
					{
						this.langsLabel.Add("ispointingleft");
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (((fispointingleft(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingleft" }))
					{
						this.langsLabel.Add("ispointingleft");
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (((fispointingleft(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingleft" }))
					{
						this.langsLabel.Add("ispointingleft");
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if (((fispointingleft(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingleft" }))
					{
						this.langsLabel.Add("ispointingleft");
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if (((fispointingleft(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingleft" }))
					{
						this.langsLabel.Add("ispointingleft");
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if (((fispointingleft(x)) && (jt && true)))
				{
					this.langsLabel.Add("ispointingleft");
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if (((fispointingleft(x)) && (jt && st)))
				{
					this.langsLabel.Add("ispointingleft");
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if (((fispointingleft(x)) && (jt && st)))
				{
					this.langsLabel.Add("ispointingleft");
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if (((fispointingleft(x)) && (jt && st)))
				{
					this.langsLabel.Add("ispointingleft");
				}
			}

			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (((fispointingright(x)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingright" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingright" }))
						{
							if (this.children[2].langsLabel.IsSupersetOf(new string[] { "ispointingright" }))
							{
								this.langsLabel.Add("ispointingright");
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if (((fispointingright(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingright" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingright" }))
						{
							this.langsLabel.Add("ispointingright");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if (((fispointingright(x)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingright" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingright" }))
						{
							this.langsLabel.Add("ispointingright");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (((fispointingright(x)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingright" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingright" }))
						{
							this.langsLabel.Add("ispointingright");
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if (((fispointingright(x)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingright" }))
					{
						this.langsLabel.Add("ispointingright");
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (((fispointingright(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingright" }))
					{
						this.langsLabel.Add("ispointingright");
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (((fispointingright(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingright" }))
					{
						this.langsLabel.Add("ispointingright");
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (((fispointingright(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingright" }))
					{
						this.langsLabel.Add("ispointingright");
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (((fispointingright(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingright" }))
					{
						this.langsLabel.Add("ispointingright");
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if (((fispointingright(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingright" }))
					{
						this.langsLabel.Add("ispointingright");
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if (((fispointingright(x)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingright" }))
					{
						this.langsLabel.Add("ispointingright");
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if (((fispointingright(x)) && (jt && true)))
				{
					this.langsLabel.Add("ispointingright");
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if (((fispointingright(x)) && (jt && st)))
				{
					this.langsLabel.Add("ispointingright");
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if (((fispointingright(x)) && (jt && st)))
				{
					this.langsLabel.Add("ispointingright");
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if (((fispointingright(x)) && (jt && st)))
				{
					this.langsLabel.Add("ispointingright");
				}
			}

			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (((fispointingdown(y)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingdown" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingdown" }))
						{
							if (this.children[2].langsLabel.IsSupersetOf(new string[] { "ispointingdown" }))
							{
								this.langsLabel.Add("ispointingdown");
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if (((fispointingdown(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingdown" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingdown" }))
						{
							this.langsLabel.Add("ispointingdown");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if (((fispointingdown(y)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingdown" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingdown" }))
						{
							this.langsLabel.Add("ispointingdown");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (((fispointingdown(y)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingdown" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingdown" }))
						{
							this.langsLabel.Add("ispointingdown");
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if (((fispointingdown(y)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingdown" }))
					{
						this.langsLabel.Add("ispointingdown");
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (((fispointingdown(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingdown" }))
					{
						this.langsLabel.Add("ispointingdown");
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (((fispointingdown(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingdown" }))
					{
						this.langsLabel.Add("ispointingdown");
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (((fispointingdown(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingdown" }))
					{
						this.langsLabel.Add("ispointingdown");
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (((fispointingdown(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingdown" }))
					{
						this.langsLabel.Add("ispointingdown");
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if (((fispointingdown(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingdown" }))
					{
						this.langsLabel.Add("ispointingdown");
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if (((fispointingdown(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingdown" }))
					{
						this.langsLabel.Add("ispointingdown");
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if (((fispointingdown(y)) && (jt && true)))
				{
					this.langsLabel.Add("ispointingdown");
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if (((fispointingdown(y)) && (jt && st)))
				{
					this.langsLabel.Add("ispointingdown");
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if (((fispointingdown(y)) && (jt && st)))
				{
					this.langsLabel.Add("ispointingdown");
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if (((fispointingdown(y)) && (jt && st)))
				{
					this.langsLabel.Add("ispointingdown");
				}
			}

			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (((fispointingup(y)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingup" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingup" }))
						{
							if (this.children[2].langsLabel.IsSupersetOf(new string[] { "ispointingup" }))
							{
								this.langsLabel.Add("ispointingup");
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if (((fispointingup(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingup" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingup" }))
						{
							this.langsLabel.Add("ispointingup");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if (((fispointingup(y)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingup" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingup" }))
						{
							this.langsLabel.Add("ispointingup");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (((fispointingup(y)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingup" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingup" }))
						{
							this.langsLabel.Add("ispointingup");
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if (((fispointingup(y)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingup" }))
					{
						this.langsLabel.Add("ispointingup");
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (((fispointingup(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingup" }))
					{
						this.langsLabel.Add("ispointingup");
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (((fispointingup(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingup" }))
					{
						this.langsLabel.Add("ispointingup");
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (((fispointingup(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingup" }))
					{
						this.langsLabel.Add("ispointingup");
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (((fispointingup(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingup" }))
					{
						this.langsLabel.Add("ispointingup");
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if (((fispointingup(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingup" }))
					{
						this.langsLabel.Add("ispointingup");
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if (((fispointingup(y)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingup" }))
					{
						this.langsLabel.Add("ispointingup");
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if (((fispointingup(y)) && (jt && true)))
				{
					this.langsLabel.Add("ispointingup");
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if (((fispointingup(y)) && (jt && st)))
				{
					this.langsLabel.Add("ispointingup");
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if (((fispointingup(y)) && (jt && st)))
				{
					this.langsLabel.Add("ispointingup");
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if (((fispointingup(y)) && (jt && st)))
				{
					this.langsLabel.Add("ispointingup");
				}
			}

			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (((fispointingfront(z)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingfront" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingfront" }))
						{
							if (this.children[2].langsLabel.IsSupersetOf(new string[] { "ispointingfront" }))
							{
								this.langsLabel.Add("ispointingfront");
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if (((fispointingfront(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingfront" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingfront" }))
						{
							this.langsLabel.Add("ispointingfront");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if (((fispointingfront(z)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingfront" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingfront" }))
						{
							this.langsLabel.Add("ispointingfront");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (((fispointingfront(z)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingfront" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingfront" }))
						{
							this.langsLabel.Add("ispointingfront");
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if (((fispointingfront(z)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingfront" }))
					{
						this.langsLabel.Add("ispointingfront");
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (((fispointingfront(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingfront" }))
					{
						this.langsLabel.Add("ispointingfront");
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (((fispointingfront(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingfront" }))
					{
						this.langsLabel.Add("ispointingfront");
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (((fispointingfront(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingfront" }))
					{
						this.langsLabel.Add("ispointingfront");
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (((fispointingfront(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingfront" }))
					{
						this.langsLabel.Add("ispointingfront");
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if (((fispointingfront(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingfront" }))
					{
						this.langsLabel.Add("ispointingfront");
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if (((fispointingfront(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingfront" }))
					{
						this.langsLabel.Add("ispointingfront");
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if (((fispointingfront(z)) && (jt && true)))
				{
					this.langsLabel.Add("ispointingfront");
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if (((fispointingfront(z)) && (jt && st)))
				{
					this.langsLabel.Add("ispointingfront");
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if (((fispointingfront(z)) && (jt && st)))
				{
					this.langsLabel.Add("ispointingfront");
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if (((fispointingfront(z)) && (jt && st)))
				{
					this.langsLabel.Add("ispointingfront");
				}
			}

			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (((fispointingback(z)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingback" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingback" }))
						{
							if (this.children[2].langsLabel.IsSupersetOf(new string[] { "ispointingback" }))
							{
								this.langsLabel.Add("ispointingback");
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if (((fispointingback(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingback" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingback" }))
						{
							this.langsLabel.Add("ispointingback");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if (((fispointingback(z)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingback" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingback" }))
						{
							this.langsLabel.Add("ispointingback");
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (((fispointingback(z)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingback" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "ispointingback" }))
						{
							this.langsLabel.Add("ispointingback");
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if (((fispointingback(z)) && (jt && true)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingback" }))
					{
						this.langsLabel.Add("ispointingback");
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (((fispointingback(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingback" }))
					{
						this.langsLabel.Add("ispointingback");
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (((fispointingback(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingback" }))
					{
						this.langsLabel.Add("ispointingback");
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (((fispointingback(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingback" }))
					{
						this.langsLabel.Add("ispointingback");
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (((fispointingback(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingback" }))
					{
						this.langsLabel.Add("ispointingback");
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if (((fispointingback(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingback" }))
					{
						this.langsLabel.Add("ispointingback");
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if (((fispointingback(z)) && (jt && st)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "ispointingback" }))
					{
						this.langsLabel.Add("ispointingback");
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if (((fispointingback(z)) && (jt && true)))
				{
					this.langsLabel.Add("ispointingback");
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if (((fispointingback(z)) && (jt && st)))
				{
					this.langsLabel.Add("ispointingback");
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if (((fispointingback(z)) && (jt && st)))
				{
					this.langsLabel.Add("ispointingback");
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if (((fispointingback(z)) && (jt && st)))
				{
					this.langsLabel.Add("ispointingback");
				}
			}

			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((fispointingup(y)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "issafe" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "issafe" }))
						{
							if (this.children[2].langsLabel.IsSupersetOf(new string[] { "issafe" }))
							{
								this.langsLabel.Add("issafe");
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((fispointingup(y)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "issafe" }))
					{
						if (this.children[1].langsLabel.IsSupersetOf(new string[] { "issafe" }))
						{
							this.langsLabel.Add("issafe");
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((fispointingup(y)))
				{
					if (this.children[0].langsLabel.IsSupersetOf(new string[] { "issafe" }))
					{
						this.langsLabel.Add("issafe");
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((fispointingup(y)))
				{
					this.langsLabel.Add("issafe");
				}
			}

		}
		public bool lbody()
		{
			return this.langsLabel.Contains("lbody");
		}
		public IEnumerable<TreeBodyPart> leftst()
		{
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((s == Side.C))
				{
					IEnumerable<TreeBodyPart> _leftst_l = children[0].leftst();
					foreach (var leftstl in _leftst_l)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { leftstl, children[1], children[2] });
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((s == Side.C))
				{
					IEnumerable<TreeBodyPart> _leftst_d = children[1].leftst();
					IEnumerable<TreeBodyPart> _leftst_u = children[0].leftst();
					foreach (var leftstd in _leftst_d)
					{
						foreach (var leftstu in _leftst_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { leftstu, leftstd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((s == Side.C))
				{
					IEnumerable<TreeBodyPart> _leftst_l = children[0].leftst();
					foreach (var leftstl in _leftst_l)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, st, at, new TreeBodyPart[2] { leftstl, children[1] });
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((s == Side.L))
				{
					IEnumerable<TreeBodyPart> _leftst_th = children[1].leftst();
					IEnumerable<TreeBodyPart> _leftst_ti = children[0].leftst();
					foreach (var leftstth in _leftst_th)
					{
						foreach (var leftstti in _leftst_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, jt, true, at, new TreeBodyPart[2] { leftstti, leftstth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((s == Side.L))
				{
					IEnumerable<TreeBodyPart> _leftst_e = children[0].leftst();
					foreach (var leftste in _leftst_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, true, at, new TreeBodyPart[1] { leftste });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((s == Side.L))
				{
					IEnumerable<TreeBodyPart> _leftst_w = children[0].leftst();
					foreach (var leftstw in _leftst_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, true, at, new TreeBodyPart[1] { leftstw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((s == Side.L))
				{
					IEnumerable<TreeBodyPart> _leftst_h = children[0].leftst();
					foreach (var leftsth in _leftst_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, jt, true, at, new TreeBodyPart[1] { leftsth });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((s == Side.L))
				{
					IEnumerable<TreeBodyPart> _leftst_k = children[0].leftst();
					foreach (var leftstk in _leftst_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, jt, true, at, new TreeBodyPart[1] { leftstk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((s == Side.L))
				{
					IEnumerable<TreeBodyPart> _leftst_a = children[0].leftst();
					foreach (var leftsta in _leftst_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, jt, true, at, new TreeBodyPart[1] { leftsta });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((s == Side.L))
				{
					IEnumerable<TreeBodyPart> _leftst_f = children[0].leftst();
					foreach (var leftstf in _leftst_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, y, z, s, jt, true, at, new TreeBodyPart[1] { leftstf });
					}
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((s == Side.L))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, y, z, s, jt, true, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((s == Side.L))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, y, z, s, jt, true, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((s == Side.L))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, y, z, s, jt, true, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> rightst()
		{
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((s == Side.C))
				{
					IEnumerable<TreeBodyPart> _rightst_r = children[2].rightst();
					foreach (var rightstr in _rightst_r)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { children[0], children[1], rightstr });
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((s == Side.C))
				{
					IEnumerable<TreeBodyPart> _rightst_d = children[1].rightst();
					IEnumerable<TreeBodyPart> _rightst_u = children[0].rightst();
					foreach (var rightstd in _rightst_d)
					{
						foreach (var rightstu in _rightst_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rightstu, rightstd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((s == Side.C))
				{
					IEnumerable<TreeBodyPart> _rightst_r = children[1].rightst();
					foreach (var rightstr in _rightst_r)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, st, at, new TreeBodyPart[2] { children[0], rightstr });
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((s == Side.L))
				{
					IEnumerable<TreeBodyPart> _rightst_th = children[1].rightst();
					IEnumerable<TreeBodyPart> _rightst_ti = children[0].rightst();
					foreach (var rightstth in _rightst_th)
					{
						foreach (var rightstti in _rightst_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, jt, true, at, new TreeBodyPart[2] { rightstti, rightstth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((s == Side.L))
				{
					IEnumerable<TreeBodyPart> _rightst_e = children[0].rightst();
					foreach (var rightste in _rightst_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, true, at, new TreeBodyPart[1] { rightste });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((s == Side.L))
				{
					IEnumerable<TreeBodyPart> _rightst_w = children[0].rightst();
					foreach (var rightstw in _rightst_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, true, at, new TreeBodyPart[1] { rightstw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((s == Side.L))
				{
					IEnumerable<TreeBodyPart> _rightst_h = children[0].rightst();
					foreach (var rightsth in _rightst_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, jt, true, at, new TreeBodyPart[1] { rightsth });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((s == Side.L))
				{
					IEnumerable<TreeBodyPart> _rightst_k = children[0].rightst();
					foreach (var rightstk in _rightst_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, jt, true, at, new TreeBodyPart[1] { rightstk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((s == Side.L))
				{
					IEnumerable<TreeBodyPart> _rightst_a = children[0].rightst();
					foreach (var rightsta in _rightst_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, jt, true, at, new TreeBodyPart[1] { rightsta });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((s == Side.L))
				{
					IEnumerable<TreeBodyPart> _rightst_f = children[0].rightst();
					foreach (var rightstf in _rightst_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, y, z, s, jt, true, at, new TreeBodyPart[1] { rightstf });
					}
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((s == Side.L))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, y, z, s, jt, true, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((s == Side.L))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, y, z, s, jt, true, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((s == Side.L))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, y, z, s, jt, true, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> spinemid()
		{
			if (this.symbol == BodyPart.SpineMid)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, true, st, at, new TreeBodyPart[2] { children[0], children[1] });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> spineshoulder()
		{
			if (this.symbol == BodyPart.SpineMid)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _spineshoulder_u = children[0].spineshoulder();
					foreach (var spineshoulderu in _spineshoulder_u)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { spineshoulderu, children[1] });
					}
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, true, st, at, new TreeBodyPart[3] { children[0], children[1], children[2] });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> neck()
		{
			if (this.symbol == BodyPart.SpineMid)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _neck_u = children[0].neck();
					foreach (var necku in _neck_u)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { necku, children[1] });
					}
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _neck_c = children[1].neck();
					foreach (var neckc in _neck_c)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { children[0], neckc, children[2] });
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Neck, x, y, z, s, true, st, at, new TreeBodyPart[1] { children[0] });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> head()
		{
			if (this.symbol == BodyPart.SpineMid)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _head_u = children[0].head();
					foreach (var headu in _head_u)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { headu, children[1] });
					}
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _head_c = children[1].head();
					foreach (var headc in _head_c)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { children[0], headc, children[2] });
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _head_h = children[0].head();
					foreach (var headh in _head_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, x, y, z, s, jt, st, at, new TreeBodyPart[1] { headh });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, x, y, z, s, true, st, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> shoulder()
		{
			if (this.symbol == BodyPart.SpineMid)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _shoulder_u = children[0].shoulder();
					foreach (var shoulderu in _shoulder_u)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { shoulderu, children[1] });
					}
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _shoulder_r = children[2].shoulder();
					IEnumerable<TreeBodyPart> _shoulder_l = children[0].shoulder();
					foreach (var shoulderr in _shoulder_r)
					{
						foreach (var shoulderl in _shoulder_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { shoulderl, children[1], shoulderr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, true, st, at, new TreeBodyPart[1] { children[0] });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> elbow()
		{
			if (this.symbol == BodyPart.SpineMid)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _elbow_u = children[0].elbow();
					foreach (var elbowu in _elbow_u)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { elbowu, children[1] });
					}
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _elbow_r = children[2].elbow();
					IEnumerable<TreeBodyPart> _elbow_l = children[0].elbow();
					foreach (var elbowr in _elbow_r)
					{
						foreach (var elbowl in _elbow_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { elbowl, children[1], elbowr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _elbow_e = children[0].elbow();
					foreach (var elbowe in _elbow_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, st, at, new TreeBodyPart[1] { elbowe });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, true, st, at, new TreeBodyPart[1] { children[0] });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> wrist()
		{
			if (this.symbol == BodyPart.SpineMid)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _wrist_u = children[0].wrist();
					foreach (var wristu in _wrist_u)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { wristu, children[1] });
					}
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _wrist_r = children[2].wrist();
					IEnumerable<TreeBodyPart> _wrist_l = children[0].wrist();
					foreach (var wristr in _wrist_r)
					{
						foreach (var wristl in _wrist_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { wristl, children[1], wristr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _wrist_e = children[0].wrist();
					foreach (var wriste in _wrist_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, st, at, new TreeBodyPart[1] { wriste });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _wrist_w = children[0].wrist();
					foreach (var wristw in _wrist_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, st, at, new TreeBodyPart[1] { wristw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, true, st, at, new TreeBodyPart[1] { children[0] });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> hand()
		{
			if (this.symbol == BodyPart.SpineMid)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _hand_u = children[0].hand();
					foreach (var handu in _hand_u)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { handu, children[1] });
					}
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _hand_r = children[2].hand();
					IEnumerable<TreeBodyPart> _hand_l = children[0].hand();
					foreach (var handr in _hand_r)
					{
						foreach (var handl in _hand_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { handl, children[1], handr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _hand_e = children[0].hand();
					foreach (var hande in _hand_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, st, at, new TreeBodyPart[1] { hande });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _hand_w = children[0].hand();
					foreach (var handw in _hand_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, st, at, new TreeBodyPart[1] { handw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _hand_h = children[0].hand();
					foreach (var handh in _hand_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, jt, st, at, new TreeBodyPart[1] { handh });
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, true, st, at, new TreeBodyPart[2] { children[0], children[1] });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> handtip()
		{
			if (this.symbol == BodyPart.SpineMid)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _handtip_u = children[0].handtip();
					foreach (var handtipu in _handtip_u)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { handtipu, children[1] });
					}
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _handtip_r = children[2].handtip();
					IEnumerable<TreeBodyPart> _handtip_l = children[0].handtip();
					foreach (var handtipr in _handtip_r)
					{
						foreach (var handtipl in _handtip_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { handtipl, children[1], handtipr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _handtip_e = children[0].handtip();
					foreach (var handtipe in _handtip_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, st, at, new TreeBodyPart[1] { handtipe });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _handtip_w = children[0].handtip();
					foreach (var handtipw in _handtip_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, st, at, new TreeBodyPart[1] { handtipw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _handtip_h = children[0].handtip();
					foreach (var handtiph in _handtip_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, jt, st, at, new TreeBodyPart[1] { handtiph });
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _handtip_ti = children[0].handtip();
					foreach (var handtipti in _handtip_ti)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, jt, st, at, new TreeBodyPart[2] { handtipti, children[1] });
					}
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, y, z, s, true, st, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> thumb()
		{
			if (this.symbol == BodyPart.SpineMid)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _thumb_u = children[0].thumb();
					foreach (var thumbu in _thumb_u)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { thumbu, children[1] });
					}
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _thumb_r = children[2].thumb();
					IEnumerable<TreeBodyPart> _thumb_l = children[0].thumb();
					foreach (var thumbr in _thumb_r)
					{
						foreach (var thumbl in _thumb_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { thumbl, children[1], thumbr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _thumb_e = children[0].thumb();
					foreach (var thumbe in _thumb_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, st, at, new TreeBodyPart[1] { thumbe });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _thumb_w = children[0].thumb();
					foreach (var thumbw in _thumb_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, st, at, new TreeBodyPart[1] { thumbw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _thumb_h = children[0].thumb();
					foreach (var thumbh in _thumb_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, jt, st, at, new TreeBodyPart[1] { thumbh });
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _thumb_th = children[1].thumb();
					foreach (var thumbth in _thumb_th)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, jt, st, at, new TreeBodyPart[2] { children[0], thumbth });
					}
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, y, z, s, true, st, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> spinebase()
		{
			if (this.symbol == BodyPart.SpineMid)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _spinebase_d = children[1].spinebase();
					foreach (var spinebased in _spinebase_d)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { children[0], spinebased });
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, true, st, at, new TreeBodyPart[2] { children[0], children[1] });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> hip()
		{
			if (this.symbol == BodyPart.SpineMid)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _hip_d = children[1].hip();
					foreach (var hipd in _hip_d)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { children[0], hipd });
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _hip_r = children[1].hip();
					IEnumerable<TreeBodyPart> _hip_l = children[0].hip();
					foreach (var hipr in _hip_r)
					{
						foreach (var hipl in _hip_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, st, at, new TreeBodyPart[2] { hipl, hipr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, true, st, at, new TreeBodyPart[1] { children[0] });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> knee()
		{
			if (this.symbol == BodyPart.SpineMid)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _knee_d = children[1].knee();
					foreach (var kneed in _knee_d)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { children[0], kneed });
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _knee_r = children[1].knee();
					IEnumerable<TreeBodyPart> _knee_l = children[0].knee();
					foreach (var kneer in _knee_r)
					{
						foreach (var kneel in _knee_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, st, at, new TreeBodyPart[2] { kneel, kneer });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _knee_k = children[0].knee();
					foreach (var kneek in _knee_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, jt, st, at, new TreeBodyPart[1] { kneek });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, true, st, at, new TreeBodyPart[1] { children[0] });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> ankle()
		{
			if (this.symbol == BodyPart.SpineMid)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _ankle_d = children[1].ankle();
					foreach (var ankled in _ankle_d)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { children[0], ankled });
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _ankle_r = children[1].ankle();
					IEnumerable<TreeBodyPart> _ankle_l = children[0].ankle();
					foreach (var ankler in _ankle_r)
					{
						foreach (var anklel in _ankle_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, st, at, new TreeBodyPart[2] { anklel, ankler });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _ankle_k = children[0].ankle();
					foreach (var anklek in _ankle_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, jt, st, at, new TreeBodyPart[1] { anklek });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _ankle_a = children[0].ankle();
					foreach (var anklea in _ankle_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, jt, st, at, new TreeBodyPart[1] { anklea });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, y, z, s, true, st, at, new TreeBodyPart[1] { children[0] });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> foot()
		{
			if (this.symbol == BodyPart.SpineMid)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _foot_d = children[1].foot();
					foreach (var footd in _foot_d)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { children[0], footd });
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _foot_r = children[1].foot();
					IEnumerable<TreeBodyPart> _foot_l = children[0].foot();
					foreach (var footr in _foot_r)
					{
						foreach (var footl in _foot_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, st, at, new TreeBodyPart[2] { footl, footr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _foot_k = children[0].foot();
					foreach (var footk in _foot_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, jt, st, at, new TreeBodyPart[1] { footk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _foot_a = children[0].foot();
					foreach (var foota in _foot_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, jt, st, at, new TreeBodyPart[1] { foota });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _foot_f = children[0].foot();
					foreach (var footf in _foot_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, y, z, s, jt, st, at, new TreeBodyPart[1] { footf });
					}
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, y, z, s, true, st, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> resetjt()
		{
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetjt_r = children[2].resetjt();
					IEnumerable<TreeBodyPart> _resetjt_c = children[1].resetjt();
					IEnumerable<TreeBodyPart> _resetjt_l = children[0].resetjt();
					foreach (var resetjtr in _resetjt_r)
					{
						foreach (var resetjtc in _resetjt_c)
						{
							foreach (var resetjtl in _resetjt_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, false, st, at, new TreeBodyPart[3] { resetjtl, resetjtc, resetjtr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetjt_th = children[1].resetjt();
					IEnumerable<TreeBodyPart> _resetjt_ti = children[0].resetjt();
					foreach (var resetjtth in _resetjt_th)
					{
						foreach (var resetjtti in _resetjt_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, false, st, at, new TreeBodyPart[2] { resetjtti, resetjtth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetjt_d = children[1].resetjt();
					IEnumerable<TreeBodyPart> _resetjt_u = children[0].resetjt();
					foreach (var resetjtd in _resetjt_d)
					{
						foreach (var resetjtu in _resetjt_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, false, st, at, new TreeBodyPart[2] { resetjtu, resetjtd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetjt_r = children[1].resetjt();
					IEnumerable<TreeBodyPart> _resetjt_l = children[0].resetjt();
					foreach (var resetjtr in _resetjt_r)
					{
						foreach (var resetjtl in _resetjt_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, false, st, at, new TreeBodyPart[2] { resetjtl, resetjtr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetjt_h = children[0].resetjt();
					foreach (var resetjth in _resetjt_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, x, y, z, s, false, st, at, new TreeBodyPart[1] { resetjth });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetjt_e = children[0].resetjt();
					foreach (var resetjte in _resetjt_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, false, st, at, new TreeBodyPart[1] { resetjte });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetjt_w = children[0].resetjt();
					foreach (var resetjtw in _resetjt_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, false, st, at, new TreeBodyPart[1] { resetjtw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetjt_h = children[0].resetjt();
					foreach (var resetjth in _resetjt_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, false, st, at, new TreeBodyPart[1] { resetjth });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetjt_k = children[0].resetjt();
					foreach (var resetjtk in _resetjt_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, false, st, at, new TreeBodyPart[1] { resetjtk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetjt_a = children[0].resetjt();
					foreach (var resetjta in _resetjt_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, false, st, at, new TreeBodyPart[1] { resetjta });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetjt_f = children[0].resetjt();
					foreach (var resetjtf in _resetjt_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, y, z, s, false, st, at, new TreeBodyPart[1] { resetjtf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, x, y, z, s, false, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, y, z, s, false, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, y, z, s, false, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, y, z, s, false, st, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> resetst()
		{
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetst_r = children[2].resetst();
					IEnumerable<TreeBodyPart> _resetst_c = children[1].resetst();
					IEnumerable<TreeBodyPart> _resetst_l = children[0].resetst();
					foreach (var resetstr in _resetst_r)
					{
						foreach (var resetstc in _resetst_c)
						{
							foreach (var resetstl in _resetst_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, false, at, new TreeBodyPart[3] { resetstl, resetstc, resetstr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetst_th = children[1].resetst();
					IEnumerable<TreeBodyPart> _resetst_ti = children[0].resetst();
					foreach (var resetstth in _resetst_th)
					{
						foreach (var resetstti in _resetst_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, jt, false, at, new TreeBodyPart[2] { resetstti, resetstth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetst_d = children[1].resetst();
					IEnumerable<TreeBodyPart> _resetst_u = children[0].resetst();
					foreach (var resetstd in _resetst_d)
					{
						foreach (var resetstu in _resetst_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, false, at, new TreeBodyPart[2] { resetstu, resetstd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetst_r = children[1].resetst();
					IEnumerable<TreeBodyPart> _resetst_l = children[0].resetst();
					foreach (var resetstr in _resetst_r)
					{
						foreach (var resetstl in _resetst_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, false, at, new TreeBodyPart[2] { resetstl, resetstr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetst_h = children[0].resetst();
					foreach (var resetsth in _resetst_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, x, y, z, s, jt, false, at, new TreeBodyPart[1] { resetsth });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetst_e = children[0].resetst();
					foreach (var resetste in _resetst_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, false, at, new TreeBodyPart[1] { resetste });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetst_w = children[0].resetst();
					foreach (var resetstw in _resetst_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, false, at, new TreeBodyPart[1] { resetstw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetst_h = children[0].resetst();
					foreach (var resetsth in _resetst_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, jt, false, at, new TreeBodyPart[1] { resetsth });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetst_k = children[0].resetst();
					foreach (var resetstk in _resetst_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, jt, false, at, new TreeBodyPart[1] { resetstk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetst_a = children[0].resetst();
					foreach (var resetsta in _resetst_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, jt, false, at, new TreeBodyPart[1] { resetsta });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetst_f = children[0].resetst();
					foreach (var resetstf in _resetst_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, y, z, s, jt, false, at, new TreeBodyPart[1] { resetstf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, x, y, z, s, jt, false, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, y, z, s, jt, false, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, y, z, s, jt, false, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, y, z, s, jt, false, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> resetat()
		{
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetat_r = children[2].resetat();
					IEnumerable<TreeBodyPart> _resetat_c = children[1].resetat();
					IEnumerable<TreeBodyPart> _resetat_l = children[0].resetat();
					foreach (var resetatr in _resetat_r)
					{
						foreach (var resetatc in _resetat_c)
						{
							foreach (var resetatl in _resetat_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, false, new TreeBodyPart[3] { resetatl, resetatc, resetatr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetat_th = children[1].resetat();
					IEnumerable<TreeBodyPart> _resetat_ti = children[0].resetat();
					foreach (var resetatth in _resetat_th)
					{
						foreach (var resetatti in _resetat_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, jt, st, false, new TreeBodyPart[2] { resetatti, resetatth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetat_d = children[1].resetat();
					IEnumerable<TreeBodyPart> _resetat_u = children[0].resetat();
					foreach (var resetatd in _resetat_d)
					{
						foreach (var resetatu in _resetat_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, false, new TreeBodyPart[2] { resetatu, resetatd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetat_r = children[1].resetat();
					IEnumerable<TreeBodyPart> _resetat_l = children[0].resetat();
					foreach (var resetatr in _resetat_r)
					{
						foreach (var resetatl in _resetat_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, st, false, new TreeBodyPart[2] { resetatl, resetatr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetat_h = children[0].resetat();
					foreach (var resetath in _resetat_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, x, y, z, s, jt, st, false, new TreeBodyPart[1] { resetath });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetat_e = children[0].resetat();
					foreach (var resetate in _resetat_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, st, false, new TreeBodyPart[1] { resetate });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetat_w = children[0].resetat();
					foreach (var resetatw in _resetat_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, st, false, new TreeBodyPart[1] { resetatw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetat_h = children[0].resetat();
					foreach (var resetath in _resetat_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, jt, st, false, new TreeBodyPart[1] { resetath });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetat_k = children[0].resetat();
					foreach (var resetatk in _resetat_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, jt, st, false, new TreeBodyPart[1] { resetatk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetat_a = children[0].resetat();
					foreach (var resetata in _resetat_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, jt, st, false, new TreeBodyPart[1] { resetata });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if (true)
				{
					IEnumerable<TreeBodyPart> _resetat_f = children[0].resetat();
					foreach (var resetatf in _resetat_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, y, z, s, jt, st, false, new TreeBodyPart[1] { resetatf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, x, y, z, s, jt, st, false, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, y, z, s, jt, st, false, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, y, z, s, jt, st, false, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if (true)
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, y, z, s, jt, st, false, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public bool isleft()
		{
			return this.langsLabel.Contains("isleft");
		}
		public bool isright()
		{
			return this.langsLabel.Contains("isright");
		}
		public bool isup()
		{
			return this.langsLabel.Contains("isup");
		}
		public bool isdown()
		{
			return this.langsLabel.Contains("isdown");
		}
		public bool isfront()
		{
			return this.langsLabel.Contains("isfront");
		}
		public bool isback()
		{
			return this.langsLabel.Contains("isback");
		}
		public bool ispointingleft()
		{
			return this.langsLabel.Contains("ispointingleft");
		}
		public bool ispointingright()
		{
			return this.langsLabel.Contains("ispointingright");
		}
		public bool ispointingdown()
		{
			return this.langsLabel.Contains("ispointingdown");
		}
		public bool ispointingup()
		{
			return this.langsLabel.Contains("ispointingup");
		}
		public bool ispointingfront()
		{
			return this.langsLabel.Contains("ispointingfront");
		}
		public bool ispointingback()
		{
			return this.langsLabel.Contains("ispointingback");
		}
		public IEnumerable<TreeBodyPart> rotFroClock()
		{
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_r = children[2].rotFroClock();
					IEnumerable<TreeBodyPart> _rotFroClock_c = children[1].rotFroClock();
					IEnumerable<TreeBodyPart> _rotFroClock_l = children[0].rotFroClock();
					foreach (var rotFroClockr in _rotFroClock_r)
					{
						foreach (var rotFroClockc in _rotFroClock_c)
						{
							foreach (var rotFroClockl in _rotFroClock_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, (frotFroClockX(x, y, z)), (frotFroClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[3] { rotFroClockl, rotFroClockc, rotFroClockr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_th = children[1].rotFroClock();
					IEnumerable<TreeBodyPart> _rotFroClock_ti = children[0].rotFroClock();
					foreach (var rotFroClockth in _rotFroClock_th)
					{
						foreach (var rotFroClockti in _rotFroClock_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, (frotFroClockX(x, y, z)), (frotFroClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[2] { rotFroClockti, rotFroClockth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_d = children[1].rotFroClock();
					IEnumerable<TreeBodyPart> _rotFroClock_u = children[0].rotFroClock();
					foreach (var rotFroClockd in _rotFroClock_d)
					{
						foreach (var rotFroClocku in _rotFroClock_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, (frotFroClockX(x, y, z)), (frotFroClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[2] { rotFroClocku, rotFroClockd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_r = children[1].rotFroClock();
					IEnumerable<TreeBodyPart> _rotFroClock_l = children[0].rotFroClock();
					foreach (var rotFroClockr in _rotFroClock_r)
					{
						foreach (var rotFroClockl in _rotFroClock_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, (frotFroClockX(x, y, z)), (frotFroClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[2] { rotFroClockl, rotFroClockr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_h = children[0].rotFroClock();
					foreach (var rotFroClockh in _rotFroClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, (frotFroClockX(x, y, z)), (frotFroClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[1] { rotFroClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_e = children[0].rotFroClock();
					foreach (var rotFroClocke in _rotFroClock_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, (frotFroClockX(x, y, z)), (frotFroClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[1] { rotFroClocke });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_w = children[0].rotFroClock();
					foreach (var rotFroClockw in _rotFroClock_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, (frotFroClockX(x, y, z)), (frotFroClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[1] { rotFroClockw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_h = children[0].rotFroClock();
					foreach (var rotFroClockh in _rotFroClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, (frotFroClockX(x, y, z)), (frotFroClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[1] { rotFroClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_k = children[0].rotFroClock();
					foreach (var rotFroClockk in _rotFroClock_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, (frotFroClockX(x, y, z)), (frotFroClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[1] { rotFroClockk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_a = children[0].rotFroClock();
					foreach (var rotFroClocka in _rotFroClock_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, (frotFroClockX(x, y, z)), (frotFroClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[1] { rotFroClocka });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_f = children[0].rotFroClock();
					foreach (var rotFroClockf in _rotFroClock_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, (frotFroClockX(x, y, z)), (frotFroClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[1] { rotFroClockf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((jt && true))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, (frotFroClockX(x, y, z)), (frotFroClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, (frotFroClockX(x, y, z)), (frotFroClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, (frotFroClockX(x, y, z)), (frotFroClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, (frotFroClockX(x, y, z)), (frotFroClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_r = children[2].rotFroClock();
					IEnumerable<TreeBodyPart> _rotFroClock_c = children[1].rotFroClock();
					IEnumerable<TreeBodyPart> _rotFroClock_l = children[0].rotFroClock();
					foreach (var rotFroClockr in _rotFroClock_r)
					{
						foreach (var rotFroClockc in _rotFroClock_c)
						{
							foreach (var rotFroClockl in _rotFroClock_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { rotFroClockl, rotFroClockc, rotFroClockr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_th = children[1].rotFroClock();
					IEnumerable<TreeBodyPart> _rotFroClock_ti = children[0].rotFroClock();
					foreach (var rotFroClockth in _rotFroClock_th)
					{
						foreach (var rotFroClockti in _rotFroClock_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rotFroClockti, rotFroClockth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_d = children[1].rotFroClock();
					IEnumerable<TreeBodyPart> _rotFroClock_u = children[0].rotFroClock();
					foreach (var rotFroClockd in _rotFroClock_d)
					{
						foreach (var rotFroClocku in _rotFroClock_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rotFroClocku, rotFroClockd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_r = children[1].rotFroClock();
					IEnumerable<TreeBodyPart> _rotFroClock_l = children[0].rotFroClock();
					foreach (var rotFroClockr in _rotFroClock_r)
					{
						foreach (var rotFroClockl in _rotFroClock_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rotFroClockl, rotFroClockr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_h = children[0].rotFroClock();
					foreach (var rotFroClockh in _rotFroClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotFroClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_e = children[0].rotFroClock();
					foreach (var rotFroClocke in _rotFroClock_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotFroClocke });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_w = children[0].rotFroClock();
					foreach (var rotFroClockw in _rotFroClock_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotFroClockw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_h = children[0].rotFroClock();
					foreach (var rotFroClockh in _rotFroClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotFroClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_k = children[0].rotFroClock();
					foreach (var rotFroClockk in _rotFroClock_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotFroClockk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_a = children[0].rotFroClock();
					foreach (var rotFroClocka in _rotFroClock_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotFroClocka });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotFroClock_f = children[0].rotFroClock();
					foreach (var rotFroClockf in _rotFroClock_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotFroClockf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((!((jt && true))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> rotFroCClock()
		{
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_r = children[2].rotFroCClock();
					IEnumerable<TreeBodyPart> _rotFroCClock_c = children[1].rotFroCClock();
					IEnumerable<TreeBodyPart> _rotFroCClock_l = children[0].rotFroCClock();
					foreach (var rotFroCClockr in _rotFroCClock_r)
					{
						foreach (var rotFroCClockc in _rotFroCClock_c)
						{
							foreach (var rotFroCClockl in _rotFroCClock_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, (frotFroCClockX(x, y, z)), (frotFroCClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[3] { rotFroCClockl, rotFroCClockc, rotFroCClockr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_th = children[1].rotFroCClock();
					IEnumerable<TreeBodyPart> _rotFroCClock_ti = children[0].rotFroCClock();
					foreach (var rotFroCClockth in _rotFroCClock_th)
					{
						foreach (var rotFroCClockti in _rotFroCClock_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, (frotFroCClockX(x, y, z)), (frotFroCClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[2] { rotFroCClockti, rotFroCClockth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_d = children[1].rotFroCClock();
					IEnumerable<TreeBodyPart> _rotFroCClock_u = children[0].rotFroCClock();
					foreach (var rotFroCClockd in _rotFroCClock_d)
					{
						foreach (var rotFroCClocku in _rotFroCClock_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, (frotFroCClockX(x, y, z)), (frotFroCClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[2] { rotFroCClocku, rotFroCClockd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_r = children[1].rotFroCClock();
					IEnumerable<TreeBodyPart> _rotFroCClock_l = children[0].rotFroCClock();
					foreach (var rotFroCClockr in _rotFroCClock_r)
					{
						foreach (var rotFroCClockl in _rotFroCClock_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, (frotFroCClockX(x, y, z)), (frotFroCClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[2] { rotFroCClockl, rotFroCClockr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_h = children[0].rotFroCClock();
					foreach (var rotFroCClockh in _rotFroCClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, (frotFroCClockX(x, y, z)), (frotFroCClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[1] { rotFroCClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_e = children[0].rotFroCClock();
					foreach (var rotFroCClocke in _rotFroCClock_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, (frotFroCClockX(x, y, z)), (frotFroCClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[1] { rotFroCClocke });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_w = children[0].rotFroCClock();
					foreach (var rotFroCClockw in _rotFroCClock_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, (frotFroCClockX(x, y, z)), (frotFroCClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[1] { rotFroCClockw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_h = children[0].rotFroCClock();
					foreach (var rotFroCClockh in _rotFroCClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, (frotFroCClockX(x, y, z)), (frotFroCClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[1] { rotFroCClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_k = children[0].rotFroCClock();
					foreach (var rotFroCClockk in _rotFroCClock_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, (frotFroCClockX(x, y, z)), (frotFroCClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[1] { rotFroCClockk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_a = children[0].rotFroCClock();
					foreach (var rotFroCClocka in _rotFroCClock_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, (frotFroCClockX(x, y, z)), (frotFroCClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[1] { rotFroCClocka });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_f = children[0].rotFroCClock();
					foreach (var rotFroCClockf in _rotFroCClock_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, (frotFroCClockX(x, y, z)), (frotFroCClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[1] { rotFroCClockf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((jt && true))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, (frotFroCClockX(x, y, z)), (frotFroCClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, (frotFroCClockX(x, y, z)), (frotFroCClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, (frotFroCClockX(x, y, z)), (frotFroCClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, (frotFroCClockX(x, y, z)), (frotFroCClockY(x, y, z)), z, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_r = children[2].rotFroCClock();
					IEnumerable<TreeBodyPart> _rotFroCClock_c = children[1].rotFroCClock();
					IEnumerable<TreeBodyPart> _rotFroCClock_l = children[0].rotFroCClock();
					foreach (var rotFroCClockr in _rotFroCClock_r)
					{
						foreach (var rotFroCClockc in _rotFroCClock_c)
						{
							foreach (var rotFroCClockl in _rotFroCClock_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { rotFroCClockl, rotFroCClockc, rotFroCClockr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_th = children[1].rotFroCClock();
					IEnumerable<TreeBodyPart> _rotFroCClock_ti = children[0].rotFroCClock();
					foreach (var rotFroCClockth in _rotFroCClock_th)
					{
						foreach (var rotFroCClockti in _rotFroCClock_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rotFroCClockti, rotFroCClockth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_d = children[1].rotFroCClock();
					IEnumerable<TreeBodyPart> _rotFroCClock_u = children[0].rotFroCClock();
					foreach (var rotFroCClockd in _rotFroCClock_d)
					{
						foreach (var rotFroCClocku in _rotFroCClock_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rotFroCClocku, rotFroCClockd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_r = children[1].rotFroCClock();
					IEnumerable<TreeBodyPart> _rotFroCClock_l = children[0].rotFroCClock();
					foreach (var rotFroCClockr in _rotFroCClock_r)
					{
						foreach (var rotFroCClockl in _rotFroCClock_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rotFroCClockl, rotFroCClockr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_h = children[0].rotFroCClock();
					foreach (var rotFroCClockh in _rotFroCClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotFroCClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_e = children[0].rotFroCClock();
					foreach (var rotFroCClocke in _rotFroCClock_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotFroCClocke });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_w = children[0].rotFroCClock();
					foreach (var rotFroCClockw in _rotFroCClock_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotFroCClockw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_h = children[0].rotFroCClock();
					foreach (var rotFroCClockh in _rotFroCClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotFroCClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_k = children[0].rotFroCClock();
					foreach (var rotFroCClockk in _rotFroCClock_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotFroCClockk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_a = children[0].rotFroCClock();
					foreach (var rotFroCClocka in _rotFroCClock_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotFroCClocka });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotFroCClock_f = children[0].rotFroCClock();
					foreach (var rotFroCClockf in _rotFroCClock_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotFroCClockf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((!((jt && true))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> rotSagClock()
		{
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_r = children[2].rotSagClock();
					IEnumerable<TreeBodyPart> _rotSagClock_c = children[1].rotSagClock();
					IEnumerable<TreeBodyPart> _rotSagClock_l = children[0].rotSagClock();
					foreach (var rotSagClockr in _rotSagClock_r)
					{
						foreach (var rotSagClockc in _rotSagClock_c)
						{
							foreach (var rotSagClockl in _rotSagClock_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, (frotSagClockY(x, y, z)), (frotSagClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[3] { rotSagClockl, rotSagClockc, rotSagClockr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_th = children[1].rotSagClock();
					IEnumerable<TreeBodyPart> _rotSagClock_ti = children[0].rotSagClock();
					foreach (var rotSagClockth in _rotSagClock_th)
					{
						foreach (var rotSagClockti in _rotSagClock_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, (frotSagClockY(x, y, z)), (frotSagClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[2] { rotSagClockti, rotSagClockth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_d = children[1].rotSagClock();
					IEnumerable<TreeBodyPart> _rotSagClock_u = children[0].rotSagClock();
					foreach (var rotSagClockd in _rotSagClock_d)
					{
						foreach (var rotSagClocku in _rotSagClock_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, (frotSagClockY(x, y, z)), (frotSagClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[2] { rotSagClocku, rotSagClockd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_r = children[1].rotSagClock();
					IEnumerable<TreeBodyPart> _rotSagClock_l = children[0].rotSagClock();
					foreach (var rotSagClockr in _rotSagClock_r)
					{
						foreach (var rotSagClockl in _rotSagClock_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, (frotSagClockY(x, y, z)), (frotSagClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[2] { rotSagClockl, rotSagClockr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_h = children[0].rotSagClock();
					foreach (var rotSagClockh in _rotSagClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, x, (frotSagClockY(x, y, z)), (frotSagClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotSagClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_e = children[0].rotSagClock();
					foreach (var rotSagClocke in _rotSagClock_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, (frotSagClockY(x, y, z)), (frotSagClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotSagClocke });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_w = children[0].rotSagClock();
					foreach (var rotSagClockw in _rotSagClock_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, (frotSagClockY(x, y, z)), (frotSagClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotSagClockw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_h = children[0].rotSagClock();
					foreach (var rotSagClockh in _rotSagClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, (frotSagClockY(x, y, z)), (frotSagClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotSagClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_k = children[0].rotSagClock();
					foreach (var rotSagClockk in _rotSagClock_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, (frotSagClockY(x, y, z)), (frotSagClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotSagClockk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_a = children[0].rotSagClock();
					foreach (var rotSagClocka in _rotSagClock_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, (frotSagClockY(x, y, z)), (frotSagClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotSagClocka });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_f = children[0].rotSagClock();
					foreach (var rotSagClockf in _rotSagClock_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, (frotSagClockY(x, y, z)), (frotSagClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotSagClockf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((jt && true))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, x, (frotSagClockY(x, y, z)), (frotSagClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, (frotSagClockY(x, y, z)), (frotSagClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, (frotSagClockY(x, y, z)), (frotSagClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, (frotSagClockY(x, y, z)), (frotSagClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_r = children[2].rotSagClock();
					IEnumerable<TreeBodyPart> _rotSagClock_c = children[1].rotSagClock();
					IEnumerable<TreeBodyPart> _rotSagClock_l = children[0].rotSagClock();
					foreach (var rotSagClockr in _rotSagClock_r)
					{
						foreach (var rotSagClockc in _rotSagClock_c)
						{
							foreach (var rotSagClockl in _rotSagClock_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { rotSagClockl, rotSagClockc, rotSagClockr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_th = children[1].rotSagClock();
					IEnumerable<TreeBodyPart> _rotSagClock_ti = children[0].rotSagClock();
					foreach (var rotSagClockth in _rotSagClock_th)
					{
						foreach (var rotSagClockti in _rotSagClock_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rotSagClockti, rotSagClockth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_d = children[1].rotSagClock();
					IEnumerable<TreeBodyPart> _rotSagClock_u = children[0].rotSagClock();
					foreach (var rotSagClockd in _rotSagClock_d)
					{
						foreach (var rotSagClocku in _rotSagClock_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rotSagClocku, rotSagClockd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_r = children[1].rotSagClock();
					IEnumerable<TreeBodyPart> _rotSagClock_l = children[0].rotSagClock();
					foreach (var rotSagClockr in _rotSagClock_r)
					{
						foreach (var rotSagClockl in _rotSagClock_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rotSagClockl, rotSagClockr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_h = children[0].rotSagClock();
					foreach (var rotSagClockh in _rotSagClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotSagClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_e = children[0].rotSagClock();
					foreach (var rotSagClocke in _rotSagClock_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotSagClocke });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_w = children[0].rotSagClock();
					foreach (var rotSagClockw in _rotSagClock_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotSagClockw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_h = children[0].rotSagClock();
					foreach (var rotSagClockh in _rotSagClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotSagClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_k = children[0].rotSagClock();
					foreach (var rotSagClockk in _rotSagClock_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotSagClockk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_a = children[0].rotSagClock();
					foreach (var rotSagClocka in _rotSagClock_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotSagClocka });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotSagClock_f = children[0].rotSagClock();
					foreach (var rotSagClockf in _rotSagClock_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotSagClockf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((!((jt && true))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> rotSagCClock()
		{
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_r = children[2].rotSagCClock();
					IEnumerable<TreeBodyPart> _rotSagCClock_c = children[1].rotSagCClock();
					IEnumerable<TreeBodyPart> _rotSagCClock_l = children[0].rotSagCClock();
					foreach (var rotSagCClockr in _rotSagCClock_r)
					{
						foreach (var rotSagCClockc in _rotSagCClock_c)
						{
							foreach (var rotSagCClockl in _rotSagCClock_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, (frotSagCClockY(x, y, z)), (frotSagCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[3] { rotSagCClockl, rotSagCClockc, rotSagCClockr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_th = children[1].rotSagCClock();
					IEnumerable<TreeBodyPart> _rotSagCClock_ti = children[0].rotSagCClock();
					foreach (var rotSagCClockth in _rotSagCClock_th)
					{
						foreach (var rotSagCClockti in _rotSagCClock_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, (frotSagCClockY(x, y, z)), (frotSagCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[2] { rotSagCClockti, rotSagCClockth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_d = children[1].rotSagCClock();
					IEnumerable<TreeBodyPart> _rotSagCClock_u = children[0].rotSagCClock();
					foreach (var rotSagCClockd in _rotSagCClock_d)
					{
						foreach (var rotSagCClocku in _rotSagCClock_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, (frotSagCClockY(x, y, z)), (frotSagCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[2] { rotSagCClocku, rotSagCClockd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_r = children[1].rotSagCClock();
					IEnumerable<TreeBodyPart> _rotSagCClock_l = children[0].rotSagCClock();
					foreach (var rotSagCClockr in _rotSagCClock_r)
					{
						foreach (var rotSagCClockl in _rotSagCClock_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, (frotSagCClockY(x, y, z)), (frotSagCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[2] { rotSagCClockl, rotSagCClockr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_h = children[0].rotSagCClock();
					foreach (var rotSagCClockh in _rotSagCClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, x, (frotSagCClockY(x, y, z)), (frotSagCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotSagCClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_e = children[0].rotSagCClock();
					foreach (var rotSagCClocke in _rotSagCClock_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, (frotSagCClockY(x, y, z)), (frotSagCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotSagCClocke });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_w = children[0].rotSagCClock();
					foreach (var rotSagCClockw in _rotSagCClock_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, (frotSagCClockY(x, y, z)), (frotSagCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotSagCClockw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_h = children[0].rotSagCClock();
					foreach (var rotSagCClockh in _rotSagCClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, (frotSagCClockY(x, y, z)), (frotSagCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotSagCClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_k = children[0].rotSagCClock();
					foreach (var rotSagCClockk in _rotSagCClock_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, (frotSagCClockY(x, y, z)), (frotSagCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotSagCClockk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_a = children[0].rotSagCClock();
					foreach (var rotSagCClocka in _rotSagCClock_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, (frotSagCClockY(x, y, z)), (frotSagCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotSagCClocka });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_f = children[0].rotSagCClock();
					foreach (var rotSagCClockf in _rotSagCClock_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, (frotSagCClockY(x, y, z)), (frotSagCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotSagCClockf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((jt && true))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, x, (frotSagCClockY(x, y, z)), (frotSagCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, (frotSagCClockY(x, y, z)), (frotSagCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, (frotSagCClockY(x, y, z)), (frotSagCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, (frotSagCClockY(x, y, z)), (frotSagCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_r = children[2].rotSagCClock();
					IEnumerable<TreeBodyPart> _rotSagCClock_c = children[1].rotSagCClock();
					IEnumerable<TreeBodyPart> _rotSagCClock_l = children[0].rotSagCClock();
					foreach (var rotSagCClockr in _rotSagCClock_r)
					{
						foreach (var rotSagCClockc in _rotSagCClock_c)
						{
							foreach (var rotSagCClockl in _rotSagCClock_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { rotSagCClockl, rotSagCClockc, rotSagCClockr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_th = children[1].rotSagCClock();
					IEnumerable<TreeBodyPart> _rotSagCClock_ti = children[0].rotSagCClock();
					foreach (var rotSagCClockth in _rotSagCClock_th)
					{
						foreach (var rotSagCClockti in _rotSagCClock_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rotSagCClockti, rotSagCClockth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_d = children[1].rotSagCClock();
					IEnumerable<TreeBodyPart> _rotSagCClock_u = children[0].rotSagCClock();
					foreach (var rotSagCClockd in _rotSagCClock_d)
					{
						foreach (var rotSagCClocku in _rotSagCClock_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rotSagCClocku, rotSagCClockd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_r = children[1].rotSagCClock();
					IEnumerable<TreeBodyPart> _rotSagCClock_l = children[0].rotSagCClock();
					foreach (var rotSagCClockr in _rotSagCClock_r)
					{
						foreach (var rotSagCClockl in _rotSagCClock_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rotSagCClockl, rotSagCClockr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_h = children[0].rotSagCClock();
					foreach (var rotSagCClockh in _rotSagCClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotSagCClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_e = children[0].rotSagCClock();
					foreach (var rotSagCClocke in _rotSagCClock_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotSagCClocke });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_w = children[0].rotSagCClock();
					foreach (var rotSagCClockw in _rotSagCClock_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotSagCClockw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_h = children[0].rotSagCClock();
					foreach (var rotSagCClockh in _rotSagCClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotSagCClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_k = children[0].rotSagCClock();
					foreach (var rotSagCClockk in _rotSagCClock_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotSagCClockk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_a = children[0].rotSagCClock();
					foreach (var rotSagCClocka in _rotSagCClock_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotSagCClocka });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotSagCClock_f = children[0].rotSagCClock();
					foreach (var rotSagCClockf in _rotSagCClock_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotSagCClockf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((!((jt && true))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> rotHorClock()
		{
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_r = children[2].rotHorClock();
					IEnumerable<TreeBodyPart> _rotHorClock_c = children[1].rotHorClock();
					IEnumerable<TreeBodyPart> _rotHorClock_l = children[0].rotHorClock();
					foreach (var rotHorClockr in _rotHorClock_r)
					{
						foreach (var rotHorClockc in _rotHorClock_c)
						{
							foreach (var rotHorClockl in _rotHorClock_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, (frotHorClockX(x, y, z)), y, (frotHorClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[3] { rotHorClockl, rotHorClockc, rotHorClockr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_th = children[1].rotHorClock();
					IEnumerable<TreeBodyPart> _rotHorClock_ti = children[0].rotHorClock();
					foreach (var rotHorClockth in _rotHorClock_th)
					{
						foreach (var rotHorClockti in _rotHorClock_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, (frotHorClockX(x, y, z)), y, (frotHorClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[2] { rotHorClockti, rotHorClockth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_d = children[1].rotHorClock();
					IEnumerable<TreeBodyPart> _rotHorClock_u = children[0].rotHorClock();
					foreach (var rotHorClockd in _rotHorClock_d)
					{
						foreach (var rotHorClocku in _rotHorClock_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, (frotHorClockX(x, y, z)), y, (frotHorClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[2] { rotHorClocku, rotHorClockd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_r = children[1].rotHorClock();
					IEnumerable<TreeBodyPart> _rotHorClock_l = children[0].rotHorClock();
					foreach (var rotHorClockr in _rotHorClock_r)
					{
						foreach (var rotHorClockl in _rotHorClock_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, (frotHorClockX(x, y, z)), y, (frotHorClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[2] { rotHorClockl, rotHorClockr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_h = children[0].rotHorClock();
					foreach (var rotHorClockh in _rotHorClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, (frotHorClockX(x, y, z)), y, (frotHorClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotHorClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_e = children[0].rotHorClock();
					foreach (var rotHorClocke in _rotHorClock_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, (frotHorClockX(x, y, z)), y, (frotHorClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotHorClocke });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_w = children[0].rotHorClock();
					foreach (var rotHorClockw in _rotHorClock_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, (frotHorClockX(x, y, z)), y, (frotHorClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotHorClockw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_h = children[0].rotHorClock();
					foreach (var rotHorClockh in _rotHorClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, (frotHorClockX(x, y, z)), y, (frotHorClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotHorClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_k = children[0].rotHorClock();
					foreach (var rotHorClockk in _rotHorClock_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, (frotHorClockX(x, y, z)), y, (frotHorClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotHorClockk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_a = children[0].rotHorClock();
					foreach (var rotHorClocka in _rotHorClock_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, (frotHorClockX(x, y, z)), y, (frotHorClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotHorClocka });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_f = children[0].rotHorClock();
					foreach (var rotHorClockf in _rotHorClock_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, (frotHorClockX(x, y, z)), y, (frotHorClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotHorClockf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((jt && true))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, (frotHorClockX(x, y, z)), y, (frotHorClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, (frotHorClockX(x, y, z)), y, (frotHorClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, (frotHorClockX(x, y, z)), y, (frotHorClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, (frotHorClockX(x, y, z)), y, (frotHorClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_r = children[2].rotHorClock();
					IEnumerable<TreeBodyPart> _rotHorClock_c = children[1].rotHorClock();
					IEnumerable<TreeBodyPart> _rotHorClock_l = children[0].rotHorClock();
					foreach (var rotHorClockr in _rotHorClock_r)
					{
						foreach (var rotHorClockc in _rotHorClock_c)
						{
							foreach (var rotHorClockl in _rotHorClock_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { rotHorClockl, rotHorClockc, rotHorClockr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_th = children[1].rotHorClock();
					IEnumerable<TreeBodyPart> _rotHorClock_ti = children[0].rotHorClock();
					foreach (var rotHorClockth in _rotHorClock_th)
					{
						foreach (var rotHorClockti in _rotHorClock_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rotHorClockti, rotHorClockth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_d = children[1].rotHorClock();
					IEnumerable<TreeBodyPart> _rotHorClock_u = children[0].rotHorClock();
					foreach (var rotHorClockd in _rotHorClock_d)
					{
						foreach (var rotHorClocku in _rotHorClock_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rotHorClocku, rotHorClockd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_r = children[1].rotHorClock();
					IEnumerable<TreeBodyPart> _rotHorClock_l = children[0].rotHorClock();
					foreach (var rotHorClockr in _rotHorClock_r)
					{
						foreach (var rotHorClockl in _rotHorClock_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rotHorClockl, rotHorClockr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_h = children[0].rotHorClock();
					foreach (var rotHorClockh in _rotHorClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotHorClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_e = children[0].rotHorClock();
					foreach (var rotHorClocke in _rotHorClock_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotHorClocke });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_w = children[0].rotHorClock();
					foreach (var rotHorClockw in _rotHorClock_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotHorClockw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_h = children[0].rotHorClock();
					foreach (var rotHorClockh in _rotHorClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotHorClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_k = children[0].rotHorClock();
					foreach (var rotHorClockk in _rotHorClock_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotHorClockk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_a = children[0].rotHorClock();
					foreach (var rotHorClocka in _rotHorClock_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotHorClocka });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotHorClock_f = children[0].rotHorClock();
					foreach (var rotHorClockf in _rotHorClock_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotHorClockf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((!((jt && true))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> rotHorCClock()
		{
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_r = children[2].rotHorCClock();
					IEnumerable<TreeBodyPart> _rotHorCClock_c = children[1].rotHorCClock();
					IEnumerable<TreeBodyPart> _rotHorCClock_l = children[0].rotHorCClock();
					foreach (var rotHorCClockr in _rotHorCClock_r)
					{
						foreach (var rotHorCClockc in _rotHorCClock_c)
						{
							foreach (var rotHorCClockl in _rotHorCClock_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, (frotHorCClockX(x, y, z)), y, (frotHorCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[3] { rotHorCClockl, rotHorCClockc, rotHorCClockr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_th = children[1].rotHorCClock();
					IEnumerable<TreeBodyPart> _rotHorCClock_ti = children[0].rotHorCClock();
					foreach (var rotHorCClockth in _rotHorCClock_th)
					{
						foreach (var rotHorCClockti in _rotHorCClock_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, (frotHorCClockX(x, y, z)), y, (frotHorCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[2] { rotHorCClockti, rotHorCClockth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_d = children[1].rotHorCClock();
					IEnumerable<TreeBodyPart> _rotHorCClock_u = children[0].rotHorCClock();
					foreach (var rotHorCClockd in _rotHorCClock_d)
					{
						foreach (var rotHorCClocku in _rotHorCClock_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, (frotHorCClockX(x, y, z)), y, (frotHorCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[2] { rotHorCClocku, rotHorCClockd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_r = children[1].rotHorCClock();
					IEnumerable<TreeBodyPart> _rotHorCClock_l = children[0].rotHorCClock();
					foreach (var rotHorCClockr in _rotHorCClock_r)
					{
						foreach (var rotHorCClockl in _rotHorCClock_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, (frotHorCClockX(x, y, z)), y, (frotHorCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[2] { rotHorCClockl, rotHorCClockr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_h = children[0].rotHorCClock();
					foreach (var rotHorCClockh in _rotHorCClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, (frotHorCClockX(x, y, z)), y, (frotHorCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotHorCClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_e = children[0].rotHorCClock();
					foreach (var rotHorCClocke in _rotHorCClock_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, (frotHorCClockX(x, y, z)), y, (frotHorCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotHorCClocke });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_w = children[0].rotHorCClock();
					foreach (var rotHorCClockw in _rotHorCClock_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, (frotHorCClockX(x, y, z)), y, (frotHorCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotHorCClockw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_h = children[0].rotHorCClock();
					foreach (var rotHorCClockh in _rotHorCClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, (frotHorCClockX(x, y, z)), y, (frotHorCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotHorCClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_k = children[0].rotHorCClock();
					foreach (var rotHorCClockk in _rotHorCClock_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, (frotHorCClockX(x, y, z)), y, (frotHorCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotHorCClockk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_a = children[0].rotHorCClock();
					foreach (var rotHorCClocka in _rotHorCClock_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, (frotHorCClockX(x, y, z)), y, (frotHorCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotHorCClocka });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_f = children[0].rotHorCClock();
					foreach (var rotHorCClockf in _rotHorCClock_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, (frotHorCClockX(x, y, z)), y, (frotHorCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[1] { rotHorCClockf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((jt && true))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, (frotHorCClockX(x, y, z)), y, (frotHorCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, (frotHorCClockX(x, y, z)), y, (frotHorCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, (frotHorCClockX(x, y, z)), y, (frotHorCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, (frotHorCClockX(x, y, z)), y, (frotHorCClockZ(x, y, z)), s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_r = children[2].rotHorCClock();
					IEnumerable<TreeBodyPart> _rotHorCClock_c = children[1].rotHorCClock();
					IEnumerable<TreeBodyPart> _rotHorCClock_l = children[0].rotHorCClock();
					foreach (var rotHorCClockr in _rotHorCClock_r)
					{
						foreach (var rotHorCClockc in _rotHorCClock_c)
						{
							foreach (var rotHorCClockl in _rotHorCClock_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { rotHorCClockl, rotHorCClockc, rotHorCClockr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_th = children[1].rotHorCClock();
					IEnumerable<TreeBodyPart> _rotHorCClock_ti = children[0].rotHorCClock();
					foreach (var rotHorCClockth in _rotHorCClock_th)
					{
						foreach (var rotHorCClockti in _rotHorCClock_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rotHorCClockti, rotHorCClockth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_d = children[1].rotHorCClock();
					IEnumerable<TreeBodyPart> _rotHorCClock_u = children[0].rotHorCClock();
					foreach (var rotHorCClockd in _rotHorCClock_d)
					{
						foreach (var rotHorCClocku in _rotHorCClock_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rotHorCClocku, rotHorCClockd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_r = children[1].rotHorCClock();
					IEnumerable<TreeBodyPart> _rotHorCClock_l = children[0].rotHorCClock();
					foreach (var rotHorCClockr in _rotHorCClock_r)
					{
						foreach (var rotHorCClockl in _rotHorCClock_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, st, at, new TreeBodyPart[2] { rotHorCClockl, rotHorCClockr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_h = children[0].rotHorCClock();
					foreach (var rotHorCClockh in _rotHorCClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotHorCClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_e = children[0].rotHorCClock();
					foreach (var rotHorCClocke in _rotHorCClock_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotHorCClocke });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_w = children[0].rotHorCClock();
					foreach (var rotHorCClockw in _rotHorCClock_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotHorCClockw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_h = children[0].rotHorCClock();
					foreach (var rotHorCClockh in _rotHorCClock_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotHorCClockh });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_k = children[0].rotHorCClock();
					foreach (var rotHorCClockk in _rotHorCClock_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotHorCClockk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_a = children[0].rotHorCClock();
					foreach (var rotHorCClocka in _rotHorCClock_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotHorCClocka });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _rotHorCClock_f = children[0].rotHorCClock();
					foreach (var rotHorCClockf in _rotHorCClock_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, y, z, s, jt, st, at, new TreeBodyPart[1] { rotHorCClockf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((!((jt && true))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> todown()
		{
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _todown_r = children[2].todown();
					IEnumerable<TreeBodyPart> _todown_c = children[1].todown();
					IEnumerable<TreeBodyPart> _todown_l = children[0].todown();
					foreach (var todownr in _todown_r)
					{
						foreach (var todownc in _todown_c)
						{
							foreach (var todownl in _todown_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, 0.0, (0.0 - 1.0), 0.0, s, jt, st, true, new TreeBodyPart[3] { todownl, todownc, todownr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _todown_th = children[1].todown();
					IEnumerable<TreeBodyPart> _todown_ti = children[0].todown();
					foreach (var todownth in _todown_th)
					{
						foreach (var todownti in _todown_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, 0.0, (0.0 - 1.0), 0.0, s, jt, st, true, new TreeBodyPart[2] { todownti, todownth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _todown_d = children[1].todown();
					IEnumerable<TreeBodyPart> _todown_u = children[0].todown();
					foreach (var todownd in _todown_d)
					{
						foreach (var todownu in _todown_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, 0.0, (0.0 - 1.0), 0.0, s, jt, st, true, new TreeBodyPart[2] { todownu, todownd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _todown_r = children[1].todown();
					IEnumerable<TreeBodyPart> _todown_l = children[0].todown();
					foreach (var todownr in _todown_r)
					{
						foreach (var todownl in _todown_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, 0.0, (0.0 - 1.0), 0.0, s, jt, st, true, new TreeBodyPart[2] { todownl, todownr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _todown_h = children[0].todown();
					foreach (var todownh in _todown_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, 0.0, (0.0 - 1.0), 0.0, s, jt, st, true, new TreeBodyPart[1] { todownh });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _todown_e = children[0].todown();
					foreach (var todowne in _todown_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, 0.0, (0.0 - 1.0), 0.0, s, jt, st, true, new TreeBodyPart[1] { todowne });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _todown_w = children[0].todown();
					foreach (var todownw in _todown_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, 0.0, (0.0 - 1.0), 0.0, s, jt, st, true, new TreeBodyPart[1] { todownw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _todown_h = children[0].todown();
					foreach (var todownh in _todown_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, 0.0, (0.0 - 1.0), 0.0, s, jt, st, true, new TreeBodyPart[1] { todownh });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _todown_k = children[0].todown();
					foreach (var todownk in _todown_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, 0.0, (0.0 - 1.0), 0.0, s, jt, st, true, new TreeBodyPart[1] { todownk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _todown_a = children[0].todown();
					foreach (var todowna in _todown_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, 0.0, (0.0 - 1.0), 0.0, s, jt, st, true, new TreeBodyPart[1] { todowna });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _todown_f = children[0].todown();
					foreach (var todownf in _todown_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, 0.0, (0.0 - 1.0), 0.0, s, jt, st, true, new TreeBodyPart[1] { todownf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((jt && true))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, 0.0, (0.0 - 1.0), 0.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, 0.0, (0.0 - 1.0), 0.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, 0.0, (0.0 - 1.0), 0.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, 0.0, (0.0 - 1.0), 0.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _todown_r = children[2].todown();
					IEnumerable<TreeBodyPart> _todown_c = children[1].todown();
					IEnumerable<TreeBodyPart> _todown_l = children[0].todown();
					foreach (var todownr in _todown_r)
					{
						foreach (var todownc in _todown_c)
						{
							foreach (var todownl in _todown_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { todownl, todownc, todownr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _todown_th = children[1].todown();
					IEnumerable<TreeBodyPart> _todown_ti = children[0].todown();
					foreach (var todownth in _todown_th)
					{
						foreach (var todownti in _todown_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, jt, st, at, new TreeBodyPart[2] { todownti, todownth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _todown_d = children[1].todown();
					IEnumerable<TreeBodyPart> _todown_u = children[0].todown();
					foreach (var todownd in _todown_d)
					{
						foreach (var todownu in _todown_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { todownu, todownd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _todown_r = children[1].todown();
					IEnumerable<TreeBodyPart> _todown_l = children[0].todown();
					foreach (var todownr in _todown_r)
					{
						foreach (var todownl in _todown_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, st, at, new TreeBodyPart[2] { todownl, todownr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _todown_h = children[0].todown();
					foreach (var todownh in _todown_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, x, y, z, s, jt, st, at, new TreeBodyPart[1] { todownh });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _todown_e = children[0].todown();
					foreach (var todowne in _todown_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, st, at, new TreeBodyPart[1] { todowne });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _todown_w = children[0].todown();
					foreach (var todownw in _todown_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, st, at, new TreeBodyPart[1] { todownw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _todown_h = children[0].todown();
					foreach (var todownh in _todown_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, jt, st, at, new TreeBodyPart[1] { todownh });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _todown_k = children[0].todown();
					foreach (var todownk in _todown_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, jt, st, at, new TreeBodyPart[1] { todownk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _todown_a = children[0].todown();
					foreach (var todowna in _todown_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, jt, st, at, new TreeBodyPart[1] { todowna });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _todown_f = children[0].todown();
					foreach (var todownf in _todown_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, y, z, s, jt, st, at, new TreeBodyPart[1] { todownf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((!((jt && true))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> toup()
		{
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _toup_r = children[2].toup();
					IEnumerable<TreeBodyPart> _toup_c = children[1].toup();
					IEnumerable<TreeBodyPart> _toup_l = children[0].toup();
					foreach (var toupr in _toup_r)
					{
						foreach (var toupc in _toup_c)
						{
							foreach (var toupl in _toup_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, 0.0, 1.0, 0.0, s, jt, st, true, new TreeBodyPart[3] { toupl, toupc, toupr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toup_th = children[1].toup();
					IEnumerable<TreeBodyPart> _toup_ti = children[0].toup();
					foreach (var toupth in _toup_th)
					{
						foreach (var toupti in _toup_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, 0.0, 1.0, 0.0, s, jt, st, true, new TreeBodyPart[2] { toupti, toupth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _toup_d = children[1].toup();
					IEnumerable<TreeBodyPart> _toup_u = children[0].toup();
					foreach (var toupd in _toup_d)
					{
						foreach (var toupu in _toup_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, 0.0, 1.0, 0.0, s, jt, st, true, new TreeBodyPart[2] { toupu, toupd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _toup_r = children[1].toup();
					IEnumerable<TreeBodyPart> _toup_l = children[0].toup();
					foreach (var toupr in _toup_r)
					{
						foreach (var toupl in _toup_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, 0.0, 1.0, 0.0, s, jt, st, true, new TreeBodyPart[2] { toupl, toupr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _toup_h = children[0].toup();
					foreach (var touph in _toup_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, 0.0, 1.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { touph });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toup_e = children[0].toup();
					foreach (var toupe in _toup_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, 0.0, 1.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { toupe });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toup_w = children[0].toup();
					foreach (var toupw in _toup_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, 0.0, 1.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { toupw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toup_h = children[0].toup();
					foreach (var touph in _toup_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, 0.0, 1.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { touph });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toup_k = children[0].toup();
					foreach (var toupk in _toup_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, 0.0, 1.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { toupk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toup_a = children[0].toup();
					foreach (var toupa in _toup_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, 0.0, 1.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { toupa });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toup_f = children[0].toup();
					foreach (var toupf in _toup_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, 0.0, 1.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { toupf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((jt && true))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, 0.0, 1.0, 0.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, 0.0, 1.0, 0.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, 0.0, 1.0, 0.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, 0.0, 1.0, 0.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _toup_r = children[2].toup();
					IEnumerable<TreeBodyPart> _toup_c = children[1].toup();
					IEnumerable<TreeBodyPart> _toup_l = children[0].toup();
					foreach (var toupr in _toup_r)
					{
						foreach (var toupc in _toup_c)
						{
							foreach (var toupl in _toup_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { toupl, toupc, toupr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toup_th = children[1].toup();
					IEnumerable<TreeBodyPart> _toup_ti = children[0].toup();
					foreach (var toupth in _toup_th)
					{
						foreach (var toupti in _toup_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, jt, st, at, new TreeBodyPart[2] { toupti, toupth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _toup_d = children[1].toup();
					IEnumerable<TreeBodyPart> _toup_u = children[0].toup();
					foreach (var toupd in _toup_d)
					{
						foreach (var toupu in _toup_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { toupu, toupd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _toup_r = children[1].toup();
					IEnumerable<TreeBodyPart> _toup_l = children[0].toup();
					foreach (var toupr in _toup_r)
					{
						foreach (var toupl in _toup_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, st, at, new TreeBodyPart[2] { toupl, toupr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _toup_h = children[0].toup();
					foreach (var touph in _toup_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, x, y, z, s, jt, st, at, new TreeBodyPart[1] { touph });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toup_e = children[0].toup();
					foreach (var toupe in _toup_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, st, at, new TreeBodyPart[1] { toupe });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toup_w = children[0].toup();
					foreach (var toupw in _toup_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, st, at, new TreeBodyPart[1] { toupw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toup_h = children[0].toup();
					foreach (var touph in _toup_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, jt, st, at, new TreeBodyPart[1] { touph });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toup_k = children[0].toup();
					foreach (var toupk in _toup_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, jt, st, at, new TreeBodyPart[1] { toupk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toup_a = children[0].toup();
					foreach (var toupa in _toup_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, jt, st, at, new TreeBodyPart[1] { toupa });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toup_f = children[0].toup();
					foreach (var toupf in _toup_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, y, z, s, jt, st, at, new TreeBodyPart[1] { toupf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((!((jt && true))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> tofront()
		{
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _tofront_r = children[2].tofront();
					IEnumerable<TreeBodyPart> _tofront_c = children[1].tofront();
					IEnumerable<TreeBodyPart> _tofront_l = children[0].tofront();
					foreach (var tofrontr in _tofront_r)
					{
						foreach (var tofrontc in _tofront_c)
						{
							foreach (var tofrontl in _tofront_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, 0.0, 0.0, 1.0, s, jt, st, true, new TreeBodyPart[3] { tofrontl, tofrontc, tofrontr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _tofront_th = children[1].tofront();
					IEnumerable<TreeBodyPart> _tofront_ti = children[0].tofront();
					foreach (var tofrontth in _tofront_th)
					{
						foreach (var tofrontti in _tofront_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, 0.0, 0.0, 1.0, s, jt, st, true, new TreeBodyPart[2] { tofrontti, tofrontth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _tofront_d = children[1].tofront();
					IEnumerable<TreeBodyPart> _tofront_u = children[0].tofront();
					foreach (var tofrontd in _tofront_d)
					{
						foreach (var tofrontu in _tofront_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, 0.0, 0.0, 1.0, s, jt, st, true, new TreeBodyPart[2] { tofrontu, tofrontd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _tofront_r = children[1].tofront();
					IEnumerable<TreeBodyPart> _tofront_l = children[0].tofront();
					foreach (var tofrontr in _tofront_r)
					{
						foreach (var tofrontl in _tofront_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, 0.0, 0.0, 1.0, s, jt, st, true, new TreeBodyPart[2] { tofrontl, tofrontr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _tofront_h = children[0].tofront();
					foreach (var tofronth in _tofront_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, 0.0, 0.0, 1.0, s, jt, st, true, new TreeBodyPart[1] { tofronth });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _tofront_e = children[0].tofront();
					foreach (var tofronte in _tofront_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, 0.0, 0.0, 1.0, s, jt, st, true, new TreeBodyPart[1] { tofronte });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _tofront_w = children[0].tofront();
					foreach (var tofrontw in _tofront_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, 0.0, 0.0, 1.0, s, jt, st, true, new TreeBodyPart[1] { tofrontw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _tofront_h = children[0].tofront();
					foreach (var tofronth in _tofront_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, 0.0, 0.0, 1.0, s, jt, st, true, new TreeBodyPart[1] { tofronth });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _tofront_k = children[0].tofront();
					foreach (var tofrontk in _tofront_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, 0.0, 0.0, 1.0, s, jt, st, true, new TreeBodyPart[1] { tofrontk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _tofront_a = children[0].tofront();
					foreach (var tofronta in _tofront_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, 0.0, 0.0, 1.0, s, jt, st, true, new TreeBodyPart[1] { tofronta });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _tofront_f = children[0].tofront();
					foreach (var tofrontf in _tofront_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, 0.0, 0.0, 1.0, s, jt, st, true, new TreeBodyPart[1] { tofrontf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((jt && true))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, 0.0, 0.0, 1.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, 0.0, 0.0, 1.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, 0.0, 0.0, 1.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, 0.0, 0.0, 1.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _tofront_r = children[2].tofront();
					IEnumerable<TreeBodyPart> _tofront_c = children[1].tofront();
					IEnumerable<TreeBodyPart> _tofront_l = children[0].tofront();
					foreach (var tofrontr in _tofront_r)
					{
						foreach (var tofrontc in _tofront_c)
						{
							foreach (var tofrontl in _tofront_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { tofrontl, tofrontc, tofrontr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _tofront_th = children[1].tofront();
					IEnumerable<TreeBodyPart> _tofront_ti = children[0].tofront();
					foreach (var tofrontth in _tofront_th)
					{
						foreach (var tofrontti in _tofront_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, jt, st, at, new TreeBodyPart[2] { tofrontti, tofrontth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _tofront_d = children[1].tofront();
					IEnumerable<TreeBodyPart> _tofront_u = children[0].tofront();
					foreach (var tofrontd in _tofront_d)
					{
						foreach (var tofrontu in _tofront_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { tofrontu, tofrontd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _tofront_r = children[1].tofront();
					IEnumerable<TreeBodyPart> _tofront_l = children[0].tofront();
					foreach (var tofrontr in _tofront_r)
					{
						foreach (var tofrontl in _tofront_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, st, at, new TreeBodyPart[2] { tofrontl, tofrontr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _tofront_h = children[0].tofront();
					foreach (var tofronth in _tofront_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, x, y, z, s, jt, st, at, new TreeBodyPart[1] { tofronth });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _tofront_e = children[0].tofront();
					foreach (var tofronte in _tofront_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, st, at, new TreeBodyPart[1] { tofronte });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _tofront_w = children[0].tofront();
					foreach (var tofrontw in _tofront_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, st, at, new TreeBodyPart[1] { tofrontw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _tofront_h = children[0].tofront();
					foreach (var tofronth in _tofront_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, jt, st, at, new TreeBodyPart[1] { tofronth });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _tofront_k = children[0].tofront();
					foreach (var tofrontk in _tofront_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, jt, st, at, new TreeBodyPart[1] { tofrontk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _tofront_a = children[0].tofront();
					foreach (var tofronta in _tofront_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, jt, st, at, new TreeBodyPart[1] { tofronta });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _tofront_f = children[0].tofront();
					foreach (var tofrontf in _tofront_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, y, z, s, jt, st, at, new TreeBodyPart[1] { tofrontf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((!((jt && true))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> toleft()
		{
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _toleft_r = children[2].toleft();
					IEnumerable<TreeBodyPart> _toleft_c = children[1].toleft();
					IEnumerable<TreeBodyPart> _toleft_l = children[0].toleft();
					foreach (var toleftr in _toleft_r)
					{
						foreach (var toleftc in _toleft_c)
						{
							foreach (var toleftl in _toleft_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, (0.0 - 1.0), 0.0, 0.0, s, jt, st, true, new TreeBodyPart[3] { toleftl, toleftc, toleftr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toleft_th = children[1].toleft();
					IEnumerable<TreeBodyPart> _toleft_ti = children[0].toleft();
					foreach (var toleftth in _toleft_th)
					{
						foreach (var toleftti in _toleft_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, (0.0 - 1.0), 0.0, 0.0, s, jt, st, true, new TreeBodyPart[2] { toleftti, toleftth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _toleft_d = children[1].toleft();
					IEnumerable<TreeBodyPart> _toleft_u = children[0].toleft();
					foreach (var toleftd in _toleft_d)
					{
						foreach (var toleftu in _toleft_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, (0.0 - 1.0), 0.0, 0.0, s, jt, st, true, new TreeBodyPart[2] { toleftu, toleftd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _toleft_r = children[1].toleft();
					IEnumerable<TreeBodyPart> _toleft_l = children[0].toleft();
					foreach (var toleftr in _toleft_r)
					{
						foreach (var toleftl in _toleft_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, (0.0 - 1.0), 0.0, 0.0, s, jt, st, true, new TreeBodyPart[2] { toleftl, toleftr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _toleft_h = children[0].toleft();
					foreach (var tolefth in _toleft_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, (0.0 - 1.0), 0.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { tolefth });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toleft_e = children[0].toleft();
					foreach (var tolefte in _toleft_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, (0.0 - 1.0), 0.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { tolefte });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toleft_w = children[0].toleft();
					foreach (var toleftw in _toleft_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, (0.0 - 1.0), 0.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { toleftw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toleft_h = children[0].toleft();
					foreach (var tolefth in _toleft_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, (0.0 - 1.0), 0.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { tolefth });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toleft_k = children[0].toleft();
					foreach (var toleftk in _toleft_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, (0.0 - 1.0), 0.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { toleftk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toleft_a = children[0].toleft();
					foreach (var tolefta in _toleft_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, (0.0 - 1.0), 0.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { tolefta });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toleft_f = children[0].toleft();
					foreach (var toleftf in _toleft_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, (0.0 - 1.0), 0.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { toleftf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((jt && true))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, (0.0 - 1.0), 0.0, 0.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, (0.0 - 1.0), 0.0, 0.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, (0.0 - 1.0), 0.0, 0.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, (0.0 - 1.0), 0.0, 0.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _toleft_r = children[2].toleft();
					IEnumerable<TreeBodyPart> _toleft_c = children[1].toleft();
					IEnumerable<TreeBodyPart> _toleft_l = children[0].toleft();
					foreach (var toleftr in _toleft_r)
					{
						foreach (var toleftc in _toleft_c)
						{
							foreach (var toleftl in _toleft_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { toleftl, toleftc, toleftr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toleft_th = children[1].toleft();
					IEnumerable<TreeBodyPart> _toleft_ti = children[0].toleft();
					foreach (var toleftth in _toleft_th)
					{
						foreach (var toleftti in _toleft_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, jt, st, at, new TreeBodyPart[2] { toleftti, toleftth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _toleft_d = children[1].toleft();
					IEnumerable<TreeBodyPart> _toleft_u = children[0].toleft();
					foreach (var toleftd in _toleft_d)
					{
						foreach (var toleftu in _toleft_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { toleftu, toleftd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _toleft_r = children[1].toleft();
					IEnumerable<TreeBodyPart> _toleft_l = children[0].toleft();
					foreach (var toleftr in _toleft_r)
					{
						foreach (var toleftl in _toleft_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, st, at, new TreeBodyPart[2] { toleftl, toleftr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _toleft_h = children[0].toleft();
					foreach (var tolefth in _toleft_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, x, y, z, s, jt, st, at, new TreeBodyPart[1] { tolefth });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toleft_e = children[0].toleft();
					foreach (var tolefte in _toleft_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, st, at, new TreeBodyPart[1] { tolefte });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toleft_w = children[0].toleft();
					foreach (var toleftw in _toleft_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, st, at, new TreeBodyPart[1] { toleftw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toleft_h = children[0].toleft();
					foreach (var tolefth in _toleft_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, jt, st, at, new TreeBodyPart[1] { tolefth });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toleft_k = children[0].toleft();
					foreach (var toleftk in _toleft_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, jt, st, at, new TreeBodyPart[1] { toleftk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toleft_a = children[0].toleft();
					foreach (var tolefta in _toleft_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, jt, st, at, new TreeBodyPart[1] { tolefta });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toleft_f = children[0].toleft();
					foreach (var toleftf in _toleft_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, y, z, s, jt, st, at, new TreeBodyPart[1] { toleftf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((!((jt && true))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> toright()
		{
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _toright_r = children[2].toright();
					IEnumerable<TreeBodyPart> _toright_c = children[1].toright();
					IEnumerable<TreeBodyPart> _toright_l = children[0].toright();
					foreach (var torightr in _toright_r)
					{
						foreach (var torightc in _toright_c)
						{
							foreach (var torightl in _toright_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, 1.0, 0.0, 0.0, s, jt, st, true, new TreeBodyPart[3] { torightl, torightc, torightr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toright_th = children[1].toright();
					IEnumerable<TreeBodyPart> _toright_ti = children[0].toright();
					foreach (var torightth in _toright_th)
					{
						foreach (var torightti in _toright_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, 1.0, 0.0, 0.0, s, jt, st, true, new TreeBodyPart[2] { torightti, torightth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _toright_d = children[1].toright();
					IEnumerable<TreeBodyPart> _toright_u = children[0].toright();
					foreach (var torightd in _toright_d)
					{
						foreach (var torightu in _toright_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, 1.0, 0.0, 0.0, s, jt, st, true, new TreeBodyPart[2] { torightu, torightd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _toright_r = children[1].toright();
					IEnumerable<TreeBodyPart> _toright_l = children[0].toright();
					foreach (var torightr in _toright_r)
					{
						foreach (var torightl in _toright_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, 1.0, 0.0, 0.0, s, jt, st, true, new TreeBodyPart[2] { torightl, torightr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((jt && true))
				{
					IEnumerable<TreeBodyPart> _toright_h = children[0].toright();
					foreach (var torighth in _toright_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, 1.0, 0.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { torighth });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toright_e = children[0].toright();
					foreach (var torighte in _toright_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, 1.0, 0.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { torighte });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toright_w = children[0].toright();
					foreach (var torightw in _toright_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, 1.0, 0.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { torightw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toright_h = children[0].toright();
					foreach (var torighth in _toright_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, 1.0, 0.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { torighth });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toright_k = children[0].toright();
					foreach (var torightk in _toright_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, 1.0, 0.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { torightk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toright_a = children[0].toright();
					foreach (var torighta in _toright_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, 1.0, 0.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { torighta });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((jt && st))
				{
					IEnumerable<TreeBodyPart> _toright_f = children[0].toright();
					foreach (var torightf in _toright_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, 1.0, 0.0, 0.0, s, jt, st, true, new TreeBodyPart[1] { torightf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((jt && true))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, 1.0, 0.0, 0.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, 1.0, 0.0, 0.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, 1.0, 0.0, 0.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((jt && st))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, 1.0, 0.0, 0.0, s, jt, st, true, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.SpineShoulder)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _toright_r = children[2].toright();
					IEnumerable<TreeBodyPart> _toright_c = children[1].toright();
					IEnumerable<TreeBodyPart> _toright_l = children[0].toright();
					foreach (var torightr in _toright_r)
					{
						foreach (var torightc in _toright_c)
						{
							foreach (var torightl in _toright_l)
							{
								yield return TreeBodyPart.MakeTree(BodyPart.SpineShoulder, x, y, z, s, jt, st, at, new TreeBodyPart[3] { torightl, torightc, torightr });
							}
						}
					}
				}
			}
			if (this.symbol == BodyPart.Hand)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toright_th = children[1].toright();
					IEnumerable<TreeBodyPart> _toright_ti = children[0].toright();
					foreach (var torightth in _toright_th)
					{
						foreach (var torightti in _toright_ti)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.Hand, x, y, z, s, jt, st, at, new TreeBodyPart[2] { torightti, torightth });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineMid)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _toright_d = children[1].toright();
					IEnumerable<TreeBodyPart> _toright_u = children[0].toright();
					foreach (var torightd in _toright_d)
					{
						foreach (var torightu in _toright_u)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineMid, x, y, z, s, jt, st, at, new TreeBodyPart[2] { torightu, torightd });
						}
					}
				}
			}
			if (this.symbol == BodyPart.SpineBase)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _toright_r = children[1].toright();
					IEnumerable<TreeBodyPart> _toright_l = children[0].toright();
					foreach (var torightr in _toright_r)
					{
						foreach (var torightl in _toright_l)
						{
							yield return TreeBodyPart.MakeTree(BodyPart.SpineBase, x, y, z, s, jt, st, at, new TreeBodyPart[2] { torightl, torightr });
						}
					}
				}
			}
			if (this.symbol == BodyPart.Neck)
			{
				if ((!((jt && true))))
				{
					IEnumerable<TreeBodyPart> _toright_h = children[0].toright();
					foreach (var torighth in _toright_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Neck, x, y, z, s, jt, st, at, new TreeBodyPart[1] { torighth });
					}
				}
			}
			if (this.symbol == BodyPart.Shoulder)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toright_e = children[0].toright();
					foreach (var torighte in _toright_e)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Shoulder, x, y, z, s, jt, st, at, new TreeBodyPart[1] { torighte });
					}
				}
			}
			if (this.symbol == BodyPart.Elbow)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toright_w = children[0].toright();
					foreach (var torightw in _toright_w)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Elbow, x, y, z, s, jt, st, at, new TreeBodyPart[1] { torightw });
					}
				}
			}
			if (this.symbol == BodyPart.Wrist)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toright_h = children[0].toright();
					foreach (var torighth in _toright_h)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Wrist, x, y, z, s, jt, st, at, new TreeBodyPart[1] { torighth });
					}
				}
			}
			if (this.symbol == BodyPart.Hip)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toright_k = children[0].toright();
					foreach (var torightk in _toright_k)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Hip, x, y, z, s, jt, st, at, new TreeBodyPart[1] { torightk });
					}
				}
			}
			if (this.symbol == BodyPart.Knee)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toright_a = children[0].toright();
					foreach (var torighta in _toright_a)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Knee, x, y, z, s, jt, st, at, new TreeBodyPart[1] { torighta });
					}
				}
			}
			if (this.symbol == BodyPart.Ankle)
			{
				if ((!((jt && st))))
				{
					IEnumerable<TreeBodyPart> _toright_f = children[0].toright();
					foreach (var torightf in _toright_f)
					{
						yield return TreeBodyPart.MakeTree(BodyPart.Ankle, x, y, z, s, jt, st, at, new TreeBodyPart[1] { torightf });
					}
				}
			}
			if (this.symbol == BodyPart.Head)
			{
				if ((!((jt && true))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Head, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.HandTip)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.HandTip, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Thumb)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Thumb, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (this.symbol == BodyPart.Foot)
			{
				if ((!((jt && st))))
				{
					yield return TreeBodyPart.MakeTree(BodyPart.Foot, x, y, z, s, jt, st, at, new TreeBodyPart[0] { });
				}
			}
			if (false) yield return null;
		}

		public IEnumerable<TreeBodyPart> t1()
		{
			List<TreeBodyPart> o1 = new List<TreeBodyPart>();
			List<TreeBodyPart> o2 = new List<TreeBodyPart>();
			List<TreeBodyPart> o3 = new List<TreeBodyPart>();
			List<TreeBodyPart> o4 = new List<TreeBodyPart>();
			o4.AddRange(this.rotFroClock());
			List<TreeBodyPart> o5 = new List<TreeBodyPart>();
			foreach (var v1 in o4)
			{
				o5.AddRange(v1.rotFroCClock());
			}
			o3 = o5;
			List<TreeBodyPart> o6 = new List<TreeBodyPart>();
			foreach (var v1 in o3)
			{
				o6.AddRange(v1.rotFroClock());
			}
			o2 = o6;
			List<TreeBodyPart> o7 = new List<TreeBodyPart>();
			foreach (var v1 in o2)
			{
				o7.AddRange(v1.leftst());
			}
			o1 = o7;
			List<TreeBodyPart> o8 = new List<TreeBodyPart>();
			foreach (var v1 in o1)
			{
				o8.AddRange(v1.wrist());
			}
			return o8;
		}
		public IEnumerable<TreeBodyPart> t2()
		{
			List<TreeBodyPart> o1 = new List<TreeBodyPart>();
			List<TreeBodyPart> o2 = new List<TreeBodyPart>();
			List<TreeBodyPart> o3 = new List<TreeBodyPart>();
			List<TreeBodyPart> o4 = new List<TreeBodyPart>();
			o4.AddRange(this.rotFroClock());
			List<TreeBodyPart> o5 = new List<TreeBodyPart>();
			foreach (var v1 in o4)
			{
				o5.AddRange(v1.rotFroClock());
			}
			o3 = o5;
			List<TreeBodyPart> o6 = new List<TreeBodyPart>();
			foreach (var v1 in o3)
			{
				o6.AddRange(v1.rotFroCClock());
			}
			o2 = o6;
			List<TreeBodyPart> o7 = new List<TreeBodyPart>();
			foreach (var v1 in o2)
			{
				o7.AddRange(v1.leftst());
			}
			o1 = o7;
			List<TreeBodyPart> o8 = new List<TreeBodyPart>();
			foreach (var v1 in o1)
			{
				o8.AddRange(v1.wrist());
			}
			return o8;
		}
		public IEnumerable<TreeBodyPart> t3()
		{
			List<TreeBodyPart> o1 = new List<TreeBodyPart>();
			List<TreeBodyPart> o2 = new List<TreeBodyPart>();
			List<TreeBodyPart> o3 = new List<TreeBodyPart>();
			List<TreeBodyPart> o4 = new List<TreeBodyPart>();
			o4.AddRange(this.rotFroClock());
			List<TreeBodyPart> o5 = new List<TreeBodyPart>();
			foreach (var v1 in o4)
			{
				o5.AddRange(v1.rotFroClock());
			}
			o3 = o5;
			List<TreeBodyPart> o6 = new List<TreeBodyPart>();
			foreach (var v1 in o3)
			{
				o6.AddRange(v1.rotFroClock());
			}
			o2 = o6;
			List<TreeBodyPart> o7 = new List<TreeBodyPart>();
			foreach (var v1 in o2)
			{
				o7.AddRange(v1.leftst());
			}
			o1 = o7;
			List<TreeBodyPart> o8 = new List<TreeBodyPart>();
			foreach (var v1 in o1)
			{
				o8.AddRange(v1.wrist());
			}
			return o8;
		}
		public IEnumerable<TreeBodyPart> crossover_arm_stretch()
		{
			List<TreeBodyPart> o1 = new List<TreeBodyPart>();
			List<TreeBodyPart> o2 = new List<TreeBodyPart>();
			List<TreeBodyPart> o3 = new List<TreeBodyPart>();
			List<TreeBodyPart> o4 = new List<TreeBodyPart>();
			List<TreeBodyPart> o5 = new List<TreeBodyPart>();
			List<TreeBodyPart> o6 = new List<TreeBodyPart>();
			List<TreeBodyPart> o7 = new List<TreeBodyPart>();
			List<TreeBodyPart> o8 = new List<TreeBodyPart>();
			List<TreeBodyPart> o9 = new List<TreeBodyPart>();
			List<TreeBodyPart> o10 = new List<TreeBodyPart>();
			List<TreeBodyPart> o11 = new List<TreeBodyPart>();
			o11.AddRange(this.rightst());
			List<TreeBodyPart> o12 = new List<TreeBodyPart>();
			foreach (var v1 in o11)
			{
				o12.AddRange(v1.wrist());
			}
			o10 = o12;
			List<TreeBodyPart> o13 = new List<TreeBodyPart>();
			foreach (var v1 in o10)
			{
				o13.AddRange(v1.elbow());
			}
			o9 = o13;
			List<TreeBodyPart> o14 = new List<TreeBodyPart>();
			foreach (var v1 in o9)
			{
				o14.AddRange(v1.toleft());
			}
			o8 = o14;
			List<TreeBodyPart> o15 = new List<TreeBodyPart>();
			foreach (var v1 in o8)
			{
				o15.AddRange(v1.resetjt());
			}
			o7 = o15;
			List<TreeBodyPart> o16 = new List<TreeBodyPart>();
			foreach (var v1 in o7)
			{
				o16.AddRange(v1.resetst());
			}
			o6 = o16;
			List<TreeBodyPart> o17 = new List<TreeBodyPart>();
			foreach (var v1 in o6)
			{
				o17.AddRange(v1.leftst());
			}
			o5 = o17;
			List<TreeBodyPart> o18 = new List<TreeBodyPart>();
			foreach (var v1 in o5)
			{
				o18.AddRange(v1.elbow());
			}
			o4 = o18;
			List<TreeBodyPart> o19 = new List<TreeBodyPart>();
			foreach (var v1 in o4)
			{
				o19.AddRange(v1.todown());
			}
			o3 = o19;
			List<TreeBodyPart> o20 = new List<TreeBodyPart>();
			foreach (var v1 in o3)
			{
				o20.AddRange(v1.resetjt());
			}
			o2 = o20;
			List<TreeBodyPart> o21 = new List<TreeBodyPart>();
			foreach (var v1 in o2)
			{
				o21.AddRange(v1.wrist());
			}
			o1 = o21;
			List<TreeBodyPart> o22 = new List<TreeBodyPart>();
			foreach (var v1 in o1)
			{
				o22.AddRange(v1.toup());
			}
			return o22;
		}
		public bool issafe()
		{
			return this.langsLabel.Contains("issafe");
		}
		public IEnumerable<TreeBodyPart> crossover_arm_stretch_safe()
		{
			bool b1;
			b1 = this.issafe();
			List<TreeBodyPart> o2 = new List<TreeBodyPart>();
			if (b1)
			{
				List<TreeBodyPart> o3 = new List<TreeBodyPart>();
				o3.AddRange(this.crossover_arm_stretch());
				foreach (var v1 in o3)
				{
					bool b4;
					b4 = v1.issafe();
					if (b4)
						o2.Add(v1);
				}
			}
			return o2;
		}
		public bool crossover_arm_stretch_safe_dom()
		{
			List<TreeBodyPart> o1 = new List<TreeBodyPart>();
			o1.AddRange(this.crossover_arm_stretch_safe());
			return o1.Count > 0;
		}
		public bool crossover_arm_stretch_dom()
		{
			List<TreeBodyPart> o1 = new List<TreeBodyPart>();
			o1.AddRange(this.crossover_arm_stretch());
			return o1.Count > 0;
		}
		public IEnumerable<TreeBodyPart> crossover_arm_stretch_lbody()
		{
			bool b1;
			b1 = this.lbody();
			List<TreeBodyPart> o2 = new List<TreeBodyPart>();
			if (b1)
			{
				o2.AddRange(this.crossover_arm_stretch());
			}
			return o2;
		}
		public bool crossover_arm_stretch_lbody_dom()
		{
			List<TreeBodyPart> o1 = new List<TreeBodyPart>();
			o1.AddRange(this.crossover_arm_stretch_lbody());
			return o1.Count > 0;
		}
		public bool lbody_test()
		{
			return this.lbody();
		}
	}

}
