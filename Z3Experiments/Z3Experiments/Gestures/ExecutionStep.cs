namespace PreposeGestures
{
    public enum MotionRestriction
    {
        None = 0,
        Slowly = 1,
        Rapidly = 2,
    }

    public class ExecutionStep
    {
        public ExecutionStep(MotionRestriction motionRestriction, Pose pose, int holdRestriction)
        {
            this.MotionRestriction = motionRestriction;
            this.Pose = pose;
            this.HoldRestriction = holdRestriction;
        }

        public ExecutionStep(MotionRestriction motionRestriction, Pose pose)
        {
            this.MotionRestriction = motionRestriction;
            this.Pose = pose;
            this.HoldRestriction = -1;
        }

        public ExecutionStep(Pose pose, int holdRestriction)
        {
            this.MotionRestriction = MotionRestriction.None;
            this.Pose = pose;
            this.HoldRestriction = holdRestriction;
        }

        public ExecutionStep(Pose pose)
        {
            this.MotionRestriction = MotionRestriction.None;
            this.Pose = pose;
            this.HoldRestriction = -1;
        }

        public override string ToString()
        {
            if (this.MotionRestriction != PreposeGestures.MotionRestriction.None && this.HoldRestriction > 0)
            {
                return string.Format("{0} {1} and hold for {2} seconds", this.MotionRestriction, this.Pose.Name, this.HoldRestriction);
            }

            if (this.MotionRestriction != PreposeGestures.MotionRestriction.None && this.HoldRestriction > 0)
            {
                return string.Format("{0} {1} and hold for {2} seconds", this.MotionRestriction, this.Pose.Name, this.HoldRestriction);
            }
            if (this.MotionRestriction != PreposeGestures.MotionRestriction.None)
            {
                return string.Format("{0} {1}", this.MotionRestriction, this.Pose.Name);
            }
            if (this.HoldRestriction > 0)
            {
                return string.Format("{0} and hold for {1} seconds", this.Pose.Name, this.HoldRestriction);
            }

            return this.Pose.Name;
        }

        public Pose Pose { get; private set; }
        public MotionRestriction MotionRestriction { get; private set; }
        public int HoldRestriction { get; private set; }
    }    
}
