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
                string lexemePosition = $"Строка {lineNumber}, позиция {position + 1}-{position + 1}"; // Строка и позиция для символа

                switch (state)
                {
                    case 0: // Начальное состояние
                        if (char.IsLetter(currentChar))
                        {
                            // Проверка на русские буквы
                            if ((currentChar >= 'А' && currentChar <= 'я') || currentChar == 'Ё' || currentChar == 'ё')
                            {
                                AddLexeme(8, "ERROR", currentChar.ToString(), lineNumber, position, 1);
                                Errors.Add($"Ошибка: Недопустимый символ (русская буква) '{currentChar}' в строке {lineNumber}, позиция {position + 1}");
                            }
                            else
                            {
                                state = 1; // Идентификатор или ключевое слово
                                currentLexeme += currentChar;
                            }
                            position++;
                        }
                        else if (char.IsWhiteSpace(currentChar))
                        {
                            // Если пробел
                            AddLexeme(4, "разделитель", "пробел", lineNumber, position, 1);
                            position++;
                        }
                        else if (currentChar == '=')
                        {
                            AddLexeme(5, "оператор присваивания", "=", lineNumber, position, 1);
                            position++;
                        }
                        else if (currentChar == '"')
                        {
                            state = 2; // Начало строки
                            currentLexeme = "\"";
                            position++;
                        }
                        else if (currentChar == ';')
                        {
                            AddLexeme(7, "конец оператора", ";", lineNumber, position, 1);
                            position++;
                        }
                        else if (!char.IsControl(currentChar))
                        {
                            AddLexeme(8, "ERROR", currentChar.ToString(), lineNumber, position, 1);
                            Errors.Add($"Ошибка: Недопустимый символ '{currentChar}' в строке {lineNumber}, позиция {position + 1}");
                            position++;
                        }
                        else
                        {
                            position++;
                        }
                        break;

                    case 1: // Идентификатор или ключевое слово
                        if (char.IsLetterOrDigit(currentChar) || currentChar == '_')
                        {
                            // Проверка на недопустимый символ в середине идентификатора
                            if ((currentChar >= 'А' && currentChar <= 'я') || currentChar == 'Ё' || currentChar == 'ё')
                            {
                                AddLexeme(KeyWords.ContainsKey(currentLexeme) ? KeyWords[currentLexeme] : 3,
                                          KeyWords.ContainsKey(currentLexeme) ? "ключевое слово" : "идентификатор",
                                          currentLexeme,
                                          lineNumber,
                                          position - currentLexeme.Length,
                                          currentLexeme.Length);
                                Errors.Add($"Ошибка: Недопустимый символ (русская буква) '{currentChar}' после идентификатора в строке {lineNumber}, позиция {position + 1}");
                                AddLexeme(8, "ERROR", currentChar.ToString(), lineNumber, position, 1);
                                currentLexeme = "";
                                state = 0;
                            }
                            else
                            {
                                currentLexeme += currentChar;
                            }
                            position++;
                        }
                        else
                        {
                            AddLexeme(KeyWords.ContainsKey(currentLexeme) ? KeyWords[currentLexeme] : 3,
                                      KeyWords.ContainsKey(currentLexeme) ? "ключевое слово" : "идентификатор",
                                      currentLexeme,
                                      lineNumber,
                                      position - currentLexeme.Length,
                                      currentLexeme.Length);
                            currentLexeme = "";
                            state = 0; // Возврат в начальное состояние
                        }
                        break;

                    case 2: // Строка
                        currentLexeme += currentChar;
                        position++;

                        if (currentChar == '"')
                        {
                            AddLexeme(6, "строка", currentLexeme, lineNumber, position - currentLexeme.Length, currentLexeme.Length);
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
            if (state == 1)
            {
                AddLexeme(KeyWords.ContainsKey(currentLexeme) ? KeyWords[currentLexeme] : 3,
                          KeyWords.ContainsKey(currentLexeme) ? "ключевое слово" : "идентификатор",
                          currentLexeme,
                          lineNumber,
                          position - currentLexeme.Length,
                          currentLexeme.Length);
            }
            else if (state == 2)
            {
                Errors.Add($"Ошибка: Незавершенная строка в строке {lineNumber}");
                AddLexeme(6, "строка (незавершенная)", currentLexeme, lineNumber, position - currentLexeme.Length, currentLexeme.Length);
            }

            UpdateOutput(outputGrid);
        }

        private void AddLexeme(int code, string type, string value, int line, int startPosition, int length)
        {
            string lexemePosition = $"Строка {line}, позиция {startPosition + 1}-{startPosition + length}";
            ResultTable.Rows.Add(
                code,
                type,
                value,
                lexemePosition
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