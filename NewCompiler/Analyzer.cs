using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace NewCompiler
{
    internal class Analyzer
    {
        private readonly Dictionary<string, int> KeyWords = new Dictionary<string, int>
        {
            { "final", 1 },
            { "String", 2 }
        };

        public DataTable ResultTable { get; private set; }
        public List<string> Errors { get; private set; }

        public Analyzer()
        {
            InitializeTable();
        }

        private void InitializeTable()
        {
            ResultTable = new DataTable();
            ResultTable.Columns.Add("Код", typeof(int));
            ResultTable.Columns.Add("Тип", typeof(string));
            ResultTable.Columns.Add("Значение", typeof(string));
            ResultTable.Columns.Add("Позиция", typeof(string));
            Errors = new List<string>();
        }

        public void Analyze(string inputText, DataGridView outputGrid)
        {
            // Очищаем предыдущие результаты
            ResultTable.Clear();
            Errors.Clear();

            if (string.IsNullOrWhiteSpace(inputText))
            {
                Errors.Add("Ошибка: Входной текст пуст");
                UpdateOutput(outputGrid);
                return;
            }

            int lineNumber = 1;
            int position = 0;
            int currentLineStartPos = 0;
            string currentLexeme = "";
            int state = 0; // Начальное состояние

            while (position < inputText.Length)
            {
                char currentChar = inputText[position];

                switch (state)
                {
                    case 0: // Начальное состояние
                        if (char.IsLetter(currentChar))
                        {
                            state = 1; // Идентификатор или ключевое слово
                            currentLexeme += currentChar;
                        }
                        else if (char.IsWhiteSpace(currentChar))
                        {
                            if (currentChar == '\n')
                            {
                                lineNumber++;
                                currentLineStartPos = position + 1;
                            }
                            AddLexeme(4, "разделитель", "пробел", lineNumber, position - currentLineStartPos);
                        }
                        else if (currentChar == '=')
                        {
                            AddLexeme(5, "оператор присваивания", "=", lineNumber, position - currentLineStartPos);
                        }
                        else if (currentChar == '"')
                        {
                            state = 2; // Начало строки
                            currentLexeme = "\"";
                        }
                        else if (currentChar == ';')
                        {
                            AddLexeme(7, "конец оператора", ";", lineNumber, position - currentLineStartPos);
                        }
                        else if (!char.IsControl(currentChar))
                        {
                            AddLexeme(8, "ERROR", currentChar.ToString(), lineNumber, position - currentLineStartPos);
                            Errors.Add($"Ошибка: Недопустимый символ '{currentChar}' в строке {lineNumber}, позиция {position - currentLineStartPos + 1}");
                        }
                        position++;
                        break;

                    case 1: // Идентификатор или ключевое слово
                        if (char.IsLetterOrDigit(currentChar) || currentChar == '_')
                        {
                            currentLexeme += currentChar;
                            position++;
                        }
                        else
                        {
                            if (KeyWords.ContainsKey(currentLexeme))
                            {
                                AddLexeme(KeyWords[currentLexeme], "ключевое слово", currentLexeme, lineNumber, position - currentLexeme.Length - currentLineStartPos);
                            }
                            else
                            {
                                AddLexeme(3, "идентификатор", currentLexeme, lineNumber, position - currentLexeme.Length - currentLineStartPos);
                            }
                            currentLexeme = "";
                            state = 0;
                        }
                        break;

                    case 2: // Строка
                        currentLexeme += currentChar;
                        position++;

                        if (currentChar == '"')
                        {
                            AddLexeme(6, "строка", currentLexeme, lineNumber, position - currentLexeme.Length - currentLineStartPos);
                            currentLexeme = "";
                            state = 0;
                        }
                        else if (currentChar == '\n')
                        {
                            Errors.Add($"Ошибка: Незавершенная строка в строке {lineNumber}");
                            lineNumber++;
                            currentLineStartPos = position;
                            state = 0;
                        }
                        break;
                }
            }

            // Обработка незавершенных лексем
            if (state == 1) // Незавершенный идентификатор
            {
                if (KeyWords.ContainsKey(currentLexeme))
                {
                    AddLexeme(KeyWords[currentLexeme], "ключевое слово", currentLexeme, lineNumber, position - currentLexeme.Length - currentLineStartPos);
                }
                else
                {
                    AddLexeme(3, "идентификатор", currentLexeme, lineNumber, position - currentLexeme.Length - currentLineStartPos);
                }
            }
            else if (state == 2) // Незавершенная строка
            {
                Errors.Add($"Ошибка: Незавершенная строка в строке {lineNumber}");
                AddLexeme(6, "строка (незавершенная)", currentLexeme, lineNumber, position - currentLexeme.Length - currentLineStartPos);
            }

            UpdateOutput(outputGrid);
        }

        private void AddLexeme(int code, string type, string value, int line, int position)
        {
            ResultTable.Rows.Add(
                code,
                type,
                value,
                $"Строка {line}, позиция {position + 1}-{position + value.Length}"
            );
        }

        private void UpdateOutput(DataGridView outputGrid)
        {
            outputGrid.DataSource = ResultTable;

            // Настройка внешнего вида таблицы
            outputGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            outputGrid.DefaultCellStyle.Font = new System.Drawing.Font("Consolas", 10);

            // Вывод ошибок
            if (Errors.Count > 0)
            {
                MessageBox.Show(string.Join("\n", Errors), "Ошибки анализа",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}