using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.Kinect.GestureRecognizer
{

    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using System.Collections.Generic;

    //public enum Move {
    //    Stay, StepLeft, StepRight, LiftLeftArm2, LiftRightArm2, LiftLeftArm, LiftRightArm, LiftLeftLowerArm, LiftRightLowerArm,
    //    DownLeftArm, DownRightArm, DownLeftLowerArm, DownRightLowerArm, CollectLeft, CollectRight
    //};   


    //public enum Move
    //{
    //    Stay,
    //    LiftLeftArmFrontalPlane, LiftRightArmFrontalPlane, LiftLeftLowerArmFrontalPlane, LiftRightLowerArmFrontalPlane, 
    //    DownLeftArmFrontalPlane, DownRightArmFrontalPlane, DownLeftLowerArmFrontalPlane, DownRightLowerArmFrontalPlane,
    //    LiftLeftArmSagittalPlane, LiftRightArmSagittalPlane, LiftLeftLowerArmSagittalPlane, LiftRightLowerArmSagittalPlane,
    //    DownLeftArmSagittalPlane, DownRightArmSagittalPlane, DownLeftLowerArmSagittalPlane, DownRightLowerArmSagittalPlane,
    //    MoveLeftLeftLowerArm, MoveLeftRightLowerArm, MoveRightLeftLowerArm, MoveRightRightLowerArm,
    //    PointLeftArmDown, PointRightArmDown, PointNeckUp, RotateLeftNeck, RotateRightNeck
    //};

  public enum Transduction
  {
    Stay,
    ToLeft, 
    ToRight, 
    ToUp, 
    ToDown, 
    ToFront, 
    ToBack, 
    RotFroClock, 
    RotFroCClock, 
    RotSagClock, 
    RotSagCClock, 
    RotHorClock, 
    RotHorCClock,
    LeftST, 
    RightST, 
    ResetJT, 
    ResetST, 
    ResetAT,
    SpineShoulder, 
    SpineBase, 
    Hand, 
    Neck, 
    Shoulder, 
    Elbow, 
    Wrist, 
    Hip, 
    Knee, 
    Head, 
    HandTip, 
    Thumb, 
    Foot
  };

    public static class TransductionUtil{

        private const int THICKNESS=3;

        private static Pen pen = new Pen(Brushes.LightGray, THICKNESS);

        public static List<Transduction> GetAll()
        {
            List<Transduction> moves = Enum.GetValues(typeof(Transduction)).Cast<Transduction>().ToList();
            moves.Remove(Transduction.Stay);
            return moves;
        }

        public static List<Transduction> Complement(Transduction m){
            List<Transduction> moves = Enum.GetValues(typeof(Transduction)).Cast<Transduction>().ToList();
            moves.Remove(m);
            moves.Remove(Transduction.Stay);
            return moves;
        }

        public static char ToChar(Transduction m)
        {
            List<Transduction> moves = Enum.GetValues(typeof(Transduction)).Cast<Transduction>().ToList();
            return (char)(moves.IndexOf(m)+96);
        }

        public static Transduction ToMove(Char m)
        {
            List<Transduction> moves = Enum.GetValues(typeof(Transduction)).Cast<Transduction>().ToList();
            return moves[((int) m)-96];
        }

        public static Pen ToColor(Transduction m)
        {            
            return pen;
        }

    }

}
