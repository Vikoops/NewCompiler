using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace NewCompiler
{
    public class Parser
    {
        public DataTable ResultTable { get; private set; }
        public List<string> Errors { get; private set; }

        // Коды лексем
        private const int IDENTIFIER_CODE = 1;
        private const int STRING_KEYWORD_CODE = 2;
        private const int FINAL_KEYWORD_CODE = 3;
        private const int SPACE_DELIMITER_CODE = 4;
        private const int ASSIGN_OPERATOR_CODE = 5;
        private const int STRING_LITERAL_CODE = 6;
        private const int SEMICOLON_CODE = 7;

        public Parser()
        {
            InitializeTable();
        }

        private void InitializeTable()
        {
            ResultTable = new DataTable();
            ResultTable.Columns.Add("Код лексемы", typeof(int));
            ResultTable.Columns.Add("Тип", typeof(string));
            ResultTable.Columns.Add("Значение", typeof(string));
            ResultTable.Columns.Add("Начало лексемы", typeof(int));
            ResultTable.Columns.Add("Конец лексемы", typeof(int));
            Errors = new List<string>();
        }

        public void Parse(string inputText)
        {
            ResultTable.Clear();
            Errors.Clear();

            if (string.IsNullOrWhiteSpace(inputText))
            {
                Errors.Add("Ошибка: Входной текст пуст");
                return;
            }

            int pos = 0;
            string currentLexeme = "";
            bool hasError = false;

            const int EXPECT_FINAL = 0;
            const int EXPECT_SPACE_AFTER_FINAL = 1;
            const int EXPECT_STRING = 2;
            const int EXPECT_SPACE_AFTER_STRING = 3;
            const int EXPECT_ID = 4;
            const int EXPECT_EQUALS = 5;
            const int EXPECT_QUOTE = 6;
            const int EXPECT_STRING_CONTENT = 7;
            const int EXPECT_SEMICOLON = 8;
            const int COMPLETED = -1;

            int state = EXPECT_FINAL;

            while (pos < inputText.Length && !hasError)
            {
                char currentChar = inputText[pos];
                int startPos = pos + 1; // Начало позиции (счёт с 1)

                switch (state)
                {
                    case EXPECT_FINAL:
                        if (char.IsLetter(currentChar))
                        {
                            currentLexeme = ReadWord(inputText, ref pos);
                            if (ContainsRussianLetters(currentLexeme))
                            {
                                Errors.Add($"Ошибка: Идентификатор содержит недопустимый символ '{currentLexeme}' на позиции {startPos}");
                                hasError = true;
                                break;
                            }

                            if (currentLexeme == "final")
                            {
                                AddResult(FINAL_KEYWORD_CODE, "ключевое слово", currentLexeme, startPos, pos);
                                state = EXPECT_SPACE_AFTER_FINAL;
                            }
                            else
                            {
                                AddResult(IDENTIFIER_CODE, "идентификатор", currentLexeme, startPos, pos);
                                Errors.Add("Ошибка: ожидалось ключевое слово 'final'");
                                hasError = true;
                            }
                        }
                        else
                        {
                            Errors.Add("Ошибка: ожидалось ключевое слово 'final'");
                            hasError = true;
                        }
                        break;

                    case EXPECT_SPACE_AFTER_FINAL:
                        if (currentChar == ' ')
                        {
                            AddResult(SPACE_DELIMITER_CODE, "разделитель", "пробел", startPos, startPos);
                            pos++;
                            state = EXPECT_STRING;
                        }
                        else
                        {
                            Errors.Add("Ошибка: ожидался пробел после 'final'");
                            hasError = true;
                        }
                        break;

                    case EXPECT_STRING:
                        if (char.IsLetter(currentChar))
                        {
                            currentLexeme = ReadWord(inputText, ref pos);
                            if (ContainsRussianLetters(currentLexeme))
                            {
                                Errors.Add($"Ошибка: Идентификатор содержит недопустимый символ '{currentLexeme}' на позиции {startPos}");
                                hasError = true;
                                break;
                            }

                            if (currentLexeme == "String")
                            {
                                AddResult(STRING_KEYWORD_CODE, "ключевое слово", currentLexeme, startPos, pos);
                                state = EXPECT_SPACE_AFTER_STRING;
                            }
                            else
                            {
                                AddResult(IDENTIFIER_CODE, "идентификатор", currentLexeme, startPos, pos);
                                Errors.Add("Ошибка: ожидалось ключевое слово 'String'");
                                hasError = true;
                            }
                        }
                        else
                        {
                            Errors.Add("Ошибка: ожидалось ключевое слово 'String'");
                            hasError = true;
                        }
                        break;

                    case EXPECT_SPACE_AFTER_STRING:
                        if (currentChar == ' ')
                        {
                            AddResult(SPACE_DELIMITER_CODE, "разделитель", "пробел", startPos, startPos);
                            pos++;
                            state = EXPECT_ID;
                        }
                        else
                        {
                            Errors.Add("Ошибка: ожидался пробел после 'String'");
                            hasError = true;
                        }
                        break;

                    case EXPECT_ID:
                        if (char.IsLetter(currentChar))
                        {
                            currentLexeme = ReadWord(inputText, ref pos);
                            if (ContainsRussianLetters(currentLexeme))
                            {
                                Errors.Add($"Ошибка: Идентификатор содержит недопустимый символ '{currentLexeme}' на позиции {startPos}");
                                hasError = true;
                                break;
                            }
                            AddResult(IDENTIFIER_CODE, "идентификатор", currentLexeme, startPos, pos);
                            state = EXPECT_EQUALS;
                        }
                        else
                        {
                            Errors.Add("Ошибка: ожидался идентификатор");
                            hasError = true;
                        }
                        break;

                    case EXPECT_EQUALS:
                        while (pos < inputText.Length && char.IsWhiteSpace(inputText[pos])) pos++;

                        if (pos < inputText.Length && inputText[pos] == '=')
                        {
                            AddResult(ASSIGN_OPERATOR_CODE, "оператор", "=", pos + 1, pos + 1);
                            pos++;
                            state = EXPECT_QUOTE;
                        }
                        else
                        {
                            Errors.Add("Ошибка: ожидался оператор '='");
                            hasError = true;
                        }
                        break;

                    case EXPECT_QUOTE:
                        while (pos < inputText.Length && char.IsWhiteSpace(inputText[pos])) pos++;

                        if (pos < inputText.Length && inputText[pos] == '"')
                        {
                            int stringStart = pos + 1;
                            currentLexeme = "\"";
                            pos++;
                            while (pos < inputText.Length && inputText[pos] != '"')
                            {
                                currentLexeme += inputText[pos];
                                pos++;
                            }

                            if (pos < inputText.Length && inputText[pos] == '"')
                            {
                                currentLexeme += "\"";
                                AddResult(STRING_LITERAL_CODE, "строка", currentLexeme, stringStart, pos + 1);
                                pos++;
                                state = EXPECT_SEMICOLON;
                            }
                            else
                            {
                                Errors.Add("Ошибка: незавершенная строка");
                                hasError = true;
                            }
                        }
                        else
                        {
                            Errors.Add("Ошибка: ожидалась открывающая кавычка '\"'");
                            hasError = true;
                        }
                        break;

                    case EXPECT_SEMICOLON:
                        while (pos < inputText.Length && char.IsWhiteSpace(inputText[pos])) pos++;

                        if (pos < inputText.Length && inputText[pos] == ';')
                        {
                            AddResult(SEMICOLON_CODE, "конец оператора", ";", pos + 1, pos + 1);
                            pos++;
                            state = COMPLETED;
                        }
                        else
                        {
                            Errors.Add("Ошибка: ожидался символ ';' после строки");
                            hasError = true;
                        }
                        break;
                }
            }

            if (!hasError && state != COMPLETED)
            {
                Errors.Add("Ошибка: неожиданный конец входных данных");
            }
        }

        private string ReadWord(string inputText, ref int pos)
        {
            string word = "";
            while (pos < inputText.Length && (char.IsLetterOrDigit(inputText[pos]) || inputText[pos] == '_'))
            {
                word += inputText[pos];
                pos++;
            }
            return word;
        }

        private bool ContainsRussianLetters(string text)
        {
            foreach (char c in text)
            {
                if (c >= '\u0400' && c <= '\u04FF') return true;
            }
            return false;
        }

        private void AddResult(int code, string type, string value, int start, int end)
        {
            ResultTable.Rows.Add(code, type, value, start, end);
        }

        public void ShowResults(DataGridView grid)
        {
            grid.DataSource = ResultTable;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            grid.ReadOnly = true;

            if (Errors.Count > 0)
            {
                MessageBox.Show(string.Join("\n", Errors), "Ошибки парсера",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
