
using System.IO;
using System.Text;

using TranslatorConsole;


Console.Write("Enter file name: ");
string? fileName;
string inputCode;
fileName = Console.ReadLine();
// считываем код из файла
if (fileName is null)
{
    Console.WriteLine("Empty file name.");
    return;
}

try {
    using FileStream fStream = new(fileName, FileMode.Open);
    byte[] buffer = new byte[fStream.Length];
    fStream.Read(buffer, 0, buffer.Length);
    inputCode = Encoding.UTF8.GetString(buffer);
}
catch (Exception ex) { 
    Console.WriteLine(ex.Message);
    return;
}

// лексический анализ
LexicalAnalyzer lexicalAnalyzer = new();
try
{
    lexicalAnalyzer.Parse(inputCode);
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex.Message);
}
var tokenTable = lexicalAnalyzer.GetTokenTable();
var identificatorTable = lexicalAnalyzer.GetIdentificators();

// синтаксический анализ
SyntaxAnalyzer syntaxAnalyzer = new(tokenTable, identificatorTable);
Node tree;
try
{
    tree = syntaxAnalyzer.Analyze();
}
catch (Exception ex) {
    Console.Error.WriteLine(ex.Message);
    return;
}
// трансляция из одного языка в другой
//var translator = new Translator(tokenTable, identificatorTable);
//Console.WriteLine(translator.Translate());







