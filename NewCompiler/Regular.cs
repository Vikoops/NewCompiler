using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace NewCompiler
{
    internal class Regular
    {
      
        private static readonly Regex PunctuationRx = new Regex(@"[^\w\s-]|_");
        private static readonly Regex AmexCardRx = new Regex(@"\b3[47]\d{2}[-\s]?\d{6}[-\s]?\d{5}\b");
        private static readonly Regex RgbColorRx = new Regex(@"(?:^|\s)rgb\(\s*(25[0-5]|2[0-4]\d|1\d{2}|\d{1,2})\s*,\s*(25[0-5]|2[0-4]\d|1\d{2}|\d{1,2})\s*,\s*(25[0-5]|2[0-4]\d|1\d{2}|\d{1,2})\s*\)",
    RegexOptions.IgnoreCase);

        /*public static void Analyze(string inputText, RichTextBox richTextBox, DataGridView dataGridView)
        {
            SetupGrid(dataGridView);

            Search(PunctuationRx, "Знаки препинания", null, richTextBox, dataGridView);

            Search(AmexCardRx, "Amex Card", ValidateAmexCard, richTextBox, dataGridView);

            Search(RgbColorRx, "RGB цвет", ValidateRgbColor, richTextBox, dataGridView);
        }*/

        public static void Analyze(string inputText, RichTextBox richTextBox, DataGridView dataGridView)
        {
            SetupGrid(dataGridView);

            var rgbMatches = RgbAutomaton.FindRgbColors(inputText);
            foreach (var (value, position) in rgbMatches)
            {
                dataGridView.Rows.Add("RGB цвет", value, position, "✔");
                richTextBox.Select(position, value.Length);
                richTextBox.SelectionBackColor = Color.Yellow;
            }

            Search(PunctuationRx, "Знаки препинания", null, richTextBox, dataGridView);
            Search(AmexCardRx, "Amex Card", ValidateAmexCard, richTextBox, dataGridView);
        }

        private static void SetupGrid(DataGridView dataGridView)
        {
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();

            dataGridView.Columns.Add("Type", "Тип");
            dataGridView.Columns.Add("Value", "Значение");
            dataGridView.Columns.Add("Position", "Позиция");
            dataGridView.Columns.Add("Valid", "Валидность");
        }

        private static void Search(Regex rx, string type, Func<string, bool> validator, RichTextBox richTextBox, DataGridView dataGridView)
        {
            foreach (Match m in rx.Matches(richTextBox.Text))
            {
                bool ok = validator?.Invoke(m.Value) ?? true;
                dataGridView.Rows.Add(type, m.Value, m.Index + 1, ok ? "✔" : "✖");

                if (ok)
                {
                    richTextBox.Select(m.Index, m.Length);
                    richTextBox.SelectionBackColor = Color.Yellow;
                }
            }
        }

        
        private static bool ValidateAmexCard(string s)
        {
            string cleaned = s.Replace(" ", "").Replace("-", "");
            return cleaned.Length == 15;
        }

        public class RgbAutomaton
        {
            private enum State
            {
                Start,
                R,
                G,
                B,
                ExpectRgb,
                ExpectOpenParen,
                ExpectNumber,
                ExpectCommaOrClose,
                Success,
                Fail
            }

            public static List<(string value, int position)> FindRgbColors(string text)
            {
                var results = new List<(string, int)>();
                State currentState = State.Start;
                StringBuilder currentMatch = new StringBuilder();
                int startPosition = 0;
                int currentNumber = 0;
                int numbersFound = 0;

                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];

                    switch (currentState)
                    {
                        case State.Start:
                            if (char.ToLower(c) == 'r')
                            {
                                currentState = State.R;
                                currentMatch.Append(c);
                                startPosition = i;
                            }
                            break;

                        case State.R:
                            if (char.ToLower(c) == 'g')
                            {
                                currentState = State.G;
                                currentMatch.Append(c);
                            }
                            else
                            {
                                Reset();
                            }
                            break;

                        case State.G:
                            if (char.ToLower(c) == 'b')
                            {
                                currentState = State.B;
                                currentMatch.Append(c);
                            }
                            else
                            {
                                Reset();
                            }
                            break;

                        case State.B:
                            if (c == '(')
                            {
                                currentState = State.ExpectNumber;
                                currentMatch.Append(c);
                            }
                            else
                            {
                                Reset();
                            }
                            break;

                        case State.ExpectNumber:
                            if (char.IsDigit(c))
                            {
                                currentNumber = c - '0';
                                currentState = State.ExpectCommaOrClose;
                                currentMatch.Append(c);
                            }
                            else if (!char.IsWhiteSpace(c))
                            {
                                Reset();
                            }
                            break;

                        case State.ExpectCommaOrClose:
                            if (char.IsDigit(c))
                            {
                                currentNumber = currentNumber * 10 + (c - '0');
                                if (currentNumber > 255)
                                {
                                    Reset();
                                }
                                else
                                {
                                    currentMatch.Append(c);
                                }
                            }
                            else if (c == ',')
                            {
                                numbersFound++;
                                if (numbersFound >= 3)
                                {
                                    Reset();
                                }
                                else
                                {
                                    currentState = State.ExpectNumber;
                                    currentMatch.Append(c);
                                }
                            }
                            else if (c == ')')
                            {
                                numbersFound++;
                                if (numbersFound == 3)
                                {
                                    currentMatch.Append(c);
                                    results.Add((currentMatch.ToString(), startPosition + 1));
                                }
                                Reset();
                            }
                            else if (!char.IsWhiteSpace(c))
                            {
                                Reset();
                            }
                            break;

                        default:
                            Reset();
                            break;
                    }
                }

                void Reset()
                {
                    currentState = State.Start;
                    currentMatch.Clear();
                    currentNumber = 0;
                    numbersFound = 0;
                }

                return results;
            }
        }

    }
}