using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace IdentificatorTableProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            Hashtable ht = new();

            ht.Add("1", "qwe");
            ht.Add("2", "asd");

            Console.WriteLine(ht["1"]);

            string sSourceData = "NotMySourceData";
            byte[] tmpSource = ASCIIEncoding.ASCII.GetBytes(sSourceData);
            byte[] tmpNewHash;
            tmpNewHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            Console.WriteLine(tmpNewHash);
        }
    }
}
