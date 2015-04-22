//////////////////////////////////////////////////////////////////////	
// hello world with the punch gesture
//////////////////////////////////////////////////////////////////////

APP simple:         
    GESTURE generic-left-punch : 
        POSE prepare-punch :
            put your left elbow behind your neck.

        POSE execute-left-punch : 
            put your left elbow in front of your neck.

        EXECUTION : 
            prepare-punch,
            rapidly execute-left-punch.
    