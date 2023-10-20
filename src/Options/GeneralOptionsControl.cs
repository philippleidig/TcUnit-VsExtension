using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;

namespace TcUnit.Options
{
    public partial class GeneralOptionsControl : UserControl
    {
        private readonly GeneralOptionsPage _optionsPage;

        public GeneralOptionsControl(GeneralOptionsPage optionsPage)
        {
            _optionsPage = optionsPage;
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            string lineEndingOfFile = _optionsPage.TestCaseTemplate.Contains("\r\n") ? "\r\n" : "\n";

            string[] lines = _optionsPage.TestCaseTemplate.Split(
                new[] { lineEndingOfFile },
                StringSplitOptions.None
            );

            richTextBox1.Text = _optionsPage.TestCaseTemplate;
            richTextBox1.Focus();

            applyTextHighlighting();
        }

        private void applyTextHighlighting()
        {
            string keywords = @"\b(IF|CASE|OF|ELSE|ELSIF|THEN|END_IF|OR|AND|NOT|WHILE|FOR|REPEAT|DO|TO|BY|RETURN|EXIT|CONTINUE|GOTO|JMP|BEGIN|COUNTER|GOTO|BS|SIN|ABS|ACOS|ASIN|ATAN|COS|EXP|EXPT|LN|LOG|SIN|SQRT|TAN|SEL|MUX|SHL|SHR|ROL|ROR|ADD|DIV|MUL|SUB|MAX|MIN)\b";
            MatchCollection keywordMatches = Regex.Matches(richTextBox1.Text, keywords);

            string comments = @"(\/\/.+?$|\/\*.+?\*\/)";
            MatchCollection commentMatches = Regex.Matches(richTextBox1.Text, comments, RegexOptions.Multiline);

            string strings = "'.+?'";
            MatchCollection stringMatches = Regex.Matches(richTextBox1.Text, strings);

            // saving the original caret position + forecolor
            int originalIndex = richTextBox1.SelectionStart;
            int originalLength = richTextBox1.SelectionLength;
            Font originalFont = richTextBox1.SelectionFont;
            Color originalColor = Color.Black;

            // removes any previous highlighting (so modified words won't remain highlighted)
            richTextBox1.SelectionStart = 0;
            richTextBox1.SelectionLength = richTextBox1.Text.Length;
            richTextBox1.SelectionFont = new Font("Courier New", 12, FontStyle.Regular);
            richTextBox1.SelectionColor = originalColor;

            // scanning...
            foreach (Match m in keywordMatches)
            {
                richTextBox1.SelectionStart = m.Index;
                richTextBox1.SelectionLength = m.Length;
                richTextBox1.SelectionFont = new Font("Courier New", 12, FontStyle.Bold);
                richTextBox1.SelectionColor = Color.MediumBlue;
            }

            foreach (Match m in commentMatches)
            {
                richTextBox1.SelectionStart = m.Index;
                richTextBox1.SelectionLength = m.Length;
                richTextBox1.SelectionFont = new Font("Courier New", 12, FontStyle.Italic);
                richTextBox1.SelectionColor = Color.Teal;
            }

            foreach (Match m in stringMatches)
            {
                richTextBox1.SelectionStart = m.Index;
                richTextBox1.SelectionLength = m.Length;
                richTextBox1.SelectionFont = new Font("Courier New", 12, FontStyle.Regular);
                richTextBox1.SelectionColor = Color.Brown;
            }

            // restoring the original colors, for further writing
            richTextBox1.SelectionStart = originalIndex;
            richTextBox1.SelectionLength = originalLength;
            richTextBox1.SelectionFont = originalFont;
            richTextBox1.SelectionColor = originalColor;

            // giving back the focus
            richTextBox1.Focus();
        } 

        private void richTextBox1_Leave(object sender, EventArgs e)
        {
            string richText = richTextBox1.Text;
            _optionsPage.TestCaseTemplate = richText;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            applyTextHighlighting();
        }

        private void richTextBox1_Load(object sender, EventArgs e)
        {
            applyTextHighlighting();
        }
    }
}
