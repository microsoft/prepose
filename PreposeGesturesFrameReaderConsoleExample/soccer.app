//////////////////////////////////////////////////////////////////////	
// Two common crowd gestures in soccer stadiums
// Ola and Uuuuh
//////////////////////////////////////////////////////////////////////

APP soccer:
	
	// so let's create a gesture
	GESTURE ola:

		// which is composed by poses
		POSE wrists_down:

		// point transforms and body parts
		point your left wrist down,
		point your right wrist down,

		// pointing down is not natural, rotation needed
		rotate your left wrist 30 degrees to your front,
		rotate your right wrist 30 degrees to your front,

		point your head up,

		// polish the gesture
		rotate your head 20 degrees to your front.		
		
		POSE ola_middle:
		// groups of body parts
		point your wrists to your front.

		POSE wrists_up:
		//put your hands above your head.
		point your wrists up,
		point your head up.

		// let's test it
		// feedback, lines and bars
		EXECUTION:
		wrists_down,

		// we need more steps
		ola_middle,
		wrists_up,

		// complete the execution
		ola_middle,
		wrists_down.

	// more gestures
	GESTURE uuuuuh:

		// another pose more complex with chained rotations
		POSE uuuuuh_hands_on_head:
		//point your elbows to your front,
		//rotate your left elbow 45 degrees to your left,
		//rotate your right elbow 45 degrees to your right,
		//point your wrists up,
		//rotate your wrists 30 degrees to your back,
		//rotate your left wrist 30 degrees to your right,
		//rotate your right wrist 30 degrees to your left.
		// better coded with restrictions (relations)
		//put your hands above your head.
		touch your head with your right hand,
		touch your head with your left hand.

		EXECUTION:
		// reusing previous pose
		wrists_down,
		uuuuuh_hands_on_head.

    
