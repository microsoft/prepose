//////////////////////////////////////////////////////////////////////	
// Gestures described based on conditioning programs
// providade by American Acadmey of Orthipaedic Surgeons (AAOS)
// http://orthoinfo.aaos.org/topic.cfm?topic=A00672
//////////////////////////////////////////////////////////////////////	


APP therapy:

    // for this gesture the user needs to have a support table
    // placing the other hand over the table to help the balance
    GESTURE pendulum:
        POSE stand-straight:
        point your spine, neck and head up.             

        POSE lean-forward:            
        rotate your spine, neck and head 30 degrees to your front,
        don't align your legs,
        align your back.

        POSE swing-forward:
        point your left arm down,
        rotate your left arm 30 degrees to your front.

        POSE swing-back:
        point your left arm down,
        rotate your left arm 30 degrees to your back.
        
        POSE swing-left:
        point your left arm down,
        rotate your left arm 30 degrees to your left.

        POSE swing-right:
        point your left arm down,
        rotate your left arm 30 degrees to your right.
        
        EXECUTION:
        stand-straight,
        lean-forward,
        slowly swing-forward,
        slowly swing-back,
        slowly swing-left,
        slowly swing-right,
        slowly swing-forward,
        slowly swing-left,
        slowly swing-back,
        slowly swing-right.
           
    GESTURE crossover-left-arm-stretch:
        POSE relax-arms:
        point your left arm down,
        point your right arm down.

        POSE stretch: 
        rotate your left arm 90 degrees counter clockwise on the frontal plane,            
        touch your left elbow with your right hand.

        EXECUTION:
        relax-arms,
        slowly stretch and hold for 30 seconds.

	GESTURE crossover-right-arm-stretch:
        POSE stretch-reverse: 
        rotate your right arm 90 degrees counter clockwise on the frontal plane,
        touch your right elbow with your left hand.

        EXECUTION:
        relax-arms,
        slowly stretch-reverse and hold for 30 seconds.
	
	//GESTURE  = MIRROR crossover-left-arm-stretch;

    GESTURE passive-external-rotation:
        POSE left-wrist-to-the-left:
        point your elbows down,
        point your wrists to your front,
        rotate your left wrist 45 degrees counter clockwise on the horizontal plane.

        POSE right-wrist-to-the-right:
        point your elbows down,
        point your wrists to your front,
        rotate your right wrist 45 degrees counter clockwise on the horizontal plane.

        EXECUTION:
        left-wrist-to-the-left and hold for 30 seconds,
        right-wrist-to-the-right and hold for 30 seconds,
        left-wrist-to-the-left and hold for 30 seconds,
        right-wrist-to-the-right and hold for 30 seconds.

    GESTURE elbows-flexion:
        POSE flex-left-elbow: 
        rotate your left wrist 180 degrees clockwise on the sagittal plane,
        point your left elbow down.

        POSE flex-right-elbow : 
        rotate your right wrist 180 degrees clockwise on the sagittal plane,
        point your right elbow down.

        EXECUTION:
        relax-arms,
        slowly flex-left-elbow and hold for 2 seconds,
        relax-arms,
        slowly flex-right-elbow and hold for 2 seconds.

    GESTURE elbows-extension:
        POSE stretch-arm-up: 
        point your left elbow up,
        point your right arm up,
        touch your right elbow with your left hand,
        align your spine.

        POSE flex-arm-back: 
        point your elbows up,        
        point your right wrist back,
        touch your right elbow with your left hand,
        align your spine.

        EXECUTION:
        stand-straight,
        flex-arm-back,
        slowly stretch-arm-up and hold for 2 seconds,
        slowly flex-arm-back.

    GESTURE head-rolls:
        POSE head-to-left:
        point your head up,
        rotate your head 20 degrees clockwise on the frontal plane.
        
        POSE head-to-right:
        point your head up,
        rotate your head 20 degrees counter clockwise on the frontal plane.

        POSE head-to-front:
        point your head up,
        rotate your head 20 degrees clockwise on the sagittal plane.
            
        POSE head-to-back :
        point your head up,
        rotate your head 20 degrees counter clockwise on the sagittal plane.

        EXECUTION :
        head-to-front,
        head-to-right and hold for 5 seconds,
        head-to-front,
        slowly head-to-left and hold for 5 seconds,
        slowly head-to-back,
        slowly head-to-right,
        slowly head-to-front,
        slowly head-to-left,
        slowly head-to-back,
        slowly head-to-right,
        slowly head-to-front,
        slowly head-to-left.

    GESTURE standing-quadriceps-stretch:
        POSE lift-ankle: 
        point your legs down,
        rotate your right ankle 90 degrees to your back,
        align your spine.

        POSE catch-ankle:
        touch your right ankle with your right hand,
        align your spine.

        POSE bend-leg:
        touch your right ankle with your right hand,
        rotate your right ankle 15 degrees to up,
        align your spine.

        EXECUTION:
        stand-straight,
        lift-ankle,
        catch-ankle,
        slowly bend-leg and hold for 30 seconds.

    GESTURE half-squats:
        POSE up-squat:
        point your legs down,
        point your arms to your front,
        point your spine up.

        POSE down-squat:
        point your arms to your front,
        rotate your knees 20 degrees to up,
        point your spine up.        

        EXECUTION:
        up-squat,
        down-squat and hold for 5 seconds,
        up-squat,
        down-squat and hold for 5 seconds.

	GESTURE shoulder-abduction:
		POSE arm-down:
		point your right arm down,
		rotate your right arm 20 degrees to your right.

		// poses as pure transformations from previous state
		POSE rot-up:
		rotate your right arm 20 degrees up.

		POSE rot-down:
		rotate your right arm 20 degrees down.

		EXECUTION:
		arm-down,
		//using repetitions to get further
		rot-up,
		rot-up,
		rot-up,
		rot-up,
		rot-up,
		rot-down,
		rot-down,
		rot-down,
		rot-down,
		rot-down.

    GESTURE shoulder-adduction:
        POSE arm-down-add:
        point your right arm down,
        rotate your right arm 30 degrees to your front.

        EXECUTION:
        arm-down,
        //using repetitions to get further
        rot-up,
        rot-up,
        rot-up,
        rot-up,
        rot-up,
        rot-down,
        rot-down,
        rot-down,
        rot-down,
        rot-down.

    //GESTURE standing-iliotibial-band-stretch:
    //    POSE cross-legs:
    //    point your right leg down,
    //    put your left foot behind your right foot,
    //    put your left foot to the right of your right foot.
    //
    //    EXECUTION:
    //    stand-straight,
    //    cross-legs and hold for 30 seconds.

	// ((half-squats x 3) | standing-quadriceps-stretch ) x 20
