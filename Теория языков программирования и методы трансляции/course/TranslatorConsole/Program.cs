
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
    return;
}
Console.WriteLine(lexicalAnalyzer.GetTokenTableString());
Console.WriteLine(lexicalAnalyzer.GetIdenTableString());

var tokenTable = lexicalAnalyzer.GetTokenTable();
var identificatorTable = lexicalAnalyzer.GetIdentificators();

// синтаксический анализ
SyntaxAnalyzer syntaxAnalyzer = new(tokenTable, identificatorTable);
Node root;
try
{
    root = syntaxAnalyzer.Analyze();
}
catch (Exception ex) {
    Console.Error.WriteLine(ex.Message);
    return;
}
// трансляция из одного языка в другой
var translator = new Translator(root, identificatorTable);
var programm = translator.Translate();
Console.WriteLine(programm);
try
{
    using FileStream fileStream = new("Output.go", FileMode.Create);
    byte[] buf = new byte[fileStream.Length];
    buf = Encoding.UTF8.GetBytes(programm);
    fileStream.Write(buf, 0, buf.Length);
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex.Message);
}


//string? res = "";

//while (res != "q" || res != "quit") { 
//    Console.Write(" >> ");
//    var commands = Console.ReadLine().Split(' ');
//    res = commands[0];

//    for (int i = 0; i < commands.Length; i++) { 
//        if (commands[i].StartsWith("--")) {
//            if (i + 1 >= commands.Length) {
//                Console.Write("Command error. Try 'help'.");
//                break;
//            }
//            switch (commands[i+1]) {
//                case "":
//                    break;
//            }
//        }
//    }

//    switch (res) { 
//        case "q" or "quit":
//            Quit(commands);
//            break;
//        case "file":
//            SelectFile(commands);
//            break;
//        case "translate":
//            SelectFile(commands);
//            break;
//        case "tree":
//            SelectFile(commands);
//            break;
//        case "iden":
//            SelectFile(commands);
//            break;
//        case "help":
//            SelectFile(commands);
//            break;
//        default:
//            Console.WriteLine("Unrecognized command. Please enter 'help' to get instruction.");
//            break;
//    }
//}


//void Quit(string[] commands) { 
    
//}
//void SelectFile(string[] commands) {
//    if (!CheckCommands(commands, new() { "filename" })) return;

//    var path = GetCommand("filename", commands);
//    if (path == null) Console.WriteLine("Can't find 'filename' flag.");

//    fileName = path;
//} 
//void Translate(string[] commands) { 
    
//}
//void GetTree(string[] commands) { }
//void GetIdentificatorTable(string[] commands) { }
//void GetHelp(string[] commands) { }

//bool CheckCommands(string[] commands, List<string> necRules) {
//    bool res = true;
//    for (int i = 0; i < commands.Length; i++)
//    {
//        if (commands[i].StartsWith("--"))
//        {
//            if (i + 1 >= commands.Length)
//            {
//                Console.Write("Command error. Try 'help'.");
//                break;
//            }

//            if (!necRules.Contains(commands[i].Substring(2))) {
//                res = false;
//                Console.WriteLine("Command doesn't have " + commands[i]);
//            }

//        }
//    }
//    return res;
//}

//string? GetCommand(string comName, string[] commands) {
//    for (int i = 0; i < commands.Length; i++) {
//        if (commands[i].StartsWith("--"))
//        {
//            if (i + 1 >= commands.Length)
//            {
//                Console.Write("Command error. Try 'help'.");
//                break;
//            }

//            if (commands[i].Substring(2) == comName) return commands[i + 1];
//        }
//    }

//    return null;
//}