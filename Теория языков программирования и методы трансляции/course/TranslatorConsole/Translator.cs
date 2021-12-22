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
        private readonly Dictionary<string, Identificator> _idenTable;
        private readonly Node _root;
        private List<string> _localIden = new();
        private int _tabs = 0;
        private string program = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ti">Таблица идентификаторов.</param>
        /// <param name="n">Корень дерева разбора.</param>
        public Translator(Node n, Dictionary<string, Identificator> ti) {
            _root = n;
            _idenTable = ti;
        }

        /// <summary>
        /// Перевод с c++ на golang.
        /// </summary>
        /// <returns>Программа на golang.</returns>
        public string Translate() {
            _tabs = 0;
            program = "package main\n";

            WriteBranch(_root);
            
            return program;
        }
        /// <summary>
        /// Итеративно разбирает каждый узел на код программы.
        /// </summary>
        /// <param name="node"></param>
        private void WriteBranch(Node node) {
            switch (node.Name) {
                case "FUNCTION":
                    break;
                default:
                    foreach (var n in node.Branches) {
                        WriteBranch(n);
                    }                    
                    break;
            }
        }

        private void ParseFunc(Node node) {
            var funcName = node.Branches[1].Name;
            var returnDataType = node.Branches[0].Name;
            Write("func " + funcName + "() ");
            WriteDataType(returnDataType);
            Write("\n");
            _tabs++;

        }

        /// <summary>
        /// Запишет тип данных.
        /// </summary>
        /// <param name="dataType">Тип данных из c++.</param>
        private void WriteDataType(string dataType) { }

        /// <summary>
        /// Записывает строку в текст программы.
        /// </summary>
        /// <param name="text">Строка для записи.</param>
        /// <param name="isLineBreak">Можно ли для данной строки делать отступ.</param>
        private void Write(string text, bool isLineBreak = true) {
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
