using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace IdentificatorTableProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            Rehashtable HTR = new();
            // Console.WriteLine(HTR.Hash("abcd"));
            // Console.WriteLine(HTR.Hash("WFADG"));
            // Console.WriteLine(HTR.Hash("ad"));
            // Console.WriteLine(HTR.Hash("aw"));
        }

        class Rehashtable{
            
            private Dictionary<int, string> _table = null;
            private int _hash_min;
            private int _hash_max;
            Rehashtable(){
                _table = new();

                byte[] asciiBytes = Encoding.ASCII.GetBytes("0");
                _hash_min = asciiBytes[0] * 3;
                asciiBytes = Encoding.ASCII.GetBytes("z");
                _hash_max = asciiBytes[0] * 3;
            }
            /// <summary>
            /// Добавляет в таблицу запись.
            /// </summary>
            /// <param name="text">Запись для добавления в таблицу.</param>
            public void Add(string text){
                int hash = GetHash(text);
                int rehash1 = 127;
                int rehash2 = 223;
                if (_table.ContainsKey(hash)){
                    int i = 1;
                    while (){
                        
                    }
                }
                else {
                    _table.Add(hash, text);
                }

            }
            /// <summary>
            /// Вычисляет хэш строки.
            /// </summary>
            /// <param name="text">Строка, для которой вычисляется хэш.</param>
            /// <returns>Хэш строки в виде строки.</returns>
            public int GetHash(string text){
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
    }
}
