using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace JSONParser
{
    public static class RichTextBoxExtensions
    {
        public static string GetText(this RichTextBox richTextBox)
        {
            return new TextRange(richTextBox.Document.ContentStart,
                richTextBox.Document.ContentEnd).Text;
        }
    }
}
