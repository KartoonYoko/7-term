using System;
using System.Linq;
using System.Collections.Generic;


namespace parser
{
    class Program
    {
        static void Main(string[] args)
        {
            List<LexemItem> LexemTable = new(){ 
                new(){ Item = "for" },
                new(){ Item = "(" },
                new(){ Item = "iden" },
                new(){ Item = ")" },
                new(){ Item = "do" },
                new(){ Item = "iden" },
                new(){ Item = ";" },
            };

            Parser parser = new(new List<Rule>() {
                new("program", 
                    new List<string>(){
                        "expr",
                }),
                new("expr", 
                    new List<string>(){
                        "for ( iden ) do",
                        "iden"
                }),
            });

        }    
    }

    public class Parser{
        private ICollection<Rule> Rules;
        public Parser(ICollection<Rule> rules){
            Rules = rules;
        }

        /// <summary>
        /// Синтаксический анализ.
        /// </summary>
        /// <param name="t">Таблица лексем.</param>
        public void Parse(ICollection<LexemItem> t){
            string buf = "";
            foreach(var item in t){
                if (string.IsNullOrEmpty(buf)) buf = item.Item;
                else buf += " " + item.Item;

                foreach(var r in Rules){
                    foreach(var part in r.RightPart){
                        if (part == buf) {}
                    }
                }
            }
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

    public class LexemItem{
        public string Item {get; set;}
        public LexemaInfo Info {get; set;} = new();
    }
    public class LexemaInfo{
        
    }
}
