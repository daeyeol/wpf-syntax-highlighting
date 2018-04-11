using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SyntaxHighlighting
{
    public class JsonSyntaxHighlightTextBox : RichTextBox
    {
        #region Dependency Property

        #region Json

        public static readonly DependencyProperty JsonProperty =
            DependencyProperty.Register("Json",
                typeof(string),
                typeof(JsonSyntaxHighlightTextBox),
                new PropertyMetadata(ChangedJsonProperty));

        public static void ChangedJsonProperty(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as JsonSyntaxHighlightTextBox).Update();
        }

        public string Json
        {
            get { return (string)GetValue(JsonProperty); }
            set { SetValue(JsonProperty, value); }
        }

        #endregion

        #region LineHeight

        public static readonly DependencyProperty LineHeightProperty =
            DependencyProperty.Register("LineHeight",
                typeof(double),
                typeof(JsonSyntaxHighlightTextBox),
                new PropertyMetadata(5d));

        public static void ChangedLineHeightProperty(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var textbox = obj as JsonSyntaxHighlightTextBox;
            textbox.Document.LineHeight = (double)e.NewValue;
        }

        public double LineHeight
        {
            get { return (double)GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        #endregion

        #region KeyColor

        public static readonly DependencyProperty KeyColorProperty =
            DependencyProperty.Register("KeyColor",
                typeof(SolidColorBrush),
                typeof(JsonSyntaxHighlightTextBox),
                new PropertyMetadata(ConvertStringToSolidBrushColor("#FF7CDCFE"),
                    new PropertyChangedCallback(ChangedKeyColorProperty)));

        public static void ChangedKeyColorProperty(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as JsonSyntaxHighlightTextBox).Update();
        }

        public SolidColorBrush KeyColor
        {
            get { return (SolidColorBrush)GetValue(KeyColorProperty); }
            set { SetValue(KeyColorProperty, value); }
        }

        #endregion

        #region StringColor

        public static readonly DependencyProperty StringColorProperty =
            DependencyProperty.Register("StringColor",
                typeof(SolidColorBrush),
                typeof(JsonSyntaxHighlightTextBox),
                new PropertyMetadata(ConvertStringToSolidBrushColor("#FFC3703C"),
                    new PropertyChangedCallback(ChangedStringColorProperty)));

        public static void ChangedStringColorProperty(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as JsonSyntaxHighlightTextBox).Update();

        }
        public SolidColorBrush StringColor
        {
            get { return (SolidColorBrush)GetValue(StringColorProperty); }
            set { SetValue(StringColorProperty, value); }
        }

        #endregion

        #region NumberColor

        public static readonly DependencyProperty NumberColorProperty =
            DependencyProperty.Register("NumberColor",
                typeof(SolidColorBrush),
                typeof(JsonSyntaxHighlightTextBox),
                new PropertyMetadata(ConvertStringToSolidBrushColor("#FFB5CEA8"),
                    new PropertyChangedCallback(ChangedNumberColorProperty)));

        public static void ChangedNumberColorProperty(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as JsonSyntaxHighlightTextBox).Update();
        }

        public SolidColorBrush NumberColor
        {
            get { return (SolidColorBrush)GetValue(NumberColorProperty); }
            set { SetValue(NumberColorProperty, value); }
        }

        #endregion

        #endregion

        #region Constructor

        public JsonSyntaxHighlightTextBox()
        {
            Foreground = Brushes.White;
            Background = ConvertStringToSolidBrushColor("#FF1E1E1E");
        }

        #endregion

        #region Private Method

        private void Update()
        {
            Document.Blocks.Clear();

            if (string.IsNullOrWhiteSpace(Json))
            {
                return;
            }

            FlowDocument flowDocument = new FlowDocument
            {
                PageWidth = ActualWidth,
                PageHeight = ActualHeight,
                LineHeight = LineHeight
            };

            Document = flowDocument;

            var lines = Json.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            var pattern = "(\"(\\\\u[a-zA-Z0-9]{4}|\\\\[^u]|[^\\\\\"])*\"(\\s*:)?|\\b(true|false|null)\\b|-?\\d+(?:\\.\\d*)?(?:[eE][+\\-]?\\d+)?)";
            var keyRanges = new List<TextRange>();
            var valueRanges = new List<TextRange>();

            foreach (var line in lines)
            {
                Paragraph paragraph = new Paragraph(new Run(line));
                flowDocument.Blocks.Add(paragraph);
            }

            var position = Document.ContentStart;

            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    var text = position.GetTextInRun(LogicalDirection.Forward);
                    var matches = Regex.Matches(text, pattern)
                        .Cast<Match>().ToList();

                    foreach (var match in matches)
                    {
                        if (string.IsNullOrWhiteSpace(match.Value))
                        {
                            continue;
                        }

                        if (match.Value.Last() == ':')
                        {
                            var range = CreteTextRange(position, match.Index, match.Length - 1);
                            keyRanges.Add(range);
                        }
                        else
                        {
                            var range = CreteTextRange(position, match.Index, match.Length);
                            valueRanges.Add(range);
                        }
                    }
                }

                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }

            foreach (var range in keyRanges)
            {
                range.ApplyPropertyValue(TextElement.ForegroundProperty, KeyColor);
            }

            foreach (var range in valueRanges)
            {
                if (Regex.IsMatch(range.Text, "[0-9]"))
                {
                    range.ApplyPropertyValue(TextElement.ForegroundProperty, NumberColor);
                }
                else
                {
                    range.ApplyPropertyValue(TextElement.ForegroundProperty, StringColor);
                }
            }
        }

        private TextRange CreteTextRange(TextPointer pointer, int index, int length)
        {
            var startPointer = pointer.GetPositionAtOffset(index);
            var endPointer = startPointer.GetPositionAtOffset(length);

            return new TextRange(startPointer, endPointer);
        }

        private static SolidColorBrush ConvertStringToSolidBrushColor(string colorString)
        {
            var color = (Color)ColorConverter.ConvertFromString(colorString);
            return new SolidColorBrush(color);
        }

        #endregion
    }
}