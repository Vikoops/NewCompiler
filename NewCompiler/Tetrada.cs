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
            public enum TokenType { Plus, Minus, Mul, Div, LParen, RParen, Identifier, End }

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

            public Parser(List<Lexer.Token> tokens)
            {
                _tokens = tokens;
                _pos = 0;
                _tempCount = 0;
            }

            private void Advance() => _pos = Math.Min(_pos + 1, _tokens.Count - 1);
            private bool Match(Lexer.TokenType expected)
            {
                if (Current.Type == expected)
                {
                    Advance();
                    return true;
                }
                Errors.Add(new SyntaxError(
                    $"Ожидалось {expected}, а встретилось '{Current.Lexeme}'",
                    Current.Position,
                    Current.Lexeme.Length));
                Advance();
                return false;
            }

            private string NewTemp() => "t" + (++_tempCount);

            public void Parse()
            {
                string result = ParseE();
                if (Current.Type != Lexer.TokenType.End)
                {
                    Errors.Add(new SyntaxError(
                        $"Лишний токен '{Current.Lexeme}'",
                        Current.Position,
                        Current.Lexeme.Length));
                }
            }

            private string ParseE()
            {
                var t = ParseT();
                return ParseA(t);
            }

            private string ParseA(string inh)
            {
                if (Current.Type == Lexer.TokenType.Plus)
                {
                    Advance();
                    var t2 = ParseT();
                    var res = NewTemp();
                    Quads.Add(new Quad("+", inh, t2, res));
                    return ParseA(res);
                }
                if (Current.Type == Lexer.TokenType.Minus)
                {
                    Advance();
                    var t2 = ParseT();
                    var res = NewTemp();
                    Quads.Add(new Quad("-", inh, t2, res));
                    return ParseA(res);
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
                if (Current.Type == Lexer.TokenType.Mul)
                {
                    Advance();
                    var o2 = ParseO();
                    var res = NewTemp();
                    Quads.Add(new Quad("*", inh, o2, res));
                    return ParseB(res);
                }
                if (Current.Type == Lexer.TokenType.Div)
                {
                    Advance();
                    var o2 = ParseO();
                    var res = NewTemp();
                    Quads.Add(new Quad("/", inh, o2, res));
                    return ParseB(res);
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
                    Advance();
                    var e = ParseE();
                    Match(Lexer.TokenType.RParen);
                    return e;
                }
                Errors.Add(new SyntaxError(
                    $"Непредвиденный токен '{Current.Lexeme}'",
                    Current.Position,
                    Current.Lexeme.Length));
                Advance();
                return string.Empty;
            }
        }

        // Интерфейс для интеграции с вашей программой
        public static void AnalyzeExpression(string input, RichTextBox richTextBox, DataGridView dataGridView)
        {
            // Сброс подсветки
            richTextBox.SelectAll();
            richTextBox.SelectionBackColor = Color.White;

            var lexer = new Lexer(input);
            lexer.Tokenize();

            // Подсветка лексических ошибок
            foreach (var err in lexer.Errors)
            {
                richTextBox.Select(err.Position, err.Length);
                richTextBox.SelectionBackColor = Color.Yellow;
            }
            if (lexer.Errors.Any()) return;

            var parser = new Parser(lexer.Tokens);
            parser.Parse();

            // Подсветка синтаксических ошибок
            foreach (var err in parser.Errors)
            {
                richTextBox.Select(err.Position, err.Length);
                richTextBox.SelectionBackColor = Color.LightPink;
            }
            if (parser.Errors.Any()) return;

            // Вывод тетрад
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
}