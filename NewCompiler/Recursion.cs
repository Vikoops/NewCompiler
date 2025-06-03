using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NewCompiler
{
    public class UnsignedNumberParser
    {
        private string input;
        private int position;
        private List<string> outputLines;
        private TextBox errorBox;
        private int indentLevel;

        public void Analyze(string inputText, DataGridView outputGrid, TextBox errorTextBox)
        {
            input = inputText.Trim();
            position = 0;
            outputLines = new List<string>();
            errorBox = errorTextBox;
            indentLevel = 0;

            outputGrid.Rows.Clear();
            errorBox.Clear();

            try
            {
                ParseNumber();

                if (position < input.Length)
                {
                    throw new Exception($"Unexpected character '{input[position]}' at position {position + 1}");
                }

                errorBox.AppendText("[SUCCESS] Parsing completed\r\n");
            }
            catch (Exception ex)
            {
                errorBox.AppendText($"[ERROR] {ex.Message}\r\n");
            }

            outputGrid.Rows.Clear();
            outputGrid.Columns.Clear();
            outputGrid.Columns.Add("Step", "Parsing Steps");

            foreach (var line in outputLines)
            {
                outputGrid.Rows.Add(line);
            }
        }

        private void ParseNumber()
        {
            // Ищем '10+' или '10-' в любом месте числа
            int expPos = input.IndexOf("10+", position);
            if (expPos == -1) expPos = input.IndexOf("10-", position);

            if (expPos != -1)
            {
                // Разбираем часть до экспоненты как decimal number
                ParseDecimalPart(expPos);

                // Разбираем экспоненциальную часть
                ParseExponent();
            }
            else
            {
                // Если нет экспоненты - просто decimal number
                if (!ParseDecimalNumber())
                {
                    throw new Exception("Invalid number format");
                }
            }
        }

        private void ParseDecimalPart(int expPos)
        {
            string decimalPart = input.Substring(position, expPos - position);
            position = expPos;

            // Разбираем целую часть
            if (decimalPart.Contains("."))
            {
                string[] parts = decimalPart.Split('.');
                if (!string.IsNullOrEmpty(parts[0]))
                {
                    AddOutput("unsigned_integer:");
                    indentLevel++;
                    foreach (char c in parts[0])
                    {
                        AddOutput($"digit: '{c}'");
                    }
                    indentLevel--;
                }

                if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1]))
                {
                    AddOutput("decimal_fraction:");
                    indentLevel++;
                    AddOutput("terminal: '.'");
                    foreach (char c in parts[1])
                    {
                        AddOutput($"digit: '{c}'");
                    }
                    indentLevel--;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(decimalPart))
                {
                    AddOutput("unsigned_integer:");
                    indentLevel++;
                    foreach (char c in decimalPart)
                    {
                        AddOutput($"digit: '{c}'");
                    }
                    indentLevel--;
                }
            }
        }

        private void ParseExponent()
        {
            AddOutput("exponential_part:");
            indentLevel++;

            // '10'
            AddOutput("terminal: '10'");
            position += 2;

            // Знак
            char sign = input[position];
            if (sign != '+' && sign != '-')
            {
                throw new Exception("Exponent must have '+' or '-' sign");
            }
            AddOutput($"terminal: '{sign}'");
            position++;

            // Цифры
            if (position >= input.Length || !char.IsDigit(input[position]))
            {
                throw new Exception("Exponent must have digits");
            }

            AddOutput("integer:");
            indentLevel++;
            while (position < input.Length && char.IsDigit(input[position]))
            {
                AddOutput($"digit: '{input[position]}'");
                position++;
            }
            indentLevel--;

            indentLevel--;
        }

        private bool ParseDecimalNumber()
        {
            // Простая реализация без учета экспоненты
            bool hasDigits = false;

            // Целая часть
            while (position < input.Length && char.IsDigit(input[position]))
            {
                if (!hasDigits)
                {
                    AddOutput("unsigned_integer:");
                    indentLevel++;
                    hasDigits = true;
                }
                AddOutput($"digit: '{input[position]}'");
                position++;
            }
            if (hasDigits) indentLevel--;

            // Дробная часть
            if (position < input.Length && input[position] == '.')
            {
                AddOutput("decimal_fraction:");
                indentLevel++;
                AddOutput("terminal: '.'");
                position++;

                bool hasFractionDigits = false;
                while (position < input.Length && char.IsDigit(input[position]))
                {
                    hasFractionDigits = true;
                    AddOutput($"digit: '{input[position]}'");
                    position++;
                }

                if (!hasFractionDigits)
                {
                    throw new Exception("Expected unsigned_integer");
                }
                indentLevel--;
            }

            return true;
        }

        private void AddOutput(string line)
        {
            outputLines.Add(new string(' ', indentLevel * 2) + line);
        }
    }
}