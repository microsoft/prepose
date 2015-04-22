using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PreposeGestureRecognizer
{
	/// <summary>
    /// Implements AvalonEdit ICompletionData interface 
    /// to provide the entries in the
    /// completion drop down.
	/// </summary>
	public class PreposeSyntaxCompletionData : ICompletionData
	{
		public PreposeSyntaxCompletionData(string text)
		{
			this.Text = text;
		}

		public System.Windows.Media.ImageSource Image
		{
			get { return null; }
		}

		public string Text { get; private set; }

		public double Priority { get { return 0; } }

		// Use this property if you want to show a fancy UIElement in the list.
		public object Content
		{
			get { return this.Text; }
		}

		public object Description
		{
            get { return this.Text; }
		}

		public void Complete(TextArea textArea, ISegment completionSegment,
			EventArgs insertionRequestEventArgs)
		{
			textArea.Document.Replace(completionSegment, this.Text);
		}
	}
}
