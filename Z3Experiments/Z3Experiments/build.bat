REM this is for building the C# files from the ANTLR4 .g4 file called PreposeGestures.g4

@echo off

REM create absolute classpath
SET oldClasspath=%CLASSPATH%
SET AROS_BASE=D:\work\SurroundWeb\private\AROS\GestureRecognizerKinectV2\
REM F:\MSRWorkspaces\XCGExp\AR_Authoring\private\AROS\GestureRecognizerKinectV2
SET cp=%AROS_BASE%\External\antlr4-csharp-4.2.2-SNAPSHOT-complete.jar
REM SET cp=%AROS_BASE%\External\antlr4-csharp-4.2.2-SNAPSHOT-complete.jar
SET CLASSPATH=.;%cp%;%CLASSPATH%

echo Generating C# Parser in Parser...

java org.antlr.v4.Tool -o Parser -Dlanguage=CSharp_v4_5 -visitor -package PreposeGestures %AROS_BASE%\Z3Experiments\Z3Experiments\Parser\PreposeGestures.g4

SET CLASSPATH=%oldClasspath%