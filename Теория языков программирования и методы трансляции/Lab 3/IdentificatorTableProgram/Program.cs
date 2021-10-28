using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace IdentificatorTableProgram
{
    class Program
    {
        static void Main(string[] args)
        {       
            string textFromFile = "";     
            try{
                using FileStream fstream = new("idens.txt", FileMode.Open);
                byte[] array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                textFromFile = System.Text.Encoding.Default.GetString(array);
            }
            catch(Exception e){
                Console.WriteLine(e.Message);
                return;
            }
            
            string[] identificators = textFromFile.Split("\r\n");

            Rehashtable HTR = new();
            var binaryTree = new BinaryTree<string>();
            foreach(string i in identificators){
                HTR.Add(i);
                binaryTree.Add(i);
            }
            HTR.ToConsole();
            Console.WriteLine();
            Console.WriteLine(new string('-', 40));
            binaryTree.PrintTree();
        }

        /// <summary>
        /// Хеш-таблица с рехешированием
        /// </summary>
        class Rehashtable{
            
            private Dictionary<int, string> _table = null;
            private int _hash_min;
            private int _hash_max;
            public Rehashtable(){
                _table = new();

                byte[] asciiBytes = Encoding.ASCII.GetBytes("0");
                _hash_min = asciiBytes[0] * 3;
                asciiBytes = Encoding.ASCII.GetBytes("z");
                _hash_max = asciiBytes[0] * 3;

                for(int i = _hash_min; i <= _hash_max; i++){
                    _table.Add(i, null);
                }
            }

            /// <summary>
            /// Добавляет в таблицу запись.
            /// </summary>
            /// <param name="text">Запись для добавления в таблицу.</param>
            public void Add(string text){
                int hash = GetHash(text);
                // если элемент уже есть в таблице - выходим
                if (_table[hash] == text){
                    return;
                }     
                
                if (_table[hash] is null){
                   _table[hash] = text;
                }
                else {
                    // если по данному адресу уже лежит идентификатор - рехешируем
                    int iNum = 1;
                    bool isEndedLoop = true;
                    while (isEndedLoop){
                        int result = GetReHash(hash, iNum);
                        //
                        if (_table[result] == text){
                            return;
                        }     
                        // если ячейка по адресу, полученному из рехеширования, пустая - заносим туда идентификатор
                        if (_table[result] is null){
                            _table[result] = text;
                            isEndedLoop = false;
                        }
                        if (hash == result) {
                            isEndedLoop = false;
                            // throw exception "table is fill up"
                            throw new Exception("Table is fill up!");
                        }
                        iNum++;
                    }
                }
            }

            /// <summary>
            /// Получить информацию, об идентификаторе.
            /// </summary>
            /// <param name="text">Имя идентификатора.</param>
            /// <returns></returns>
            public string GetItem(string text){
                int hash = GetHash(text);

                //
                if (_table[hash] == null){
                    // thrwo exception, can't find element
                    throw new Exception("Can't find the element: " + text);
                }

                if (_table[hash] == text){
                    return _table[hash];
                }

                int iNum = 1;
                bool isEndedLoop = true;
                while (isEndedLoop){
                    int result = GetReHash(hash, iNum);
                    //
                    if (_table[hash] == null || hash == result){
                        // thrwo exception, can't find element
                        throw new Exception("Can't find the element: " + text);
                    }
                    if (_table[result] == text){
                        return _table[result];
                    }
                    iNum++;
                }

                return "";
            }

            /// <summary>
            /// Выводит таблицу идентификаторов на консоль.
            /// </summary>
            /// <param name="i">Кол-во идентификаторо в одной строке.</param>
            public void ToConsole(int i = 10){
                int iNum = 0;
                foreach(KeyValuePair<int, string> entry in _table)
                {
                    iNum++;
                    if (iNum % i != 0){
                        Console.Write(entry + "\t");
                    }
                    else{
                        Console.WriteLine(entry);
                    }
                }
            }

            /// <summary>
            /// Рехэширование.
            /// </summary>
            /// <param name="hash">Хеш, для которого нужно посчитать новый хеш.</param>
            /// <param name="iNum">Цикл итерации.</param>
            /// <returns>Новый хеш.</returns>
            private int GetReHash(int hash, int iNum){
                int rehash1 = 127;
                int rehash2 = 223;
                int result;
                result = (hash - _hash_min + iNum * rehash1 % rehash2) % (_hash_max - _hash_min + 1) + _hash_min;
                if (result < _hash_min) {
                    result = _hash_min;
                }

                return result;
            }

            /// <summary>
            /// Вычисляет хэш строки.
            /// </summary>
            /// <param name="text">Строка, для которой вычисляется хэш.</param>
            /// <returns>Хэш строки в виде строки.</returns>
            private int GetHash(string text){
                int hash = 0;
                byte[] asciiBytes = Encoding.ASCII.GetBytes(text);
                if (text.Length > 3){                    
                    byte code1, code2, code3;
                    code1 = asciiBytes[0];
                    code2 = asciiBytes[asciiBytes.Length / 2];
                    code3 = asciiBytes[asciiBytes.Length - 1];

                    hash = code1 + code2 + code3;
                }
                else {
                    hash = 3 * asciiBytes[0];
                }

                return hash;
            }

        }

        /// <summary>
        /// Расположения узла относительно родителя
        /// </summary>
        public enum Side
        {
            Left,
            Right
        }

        /// <summary>
        /// Узел бинарного дерева
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class BinaryTreeNode<T> where T : IComparable
        {
            /// <summary>
            /// Конструктор класса
            /// </summary>
            /// <param name="data">Данные</param>
            public BinaryTreeNode(T data)
            {
                Data = data;
            }

            /// <summary>
            /// Данные которые хранятся в узле
            /// </summary>
            public T Data { get; set; }

            /// <summary>
            /// Левая ветка
            /// </summary>
            public BinaryTreeNode<T> LeftNode { get; set; }

            /// <summary>
            /// Правая ветка
            /// </summary>
            public BinaryTreeNode<T> RightNode { get; set; }

            /// <summary>
            /// Родитель
            /// </summary>
            public BinaryTreeNode<T> ParentNode { get; set; }

            /// <summary>
            /// Расположение узла относительно его родителя
            /// </summary>
            public Side? NodeSide =>
                ParentNode == null
                ? (Side?)null
                : ParentNode.LeftNode == this
                    ? Side.Left
                    : Side.Right;

            /// <summary>
            /// Преобразование экземпляра класса в строку
            /// </summary>
            /// <returns>Данные узла дерева</returns>
            public override string ToString() => Data.ToString();
        }

        /// <summary>
        /// Бинарное дерево
        /// </summary>
        /// <typeparam name="T">Тип данных хранящихся в узлах</typeparam>
        public class BinaryTree<T> where T : IComparable
        {
            /// <summary>
            /// Корень бинарного дерева
            /// </summary>
            public BinaryTreeNode<T> RootNode { get; set; }

            /// <summary>
            /// Добавление нового узла в бинарное дерево
            /// </summary>
            /// <param name="node">Новый узел</param>
            /// <param name="currentNode">Текущий узел</param>
            /// <returns>Узел</returns>
            public BinaryTreeNode<T> Add(BinaryTreeNode<T> node, BinaryTreeNode<T> currentNode = null)
            {
                if (RootNode == null)
                {
                    node.ParentNode = null;
                    return RootNode = node;
                }

                currentNode = currentNode ?? RootNode;
                node.ParentNode = currentNode;
                int result;
                return (result = node.Data.CompareTo(currentNode.Data)) == 0
                    ? currentNode
                    : result < 0
                        ? currentNode.LeftNode == null
                            ? (currentNode.LeftNode = node)
                            : Add(node, currentNode.LeftNode)
                        : currentNode.RightNode == null
                            ? (currentNode.RightNode = node)
                            : Add(node, currentNode.RightNode);
            }

            /// <summary>
            /// Добавление данных в бинарное дерево
            /// </summary>
            /// <param name="data">Данные</param>
            /// <returns>Узел</returns>
            public BinaryTreeNode<T> Add(T data)
            {
                return Add(new BinaryTreeNode<T>(data));
            }

            /// <summary>
            /// Поиск узла по значению
            /// </summary>
            /// <param name="data">Искомое значение</param>
            /// <param name="startWithNode">Узел начала поиска</param>
            /// <returns>Найденный узел</returns>
            public BinaryTreeNode<T> FindNode(T data, BinaryTreeNode<T> startWithNode = null)
            {
                startWithNode = startWithNode ?? RootNode;
                int result;
                return (result = data.CompareTo(startWithNode.Data)) == 0
                    ? startWithNode
                    : result < 0
                        ? startWithNode.LeftNode == null
                            ? null
                            : FindNode(data, startWithNode.LeftNode)
                        : startWithNode.RightNode == null
                            ? null
                            : FindNode(data, startWithNode.RightNode);
            }

            /// <summary>
            /// Удаление узла бинарного дерева
            /// </summary>
            /// <param name="node">Узел для удаления</param>
            public void Remove(BinaryTreeNode<T> node)
            {
                if (node == null)
                {
                    return;
                }

                var currentNodeSide = node.NodeSide;
                //если у узла нет подузлов, можно его удалить
                if (node.LeftNode == null && node.RightNode == null)
                {
                    if (currentNodeSide == Side.Left)
                    {
                        node.ParentNode.LeftNode = null;
                    }
                    else
                    {
                        node.ParentNode.RightNode = null;
                    }
                }
                //если нет левого, то правый ставим на место удаляемого 
                else if (node.LeftNode == null)
                {
                    if (currentNodeSide == Side.Left)
                    {
                        node.ParentNode.LeftNode = node.RightNode;
                    }
                    else
                    {
                        node.ParentNode.RightNode = node.RightNode;
                    }

                    node.RightNode.ParentNode = node.ParentNode;
                }
                //если нет правого, то левый ставим на место удаляемого 
                else if (node.RightNode == null)
                {
                    if (currentNodeSide == Side.Left)
                    {
                        node.ParentNode.LeftNode = node.LeftNode;
                    }
                    else
                    {
                        node.ParentNode.RightNode = node.LeftNode;
                    }

                    node.LeftNode.ParentNode = node.ParentNode;
                }
                //если оба дочерних присутствуют, 
                //то правый становится на место удаляемого,
                //а левый вставляется в правый
                else
                {
                    switch (currentNodeSide)
                    {
                        case Side.Left:
                            node.ParentNode.LeftNode = node.RightNode;
                            node.RightNode.ParentNode = node.ParentNode;
                            Add(node.LeftNode, node.RightNode);
                            break;
                        case Side.Right:
                            node.ParentNode.RightNode = node.RightNode;
                            node.RightNode.ParentNode = node.ParentNode;
                            Add(node.LeftNode, node.RightNode);
                            break;
                        default:
                            var bufLeft = node.LeftNode;
                            var bufRightLeft = node.RightNode.LeftNode;
                            var bufRightRight = node.RightNode.RightNode;
                            node.Data = node.RightNode.Data;
                            node.RightNode = bufRightRight;
                            node.LeftNode = bufRightLeft;
                            Add(bufLeft, node);
                            break;
                    }
                }
            }


            /// <summary>
            /// Удаление узла дерева
            /// </summary>
            /// <param name="data">Данные для удаления</param>
            public void Remove(T data)
            {
                var foundNode = FindNode(data);
                Remove(foundNode);
            }


            /// <summary>
            /// Вывод бинарного дерева
            /// </summary>
            public void PrintTree()
            {
                PrintTree(RootNode);
            }

            /// <summary>
            /// Вывод бинарного дерева начиная с указанного узла
            /// </summary>
            /// <param name="startNode">Узел с которого начинается печать</param>
            /// <param name="indent">Отступ</param>
            /// <param name="side">Сторона</param>
            private void PrintTree(BinaryTreeNode<T> startNode, string indent = "", Side? side = null)
            {
                if (startNode != null)
                {
                    var nodeSide = side == null ? "+" : side == Side.Left ? "L" : "R";
                    Console.WriteLine($"{indent} [{nodeSide}]- {startNode.Data}");
                    indent += new string(' ', 3);
                    //рекурсивный вызов для левой и правой веток
                    PrintTree(startNode.LeftNode, indent, Side.Left);
                    PrintTree(startNode.RightNode, indent, Side.Right);
                }
            }
        }

    }
}
