using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace NewCompiler
{
    internal class Analyzer
    {
        private readonly Dictionary<string, int> KeyWords = new Dictionary<string, int>
        {
            { "final", 3 },
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

                if (currentChar == '\n')
                {
                    lineNumber++;
                    currentLineStartPos = position + 1;
                    position++;
                    continue;
                }

                string lexemePosition = $"Строка {lineNumber}, позиция {position - currentLineStartPos + 1}";

                switch (state)
                {
                    case 0: // Начальное состояние
                        if (char.IsLetter(currentChar))
                        {
                            if (IsInvalidIdentifierChar(currentChar))
                            {
                                AddErrorLexeme(currentChar.ToString(), lineNumber, position - currentLineStartPos, 1, lexemePosition);
                                position++;
                            }
                            else
                            {
                                state = 1;
                                currentLexeme = currentChar.ToString();
                                position++;
                            }
                        }
                        else if (char.IsWhiteSpace(currentChar))
                        {
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
                            AddErrorLexeme(currentChar.ToString(), lineNumber, position - currentLineStartPos, 1, lexemePosition);
                            position++;
                        }
                        break;

                    case 1: // Собираем идентификатор/ключевое слово
                        if (IsInvalidIdentifierChar(currentChar))
                        {
                            // Завершаем текущую лексему (если она есть)
                            if (currentLexeme.Length > 0)
                            {
                                AddLexeme(KeyWords.ContainsKey(currentLexeme) ? KeyWords[currentLexeme] : 1,
                                          KeyWords.ContainsKey(currentLexeme) ? "ключевое слово" : "идентификатор",
                                          currentLexeme,
                                          lineNumber,
                                          position - currentLexeme.Length - currentLineStartPos,
                                          currentLexeme.Length);
                            }

                            // Добавляем ошибку для недопустимого символа
                            AddErrorLexeme(currentChar.ToString(), lineNumber, position - currentLineStartPos, 1, lexemePosition);

                            // Сбрасываем состояние и переходим к следующему символу
                            currentLexeme = "";
                            state = 0;
                            position++;
                        }
                        else if (char.IsLetterOrDigit(currentChar) || currentChar == '_')
                        {
                            currentLexeme += currentChar;
                            position++;
                        }
                        else
                        {
                            // Конец идентификатора
                            if (currentLexeme.Length > 0)
                            {
                                AddLexeme(KeyWords.ContainsKey(currentLexeme) ? KeyWords[currentLexeme] : 1,
                                          KeyWords.ContainsKey(currentLexeme) ? "ключевое слово" : "идентификатор",
                                          currentLexeme,
                                          lineNumber,
                                          position - currentLexeme.Length - currentLineStartPos,
                                          currentLexeme.Length);
                            }

                            currentLexeme = "";
                            state = 0;
                        }
                        break;

                    case 2: // Собираем строку
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

            // Обработка оставшихся данных после завершения цикла
            if (state == 1 && currentLexeme.Length > 0)
            {
                AddLexeme(KeyWords.ContainsKey(currentLexeme) ? KeyWords[currentLexeme] : 1,
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

            // Пост-обработка результатов - удаление идентификаторов/ключей после ERROR
            RemoveIdentifiersAfterErrors();

            // Обновляем DataGridView с очищенными результатами
            outputGrid.DataSource = ResultTable;
        }

        private void RemoveIdentifiersAfterErrors()
        {
            if (ResultTable.Rows.Count < 2) return;

            // Создаем список индексов строк для удаления
            List<int> rowsToRemove = new List<int>();

            for (int i = 0; i < ResultTable.Rows.Count - 1; i++)
            {
                var currentRow = ResultTable.Rows[i];
                var nextRow = ResultTable.Rows[i + 1];

                // Проверяем, является ли текущая строка ошибкой (ERROR)
                if (currentRow["Тип"].ToString() == "ERROR")
                {
                    // Проверяем, является ли следующая строка идентификатором или ключевым словом
                    string nextType = nextRow["Тип"].ToString();
                    if (nextType == "идентификатор" || nextType == "ключевое слово")
                    {
                        rowsToRemove.Add(i + 1); // Запоминаем индекс следующей строки
                    }
                }
            }

            // Удаляем строки в обратном порядке, чтобы индексы не сдвигались
            foreach (int index in rowsToRemove.OrderByDescending(x => x))
            {
                ResultTable.Rows.RemoveAt(index);
            }
        }

        private bool IsInvalidIdentifierChar(char c)
        {
            // Недопустимые символы в идентификаторе
            return (c >= 'А' && c <= 'я') || c == 'Ё' || c == 'ё' ||
                   c == '@' || c == '#' || c == '$' || c == '%' || c == '^' ||
                   c == '&' || c == '*' || c == '!' || c == '?' || c == '~' ||
                   c == '`' || c == '[' || c == ']' || c == '{' || c == '}' ||
                   c == '|' || c == '\\' || c == '/' || c == '<' || c == '>' ||
                   c == ';' || c == ':' || c == '"' || c == '\'' || c == ',';
        }

        private void AddLexeme(int code, string type, string value, int line, int startPosition, int length)
        {
            string lexemePosition = $"Строка {line}, позиция {startPosition + 1}-{startPosition + length}";
            ResultTable.Rows.Add(code, type, value, lexemePosition);
        }

        private void AddErrorLexeme(string value, int line, int startPosition, int length, string lexemePosition)
        {
            ResultTable.Rows.Add(8, "ERROR", value, lexemePosition);
            Errors.Add($"Ошибка: Недопустимый символ '{value}' в {lexemePosition}");
        }

        private void UpdateOutput(DataGridView outputGrid)
        {
            outputGrid.DataSource = ResultTable;
            outputGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            outputGrid.DefaultCellStyle.Font = new System.Drawing.Font("Consolas", 10);

            if (Errors.Count > 0)
            {
                string allErrors = string.Join(Environment.NewLine, Errors);
                MessageBox.Show(allErrors, "Ошибки анализа",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}