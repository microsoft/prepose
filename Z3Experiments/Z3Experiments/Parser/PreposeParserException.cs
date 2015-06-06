using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreposeGestures
{
    public class PreposeParserException : Exception
    {
        private readonly int startLineNumber;
        private readonly int startColumnNumber;

        private readonly int endLineNumber;
        private readonly int endColumnNumber;

        public int StartLineNumber
        {
            get { return this.startLineNumber; }
        }

        public int StartColumnNumber
        {
            get { return this.startColumnNumber; }
        }

        public int EndLineNumber
        {
            get { return this.endLineNumber; }
        }

        public int EndColumnNumber
        {
            get { return this.endColumnNumber; }
        }

        public PreposeParserException(string message, ParserRuleContext currentContext)
            : base(message)
        {
            this.startLineNumber = currentContext.start.Line;
            this.startColumnNumber = currentContext.start.Column;

            this.endLineNumber = currentContext.stop.Line;
            this.endColumnNumber = currentContext.stop.Column;
        }
    }
}
