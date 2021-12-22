using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TranslatorConsole.Identificators;


namespace TranslatorConsole
{
    /// <summary>
    /// Класс для трансляции с c++ на golang.
    /// </summary>
    public class Translator
    {
        private readonly List<Token> _tokenTable;
        private readonly Dictionary<string, Identificator> _idenTable;
        private List<string> _localIden = new();
        private int _tabs = 0;
        private string program = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ti">Таблица идентификаторов.</param>
        /// <param name="table">Таблица лексем.</param>
        public Translator(List<Token> table, Dictionary<string, Identificator> ti) {
            _tokenTable = table;
            _idenTable = ti;
        }

        /// <summary>
        /// Перевод с c++ на golang.
        /// </summary>
        /// <returns>Программа на golang.</returns>
        public string Translate() {
            _tabs = 0;
            program = "package main\n";
            foreach (var t in _tokenTable)
            {
                if (!t.IsTranslatable) continue;
                if (t.Type == TokenType.VOID) Write("func ");
                else if (t.Type == TokenType.IDENTIFICATOR) 
                    WriteIdentificator(t.Value);
                else if (t.Type == TokenType.DELIMITER_LBRACKET) Write(t.Value + " ");
                else if (t.Type == TokenType.DELIMITER_RBRACKET) Write(t.Value + " ");
                else if (t.Type == TokenType.DELIMITER_CURLY_LBRACKET)
                {
                    Write(t.Value + "\n");
                    _tabs++;
                }
                else if (t.Type == TokenType.DELIMITER_CURLY_RBRACKET)
                {
                    _tabs--;
                    Write(t.Value + "\n");
                }
                else if (t.Type == TokenType.DELIMITER_SEMICOLON) Write("\n");
            }
            return program;
        }
        /// <summary>
        /// Запишет идентификатор в текст программы.
        /// </summary>
        /// <param name="idenName">Название идентификатора.</param>
        private void WriteIdentificator(string idenName) {
            var id = _idenTable[idenName];
            if (id is VariableIdentificator variable) {
                if (_localIden.Contains(idenName)) {
                    Write(variable.Name, true);
                }
                else {
                    _localIden.Add(idenName);
                    string v = variable.Type;
                    if (v == "int") v = "int32";
                    else if (v == "long") v = "int64";
                    else if (v == "float") v = "float32";
                    else if (v == "double") v = "float64";

                    Write("var " + variable.Name + " " + v, true);
                }
            }
            else if (id is FunctionIdentificator func) {
                
                if (_localIden.Contains(idenName))
                {
                    Write(func.Name, true);
                }
                else
                {
                    string args = ")";
                    foreach (var arg in func.Arguments)
                    {

                    }
                    args += ")";
                    _localIden.Add(idenName);
                    Write("func " + func.Name, true);
                }
            }            
        }
        /// <summary>
        /// Записывает строку в текст программы.
        /// </summary>
        /// <param name="text">Строка для записи.</param>
        /// <param name="isLineBreak">Можно ли для данной строки делать отступ.</param>
        private void Write(string text, bool isLineBreak = false) {
            if (isLineBreak)
            {
                for (int i = 0; i < _tabs; i++)
                {
                    program += "\t";
                }
            }
            program += text;
        }
    }
}
