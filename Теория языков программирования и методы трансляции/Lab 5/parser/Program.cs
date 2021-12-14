using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using MyTree;


namespace parser
{
    class Program
    {
        static void Main(string[] args)
        {
            using FileStream fStream = new("program.txt", FileMode.OpenOrCreate);
            byte[] arr = new byte[fStream.Length];
            fStream.Read(arr, 0, arr.Length);
            
            Parser parser = new(new List<Rule>() {
                new("expr", new List<string>(){
                    "for ( iden ; compareop ; iden ) do expr",
                    "statement"
                }),
                new("statement", new List<string>(){
                    "iden := strconst ;",
                    "iden := compareop ;",
                    "iden := iden ;",
                }),
                new("compareop", new List<string>(){
                    "iden < iden",
                    "iden > iden",
                    "iden = iden",
                }),
            });
            string programText = System.Text.Encoding.Default.GetString(arr);
            try{
                parser.Parse(programText);
                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine("Programm compiled succesfull!");
                Console.ResetColor();
                parser.ParseTree(programText);
            }
            catch(Exception e){
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }

        }    
    }

    public class Parser{
        private List<Rule> Rules;
        public Parser(List<Rule> rules){
            Rules = rules;
        }

        /// <summary>
        /// Синтаксический анализ.
        /// </summary>
        /// <param name="t">Таблица лексем.</param>
        
        public void Parse(string str){
            // стек
            string buf = "";
            for(int i = 0; i < str.Length; i++){
                buf += str[i];
                Console.WriteLine(buf);
                // ищем свертку до тех пор пока не подойдет ни одно правило
                bool isEnd = true;
                do{
                    isEnd = true;
                    // есть ли свертка для текущего положения
                    foreach(var r in Rules){
                        // проходимся по каждой правой части текущего правила
                        foreach(var rightPart in r.RightPart){
                            var pos = buf.IndexOf(rightPart);
                            if (pos != -1) {
                                buf = buf.Remove(pos, rightPart.Length);
                                buf = buf.Insert(pos, r.LeftPart);
                                isEnd = false;
                                Console.WriteLine(buf);
                            }
                        }
                    }
                }while(!isEnd);

                if (i == str.Length - 1){
                    if (buf != Rules[0].LeftPart){
                        // если стек не содержит целевой символ в конце обработки
                        throw new Exception("Stack doesn't contain target symbol. Stack: " 
                            + buf);
                    }
                }
            }
        }
        /// <summary>
        /// Синтаксический анализ.
        /// </summary>
        /// <param name="t">Таблица лексем.</param>
        public void ParseTree(string str){
            
            List<string> list = new();
            bool isNotEnd = true;
            while(isNotEnd){
                list = new();
                foreach(var r in Rules){
                    // проходимся по каждой правой части текущего правила
                    foreach(var rightPart in r.RightPart){
                        var buf = str;
                        var pos = buf.IndexOf(rightPart);                                              
                        
                        if (pos != -1) {
                            str = str.Remove(pos, rightPart.Length);
                            str = str.Insert(pos, r.LeftPart);
                        
                            Console.Write("(");                            
                            Console.Write(rightPart);
                            Console.Write(")");
                            list.Add(r.LeftPart);
                            //Console.Write(r.LeftPart + " ");
                            if (str == Rules[0].LeftPart) isNotEnd = false;
                        }
                    }
                }
                Console.WriteLine();
                foreach(var l in list){
                    Console.Write(l + "\t");
                }
                Console.WriteLine();
            }
            Console.Write("(");
            
            Console.Write(")");
        }
            
    }

    public class Rule{
        
        public string LeftPart {get; set;}
        /// <summary>
        /// Набор правил. Каждое правило - набор лексем в определенной последовательности.
        /// </summary>
        /// <value></value>
        public List<string> RightPart {get; set;}

        public Rule(string rightPart, List<string> rules){
            LeftPart = rightPart;
            RightPart = rules;
            // RightPart = new();
            // foreach(var r in rules){
            //     var arr = r.Split(' ');
            //     var l = new List<string>();
            //     for(int i = 0; i < arr.Length; i++){
            //         l.Add(arr[i]);
            //     }
            //     RightPart.Add(l);
            // }
        }


    }
}
