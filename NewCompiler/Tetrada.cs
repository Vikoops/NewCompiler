using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NewCompiler
{
    public class Tetrada
    {
        // Лексический анализатор
        public class Lexer
        {
            public enum TokenType { Plus, Assign, Minus, Mul, Div, LParen, RParen, Identifier, End }

            public class Token
            {
                public TokenType Type { get; }
                public string Lexeme { get; }
                public int Position { get; }

                public Token(TokenType type, string lexeme, int pos)
                {
                    Type = type;
                    Lexeme = lexeme;
                    Position = pos;
                }
            }

            public class LexicalError
            {
                public string Message { get; }
                public int Position { get; }
                public int Length { get; }

                public LexicalError(string msg, int pos, int len)
                {
                    Message = msg;
                    Position = pos;
                    Length = len;
                }
            }

            private readonly string _input;
            private int _idx;

            public List<Token> Tokens { get; } = new List<Token>();
            public List<LexicalError> Errors { get; } = new List<LexicalError>();

            public Lexer(string input)
            {
                _input = input;
                _idx = 0;
            }

            public void Tokenize()
            {
                while (_idx < _input.Length)
                {
                    char c = _input[_idx];
                    if (char.IsWhiteSpace(c))
                    {
                        _idx++;
                        continue;
                    }

                    if (char.IsLetter(c))
                    {
                        int start = _idx;
                        while (_idx < _input.Length && char.IsLetter(_input[_idx]))
                            _idx++;
                        string lex = _input.Substring(start, _idx - start);
                        Tokens.Add(new Token(TokenType.Identifier, lex, start));
                        continue;
                    }

                    switch (c)
                    {
                        case '+': Tokens.Add(new Token(TokenType.Plus, "+", _idx)); break;
                        case '-': Tokens.Add(new Token(TokenType.Minus, "-", _idx)); break;
                        case '*': Tokens.Add(new Token(TokenType.Mul, "*", _idx)); break;
                        case '/': Tokens.Add(new Token(TokenType.Div, "/", _idx)); break;
                        case '(': Tokens.Add(new Token(TokenType.LParen, "(", _idx)); break;
                        case ')': Tokens.Add(new Token(TokenType.RParen, ")", _idx)); break;
                        case '=': Tokens.Add(new Token(TokenType.Assign, "=", _idx)); break;
                        default:
                            Errors.Add(new LexicalError($"Недопустимый символ '{c}'", _idx, 1));
                            break;
                    }
                    _idx++;
                }

                Tokens.Add(new Token(TokenType.End, string.Empty, _idx));
            }
        }

        // Синтаксический анализатор и генератор тетрад
        public class Parser
        {
            public class Quad
            {
                public string Op { get; }
                public string Arg1 { get; }
                public string Arg2 { get; }
                public string Result { get; }

                public Quad(string op, string a1, string a2, string res)
                {
                    Op = op;
                    Arg1 = a1;
                    Arg2 = a2;
                    Result = res;
                }
            }

            public class SyntaxError
            {
                public string Message { get; }
                public int Position { get; }
                public int Length { get; }

                public SyntaxError(string msg, int pos, int len)
                {
                    Message = msg;
                    Position = pos;
                    Length = len;
                }
            }

            private readonly List<Lexer.Token> _tokens;
            private int _pos;
            private int _tempCount;

            public List<Quad> Quads { get; } = new List<Quad>();
            public List<SyntaxError> Errors { get; } = new List<SyntaxError>();

            private Lexer.Token Current => _tokens[_pos];
            private Lexer.Token Peek(int offset) => (_pos + offset) < _tokens.Count ? _tokens[_pos + offset] : new Lexer.Token(Lexer.TokenType.End, string.Empty, _pos + offset);

            public Parser(List<Lexer.Token> tokens)
            {
                _tokens = tokens;
                _pos = 0;
                _tempCount = 0;
            }

            private void Advance()
            {
                if (_pos < _tokens.Count - 1)
                    _pos++;
            }

            private bool Match(Lexer.TokenType expected)
            {
                if (Current.Type == expected)
                {
                    Advance();
                    return true;
                }
                Errors.Add(new SyntaxError(
                    $"Ожидался «{expected}», а встретилось «{Current.Lexeme}»",
                    Current.Position,
                    Current.Lexeme.Length));
                return false;
            }

            private string NewTemp() => "t" + (++_tempCount);

            public void Parse()
            {
                ParseStatement();
                if (Current.Type != Lexer.TokenType.End)
                {
                    Errors.Add(new SyntaxError(
                        $"Лишний токен «{Current.Lexeme}» после конца оператора",
                        Current.Position,
                        Current.Lexeme.Length));
                }
            }

            private void ParseStatement()
            {
                // Assignment: id = expr
                if (Current.Type == Lexer.TokenType.Identifier && Peek(1).Type == Lexer.TokenType.Assign)
                {
                    var id = Current.Lexeme;
                    Advance(); // id
                    Advance(); // =
                    var exprRes = ParseE();
                    if (string.IsNullOrEmpty(exprRes))
                    {
                        Errors.Add(new SyntaxError(
                            "Ожидалось выражение после «=»",
                            Current.Position,
                            Current.Lexeme.Length));
                    }
                    else
                    {
                        Quads.Add(new Quad("=", exprRes, string.Empty, id));
                    }
                }
                // Unexpected closing parenthesis at top-level
                else if (Current.Type == Lexer.TokenType.RParen)
                {
                    Errors.Add(new SyntaxError(
                        "Неправильная закрывающая скобка без соответствующей открывающей",
                        Current.Position,
                        Current.Lexeme.Length));
                    Advance();
                }
                else
                {
                    // Regular expression
                    _ = ParseE();
                }
            }

            private string ParseE()
            {
                if (Current.Type == Lexer.TokenType.RParen)
                {
                    Errors.Add(new SyntaxError(
                        "Неправильная закрывающая скобка без соответствующей открывающей",
                        Current.Position,
                        Current.Lexeme.Length));
                    Advance();
                    return string.Empty;
                }

                if (Current.Type == Lexer.TokenType.Plus || Current.Type == Lexer.TokenType.Minus ||
                    Current.Type == Lexer.TokenType.Mul || Current.Type == Lexer.TokenType.Div)
                {
                    // Operator without left operand
                    Errors.Add(new SyntaxError(
                        $"Ожидался идентификатор или «(» перед «{Current.Lexeme}»",
                        Current.Position,
                        Current.Lexeme.Length));
                    Advance();
                }

                var t = ParseT();
                return ParseA(t);
            }

            private string ParseA(string inh)
            {
                while (Current.Type == Lexer.TokenType.Plus || Current.Type == Lexer.TokenType.Minus)
                {
                    var op = Current.Lexeme;
                    Advance();
                    var t2 = ParseT();
                    var res = NewTemp();
                    Quads.Add(new Quad(op, inh, t2, res));
                    inh = res;
                }
                return inh;
            }

            private string ParseT()
            {
                var o = ParseO();
                return ParseB(o);
            }

            private string ParseB(string inh)
            {
                while (Current.Type == Lexer.TokenType.Mul || Current.Type == Lexer.TokenType.Div)
                {
                    var op = Current.Lexeme;
                    Advance();
                    var o2 = ParseO();
                    var res = NewTemp();
                    Quads.Add(new Quad(op, inh, o2, res));
                    inh = res;
                }
                return inh;
            }

            private string ParseO()
            {
                if (Current.Type == Lexer.TokenType.Identifier)
                {
                    var id = Current.Lexeme;
                    Advance();
                    return id;
                }
                if (Current.Type == Lexer.TokenType.LParen)
                {
                    var pos = Current.Position;
                    Advance();
                    var e = ParseE();
                    if (!Match(Lexer.TokenType.RParen))
                    {
                        Errors.Add(new SyntaxError(
                            "Отсутствует закрывающая скобка «)»",
                            pos,
                            1));
                    }
                    return e;
                }
                if (Current.Type == Lexer.TokenType.RParen)
                {
                    Errors.Add(new SyntaxError(
                        "Неправильная закрывающая скобка без соответствующей открывающей",
                        Current.Position,
                        Current.Lexeme.Length));
                    Advance();
                    return string.Empty;
                }

                // Unexpected token
                Errors.Add(new SyntaxError(
                    $"Непредвиденный токен «{Current.Lexeme}», ожидался идентификатор или «(»",
                    Current.Position,
                    Current.Lexeme.Length));
                Advance();
                return string.Empty;
            }
        }

        // Интерфейс для интеграции с вашей программой
        public static void AnalyzeExpression(string input, RichTextBox richTextBox, DataGridView dataGridView, TextBox errorTextBox = null)
        {
            // Сброс подсветки
            richTextBox.SelectAll();
            richTextBox.SelectionBackColor = Color.White;

            // Очистка предыдущих ошибок
            if (errorTextBox != null)
            {
                errorTextBox.Clear();
                errorTextBox.Visible = false;
            }

            var lexer = new Lexer(input);
            lexer.Tokenize();

            bool hasErrors = false;

            // Обработка лексических ошибок
            if (lexer.Errors.Any())
            {
                hasErrors = true;
                foreach (var err in lexer.Errors)
                {
                    richTextBox.Select(err.Position, err.Length);
                    richTextBox.SelectionBackColor = Color.Yellow;

                    if (errorTextBox != null)
                    {
                        errorTextBox.AppendText($"Лексическая ошибка: {err.Message} (позиция {err.Position})\r\n");
                        errorTextBox.Visible = true;
                    }
                }
            }

            if (!hasErrors)
            {
                var parser = new Parser(lexer.Tokens);
                parser.Parse();

                // Обработка синтаксических ошибок
                if (parser.Errors.Any())
                {
                    hasErrors = true;
                    foreach (var err in parser.Errors)
                    {
                        richTextBox.Select(err.Position, err.Length);
                        richTextBox.SelectionBackColor = Color.LightPink;

                        if (errorTextBox != null)
                        {
                            errorTextBox.AppendText($"Синтаксическая ошибка: {err.Message} (позиция {err.Position})\r\n");
                            errorTextBox.Visible = true;
                        }
                    }
                }

                // Если ошибок нет - выводим тетрады
                if (!hasErrors)
                {
                    dataGridView.Columns.Clear();
                    dataGridView.Columns.Add("op", "Операция");
                    dataGridView.Columns.Add("arg1", "Аргумент 1");
                    dataGridView.Columns.Add("arg2", "Аргумент 2");
                    dataGridView.Columns.Add("result", "Результат");
                    dataGridView.Rows.Clear();

                    foreach (var q in parser.Quads)
                    {
                        dataGridView.Rows.Add(q.Op, q.Arg1, q.Arg2, q.Result);
                    }
                }
            }

            if (hasErrors)
            {
                dataGridView.Rows.Clear();
                if (errorTextBox != null)
                {
                    errorTextBox.Focus();
                }
            }
        }
    }
}