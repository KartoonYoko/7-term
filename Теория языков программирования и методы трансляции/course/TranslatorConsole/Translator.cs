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
                    ParseFunc(node);
                    break;
                case "ANNOUNCE":
                    ParseAnnounce(node);
                    break;
                case "ASSIGN":
                    ParseAssign(node);
                    break;
                case "IF_STATE":
                    ParseIf(node);
                    break;
                case "RETURN_STATE":
                    ParseReturn(node);
                    break;
                case "FUNC_USE":
                    ParseFunctionUse(node);
                    break;
                case "OPERATOR":
                    ParseOperator(node);
                    break;
                case "FOR_STATE":
                    ParseForState(node);
                    break;
                case "PRINT":
                    ParsePrint(node);
                    break;
                case "CONST_STRING":
                    ParseString(node);
                    break;
                default:
                    // при неподходящем имени разбираем подузлы данного узла
                    if (!node.IsLeaf())
                    {
                        foreach (var n in node.Branches)
                        {
                            WriteBranch(n);
                        }
                    }
                    break;
            }
        }

        private void ParseFunc(Node node) {
            var funcName = node.Branches[1].Token.Value;
            var returnDataType = node.Branches[0].Token.Value;
            var param = node.Branches[2];
            if (param.Name == "FUNC_ARG")
            {
                Write("func " + funcName + "("+
                    param.Branches[2].Token.Value + " " +
                    WriteDataType(param.Branches[1].Token.Value) +
                    ") " +
                    WriteDataType(returnDataType) + "{\n");
            }
            else
            {
                Write("func " + funcName + "() " + WriteDataType(returnDataType) + "{\n");
            }
            _tabs++;
            foreach (var n in node.Branches)
            {
                WriteBranch(n);
            }
            _tabs--;
            Write("}\n");            
        }
        private void ParseAssign(Node node) {
            var iden = node.Branches[0].Token.Value;
            var value = node.Branches[2].Token.Value;
            Write(iden + " = " + value + "\n");
        }
        private void ParseAnnounce(Node node) {
            var dataType = node.Branches[0].Token.Value;
            var iden = node.Branches[1].Token.Value;
            
            Write("var " + iden + " " + WriteDataType(dataType) + "\n");
        }
        private void ParseIf(Node node) {
            var compare = node.Branches[2];
            var statements = node.Branches[5];
            Write("if ");
            ParseCompare(compare);
            Write("{\n");
            _tabs++;
            WriteBranch(statements);
            _tabs--;
            Write("}\n");
        }
        private void ParseCompare(Node node) {
            var first = node.Branches[0].Token.Value;
            string? comp;
            string? second;
            if (node.Branches.Count == 3) {
                comp = node.Branches[1].Token.Value;
                second = node.Branches[2].Token.Value;                
            }
            else if (node.Branches.Count == 4) {
                comp = node.Branches[1].Token.Value +
                    node.Branches[2].Token.Value;
                second = node.Branches[3].Token.Value;
            }            
            else throw new Exception("Compare parse error. Wrong count branches. " + node.Branches.ToString());
            
            if (string.IsNullOrEmpty(comp) || string.IsNullOrEmpty(second))
            {
                throw new Exception("Compare parse error.");
            }
            Write(first + " " + comp + " " + second, false);
        }
        private void ParseReturn(Node node) {
            string value;
            if (node.Branches[1].IsLeaf())
            {
                value = node.Branches[1].Token.Value;
                Write("return " + value);
            }
            else {
                Write("return ");
                WriteBranch(node.Branches[1]);
            }
            Write("\n");
        }
        private void ParseFunctionUse(Node node) {
            var func = node.Branches[0].Token.Value;
            var param = node.Branches[2];
            Write(func + "(");
            if (param.IsLeaf()) Write(param.Token.Value, false);
            else WriteBranch(param);
            Write(")", false);
        }
        private void ParseOperator(Node node) {
            var first = node.Branches[0];
            var second = node.Branches[2];
            var sign = node.Branches[1].Token.Value;
            if (first.IsLeaf()) Write(first.Token.Value, false);
            else WriteBranch(first);
            Write(" " + sign + " ", false);
            if (second.IsLeaf()) Write(second.Token.Value, false);
            else WriteBranch(second);
        }
        private void ParseForState(Node node) {
            var assign = node.Branches[2];
            var assignBody = assign.Branches[0].Branches[0];
            var firstAssign = assignBody.Branches[0].Token.Value;
            var signAssign = assignBody.Branches[1].Token.Value;
            var secondAssign = assignBody.Branches[2].Token.Value;
            var compare = node.Branches[3];
            var unar = node.Branches[5];
            var body = node.Branches[8];
            Write("for " + firstAssign + " " + signAssign + " " +
                secondAssign + "; ");
            ParseCompare(compare);
            Write("; ");
            ParseUnar(unar);
            Write("{\n");
            WriteBranch(body);
            Write("}\n");
        }
        private void ParseUnar(Node node) {
            var f = node.Branches[0].Token.Value;
            var sign = node.Branches[1].Token.Value +
                node.Branches[2].Token.Value;
            Write(f + sign);
        }
        private void ParsePrint(Node node)
        {
            Write("fmt.Print(");
            string value = "";
            WriteBranch(node.Branches[6]);
            Write(value + ")\n");
            if (node.Branches.Count > 8)
            {
                Write("fmt.Print(");
                WriteBranch(node.Branches[9]);
                Write(value + ")\n");
            }
        }
        private void ParseString(Node node) {
            var value = node.Token.Value;
            Write(value, false);
        }

        /// <summary>
        /// Вернет тип данных для golang.
        /// </summary>
        /// <param name="dataType">Тип данных из c++.</param>
        private static string WriteDataType(string dataType) {
            return dataType switch
            {
                "int" => "int32",
                "long" => "int32",
                "float" => "int32",
                "double" => "int32",
                "void" => "",
                _ => throw new Exception("Unknown data type: " + dataType),
            };
        }
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
                    program += "  ";
                }
            }
            program += text;
        }
    }
}
