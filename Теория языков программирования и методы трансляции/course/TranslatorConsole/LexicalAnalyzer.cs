
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
            // 
            KeyWordTable.Add("break");
            KeyWordTable.Add("else");
            KeyWordTable.Add("if");
            KeyWordTable.Add("while");
            KeyWordTable.Add("for");
            KeyWordTable.Add("return");
            //
            KeyWordTable.Add("std");
            KeyWordTable.Add("cout");
            KeyWordTable.Add("<<");
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
            string? error = null;
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
                        t.TypeStr = "IDEN";
                        var token = TokenTable[^1];

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
                    if (regexDigit.IsMatch(buf)) { }
                    else if (regexFloat.IsMatch(buf)) { }
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
                        TypeStr = nextChar
                    };

                    pos++;
                    linePos++;
                }
                else if (nextChar == ">") {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        TypeStr = nextChar
                    };

                    pos++;
                    linePos++;
                }
                else if (nextChar == "<") {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        TypeStr = nextChar

                    };
                    pos++;
                    linePos++;
                }
                else if (nextChar == "+") {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        TypeStr = nextChar
                    };
                    pos++;
                    linePos++;
                }
                else if (nextChar == "-") {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        TypeStr = nextChar
                    };
                    pos++;
                    linePos++;
                }
                else if (nextChar == "/") {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        TypeStr = nextChar
                    };
                    pos++;
                    linePos++;
                }
                else if (nextChar == "*") {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        TypeStr = nextChar
                    };
                    pos++;
                    linePos++;
                }
                else if (nextChar == "(")
                {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        TypeStr = nextChar
                    };
                    // если предыдущая лексема - идентификатор, меняем ей тип на фукнцию
                    if (TokenTable[^1].TypeStr == "IDEN") {
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
                        TypeStr = nextChar
                    };
                    pos++;
                    linePos++;
                }
                else if (nextChar == "{")
                {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        TypeStr = nextChar
                    };
                    pos++;
                    linePos++;
                }
                else if (nextChar == "}")
                {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        TypeStr = nextChar
                    };
                    pos++;
                    linePos++;
                }
                else if (nextChar == ";")
                {
                    t = new Token(pos, line, fileName, nextChar)
                    {
                        TypeStr = nextChar
                    };
                    pos++;
                    linePos++;
                }
                else if (nextChar == ":")
                {
                    t = new Token(pos, line, fileName, nextChar)
                    {
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
                    
                    string s = line + ":" + linePos + ": unrecognized symbol: " + nextChar; ;
                    if (string.IsNullOrEmpty(error)) error = s;
                    else error += "\n" + s;
                }

                if (t is not null) TokenTable.Add(t);
            }

            if (!string.IsNullOrEmpty(error)) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
                Console.ResetColor();
                throw new Exception(error);
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
                    keyWord.TypeStr = "DATA_TYPE";
                    break;
                case "long":
                    keyWord.TypeStr = "DATA_TYPE";
                    break;
                case "float":
                    keyWord.TypeStr = "DATA_TYPE";
                    break;
                case "double":
                    keyWord.TypeStr = "DATA_TYPE";
                    break;
                case "void":
                    keyWord.TypeStr = "DATA_TYPE";
                    break;
                case "break":
                    keyWord.TypeStr = "BREAK";
                    break;
                case "else":
                    keyWord.TypeStr = "ELSE";
                    break;
                case "if":
                    keyWord.TypeStr = "IF";
                    break;
                case "while":
                    keyWord.TypeStr = "WHILE";
                    break;
                case "for":
                    keyWord.TypeStr = "FOR";
                    break;
                case "return":
                    keyWord.TypeStr = "RETURN";
                    break;
                case "std":
                    keyWord.TypeStr = "STD";
                    break;
                case "cout":
                    keyWord.TypeStr = "COUT";
                    break;
                case "<<":
                    keyWord.TypeStr = "<<";
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
                res += t.LinePos + ":" + t.TypeStr + " " + t.Value + "\n";
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
        public Token(int p, int linep, string file, string v) {
            Pos = p;
            FileName = file;
            LinePos = linep;
            Value = v;
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
