grammar PreposeGestures;

app :
	'APP' ID ':' (gesture '.')+ EOF;
gesture :
	'GESTURE' ID ':' 
	pose+   // declaring the poses
	execution;      // defining an order + repeatitions
pose :
	'POSE' ID ':' statement (',' statement)* '.';

statement :
	transform | restriction;

transform :
	point_to_transform 
	| rotate_plane_transform
	| rotate_direction_transform;

rotate_plane_transform :  
	'rotate' 'your' body_part ((',' 'your'? body_part)* 'and' 'your'? body_part)? NUMBER 'degrees' angular_direction 'on' 'the' ? reference_plane;

rotate_direction_transform :  
	'rotate' 'your' body_part ((',' 'your'? body_part)* 'and' 'your'? body_part)? NUMBER 'degrees' ('to' | 'to' 'your')? direction;

point_to_transform :    
	'point' 'your'? body_part ((',' 'your'? body_part)* 'and' 'your'? body_part)? ('to' | 'to' 'your')? direction;

body_part :
	joint | 
	arm |
	leg |
	spine |
	back |
	arms |
	legs |
	shoulders |
	wrists |
	elbows |
	hands |
	hands_tips |
	thumbs |
	hips |
	knees |
	ankles |
	feet |
	you ;

arm : side 'arm';
leg : side 'leg';
spine : 'spine';
back : 'back';
arms : 'arms';
legs : 'legs';
shoulders : 'shoulders';
wrists : 'wrists';
elbows : 'elbows';
hands : 'hands';
hands_tips : 'hands' 'tips';
thumbs : 'thumbs';
hips : 'hips';
knees : 'knees';
ankles : 'ankles';
feet : 'feet';
you : 'you';

//arm and leg should be functions that return the 
//corresponding set of joints

joint :
	center_joint |
	side sided_joint;

center_joint :
	'neck' |
	'head' |
	'spine' 'mid' |
	'spine' 'base' |
	'spine' 'shoulder'; // todo: add more

side :
	'left' |
	'right';

sided_joint :
	'shoulder' |
	'elbow' |
	'wrist' |
	'hand' |
	'hand' 'tip' |
	'thumb' |
	'hip' |
	'knee' |
	'ankle' |
	'foot';

direction :
	'up' 
	| 'down' 
	| 'front' 
	| 'back' 
	| side;
				
angular_direction : 
	'clockwise' |
	'counter' 'clockwise';

reference_plane :
	'frontal' 'plane' |
	'sagittal' 'plane' |
	'horizontal' 'plane';

restriction :
	dont? touch_restriction 
	| dont? put_restriction 
	| dont? align_restriction;

dont:
	'don\'t';

touch_restriction :
	'touch' 'your'? body_part 'with' 'your'? side 'hand';

put_restriction :   
	'put' 'your'? body_part ((',' 'your'? body_part)* 'and' 'your'? body_part)? relative_direction body_part;

align_restriction :
	'align' 'your'? body_part ((',' 'your'? body_part)* 'and' 'your'? body_part)?;

relative_direction :    
	'in' 'front' 'of' 'your' |
	'behind' 'your' |
	(('on' 'top' 'of') | 'above') 'your' |
	'below' 'your' |
	'to' 'the' side 'of' 'your';

execution : 
	'EXECUTION' ':'
	(
		repeat 'the' 'following' 'steps' NUMBER execution_step (',' execution_step)* 
		| execution_step (',' execution_step)*
	);

execution_step :  
	motion_constraint? ID ('and' hold_constraint)?; // pose ID

motion_constraint : 
	'slowly' | 'rapidly';

hold_constraint :   
	'hold' 'for' NUMBER 'seconds';

repeat :        
	'repeat' NUMBER 'times';

NUMBER : DIGIT+;

DIGIT : '0'..'9';

COMMENT : (
	('#'| '//') ~[\r\n]* '\r'? '\n' 
		| '/*' .*? '*/' 
	)  -> skip ;

ID
	: ('a'..'z' | 'A'..'Z' | '_' | '-' | 'a'..'z' | 'A'..'Z' | '$' | DIGIT)+
	;

WS
	: [ \r\n\t] -> skip
	;
