
using System.Collections.Generic;
using System.Text.RegularExpressions;

using TranslatorConsole.Identificators;

namespace TranslatorConsole
{
    /// <summary>
    /// Лексический анализатор. Строит таблицу лексем по входному тексту программы.
    /// </summary>
    public class LexicalAnalyzer
    {
        /// <summary>
        /// Таблица лексем.
        /// </summary>
        private List<Token> TokenTable { get; set; } = new();
        /// <summary>
        /// Таблица идентификаторов.
        /// </summary>
        private Dictionary<string, Identificator> IdentificatorTable { get; set; } = new();
        /// <summary>
        /// Таблица ключевых слов.
        /// </summary>
        private List<string> KeyWordTable { get; set; } = new();
        public LexicalAnalyzer() {
            // типы
            KeyWordTable.Add("int");
            KeyWordTable.Add("bool");
            KeyWordTable.Add("double");
            KeyWordTable.Add("float");
            KeyWordTable.Add("long");
            KeyWordTable.Add("short");
            KeyWordTable.Add("void");
            // циклы
            KeyWordTable.Add("break");
            KeyWordTable.Add("else");
            KeyWordTable.Add("if");
            KeyWordTable.Add("while");
        }
        /// <summary>
        /// Разбирает текст программы на лексемы.
        /// </summary>
        /// <param name="progamText"></param>
        public void Parse(string programText) {
            // любые слова; слова - идентификаторы или ключевые
            var regexWord = new Regex(@"([a-zA-Z]|_)([a-zA-Z]|_|[0-9])*");
            // строки
            var regexString = new Regex("\".*\"");
            // разделители
            var regexDelimiter = new Regex(@"\(|\)|{|}|;|<|>");
            // пустые символы
            var regexWhiteSpaces = new Regex(@" |\t|\r");
            // перевод на новую строку
            var regexNewLine = new Regex(@"\r\n|\n");
            // целые числа
            var regexDigit = new Regex(@"[0-9]+");
            // с плавающей точкой
            var regexFloat = new Regex(@"(0|[0-9]+)\.[0-9]+");

            // начало слова
            var regexWordBegin = new Regex(@"[a-zA-Z_]");
            // начало строки в плюсах 
            var regexStringBegin = new Regex("[\"]");
            // начало числа
            var regexDigitBegin = new Regex(@"[0-9]|\.");

            string fileName = "";

            int pos = 0;
            int line = 1;
            int linePos = 1;
            while (pos < programText.Length) {
                string buf = "";
                string nextChar = programText.Substring(pos, 1);
                Token? t = null;
                // ключевые слова или идентификаторы
                if (regexWordBegin.IsMatch(nextChar))
                {
                    while (pos < programText.Length)
                    {
                        if (!regexWordBegin.IsMatch(programText.Substring(pos, 1))) break;

                        buf += programText[pos];

                        pos++;
                        linePos++;
                    }
                    if (!regexWord.IsMatch(buf))
                    {
                        throw new Exception(line + ":" + linePos + ": Wrong syntax of word: " + buf);
                    }

                    t = new Token(pos, line, fileName, buf);

                    if (KeyWordTable.Contains(buf))
                    {
                        AddKeyWordType(t);
                    }
                    else
                    {
                        t.Type = TokenType.IDENTIFICATOR;
                        t.TypeStr = "IDEN";
                        // если предыдущий токен - тип данных, то делаем его не транслируемым
                        // т.к. эта информация будет храниться в списке идентификаторов
                        var token = TokenTable[^1];
                        if (token.Type == TokenType.DATA_TYPE) token.IsTranslatable = false;
                        // добавляем в таблицу идентификаторов, если еще нет
                        if (!IdentificatorTable.ContainsKey(buf)) {
                            IdentificatorTable.Add(buf,
                            new VariableIdentificator()
                            {
                                Name = buf,
                                Type = token.Value
                            });
                        }                        
                    }
                }
                else if (regexStringBegin.IsMatch(nextChar))
                {
                    // считываем символы пока не встретим конец строки
                    buf += "\"";
                    pos++;
                    linePos++;

                    while (pos < programText.Length)
                    {
                        if (programText.Substring(pos, 1) == "\"")
                        {
                            buf += programText[pos];
                            pos++;
                            linePos++;
                            break;
                        }

                        buf += programText[pos];

                        pos++;
                        linePos++;
                    }
                    if (!regexString.IsMatch(buf))
                    {
                        throw new Exception(line + ":" + linePos + ": Wrong syntax of string: " + buf);
                    }

                    t = new Token(pos, line, fileName, buf)
                    {
                        Type = TokenType.STRING_CONST,
                        TypeStr = "CONST_STRING"
                    };
                }
                else if (regexNewLine.IsMatch(nextChar))
                {
                    line++;
                    pos++;
                    linePos = 0;
                }
                else if (regexDigitBegin.IsMatch(nextChar))
                {
                    // число
                    while (pos < programText.Length)
                    {
                        if (!regexDigitBegin.IsMatch(programText.Substring(pos, 1))) break;

                        buf += programText[pos];

                        pos++;
                        linePos++;
                    }

                    t = new Token(pos, line, fileName, buf);
                    if (regexDigit.IsMatch(buf)) t.Type = TokenType.INTEGER_CONST;
                    else if (regexFloat.IsMatch(buf)) t.Type = TokenType.FLOAT_CONST;
                    else throw new Exception(line + ":" + linePos + ": Wrong syntax of number: " + buf);
                    t.TypeStr = "CONST_DIGIT";
                }
                else if (regexWhiteSpaces.IsMatch(nextChar))
                {
                    pos++;
                    linePos++;
                }
                else if (nextChar == "=") {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        Type = TokenType.SIGN_EQUAL,
                        TypeStr = nextChar
                    };

                    pos++;
                    linePos++;
                }
                else if (nextChar == ">") {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        Type = TokenType.SIGN_GREATER,
                        TypeStr = nextChar
                    };

                    pos++;
                    linePos++;
                }
                else if (nextChar == "<") {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        Type = TokenType.SIGN_LESS,
                        TypeStr = nextChar

                    };
                    pos++;
                    linePos++;
                }
                else if (nextChar == "+") {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        Type = TokenType.SIGN_PLUS,
                        TypeStr = nextChar
                    };
                    pos++;
                    linePos++;
                }
                else if (nextChar == "-") {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        Type = TokenType.SIGN_MINUS,
                        TypeStr = nextChar
                    };
                    pos++;
                    linePos++;
                }
                else if (nextChar == "/") {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        Type = TokenType.SIGN_DIVIDE,
                        TypeStr = nextChar
                    };
                    pos++;
                    linePos++;
                }
                else if (nextChar == "*") {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        Type = TokenType.SIGN_MULTIPLY,
                        TypeStr = nextChar
                    };
                    pos++;
                    linePos++;
                }
                else if (nextChar == "(")
                {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        Type = TokenType.DELIMITER_LBRACKET,
                        TypeStr = nextChar
                    };
                    // если предыдущая лексема - идентификатор, меняем ей тип на фукнцию
                    if (TokenTable[^1].Type == TokenType.IDENTIFICATOR) {
                        var token = TokenTable[^1];
                        var iden = IdentificatorTable[token.Value];
                        if (iden is VariableIdentificator vi) {
                            var func = new FunctionIdentificator() {
                                Name = vi.Name,
                                ReturnType = vi.Type
                            };

                            IdentificatorTable.Remove(token.Value);
                            IdentificatorTable[token.Value] = func;
                        }
                    }
                    pos++;
                    linePos++;
                }
                else if (nextChar == ")")
                {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        Type = TokenType.DELIMITER_RBRACKET,
                        TypeStr = nextChar
                    };
                    pos++;
                    linePos++;
                }
                else if (nextChar == "{")
                {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        Type = TokenType.DELIMITER_CURLY_LBRACKET,
                        TypeStr = nextChar
                    };
                    pos++;
                    linePos++;
                }
                else if (nextChar == "}")
                {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        Type = TokenType.DELIMITER_CURLY_RBRACKET,
                        TypeStr = nextChar
                    };
                    pos++;
                    linePos++;
                }
                else if (nextChar == ";")
                {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        Type = TokenType.DELIMITER_SEMICOLON,
                        TypeStr = nextChar
                    };
                    pos++;
                    linePos++;
                }
                else
                {
                    // Неизвестный символ
                    pos++;
                    linePos++;
                    Console.WriteLine(line + ":" + linePos + " unrecognized symbol: " + nextChar);
                }

                if (t is not null) TokenTable.Add(t);
            }
        }

        /// <summary>
        /// Добавит необходимы тип к ключевому слову.
        /// </summary>
        /// <param name="keyWord"></param>
        /// <exception cref="Exception"></exception>
        private void AddKeyWordType(Token keyWord) {
            switch (keyWord.Value) {
                case "int":
                    keyWord.Type = TokenType.DATA_TYPE;
                    keyWord.TypeStr = "DATA_TYPE";
                    break;
                case "long":
                    keyWord.Type = TokenType.DATA_TYPE;
                    keyWord.TypeStr = "DATA_TYPE";
                    break;
                case "float":
                    keyWord.Type = TokenType.DATA_TYPE;
                    keyWord.TypeStr = "DATA_TYPE";
                    break;
                case "double":
                    keyWord.Type = TokenType.DATA_TYPE;
                    keyWord.TypeStr = "DATA_TYPE";
                    break;
                case "void":
                    keyWord.Type = TokenType.VOID;
                    keyWord.TypeStr = "DATA_TYPE";
                    break;
                case "break":
                    keyWord.Type = TokenType.BREAK;
                    keyWord.TypeStr = "BREAK";
                    break;
                case "else":
                    keyWord.Type = TokenType.ELSE;
                    keyWord.TypeStr = "ELSE";
                    break;
                case "if":
                    keyWord.Type = TokenType.IF;
                    keyWord.TypeStr = "IF";
                    break;
                case "while":
                    keyWord.Type = TokenType.WHILE;
                    keyWord.TypeStr = "WHILE";
                    break;
                default: throw new Exception("Unrecognized keyword.");
            }
        }

        public List<Token> GetTokenTable() {
            return TokenTable;
        }
        public Dictionary<string, Identificator> GetIdentificators() { 
            return IdentificatorTable;
        }
        public string GetTokenTableString() {
            string res = "";
            foreach (Token t in TokenTable)
            {
                res += t.LinePos + ":" + t.Type.ToString() + " " + t.Value + "\n";
            }
            return res;
        }
        public string GetIdenTableString() {
            string res = "";
            foreach (var i in IdentificatorTable) {
                res += i.Key + " " + i.Value + "\n";
            }
            return res;
        }

    }

    public enum TokenType {
        INCLUDE,
        INCLUDE_NAME,
        FUNCTION,
        IDENTIFICATOR,
        KEYWORD,
        DATA_TYPE,                  // тип данных
        DATA_TYPE_INT,
        DATA_TYPE_FLOAT,
        DATA_TYPE_DOUBLE,
        VOID,
        DATA_TYPE_LONG,
        BREAK,
        IF,
        WHILE,
        ELSE,
        STRING_CONST,               // строковая константа в кавычках
        FLOAT_CONST,                // любая числовая константа с плавающей точкой
        INTEGER_CONST,              // любая целая константа
        UNDEFINED,                  // неопределенный тип
        SIGN_GREATER,               // знак больше
        SIGN_LESS,                  // меньше
        SIGN_EQUAL,                 // равно
        SIGN_MULTIPLY,              // умножить
        SIGN_DIVIDE,                // разделить
        SIGN_PLUS,                  // плюс
        SIGN_MINUS,                 // минус
        DELIMITER_LBRACKET,         // левая круглая скобка
        DELIMITER_RBRACKET,         // правая круглая
        DELIMITER_COMMA,            // запятая
        DELIMITER_SEMICOLON,        // точка с заяпятой
        DELIMITER_CURLY_LBRACKET,   // фигурная левая скобка
        DELIMITER_CURLY_RBRACKET    // фигурная правая

    }

    /// <summary>
    /// Класс лексемы (токена). Описывает единичную структуру языка.
    /// </summary>
    public class Token {
        /// <summary>
        /// Номер строки, на которой находится токен.
        /// </summary>
        public int LinePos { get; set; }
        /// <summary>
        /// Пощиция токена в файле.
        /// </summary>
        public int Pos { get; set; }
        /// <summary>
        /// Название файла, в котором находится токен.
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Тип токена.
        /// </summary>
        public TokenType Type { get; set; }
        public string TypeStr { get; set; }

        /// <summary>
        /// Значение токена.
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Учавствует ли токен в переводе на другой язык.
        /// </summary>
        public bool IsTranslatable { get; set; } = true;

        private readonly Dictionary<string, Identificator> _identificatorTable;
        public Token() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p">Позиция в файле.</param>
        /// <param name="linep">Номер строки.</param>
        /// <param name="file">Название файла.</param>
        /// <param name="v">Значение.</param>
        public Token(int p, int linep, string file, string v, TokenType t = TokenType.UNDEFINED) {
            Pos = p;
            FileName = file;
            LinePos = linep;
            Value = v;
            Type = t;
        }
        public Token(int p, int linep, string file, string v, 
            Dictionary<string, Identificator> t) {
            Pos = p;
            FileName = file;
            LinePos = linep;
            Value = v;
            _identificatorTable = t;
        }
    }



}
