// this takes too long to synthesize the next 
// pose and needs debugging with Z3
APP sample-gesture-app:
	GESTURE sample-gesture:
		POSE handoh:
		put your right hand above your head.

		POSE righthf:
		put your right wrist in front of your left wrist.

		POSE lefthf:
		put your left wrist in front of your right wrist.

		EXECUTION:
		handoh,
		righthf,
		lefthf,
		righthf,
		lefthf,
		righthf,
		lefthf,
		righthf,
		lefthf,
		righthf,
		lefthf,
		righthf,
		lefthf,
		righthf,
		lefthf.
