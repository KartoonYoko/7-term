using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TranslatorConsole.Identificators;

namespace TranslatorConsole
{
    /// <summary>
    /// Синтаксический анализатор c++.
    /// </summary>
    public class SyntaxAnalyzer
    {
        private readonly List<Token> _tokenTable;
        private readonly Dictionary<string, Identificator> _idenTable;
        private readonly List<Rule> _rules;
        private readonly Buffer _buffer;
        
        public SyntaxAnalyzer(List<Token> tokens, Dictionary<string, Identificator> idens) {
            _tokenTable = tokens;
            _idenTable = idens;
            _rules = new();

            // программа
            _rules.Add(new Rule("programm", new()
            {
                "SEVERAL_FUNC"
            }));            
            _rules.Add(new("SEVERAL_FUNC", new()
            {
                "SEVERAL_FUNC FUNCTION",
                "FUNCTION"
            }));
            // функция
            _rules.Add(new("FUNCTION", new() {
                "DATA_TYPE IDEN ( ) { SEVERAL_STATE }"
            }));
            // несколько операторов
            _rules.Add(new("SEVERAL_STATE", new()
            {
                "SEVERAL_STATE STATEMENT",
                "STATEMENT"
            }));
            // оператор
            _rules.Add(new("STATEMENT", new()
            {
                "ANNOUNCE",
                "ASSIGN"
            }));

            // объявление переменной
            _rules.Add(new Rule("ANNOUNCE", new()
            {
                "DATA_TYPE IDEN ;",
                "DATA_TYPE IDEN ;"
            }));
            // инициализация перременной
            _rules.Add(new Rule("ASSIGN", new()
            {
                "IDEN = CONST_STRING ;",
                "IDEN = CONST_DIGIT ;"
            }));

            _buffer = new(_rules);
        }

        /// <summary>
        /// Проанализирует таблицу лексем и составит дерево разбора.
        /// </summary>
        public Node Analyze() {
            foreach (var token in _tokenTable) { 
                _buffer.Add(token);
            }

            return _buffer.GetTree();
        }


        /// <summary>
        /// Буффер, для хранения токенов.
        /// 
        /// Одновременно с добавлением новых токенов в буффер создает дерево разбора для транслятора.
        /// </summary>
        private class Buffer {
            /// <summary>
            /// Хранит список токенов в строковом варианте для разбора правил.
            /// </summary>
            private string StringBuffer = "";
            /// <summary>
            /// Хранит список токено для составления дерева разбора.
            /// </summary>
            private readonly List<Node> NodeBuffer = new();
            /// <summary>
            /// Правила разбора.
            /// </summary>
            private readonly List<Rule> Rules;

            public Buffer(List<Rule> r) { 
                Rules = r;
            }

            public Node GetTree() {
                if (NodeBuffer.Count != 1) throw new Exception("Analyze tree error");

                return NodeBuffer[0];
            }

            /// <summary>
            /// Добавление токена в буффер.
            /// </summary>
            /// <param name="t">Добавление токена в буфер.</param>
            public void Add(Token t) {
                // добавляем новый токен в строку, с помощью которой 
                // будем искать правила и производить свертку
                if (string.IsNullOrEmpty(StringBuffer)) StringBuffer = t.TypeStr;
                else StringBuffer += " " + t.TypeStr;

                // добавляем в список соответствующий узел
                Node n = new(t.TypeStr, t);
                NodeBuffer.Add(n);

                // производим свёртку, если подходит под правила
                CheckRules();
            }
            /// <summary>
            /// Сворачивает StringBuffer и NodeBuffer,
            /// если подходит под какое-либо правило.
            /// </summary>
            private void CheckRules() {
                bool isEnd;
                do
                {
                    isEnd = true;
                    foreach (var rule in Rules)
                    {
                        var l = rule.LeftPart;
                        foreach (var r in rule.RightPart)
                        {
                            var pos = StringBuffer.IndexOf(r);
                            if (pos != -1)
                            {
                                // не оканчиваем разбор на следующей итерации, т.к встретилось правило
                                // и может встретиться еще после замены буфферов
                                isEnd = false;
                                // выясним с какой позиции нужные узлы в буффере
                                int count = -1;
                                for (int i = 0; i < pos; i++)
                                {
                                    if (StringBuffer[i] == ' ') count++;
                                }
                                // кол-во узлов для свёртки
                                var countNodes = r.Split(' ').Length;
                                var idLastNode = count + countNodes;
                                Node n = new(l);
                                for (int i = count + 1; i <= idLastNode; i++)
                                {
                                    n.AddBranch(NodeBuffer[i]);
                                }
                                // удаляем узлы из буффера
                                for (int i = idLastNode; i > count; i--)
                                {
                                    NodeBuffer.RemoveAt(i);
                                }
                                // новый узел вместо старых
                                NodeBuffer.Insert(count + 1, n);

                                // та же замена уже в строковом буффере
                                StringBuffer = StringBuffer.Remove(pos, r.Length);
                                StringBuffer = StringBuffer.Insert(pos, l);

                            }
                        }
                    }
                } while (!isEnd);

            }
        }
        

    }
    /// <summary>
    /// Синтаксическое правило.
    /// </summary>
    public class Rule {
        public string LeftPart { get; set; }
        public List<string> RightPart { get; set; }

        public Rule(string left, List<string> right) {
            LeftPart = left;
            RightPart = right; 
        }
    }

    /// <summary>
    /// Узел дерева разбора.
    /// </summary>
    public class Node { 
        /// <summary>
        /// Название узла. Берется из левой части правила.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Токен, относящийся к данному узлу.
        /// </summary>
        public Token? Token { get; set; }

        /// <summary>
        /// Ветви от узла.
        /// </summary>
        public List<Node>? Branches { get; set; }
        public Node(string n, Token t) {
            Name = n;
            Token = t;
        }
        public Node(string n)
        {
            Name = n;
        }
        /// <summary>
        /// Добавить узел.
        /// </summary>
        public void AddBranch(Node n) {
            if (Branches is null) Branches = new();
            Branches.Add(n);
        }


        /// <summary>
        /// Является ли узел листом дерева.
        /// </summary>
        /// <returns>true - если является, иначе - false.</returns>
        public bool IsLeaf() {
            if (Branches is null) return true;
            else return false;
        }
    }
}
