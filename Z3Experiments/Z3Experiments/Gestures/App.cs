using Antlr4.Runtime;
using PreposeGestures.Parser;
using System;
using System.Collections.Generic;

namespace PreposeGestures
{
    /// <summary>
    /// Representation of the parsed code 
    /// </summary>
	public class App
	{
		public IList<Gesture> Gestures = new List<Gesture>();
        public App(string name)
        {
			this.Name = name;
		}

        public App(string name, IList<Gesture> gestures, int precision = 15)
		{
			this.Name = name;
            this.Gestures = gestures;
		}

		public string Name { get; private set; }

		public override string ToString()
		{
			return string.Format("APP {0} = \n\t{1}", Name, string.Join("\n\n\t", this.Gestures));
		}

		public static App ReadApp(string filename)
		{
			var input = new Antlr4.Runtime.AntlrFileStream(filename);    //"..\\..\\Tests\\simple.app"
			//var input = new Antlr4.Runtime.AntlrInputStream(inputString);    //"..\\..\\Tests\\simple.app"
			var lexer = new PreposeGesturesLexer(input);
			var tokens = new CommonTokenStream(lexer);
			var parser = new PreposeGesturesParser(tokens);
			var tree = parser.app(); // parse
			var visitor = new AppConverter();
			var app = (App)visitor.Visit(tree);

			return app;
		}

		public static App ReadAppText(string inputString)
		{
			//var input = new Antlr4.Runtime.AntlrFileStream(filename);    //"..\\..\\Tests\\simple.app"
			var input = new Antlr4.Runtime.AntlrInputStream(inputString);    //"..\\..\\Tests\\simple.app"
			var lexer = new PreposeGesturesLexer(input);
			var tokens = new CommonTokenStream(lexer);
			var parser = new PreposeGesturesParser(tokens);
			var tree = parser.app(); // parse
			var visitor = new AppConverter();
			var app = (App)visitor.Visit(tree);

			return app;
		}
    }

	
}
