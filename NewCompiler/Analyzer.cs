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
            int state = 0;

            while (position < inputText.Length)
            {
                char currentChar = inputText[position];

                // Обработка переноса строки
                if (currentChar == '\n')
                {
                    lineNumber++;
                    currentLineStartPos = position + 1;
                    position++;
                    continue;
                }

                // Пропускаем служебные символы (кроме пробела)
                if (char.IsControl(currentChar) && currentChar != ' ' && currentChar != '\t')
                {
                    position++;
                    continue;
                }

                string lexemePosition = $"Строка {lineNumber}, позиция {position - currentLineStartPos + 1}";

                switch (state)
                {
                    case 0:
                        if (char.IsLetter(currentChar))
                        {
                            if ((currentChar >= 'А' && currentChar <= 'я') || currentChar == 'Ё' || currentChar == 'ё')
                            {
                                AddLexeme(8, "ERROR", currentChar.ToString(), lineNumber, position - currentLineStartPos, 1);
                                Errors.Add($"Ошибка: Недопустимый символ (русская буква) '{currentChar}' в строке {lineNumber}, позиция {position - currentLineStartPos + 1}");
                            }
                            else
                            {
                                state = 1;
                                currentLexeme += currentChar;
                            }
                            position++;
                        }
                        else if (char.IsWhiteSpace(currentChar))
                        {
                            // Добавляем только обычные пробелы и табы
                            if (currentChar == ' ' || currentChar == '\t')
                            {
                                AddLexeme(4, "разделитель", currentChar == ' ' ? "пробел" : "табуляция", lineNumber, position - currentLineStartPos, 1);
                            }
                            position++;
                        }
                        else if (currentChar == '=')
                        {
                            AddLexeme(5, "оператор присваивания", "=", lineNumber, position - currentLineStartPos, 1);
                            position++;
                        }
                        else if (currentChar == '"')
                        {
                            state = 2;
                            currentLexeme = "\"";
                            position++;
                        }
                        else if (currentChar == ';')
                        {
                            AddLexeme(7, "конец оператора", ";", lineNumber, position - currentLineStartPos, 1);
                            position++;
                        }
                        else
                        {
                            AddLexeme(8, "ERROR", currentChar.ToString(), lineNumber, position - currentLineStartPos, 1);
                            Errors.Add($"Ошибка: Недопустимый символ '{currentChar}' в строке {lineNumber}, позиция {position - currentLineStartPos + 1}");
                            position++;
                        }
                        break;

                    case 1:
                        if (char.IsLetterOrDigit(currentChar) || currentChar == '_')
                        {
                            if ((currentChar >= 'А' && currentChar <= 'я') || currentChar == 'Ё' || currentChar == 'ё')
                            {
                                AddLexeme(KeyWords.ContainsKey(currentLexeme) ? KeyWords[currentLexeme] : 3,
                                          KeyWords.ContainsKey(currentLexeme) ? "ключевое слово" : "идентификатор",
                                          currentLexeme,
                                          lineNumber,
                                          position - currentLexeme.Length - currentLineStartPos,
                                          currentLexeme.Length);
                                Errors.Add($"Ошибка: Недопустимый символ (русская буква) '{currentChar}' после идентификатора в строке {lineNumber}, позиция {position - currentLineStartPos + 1}");
                                AddLexeme(8, "ERROR", currentChar.ToString(), lineNumber, position - currentLineStartPos, 1);
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
                                      position - currentLexeme.Length - currentLineStartPos,
                                      currentLexeme.Length);
                            currentLexeme = "";
                            state = 0;
                        }
                        break;

                    case 2:
                        currentLexeme += currentChar;
                        position++;

                        if (currentChar == '"')
                        {
                            AddLexeme(6, "строка", currentLexeme, lineNumber, position - currentLexeme.Length - currentLineStartPos, currentLexeme.Length);
                            currentLexeme = "";
                            state = 0;
                        }
                        else if (currentChar == '\n')
                        {
                            Errors.Add($"Ошибка: Незавершенная строка в строке {lineNumber}");
                        }
                        break;
                }
            }

            if (state == 1)
            {
                AddLexeme(KeyWords.ContainsKey(currentLexeme) ? KeyWords[currentLexeme] : 3,
                          KeyWords.ContainsKey(currentLexeme) ? "ключевое слово" : "идентификатор",
                          currentLexeme,
                          lineNumber,
                          position - currentLexeme.Length - currentLineStartPos,
                          currentLexeme.Length);
            }
            else if (state == 2)
            {
                Errors.Add($"Ошибка: Незавершенная строка в строке {lineNumber}");
                AddLexeme(6, "строка (незавершенная)", currentLexeme, lineNumber, position - currentLexeme.Length - currentLineStartPos, currentLexeme.Length);
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
            outputGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            outputGrid.DefaultCellStyle.Font = new System.Drawing.Font("Consolas", 10);

            if (Errors.Count > 0)
            {
                MessageBox.Show(Errors[0], "Ошибка анализа",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}